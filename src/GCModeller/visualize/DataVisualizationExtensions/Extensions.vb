#Region "Microsoft.VisualBasic::49504569186eee383ddbc202c8e14004, GCModeller\visualize\DataVisualizationExtensions\Extensions.vb"

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

    '   Total Lines: 96
    '    Code Lines: 72
    ' Comment Lines: 7
    '   Blank Lines: 17
    '     File Size: 3.67 KB


    ' Module Extensions
    ' 
    '     Function: DrawCatalogProfiling
    ' 
    '     Sub: DrawingCOGColors
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Visualize.CatalogProfiling

Public Module Extensions

    Public Const COGNotAssign As String = "COG_NOT_ASSIGN"

    ''' <summary>
    ''' 绘制COG分类的颜色，请注意，对于没有COG颜色分类的，情使用空字符串来替代
    ''' </summary>
    ''' <param name="dev">gdi/svg graphics engine</param>
    ''' <remarks></remarks>
    ''' 
    <Extension> Public Sub DrawingCOGColors(ByRef dev As IGraphics,
                                            COGsColor As Dictionary(Of String, Brush),
                                            ref As Point,
                                            legendFont As Font,
                                            width As Integer,
                                            margin As Integer)

        Dim top As Integer = ref.Y - 100
        Dim left As Integer = ref.X
        Dim legendHeight As Integer = 20
        Dim FontHeight As Single = dev.MeasureString("COG", legendFont).Height
        Dim d As Single = (legendHeight - FontHeight) / 2
        Dim colors = LinqAPI.MakeList(Of KeyValuePair(Of String, Brush)) <=
 _
            From x As KeyValuePair(Of String, Brush)
            In COGsColor
            Where Not String.IsNullOrEmpty(x.Key)
            Select x
            Order By x.Key Ascending

        Dim notAssigned As Brush = Nothing

        If COGsColor.ContainsKey("") Then
            notAssigned = COGsColor("")
        ElseIf COGsColor.ContainsKey(COGNotAssign) Then
            notAssigned = COGsColor(COGNotAssign)
        Else
            notAssigned = Brushes.Brown
        End If

        Dim rect As Rectangle
        Dim location As Point
        Dim g = dev
        Dim plot = Sub(category$, color As Brush)
                       location = New Point(left + 110, top + d)
                       rect = New Rectangle With {
                           .Location = New Point(left, top),
                           .Size = New Size(100, legendHeight)
                       }

                       Call g.FillRectangle(color, rect)
                       Call g.DrawString(category, legendFont, Brushes.Black, location)

                       left += 120 + g.MeasureString(category, legendFont).Width

                       If left >= width - margin Then
                           left = margin
                           top += 2 * legendHeight
                       End If
                   End Sub

        ' 不会绘制空名称以及未赋值的分类的颜色
        For Each color As KeyValuePair(Of String, Brush) In colors _
            .Where(Function(k)
                       Return Not k.Key.StringEmpty AndAlso k.Key <> COGNotAssign
                   End Function)

            Call plot(category:=color.Key, color:=color.Value)
        Next

        Call plot(category:=COGNotAssign, color:=notAssigned)
    End Sub

    <Extension>
    Public Function DrawCatalogProfiling(Of T As ICOGCatalog)(res As GraphicsData, genes As IEnumerable(Of T), left%, size$) As GraphicsData
        Dim legend As GraphicsData = genes.COGCatalogProfilingPlot(size)

        Using g As IGraphics = (res).CreateGraphics
            Dim cogLegendTop = res.Height - 150
            Dim top = cogLegendTop - legend.Height

            Call g.DrawImageUnscaled(legend, New Point(left, top))
        End Using

        Return res
    End Function
End Module
