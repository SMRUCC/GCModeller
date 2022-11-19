#Region "Microsoft.VisualBasic::18f835fe3fab9b062e4c921022e439d4, GCModeller\annotations\GO\EnrichBubbles.vb"

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

    '   Total Lines: 125
    '    Code Lines: 95
    ' Comment Lines: 17
    '   Blank Lines: 13
    '     File Size: 5.72 KB


    ' Module EnrichBubbles
    ' 
    '     Function: BubbleModel, BubblePlot, EnrichResult
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Visualize.CatalogProfiling
Imports stdNum = System.Math

Public Module EnrichBubbles

    ''' <summary>
    ''' GO富集结果可视化
    ''' </summary>
    ''' <param name="data">KOBAS富集计算分析的结果数据</param>
    ''' <param name="GO_terms">GO数据库</param>
    ''' <param name="size">输出的图像的大小</param>
    ''' <param name="padding">留白的大小</param>
    ''' <param name="bg">背景色</param>
    ''' <param name="unenrichColor">未被富集的go term的颜色，即那些pvalue值大于<paramref name="pvalue"/>参数值的go term的颜色，默认为浅灰色</param>
    ''' <param name="pvalue">pvalue阈值</param>
    ''' <param name="legendFont">legend的字体CSS</param>
    ''' <param name="geneIDFont">term标签的显示字体CSS</param>
    ''' <param name="displays">每一个GO的命名空间分类之下的显示标签label的term的数量，默认为每个命名空间显示10个term的标签</param>
    ''' <param name="titleFontCSS">标题字体的CSS字体</param>
    ''' <param name="title">Plot绘图的标题</param>
    ''' <returns></returns>
    <Extension>
    Public Function BubblePlot(data As IEnumerable(Of EnrichmentTerm),
                               GO_terms As Dictionary(Of Term),
                               Optional size$ = "1600,1200",
                               Optional padding$ = g.DefaultPadding,
                               Optional bg$ = "white",
                               Optional unenrichColor$ = "gray",
                               Optional pvalue# = 0.01,
                               Optional legendFont$ = CSSFont.PlotSmallTitle,
                               Optional geneIDFont$ = CSSFont.Win10Normal,
                               Optional radius$ = "10,50",
                               Optional displays% = 10,
                               Optional serialTopn As Boolean = False,
                               Optional titleFontCSS$ = CSSFont.Win7Large,
                               Optional title$ = "GO enrichment",
                               Optional bubbleBorder As Boolean = True,
                               Optional correlatedPvalue As Boolean = True,
                               Optional ppi As Integer = 300) As GraphicsData

        Dim enrichResult = data.EnrichResult(GO_terms)
        Dim unenrich As Color = unenrichColor.TranslateColor
        Dim math As New ExpressionEngine
        Dim termsData As Dictionary(Of String, BubbleTerm()) = data _
            .EnrichResult(GO_terms) _
            .ToDictionary(Function(cat) cat.Key,
                          Function(cat)
                              Return cat.Value.BubbleModel(correlatedPvalue).ToArray
                          End Function)

        With New Dictionary(Of String, Color)

            !cellular_component = Color.Red
            !molecular_function = Color.Blue
            !biological_process = Color.Green

            Dim theme As New Theme With {
                .padding = padding,
                .background = bg
            }
            Dim bubble As New CatalogBubblePlot(
                data:=termsData,
                enrichColors:= .ByRef,
                showBubbleBorder:=bubbleBorder,
                displays:=New LabelDisplayStrategy With {.displays = displays, .serialTopn = serialTopn},
                pvalue:=-stdNum.Log10(pvalue),
                unenrich:=unenrichColor.TranslateColor,
                theme:=theme,
                bubbleSize:=radius.Split(","c).Select(AddressOf Val).ToArray
            )

            Return bubble.Plot(size, ppi:=ppi)
        End With
    End Function

    <Extension>
    Public Function EnrichResult(data As IEnumerable(Of EnrichmentTerm), GO_terms As Dictionary(Of Term)) As Dictionary(Of String, EnrichmentTerm())
        Dim result As New Dictionary(Of String, List(Of EnrichmentTerm))

        For Each term As EnrichmentTerm In data.Where(Function(t) GO_terms.ContainsKey(t.ID))
            Dim goTerm As Term = GO_terms(term.ID)

            If Not result.ContainsKey(goTerm.namespace) Then
                Call result.Add(goTerm.namespace, New List(Of EnrichmentTerm))
            End If

            Call result(goTerm.namespace).Add(term)
        Next

        ' Dim reorders = result.ToArray.OrderByDescending(Function(x) x.Value.Count)
        Dim out As New Dictionary(Of String, EnrichmentTerm())
        For Each ns In result
            With ns
                Call out.Add(.Key, .Value)
            End With
        Next
        Return out
    End Function

    <Extension>
    Public Iterator Function BubbleModel(terms As IEnumerable(Of EnrichmentTerm), correlatedPvalue As Boolean) As IEnumerable(Of BubbleTerm)
        For Each gene As EnrichmentTerm In terms
            Yield New BubbleTerm With {
                .data = gene.number,
                .Factor = gene.number / gene.Backgrounds,
                .PValue = gene.P(correlatedPvalue),
                .termId = gene.Term
            }
        Next
    End Function
End Module
