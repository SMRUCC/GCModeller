#Region "Microsoft.VisualBasic::a0428a61937522aa1dec1b9638382f83, CLI_tools\KEGG\CLI\CLI.vb"

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

    ' Module CLI
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Blastn, BuildKEGGOrthology, BuildKORepository, CreateTABLE, Download16SRNA
    '               DownloadMappedSequence, DownloadOrthologs, DownloadReferenceMap, DownloadSequence, FunctionAnalysis
    '               GetFastaBySp, ImportsDb, ImportsKODatabase, IndexSubMatch, QueryGenes
    '               QueryOrthology
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.ReferenceMap
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.KEGG
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

<Cite(Title:="KAAS: an automatic genome annotation and pathway reconstruction server", PubMed:=17526522,
      DOI:="10.1093/nar/gkm321",
      Abstract:="The number of complete and draft genomes is rapidly growing in recent years, and it has become increasingly important to automate the identification of functional properties and biological roles of genes in these genomes. 
In the KEGG database, genes in complete genomes are annotated with the KEGG orthology (KO) identifiers, or the K numbers, based on the best hit information using Smith-Waterman scores as well as by the manual curation. 
Each K number represents an ortholog group of genes, and it is directly linked to an object in the KEGG pathway map or the BRITE functional hierarchy. Here, we have developed a web-based server called KAAS (KEGG Automatic Annotation Server: http://www.genome.jp/kegg/kaas/) i.e. an implementation of a rapid method to automatically assign K numbers to genes in the genome, enabling reconstruction of KEGG pathways and BRITE hierarchies. 
The method is based on sequence similarities, bi-directional best hit information and some heuristics, and has achieved a high degree of accuracy when compared with the manually curated KEGG GENES database.",
      Authors:="Moriya, Y.
Itoh, M.
Okuda, S.
Yoshizawa, A. C.
Kanehisa, M.",
      AuthorAddress:="Bioinformatics Center, Institute for Chemical Research, Kyoto University, Gokasho, Uji, Kyoto 611-0011, Japan.",
      Keywords:="Animals
Artificial Intelligence
Automation
Chromosome Mapping/*methods
Computational Biology/*methods
Database Management Systems
Documentation/*methods
*Genome
Humans
Information Storage and Retrieval/methods
Internet
Proteome/*classification/*metabolism
Sequence Analysis/*methods
Signal Transduction/*physiology
*Vocabulary, Controlled",
      Pages:="W182-5",
      Journal:="Nucleic acids research",
      Issue:="Web Server issue", Year:=2007, Volume:=35,
      ISSN:="1362-4962 (Electronic);
0305-1048 (Linking)")>
<Cite(Title:="KEGG for integration and interpretation of large-scale molecular data sets", Year:=2012, Volume:=40,
      Issue:="Database issue",
      DOI:="10.1093/nar/gkr988", Journal:="Nucleic Acids Res",
      Pages:="D109-14", Authors:="Kanehisa, M.
Goto, S.
Sato, Y.
Furumichi, M.
Tanabe, M.",
      Abstract:="Kyoto Encyclopedia of Genes and Genomes (KEGG, http://www.genome.jp/kegg/ or http://www.kegg.jp/) is a database resource that integrates genomic, chemical and systemic functional information. 
In particular, gene catalogs from completely sequenced genomes are linked to higher-level systemic functions of the cell, the organism and the ecosystem. Major efforts have been undertaken to manually create a knowledge base for such systemic functions by capturing and organizing experimental knowledge in computable forms; namely, in the forms of KEGG pathway maps, BRITE functional hierarchies and KEGG modules. 
Continuous efforts have also been made to develop and improve the cross-species annotation procedure for linking genomes to the molecular networks through the KEGG Orthology system. Here we report KEGG Mapper, a collection of tools for KEGG PATHWAY, BRITE and MODULE mapping, enabling integration and interpretation of large-scale data sets. 
We also report a variant of the KEGG mapping procedure to extend the knowledge base, where different types of data and knowledge, such as disease genes and drug targets, are integrated as part of the KEGG molecular networks. 
Finally, we describe recent enhancements to the KEGG content, especially the incorporation of disease and drug information used in practice and in society, to support translational bioinformatics.",
      AuthorAddress:="Bioinformatics Center, Institute for Chemical Research, Kyoto University, Uji, Kyoto 611-0011, Japan. kanehisa@kuicr.kyoto-u.ac.jp",
      Keywords:="Computational Biology
*Databases, Factual
Disease
Genomics
Humans
Knowledge Bases
Molecular Sequence Annotation
Pharmacological Phenomena
Software
Systems Integration",
      PubMed:=22080510)>
<Cite(Title:="KEGG: Kyoto Encyclopedia of Genes and Genomes",
      Abstract:="KEGG (Kyoto Encyclopedia of Genes and Genomes) is a knowledge base for systematic analysis of gene functions, linking genomic information with higher order functional information. 
The genomic information is stored in the GENES database, which is a collection of gene catalogs for all the completely sequenced genomes and some partial genomes with up-to-dateannotation of gene functions. 
The higher order functional information is stored in the PATHWAY database, which contains graphical representations of cellular processes, such as metabolism, membrane transport, signal transduction and cell cycle. 
The PATHWAY database is supplemented by a set of ortholog group tables for the information about conserved subpathways (pathway motifs), which are often encoded by positionally coupled genes on the chromosome and which are especially useful in predicting gene functions.
A third database in KEGG is LIGAND for the information about chemical compounds, enzyme molecules and enzymatic reactions. KEGG provides Java graphics tools for browsing genome maps, comparing two genomemaps and manipulating expression maps, as well as computational tools for sequence comparison,
graph comparison and path computation. The KEGG databases are daily updated and made freely available (http://www.genome.ad.jp/kegg/).",
      AuthorAddress:="Tel: +81774383270; Fax: +81774383269; Email: kanehisa@kuicr.kyoto-u.ac.jp",
      Authors:="Minoru Kanehisa,
Susumu Goto", Year:=2000, Volume:=28, Issue:="1",
      Journal:="Nucleic Acids Research", Pages:="27-30",
      StartPage:=27,
      URL:="http://www.genome.ad.jp/kegg/")>
<Package("KEGG.WebServices.CLI",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Category:=APICategories.CLI_MAN,
                  Url:="http://www.kegg.jp/",
                  Description:="KEGG web services API tools.")>
<CLI> Module CLI

    <ExportAPI("/blastn")>
    <Usage("/blastn /query <query.fasta> [/out <outDIR>]")>
    <Description("Blastn analysis of your DNA sequence on KEGG server for the functional analysis.")>
    Public Function Blastn(args As CommandLine) As Integer
        Dim queryFile As String = args("/query")
        Dim out As String = args.GetValue("/out", queryFile.TrimSuffix)
        Dim query As New FASTA.FastaFile(queryFile)

        For Each seq As FastaSeq In query
            Dim path As String = $"{out}/{seq.Title.NormalizePathString(False)}.csv"
            Dim result = Global.KEGG_tools.Blastn.Submit(seq)
            Call result.SaveTo(path)
        Next

        Return 0
    End Function

    <ExportAPI("/Download.Ortholog")>
    <Description("Downloads the KEGG gene ortholog annotation data from the web server.")>
    <Usage("/Download.Ortholog -i <gene_list_file.txt/gbk> -export <exportedDIR> [/gbk /sp <KEGG.sp>]")>
    Public Function DownloadOrthologs(args As CommandLine) As Integer
        Dim GBK As Boolean = args.GetBoolean("/gbk")
        Dim GeneList As String()
        Dim ExportedDir As String = args("-export")
        Dim in$ = args <= "-i"

        If Not GBK Then
            GeneList = in$.ReadAllLines()
        Else
            Dim gb = GBFF.File.Load([in])
            GeneList = gb.GeneList.Select(Function(g) g.Name)
        End If

        If Not GBK Then
            Call OrthologExport.HandleQuery(GeneList, ExportedDir)
        Else
            Dim sp As String = args("/sp")
            Call OrthologExport.HandleQuery(GeneList, ExportedDir, sp)
        End If

        Return 0
    End Function

    <ExportAPI("/ko.index.sub.match", Usage:="/ko.index.sub.match /index <index.csv> /maps <maps.csv> /key <key> /map <mapTo> [/out <out.csv>]")>
    Public Function IndexSubMatch(args As CommandLine) As Integer
        Dim index As String = args("/index")
        Dim maps As String = args("/maps")
        Dim key As String = args("/key")
        Dim map As String = args("/map")
        Dim out As String = args.GetValue("/out", maps.TrimSuffix & ".sub_matches.csv")
        Dim mappings As IEnumerable(Of Map(Of String, String)) = maps.LoadMappings(key, map)
        Dim result As KO_gene() = KEGGOrthology.IndexSubMatch(mappings, index).ToArray
        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Imports.SSDB", Usage:="/Imports.SSDB /in <source.DIR> [/out <ssdb.csv>]")>
    Public Function ImportsDb(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & ".Csv")
        Dim ssdb = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.API.Transform(inDIR)
        Return ssdb.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' Query the KEGG database for nucleotide sequence and protein sequence by using a keywork.
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("-query")>
    <Usage("-query -keyword <keyword> -o <out_dir>")>
    <Description("Query the KEGG database for nucleotide sequence and protein sequence by using a keywork.")>
    Public Function QueryGenes(args As CommandLine) As Integer
        Dim Out As String = args("-o")
        Dim Keyword As String = args("-keyword")
        Dim ChunkBuffer As List(Of WebServices.QueryEntry) = New List(Of WebServices.QueryEntry)

        For i As Integer = 1 To 100
            Dim EntryList = WebServices.HandleQuery(Keyword, i)

            If EntryList.IsNullOrEmpty Then
                Exit For
            Else
                Call ChunkBuffer.AddRange(EntryList)
            End If
        Next

        Call ChunkBuffer.SaveTo(Out & String.Format("/{0}.entry_list.csv", Keyword.NormalizePathString), False)

        Dim Fasta As FASTA.FastaFile = New FASTA.FastaFile
        Dim Nt As FASTA.FastaFile = New FASTA.FastaFile

        For Each Entry In ChunkBuffer
            Dim Protein = WebRequest.FetchSeq(Entry)

            If Protein Is Nothing Then
                Continue For
            Else
                Call Nt.Add(WebRequest.FetchNt(Entry))
                Call Fasta.Add(Protein)
            End If

            Call Console.Write("-")
        Next

        Call Fasta.Save(Out & String.Format("/{0}.proteins.fasta", Keyword.NormalizePathString))
        Call Nt.Save(Out & String.Format("/{0}.nt_list.fasta", Keyword.NormalizePathString))

        Return 0
    End Function

    ''' <summary>
    ''' 将所下载的数据按照物种分类来查看代谢系统的分布情况
    ''' </summary>
    ''' <param name="argvs"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("-Table.Create", Usage:="-table.create -i <input_dir> -o <out_csv>")>
    <ArgumentAttribute("-i", Description:="This parameter specific the source directory input of the download data.")>
    Public Function CreateTABLE(argvs As CommandLine) As Integer
        Dim Inputs As String() = FileIO.FileSystem.GetFiles(argvs("-i"), FileIO.SearchOption.SearchTopLevelOnly, "*.csv").ToArray
        Dim GeneData = (From path As String
                        In Inputs.AsParallel
                        Select ID = path.BaseName,
                            DataRow = (From entry As QueryEntry
                                       In path.LoadCsv(Of QueryEntry)(False)
                                       Select entry
                                       Group By entry.speciesID Into Group) _
                                               .ToDictionary(Function(obj) obj.speciesID,
                                                             Function(obj) obj.Group.First)).ToArray
        Dim File As New IO.File
        Dim MatrixBuilder As New IO.File

        Call File.AppendLine({"sp.Class", "sp.Kingdom", "sp.Phylum", "sp.KEGGId"})
        Call MatrixBuilder.AppendLine({"Class", "sp"})

        For Each col In GeneData
            Call File.Last.Add(col.ID)
            Call MatrixBuilder.Last.Add(col.ID)
        Next

        Dim OrganismList = bGetObject.Organism.GetOrganismListFromResource
        'Dim [ClassList] = (From sp In OrganismList.ToArray Select sp.Class Distinct).ToArray
        'For Each cls In ClassList
        '    Call MatrixBuilder.Last.Add("Class." & cls)
        'Next

        For Each sp As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism.Organism In OrganismList.Eukaryotes.Join(OrganismList.Prokaryote)
            Call File.Add(New String() {sp.Class, sp.Kingdom, sp.Phylum, sp.KEGGId})
            Call MatrixBuilder.AppendLine({sp.Class, sp.KEGGId})

            For Each col In GeneData
                If col.DataRow.ContainsKey(sp.KEGGId) Then
                    Call File.Last.Add(col.DataRow(sp.KEGGId).locusID)
                    Call MatrixBuilder.Last.Add("1")
                Else
                    Call File.Last.Add("")
                    Call MatrixBuilder.Last.Add("0")
                End If
            Next
            'For Each cls In ClassList
            '    If String.Equals(cls, sp.Class) Then
            '        Call MatrixBuilder.Last.Add("1")
            '    Else
            '        Call MatrixBuilder.Last.Add("0")
            '    End If
            'Next
        Next

        Dim Saved As String = argvs("-o")

        Call MatrixBuilder.Save(Saved & ".phylip_tree.csv", False)

        Return File.Save(Saved, False)
    End Function

    <ExportAPI("-query.orthology", Usage:="-query.orthology -keyword <gene_name> -o <output_csv>")>
    Public Function QueryOrthology(argvs As CommandLine) As Integer
        Dim EntryList = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.API.HandleQuery(argvs("-keyword"))
        Dim GeneEntries As New List(Of QueryEntry)

        For Each EntryPoint As QueryEntry In EntryList
            Call GeneEntries.AddRange(SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.SSDB.API.HandleDownload(EntryPoint.locusID))
        Next

        Call GeneEntries.SaveTo(argvs <= "-o", False)

        Return 0
    End Function

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    ''' <summary>
    ''' 从KEGG数据库之中读取数据到本地数据库之中
    ''' </summary>
    ''' <returns></returns>
    <ExportAPI("-Build.KO", Usage:="-Build.KO [/fill-missing]", Info:="Download data from KEGG database to local server.")>
    Public Function BuildKEGGOrthology(args As CommandLine) As Integer
        '   If args.GetBoolean("/fill-missing") Then
        ' Call __fillMissing()
        ' Return 0
        '  End If

        '  Dim mysql As New ConnectionUri With {.Database = "jp_kegg2", .IPAddress = "localhost", .Password = 1234, .User = "root", .ServicesPort = 3306}

        ' Dim LocalMySQL As New Procedures.Orthology(mysql)
        '  Dim stateFile As String = "./orthology.xml"
        '  Dim last = LocalMySQL.GetLast
        'Dim start As Integer
        'Dim Entries As String() = (From s As String
        '                           In LocalMySQL.BriefData.GetEntries
        '                           Where Not String.IsNullOrEmpty(s)
        '                           Select s
        '                           Distinct
        '                           Order By s Ascending).ToArray
        'If last Is Nothing Then
        '    start = 0
        'Else
        '    start = Array.IndexOf(Entries, last.Entry)
        'End If

        ' Dim fggfdg = SMRUCC.genomics.Assembly.KEGG.DBGET.WebParser.QueryURL("E:\GCModeller\BuildTools\K  02992.html")
        ' Call LocalMySQL.Update(fggfdg)

        Dim entries$() = htext.ko00001 _
            .Hierarchical _
            .GetEntries _
            .Where(Function(s) Not String.IsNullOrEmpty(s)) _
            .Distinct _
            .ToArray

        Using progress As New ProgressBar("Download KO database...", 1, CLS:=True)
            Dim tick As New ProgressProvider(progress, entries.Length)

            WebServiceUtils.Proxy = "http://127.0.0.1:8087/"

            For Each ko$ In entries
                Dim stateFile As String = $"{App.HOME}/ko00001/{ko}.xml"

                Try
                    If Not stateFile.FileExists Then
                        Dim orthology = bGetObject.SSDB.API.Query(ko)
                        '  Call LocalMySQL.Update(orthology)
                        Call Threading.Thread.Sleep(1 * 1000)
                        Call orthology.GetXml.SaveTo(stateFile)
                    End If
                Catch ex As Exception
                    ex = New Exception(ko, ex)
                    Call ex.PrintException
                    Call App.LogException(ex)
                End Try

                Call progress.SetProgress(tick.StepProgress(), "ETA " & tick.ETA().FormatTime)
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/Build.Ko.repository", Usage:="/Build.Ko.repository /DIR <DIR> /repo <root>")>
    <Group(CLIGroups.Repository_cli)>
    Public Function BuildKORepository(args As CommandLine) As Integer
        Dim DIR As String = args("/DIR")
        Dim repoRoot As String = args("/repo")

        Call KEGGOrthology.FileCopy(DIR, repoRoot & "/ko00001/") _
            .ToArray _
            .GetJson _
            .SaveTo(repoRoot & "/ko00001.copy_failures.json")

        Dim repo As New KEGGOrthology(repoRoot)

        Call repo.BuildLocusIndex()

        Return 0
    End Function

    <ExportAPI("/Imports.KO",
               Info:="Imports the KEGG reference pathway map and KEGG orthology data as mysql dumps.",
               Usage:="/Imports.KO /pathways <DIR> /KO <DIR> [/save <DIR>]")>
    Public Function ImportsKODatabase(args As CommandLine) As Integer
        Dim pathway$ = args <= "/pathways"
        Dim KO$ = args <= "/KO"
        Dim save$ = args.GetValue("/save", pathway & "-" & KO.BaseName & ".Dumps/")

        '  Call DumpProcedures.DumpReferencePathwayMap(pathway, save)
        Call DumpProcedures.DumpKO(KO, save)

        Return 0
    End Function

    ''' <summary>
    ''' 下载参考代谢途径的数据
    ''' </summary>
    ''' <param name="argvs"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("-query.ref.map", Usage:="-query.ref.map -id <id> -o <out_dir>")>
    Public Function DownloadReferenceMap(argvs As CommandLine) As Integer
        Dim Map As ReferenceMapData = ReferenceMapData.Download(ID:=argvs("-id"))
        Call Map.GetXml.SaveTo(argvs("-o"))
        Return True
    End Function

    <ExportAPI("-function.association.analysis", Usage:="-function.association.analysis -i <matrix_csv>")>
    Public Function FunctionAnalysis(argvs As CommandLine) As Integer
        Dim MAT = FileLoader.FastLoad(argvs("-i"))
        Call PathwayAssociationAnalysis.Analysis(MAT, argvs("-i"))
        Return 0
    End Function

    ''' <summary>
    ''' Download 16S rRNA data from KEGG.
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/16S_rRNA")>
    <Description("Download 16S rRNA data from KEGG.")>
    <Usage("/16s_rna [/out <outDIR>]")>
    Public Function Download16SRNA(args As CommandLine) As Integer
        Dim outDIR As String = args.GetValue("/out", App.CurrentDirectory & "/")
        Dim fasta As FastaFile = Download16S_rRNA(outDIR)
        Return fasta _
            .Save($"{outDIR}/16S_rRNA.fasta", Encoding.ASCII) _
            .CLICode
    End Function

    ''' <summary>
    ''' 从一个给定的fasta文件之中挑选出指定物种列表的fasta序列到一个新的文件中
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Fasta.By.Sp")>
    <Usage("/Fasta.By.Sp /in <KEGG.fasta> /sp <sp.list> [/out <out.fasta>]")>
    <Description("Picks the fasta sequence from the input sequence database by a given species list.")>
    Public Function GetFastaBySp(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim sp As String = args("/sp")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "." & sp.BaseName & ".fasta")
        Dim fasta As New FASTA.FastaFile([in])
        Dim splist As List(Of String) = sp.ReadAllLines.ToList(Function(s) s.ToLower)
        Dim LQuery As IEnumerable(Of FASTA.FastaSeq) =
            From fa As FASTA.FastaSeq
            In fasta
            Let tag As String = fa.Headers(Scan0).Split(":"c).First.ToLower
            Where splist.IndexOf(tag) > -1
            Select fa '

        Return New FASTA.FastaFile(LQuery).Save(out, Encoding.ASCII)
    End Function

    ''' <summary>
    ''' 下载通过uniprot数据库map得到的kegg编号的序列的列表
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Download.Mapped.Sequence")>
    <Usage("/Download.Mapped.Sequence /map <map.list> [/nucl /out <seq.fasta>]")>
    Public Function DownloadMappedSequence(args As CommandLine) As Integer
        Dim in$ = args <= "/map"
        Dim isNucl As Boolean = args.IsTrue("/nucl")
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.{"prot" Or "nucl".When(isNucl)}_seq.fasta"
        Dim tokens$()
        Dim accession$
        Dim kegg_ids$()
        Dim fastaQuery As New FetchSequence(isNucl, cache:=$"{out.ParentPath}/.dbget/{"prot" Or "nucl".When(isNucl)}/")

        Using writer As StreamWriter = out.OpenWriter
            For Each line In [in].ReadAllLines
                tokens = line.Split(ASCII.TAB)
                accession = tokens(Scan0)
                kegg_ids = tokens.Skip(1).ToArray

                For Each fa As FastaSeq In kegg_ids.SafeQuery _
                    .Select(Function(id)
                                Return fastaQuery.Query(Of FastaSeq)(New QueryEntry(id), ".html")
                            End Function)

                    If Not fa Is Nothing Then
                        Call writer.WriteLine(fa.GenerateDocument(-1))
                    End If
                Next
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/Download.Fasta")>
    <Usage("/Download.Fasta /query <querySource.txt> [/out <outDIR> /source <existsDIR>]")>
    <Description("Download fasta sequence from KEGG database web api.")>
    <ArgumentAttribute("/query", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(QuerySource)},
              Description:="This file should contains the locus_tag id list for download sequence.")>
    Public Function DownloadSequence(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim out As String = args("/out") Or query.ParentPath
        ' 假若不存在这个参数的输入的话，将路径指向一个空文件夹，减少搜索的时间
        Dim sourceDIR As String = args("/source") Or out
        Dim loadSource = sourceDIR.LoadSourceEntryList("*.fasta", "*.fa", "*.fsa", "*.fas")
        Dim querySource As QuerySource = WebServices.QuerySource.DocParser(query)
        Dim sp As String = querySource.QuerySpCode
        Dim outExists = out.LoadSourceEntryList("*.fasta", "*.fa", "*.fsa", "*.fas")

        If String.IsNullOrEmpty(sp) Then
            Call $"{querySource.genome}:{querySource.locusId.First} may not exists in the KEGG database...".__DEBUG_ECHO
            Return -100
        End If

        Dim listFiles As New List(Of String)

        For Each sId As String In querySource.locusId
            Dim path As String = $"{out}/{sId}.fasta"

            Call listFiles.Add(path)

            If outExists.ContainsKey(sId) Then
                Continue For
            End If

            If loadSource.ContainsKey(sId) Then
                If SafeCopyTo(loadSource(sId), path) Then
                    Continue For
                End If
            End If

            Dim protein As FastaSeq = WebRequest.FetchSeq(sp, sId)

            If Not protein Is Nothing Then
                Call protein.SaveTo(path)
            Else
                Call $"{sId} is not available on KEGG database...".__DEBUG_ECHO
            End If

            Call Thread.Sleep(1000)
        Next

        Dim outFile$ = out.TrimDIR & ".fasta"
        Dim result As FastaSeq() = LinqAPI.Exec(Of FastaSeq) _
 _
            () <= From fa As String
                  In listFiles
                  Where fa.FileExists
                  Select New FastaSeq(fa)

        Return New FastaFile(result) _
            .Save(path:=outFile) _
            .CLICode
    End Function
End Module
