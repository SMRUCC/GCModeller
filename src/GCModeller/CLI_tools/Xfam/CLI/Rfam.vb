#Region "Microsoft.VisualBasic::9baeda918af5cb0dc1599b64342761fb, CLI_tools\Xfam\CLI\Rfam.vb"

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
    '     Function: LoadCMSearch, LoadDoc, RfamAnalysis, RfamGenomicsContext, RfamRegulatory
    '               RFamRegulons, RfamSites
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Xfam.Rfam
Imports SMRUCC.genomics.Data.Xfam.Rfam.Infernal
Imports SMRUCC.genomics.Data.Xfam.Rfam.Infernal.cmscan
Imports SMRUCC.genomics.Data.Xfam.Rfam.Infernal.cmsearch
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Partial Module CLI

    <ExportAPI("/Rfam",
               Usage:="/Rfam /in <blastMappings.Csv.DIR> /PTT <pttDIR> [/prefix <sp_prefix> /out <out.Rfam.csv> /offset 10 /non-directed]")>
    <ArgumentAttribute("/prefix", True,
                   Description:="Optional for the custom RNA id, is this parameter value is nothing, then the id prefix will be parsed from the PTT file automaticslly.")>
    <Group(Program.RfamCliTools)>
    Public Function RfamAnalysis(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim outCsv As String = args.GetValue("/out", inDIR & ".Csv")
        Dim PTT As String = args("/ptt")
        Dim Rfam As String = GCModeller.FileSystem.Xfam.Rfam.Rfam & "/Rfam.Csv"
        Dim prefix As String = args("/prefix")
        Dim offset As Integer = args.GetValue("/offset", 10)
        Dim nonDirected As Boolean = args.GetBoolean("/non-directed")
        If nonDirected = True Then
            Call $"Will use all direction genes for context analysis...".__DEBUG_ECHO
            nonDirected = False
        End If
        Dim result = RfamAnalysisBatch(
            inDIR, Rfam, PTT,
            locusPrefix:=prefix,
            offset:=offset,
            sourceDirect:=nonDirected) ' 请注意，由于这里的单词的含义是相反的，所以这里需要逻辑反转
        Return result.SaveTo(outCsv).CLICode
    End Function

    ''' <summary>
    ''' 根据blastn的结果和meme的对基因组的扫描分析结果分析出可能的RNA调控功能
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Rfam.Regulatory",
               Usage:="/Rfam.Regulatory /query <RfamilyMappings.csv> /mast <mastsites.csv> [/out <out.csv>]")>
    <Group(Program.RfamCliTools)>
    Public Function RfamRegulatory(args As CommandLine) As Integer
        Dim queryMappings As String = args("/query")
        Dim mast As String = args("/mast")
        Dim out As String = args.GetValue("/out", queryMappings.TrimSuffix & ".Rfam.Regulatory.Csv")
        Dim query = queryMappings.LoadCsv(Of Rfamily)
        Dim mastSites = mast.LoadCsv(Of MastSites)
        Dim regulations = SMRUCC.genomics.Data.Xfam.Rfam.RfamRegulatory.AnalysisRegulatory(query.ToArray, mastSites.ToArray)
        Return regulations.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Load.cmscan", Usage:="/Load.cmscan /in <stdout.txt> [/out <out.Xml>]")>
    <Group(Program.RfamCliTools)>
    Public Function LoadDoc(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Xml")
        Dim Xml As cmscan.ScanSites = [in].LoadCmScan
        Call Xml.QueryHits.hits.Join(Xml.QueryHits.uncertainHits).SaveTo(out.TrimSuffix & ".hits.Csv")
        Return Xml.SaveAsXml(out).CLICode
    End Function

    <ExportAPI("/Load.cmsearch", Usage:="/Load.cmsearch /in <stdio.txt> /out <out.Xml>")>
    <Group(Program.RfamCliTools)>
    Public Function LoadCMSearch(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Xml")
        Dim Xml As cmsearch.SearchSites = [in].LoadCMSearch
        Call Xml.GetDataFrame.SaveTo(out.TrimSuffix & ".hits.Csv")
        Return Xml.SaveAsXml(out).CLICode
    End Function

    <ExportAPI("/Rfam.GenomicsContext",
               Usage:="/Rfam.GenomicsContext /in <scan_sites.Csv> /PTT <genome.PTT> [/dist 500 /out <out.csv>]")>
    <Group(Program.RfamCliTools)>
    Public Function RfamGenomicsContext(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim PTT As String = args("/PTT")
        Dim dist As Integer = args.GetValue("/dist", 500)
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "." & PTT.BaseName & ".Csv")
        Dim genome As PTT = TabularFormat.PTT.Load(PTT)
        Dim sites As IEnumerable(Of HitDataRow) = [in].LoadCsv(Of HitDataRow)
        Dim result As New List(Of HitDataRow)

        For Each site As HitDataRow In sites
            Dim relates = genome.GetRelatedGenes(site.MappingLocation, True, dist)

            If relates.Length = 0 Then
                site.LociDescrib = "intergenic"
                result += site
            Else
                For Each x In relates
                    Dim cl As HitDataRow = site.Copy
                    cl.ORF = x.Gene.Synonym
                    cl.direction = x.Gene.Strand
                    cl.distance = x.ATGDist(site.MappingLocation)
                    cl.LociDescrib = x.ToString

                    result += cl
                Next
            End If
        Next

        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Rfam.Sites.seq",
               Usage:="/Rfam.Sites.Seq /nt <nt.fasta> /sites <sites.csv> [/out out.fasta]")>
    <Group(Program.RfamCliTools)>
    Public Function RfamSites(args As CommandLine) As Integer
        Dim [in] As String = args("/nt")
        Dim sites As String = args("/sites")
        Dim out As String =
            args.GetValue("/out", sites.TrimSuffix & $".{[in].BaseName}.fasta")
        Dim fa As New FASTA.FastaSeq([in])
        Dim parser As IPolymerSequenceModel = fa
        Dim seqs As New List(Of Bac_sRNA.org.Sequence)
        Dim ntTitle As String = fa.Headers.Last.Trim

        For Each hit As HitDataRow In sites.LoadCsv(Of HitDataRow)
            If hit.data("rank").Trim.Last <> "!"c Then
                Continue For
            End If

            Dim seq As SimpleSegment = parser.CutSequenceLinear(hit.MappingLocation)
            Dim tag As String = $"{hit.ORF}:{hit.distance}"
            seqs += New Bac_sRNA.org.Sequence(tag, ntTitle, Strings.Split(hit.data("Query"), " ").First, seq.SequenceData, hit.MappingLocation)
        Next

        Dim faFile As New FASTA.FastaFile(From x As Bac_sRNA.org.Sequence
                                          In seqs
                                          Select x.ToFasta)
        Return faFile.Save(out).CLICode
    End Function

    <ExportAPI("/Rfam.Regulons",
               Usage:="/Rfam.Regulons /in <cmsearch.hits.csv> /regulons <regprecise.regulons.hits.csv> [/out <out.csv>]")>
    <Group(Program.RfamCliTools)>
    Public Function RFamRegulons(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim regulons As String = args - "/regulons"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & "-" & regulons.BaseName & ".Csv")
        Dim rfams As IEnumerable(Of HitDataRow) = [in].LoadCsv(Of HitDataRow)
        Dim operons As IEnumerable(Of RegPreciseOperon) = regulons.LoadCsv(Of RegPreciseOperon)
        Dim LQuery = (From x As HitDataRow
                      In rfams.AsParallel
                      Let sites = (From operon As RegPreciseOperon
                                   In operons  ' 找到在regulon里面同时存在Rfam编号以及ORF的位点联系
                                   Where Not operon.Regulators.IsNullOrEmpty AndAlso
                                       Array.IndexOf(operon.Regulators, x.RfamAcc) > -1 AndAlso
                                       Array.IndexOf(operon.Operon, x.ORF) > -1
                                   Select operon).ToArray
                      Where Not sites.IsNullOrEmpty
                      Select sites,
                          x)
        Dim result As New List(Of HitDataRow)

        For Each Hitass In LQuery
            For Each operon As RegPreciseOperon In Hitass.sites
                Dim rfam As HitDataRow = Hitass.x.Copy

                rfam.data.Add(NameOf(operon.Effector), operon.Effector)
                rfam.data.Add(NameOf(operon.Pathway), operon.Pathway)
                rfam.data.Add("Operon.Strand", operon.Strand)
                rfam.data.Add(NameOf(operon.Operon), operon.Operon.JoinBy("; "))
                rfam.data.Add(NameOf(operon.bbh), operon.bbh.JoinBy("; "))

                result += rfam
            Next
        Next

        Return result.SaveTo(out)
    End Function
End Module
