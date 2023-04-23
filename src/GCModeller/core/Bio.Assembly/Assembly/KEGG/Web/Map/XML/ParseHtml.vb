#Region "Microsoft.VisualBasic::399a350f1a606e540781906f38670a31, GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\Map\XML\ParseHtml.vb"

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

'   Total Lines: 79
'    Code Lines: 56
' Comment Lines: 11
'   Blank Lines: 12
'     File Size: 3.01 KB


'     Module ParseHtmlExtensions
' 
'         Function: GetEntryInfo, ParseHTML
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports dirFs = Microsoft.VisualBasic.FileIO.Directory
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.WebServices

    Module ParseHtmlExtensions

        Const data$ = "<map .*name[=]""mapdata"".*>.+?</map>"

        '<html>
        '<!---
        'ENTRY       map01100
        'DEFINITION  Metabolic pathways - Reference pathway
        '--->

        Private Function GetEntryInfo(html As String) As NamedValue(Of String)
            Dim comment$ = html.GetHtmlComments.First
            Dim text = comment.LineTokens
            Dim entry = text(1).GetTagValue(" ", trim:=True).Value
            Dim definition = text(2).GetTagValue(" ", trim:=True).Value

            Return New NamedValue(Of String) With {
                .Name = entry,
                .Value = definition
            }
        End Function

        Private Function parseShapes(map As String) As Area()
            Dim areas = map.LineTokens.Select(Function(si) si.Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF)).ToArray
            Dim shapes = areas _
                .Where(Function(si) si.ToLower.StartsWith("<area")) _
                .Select(AddressOf Area.Parse) _
                .ToArray

            Return shapes
        End Function

        Private Function HttpGetPathwayMapImage(id As String, fs As IFileSystemEnvironment) As String
            Dim url As String = $"https://rest.kegg.jp/get/{id}/image"
            Dim img As String
            Dim cachePath As String = $"/images/{id}.png"
            Dim tmp$ = TempFileSystem.GetAppSysTempFile(".png", "mapFetchs_" & App.PID.ToHexString, "kegg_map_")

            If Not fs.FileExists(cachePath) Then
                ' download pathway map image to local cache
                Call url.DownloadFile(save:=tmp)
                Call fs.DeleteFile(cachePath)

                Using file As Stream = fs.OpenFile(cachePath, FileMode.CreateNew, FileAccess.Write)
                    Dim buf As Byte() = tmp.ReadBinary

                    Call file.Write(buf, Scan0, buf.Length)
                    Call file.Flush()
                End Using

                Call fs.Flush()
            End If

            img = fs.OpenFile(cachePath, FileMode.Open).LoadImage(throwEx:=False).ToBase64String
            img = FastaSeq.SequenceLineBreak(200, img)
            img = vbLf & img _
                .LineTokens _
                .Select(Function(s) New String(" ", 4) & s) _
                .JoinBy(ASCII.LF) & vbLf

            Return img
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="html"></param>
        ''' <param name="url">The original source url of this map data</param>
        ''' <returns></returns>
        Public Function ParseHTML(html As String, Optional url$ = Nothing, Optional fs As IFileSystemEnvironment = Nothing) As Map
            Dim map$ = r.Match(html, data, RegexICSng).Value
            Dim info As NamedValue(Of String) = GetEntryInfo(html)
            Dim shapes As Area() = parseShapes(map)
            Dim desc As String = r.Match(html, "<div id[=]""description"".+?</div>", RegexICSng) _
                .Value _
                .GetValue _
                .Trim(" "c, ASCII.TAB, ASCII.CR, ASCII.LF)
            Dim cache_temp As String = App.ProductSharedTemp & "/kegg_maps/"

            If fs Is Nothing Then
                fs = dirFs.FromLocalFileSystem(cache_temp)
            End If

            Dim img As String = HttpGetPathwayMapImage(info.Name, fs)

            Return New Map With {
                .PathwayImage = img,
                .shapes = shapes,
                .EntryId = info.Name,
                .name = info.Value,
                .URL = url,
                .description = desc
            }
        End Function
    End Module
End Namespace
