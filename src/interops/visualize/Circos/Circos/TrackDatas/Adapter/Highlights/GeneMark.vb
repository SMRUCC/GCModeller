#Region "Microsoft.VisualBasic::7bc26d5c0089f51276e92d43c71c2661, ..\interops\visualize\Circos\Circos\TrackDatas\Adapter\Highlights\Gene.vb"

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
Imports System.Text
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Visualize.Circos.Colors
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.ComponentModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots

Namespace TrackDatas.Highlights

    ''' <summary>
    ''' 使用highlights来标记基因组之中的基因
    ''' </summary>
    Public Class GeneMark : Inherits Highlights

        Public Property COGColors As Dictionary(Of String, String)

        ''' <summary>
        ''' 在这里是使用直方图来显示基因的位置的
        ''' </summary>
        ''' <param name="annos"></param>
        ''' <param name="Color"></param>
        Sub New(annos As IEnumerable(Of IGeneBrief), Color As Dictionary(Of String, String))
            __source = LinqAPI.MakeList(Of ValueTrackData) <=
 _
                From gene As IGeneBrief
                In annos
                Let COG As String = If(String.IsNullOrEmpty(gene.COG), "-", gene.COG)
                Let fill As String = If(
                    Color.ContainsKey(COG),
                    Color(COG),
                    CircosColor.DefaultCOGColor)
                Select New ValueTrackData With {
                    .start = CInt(gene.Location.Left),
                    .end = CInt(gene.Location.Right),
                    .value = 1,
                    .chr = "chr1",
                    .formatting = New Formatting With {
                        .fill_color = fill
                    }
                }

            COGColors = Color
        End Sub

        ''' <summary>
        ''' 直接从motif位点构建，这个模型并不显示标签信息
        ''' 使用<see cref="HighLight"/>生成track数据
        ''' 
        ''' ```vbnet
        ''' Dim track As New HighLight(New GeneMark(genes, colors))
        ''' ```
        ''' </summary>
        ''' <param name="sites"></param>
        ''' <param name="color"></param>
        Sub New(sites As IEnumerable(Of IMotifSite), color As Dictionary(Of String, String), Optional chr$ = "chr1")
            Dim locis As IMotifSite() = sites.ToArray
            Dim types$() = locis _
                .Select(Function(x) x.Type) _
                .Distinct _
                .ToArray

            COGColors = color
            __source = LinqAPI.MakeList(Of ValueTrackData) <=
                From site As IMotifSite
                In locis
                Let COG = If(String.IsNullOrEmpty(site.Type), "-", site.Type)
                Let fill = If(
                    color.ContainsKey(COG),
                    color(COG),
                    CircosColor.DefaultCOGColor)
                Select New ValueTrackData With {
                    .start = site.Site.Left,
                    .end = site.Site.Right,
                    .value = 1,
                    .chr = chr,
                    .formatting = New Formatting With {
                        .fill_color = fill
                    }
                }
        End Sub

        ''' <summary>
        ''' 假若使用这个构造函数的话，这个需要手工初始化<see cref="__source"/>和<see cref="COGColors"/>
        ''' </summary>
        Protected Sub New()
        End Sub

        Public Function LegendsDrawing(ref As Point, ByRef gdi As GDIPlusDeviceHandle) As Point
            Dim COGColors = (From clProfile
                             In Me.COGColors
                             Select clProfile.Key,
                                 Cl = CircosColor.FromKnownColorName(clProfile.Value)) _
                                    .ToDictionary(Function(cl) cl.Key,
                                                  Function(x) DirectCast(New SolidBrush(x.Cl), Brush))
            Dim Margin As Integer = 50
            Dim font As New Font(FontFace.MicrosoftYaHei, 20)
            Dim w As Integer = CInt(gdi.Width * 0.3)

            ref = New Point(Margin, gdi.Height - 7 * Margin)

            Call gdi.Graphics.DrawingCOGColors(COGColors, ref, font, w, Margin)

            Return ref
        End Function
    End Class
End Namespace
