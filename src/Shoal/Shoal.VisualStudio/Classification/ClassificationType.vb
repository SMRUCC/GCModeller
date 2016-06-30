Imports System.ComponentModel.Composition
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens
Imports Microsoft.VisualStudio.Text.Classification
Imports Microsoft.VisualStudio.Utilities

Namespace OokLanguage

    Module OrdinaryClassificationDefinition

        ''' <summary>
        ''' Defines the "ordinary" classification type.
        ''' </summary>
        <Export(GetType(ClassificationTypeDefinition)), Name(NameOf(TokenTypes.CollectionElement))>
        Public CollectionElement As ClassificationTypeDefinition = Nothing

        ''' <summary>
        ''' Defines the "ordinary" classification type.
        ''' </summary>
        <Export(GetType(ClassificationTypeDefinition)), Name(NameOf(TokenTypes.EntryPoint))>
        Public EntryPoint As ClassificationTypeDefinition = Nothing

        ''' <summary>
        ''' Defines the "ordinary" classification type.
        ''' </summary>
        <Export(GetType(ClassificationTypeDefinition)), Name(NameOf(TokenTypes.InternalExpression))>
        Public InternalExpression As ClassificationTypeDefinition = Nothing

        <Export(GetType(ClassificationTypeDefinition)), Name(NameOf(TokenTypes.LeftAssignedVariable))>
        Public LeftAssignedVariable As ClassificationTypeDefinition = Nothing

        <Export(GetType(ClassificationTypeDefinition)), Name(NameOf(TokenTypes.Operator))>
        Public [Operator] As ClassificationTypeDefinition = Nothing

        <Export(GetType(ClassificationTypeDefinition)), Name(NameOf(TokenTypes.ParameterName))>
        Public ParameterName As ClassificationTypeDefinition = Nothing
    End Module
End Namespace
