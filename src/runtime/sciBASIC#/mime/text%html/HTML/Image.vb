﻿#Region "Microsoft.VisualBasic::dbbbd2c41e1657db270004a827fe80eb, mime\text%html\HTML\Image.vb"

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

    '     Class Image
    ' 
    '         Properties: Align, Alt, Border, HSpace, source
    '                     Text
    ' 
    '         Function: GetImages, ResParser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text.HtmlParser

Namespace HTML

    Public Class Image

        <XmlAttribute> Public Property Border As String
        <XmlAttribute> Public Property HSpace As String
        <XmlAttribute> Public Property Alt As String
        <XmlAttribute> Public Property source As String
        <XmlAttribute> Public Property Align As String

        Public Overrides Function ToString() As String
            Dim attrs As StringBuilder = New StringBuilder(1024)

            If Not String.IsNullOrEmpty(Border) Then Call attrs.Append($"{NameOf(Border)}=""{Border}"" ")
            If Not String.IsNullOrEmpty(HSpace) Then Call attrs.Append($"{NameOf(HSpace)}=""{HSpace}"" ")
            If Not String.IsNullOrEmpty(Alt) Then Call attrs.Append($"{NameOf(Alt)}=""{Alt}"" ")
            If Not String.IsNullOrEmpty(source) Then Call attrs.Append($"src=""{source}"" ")
            If Not String.IsNullOrEmpty(Align) Then Call attrs.Append($"{NameOf(Align)}=""{Align}"" ")

            Return $"<img {attrs.ToString} />"
        End Function

        ''' <summary>
        ''' 这个Image对象的原始的html字符串
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Text As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="img">&lt;img> html标签</param>
        ''' <returns></returns>
        Public Shared Function ResParser(img As String) As Image
            Dim src As String = img.src

            Return New Image With {
                ._Text = img,
                .source = src
            }
        End Function

        Public Shared Function GetImages(html As String) As Image()
            Dim data As String() = Regex.Matches(html, HtmlStrips.imgHtmlTagPattern, RegexICSng).ToArray
            Dim res As Image() = data.Select(Function(tag) Image.ResParser(tag)).ToArray
            Return res
        End Function
    End Class
End Namespace
