﻿#Region "Microsoft.VisualBasic::d4825c0654cabb0bd68152c6216d1403, Bio.Assembly\Assembly\KEGG\Web\Map\Map.vb"

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

    '     Class Map
    ' 
    '         Properties: Areas, ID, Name, PathwayImage, URL
    ' 
    '         Function: GetEntryInfo, GetImage, GetMembers, ParseHTML, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.WebServices

    <XmlRoot("Map", [Namespace]:="http://GCModeller.org/core/KEGG/KGML_map.xsd")>
    Public Class Map : Inherits XmlDataModel
        Implements INamedValue

        <XmlAttribute>
        Public Property ID As String Implements IKeyedEntity(Of String).Key

        <XmlElement("name")>
        Public Property Name As String
        Public Property URL As String

        ''' <summary>
        ''' 节点的位置，在这里面包含有代谢物(小圆圈)以及基因(方块)的位置定义
        ''' </summary>
        ''' <returns></returns>
        <XmlArray("shapes")>
        Public Property Areas As Area()

        ''' <summary>
        ''' base64 image
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("KEGGmap")>
        Public Property PathwayImage As String

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetMembers() As String()
            Return Areas _
                .Select(Function(a) a.IDVector) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End Function

        Public Function GetImage() As Image
            Dim lines$() = PathwayImage _
                .Trim(" "c, ASCII.LF, ASCII.CR) _
                .LineTokens _
                .Select(AddressOf Trim) _
                .ToArray
            Dim base64$ = String.Join("", lines)
            Return base64.GetImage
        End Function

        Public Overrides Function ToString() As String
            Return Areas.GetJson
        End Function

        Const data$ = "<map name=""mapdata"">.+?</map>"

        '<html>
        '<!---
        'ENTRY       map01100
        'DEFINITION  Metabolic pathways - Reference pathway
        '--->

        Private Shared Function GetEntryInfo(html As String) As NamedValue(Of String)
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

        Public Shared Function ParseHTML(url$) As Map
            Dim html$ = url.GET
            Dim map$ = r.Match(html, data, RegexICSng).Value
            Dim areas = map.LineTokens.Skip(1).ToArray
            Dim img = r.Match(html, mapImageURL, RegexICSng).Value
            Dim tmp$ = App.GetAppSysTempFile
            Dim shapes = areas _
                .Take(areas.Length - 1) _
                .Select(AddressOf Area.Parse) _
                .ToArray

            With "http://www.genome.jp/" & img.src
                Call .DownloadFile(tmp)

                img = tmp.LoadImage.ToBase64String
                img = FastaSeq.SequenceLineBreak(200, img)
                img = vbLf & img _
                    .LineTokens _
                    .Select(Function(s) New String(" ", 4) & s) _
                    .JoinBy(ASCII.LF) & vbLf
            End With

            Dim info As NamedValue(Of String) = GetEntryInfo(html)

            Return New Map With {
                .PathwayImage = img,
                .Areas = shapes,
                .ID = info.Name,
                .Name = info.Value,
                .URL = url
            }
        End Function
    End Class
End Namespace
