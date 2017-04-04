#Region "Microsoft.VisualBasic::68565dc1e09f14393fbef138fc8c8a5f, ..\visualize\GCModeller.DataVisualization\Extensions.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel

Public Module Extensions

    Const COGNotAssign As String = "COG_NOT_ASSIGN"

    ''' <summary>
    ''' 绘制COG分类的颜色，请注意，对于没有COG颜色分类的，情使用空字符串来替代
    ''' </summary>
    ''' <param name="gdi"></param>
    ''' <remarks></remarks>
    ''' 
    <Extension> Public Sub DrawingCOGColors(ByRef gdi As Graphics,
                                            COGsColor As Dictionary(Of String, Brush),
                                            ref As Point,
                                            legendFont As Font,
                                            width As Integer,
                                            margin As Integer)

        Dim top As Integer = ref.Y - 100
        Dim left As Integer = ref.X
        Dim legendHeight As Integer = 20
        Dim FontHeight As Single = gdi.MeasureString("COG", legendFont).Height
        Dim d As Single = (legendHeight - FontHeight) / 2
        Dim colors = LinqAPI.MakeList(Of KeyValuePair(Of String, Brush)) <=
 _
            From x As KeyValuePair(Of String, Brush)
            In COGsColor
            Where Not String.IsNullOrEmpty(x.Key)
            Select x
            Order By x.Key Ascending

        If COGsColor.ContainsKey("") Then
            Dim Cl As Brush = COGsColor("")
            Call colors.Add(COGNotAssign, Cl)
        End If

        Dim regn As Region
        Dim rect As Rectangle

        For Each color As KeyValuePair(Of String, Brush) In colors

            rect = New Rectangle(New Point(left, top),
                                 New Size(100, legendHeight))
            regn = New Region(rect)

            Call gdi.FillRegion(color.Value, regn)
            Call gdi.DrawString(color.Key,
                                legendFont,
                                Brushes.Black,
                                New Point(left + 110, top + d))

            left += 120 + gdi.MeasureString(color.Key, legendFont).Width

            If left >= width - margin Then
                left = margin
                top += 2 * legendHeight
            End If
        Next
    End Sub

    <Extension>
    Public Function DrawCatalogProfiling(Of T As ICOGCatalog)(res As GraphicsData, genes As IEnumerable(Of T), left%, size As Size) As GraphicsData
        Dim legend As GraphicsData = genes.COGCatalogProfilingPlot(size)

        Using g As IGraphics = (res).CreateGraphics
            Dim cogLegendTop = res.Height - 150
            Dim top = cogLegendTop - legend.Height

            Call g.DrawImageUnscaled(legend, New Point(left, top))
        End Using

        Return res
    End Function
End Module
