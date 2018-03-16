#Region "Microsoft.VisualBasic::dd277e9d5d4610c389c9b0c175be5f5e, visualize\Cytoscape\Cytoscape\Graph\cytoscape.js\Styles\Translator.vb"

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

    '     Class CSSTranslator
    ' 
    '         Properties: BackgroundColor, BackgroundOpacity, BorderOpacity, Color, FontSize
    '                     Shape, TextOpacity, Width
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace CytoscapeGraphView.Cyjs.style

    Public Class CSSTranslator

        Dim css As Dictionary(Of String, String)

        Public Property TextOpacity As Integer
            Get
                Return CInt(Val(css.TryGetValue("text-opacity")) * 255)
            End Get
            Set(value As Integer)
                css("text-opacity") = value / 255
            End Set
        End Property

        Public Property BackgroundColor As Color
            Get
                Return css.TryGetValue("background-color").TranslateColor
            End Get
            Set(value As Color)
                css("background-color") = value.RGBExpression
            End Set
        End Property

        Public Property FontSize As Single
            Get
                Return Val(css.TryGetValue("font-size"))
            End Get
            Set(value As Single)
                css("font-size") = value
            End Set
        End Property

        Public Property BackgroundOpacity As Integer
            Get
                Return CInt(Val(css.TryGetValue("background-opacity")) * 255)
            End Get
            Set(value As Integer)
                css("background-opacity") = value / 255
            End Set
        End Property

        Public Property Shape As String
            Get
                Return css("shape")
            End Get
            Set(value As String)
                css("shape") = value
            End Set
        End Property

        Public Property Width As Single
            Get
                Return Val(css("width"))
            End Get
            Set(value As Single)
                css("width") = value
            End Set
        End Property

        Public Property Color As Color
            Get
                Return css.TryGetValue("color").TranslateColor
            End Get
            Set(value As Color)
                css("color") = value.RGBExpression
            End Set
        End Property

        Public Property BorderOpacity As Integer
            Get
                Return Val(css.TryGetValue("border-opacity")) * 255
            End Get
            Set(value As Integer)
                css("border-opacity") = value / 255
            End Set
        End Property

        Sub New(css As Dictionary(Of String, String))
            Me.css = css
        End Sub

        Public Overrides Function ToString() As String
            Return css.GetJson
        End Function
    End Class
End Namespace
