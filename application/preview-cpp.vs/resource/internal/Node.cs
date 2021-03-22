using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ClangSharp;

namespace resource.preview
{
    class Node : INode
    {
        public enum Specifier
        {
            PUBLIC,
            PROTECTED,
            PRIVATE,
            NONE
        }
        public CXCursor m_cursor { get; set; } // cursor
        public CXCursorKind m_kind { get; set; } // kind
        public string m_type { get; set; } // type
        public string m_data_type { get; set; } // Data type
        public string m_name { get; set; } // short name
        public string m_full_name { get; set; } // full name
        public int m_line { get; set; }
        public int m_column { get; set; }
        public Specifier m_specifier { get; set; }

        public List<Node> m_childrens { get; set; }
        public Node()
        {
            m_kind = CXCursorKind.CXCursor_UnexposedDecl;
            m_type = "";
            m_name = "CPP_Preview_ROOT";
            m_line = 0;
            m_column = 0;
            m_specifier = Specifier.NONE;
            m_childrens = new List<Node>();
        }
        public Node(CXCursor cursor)
        {
            CXFile file;
            uint line;
            uint column;
            uint offset;
            CXSourceLocation location = clang.getCursorLocation(cursor);
            clang.getSpellingLocation(location, out file, out line, out column, out offset);
            m_cursor = cursor;
            m_kind = clang.getCursorKind(cursor);
            m_type = clang.getCursorKindSpelling(m_kind).ToString();
            m_name = clang.getCursorSpelling(cursor).ToString();
            m_full_name = Utils.GetNamespace(cursor) + m_name;
            m_line = (int)line;
            m_column = (int)column;
            m_specifier = Utils.GetAccessSpecifier(cursor);
            m_childrens = new List<Node>();
        }
        public Node(CXCursor cursor, string type, string data_type, string name, string full_name) : this(cursor)
        {
            m_type = type;
            m_data_type = data_type;
            m_name = name;
            m_full_name = full_name;
        }

        public Node(CXCursor cursor, CXCursorKind type, Specifier specifier, string name, string full_name, string value, int line, int column)
        {
            m_cursor = cursor;
            m_kind = type;
            m_name = name;
            m_full_name = full_name;
            m_line = line;
            m_column = column;
            m_specifier = specifier;
            m_childrens = new List<Node>();
        }
        public Node find(CXCursor cursor)
        {
            if (this == null)
                return null;
            if (clang.equalCursors(this.m_cursor, cursor) > 0)
                return this;
            foreach (var child in this.m_childrens)
            {
                if (clang.equalCursors(child.m_cursor, cursor) > 0)
                {
                    return child;
                }
                child.find(cursor);
            }
            return null;
        }
        public bool find(Node node)
        {
            if (this == null)
                return false;
            if (this.isEqual(node))
                return true;
            foreach (var child in this.m_childrens)
            {
                if (child.find(node))
                {
                    return true;
                }
            }
            return false;
        }
        public bool findType(CXCursorKind kind)
        {
            if (this == null)
                return false;
            if (m_kind == kind)
                return true;
            foreach (var child in this.m_childrens)
            {
                if (child.m_kind == kind)
                {
                    return true;
                }
                child.findType(kind);
            }
            return false;
        }
        public void printChildrens(atom.Trace context, CXCursorKind kind, int level, bool full)
        {
            if (m_childrens.Count > 0)
            {
                foreach (var node in m_childrens)
                {
                    if (node.m_kind == kind)
                    {
                        node.print(context, level, full);
                    }
                    node.printChildrens(context, kind, level, full);
                }
            }
        }
        public virtual void print(atom.Trace context, int level, bool full)
        {
            Console.WriteLine("Need to implement");
        }

        public static Node Analyze(CXCursor cursor)
        {
            var result = new Node(cursor);
            CXCursorKind kind = clang.getCursorKind(cursor);
            if ((kind == CXCursorKind.CXCursor_ClassDecl) || (kind == CXCursorKind.CXCursor_StructDecl))
            {
                result = new ClassNode(cursor);
                if (result.m_kind != CXCursorKind.CXCursor_UnexposedDecl)
                    Parser.s_Root.m_childrens.Add(result);
            }
            else if ((kind == CXCursorKind.CXCursor_EnumDecl) || (kind == CXCursorKind.CXCursor_EnumConstantDecl))
            {
                var node = new EnumNode(cursor);
                node.addInTree();
            }
            else if ((kind == CXCursorKind.CXCursor_FunctionDecl) ||
                    (kind == CXCursorKind.CXCursor_CXXMethod) ||
                    //(kind == CXCursorKind.CXCursor_FunctionTemplate) ||
                    (kind == CXCursorKind.CXCursor_Constructor) ||
                    (kind == CXCursorKind.CXCursor_Destructor))
            {
                var node = new FunctionNode(cursor);
                if (node.m_kind != CXCursorKind.CXCursor_UnexposedDecl)
                    node.addInTree();
            }
            if (kind == CXCursorKind.CXCursor_FieldDecl)
            {
                var node = new FieldNode(cursor);
                node.addInTree();
            }

            return result;
        }
        public bool isEqual(Node node)
        {
            if ((this.m_kind == node.m_kind) &&
                    (this.m_type == node.m_type) &&
                    (this.m_data_type == node.m_data_type) &&
                    (this.m_specifier == node.m_specifier) &&
                    (this.m_full_name == node.m_full_name))
            {
                return true;
            }
            return false;
        }
    }
}
