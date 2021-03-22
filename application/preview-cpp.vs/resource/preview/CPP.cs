
using ClangSharp;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;

namespace resource.preview
{
    public class CPP : cartridge.AnyPreview
    {
        internal class HINT
        {
            public static string DATA_TYPE = "[[Data type]]";
            public static string METHOD_TYPE = "[[Method type]]";
        }

        protected override void _Execute(atom.Trace context, string url)
        {
            var a_Context = CSharpSyntaxTree.ParseText(File.ReadAllText(url)).WithFilePath(url).GetRoot();
            {
                context.
                    SetState(NAME.STATE.EXPAND).
                    Send(NAME.PATTERN.FOLDER, 1, "[[Info]]");
                {
                    context.
                        SetValue(url).
                        Send(NAME.PATTERN.VARIABLE, 2, "[[File name]]");
                    context.
                        SetValue(Utils.GetLinesCount(url).ToString()).
                        Send(NAME.PATTERN.VARIABLE, 2, "[[File size]]");
                }
            }

            var Parser = new Parser(url, context);
            Parser.Parse();

            if (Parser.s_Root.findType(CXCursorKind.CXCursor_ClassDecl))
            {
                context.Send(NAME.PATTERN.FOLDER, 1, "[[Classes]]");
                Parser.s_Root.printChildrens(context, CXCursorKind.CXCursor_ClassDecl, 2, true);
            }

            if (Parser.s_Root.findType(CXCursorKind.CXCursor_StructDecl))
            {
                context.Send(NAME.PATTERN.FOLDER, 1, "[[Structs]]");
                Parser.s_Root.printChildrens(context, CXCursorKind.CXCursor_StructDecl, 2, true);
            }

            if (Parser.s_Root.findType(CXCursorKind.CXCursor_EnumDecl))
            {
                context.Send(NAME.PATTERN.FOLDER, 1, "[[Enums]]");
                Parser.s_Root.printChildrens(context, CXCursorKind.CXCursor_EnumDecl, 2, true);
            }

            if (Parser.s_Root.findType(CXCursorKind.CXCursor_FunctionDecl))
            {
                context.Send(NAME.PATTERN.FOLDER, 1, "[[Functions]]");
                Parser.s_Root.printChildrens(context, CXCursorKind.CXCursor_FunctionDecl, 2, true);
                Parser.s_Root.printChildrens(context, CXCursorKind.CXCursor_CXXMethod, 2, true);
                Parser.s_Root.printChildrens(context, CXCursorKind.CXCursor_FunctionTemplate, 2, true);
            }
        }
    };
}
