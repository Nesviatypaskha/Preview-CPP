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
    internal sealed class FieldNode : Node
    {

        public class AnalyzedField
        {
            public String TypeName { get; set; }
            public String VariableName { get; set; }
            public int ArraySize { get; set; }
            public bool IsPtr { get; set; }
        }
        public FieldNode(CXCursor cursor) : base(cursor)
        {
            var type = clang.getCursorType(cursor);
            var arrCount = clang.getArraySize(type);
            var field = new AnalyzedField();
            field.ArraySize = (int)arrCount;
            CXType vtype;
            if (arrCount == 0)
            {
                vtype = type;
            }
            else
            {
                vtype = clang.getArrayElementType(type);
            }
            while (vtype.kind == CXTypeKind.CXType_Typedef)
            {
                vtype = clang.getCanonicalType(vtype);
            }
            if (vtype.kind == CXTypeKind.CXType_Unexposed)
            {
                //                break;
            }
            if (vtype.kind == CXTypeKind.CXType_Pointer)
            {
                field.IsPtr = true;
                vtype = clang.getPointeeType(vtype);
                while (vtype.kind == CXTypeKind.CXType_Typedef)
                {
                    vtype = clang.getCanonicalType(vtype);
                }
            }
            else
            {
                field.IsPtr = false;
            }
            field.TypeName = clang.getTypeSpelling(vtype).ToString();
            field.VariableName = clang.getCursorSpelling(cursor).ToString();
            m_data_type = clang.getTypeSpelling(clang.getCursorType(cursor)).ToString();
        }
        public void addInTree()
        {

            if (this == null)
                return;
            CXCursor parent = clang.getCursorSemanticParent(m_cursor);
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
                         (parent_node.m_kind == CXCursorKind.CXCursor_StructDecl))
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
                SetComment(m_type).
                SetCommentHint(HINT.DATA_TYPE).
                SetUrlLine(m_line).
                SetUrlPosition(m_column).
                SetUrl(Parser.m_url).
                Send(NAME.PATTERN.VARIABLE, level, m_data_type + " " + (full ? m_full_name : m_name));
        }
    }
}