#Region "Microsoft.VisualBasic::06e328f15e2d98b98a83c50b0443c401, GCModeller\data\RegulonDatabase\Regprecise\WebServices\WebParser\Regulon.vb"

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

    '   Total Lines: 78
    '    Code Lines: 54
    ' Comment Lines: 10
    '   Blank Lines: 14
    '     File Size: 2.71 KB


    '     Class Regulome
    ' 
    '         Properties: regulators
    ' 
    '         Function: getCollection, getSize, ToString
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
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Regprecise

    <XmlType("regulome", [Namespace]:=BacteriaRegulome.regulomeNamespace)>
    Public Class Regulome : Inherits ListOf(Of Regulator)
        Implements Enumeration(Of Regulator)

        <XmlElement("regulator")>
        Public Property regulators As Regulator()

        Public Overrides Function ToString() As String
            Return regulators.GetJson
        End Function

        Protected Overrides Function getSize() As Integer
            Return regulators.Length
        End Function

        Protected Overrides Function getCollection() As IEnumerable(Of Regulator)
            Return regulators
        End Function
    End Class

    Public Class RegulatedGene

        <XmlAttribute> Public Property vimssId As String
        <XmlAttribute> Public Property locusId As String
        <XmlAttribute> Public Property name As String

        <XmlAttribute>
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
