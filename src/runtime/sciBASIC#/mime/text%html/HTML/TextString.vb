#Region "Microsoft.VisualBasic::09f1a6775f814b7fbf7b71ef46fbc302, mime\text%html\HTML\TextString.vb"

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

'     Class TextString
' 
'         Properties: Font, Text
' 
'         Function: ToString
' 
'     Module TextAPI
' 
'         Function: __getFontStyle, __nextEndTag, __nextTag, __setFont, __setFontStyle
'                   TryParse
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports Microsoft.VisualBasic.Text.Xml

Namespace HTML

    Public Class TextString

        Public Property Font As Font
        Public Property Text As String

        Public Overrides Function ToString() As String
            Return Text
        End Function
    End Class

    Public Module TextAPI

        ' html -->  <font face="Microsoft YaHei" size="1.5"><strong>text</strong><b><i>value</i></b></font> 
        ' 解析上述的表达式会产生一个栈，根据html标记来赋值字符串的gdi+属性

        ''' <summary>
        ''' 执行html栈空间解析
        ''' 
        ''' > 在这里假设所有的文本之中的``&lt;``符号已经被转义为``&amp;lt;``
        ''' </summary>
        ''' <param name="html">假设所传递进入这个函数参数的html文本字符串都是完全正确的格式的</param>
        ''' <returns></returns>
        Public Iterator Function TryParse(html$, Optional defaultFontCSS$ = CSSFont.Win7Normal) As IEnumerable(Of TextString)
            Dim defaultFont As Font = CSSFont.TryParse(defaultFontCSS)
            Dim buffer As New Pointer(Of Char)(html.ToCharArray)

            For Each part As TextString In htmlParser(buffer, defaultFont)
                ' 在这里处理转义
                part.Text = XmlEntity.UnescapeHTML(part.Text)
                Yield part
            Next
        End Function

        Private Iterator Function htmlParser(html As Pointer(Of Char), defaultFont As Font) As IEnumerable(Of TextString)
            Dim charsbuffer As New List(Of Char)
            Dim c As Char
            Dim bold As Boolean = False
            Dim italic As Boolean = False
            Dim currentFont As Font = defaultFont

            Do While Not html.EndRead
                c = ++html

                ' 遇到了一个html标签的起始符号
                If c = "<"c Then
                    c = +html

                    If charsbuffer.Count > 0 Then
                        Yield New TextString With {
                            .Font = currentFont,
                            .Text = New String(charsbuffer.PopAll)
                        }
                    End If

                    ' 这个是一个结束的标记
                    If c = "/"c Then
                        Dim tag As String = html.nextEndTag

                        Select Case tag.ToLower
                            Case "font"
                                currentFont = defaultFont
                            Case "b", "strong"
                                bold = False
                                currentFont = currentFont.getLocalScopeFontStyle(bold, italic)
                            Case "i"
                                italic = False
                                currentFont = currentFont.getLocalScopeFontStyle(bold, italic)
                        End Select
                    Else
                        ' 这个是一个html标记的开始
                        Dim tag As HtmlElement = html.__nextTag(c)
                        Dim tagName As String = tag.Name.ToLower

                        Select Case tagName
                            Case "font"
                                currentFont = tag.setFont(bold, italic, defaultFont)
                            Case "strong", "b"
                                bold = True
                                currentFont = currentFont.getLocalScopeFontStyle(bold, italic)
                            Case "i"
                                italic = True
                                currentFont = currentFont.getLocalScopeFontStyle(bold, italic)
                            Case "br"
                                charsbuffer += vbLf
                            Case Else

                        End Select
                    End If

                    html.MoveNext()
                Else
                    charsbuffer += c
                End If
            Loop

            If charsbuffer.Count > 0 Then
                Yield New TextString With {
                    .Font = currentFont,
                    .Text = New String(charsbuffer.PopAll)
                }
            End If
        End Function

        Const FontFaceTag As String = "face"
        Const FontSizeTag As String = "size"

        <Extension>
        Private Function getLocalScopeFontStyle(font As Font, bold As Boolean, italic As Boolean) As Font
            Return New Font(font, getFontStyle(bold, italic))
        End Function

        Private Function getFontStyle(bold As Boolean, italic As Boolean) As FontStyle
            Dim style As FontStyle

            If Not bold AndAlso Not italic Then
                style = FontStyle.Regular
            Else
                If bold Then
                    style += FontStyle.Bold
                End If
                If italic Then
                    style += FontStyle.Italic
                End If
            End If

            Return style
        End Function

        <Extension>
        Private Function setFont(font As HtmlElement, bold As Boolean, italic As Boolean, [default] As Font) As Font
            Dim name As String = font(FontFaceTag).Value
            Dim size As String = font(FontSizeTag).Value

            If String.IsNullOrEmpty(name) Then
                name = [default].Name
            End If
            If String.IsNullOrEmpty(size) Then
                size = [default].Size
            End If

            Dim style As FontStyle = getFontStyle(bold, italic)
            Dim sz As Single = Scripting.CTypeDynamic(Of Single)(size)

            Return New Font(name, sz, style)
        End Function

        <Extension>
        Private Function nextEndTag(buffer As Pointer(Of Char)) As String
            Dim chars As New List(Of Char)

            Do While Not buffer.EndRead AndAlso buffer.Current <> ">"c
                chars += (+buffer)
            Loop

            Return New String(chars)
        End Function

        <Extension>
        Private Function __nextTag(str As Pointer(Of Char), c As Char) As HtmlElement
            Dim chars As New List(Of Char) From {c}
            Dim tag As New HtmlElement

            Do While Not str.EndRead AndAlso str.Current <> " "c AndAlso str.Current <> ">"c
                chars += +str
            Loop

            tag.Name = New String(chars.PopAll)

            Dim name As String
            Dim stacked As Boolean

            Do While Not str.EndRead
                If str.Current = ">"c Then
                    Exit Do
                End If

                Do While Not str.EndRead AndAlso str.Current <> "="c
                    If str.Current = " "c Then
                        If chars.Count > 0 Then
                            Do While Not str.EndRead AndAlso ++str <> "="c
                            Loop
                            Exit Do   ' 在这里进行解析的是属性的名称，不允许有空格
                        Else
                            Call str.MoveNext()
                        End If
                    Else
                        chars += +str
                    End If
                Loop
                name = New String(chars.PopAll)
                str.MoveNext()

                Do While Not str.EndRead
                    If str.Current = """"c Then
                        If chars.Count = 0 AndAlso stacked = False Then
                            stacked = True
                            str.MoveNext()
                        Else ' 这里是一个结束的标志，准备开始下一个token
                            stacked = False
                            str.MoveNext()
                            Exit Do
                        End If
                    Else
                        If str.Current = " "c Then
                            If Not chars.Count = 0 Then
                                chars += " "c
                            End If
                            str.MoveNext()
                        Else
                            chars += +str
                        End If
                    End If
                Loop

                Call tag.Add(New ValueAttribute(name, New String(chars.PopAll)))
            Loop

            Return tag
        End Function
    End Module
End Namespace
