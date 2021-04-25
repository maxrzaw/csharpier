using System.Linq;
using CSharpier.DocTypes;
using CSharpier.SyntaxPrinter;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpier
{
    public partial class Printer
    {
        private Doc PrintTypeParameterConstraintClauseSyntax(
            TypeParameterConstraintClauseSyntax node
        ) {
            return Docs.Group(
                this.PrintSyntaxToken(
                    node.WhereKeyword,
                    afterTokenIfNoTrailing: " "
                ),
                this.Print(node.Name),
                " ",
                this.PrintSyntaxToken(
                    node.ColonToken,
                    afterTokenIfNoTrailing: " "
                ),
                Docs.Indent(
                    SeparatedSyntaxList.Print(
                        node.Constraints,
                        this.Print,
                        Docs.Line
                    )
                )
            );
        }
    }
}
