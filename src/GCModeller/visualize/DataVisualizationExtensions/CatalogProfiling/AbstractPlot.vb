#Region "Microsoft.VisualBasic::8518219daf1f898b42bfeb0cdfa7572e, GCModeller\visualize\DataVisualizationExtensions\CatalogProfiling\AbstractPlot.vb"

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

'   Total Lines: 118
'    Code Lines: 99
' Comment Lines: 5
'   Blank Lines: 14
'     File Size: 5.19 KB


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

        ''' <summary>
        ''' union the pathway data in multiple groups
        ''' </summary>
        ''' <returns></returns>
        Protected Overridable Function getPathways() As Dictionary(Of String, String())
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

        Private Structure term_data

            Dim bubble As BubbleTerm
            Dim category As String
            Dim sampleName As String

            Sub New(i As BubbleTerm, category As String, sampleName As String)
                Me.bubble = i
                Me.category = category
                Me.sampleName = sampleName
            End Sub

        End Structure

        Private Structure term_rank

            Dim t As IGrouping(Of String, term_data)
            Dim category As String
            Dim rsd As Double

            Sub New(t As IGrouping(Of String, term_data), category As String, rsd As Double)
                Me.t = t
                Me.category = category
                Me.rsd = rsd
            End Sub

        End Structure

        Private Shared Function evalFilter(d As term_data) As Boolean
            Dim i = d.bubble
            Dim test1 = Not (i.data.IsNaNImaginary OrElse i.Factor.IsNaNImaginary OrElse i.PValue.IsNaNImaginary)
            Dim test2 = i.data * i.PValue > 0 AndAlso i.Factor > 0

            Return test1 AndAlso test2
        End Function

        Private Shared Function evalRsdRank(t As IGrouping(Of String, term_data), eval As Func(Of BubbleTerm, Double), nsamples As Integer) As term_rank
            Dim category As String = t.First.category
            Dim rsd As Double = t _
                .Select(Function(xi) eval(xi.bubble)) _
                .JoinIterates(0.0.Repeats(nsamples - t.Count).Select(Function(any, i) (i + 1) * 999)) _
                .RSD()

            Return New term_rank(t, category, rsd)
        End Function

        ''' <summary>
        ''' evaluate data ranking for bubble data and do data filtering
        ''' </summary>
        ''' <param name="multiples"></param>
        ''' <param name="topN"></param>
        ''' <param name="eval"></param>
        ''' <returns></returns>
        Public Shared Iterator Function TopBubbles(multiples As IEnumerable(Of NamedValue(Of Dictionary(Of String, BubbleTerm()))),
                                                   topN As Integer,
                                                   topNSample As Integer,
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
                                            Return b.Value.Select(Function(i) New term_data(i, category:=b.Key, sampleName:=d.Name))
                                        End Function) _
                                .IteratesALL
                        End Function) _
                .IteratesALL _
                .Where(Function(d) evalFilter(d)) _
                .GroupBy(Function(i) i.bubble.termId) _
                .Select(Function(t) evalRsdRank(t, eval, nsamples)) _
                .GroupBy(Function(i) i.category) _
                .Select(Function(i)
                            Return i _
                                .OrderByDescending(Function(a) a.rsd) _
                                .Take(topN) _
                                .ToArray
                        End Function) _
                .ToArray

            For Each sample As IGrouping(Of String, term_data) In takes _
                .IteratesALL _
                .Select(Function(a) a.t) _
                .IteratesALL _
                .GroupBy(Function(a)
                             Return a.sampleName
                         End Function) _
                .OrderByDescending(Function(d) d.Count) _
                .Take(topNSample)

                Dim cats = sample _
                    .GroupBy(Function(a) a.category) _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return a.Select(Function(i) i.bubble).ToArray
                                  End Function)

                Yield New NamedValue(Of Dictionary(Of String, BubbleTerm())) With {
                    .Name = sample.Key,
                    .Value = cats
                }
            Next
        End Function

    End Class
End Namespace
