Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports stdNum = System.Math

Namespace CollectionSet

    Public Class UpsetData

        ''' <summary>
        ''' the raw input group labels
        ''' </summary>
        ''' <returns></returns>
        Public Property groups As String()
        ''' <summary>
        ''' combination between the groups and the 
        ''' intersection data set of the data.
        ''' </summary>
        ''' <returns></returns>
        Public Property compares As FactorGroup

        Public ReadOnly Property size As Integer
            Get
                Return compares.data.Length
            End Get
        End Property

        Public Shared Function CreateUpSetData(collectionSet As IntersectionData, intersectionCut As Integer, desc As Boolean) As UpsetData
            Dim collectionSetLabels As String() = collectionSet.GetAllCollectionTags
            Dim factor As FactorGroup = collectionSet.groups
            Dim allCompares As String()() = getCombinations(collectionSetLabels) _
                .IteratesALL _
                .ToArray
            Dim intersectList As (index As Index(Of String), intersect As String())() = getIntersectList(factor, allCompares, collectionSetLabels) _
                .Where(Function(d) d.intersect.Length > intersectionCut) _
                .Sort(Function(d) d.intersect.Length, desc) _
                .ToArray
            Dim barData As New List(Of NamedCollection(Of String))
            Dim htmlColor As String = factor.color.ToHtmlColor

            For Each combine In intersectList
                Dim intersect As String() = combine.intersect

                Call New NamedCollection(Of String) With {
                    .name = combine.index.Objects.JoinBy(" vs "),
                    .value = intersect,
                    .description = htmlColor
                }.DoCall(AddressOf barData.Add)
            Next

            Dim factorCombines As New FactorGroup With {
                .color = factor.color,
                .factor = factor.factor,
                .data = barData.ToArray
            }

            Return New UpsetData With {
                .groups = factor.data _
                    .Select(Function(a) a.name) _
                    .ToArray,
                .compares = factorCombines
            }
        End Function

        ''' <summary>
        ''' 2 vs 2 -> a vs b vs c vs ...
        ''' </summary>
        ''' <returns></returns>
        Private Shared Iterator Function getCombinations(collectionSetLabels As String()) As IEnumerable(Of String()())
            For i As Integer = 2 To collectionSetLabels.Length
                Yield collectionSetLabels _
                    .AllCombinations(size:=i) _
                    .GroupBy(Function(combine)
                                 Return combine.Distinct.OrderBy(Function(str) str).JoinBy("---")
                             End Function) _
                    .Select(Function(group)
                                Return group.First.Distinct.ToArray
                            End Function) _
                    .ToArray
            Next
        End Function

        Private Shared Iterator Function getIntersectList(factor As FactorGroup, allCompares As String()(), collectionSetLabels As String()) As IEnumerable(Of (index As Index(Of String), intersect As String()))
            For Each combine As String() In allCompares
                Dim intersect As String() = factor _
                    .GetIntersection(combine) _
                    .ToArray

                Yield (index:=combine.Indexing, intersect)
            Next

            For Each lbl As String In collectionSetLabels
                Dim unique As String() = factor.GetUniqueId(lbl)

                Yield ({lbl}.Indexing, unique)
            Next
        End Function
    End Class
End Namespace