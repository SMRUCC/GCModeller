#Region "Microsoft.VisualBasic::d265733bae5bff336ec6370d16916948, ..\GCModeller\CLI_tools\RegPrecise\CLI\DownloadAPI.vb"

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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Organism
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.SequenceModel

<PackageNamespace("RegPrecise.CLI", Category:=APICategories.CLI_MAN)>
Public Module CLI

    <ExportAPI("/Repository.Fetch",
               Usage:="/Repository.Fetch /imports <RegPrecise.Xml> /genbank <NCBI_Genbank_DIR> [/full /out <outDIR>]")>
    Public Function FetchRepostiory(args As CommandLine) As Integer
        Dim [in] As String = args("/imports")
        Dim genbank As String = args("/genbank")
        Dim out As String = [in].TrimSuffix & ".Fasta/"
        Dim repository As New Genbank(genbank)
        Dim trim As Boolean = Not args.GetBoolean("/full")

        For Each genome As BacteriaGenome In [in].LoadXml(Of TranscriptionFactors).BacteriaGenomes
            Dim path As String = out & $"/{genome.BacteriaGenome.name.NormalizePathString}.fasta"
            Dim query As QuerySource = genome.CreateKEGGQuery
            Dim entry As GenbankIndex = repository.Query(query)

            Call query.GetDoc.SaveTo(path.TrimSuffix & ".txt")

            If Not entry Is Nothing Then
                Dim gb As GBFF.File = entry.Gbk(genbank)

                If gb Is Nothing Then
                    Call VBDebugger.Warning(entry.GetJson & " genbank database not found!")
                    Continue For
                End If

                Dim fasta As FASTA.FastaFile = gb.ExportProteins_Short

                If trim Then
                    Dim hash As Dictionary(Of String, FASTA.FastaToken) =
                        fasta.ToDictionary(Function(x) x.Attributes.First.Split.First)

                    fasta = New FASTA.FastaFile(From locus As String
                                                In query.locusId
                                                Where hash.ContainsKey(locus)
                                                Select hash(locus))
                End If

                Call fasta.Save(path)
            End If
        Next

        Return 0
    End Function

    <ExportAPI("/Fetches", Usage:="/Fetches /ncbi <all_gbk.DIR> /imports <inDIR> /out <outDIR>")>
    Public Function Fetch(args As CommandLine) As Integer
        Dim allGBKs = FileIO.FileSystem.GetDirectories(args("/ncbi"), FileIO.SearchOption.SearchTopLevelOnly)
        Dim gbkHash = allGBKs.ToDictionary(Function(x) x.BaseName)
        Dim inDIR As String = args("/imports")
        Dim DIRs = FileIO.FileSystem.GetDirectories(inDIR)
        Dim out As String = args("/out")
        Dim CLIs As New List(Of String)

        Call $"Get {DIRs.Count} directories...".__DEBUG_ECHO

        For Each DIR As String In DIRs
            Dim query As String = DIR & "/query.txt"
            query = query.ReadFirstLine.Trim.Replace(" ", "_")

#If Not DEBUG Then
            If Not FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.fasta", "*.fa", "*.fsa").IsNullOrEmpty Then
                Continue For '  已经存在数据了，则不需要再导数据了
            End If
