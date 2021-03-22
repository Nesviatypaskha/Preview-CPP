using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClangSharp;

namespace resource.preview
{
    internal static class Utils
    {
        public static Node.Specifier GetAccessSpecifier(CXCursor cursor)
        {
            Node.Specifier result;
            switch (clang.getCXXAccessSpecifier(cursor))
            {
                case CX_CXXAccessSpecifier.CX_CXXPublic:
                    result = Node.Specifier.PUBLIC;
                    break;
                case CX_CXXAccessSpecifier.CX_CXXProtected:
                    result = Node.Specifier.PROTECTED;
                    break;
                case CX_CXXAccessSpecifier.CX_CXXPrivate:
                    result = Node.Specifier.PRIVATE;
                    break;
                default:
                    result = Node.Specifier.NONE;
                    break;
            }
            return result;
        }
        public static string GetNamespace(CXCursor cursor)
        {
            string result = "";
            if (clang.getCursorKind(cursor) != CXCursorKind.CXCursor_TranslationUnit)
            {
                var cursor_parent = clang.getCursorSemanticParent(cursor);
                while (clang.getCursorKind(cursor_parent) != CXCursorKind.CXCursor_TranslationUnit)
                {
                    result = clang.getCursorSpelling(cursor_parent).ToString() + "::" + result;
                    cursor_parent = clang.getCursorSemanticParent(cursor_parent);
                }
            }
            return result;
        }
        public static int GetLinesCount(string url)
        {
            var lines = 0;
            foreach (var line in File.ReadAllLines(url))
            {
                lines++;
            }
            return lines;
        }
        public static string ToPlainTypeString(this CXType type, string unknownType = "UnknownType")
        {
            var canonical = clang.getCanonicalType(type);
            switch (type.kind)
            {
                case CXTypeKind.CXType_Bool:
                    return "bool";
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_Char_U:
                    return "char";
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_Char_S:
                    return "sbyte";
                case CXTypeKind.CXType_UShort:
                    return "ushort";
                case CXTypeKind.CXType_Short:
                    return "short";
                case CXTypeKind.CXType_Float:
                    return "float";
                case CXTypeKind.CXType_Double:
                    return "double";
                case CXTypeKind.CXType_Int:
                    return "int";
                case CXTypeKind.CXType_UInt:
                    return "uint";
                case CXTypeKind.CXType_Pointer:
                    return "pointer";
                case CXTypeKind.CXType_NullPtr:
                    return "null_ptr";
                case CXTypeKind.CXType_Long:
                    return "int";
                case CXTypeKind.CXType_ULong:
                    return "int";
                case CXTypeKind.CXType_LongLong:
                    return "long";
                case CXTypeKind.CXType_ULongLong:
                    return "ulong";
                case CXTypeKind.CXType_Void:
                    return "void";
                case CXTypeKind.CXType_Unexposed:
                    if (canonical.kind == CXTypeKind.CXType_Unexposed)
                    {
                        return clang.getTypeSpelling(canonical).ToString();
                    }
                    return canonical.ToPlainTypeString();
                default:
                    return unknownType;
            }
        }
    }
}
