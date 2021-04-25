using System;
using System.Collections.Generic;
using System.Linq;
using CSharpier.DocTypes;
using CSharpier.SyntaxPrinter;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpier
{
    public partial class Printer
    {
        // TODO partial - The three BaseX files in here can probably go in SyntaxNodePrinters. They are all abstract types
        // so I believe if we are generating the code correctly they will work fine.
        // The only weird one is this one, because it also needs to accept a LocalFunctionStatementSyntax
        // I think we can just add a LocalFunctionStatement.Print class/method that pass the node into BaseMethodDeclaration.Print
        // and this will continue to accept CSharpSyntaxNode
        private Doc PrintBaseMethodDeclarationSyntax(CSharpSyntaxNode node)
        {
            SyntaxList<AttributeListSyntax>? attributeLists = null;
            SyntaxTokenList? modifiers = null;
            TypeSyntax? returnType = null;
            ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier = null;
            TypeParameterListSyntax? typeParameterList = null;
            Doc identifier = Docs.Null;
            var constraintClauses = Enumerable.Empty<TypeParameterConstraintClauseSyntax>();
            ParameterListSyntax? parameterList = null;
            BlockSyntax? body = null;
            ArrowExpressionClauseSyntax? expressionBody = null;
            SyntaxToken? semicolonToken = null;
            string? groupId = null;

            if (
                node is BaseMethodDeclarationSyntax baseMethodDeclarationSyntax
            ) {
                attributeLists = baseMethodDeclarationSyntax.AttributeLists;
                modifiers = baseMethodDeclarationSyntax.Modifiers;
                parameterList = baseMethodDeclarationSyntax.ParameterList;
                body = baseMethodDeclarationSyntax.Body;
                expressionBody = baseMethodDeclarationSyntax.ExpressionBody;
                if (node is MethodDeclarationSyntax methodDeclarationSyntax)
                {
                    returnType = methodDeclarationSyntax.ReturnType;
                    explicitInterfaceSpecifier = methodDeclarationSyntax.ExplicitInterfaceSpecifier;
                    identifier = this.PrintSyntaxToken(
                        methodDeclarationSyntax.Identifier
                    );
                    typeParameterList = methodDeclarationSyntax.TypeParameterList;
                    constraintClauses = methodDeclarationSyntax.ConstraintClauses;
                }

                semicolonToken = baseMethodDeclarationSyntax.SemicolonToken;
            }
            else if (
                node is LocalFunctionStatementSyntax localFunctionStatementSyntax
            ) {
                attributeLists = localFunctionStatementSyntax.AttributeLists;
                modifiers = localFunctionStatementSyntax.Modifiers;
                returnType = localFunctionStatementSyntax.ReturnType;
                identifier = SyntaxTokens.Print(
                    localFunctionStatementSyntax.Identifier
                );
                typeParameterList = localFunctionStatementSyntax.TypeParameterList;
                parameterList = localFunctionStatementSyntax.ParameterList;
                constraintClauses = localFunctionStatementSyntax.ConstraintClauses;
                body = localFunctionStatementSyntax.Body;
                expressionBody = localFunctionStatementSyntax.ExpressionBody;
                semicolonToken = localFunctionStatementSyntax.SemicolonToken;
            }

            var docs = new List<Doc> { ExtraNewLines.Print(node) };

            if (attributeLists.HasValue)
            {
                docs.Add(this.PrintAttributeLists(node, attributeLists.Value));
            }
            if (modifiers.HasValue)
            {
                docs.Add(Modifiers.Print(modifiers.Value));
            }

            if (returnType != null)
            {
                // TODO 1 preprocessor stuff is going to be painful, because it doesn't parse some of it. Could we figure that out somehow? that may get complicated
                docs.Add(this.Print(returnType), " ");
            }

            if (explicitInterfaceSpecifier != null)
            {
                docs.Add(
                    this.Print(explicitInterfaceSpecifier.Name),
                    SyntaxTokens.Print(explicitInterfaceSpecifier.DotToken)
                );
            }

            if (identifier != Docs.Null)
            {
                docs.Add(identifier);
            }

            if (
                node is ConversionOperatorDeclarationSyntax conversionOperatorDeclarationSyntax
            ) {
                docs.Add(
                    this.PrintSyntaxToken(
                        conversionOperatorDeclarationSyntax.ImplicitOrExplicitKeyword,
                        " "
                    ),
                    this.PrintSyntaxToken(
                        conversionOperatorDeclarationSyntax.OperatorKeyword,
                        " "
                    ),
                    this.Print(conversionOperatorDeclarationSyntax.Type)
                );
            }
            else if (
                node is OperatorDeclarationSyntax operatorDeclarationSyntax
            ) {
                docs.Add(
                    this.Print(operatorDeclarationSyntax.ReturnType),
                    " ",
                    this.PrintSyntaxToken(
                        operatorDeclarationSyntax.OperatorKeyword,
                        " "
                    ),
                    this.PrintSyntaxToken(
                        operatorDeclarationSyntax.OperatorToken
                    )
                );
            }

            if (typeParameterList != null)
            {
                docs.Add(this.PrintTypeParameterListSyntax(typeParameterList));
            }

            if (parameterList != null)
            {
                // if there are no parameters, but there is a super long method name, a groupId
                // will cause SpaceBrace when it isn't wanted.
                if (parameterList.Parameters.Count > 0)
                {
                    groupId = Guid.NewGuid().ToString();
                }
                docs.Add(this.PrintParameterListSyntax(parameterList, groupId));
            }

            docs.Add(this.PrintConstraintClauses(constraintClauses));
            if (body != null)
            {
                docs.Add(
                    groupId != null
                        ? this.PrintBlockSyntaxWithConditionalSpace(
                                body,
                                groupId
                            )
                        : this.PrintBlockSyntax(body)
                );
            }
            else
            {
                if (expressionBody != null)
                {
                    docs.Add(
                        this.PrintArrowExpressionClauseSyntax(expressionBody)
                    );
                }
            }

            if (semicolonToken.HasValue)
            {
                docs.Add(SyntaxTokens.Print(semicolonToken.Value));
            }

            return Docs.Concat(docs);
        }
    }
}