#End If
            Dim lkey As String = Text.Similarity.StringSelection(query, gbkHash.Keys, cut:=0.65)

            If String.IsNullOrEmpty(lkey) Then
                Call $"{lkey} is not found!".__DEBUG_ECHO
                Continue For
            Else
                Dim gbkDIR As String = gbkHash(lkey)

                query = DIR & "/query.txt"
                CLIs += $"{GetType(CLI).API(NameOf(FetchThread))} /gbk {gbkDIR.CliPath} /query {query.CliPath} /out {out.CliPath}"
            End If
        Next

        Return App.SelfFolks(CLIs, LQuerySchedule.Recommended_NUM_THREADS)
    End Function

    <ExportAPI("/Fetches.Thread", Usage:="/Fetches.Thread /gbk <gbkDIR> /query <query.txt> /out <outDIR>")>
    Public Function FetchThread(args As CommandLine) As Integer
        Dim gbkDIR As String = args("/gbk")
        Dim query As String = args("/query")
        Dim out As String = args("/out")
        Dim locus As String() = query.ReadAllLines.Skip(2).ToArray
        Dim fasta As New List(Of FASTA.FastaToken)

        For Each gbk As String In FileIO.FileSystem.GetFiles(gbkDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.gbk", "*.gb")
            Dim gb As GBFF.File = GBFF.File.Load(gbk)
            Dim prot = (From x As FASTA.FastaToken In gb.ExportProteins_Short(True)
                        Select x
                        Group x By x.Attributes.First Into Group) _
                             .ToDictionary(Function(x) x.First,
                                           Function(x) x.Group.First)
            For Each id As String In locus
                If prot.ContainsKey(id) Then
                    Dim fa As FASTA.FastaToken = prot(id)
                    Call fa.Save(query.ParentPath & $"/{id}.fasta", Encodings.ASCII)
                    fasta += fa
                End If
            Next

            If fasta.Count > 0 Then
                Exit For
            End If
        Next

        If fasta.Count > 0 Then
            out = out & $"/{query.ParentDirName}.fasta"
            Call New FASTA.FastaFile(fasta).Save(out, Encodings.ASCII)
        End If

        Return 0
    End Function

    ''' <summary>
    ''' 将下载得到的fasta序列文件进行合并
    ''' </summary>
    ''' <returns></returns>
    '''
    <ExportAPI("/Merge.RegPrecise.Fasta",
               Info:="",
               Usage:="/Merge.RegPrecise.Fasta [/in <inDIR> /out outDIR /offline]")>
    Public Function MergeDownload(args As CommandLine) As Integer
        Dim inDIR As String = args.GetValue("/in", App.CurrentDirectory)
        Dim DIRs As IEnumerable(Of String) = ls - l - lsDIR <= inDIR
        Dim genomes As IEnumerable(Of String) = ls - l - wildcards("*.xml") <= inDIR
        Dim RegPreciseFasta As New FASTA.FastaFile
        Dim out As String = args.GetValue("/out", inDIR)
        Dim offline As Boolean = args.GetBoolean("/offline")

        Call $"Get {DIRs.Count} directories and {genomes.Count} genomes...".__DEBUG_ECHO

        For Each genome As BacteriaGenome In genomes.Select(AddressOf LoadXml(Of BacteriaGenome))
            Dim name As String = genome.BacteriaGenome.name.NormalizePathString
            Dim DIR As String = inDIR & "/" & name
            Dim sp As String = genome.CreateKEGGQuery.QuerySpCode(offline)

            If String.IsNullOrEmpty(sp) Then
                Continue For
            Else
                Call name.__DEBUG_ECHO
            End If

            Dim fastas As IEnumerable(Of String) = ls - l - wildcards("*.fasta", "*.fas", "*.fa", "*.fsa") <= DIR

            For Each fa As FASTA.FastaToken In fastas.Select(AddressOf FASTA.FastaToken.Load)
                If fa.Attributes.First.Split(":"c).First = sp Then
                    Dim fas As FASTA.FastaFile = FASTA.Reflection.Merge(DIR, trim:=True, rawTitle:=False)
                    Dim path As String = $"{out}/{name.Replace(" ", "_")}.fasta"   ' blast 程序的序列文件名之中不可以有空格

                    Call RegPreciseFasta.AddRange(fas)
                    Call fas.Save(path, Encodings.ASCII)

                    Exit For
                End If
            Next
        Next

        Return RegPreciseFasta.Save(out & "/RegPrecise.fasta").CLICode
    End Function

    ''' <summary>
    ''' 下载在Regprecise数据库之中的调控和被调控的基因的蛋白质序列，以方便进行regulon的推测和构建
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Fasta.Downloads",
               Info:="Download protein fasta sequence from KEGG database.",
               Usage:="Fasta.Downloads /source <sourceDIR> [/out <outDIR> /keggTools <kegg.exe>]")>
    Public Function DownloadFasta(args As CommandLine) As Integer
        Dim sourceDIR As String = args("/source")
        Dim out As String = args.GetValue("/out", sourceDIR)
        Dim KEGG As String = args.GetValue("/keggTools", App.HOME & "/kegg.exe")
        Dim existsDIR As String = RegPrecise.GCModeller.FileSystem.RegPreciseRegulatorFasta

        For Each file As String In FileIO.FileSystem.GetFiles(sourceDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
            Dim genome = file.LoadXml(Of BacteriaGenome)
            Dim outDIR As String = out & "/" & genome.BacteriaGenome.name.NormalizePathString(True)
            Dim exists As String = existsDIR & "/" & genome.BacteriaGenome.name.NormalizePathString(True)
            Dim query = genome.CreateKEGGQuery
            Dim queryFile As String = outDIR & "/query.txt"
            Dim CLI As String = $"Download.Sequence /query {queryFile.CliPath} /out {outDIR.CliToken} /source {exists.CliToken}"

            Call query.GetDoc.SaveTo(queryFile)
            Call New IORedirectFile(KEGG, CLI).Run()
        Next

        Return 0
    End Function

    <ExportAPI("/ProtMotifs.Downloads",
               Info:="Download protein domain motifs structures from KEGG ssdb.",
               Usage:="/ProtMotifs.Downloads /source <source.DIR> [/kegg.Tools <./kegg.exe>]")>
    Public Function DownloadProteinMotifs(args As CommandLine) As Integer
        Dim sourceDIR As String = args("/source")
        Dim KEGG As String = args.GetValue("/keggTools", App.HOME & "/kegg.exe")
        Dim queries As IEnumerable(Of String) = ls - l - r - wildcards("query.txt") << FileHandles.OpenHandle(sourceDIR)

        For Each query As String In queries
            If query.ParentPath.EnumerateFiles("*.fasta", "*.fsa", "*.fa").IsNullOrEmpty Then
                Continue For
            End If

            Dim CLI As String = $"/Gets.prot_motif /query {query.CliPath}"
            Call New IORedirectFile(KEGG, CLI).Run()
        Next

        Return 0
    End Function

    <ExportAPI("/Download.Regprecise",
               Info:="Download Regprecise database from Web API",
               Usage:="Download.Regprecise [/work ./ /save <saveXml>]")>
    Public Function DownloadRegprecise2(args As CommandLine) As Integer
        Dim WORK As String = args.GetValue("/work", App.CurrentDirectory & "/RegpreciseDownloads/")
        Dim Db As TranscriptionFactors = WebAPI.Download(WORK)
        Dim out As String = args.GetValue("/save", App.CurrentDirectory & "/Regprecise.Xml")

        Return Db.GetXml.SaveTo(out)
    End Function

    <ExportAPI("/Download.Motifs", Usage:="/Download.Motifs /imports <RegPrecise.DIR> [/export <EXPORT_DIR>]")>
    Public Function DownloadMotifSites(args As CommandLine) As Integer
        Dim inDIR As String = args("/imports")
        Dim genomes = inDIR.EnumerateFiles("*.xml").ToArray(Function(path) path.LoadXml(Of BacteriaGenome))
        Dim EXPORT As String = args.GetValue("/export", inDIR.ParentPath & "/Motif_PWM/")
        Dim sites As IEnumerable(Of String) = genomes.ToArray(
            Function(g) g.Regulons.Regulators.ToArray(
            Function(x) x.SiteMore),
            Function(g) Not g.Regulons Is Nothing AndAlso
                        Not g.Regulons.Regulators.IsNullOrEmpty).MatrixAsIterator.Distinct.ToArray
        Dim list As List(Of String) = (App.SysTemp & "/process.txt").ReadAllLines.ToList

        For Each url In sites.SeqIterator
            If String.Equals(url.obj, MotifWebAPI.RegPrecise, StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If
            If list.IndexOf(url.obj) > -1 Then
                Continue For
            End If

            Dim motif As MotifSitelog = MotifWebAPI.Download(url)
            Dim name As String = EXPORT & motif.Taxonomy.Key.NormalizePathString & "/" & motif.Regulog.Key.NormalizePathString
            Dim path As String = name & ".Xml"
            Call motif.SaveAsXml(path)
            Call motif.logo.DownloadFile(name & ".png")

            list += url.obj
            Call list.FlushAllLines(App.SysTemp & "/process.txt")
        Next

        Return 0
    End Function
End Module

