
using ClangSharp;
using System.IO;

namespace resource.preview
{
    public class CPP : extension.AnyPreview
    {
        internal class HINT
        {
            public static string DATA_TYPE = "[[[Data type]]]";
            public static string METHOD_TYPE = "[[[Method type]]]";
        }

        protected override void _Execute(atom.Trace context, int level, string url, string file)
        {
            {
                context.Send(atom.Trace.NAME.SOURCE.PREVIEW, atom.Trace.NAME.EVENT.FOLDER, 1, "[[[Info]]]");
                {
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[File Name]]]", url);
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[File Size]]]", File.ReadAllText(file).Length.ToString());
                    context.Send(NAME.SOURCE.PREVIEW, NAME.EVENT.PARAMETER, level + 1, "[[[Language]]]", "C++");
                }
            }
            {
                var Parser = new Parser(file, context);
                {
                    Parser.Parse();
                }
                if (Parser.s_Root.findType(CXCursorKind.CXCursor_ClassDecl))
                {
                    context.Send(atom.Trace.NAME.SOURCE.PREVIEW, atom.Trace.NAME.EVENT.FOLDER, 1, "[[[Classes]]]");
                    Parser.s_Root.printChildrens(context, CXCursorKind.CXCursor_ClassDecl, 2, true);
                }

                if (Parser.s_Root.findType(CXCursorKind.CXCursor_StructDecl))
                {
                    context.Send(atom.Trace.NAME.SOURCE.PREVIEW, atom.Trace.NAME.EVENT.FOLDER, 1, "[[[Structs]]]");
                    Parser.s_Root.printChildrens(context, CXCursorKind.CXCursor_StructDecl, 2, true);
                }

                if (Parser.s_Root.findType(CXCursorKind.CXCursor_EnumDecl))
                {
                    context.Send(atom.Trace.NAME.SOURCE.PREVIEW, atom.Trace.NAME.EVENT.FOLDER, 1, "[[[Enums]]]");
                    Parser.s_Root.printChildrens(context, CXCursorKind.CXCursor_EnumDecl, 2, true);
                }

                if (Parser.s_Root.findType(CXCursorKind.CXCursor_FunctionDecl))
                {
                    context.Send(atom.Trace.NAME.SOURCE.PREVIEW, atom.Trace.NAME.EVENT.FOLDER, 1, "[[[Functions]]]");
                    Parser.s_Root.printChildrens(context, CXCursorKind.CXCursor_FunctionDecl, 2, true);
                    Parser.s_Root.printChildrens(context, CXCursorKind.CXCursor_CXXMethod, 2, true);
                    Parser.s_Root.printChildrens(context, CXCursorKind.CXCursor_FunctionTemplate, 2, true);
                }
            }
        }
    };
}
