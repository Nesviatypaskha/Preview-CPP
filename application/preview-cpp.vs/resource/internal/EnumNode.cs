using ClangSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static cartridge.AnyPreview;
using static resource.preview.CPP;

namespace resource.preview
{
    internal sealed class EnumNode : Node
    {
        private class CONSTANT
        {
            public const string ENUM = "enum";
            public const string ENUM_CONST = "enum constant";
        }
        public EnumNode(CXCursor cursor) : base(cursor)
        {
            if (Parser.s_Root.find(this))
            {
                this.m_kind = CXCursorKind.CXCursor_UnexposedDecl;
            }
            else
            {
                switch (m_kind)
                {
                    case CXCursorKind.CXCursor_EnumDecl:
                        m_type = CONSTANT.ENUM;
                        m_data_type = "enum";
                        break;
                    case CXCursorKind.CXCursor_EnumConstantDecl:
                        m_type = CONSTANT.ENUM_CONST;
                        m_data_type = "field";
                        break;
                    default:
                        break;
                }
                m_full_name = Utils.GetNamespace(cursor) + m_name;
            }
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
                else if (parent_node.m_kind == CXCursorKind.CXCursor_EnumDecl)
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
                SetComment("enum").
                SetCommentHint(HINT.DATA_TYPE).
                SetUrlLine(m_line).
                SetUrlPosition(m_column).
                SetUrl(Parser.m_url).
                Send(NAME.PATTERN.CLASS, level, (full ? m_full_name : m_name)); 
            printChildrens(context, CXCursorKind.CXCursor_EnumConstantDecl, level + 1, false);
        }
    }
}
