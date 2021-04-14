﻿#Region "Microsoft.VisualBasic::89305fcac0aba7b996d6e7fef254039c, core\Bio.Assembly\Assembly\KEGG\Web\Map\XML\ParseHtml.vb"

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

'     Module ParseHtmlExtensions
' 
'         Function: GetEntryInfo, ParseHTML
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.WebServices

    Module ParseHtmlExtensions

        Const data$ = "<map name=""mapdata"">.+?</map>"

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

        Const mapImageURL$ = "<img src="".+?"" name=""pathwayimage"" usemap=""#mapdata"".+?/>"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="html"></param>
        ''' <param name="url">The original source url of this map data</param>
        ''' <returns></returns>
        Public Function ParseHTML(html As String, Optional url$ = Nothing) As Map
            Dim map$ = r.Match(html, data, RegexICSng).Value
            Dim areas = map.LineTokens.Skip(1).ToArray
            Dim img = r.Match(html, mapImageURL, RegexICSng).Value
            Dim tmp$ = TempFileSystem.GetAppSysTempFile(".png", "mapFetchs_" & App.PID, "kegg_map_")
            Dim info As NamedValue(Of String) = GetEntryInfo(html)
            Dim shapes = areas _
                .Take(areas.Length - 1) _
                .Select(AddressOf Area.Parse) _
                .ToArray

            With "http://www.genome.jp/" & img.src
                Call .DownloadFile(tmp)

                If (Not tmp.FileExists) OrElse tmp.LoadImage(throwEx:=False) Is Nothing Then
                    img = $"https://www.genome.jp/kegg/pathway/map/{info.Name}.png"
                    img.DownloadFile(tmp)
                End If

                img = tmp.LoadImage.ToBase64String
                img = FastaSeq.SequenceLineBreak(200, img)
                img = vbLf & img _
                    .LineTokens _
                    .Select(Function(s) New String(" ", 4) & s) _
                    .JoinBy(ASCII.LF) & vbLf
            End With

            Return New Map With {
                .PathwayImage = img,
                .shapes = shapes,
                .id = info.Name,
                .Name = info.Value,
                .URL = url
            }
        End Function
    End Module
End Namespace
