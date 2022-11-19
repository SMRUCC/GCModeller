#Region "Microsoft.VisualBasic::9a4ce385cc014d8a46e7ec8824783f6b, GCModeller\core\Bio.Assembly\Assembly\KEGG\Web\PathwayMapping.vb"

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

    '   Total Lines: 338
    '    Code Lines: 211
    ' Comment Lines: 86
    '   Blank Lines: 41
    '     File Size: 16.93 KB


    '     Module PathwayMapping
    ' 
    '         Function: CustomPathwayTable, DefaultKOTable, GetKOlist, (+3 Overloads) KOCatalog, ShowEnrichmentPathway
    ' 
    '         Sub: ColorPathway, (+2 Overloads) Reconstruct
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Namespace Assembly.KEGG.WebServices

    ''' <summary>
    ''' 需要程序处于联网状态
    ''' </summary>
    Public Module PathwayMapping

        Const yes$ = NameOf(yes)
        Const no$ = NameOf(no)

        ''' <summary>
        ''' ###### KEGG Mapper – Reconstruct Pathway
        ''' 
        ''' > http://www.genome.jp/kegg/tool/map_pathway.html
        ''' 
        ''' **Reconstruct Pathway** is a KEGG PATHWAY mapping tool that assists genome and metagenome annotations. 
        ''' The input data is a single gene list (for a single organism) or multiple gene lists (for multiple 
        ''' organisms) annotated with KEGG Orthology (KO) identifiers or K numbers. Each line of the gene list 
        ''' contains the user-defined gene identifier followed by, if any, the assigned K number. The mapping is 
        ''' performed through the K numbers against the KEGG reference pathways. 
        ''' </summary>
        ''' <param name="list$">Enter gene list with KO annotation(line format:  ``geneID\tKO``)</param>
        ''' <param name="globalmap">Include global/overview maps</param>
        <Extension>
        Public Sub Reconstruct(list As IEnumerable(Of NamedValue(Of String)), Optional globalmap As Boolean = True, Optional work$ = "./")
            Call Reconstruct(list.Select(Function(x) $"{x.Name}{ASCII.TAB}{x.Value}").JoinBy(ASCII.LF), globalmap, work)
        End Sub

        ''' <summary>
        ''' ###### KEGG Mapper – Reconstruct Pathway
        ''' 
        ''' > http://www.genome.jp/kegg/tool/map_pathway.html
        ''' 
        ''' **Reconstruct Pathway** is a KEGG PATHWAY mapping tool that assists genome and metagenome annotations. 
        ''' The input data is a single gene list (for a single organism) or multiple gene lists (for multiple 
        ''' organisms) annotated with KEGG Orthology (KO) identifiers or K numbers. Each line of the gene list 
        ''' contains the user-defined gene identifier followed by, if any, the assigned K number. The mapping is 
        ''' performed through the K numbers against the KEGG reference pathways. 
        ''' </summary>
        ''' <param name="list$">Enter gene list with KO annotation</param>
        ''' <param name="globalmap">Include global/overview maps</param>
        Public Sub Reconstruct(list$, Optional globalmap As Boolean = True, Optional work$ = "./")
            If list.FileExists(True) Then
                list = list.ReadAllText
            End If

            Dim args As New NameValueCollection

            Call $"Reconstruct Pathway for {list.LineTokens.Length} genes...".__DEBUG_ECHO
            Call args.Add(NameOf(globalmap), If(globalmap, yes, no))
            Call args.Add("submit", "Exec")
            Call args.Add("unclassified", list)

            Dim htext = Pathway.LoadFromResource.ToDictionary(Function(x) x.EntryId)
            Dim html$ = "http://www.genome.jp/kegg-bin/find_pathway_object".POST(args, , "http://www.genome.jp/kegg/tool/map_pathway.html").html

            Const mapLink$ = "<a href=""/kegg-bin/show_pathway[^""]+"" target=""_map"">"

            Dim links$() = Regex.Matches(html, mapLink, RegexICSng).ToArray
            Dim img$
            Dim id$

            For Each link$ In links

                link = link.href
                id = Regex.Match(link, "map\d+", RegexICSng).Value
                id = Regex.Match(id, "\d+").Value

                Dim path$ = $"{work}/{htext(id).GetPathCategory}/map{id}.png"

                If Not path.FileLength > 5 Then
                    html = ("http://www.genome.jp" & link).GET
                    img = Regex.Match(html, "src=""[^""]+map.+?\.png""", RegexICSng).Value
                    img = "http://www.genome.jp" & img.src

                    Call img.DownloadFile(path)
                    Call Thread.Sleep(1000)
                End If
            Next
        End Sub

        ''' <summary>
        ''' **Search&amp;Color Pathway** is an advanced version of the KEGG pathway mapping tool, where given objects 
        ''' (genes, proteins, compounds, glycans, reactions, drugs, etc.) are searched against KEGG pathway 
        ''' maps and found objects are marked in any background and foreground colors (bgcolor and fgcolor). 
        ''' The objects in different types of pathway maps are specified by the following KEGG identifiers 
        ''' and aliases. 
        ''' 
        ''' > http://www.kegg.jp/kegg/tool/map_pathway2.html
        ''' </summary>
        ''' <param name="list$"></param>
        ''' <param name="work$"></param>
        Public Sub ColorPathway(list$,
                                Optional target As Boolean = False,
                                Optional reference As Boolean = True,
                                Optional warning As Boolean = False,
                                Optional all As Boolean = False,
                                Optional work$ = "./")

            If list.FileExists(True) Then
                list = list.ReadAllText
            End If

            Dim args As New NameValueCollection

            Call $"Reconstruct Pathway for {list.LineTokens.Length} genes...".__DEBUG_ECHO
            Call args.Add("org", "ko")
            Call args.Add("other_dbs", "")
            Call args.Add("unclassified", list)
            Call args.Add("default", "pink")
            Call args.Add(NameOf(target), If(target, "alias", ""))
            Call args.Add(NameOf(reference), If(reference, "white", ""))
            Call args.Add(NameOf(warning), If(warning, "yes", ""))
            Call args.Add(NameOf(all), If(all, "1", ""))
            Call args.Add("submit", "Exec")

            Dim html = "http://www.kegg.jp/kegg-bin/color_pathway_object".POST(args, Referer:="http://www.kegg.jp/kegg/tool/map_pathway2.html").html

            Const mapLinks$ = "href=""/kegg-bin/show_pathway\?.+?/ko\d+\.args"" target=""_map"""
            Const imgLink$ = "src=""/tmp/mark_pathway.+?/ko\d+.*?\.png"""

            Dim links$() = Regex.Matches(html, mapLinks, RegexICSng).ToArray(AddressOf href)
            Dim img$
            Dim Ko As Dictionary(Of String, Pathway) = Pathway.LoadFromResource.ToDictionary(Function(x) x.EntryId)
            Dim id$

            For Each link As String In links.Select(Function(url) "http://www.kegg.jp" & url)

                id = Regex.Match(link, "ko\d+\.args", RegexICSng).Value
                id = Regex.Match(id, "\d+").Value

                Dim path$

                If Ko.ContainsKey(id) Then
                    path = $"{work}/{Ko(id).GetPathCategory}/ko{id}.png"
                Else
                    path = work & $"/unknown/ko{id}.png"
                    Call path.Warning
                End If

                If Not path.FileLength > 5 Then
                    html = link.GET

                    ' src="/tmp/mark_pathway148425218533695/ko01100_0.3533695.png"
                    img = Regex.Match(html, imgLink, RegexICSng).Value
                    img = "http://www.kegg.jp" & img.src

                    Call img.DownloadFile(path)
                    Call Thread.Sleep(1000)
                End If
            Next
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="geneIDs"></param>
        ''' <param name="DIR$">
        ''' 文件夹之中存放着以基因号为文件名的<see cref="KEGG.DBGET.bGetObject.SSDB.OrthologREST"/>Xml
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function GetKOlist(geneIDs As IEnumerable(Of String), DIR$) As NamedValue(Of String)()
            Dim out As New List(Of NamedValue(Of String))

            For Each id As String In geneIDs
                Dim path$ = DIR & $"/{id}.xml"
                Dim ko As OrthologREST = path.LoadXml(Of OrthologREST)

                For Each hit As SShit In ko.Orthologs
                    If Not hit.KO.Value.StringEmpty Then
                        out += New NamedValue(Of String) With {
                            .Name = id,
                            .Value = hit.KO.Value
                        }
                        Call $"[{hit.KO.Value}] {id}".__DEBUG_ECHO
                        Exit For
                    End If
                Next
            Next

            Return out
        End Function

        ''' <summary>
        ''' KEGG直系同源分类统计
        ''' </summary>
        ''' <param name="KO_maps">``{geneID -> KO}`` map data collection.</param>
        ''' <returns></returns>
        <Extension>
        Public Function KOCatalog(KO_maps As IEnumerable(Of NamedValue(Of String)), KO_htext As Dictionary(Of String, BriteHText)) As NamedValue(Of Dictionary(Of String, String))()
            Dim pathways = LinqAPI.Exec(Of NamedValue(Of Dictionary(Of String, String))) _
 _
                () <= From x As NamedValue(Of String)
                      In KO_maps
                      Where KO_htext.ContainsKey(x.Value)
                      Let path = KO_htext(x.Value)
                      Let subcate = path.Parent
                      Let cate = subcate.Parent
                      Let cls = cate.Parent
                      Let catalog As Dictionary(Of String, String) =
                          New Dictionary(Of String, String) From {
                              {"KO", x.Value},
                              {"Category", cate.description},
                              {"Class", cls.description},
                              {"SubCatalog", subcate.description},
                              {"Function", path.description}
                      }
                      Select New NamedValue(Of Dictionary(Of String, String)) With {
                          .Name = x.Name,
                          .Value = catalog
                      }

            Return pathways
        End Function

        ''' <summary>
        ''' KEGG直系同源分类统计
        ''' </summary>
        ''' <param name="KO_maps">``{geneID -> KO}`` map data collection.</param>
        ''' <returns></returns>
        <Extension>
        Public Function KOCatalog(KO_maps As IEnumerable(Of NamedValue(Of String))) As NamedValue(Of Dictionary(Of String, String))()
            Dim ko_htext As Dictionary(Of String, BriteHText) = DefaultKOTable()
            Return KO_maps.KOCatalog(ko_htext)
        End Function

        ''' <summary>
        ''' Build default from <see cref="BriteHText.Load_ko00001"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function DefaultKOTable() As Dictionary(Of String, BriteHText)
            Dim KO_htext = BriteHText _
                .Load_ko00001 _
                .EnumerateEntries _
                .Where(Function(x) Not x.entryID Is Nothing) _
                .GroupBy(Function(x) x.entryID) _
                .ToDictionary(Function(x) x.Key,
                              Function(x)
                                  Return x.First
                              End Function)
            Return KO_htext
        End Function

        ''' <summary>
        ''' Custom KO classification set can be download from: http://www.kegg.jp/kegg-bin/get_htext?ko00001.keg. 
        ''' You can replace the %s mark Using kegg organism code In url example As: 
        ''' http://www.kegg.jp/kegg-bin/download_htext?htext=%s00001&amp;format=htext&amp;filedir= for download the custom KO classification set.
        ''' </summary>
        ''' <param name="KO_maps"></param>
        ''' <param name="ko00001$">User custom classification database</param>
        ''' <returns></returns>
        <Extension>
        Public Function KOCatalog(KO_maps As IEnumerable(Of NamedValue(Of String)), ko00001$) As NamedValue(Of Dictionary(Of String, String))()
            Dim KO_htext = BriteHTextParser _
                .Load(ko00001.SolveStream) _
                .EnumerateEntries _
                .Where(Function(x) Not x.entryID Is Nothing) _
                .Select(Function(x)
                            Return New With {
                                .EntryID = x.description _
                                    .Match("\sK\d{5}\s") _
                                    .Trim,
                                .KO = x
                            }
                        End Function) _
                .Where(Function(x) Not x.EntryID.StringEmpty) _
                .GroupBy(Function(x) x.EntryID) _
                .ToDictionary(Function(x) x.Key,
                              Function(x)
                                  Return x.First.KO
                              End Function)
            Return KO_maps.KOCatalog(KO_htext)
        End Function

        ''' <summary>
        ''' http://www.kegg.jp/kegg-bin/get_htext?ko00001.keg
        ''' </summary>
        ''' <param name="ko00001$"></param>
        ''' <returns></returns>
        Public Function CustomPathwayTable(ko00001$) As Dictionary(Of String, BriteHText)
            Dim KO_htext = BriteHTextParser _
               .Load(ko00001.SolveStream) _
               .EnumerateEntries _
               .Select(Function(x)
                           With x.parent
                               If .ByRef Is Nothing Then
                                   Return Nothing
                               Else
                                   Dim PATH As String = .classLabel _
                                       .Match("PATH[:].+\d+") _
                                       .Trim("]"c) _
                                       .GetTagValue(":") _
                                       .Value
                                   Return New NamedValue(Of BriteHText)(PATH, .ByRef)
                               End If
                           End With
                       End Function) _
               .Where(Function(x) Not x.Name.StringEmpty) _
               .GroupBy(Function(x) x.Name) _
               .ToDictionary(Function(x) x.Key,
                             Function(x)
                                 Return x.First.Value
                             End Function)
            Return KO_htext
        End Function

        Public Const KEGG_show_pathway$ = "http://www.genome.jp/kegg-bin/show_pathway?"

        ''' <summary>
        ''' url can be encoding by <see cref="URLEncoder"/>
        ''' </summary>
        ''' <param name="url$">
        ''' Example as: http://www.genome.jp/kegg-bin/show_pathway?aor01100/aor:AOR_1_348154%09red/aor:AOR_1_1018164%09red/aor:AOR_1_46074%09red/aor:AOR_1_1132054%09red/aor:AOR_1_1796154%09red/aor:AOR_1_724024%09red/aor:AOR_1_980074%09red/aor:AOR_1_132064%09red/aor:AOR_1_936184%09red/aor:AOR_1_750024%09red/aor:AOR_1_858084%09red/aor:AOR_1_920184%09red/aor:AOR_1_1152144%09red/aor:AOR_1_1464054%09red/aor:AOR_1_506014%09red/aor:AOR_1_26114%09red/aor:AOR_1_654074%09red/aor:AOR_1_336094%09red/aor:AOR_1_700094%09red/aor:AOR_1_2326154%09red/aor:AOR_1_448144%09red/aor:AOR_1_1152014%09red/aor:AOR_1_964164%09red/aor:AOR_1_556094%09red/aor:AOR_1_76084%09red/aor:AOR_1_2070174%09red/aor:AOR_1_664034%09red/aor:AOR_1_890144%09red/aor:AOR_1_1888174%09red/aor:AOR_1_2198154%09red/aor:AOR_1_598144%09red/aor:AOR_1_1676014%09red/aor:AOR_1_1160154%09red/aor:AOR_1_362184%09red/aor:AOR_1_236174%09red/aor:AOR_1_514024%09red/aor:AOR_1_1554054%09red/aor:AOR_1_2706174%09red/aor:AOR_1_1692144%09red/aor:AOR_1_1046084%09red/aor:AOR_1_340154%09red/aor:AOR_1_968134%09red/aor:AOR_1_562034%09red/aor:AOR_1_1214024%09red/aor:AOR_1_1124054%09red/aor:AOR_1_988014%09red/aor:AOR_1_780164%09red/aor:AOR_1_622134%09red/aor:AOR_1_284154%09red/aor:AOR_1_968024%09red/aor:AOR_1_1062184%09red/aor:AOR_1_1274164%09red/aor:AOR_1_1272164%09red/aor:AOR_1_1114084%09red/aor:AOR_1_990184%09red/aor:AOR_1_2146154%09red/aor:AOR_1_1074144%09red/aor:AOR_1_1056134%09red/aor:AOR_1_504114%09red/aor:AOR_1_560024%09red/aor:AOR_1_462144%09red/aor:AOR_1_858054%09red/aor:AOR_1_2842174%09red
        ''' 
        ''' Which this enrichment result url can be obtained from KOBAS KEGG enrichment analysis.
        ''' </param>
        ''' <param name="save">File name</param>
        ''' <returns></returns>
        Public Function ShowEnrichmentPathway(url$, save$) As Boolean
            Dim html$ = url.GET
            Dim img = Regex.Match(html, "<img src=""/tmp/mark_pathway[^""]+"" name=""pathwayimage"".+?/>", RegexICSng).Value
            img = "http://www.genome.jp/" & img.src
            Return img.DownloadFile(save)
        End Function
    End Module
End Namespace
