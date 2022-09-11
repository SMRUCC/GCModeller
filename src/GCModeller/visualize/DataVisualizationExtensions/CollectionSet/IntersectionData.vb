#Region "Microsoft.VisualBasic::ec43e9d3201958259687ec4e47f4ed3c, GCModeller\visualize\DataVisualizationExtensions\CollectionSet\IntersectionData.vb"

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

    '   Total Lines: 51
    '    Code Lines: 37
    ' Comment Lines: 4
    '   Blank Lines: 10
    '     File Size: 1.68 KB


    '     Class IntersectionData
    ' 
    '         Properties: groups, size
    ' 
    '         Function: GetAllCollectionTags, GetSetSize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq

Namespace CollectionSet

    Public Class IntersectionData

        Public Property groups As FactorGroup

        Public ReadOnly Property size As Integer
            Get
                Return groups.data.Length
            End Get
        End Property

        Public Function GetSetSize() As NamedValue(Of Integer)()
            Dim allLabels = groups.data.GroupBy(Function([set]) [set].name).ToArray
            Dim counts = allLabels _
                .Select(Function(d)
                            Dim name As String = d.Key
                            Dim num As Integer = d _
                                .Select(Function([set]) [set].value) _
                                .IteratesALL _
                                .Distinct _
                                .Count

                            Return New NamedValue(Of Integer) With {
                                .Name = name,
                                .Value = num
                            }
                        End Function) _
                .ToArray

            Return counts
        End Function

        ''' <summary>
        ''' get the labels of all collection set like ``a vs b``, etc
        ''' </summary>
        ''' <returns></returns>
        Public Function GetAllCollectionTags() As String()
            Return groups.data _
                .Select(Function(t) t.name) _
                .Distinct _
                .ToArray
        End Function

    End Class

End Namespace
