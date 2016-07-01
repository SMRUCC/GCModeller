#Region "Microsoft.VisualBasic::0fd217f52635bfb9fe611f87573941b7, ..\GCModeller\data\RegulonDatabase\Regprecise\WebServices\WebParser\Regulon.vb"

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

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization

Namespace Regprecise

    Public Class Regulon

        <XmlElement> Public Property Regulators As Regulator()

    End Class

    Public Class RegulatedGene

        <XmlAttribute> Public Property vimssId As String
        <XmlAttribute> Public Property LocusId As String
        <XmlAttribute> Public Property Name As String
        <XmlAttribute> Public Property [Function] As String

        Public Overrides Function ToString() As String
            Return vimssId & vbTab & LocusId & vbTab & Name
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
            Dim tokens As String() = doc.lTokens
            Return __parser(tokens)
        End Function

        Private Shared Function __parser(s As String) As RegulatedGene
            Dim Tokens As String() = Strings.Split(s, vbTab)
            Dim gene As New RegulatedGene With {
                .vimssId = Tokens.Get(Scan0),
                .LocusId = Tokens.Get(1),
                .Name = Tokens.Get(2)
            }
            Return gene
        End Function
    End Class
End Namespace
