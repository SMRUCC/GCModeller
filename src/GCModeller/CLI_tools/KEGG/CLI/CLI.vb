#Region "Microsoft.VisualBasic::c36c8da3b402d2d4ef786ba6aac880dd, ..\GCModeller\CLI_tools\KEGG\CLI\CLI.vb"

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

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.ReferenceMap
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
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
<PackageNamespace("KEGG.WebServices.CLI",
                  Publisher:="xie.guigang@gcmodeller.org",
                  Category:=APICategories.CLI_MAN,
                  Url:="http://www.kegg.jp/",
                  Description:="KEGG web services API tools.")>
Module CLI

    <ExportAPI("/blastn", Usage:="/blastn /query <query.fasta> [/out <outDIR>]", Info:="Blastn analysis of your DNA sequence on KEGG server for the functional analysis.")>
    Public Function Blastn(args As CommandLine) As Integer
        Dim queryFile As String = args("/query")
        Dim out As String = args.GetValue("/out", queryFile.TrimSuffix)
        Dim query As New FASTA.FastaFile(queryFile)

        For Each seq As FastaToken In query
            Dim path As String = $"{out}/{seq.Title.NormalizePathString(False)}.csv"
            Dim result = Global.KEGG_tools.Blastn.Submit(seq)
            Call result.SaveTo(path)
        Next

        Return 0
    End Function

    <ExportAPI("/Download.Ortholog",
               Info:="Downloads the KEGG gene ortholog annotation data from the web server.",
               Usage:="/Download.Ortholog -i <gene_list_file.txt/gbk> -export <exportedDIR> [/gbk /sp <KEGG.sp>]",
               Example:="")>
    Public Function DownloadOrthologs(args As CommandLine) As Integer
        Dim GBK As Boolean = args.GetBoolean("/gbk")
        Dim GeneList As String()
        Dim ExportedDir As String = args("-export")
        If Not GBK Then
            GeneList = IO.File.ReadAllLines(args("-i"))
        Else
            Dim gb = GBFF.File.Load(args("-i"))
            GeneList = gb.GeneList.ToArray(Function(g) g.Key)
        End If

        If Not GBK Then
            Call OrthologExport.HandleQuery(GeneList, ExportedDir)
        Else
            Dim sp As String = args("/sp")
            Call OrthologExport.HandleQuery(GeneList, ExportedDir, sp)
        End If

        Return 0
    End Function

    <ExportAPI("/Imports.SSDB", Usage:="/Imports.SSDB /in <source.DIR> [/out <ssdb.csv>]")>
    Public Function ImportsDb(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & ".Csv")
        Dim ssdb = DBGET.bGetObject.SSDB.API.Transform(inDIR)
        Return ssdb.SaveTo(out).CLICode
    End Function

    <ExportAPI("-query",
               Usage:="-query -keyword <keyword> -o <out_dir>",
               Info:="Query the KEGG database for nucleotide sequence and protein sequence by using a keywork.")>
    Public Function QueryGenes(argvs As CommandLine) As Integer
        Dim Out As String = argvs("-o")
        Dim Keyword As String = argvs("-keyword")
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
    <Argument("-i", Description:="This parameter specific the source directory input of the download data.")>
    Public Function CreateTABLE(argvs As CommandLine) As Integer
        Dim Inputs As String() = FileIO.FileSystem.GetFiles(argvs("-i"), FileIO.SearchOption.SearchTopLevelOnly, "*.csv").ToArray
        Dim GeneData = (From path As String
                        In Inputs.AsParallel
                        Select ID = IO.Path.GetFileNameWithoutExtension(path),
                            DataRow = (From entry As QueryEntry
                                       In path.LoadCsv(Of QueryEntry)(False)
                                       Select entry
                                       Group By entry.SpeciesId Into Group) _
                                               .ToDictionary(Function(obj) obj.SpeciesId,
                                                             Function(obj) obj.Group.First)).ToArray
        Dim File As New DocumentStream.File
        Dim MatrixBuilder As New DocumentStream.File

        Call File.AppendLine({"sp.Class", "sp.Kingdom", "sp.Phylum", "sp.KEGGId"})
        Call MatrixBuilder.AppendLine({"Class", "sp"})

        For Each col In GeneData
            Call File.Last.Add(col.ID)
            Call MatrixBuilder.Last.Add(col.ID)
        Next

        Dim OrganismList = DBGET.bGetObject.Organism.GetOrganismListFromResource
        'Dim [ClassList] = (From sp In OrganismList.ToArray Select sp.Class Distinct).ToArray
        'For Each cls In ClassList
        '    Call MatrixBuilder.Last.Add("Class." & cls)
        'Next

        For Each sp As DBGET.bGetObject.Organism.Organism In OrganismList.Eukaryotes.ToList + OrganismList.Prokaryote
            Call File.Add(New String() {sp.Class, sp.Kingdom, sp.Phylum, sp.KEGGId})
            Call MatrixBuilder.AppendLine({sp.Class, sp.KEGGId})

            For Each col In GeneData
                If col.DataRow.ContainsKey(sp.KEGGId) Then
                    Call File.Last.Add(col.DataRow(sp.KEGGId).LocusId)
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
        Dim EntryList = DBGET.bGetObject.SSDB.API.HandleQuery(argvs("-keyword"))
        Dim GeneEntries As List(Of QueryEntry) = New List(Of QueryEntry)

        For Each EntryPoint As QueryEntry In EntryList
            Call GeneEntries.AddRange(DBGET.bGetObject.SSDB.API.HandleDownload(EntryPoint.LocusId))
        Next

        Call GeneEntries.SaveTo(argvs("-o"), False)

        Return 0
    End Function

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    <ExportAPI("/Pull.Seq", Info:="Downloads the missing sequence in the local KEGG database from the KEGG database server.")>
    Public Function PullSequence(args As CommandLine) As Integer
        Dim donwloader As New SeuqneceDownloader(MySQLExtensions.GetURI)
        Call donwloader.RunTask()
        Return 0
    End Function

    Private Sub __fillMissing()
        Dim LocalMySQL As New Procedures.Orthology(MySQL)
        Call LocalMySQL.FillMissing()
    End Sub

    <ExportAPI("--Export.KO")>
    Public Function ExportKO(args As CommandLine) As Integer

    End Function

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
        Using progress As New Terminal.ProgressBar("Download KO database...",, True)
            Dim tick As New Terminal.ProgressProvider(entries.Length)

            WebServiceUtils.Proxy = "http://127.0.0.1:8087/"

            For Each ko$ In entries
                Dim stateFile As String = $"{App.HOME}/ko00001/{ko}.xml"

                Try
                    If Not stateFile.FileExists Then
                        Dim orthology = bGetObject.SSDB.API.Query(ko)
                        '  Call LocalMySQL.Update(orthology)
                        Call orthology.GetXml.SaveTo(stateFile)
                    End If
                Catch ex As Exception
                    ex = New Exception(ko, ex)
                    Call ex.PrintException
                    Call App.LogException(ex)
                End Try

                Call Threading.Thread.Sleep(1 * 1000)
                Call progress.SetProgress(
                    tick.StepProgress(),
                    "ETA " & tick.ETA(progress.ElapsedMilliseconds).FormatTime)
            Next
        End Using

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

    <ExportAPI("-ref.map.download", Usage:="-ref.map.download -o <out_dir>")>
    Public Function DownloadReferenceMapDatabase(argvs As CommandLine) As Integer
        Dim OutDir As String = argvs("-o")
        Dim IDList = DBGET.BriteHEntry.Pathway.LoadFromResource
        Dim DownloadLQuery = (From ID As DBGET.BriteHEntry.Pathway
                              In IDList
                              Let MapID As String = "map" & ID.EntryId
                              Let Map = ReferenceMapData.Download(MapID)
                              Select Map.GetXml.SaveTo(OutDir & "/" & MapID & ".xml")).ToArray
        Return 0
    End Function

    <ExportAPI("-function.association.analysis", Usage:="-function.association.analysis -i <matrix_csv>")>
    Public Function FunctionAnalysis(argvs As CommandLine) As Integer
        Dim MAT = DocumentStream.File.FastLoad(argvs("-i"))
        Call PathwayAssociationAnalysis.Analysis(MAT)
        Return 0
    End Function

    <ExportAPI("/16S_rRNA", Usage:="/16s_rna [/out <outDIR>]")>
    Public Function Download16SRNA(args As CommandLine) As Integer
        Dim outDIR As String = args.GetValue("/out", App.CurrentDirectory & "/")
        Dim fasta As FASTA.FastaFile = Download16S_rRNA(outDIR)
        Return fasta.Save($"{outDIR}/16S_rRNA.fasta", Encoding.ASCII).CLICode
    End Function

    <ExportAPI("/Fasta.By.Sp",
               Usage:="/Fasta.By.Sp /in <KEGG.fasta> /sp <sp.list> [/out <out.fasta>]")>
    Public Function GetFastaBySp(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim sp As String = args("/sp")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "." & sp.BaseName & ".fasta")
        Dim fasta As New FASTA.FastaFile([in])
        Dim splist As List(Of String) = sp.ReadAllLines.ToList(Function(s) s.ToLower)
        Dim LQuery As IEnumerable(Of FASTA.FastaToken) =
            From fa As FASTA.FastaToken
            In fasta
            Let tag As String = fa.Attributes(Scan0).Split(":"c).First.ToLower
            Where splist.IndexOf(tag) > -1
            Select fa '

        Return New FASTA.FastaFile(LQuery).Save(out, Encoding.ASCII)
    End Function

    <ExportAPI("Download.Sequence", Usage:="Download.Sequence /query <querySource.txt> [/out <outDIR> /source <existsDIR>]")>
    Public Function DownloadSequence(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim out As String = args.GetValue("/out", query.ParentPath)
        Dim sourceDIR As String = args.GetValue("/source", out)  ' 假若不存在这个参数的输入的话，将路径指向一个空文件夹，减少搜索的时间
        Dim loadSource = sourceDIR.LoadSourceEntryList("*.fasta", "*.fa", "*.fsa", "*.fas")
        Dim querySource As QuerySource = WebServices.QuerySource.DocParser(query)
        Dim sp As String = querySource.QuerySpCode
        Dim outExists = out.LoadSourceEntryList("*.fasta", "*.fa", "*.fsa", "*.fas")

        If String.IsNullOrEmpty(sp) Then
            Call $"{querySource.genome}:{querySource.locusId.First} may not exists in the KEGG database...".__DEBUG_ECHO
            Return -100
        End If

        Dim lstFiles As New List(Of String)

        For Each sId As String In querySource.locusId
            Dim path As String = $"{out}/{sId}.fasta"

            Call lstFiles.Add(path)

            If outExists.ContainsKey(sId) Then
                Continue For
            End If

            If loadSource.ContainsKey(sId) Then
                If SafeCopyTo(loadSource(sId), path) Then
                    Continue For
                End If
            End If

            Dim prot As FASTA.FastaToken = FetchSeq(sp, sId)
            If Not prot Is Nothing Then
                Call prot.SaveTo(path)
            Else
                Call $"{sId} is not available on KEGG database...".__DEBUG_ECHO
            End If
        Next

        Dim result = LinqAPI.Exec(Of FastaToken) <=
            From fa As String
            In lstFiles
            Where fa.FileExists
            Select New FastaToken(fa)

        Return New FastaFile(result).Save(out & ".fasta")
    End Function

    <ExportAPI("/Download.Pathway.Maps",
               Usage:="/Download.Pathway.Maps /sp <kegg.sp_code> [/out <EXPORT_DIR>]")>
    Public Function DownloadPathwayMaps(args As CommandLine) As Integer
        Dim sp As String = args("/sp")
        Dim EXPORT As String = args.GetValue("/out", App.CurrentDirectory & "/" & sp)
        Dim all = LinkDB.Pathways.AllEntries(sp).ToArray

        Call LinkDB.Pathways.Downloads(sp, EXPORT).ToArray

        Return 0
    End Function
End Module
