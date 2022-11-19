#Region "Microsoft.VisualBasic::c73e5e769577533442d88f6fc9956bb3, GCModeller\visualize\DataVisualizationExtensions\CatalogProfiling\LabelDisplayStrategy.vb"

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

    '   Total Lines: 87
    '    Code Lines: 59
    ' Comment Lines: 18
    '   Blank Lines: 10
    '     File Size: 3.31 KB


    '     Class LabelDisplayStrategy
    ' 
    '         Properties: displays, serialTopn
    ' 
    '         Function: [Default], filterLabelDisplays
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Namespace CatalogProfiling

    ''' <summary>
    ''' the bubble label display strategy
    ''' </summary>
    Public Class LabelDisplayStrategy

        ''' <summary>
        ''' show top n labels for each serial category? 
        ''' otherwise set this property false will make 
        ''' the top n labels between all terms data.
        ''' </summary>
        ''' <returns></returns>
        Public Property serialTopn As Boolean
        ''' <summary>
        ''' the number of the term labels to display on 
        ''' the charting.
        ''' </summary>
        ''' <returns></returns>
        Public Property displays As Integer

        Public Shared Function [Default]() As LabelDisplayStrategy
            Return New LabelDisplayStrategy With {
                .displays = 5,
                .serialTopn = False
            }
        End Function

        Friend Function filterLabelDisplays(serials As SerialData()) As SerialData()
            Dim pt As PointData

            If serialTopn Then
                For Each serial As SerialData In From s As SerialData
                                                 In serials
                                                 Where s.title <> CatalogBubblePlot.unenrichTerm

                    ' 只显示前displays个term的标签字符串，
                    ' 其余的term的标签字符串都设置为空值， 就不会被显示出来了
                    For i As Integer = displays To serial.pts.Length - 1
                        pt = serial.pts(i)
                        serial.pts(i) = New PointData With {
                            .pt = pt.pt,
                            .tag = Nothing,
                            .value = pt.value,
                            .color = pt.color
                        }
                    Next
                Next
            Else
                ' top n labels between all terms
                Dim allTerms = From s As SerialData
                               In serials
                               Where s.title <> CatalogBubblePlot.unenrichTerm
                               Select s.pts

                ' get a index of labels to keeps
                Dim orders As Index(Of String) = allTerms _
                    .IteratesALL _
                    .OrderByDescending(Function(t) t.pt.Y) _
                    .Take(displays) _
                    .Select(Function(t) t.tag) _
                    .Indexing

                For Each s As SerialData In serials
                    For i As Integer = 0 To s.pts.Length - 1
                        If Not s.pts(i).tag Like orders Then
                            pt = s.pts(i)
                            s.pts(i) = New PointData With {
                                .pt = pt.pt,
                                .tag = Nothing,
                                .value = pt.value,
                                .color = pt.color
                            }
                        End If
                    Next
                Next
            End If

            Return serials
        End Function
    End Class
End Namespace
