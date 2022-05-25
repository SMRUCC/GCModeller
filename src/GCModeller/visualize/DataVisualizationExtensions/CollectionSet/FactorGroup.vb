#Region "Microsoft.VisualBasic::2fc439053c5f3af3abcb29360045ed7d, visualize\DataVisualizationExtensions\CollectionSet\FactorGroup.vb"

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

    '     Class FactorGroup
    ' 
    '         Properties: color, data, factor
    ' 
    '         Function: GetAllUniques, GetIntersection, GetUniqueId
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace CollectionSet

    Public Class FactorGroup

        ''' <summary>
        ''' the feature set name
        ''' </summary>
        ''' <returns></returns>
        Public Property factor As String
        ''' <summary>
        ''' the data feature collection
        ''' </summary>
        ''' <returns></returns>
        Public Property data As NamedCollection(Of String)()
        Public Property color As Color

        Public Iterator Function GetAllUniques() As IEnumerable(Of NamedCollection(Of String))
            Dim allIndex As NamedValue(Of Index(Of String))() = data _
                .Select(Function(t)
                            Return New NamedValue(Of Index(Of String)) With {
                                .Name = t.name,
                                .Value = t.value.Indexing
                            }
                        End Function) _
                .ToArray

            For Each [set] As NamedCollection(Of String) In data
                Dim others = allIndex.Where(Function(i) i.Name <> [set].name).ToArray
                Dim excepts As String() = [set] _
                    .Where(Function(id)
                               Return others.All(Function(i) Not id Like i.Value)
                           End Function) _
                    .ToArray

                Yield New NamedCollection(Of String) With {
                    .name = [set].name,
                    .value = excepts,
                    .description = $"{excepts.Length} unique id between all collection set"
                }
            Next
        End Function

        Public Function GetUniqueId(name As String) As String()
            Dim target As NamedCollection(Of String) = data _
                .Where(Function(t) t.name = name) _
                .FirstOrDefault

            If target.IsEmpty Then
                Return {}
            End If

            Dim others = data _
                .Where(Function(t) t.name <> name) _
                .Select(Function(t) t.value) _
                .IteratesALL _
                .Distinct _
                .Indexing
            Dim unique = target.Where(Function(id) Not id Like others).ToArray

            Return unique
        End Function

        Public Iterator Function GetIntersection(collections As String()) As IEnumerable(Of String)
            Dim allIndex As NamedValue(Of Index(Of String))() = data _
                .Where(Function(t)
                           Return collections.IndexOf(t.name) > -1
                       End Function) _
                .Select(Function(t)
                            Return New NamedValue(Of Index(Of String)) With {
                                .Name = t.name,
                                .Value = t.value.Indexing
                            }
                        End Function) _
                .ToArray
            Dim allLabels As String() = allIndex _
                .Select(Function(t) t.Value.Objects) _
                .IteratesALL _
                .Distinct _
                .ToArray

            ' all index must contains the target id
            For Each id As String In allLabels
                Dim countN As Integer = Aggregate i As NamedValue(Of Index(Of String))
                                        In allIndex
                                        Where id Like i.Value
                                        Let hit = 1
                                        Into Sum(hit)

                If countN = collections.Length Then
                    Yield id
                End If
            Next
        End Function

    End Class
End Namespace
