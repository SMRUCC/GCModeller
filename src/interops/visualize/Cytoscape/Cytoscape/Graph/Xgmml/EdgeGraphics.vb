﻿#Region "Microsoft.VisualBasic::438c688c3e581e6a739a9dd83ff0d115, visualize\Cytoscape\Cytoscape\Graph\Xgmml\EdgeGraphics.vb"

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

    '     Class EdgeGraphics
    ' 
    '         Properties: EdgeLabel, Fill, FontColor, LabelSize, LineColor
    '                     Width
    ' 
    '         Function: GetLabelFont
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Imaging

Namespace CytoscapeGraphView.XGMML

    Public Class EdgeGraphics : Inherits AttributeDictionary

        <XmlAttribute("width")> Public Property Width As Double
        <XmlAttribute("fill")> Public Property Fill As String

        Public ReadOnly Property LineColor As Color
            Get
                Dim Hex As String = Mid(Fill, 2)
                Dim alpha = Me("EDGE_TRANSPARENCY")
                Dim r = HexColor.HexToARGB(Hex, If(alpha Is Nothing, 255, Val(alpha.Value)))
                Return r
            End Get
        End Property

        Public ReadOnly Property LabelSize As Integer
            Get
                Dim attr = Me("EDGE_LABEL_FONT_SIZE")
                If attr Is Nothing Then
                    Return 10
                Else
                    Return Val(attr.Value)
                End If
            End Get
        End Property

        Public Function GetLabelFont(Scale As Double) As Font
            Dim size = Scale * LabelSize
            Dim Font = Me("EDGE_LABEL_FONT_FACE")
            If Font Is Nothing Then
                Return New Font(FontFace.MicrosoftYaHei, size)
            Else
                Return New Font(Font.Value.Split("."c).First, size)
            End If
        End Function

        Public ReadOnly Property FontColor As Color
            Get
                Dim clattr = Me("EDGE_LABEL_COLOR")
                Dim alpha = Me("EDGE_LABEL_TRANSPARENCY")

                If clattr Is Nothing Then
                    Return Color.Black
                End If

                Return HexColor.HexToARGB(Mid(clattr.Value, 2), If(alpha IsNot Nothing, Val(alpha.Value), 255))
            End Get
        End Property

        Public ReadOnly Property EdgeLabel As String
            Get
                Dim attr = Me("EDGE_LABEL")
                If attr Is Nothing Then
                    Return ""
                Else
                    Return attr.Value
                End If
            End Get
        End Property
    End Class
End Namespace
