namespace CSharpier.SyntaxPrinter.SyntaxNodePrinters;

internal static class Argument
{
    public static Doc Print(ArgumentSyntax node, PrintingContext context)
    {
        return Doc.Concat(PrintModifiers(node, context), Node.Print(node.Expression, context));
    }

    public static Doc PrintModifiers(ArgumentSyntax node, PrintingContext context)
    {
        var docs = new List<Doc>(2);
        if (node.NameColon != null)
        {
            docs.Add(BaseExpressionColon.Print(node.NameColon, context));
        }

        if (node.RefKindKeyword.RawSyntaxKind() != SyntaxKind.None)
        {
            docs.Add(Token.PrintWithSuffix(node.RefKindKeyword, " ", context));
        }

        return Doc.Concat(docs);
    }
}
