Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace Metagenomics

    ''' <summary>
    ''' Parser and stringfier of <see cref="Taxonomy"/> object.
    ''' </summary>
    Public Class BIOMTaxonomyParser : Implements IParser

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj">
        ''' Object value should be in data type <see cref="Taxonomy"/>
        ''' </param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function ToString(obj As Object) As String Implements IParser.ToString
            Return DirectCast(obj, Taxonomy).ToString(BIOMstyle:=True)
        End Function

        ''' <summary>
        ''' Create a <see cref="Taxonomy"/> object from parse taxonomy string
        ''' </summary>
        ''' <param name="content"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TryParse(content As String) As Object Implements IParser.TryParse
            Return Parse(biomString:=content)
        End Function

        ''' <summary>
        ''' parse auto
        ''' </summary>
        ''' <param name="biomString"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Parse(biomString As String) As Taxonomy
            If biomString.StartsWith("superkingdom__") Then
                Return BIOMTaxonomy.TaxonomyParserAlt(biomString).AsTaxonomy
            ElseIf biomString.StartsWith("k__") Then
                Return BIOMTaxonomy.TaxonomyParser(biomString).AsTaxonomy
            Else
                Return BIOMTaxonomy.TaxonomyFromString(biomString)
            End If
        End Function
    End Class
End Namespace