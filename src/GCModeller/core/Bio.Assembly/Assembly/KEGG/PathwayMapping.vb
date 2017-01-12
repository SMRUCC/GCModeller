Imports System.Collections.Specialized
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.Text.HtmlParser
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
                    html = ("http://www.genome.jp" & link.href).GET
                    img = Regex.Match(html, "src=""[^""]+map.+?\.png""", RegexICSng).Value
                    img = "http://www.genome.jp" & img.ImageSource

                    Call img.DownloadFile(path)
                    Call Thread.Sleep(1000)
                End If
            Next
        End Sub
    End Module
End Namespace