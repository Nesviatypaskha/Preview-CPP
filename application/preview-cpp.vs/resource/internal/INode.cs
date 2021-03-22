using ClangSharp;
namespace resource.preview
{
    internal interface INode
    {
        void printChildrens(atom.Trace context, CXCursorKind kind, int level, bool full);
        void print(atom.Trace context, int level, bool full);
    }
}
