#Region "Microsoft.VisualBasic::4bf3ca1199ee4a4df06a6a181edaa3b2, GCModeller\visualize\DataVisualizationExtensions\CatalogProfiling\AbstractPlot.vb"

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

'   Total Lines: 123
'    Code Lines: 106
' Comment Lines: 3
'   Blank Lines: 14
'     File Size: 5.33 KB


'     Class MultipleCategoryProfiles
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: getCategories, getPathways, TopBubbles
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace CatalogProfiling

    Public MustInherit Class MultipleCategoryProfiles : Inherits Plot

        ''' <summary>
        ''' the multiple groups data
        ''' 
        ''' [group_label => [category => term_bubbles]]
        ''' </summary>
        Protected Friend ReadOnly multiples As List(Of NamedValue(Of Dictionary(Of String, BubbleTerm())))

        Protected Sub New(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))), theme As Theme)
            Call MyBase.New(theme)

            Me.multiples = multiples.AsList
        End Sub

        Protected Function getCategories() As String()
            Return multiples _
                .Select(Function(t) t.Value.Keys) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End Function

        Protected Function getPathways() As Dictionary(Of String, String())
            Return multiples _
                .Select(Function(t) t.Value.Select(Function(b) b.Value.Select(Function(a) (a.termId, b.Key)))) _
                .IteratesALL _
                .IteratesALL _
                .GroupBy(Function(t) t.Key) _
                .ToDictionary(Function(a) a.Key,
                              Function(b)
                                  Return b _
                                      .Select(Function(t) t.termId) _
                                      .Distinct _
                                      .ToArray
                              End Function)
        End Function

        Public Shared Iterator Function TopBubbles(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))), 
                                                   topN As Integer, 
                                                   eval As Func(Of BubbleTerm, Double)) As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm())))
            Dim all = multiples.ToArray
            Dim categories As String() = all _
               .Select(Function(t) t.Value.Keys) _
               .IteratesALL _
               .Distinct _
               .ToArray
            Dim nsamples As Double = all.Length
            Dim takes = all _
                .Select(Function(d)
                            Return d.Value _
                                .Select(Function(b)
                                            Return b.Value.Select(Function(i) (i, category:=b.Key, sampleName:=d.Name))
                                        End Function)
                        End Function) _
                .IteratesALL _
                .IteratesALL _
                .Where(Function(d)
                           Dim i = d.i
                           Dim test1 = Not (i.data.IsNaNImaginary OrElse i.Factor.IsNaNImaginary OrElse i.PValue.IsNaNImaginary)
                           Dim test2 = i.data * i.PValue > 0 AndAlso i.Factor > 0

                           Return test1 AndAlso test2
                       End Function) _
                .GroupBy(Function(i) i.i.termId) _
                .Select(Function(t)
                            Dim category As String = t.First.category
                            Dim rsd As Double = t _
                                .Select(Function(xi) eval(xi.i)) _
                                .JoinIterates(0.0.Repeats(nsamples - t.Count).Select(Function(any) randf.NextDouble * 99999)) _
                                .RSD()

                            Return (t, Category, rsd)
                        End Function) _
                .GroupBy(Function(i) i.category) _
                .Select(Function(i)
                            Return i _
                                .OrderByDescending(Function(a) a.rsd) _
                                .Take(topN) _
                                .ToArray
                        End Function) _
                .ToArray

            For Each sample In takes _
                .IteratesALL _
                .Select(Function(a) a.t) _
                .IteratesALL _
                .GroupBy(Function(a)
                             Return a.sampleName
                         End Function) _
                .OrderByDescending(Function(d) d.Count) _
                .Take(12)

                Dim cats = sample _
                    .GroupBy(Function(a) a.category) _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return a.Select(Function(i) i.i).ToArray
                                  End Function)

                Yield New NamedValue(Of Dictionary(Of String, BubbleTerm())) With {
                    .Name = sample.Key,
                    .Value = cats
                }
            Next
        End Function

    End Class
End Namespace
