#Region "Microsoft.VisualBasic::27f10de68c2cad339fe93c683240aeec, CLI_tools\RegPrecise\CLI\Regulon.vb"

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
'     Function: GetSites
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    <ExportAPI("/Gets.Sites.Genes",
               Usage:="/Gets.Sites.Genes /in <tf.bbh.csv> /sites <motiflogs.csv> [/out <out.csv>]")>
    Public Function GetSites(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim sites As String = args("/sites")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $"-{sites.BaseName}.csv")
        Dim bbh As IEnumerable(Of BBHIndex) = [in].LoadCsv(Of BBHIndex)
        Dim motifLogs As IEnumerable(Of MotifLog) = sites.LoadCsv(Of MotifLog)
        Dim Xmls As IEnumerable(Of String) = FileIO.FileSystem.GetFiles(GCModeller.FileSystem.GetRepositoryRoot & "/RegpreciseDownloads/", FileIO.SearchOption.SearchTopLevelOnly, "*.xml")
        Dim MotifLDMs = (From log As BacteriaRegulome
                         In Xmls.Select(AddressOf LoadXml(Of BacteriaRegulome))
                         Where Not log.regulons Is Nothing AndAlso
                             Not log.regulons.regulators.IsNullOrEmpty
                         Select log)
        Dim result As New List(Of MotifLog)
        Dim bbhhash = BBHIndex.BuildHitsHash(bbh, True)
        Dim motifsHash = (From x As MotifLog
                          In motifLogs
                          Select x
                          Group x By x.Regulog Into Group) _
                               .ToDictionary(Function(x) x.Regulog,
                                             Function(x) x.Group.ToArray)
        Dim logsHash As New List(Of Regulator)

        For Each regulog As BacteriaRegulome In MotifLDMs
            For Each TF As Regulator In regulog.regulons.regulators
                If TF.type <> Types.TF Then
                    Continue For
                End If
                If Not bbhhash.ContainsKey(TF.regulator.name) Then
                    Continue For
                End If
                If Not motifsHash.ContainsKey(TF.regulog.name) Then
                    Continue For
                End If

#If DEBUG Then
                logsHash += TF
#End If

                Dim maps As String() = bbhhash(TF.regulator.name)
                Dim sitesFound As MotifLog() = motifsHash(TF.regulog.name)

                For Each site In sitesFound
                    site.tag = String.Join("; ", maps)
                Next

                result += sitesFound

                Call Console.Write(".")
            Next
        Next

#If DEBUG Then
        Dim test = (From x As Regulator
                    In logsHash
                    Select x
                    Group x By x.Regulog.name Into Group) _
                         .ToDictionary(Function(x) x.name,
                                       Function(x) x.Group.ToArray)
        result = New List(Of MotifLog)(result.OrderBy(Function(x) x.ID))
#End If
        Return result.SaveTo(out)
    End Function

    <ExportAPI("/Export.Regprecise.motifs")>
    <Usage("/Export.Regprecise.motifs /in <dir=genome_regprecise.xml> [/out <motifs.fasta>]")>
    <Description("Export Regprecise motif sites as a single fasta sequence file.")>
    Public Function ExportRegpreciseMotifSites(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimDIR}.motifs.fasta"
        Dim fasta$

        Using writer As StreamWriter = out.OpenWriter(Encodings.ASCII)
            For Each genome As String In ls - l - "*.xml" <= [in]
                Dim data As BacteriaRegulome = genome.LoadXml(Of BacteriaRegulome)
                Dim regulators As Regulator() = data.regulons.regulators

                For Each regulator As Regulator In regulators
                    For Each site In regulator.regulatorySites.Where(Function(motif) Not motif.SequenceData.StringEmpty)
                        fasta = New FastaSeq With {
                            .SequenceData = Regtransbase.WebServices.Regulator.SequenceTrimming(site.SequenceData).Replace("-"c, "N"c),
                            .Headers = {
                                site.locus_tag,
                                site.position,
                                regulator.family,
                                site.bacteria
                            }
                        }.GenerateDocument(-1)

                        Call writer.WriteLine(fasta)
                    Next
                Next

                Call genome.BaseName.__INFO_ECHO
            Next
        End Using

        Return 0
    End Function

    ''' <summary>
    ''' 关联需要注释的蛋白质在Regprecise数据库之中的信息
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/regulators.bbh", Usage:="/regulators.bbh /bbh <bbh.index.Csv> /regprecise <repository.directory> [/out <save.csv>]")>
    <Description("Compiles for the regulators in the bacterial genome mapped on the regprecise database using bbh method.")>
    <Group(CLIGroups.RegulonTools)>
    Public Function RegulatorsBBh(args As CommandLine) As Integer
        Dim in$ = args <= "/bbh"
        Dim repo$ = args <= "/regprecise"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.regprecise.regulators.csv"
        Dim bbh As Dictionary(Of String, BBHIndex()) = [in] _
            .LoadCsv(Of BBHIndex) _
            .Where(Function(map)
                       Return Not map.HitName.StringEmpty AndAlso map.identities > 0
                   End Function) _
            .GroupBy(Function(map) map.QueryName) _
            .ToDictionary(Function(map) map.Key,
                          Function(g)
                              Return g.ToArray
                          End Function)



        For Each genome As BacteriaRegulome In (ls - l - r - "*.Xml" <= repo).Select(AddressOf LoadXml(Of BacteriaRegulome))

        Next
    End Function
End Module
