#Region "Microsoft.VisualBasic::27afe8e233f3742567a9c6ac0b3123c1, meme_suite\MEME.DocParser\MEME\HtmlParser\ParserAPI.vb"

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

    '     Module ParserAPI
    ' 
    '         Function: CreateDictionary, LoadDocument
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection

Namespace DocumentFormat.MEME.HTML

    <[Namespace]("MEME.Parser.HTML")>
    Public Module ParserAPI

        Public Const Seperator As String = "<!--++++++++++++++++++++++++++++FINISHED DATA++++++++++++++++++++++++++++++++-->"

        ''' <summary>
        ''' <see cref="Motif.MotifId"></see>和<see cref="MEMEOutput.MatchedMotif"></see>二者的值是一样的，故而可以直接使用<see cref="MEMEOutput.MatchedMotif"></see>进行查询
        ''' </summary>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("generate.motif_entry")>
        Public Function CreateDictionary(data As IEnumerable(Of MEMEHtml)) As Dictionary(Of String, Motif)
            Dim DictEntries As Dictionary(Of String, Motif) = New Dictionary(Of String, Motif)
            For Each Entry As MEMEHtml In data
                For Each Motif In (From item In Entry.Motifs Select New KeyValuePair(Of String, Motif)(item.MotifId(Entry.ObjectId), item))
                    Call DictEntries.Add(Motif.Key, Motif.Value)
                Next
            Next

            Return DictEntries
        End Function

        Const Line As String = "<h2 class=""mainh"">Motif \d+</h2></td>.+?<p>Time .+? secs\.</p>"

        <ExportAPI("meme.load_html")>
        Public Function LoadDocument(url As String) As MEMEHtml
            Dim pageContent As String = Strings.Split(url.GET(), Seperator).Last
            Dim strMotifs As String() = Regex.Matches(pageContent, Line, RegexICSng).ToArray
            Dim Motifs = (From strValue As String In strMotifs Select Motif.TryParse(strValue)).ToArray
            Dim ObjectId As String = ""

            If FileIO.FileSystem.FileExists(url) Then
                ObjectId = FileIO.FileSystem.GetFileInfo(url).DirectoryName
                ObjectId = FileIO.FileSystem.GetDirectoryInfo(ObjectId).Name
                Dim p As Integer = InStrRev(ObjectId, ".") - 1
                If p > 0 Then ObjectId = Mid(ObjectId, 1, p)
            End If

            Return New MEMEHtml With {
                .Motifs = Motifs,
                .ObjectId = ObjectId
            }
        End Function
    End Module
End Namespace
