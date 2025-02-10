#Region "Microsoft.VisualBasic::36a87433f90375d17eee5eb908435ce8, visualize\Cytoscape\Cytoscape\Graph\Xgmml\File\network\NodeGraphics.vb"

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

    '   Total Lines: 95
    '    Code Lines: 83 (87.37%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 12 (12.63%)
    '     File Size: 3.69 KB


    '     Class NodeGraphics
    ' 
    '         Properties: Fill, FillColor, h, LabelColor, NODE_LABEL_FONT_SIZE
    '                     Outline, radius, Type, w, Width
    '                     x, y, z
    ' 
    '         Function: GetLabelFont
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Imaging

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
#End If

Namespace CytoscapeGraphView.XGMML.File

    Public Class NodeGraphics : Inherits AttributeDictionary

        <XmlAttribute("outline")> Public Property Outline As String
        <XmlAttribute> Public Property z As String
        <XmlAttribute("type")> Public Property Type As String
        <XmlAttribute("fill")> Public Property Fill As String
        <XmlAttribute> Public Property x As Double
        <XmlAttribute("width")> Public Property Width As Double
        <XmlAttribute> Public Property w As Double
        <XmlAttribute> Public Property h As Double
        <XmlAttribute> Public Property y As Double

        Public ReadOnly Property NODE_LABEL_FONT_SIZE As Integer
            Get
                Dim attr = Me.Value("NODE_LABEL_FONT_SIZE")
                If attr Is Nothing OrElse String.IsNullOrEmpty(attr.Value) Then
                    Return Math.Min(w, h)
                Else
                    Return Val(attr.Value)
                End If
            End Get
        End Property

        Public ReadOnly Property FillColor As Color
            Get
                Dim Hex As String = Mid(Fill, 2)
                Dim alpha = Me("NODE_TRANSPARENCY")
                Dim r = HexColor.HexToARGB(Hex, If(alpha Is Nothing, 255, Val(alpha.Value)))
                Return r
            End Get
        End Property

        Public Function GetLabelFont(Scale As Double) As Font
            Dim fName = Me("NODE_LABEL_FONT_FACE")
            Dim size As Integer = NODE_LABEL_FONT_SIZE * Scale

            If Not fName Is Nothing Then
                Return New Font(fName.Value.Split("."c).FirstOrDefault, size)
            Else
                Return New Font(FontFace.MicrosoftYaHei, size)
            End If
        End Function

        Public ReadOnly Property radius As Single
            Get
                Return Math.Sqrt(w ^ 2 + h ^ 2) / 2
            End Get
        End Property

        Public ReadOnly Property LabelColor As Color
            Get
                Dim clattr = Me("NODE_LABEL_COLOR")
                Dim clattrAlpha = Me("NODE_LABEL_TRANSPARENCY")

                If clattr Is Nothing Then
                    Return Color.Black
                End If

                Return HexColor.HexToARGB(Mid(clattr.Value, 2), If(clattrAlpha Is Nothing, 255, Val(clattrAlpha.Value)))
            End Get
        End Property
    End Class
End Namespace
