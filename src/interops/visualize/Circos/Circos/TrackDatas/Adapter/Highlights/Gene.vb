#Region "Microsoft.VisualBasic::8894a36bf6114e2d31767fc7ee52bcd4, ..\interops\visualize\Circos\Circos\TrackDatas\Adapter\Highlights\Gene.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports SMRUCC.genomics.GCModeller.DataVisualization
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language

Namespace TrackDatas.Highlights

    ''' <summary>
    ''' 使用highlights来标记基因组之中的基因
    ''' </summary>
    Public Class GeneMark : Inherits Highlights

        Dim COGColors As Dictionary(Of String, String)

        ''' <summary>
        ''' 在这里是使用直方图来显示基因的位置的
        ''' </summary>
        ''' <param name="annos"></param>
        ''' <param name="Color"></param>
        Sub New(annos As IEnumerable(Of IGeneBrief), Color As Dictionary(Of String, String))
            Me.__source =
                LinqAPI.MakeList(Of ValueTrackData) <=
                    From gene As IGeneBrief
                    In annos
                    Let COG As String = If(String.IsNullOrEmpty(gene.COG), "-", gene.COG)
                    Let attr As ValueTrackData = New ValueTrackData With {
                        .start = CInt(gene.Location.Left),
                        .end = CInt(gene.Location.Right),
                        .formatting = New Formatting With {
                            .fill_color = If(Color.ContainsKey(COG), Color(COG), CircosColor.DefaultCOGColor)
                            },
                        .value = 1,
                        .chr = "chr1"
                        }
                    Select attr
            Me.COGColors = Color
        End Sub

        Public Function LegendsDrawing(ref As Point, ByRef gdi As GDIPlusDeviceHandle) As Point
            Dim COGColors = (From clProfile In Me.COGColors
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
