#Region "Microsoft.VisualBasic::c0d764193ff1bd6d2312668fc76bea42, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\LinkDB\Module.vb"

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
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel

Namespace Assembly.KEGG.DBGET.LinkDB

    Public Module Modules

        Public Const URL_KEGG_MODULES_ENTRY_PAGE As String = "http://www.genome.jp/dbget-bin/get_linkdb?-t+module+genome:{0}"
        Public Const SEPERATOR As String = "<pre>ID                   Definition"

        Public Function Download(speciesId As String) As [Module]()
            Dim pageContent As String = Strings.Split(String.Format(URL_KEGG_MODULES_ENTRY_PAGE, speciesId).GET, SEPERATOR).Last
            Dim Entries As String() = (From Match As Match
                                       In Regex.Matches(pageContent, "<a href="".+?"">", RegexOptions.Singleline + RegexOptions.IgnoreCase)
                                       Select Match.Value).ToArray
            Dim Modules As [Module]() = New [Module](Entries.Count - 1) {}

            For i As Integer = 0 To Modules.Length - 1
                Dim url = "http://www.genome.jp" & GetUrl(Entries(i))
                Modules(i) = [Module].Download(url)
            Next

            Return Modules
        End Function

        Public Function Download(speciesId As String, Export As String) As [Module]()
            Dim pageContent As String = Strings.Split(String.Format(URL_KEGG_MODULES_ENTRY_PAGE, speciesId).GET, SEPERATOR).Last
            Dim Entries As String() = (From Match As Match
                                       In Regex.Matches(pageContent, "<a href="".+?"">.+?</a>.+?$", RegexOptions.Multiline + RegexOptions.IgnoreCase)
                                       Select Match.Value).ToArray
            Dim Genes As KeyValuePairObject(Of KeyValuePair(Of String, String), KeyValuePair())() =
                New KeyValuePairObject(Of KeyValuePair(Of String, String), KeyValuePair())(Entries.Length - 2) {}
            Dim Downloader As New System.Net.WebClient()

            Entries = Entries.Take(Entries.Length - 1).ToArray

            If String.IsNullOrEmpty(Export) Then
                Export = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache) & "/Modules/"
            End If

            Call FileIO.FileSystem.CreateDirectory(Export)

            Dim ModuleList As List(Of [Module]) = New List(Of [Module])

            For i As Integer = 0 To Genes.Length - 1
                Dim Item As String = Entries(i)
                Dim Entry As String = Regex.Match(Item, ">.+?</a>").Value
                Entry = Mid(Entry, 2, Len(Entry) - 5)
                Dim Description As String = Strings.Split(Item, "</a>").Last.Trim
                Dim Url As String = String.Format(KEGGgenes.URL_MODULE_GENES, Entry)
                Dim ImageUrl = String.Format("http://www.genome.jp/tmp/pathway_thumbnail/{0}.png", Entry)

                Try
                    Dim ObjUrl = "http://www.genome.jp" & GetUrl(Item)
                    Dim SaveFile As String = String.Format("{0}/Webpages/{1}.html", Export, Entry)

                    Call ObjUrl.GET.SaveTo(SaveFile)
                    Call ModuleList.Add([Module].Download(SaveFile))
                    Call ModuleList.Last.GetXml.SaveTo(String.Format("{0}/{1}.xml", Export, Entry))
                    Call Downloader.DownloadFile(ImageUrl, String.Format("{0}/{1}.png", Export, Entry))
                Catch ex As Exception

                End Try

                Genes(i) = New KeyValuePairObject(Of KeyValuePair(Of String, String), KeyValuePair()) With {
                    .Key = New KeyValuePair(Of String, String)(Entry, Description),
                    .Value = KEGGgenes.Download(Url).ToArray
                }
            Next

            Return __createBriefModuleData(Genes)
        End Function

        Private Function __createBriefModuleData(Items As KeyValuePairObject(Of KeyValuePair(Of String, String), KeyValuePair())()) As [Module]()
            Dim LQuery = (From item In Items
                          Select New [Module] With {
                              .EntryId = item.Key.Key,
                              .Description = item.Key.Value,
                              .PathwayGenes = item.Value}).ToArray
            Return LQuery
        End Function

        Const HREF As String = "href="".+?"""

        Public Function GetUrl(href As String) As String
            href = Regex.Match(href, Modules.HREF).Value
            href = Mid(href, 7)
            href = Mid(href, 1, Len(href) - 1)
            Return href
        End Function
    End Module
End Namespace
