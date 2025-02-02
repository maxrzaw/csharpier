using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SyntaxFinder;

public static class SyntaxTriviaExtensions
{
    public static bool IsComment(this SyntaxTrivia syntaxTrivia)
    {
        return syntaxTrivia.Kind()
            is SyntaxKind.SingleLineCommentTrivia
                or SyntaxKind.MultiLineCommentTrivia
                or SyntaxKind.SingleLineDocumentationCommentTrivia
                or SyntaxKind.MultiLineDocumentationCommentTrivia;
    }
}
