#Region "Microsoft.VisualBasic::eec7d45930cee8caf02f5cc3697ee215, ..\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Reference.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.HtmlParser

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' 参考文献
    ''' </summary>
    Public Class Reference

        <XmlElement> Public Property Authors As String()
        <XmlText> Public Property Title As String
        <XmlAttribute> Public Property Journal As String
        <XmlAttribute> Public Property Reference As String

        Public ReadOnly Property PMID As Long
            Get
                Return CLng(Val(Regex.Match(Reference, "PMID[:]\s*\d+", RegexICSng).Value))
            End Get
        End Property

        Sub New()
        End Sub

        ''' <summary>
        ''' 解析Disease文件之中的参考文献数据
        ''' </summary>
        ''' <param name="meta$"></param>
        ''' <remarks>
        ''' Example as:
        ''' 
        ''' ```
        ''' REFERENCE   PMID:19585782 (description, env_factor)
        '''   AUTHORS   Larsen JC, Johnson NH
        '''   TITLE     Pathogenesis Of Burkholderia pseudomallei And Burkholderia mallei.
        '''   JOURNAL   Mil Med 174:647-51 (2009)
        ''' ```
        ''' </remarks>
        Sub New(meta$())
            Dim data = meta.ToDictionary(
                Function(k) Mid(k, 1, 12).StripBlank,
                Function(v) Mid(v, 13).StripBlank)

            Authors = data.TryGetValue("AUTHORS").StringSplit(",\s*")
            Title = data.TryGetValue("TITLE")
            Journal = data.TryGetValue("JOURNAL")
            Reference = data.TryGetValue("REFERENCE")
        End Sub

        ''' <summary>
        ''' HTML parser
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        Public Shared Function References(data As String()) As Reference()
            Return data.ToArray(AddressOf ReferenceParserHTML)
        End Function

        Const REF_ITEM As String = "<td .+?</div></td></tr>"
        Const DIVInternal$ = "<div .+?>.+?</div>"

        Public Shared Function ReferenceParserHTML(html$) As Reference
            Dim tokens As String() = Regex.Matches(html, REF_ITEM).ToArray
            tokens = tokens _
                .Select(Function(s) Regex.Match(s, DIVInternal).Value) _
                .ToArray

            Dim i As int = Scan0
            Dim PMID As String = tokens.Get(++i).GetValue
            Dim Authors As String = tokens.Get(++i).GetValue
            Dim Title As String = tokens.Get(++i).GetValue
            Dim Journal As String = tokens.Get(++i).GetValue

            If Regex.Match(PMID, "PMID[:]<a").Success Then
                PMID = PMID.GetValue
            End If

            Return New Reference With {
                .Authors = Strings.Split(Authors, ", "),
                .Title = Title,
                .Journal = Journal,
                .Reference = PMID
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"{ String.Join(", ", Authors) }. {Title}. {Journal}.  PMID:{Reference}"
        End Function
    End Class
End Namespace
