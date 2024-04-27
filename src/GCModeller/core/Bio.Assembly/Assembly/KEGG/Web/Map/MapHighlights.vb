#Region "Microsoft.VisualBasic::fc31e5181a77bacaf4c88d439e333117, G:/GCModeller/src/GCModeller/core/Bio.Assembly//Assembly/KEGG/Web/Map/MapHighlights.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 94
    '    Code Lines: 59
    ' Comment Lines: 20
    '   Blank Lines: 15
    '     File Size: 4.21 KB


    '     Class MapHighlights
    ' 
    '         Properties: compounds, genes, proteins
    ' 
    '         Function: CheckTextColorWarning, CreateAuto, CreateCompounds, CreateGenes, CreateProteins
    '                   GetGeneProteinTuples, PopulateAllHighlights
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' mix rendering
    ''' </summary>
    Public Class MapHighlights

        Public Property compounds As NamedValue(Of String)() = {}
        Public Property genes As NamedValue(Of String)() = {}
        Public Property proteins As NamedValue(Of String)() = {}

        ''' <summary>
        ''' warning about the elements which has the plot color that the same as the text color,
        ''' 
        ''' example as: text color is white, and the gene color is white, so that we will could not
        ''' read the text label, due to the reason of the color of these two element are the same.
        ''' </summary>
        ''' <param name="highlights"></param>
        ''' <param name="text_color"></param>
        ''' <returns></returns>
        Public Shared Iterator Function CheckTextColorWarning(highlights As IEnumerable(Of NamedValue(Of String)), text_color As Color) As IEnumerable(Of String)
            For Each item As NamedValue(Of String) In highlights.SafeQuery
                If item.Value.TranslateColor(throwEx:=False).Equals(text_color, tolerance:=6) Then
                    Yield item.Name
                End If
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function PopulateAllHighlights() As IEnumerable(Of NamedValue(Of String))
            Return compounds.JoinIterates(genes).JoinIterates(proteins)
        End Function

        Public Iterator Function GetGeneProteinTuples() As IEnumerable(Of NamedValue(Of (gene_color$, protein_color$)))
            Dim geneSet = genes.GroupBy(Function(a) a.Name).ToDictionary(Function(a) a.Key, Function(a) a.First.Value)
            Dim protSet = proteins.GroupBy(Function(a) a.Name).ToDictionary(Function(a) a.Key, Function(a) a.First.Value)
            Dim unionIdSet As String() = geneSet.Keys.JoinIterates(protSet.Keys).Distinct.ToArray

            For Each id As String In unionIdSet
                If geneSet.ContainsKey(id) AndAlso protSet.ContainsKey(id) Then
                    Yield New NamedValue(Of (gene_color As String, protein_color As String))(id, (geneSet(id), protSet(id)))
                End If
            Next
        End Function

        Public Shared Function CreateCompounds(list As IEnumerable(Of NamedValue(Of String))) As MapHighlights
            Return New MapHighlights With {.compounds = list.ToArray}
        End Function

        Public Shared Function CreateGenes(list As IEnumerable(Of NamedValue(Of String))) As MapHighlights
            Return New MapHighlights With {.genes = list.ToArray}
        End Function

        Public Shared Function CreateProteins(list As IEnumerable(Of NamedValue(Of String))) As MapHighlights
            Return New MapHighlights With {.proteins = list.ToArray}
        End Function

        ''' <summary>
        ''' check highlights automatically via kegg id prefix
        ''' </summary>
        ''' <param name="list"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' can not determine the gene/proteins at here
        ''' </remarks>
        Public Shared Function CreateAuto(list As IEnumerable(Of NamedValue(Of String))) As MapHighlights
            Dim compounds As New List(Of NamedValue(Of String))
            Dim genes As New List(Of NamedValue(Of String))

            For Each node As NamedValue(Of String) In list
                If node.Name.IsPattern(KEGGCompoundIDPatterns) Then
                    compounds.Add(node)
                Else
                    genes.Add(node)
                End If
            Next

            Return New MapHighlights With {
                .compounds = compounds.ToArray,
                .genes = genes.ToArray
            }
        End Function

    End Class

End Namespace
