﻿#Region "Microsoft.VisualBasic::0bfd88a629503931126bbe64dd416c02, mime\text%html\HTML\HTML.vb"

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

    '     Class HTML
    ' 
    '         Properties: Body, Head, Language
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToArray, ToString
    ' 
    '     Class HtmlHead
    ' 
    '         Properties: CSS, Title
    ' 
    '         Sub: SetBodyBackground
    ' 
    '     Class CSS
    ' 
    '         Properties: Elements, InnerText
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: Add
    ' 
    '     Class CSSElement
    ' 
    '         Properties: Name, Properties
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.MIME.Markup.StreamWriter
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace HTML.DDM

    Public Class HTML

        Public Property Head As HtmlHead
        Public Property Body As HtmlElement

        Public Property Language As String = "zh-cn"

        Sub New(Document As HtmlDocument)

        End Sub

        Sub New()
        End Sub

        Public Function ToArray() As HtmlDocument
            Dim array As New List(Of InnerPlantText)
            Call array.Add(Head)
            Call array.Add(Body)

            Return New HtmlDocument With {
            .Tags = {New HtmlElement With {
                .HtmlElements = array.ToArray,
                .Name = "html"}
            }
        }
        End Function

        Public Overrides Function ToString() As String
            Return HTMLWriter.ToString(ToArray)
        End Function
    End Class

    Public Class HtmlHead : Inherits HtmlElement

        Public Property CSS As CSS
        Public Property Title As HtmlElement

        Public Sub SetBodyBackground(color As String)
            If CSS Is Nothing Then
                CSS = New CSS
            End If

            Dim backColor As New KeyValuePair With {
                .Key = "background-color",
                .Value = color
            }
            Dim cssElement = New CSSElement With {
                .Name = "body",
                .Properties = New List(Of KeyValuePair)({backColor})
            }
            Call CSS.Add(cssElement)
        End Sub
    End Class

    Public Class CSS : Inherits HtmlElement

        Public ReadOnly Property Elements As IReadOnlyList(Of CSSElement)
            Get
                Return _cssElements
            End Get
        End Property

        Dim _cssElements As List(Of CSSElement)

        Sub New()
            _cssElements = New List(Of CSSElement)
        End Sub

        Public Overloads Sub Add(element As CSSElement)
            Call _cssElements.Add(element)
        End Sub

        <XmlText> Public Overrides Property InnerText As String
            Get
                Dim values As String() = _cssElements.Select(Function(css) css.ToString).ToArray
                Return String.Join(vbCrLf, values)
            End Get
            Set(value As String)
                MyBase.InnerText = value
            End Set
        End Property
    End Class

    Public Class CSSElement

        Public Property Properties As List(Of KeyValuePair)
        Public Property Name As String

        Public Overrides Function ToString() As String
            Dim pValues As String() = Properties.Select(Function(prop) $"{prop.Key}: {prop.Value}").ToArray
            Return $"{Name} {"{"} {String.Join("; ", pValues)} {"}"}"
        End Function
    End Class
End Namespace
