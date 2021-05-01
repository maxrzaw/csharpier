using CSharpier.DocTypes;
using CSharpier.SyntaxPrinter;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpier
{
    public partial class Printer
    {
        private Doc PrintLocalDeclarationStatementSyntax(
            LocalDeclarationStatementSyntax node
        ) {
            return Doc.Concat(
                ExtraNewLines.Print(node),
                this.PrintSyntaxToken(
                    node.AwaitKeyword,
                    afterTokenIfNoTrailing: " "
                ),
                this.PrintSyntaxToken(
                    node.UsingKeyword,
                    afterTokenIfNoTrailing: " "
                ),
                Modifiers.Print(node.Modifiers),
                this.PrintVariableDeclarationSyntax(node.Declaration),
                Token.Print(node.SemicolonToken)
            );
        }
    }
}
