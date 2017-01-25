#Region "Microsoft.VisualBasic::d93291841927ceb89bb188d1f894290e, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\PathwayMapping.vb"

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

Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.HtmlParser
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

            Call $"Reconstruct Pathway for {list.lTokens.Length} genes...".__DEBUG_ECHO
            Call args.Add(NameOf(globalmap), If(globalmap, yes, no))
            Call args.Add("submit", "Exec")
            Call args.Add("unclassified", list)

            Dim htext = Pathway.LoadFromResource.ToDictionary(Function(x) x.EntryId)
            Dim html$ = "http://www.genome.jp/kegg-bin/find_pathway_object".POST(args, "http://www.genome.jp/kegg/tool/map_pathway.html")

            Const mapLink$ = "<a href=""/kegg-bin/show_pathway[^""]+"" target=""_map"">"

            Dim links$() = Regex.Matches(html, mapLink, RegexICSng).ToArray
            Dim img$
            Dim id$

            For Each link$ In links

                link = link.href
                id = Regex.Match(link, "map\d+", RegexICSng).Value
                id = Regex.Match(id, "\d+").Value

                Dim path$ = Pathway.CombineDIR(htext(id), work) & $"/map{id}.png"

                If Not path.FileLength > 5 Then
                    html = ("http://www.genome.jp" & link).GET
                    img = Regex.Match(html, "src=""[^""]+map.+?\.png""", RegexICSng).Value
                    img = "http://www.genome.jp" & img.ImageSource

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

            Call $"Reconstruct Pathway for {list.lTokens.Length} genes...".__DEBUG_ECHO
            Call args.Add("org", "ko")
            Call args.Add("other_dbs", "")
            Call args.Add("unclassified", list)
            Call args.Add("default", "pink")
            Call args.Add(NameOf(target), If(target, "alias", ""))
            Call args.Add(NameOf(reference), If(reference, "white", ""))
            Call args.Add(NameOf(warning), If(warning, "yes", ""))
            Call args.Add(NameOf(all), If(all, 1, ""))
            Call args.Add("submit", "Exec")

            Dim html = "http://www.kegg.jp/kegg-bin/color_pathway_object".POST(args, Referer:="http://www.kegg.jp/kegg/tool/map_pathway2.html")

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
                    path = Pathway.CombineDIR(Ko(id), work) & $"/ko{id}.png"
                Else
                    path = work & $"/unknown/ko{id}.png"
                    Call path.Warning
                End If

                If Not path.FileLength > 5 Then
                    html = link.GET

                    ' src="/tmp/mark_pathway148425218533695/ko01100_0.3533695.png"
                    img = Regex.Match(html, imgLink, RegexICSng).Value
                    img = "http://www.kegg.jp" & img.ImageSource

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
                    If Not hit.KO.Value.IsBlank Then
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

        <Extension>
        Public Function KOCatalog(KO_maps As IEnumerable(Of NamedValue(Of String))) As NamedValue(Of Dictionary(Of String, String))()
            Dim KO_htext As Dictionary(Of String, BriteHText) = BriteHText _
                .Load_ko00001 _
                .EnumerateEntries _
                .Where(Function(x) Not x.EntryId Is Nothing) _
                .GroupBy(Function(x) x.EntryId) _
                .ToDictionary(Function(x) x.Key,
                              Function(x) x.First)
            Dim pathways = LinqAPI.Exec(Of NamedValue(Of Dictionary(Of String, String))) <=
 _
                From x As NamedValue(Of String)
                In KO_maps
                Where KO_htext.ContainsKey(x.Value)
                Let path = KO_htext(x.Value)
                Let subcate = path.Parent
                Let cate = subcate.Parent
                Let cls = cate.Parent
                Let catalog As Dictionary(Of String, String) =
                    New Dictionary(Of String, String) From {
                        {"KO", x.Value},
                        {"Category", cate.Description},
                        {"Class", cls.Description},
                        {"SubCatalog", subcate.Description},
                        {"Function", path.Description}
                }
                Select New NamedValue(Of Dictionary(Of String, String)) With {
                    .Name = x.Name,
                    .Value = catalog
                }

            Return pathways
        End Function
    End Module
End Namespace
