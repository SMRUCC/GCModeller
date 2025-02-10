#Region "Microsoft.VisualBasic::639ea810e4d13465d7e663d7dbad0383, visualize\ChromosomeMap\FootPrintMap\TSSs.vb"

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

    '   Total Lines: 60
    '    Code Lines: 42 (70.00%)
    ' Comment Lines: 11 (18.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (11.67%)
    '     File Size: 2.52 KB


    '     Class TSSs
    ' 
    '         Properties: Strand, Synonym
    ' 
    '         Sub: Draw
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.ComponentModel.Loci

#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
Imports LineCap = System.Drawing.Drawing2D.LineCap
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
Imports LineCap = Microsoft.VisualBasic.Imaging.LineCap
#End If


Namespace DrawingModels

    Public Class TSSs : Inherits Site

        ''' <summary>
        ''' 链的方向决定箭头的方向，正向链箭头向右，反向链则箭头向左
        ''' </summary>
        ''' <returns></returns>
        Public Property Strand As Strands
        Public Property Synonym As String

        ''' <summary>
        ''' 绘制有左右方向的折线，上面的终端有小箭头
        ''' </summary>
        ''' <param name="Device"></param>
        ''' <param name="Location"></param>
        ''' <param name="FlagLength"></param>
        ''' <param name="FLAG_HEIGHT"></param>
        Public Overrides Sub Draw(Device As IGraphics, Location As Point, FlagLength As Integer, FLAG_HEIGHT As Integer)
            Dim Arrow = New Pen(Color.Black, 3)
            Arrow.EndCap = LineCap.ArrowAnchor

            Call Device.DrawLine(Pens.Black, Location, New Point(Location.X, Location.Y - FLAG_HEIGHT))
            Call Device.DrawLine(Arrow, New Point(Location.X, Location.Y - FLAG_HEIGHT), New Point(Location.X + Strand * FlagLength, Location.Y - FLAG_HEIGHT))
        End Sub
    End Class
End Namespace
