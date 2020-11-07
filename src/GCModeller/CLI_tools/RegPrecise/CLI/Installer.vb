#Region "Microsoft.VisualBasic::70f57ac99e7958965c051d6bdaa7d219, CLI_tools\RegPrecise\CLI\Installer.vb"

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
    '     Function: __path, __title, ExportRegulators, InstallRegPreciseMotifs, SelectTFBBH
    '               SelectTFPfams
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Regtransbase.WebServices.Regulator
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/install.motifs", Usage:="/install.motifs /imports <motifs.DIR>")>
    Public Function InstallRegPreciseMotifs(args As CommandLine) As Integer
        Dim importsDIR As String = args("/imports")
        Dim files = FileIO.FileSystem.GetFiles(importsDIR, FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
        Dim motifs = files.Select(Function(m) m.LoadXml(Of MotifSitelog))
        Dim EXPORT As String = GCModeller.FileSystem.RegpreciseRoot & "/MotifSites/"

        For Each motif As MotifSitelog In motifs
            Dim GroupQuery = From x As Regtransbase.WebServices.MotifFasta
                             In motif.Sites
                             Select x
                             Group x By uid = $"{x.locus_tag}:{x.position}" Into Group
            Dim sites As List(Of FastaSeq) =
                GroupQuery.ToList(
                    Function(x) New FastaSeq(
                        {x.uid}, SequenceTrimming(x.Group.First.SequenceData)))
            Dim fa As New FastaFile(sites)
            Dim i As Integer = 1
            Dim path As String = __path(EXPORT, motif)

            Do While fa.Count < 6
                Dim copy = sites.Select(Function(x) New FastaSeq({__title(x.Headers.First, i)}, x.SequenceData))
                Call fa.AddRange(copy)
                i += 1
            Loop

            Call fa.Save(path, Encodings.ASCII)
        Next

        Return 0
    End Function

    Private Function __path(EXPORT As String, motif As MotifSitelog) As String
        Dim name As String = MotifSitelog.Name(motif)
        Dim path As String = $"{EXPORT}/{name}.fasta"
        Return path
    End Function

    Private Function __title(s As String, i As Integer) As String
        Dim tokens As String() = s.Split(":"c)
        tokens(0) = tokens(0) & "_" & i.ToString
        Return String.Join(":", tokens)
    End Function

    ''' <summary>
    ''' 从下载得到的FASTA数据库之中导出所有的调控因子的序列数据
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Export.Regulators",
               Info:="Exports all of the fasta sequence of the TF regulator from the download RegPrecsie FASTA database.",
               Usage:="/Export.Regulators /imports <regprecise.downloads.DIR> /Fasta <regprecise.fasta> [/locus-out /out <out.fasta>]")>
    <ArgumentAttribute("/locus-out", True,
                   Description:="Does the program saves a copy of the TF locus_tag list at the mean time of the TF fasta sequence export.")>
    Public Function ExportRegulators(args As CommandLine) As Integer
        Dim xmls As IEnumerable(Of String) = ls - l - r - wildcards("*.xml") << args + "/imports"
        Dim fasta As String = args <= "/fasta"
        Dim prot As New FastaFile(fasta)
        Dim out As String = ("/out" <= args) ^ $"{fasta.TrimSuffix}-regulators.fasta"

        Call $" >>>> {out.ToFileURL} fro {xmls.Count} genomes....".__DEBUG_ECHO
        Call $"Create hash for {fasta.ToFileURL}".__DEBUG_ECHO

        Dim protHash = (From x As FastaSeq
                        In prot
                        Select x, locus = x.Headers.First.Split(":"c).Last.Trim    ' 按照基因编号生成哈希表
                        Group By locus Into Group) _
                             .ToDictionary(Function(x) x.locus,
                                           Function(x) x.Group.First.x)
        prot = New FastaFile

        For Each genome As BacteriaRegulome In (From path As String In xmls Select path.LoadXml(Of BacteriaRegulome))
            Dim regulators As String() = genome.ListRegulators
            prot += From sid As String     ' 在当前的基因组里面查找哈希表里面的序列并添加导结果集合之中
                    In regulators
                    Where protHash.ContainsKey(sid)
                    Select protHash(sid)
            Call ".".Echo
        Next

        If args.GetBoolean("/locus-out") Then
            Dim outLocus As String() = prot.Select(Function(x) x.Headers(Scan0))
            Call outLocus.FlushAllLines(out.TrimSuffix & "-locus_tags.txt")
        End If

        Return prot > out
    End Function

    <ExportAPI("/Select.TF.Pfam-String",
               Usage:="/Select.TF.Pfam-String /pfam-string <RegPrecise.pfam-string.csv> /imports <regprecise.downloads.DIR> [/out <TF.pfam-string.csv>]")>
    Public Function SelectTFPfams(args As CommandLine) As Integer
        Dim [in] As String = args - "/pfam-string"
        Dim xmls As IEnumerable(Of String) = ls - l - r - wildcards("*.xml") << args + "/imports"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-TF.Pfam-String.Csv")
        Dim pfamHash As Dictionary(Of String, PfamString) = (From x As PfamString
                                                             In [in].LoadCsv(Of PfamString)
                                                             Select uid = x.ProteinId.Split(":"c).Last,
                                                                 x
                                                             Group By uid Into Group) _
                                                                     .ToDictionary(Function(x) x.uid,
                                                                                   Function(x) x.Group.First.x)
        Dim list As New List(Of PfamString)

        For Each genome As BacteriaRegulome In (From path As String In xmls Select path.LoadXml(Of BacteriaRegulome))
            Dim regulators As String() = genome.ListRegulators
            list += (From sid As String In regulators Where pfamHash.ContainsKey(sid) Select pfamHash(sid))
            Call ".".Echo
        Next

        Return list.SaveTo(out)
    End Function

    ''' <summary>
    ''' 这个函数会默认将KEGG里面的物种简写代码进行移除
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Select.TF.BBH",
               Usage:="/Select.TF.BBH /bbh <bbh.csv> /imports <RegPrecise.downloads.DIR> [/out <out.bbh.csv>]")>
    Public Function SelectTFBBH(args As CommandLine) As Integer
        Dim [in] As String = args("/bbh")
        Dim [imports] As String = args("/imports")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".RegPrecise.TF.Csv")
        Dim bbh As IEnumerable(Of BBHIndex) = [in].LoadCsv(Of BBHIndex)
        Dim xmls As IEnumerable(Of String) = ls - l - r - wildcards("*.xml") << args + "/imports"
        Dim bbhHash As Dictionary(Of String, BBHIndex()) =
            (From x As BBHIndex
             In bbh
             Where x.isMatched
             Select x,
                 uid = x.QueryName.Split(":"c).Last  ' 移除KEGG的物种简写代码
             Group By uid Into Group) _
                     .ToDictionary(Function(x) x.uid,
                                   Function(x)
                                       Return x.Group.Select(Function(o) o.x).ToArray
                                   End Function)
        Dim list As New List(Of BBHIndex)

        For Each genome As BacteriaRegulome In (From path As String In xmls Select path.LoadXml(Of BacteriaRegulome))
            Dim regulators As String() = genome.ListRegulators
            list += From sid As String In regulators Where bbhHash.ContainsKey(sid) Select bbhHash(sid)
            Call ".".Echo
        Next

        Return list.SaveTo(out).CLICode
    End Function
End Module
