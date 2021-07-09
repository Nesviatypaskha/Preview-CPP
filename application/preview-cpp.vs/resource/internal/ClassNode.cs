using ClangSharp;
//using resource.file_helpers;
using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static cartridge.AnyPreview;
//using static resource.preview.CPP;
//
using System.Runtime.Remoting.Activation;
using System.Runtime.CompilerServices;
using static resource.preview.CPP;

namespace resource.preview
{
    internal sealed class ClassNode : Node
    {
        private class CONSTANT
        {
            public const string CLASS = "class";
            public const string STRUCT = "struct";
            public const string NAMESPACE = "namespace";
        }
        public ClassNode(CXCursor cursor) : base(cursor)
        {
            if (Parser.s_Root.find(this))
            {
                this.m_kind = CXCursorKind.CXCursor_UnexposedDecl;
            }
            else
            {
                switch (m_kind)
                {
                    case CXCursorKind.CXCursor_ClassDecl:
                        m_type = CONSTANT.CLASS;
                        break;
                    case CXCursorKind.CXCursor_StructDecl:
                        m_type = CONSTANT.STRUCT;
                        break;
                    case CXCursorKind.CXCursor_Namespace:
                        m_type = CONSTANT.NAMESPACE;
                        break;
                    default:
                        break;
                }
                m_data_type = m_type;
                m_full_name = Utils.GetNamespace(cursor) + m_name;
            }
        }
        public override void print(atom.Trace context, int level, bool full)
        {
            context.
                SetComment("class", HINT.DATA_TYPE).
                SetUrl(Parser.m_url, m_line, m_column).
                Send(atom.Trace.NAME.SOURCE.PREVIEW, atom.Trace.NAME.TYPE.CLASS, level, (full ? m_full_name : m_name));
            printChildrens(context, CXCursorKind.CXCursor_FieldDecl, level + 1, false);
            printChildrens(context, CXCursorKind.CXCursor_Constructor, level + 1, false);
            printChildrens(context, CXCursorKind.CXCursor_Destructor, level + 1, false);
            printChildrens(context, CXCursorKind.CXCursor_CXXMethod, level + 1, false);
            printChildrens(context, CXCursorKind.CXCursor_FunctionDecl, level + 1, false);
        }
    }
}
