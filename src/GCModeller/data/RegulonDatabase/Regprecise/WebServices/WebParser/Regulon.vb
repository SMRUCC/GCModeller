﻿#Region "Microsoft.VisualBasic::313f095c3fb5a2592c706dcbe81a608b, data\RegulonDatabase\Regprecise\WebServices\WebParser\Regulon.vb"

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

    '     Class Regulon
    ' 
    '         Properties: regulators
    ' 
    '         Function: ToString
    ' 
    '     Class RegulatedGene
    ' 
    '         Properties: description, locusId, name, vimssId
    ' 
    '         Function: (+2 Overloads) __parser, DocParser, ParseDoc, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Regprecise

    Public Class Regulon

        <XmlElement>
        Public Property regulators As Regulator()

        Public Overrides Function ToString() As String
            Return regulators.GetJson
        End Function
    End Class

    Public Class RegulatedGene

        <XmlAttribute> Public Property vimssId As String
        <XmlAttribute> Public Property locusId As String
        <XmlAttribute> Public Property name As String

        <XmlText>
        Public Property description As String

        Public Overrides Function ToString() As String
            Return vimssId & vbTab & locusId & vbTab & name
        End Function

        ''' <summary>
        ''' 从文件系统上面的一个文本文件之中解析出基因的摘要数据
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function ParseDoc(path As String) As RegulatedGene()
            Dim lines As String() = IO.File.ReadAllLines(path)
            Return __parser(lines)
        End Function

        Private Shared Function __parser(lines As String()) As RegulatedGene()
            Dim LQuery = (From line As String In lines
                          Where Not String.IsNullOrEmpty(line)
                          Select __parser(line)).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' 解析文档文本
        ''' </summary>
        ''' <param name="doc"></param>
        ''' <returns></returns>
        Public Shared Function DocParser(doc As String) As RegulatedGene()
            Dim tokens As String() = doc.LineTokens
            Return __parser(tokens)
        End Function

        Private Shared Function __parser(s As String) As RegulatedGene
            Dim Tokens As String() = Strings.Split(s, vbTab)
            Dim gene As New RegulatedGene With {
                .vimssId = Tokens.ElementAtOrDefault(Scan0),
                .locusId = Tokens.ElementAtOrDefault(1),
                .name = Tokens.ElementAtOrDefault(2)
            }
            Return gene
        End Function
    End Class
End Namespace
