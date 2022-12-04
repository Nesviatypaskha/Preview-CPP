using ClangSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static resource.preview.CPP;

namespace resource.preview
{
    internal sealed class FunctionNode : Node
    {
        public static string GetCursorDeclared(CXCursor cursor)
        {
            var result = "";
            if (clang.CXXMethod_isConst(cursor) > 0) { result += "const "; };
            if (clang.CXXMethod_isPureVirtual(cursor) > 0) { result += "pure vitual "; };
            if (clang.CXXMethod_isStatic(cursor) > 0) { result += "static "; };
            if (clang.CXXMethod_isVirtual(cursor) > 0) { result += "virtual "; };

            return result;
        }
        public FunctionNode(CXCursor cursor) : base(cursor)
        {
            switch (m_kind)
            {
                case CXCursorKind.CXCursor_CXXMethod:
                    m_type = "method";
                    break;
                case CXCursorKind.CXCursor_FunctionTemplate:
                    m_type = "function template";
                    break;
                case CXCursorKind.CXCursor_Destructor:
                    m_type = "destructor";
                    break;
                case CXCursorKind.CXCursor_Constructor:
                    m_type = "constructor";
                    break;
                default:
                    m_type = "function";
                    break;
            }
            string return_type, name, full_name;
            GetFunctionPrototype(cursor, out return_type, out name, out full_name);
            m_data_type = return_type;
            m_name = name;
            m_full_name = full_name;
            if (Parser.s_Root.find(this))
            {
                m_kind = CXCursorKind.CXCursor_UnexposedDecl;
            }
        }

        public static void GetFunctionPrototype(CXCursor cursor, out string return_type, out string name, out string fullname)
        {
            CXType type = clang.getCursorType(cursor);
            string function_name = "|" + clang.getCursorSpelling(cursor).ToString();
            return_type = clang.getTypeSpelling(clang.getResultType(type)).ToString();
            name = return_type + " " + function_name + "(";
            int num_args = clang.Cursor_getNumArguments(cursor);
            for (uint i = 0; i < num_args; ++i)
            {
                var arg_cursor = clang.Cursor_getArgument(cursor, i);
                var arg_name = clang.getCursorSpelling(arg_cursor).ToString();
                if (String.IsNullOrEmpty(arg_name))
                {
                    arg_name = "no name!";
                }
                var arg_data_type = clang.getTypeSpelling(clang.getArgType(type, i)).ToString();
                name += arg_data_type + " " + arg_name + ",";
            }
            if (num_args > 0)
                name = name.Remove(name.Length - 1);
            name += ")";
            fullname = name.Replace("|", Utils.GetNamespace(cursor));
            name = name.Replace("|", "");
        }
        public void addInTree()
        {
            if (this == null)
                return;
            CXCursor parent = clang.getCursorSemanticParent(this.m_cursor);
            if (clang.getCursorKind(parent) == CXCursorKind.CXCursor_TranslationUnit)
            {
                Parser.s_Root.m_childrens.Add(this);
            }
            else
            {
                Node parent_node = Parser.s_Root.find(parent);
                if (parent_node == null)
                {
                    Analyze(parent).m_childrens.Add(this);
                }
                else if ((parent_node.m_kind == CXCursorKind.CXCursor_ClassDecl) ||
                         (parent_node.m_kind == CXCursorKind.CXCursor_StructDecl) ||
                         (parent_node.m_kind == CXCursorKind.CXCursor_Namespace))
                {
                    parent_node.m_childrens.Add(this);
                }
                else
                {
                    Parser.s_Root.m_childrens.Add(this);
                }
            }
        }
        public override void print(atom.Trace context, int level, bool full)
        {
            context.
                SetComment(m_type, HINT.DATA_TYPE).
                SetUrl(Parser.m_url, m_line, m_column).
                Send(atom.Trace.NAME.SOURCE.PREVIEW, atom.Trace.NAME.EVENT.FUNCTION, level, (full ? m_full_name : m_name));
        }
    }
}