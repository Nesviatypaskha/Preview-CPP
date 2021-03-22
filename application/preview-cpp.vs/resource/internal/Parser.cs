using ClangSharp;
using Microsoft.VisualStudio.Shell.Interop;
using resource.preview;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static cartridge.AnyPreview;
using static resource.preview.CPP;

namespace resource.preview
{
    class Parser
    {
        public static string m_url;
        public static atom.Trace m_Context;
        public static preview.Node s_Root;// = new preview.Node();
        public Parser(string url, atom.Trace context)
        {
            m_url = url;
            m_Context = context;
        }
        public void Parse()
        {
            s_Root = new Node();
            CXIndex index = clang.createIndex(0, 0);
            string[] arr = {"-std=c++17", "-ast-dump" };
            // -ast-dump
            // OnlyLocalDecls 
            CXUnsavedFile unsavedFile;
            CXTranslationUnit translationUnit;
            // TODO: grab errors and warnings
            //var translationUnitError = clang.parseTranslationUnit2(index, "C:\\Users\\Alex\\source\\repos\\clang_test\\clang_test\\test.cpp", arr, 2, out unsavedFile, 0, 0, out translationUnit);
            var translationUnitError = clang.parseTranslationUnit2(index, m_url, arr, 2, out unsavedFile, 0, 0, out translationUnit);

            if (translationUnitError != CXErrorCode.CXError_Success)
            {
                Console.WriteLine("Error: " + translationUnitError);
                var numDiagnostics = clang.getNumDiagnostics(translationUnit);

                for (uint i = 0; i < numDiagnostics; ++i)
                {
                    var diagnostic = clang.getDiagnostic(translationUnit, i);
                    Console.WriteLine(clang.getDiagnosticSpelling(diagnostic).ToString());
                    clang.disposeDiagnostic(diagnostic);
                }
            }
            else
            {
                traverse(translationUnit);
                clang.disposeTranslationUnit(translationUnit);
            }
            clang.disposeIndex(index);
        }
        private void traverse(CXTranslationUnit tu)
        {
            CXCursor root = clang.getTranslationUnitCursor(tu);

            CXCursorKind kind = clang.getCursorKind(root);
            Console.WriteLine((clang.getCursorKindSpelling(kind)).ToString());

            clang.visitChildren(root, Visit, new CXClientData(new IntPtr()));
        }
        public static CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data)
        {
            if (clang.Location_isFromMainFile(clang.getCursorLocation(cursor)) == 0)
                return CXChildVisitResult.CXChildVisit_Continue;

            CXSourceLocation location = clang.getCursorLocation(cursor);
            if (clang.Location_isInSystemHeader(location) != 0)
            {
                return CXChildVisitResult.CXChildVisit_Continue;
            }

            CXCursorKind kind = clang.getCursorKind(cursor);
            if (clang.isDeclaration(kind) > 0)
            {
                Node.Analyze(cursor);
                clang.visitChildren(cursor, Visit, new CXClientData(new IntPtr()));
            }
            return CXChildVisitResult.CXChildVisit_Continue;
        }
    }
}
