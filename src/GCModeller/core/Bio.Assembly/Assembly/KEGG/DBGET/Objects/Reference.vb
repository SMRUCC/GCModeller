#Region "Microsoft.VisualBasic::abb11247e27df24c529e6842ce72b157, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Reference.vb"

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

    '   Total Lines: 103
    '    Code Lines: 64
    ' Comment Lines: 22
    '   Blank Lines: 17
    '     File Size: 3.63 KB


    '     Class Reference
    ' 
    '         Properties: Authors, DOI, Journal, PMID, Reference
    '                     Title
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ReferenceParserHTML, References, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' 参考文献
    ''' </summary>
    Public Class Reference

        <XmlElement>
        Public Property Authors As String()
        Public Property Title As String

        <XmlAttribute> Public Property Journal As String
        <XmlAttribute> Public Property Reference As String
        <XmlAttribute> Public Property DOI As String

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
            Return data.Select(AddressOf ReferenceParserHTML).ToArray
        End Function

        Const REF_ITEM As String = "<td .+?</div></td></tr>"
        Const DIVInternal$ = "<div .+?>.+?</div>"

        Public Shared Function ReferenceParserHTML(html$) As Reference
            Dim tokens As String() = Regex.Matches(html, REF_ITEM).ToArray
            tokens = tokens _
                .Select(Function(s) Regex.Match(s, DIVInternal).Value) _
                .ToArray

            Dim i As i32 = Scan0
            Dim PMID As String = tokens.ElementAtOrDefault(++i).GetValue
            Dim Authors As String = tokens.ElementAtOrDefault(++i).GetValue
            Dim Title As String = tokens.ElementAtOrDefault(++i).GetValue
            Dim Journal As String = tokens.ElementAtOrDefault(++i).GetValue
            Dim DOI$

            If Regex.Match(PMID, "PMID[:]<a").Success Then
                PMID = PMID.GetValue
            End If

            Dim ref = Journal.GetTagValue("DOI:")

            Journal = ref.Name.StripHTMLTags(stripBlank:=True)
            DOI = ref.Value.href

            Return New Reference With {
                .Authors = Strings.Split(Authors, ", "),
                .Title = Title,
                .Journal = Journal,
                .Reference = PMID,
                .DOI = DOI
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"{ String.Join(", ", Authors) }. {Title}. {Journal}.  PMID:{Reference}"
        End Function
    End Class
End Namespace
