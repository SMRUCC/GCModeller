#Region "Microsoft.VisualBasic::6e4d1a6ea9db92bae5ae042046be965c, visualize\SyntenyVisual\DrawingModel.vb"

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

    ' Class DrawingModel
    ' 
    '     Properties: briefs, Links, margin, penWidth, size
    ' 
    '     Function: Visualize
    ' 
    ' Class GenomeBrief
    ' 
    '     Properties: Name, Size, Y
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Text
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' 绘制很多个基因组之间的共线性关系可视化
''' </summary>
Public Class DrawingModel

    Public Property margin As Size
    Public Property Links As Line()
    Public Property size As Size
    ''' <summary>
    ''' 在这里控制基因组共线性的绘制的连线的粗细
    ''' </summary>
    ''' <returns></returns>
    Public Property penWidth As Integer
    Public Property briefs As GenomeBrief()

    Public Function Visualize() As GraphicsData
        Dim font As New Font(FontFace.MicrosoftYaHei, 12, FontStyle.Italic)  ' 默认的字体
        Dim cssFont$ = New CSSFont(font).CSSValue
        Dim texts As Image() = briefs.Select(Function(x)
                                                 Dim g As IGraphics = DriverLoad.CreateDefaultRasterGraphics(New Size(600, 200), Color.Transparent)
                                                 g.DrawString(x.Name, font, Brushes.Black, New PointF)
                                                 g.Flush()

                                                 Return DirectCast(g, GdiRasterGraphics).ImageResource
                                             End Function).ToArray
        Dim maxtLen As Integer = texts.Select(Function(x) x.Width).Max
        Dim cl As SolidBrush = New SolidBrush(Color.Black)
        Dim totalSize As New Size(size.Width + maxtLen * 1.5, size.Height)

        Return g.GraphicsPlots(totalSize, Padding.Zero, "white",
                               Sub(ByRef gdi, canvas)
                                   Dim dh As Integer = briefs.First.Name.MeasureSize(gdi, font).Height / 2

                                   For Each lnk As Line In Links   ' 首先绘制连线
                                       Call lnk.Draw(gdi, penWidth)
                                   Next

                                   For Each x As SeqValue(Of Image) In texts.SeqIterator    '然后绘制基因组的简单表示，以及显示标题
                                       Dim y As Integer = briefs(x).Y
                                       Dim pt1 As New Point(margin.Width, y)
                                       Dim pt2 As New Point(size.Width - margin.Width, y)

                                       Call gdi.DrawImageUnscaled(+x, New Point(size.Width, y - dh))
                                       Call gdi.DrawLine(New Pen(Color.Gray, 10), pt1, pt2)
                                   Next
                               End Sub)
    End Function
End Class

''' <summary>
''' The simple abstract of the genome drawing data.
''' </summary>
Public Class GenomeBrief

    Public Property Y As Integer
    ''' <summary>
    ''' The display title.(由于需要兼容html文本，所以这里是被当做html文本来对待了)
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
    ''' <summary>
    ''' The length of the genome nt sequence
    ''' </summary>
    ''' <returns></returns>
    Public Property Size As String

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class
