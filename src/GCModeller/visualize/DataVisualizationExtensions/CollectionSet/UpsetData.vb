#Region "Microsoft.VisualBasic::13e867ab5af62c44e73332c16159a945, GCModeller\visualize\DataVisualizationExtensions\CollectionSet\UpsetData.vb"

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

    '   Total Lines: 122
    '    Code Lines: 92
    ' Comment Lines: 15
    '   Blank Lines: 15
    '     File Size: 5.17 KB


    '     Class UpsetData
    ' 
    '         Properties: allData, collectionSetLabels, compares, desc, groups
    '                     index, intersectionCut, setSize, size
    ' 
    '         Function: CreateUpSetData, getCombinations, getIntersectList
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

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
        Public Property desc As Boolean
        Public Property intersectionCut As Integer
        Public Property setSize As NamedValue(Of Integer)()
        Public Property allData As Dictionary(Of String, NamedCollection(Of String))
        Public Property index As Index(Of String)()

        Public ReadOnly Property size As Integer
            Get
                Return compares.data.Length
            End Get
        End Property

        Public ReadOnly Property collectionSetLabels As String()
            Get
                Return groups
            End Get
        End Property

        Public Shared Function CreateUpSetData(collectionSet As IntersectionData, intersectionCut As Integer, desc As Boolean) As UpsetData
            Dim collectionSetLabels As String() = collectionSet.GetAllCollectionTags
            Dim factor As FactorGroup = collectionSet.groups
            Dim allCompares As String()() = getCombinations(collectionSetLabels) _
                .IteratesALL _
                .ToArray
            Dim allData As Dictionary(Of String, NamedCollection(Of String)) = factor.GetAllUniques.ToDictionary(Function(i) i.name)
            ' index - a vs b
            ' intersect - id intersection between a / b
            Dim intersectList As (index As Index(Of String), intersect As String())() = getIntersectList(factor, allCompares, collectionSetLabels) _
                .Where(Function(d) d.intersect.Length > intersectionCut) _
                .Sort(Function(d) d.intersect.Length, desc) _
                .ToArray
            Dim barData As New List(Of NamedCollection(Of String))
            Dim htmlColor As String = factor.color.ToHtmlColor
            Dim index As New List(Of Index(Of String))

            For Each combine In intersectList
                Dim intersect As String() = combine.intersect

                Call index.Add(combine.index)
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
                .compares = factorCombines,
                .desc = desc,
                .intersectionCut = intersectionCut,
                .setSize = collectionSet.GetSetSize,
                .allData = allData,
                .index = index.ToArray
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
