#Region "Microsoft.VisualBasic::23dc9aaf33cbef58b601a9e372d49e57, analysis\Motifs\CRISPR\CRT\ShellScriptAPI.vb"

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

    ' Module ShellScriptAPI
    ' 
    '     Function: (+2 Overloads) BatchScan, BoyerMoore, CRISPR_Profile, ExportRepeatsLoci, ExportSpacerLoci
    '               ReadCRISPRScanXml, SaveResult, (+4 Overloads) ScanMotif, VNTR_Profile
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.CRISPR.CRT.Output
Imports SMRUCC.genomics.Analysis.CRISPR.CRT.SearchingModel
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

<Package("CRISPR.Search",
                    Url:="http://www.room220.com/crt",
                    Cites:="Bland, C., et al. (2007). ""CRISPR recognition tool (CRT): a tool For automatic detection Of clustered regularly interspaced palindromic repeats."" BMC Bioinformatics 8: 209.
<p>	BACKGROUND: Clustered Regularly Interspaced Palindromic Repeats (CRISPRs) are a novel type of direct repeat found in a wide range of bacteria and archaea. CRISPRs are beginning to attract attention because of their proposed mechanism; that is, defending their hosts against invading extrachromosomal elements such as viruses. Existing repeat detection tools do a poor job of identifying CRISPRs due to the presence of unique spacer sequences separating the repeats. In this study, a new tool, CRT, is introduced that rapidly and accurately identifies CRISPRs in large DNA strings, such as genomes and metagenomes. RESULTS: CRT was compared to CRISPR detection tools, Patscan and Pilercr. In terms of correctness, CRT was shown to be very reliable, demonstrating significant improvements over Patscan for measures precision, recall and quality. When compared to Pilercr, CRT showed improved performance for recall and quality. In terms of speed, CRT proved to be a huge improvement over Patscan. Both CRT and Pilercr were comparable in speed, however CRT was faster for genomes containing large numbers of repeats. CONCLUSION: In this paper a new tool was introduced for the automatic detection of CRISPR elements. This tool, CRT, showed some important improvements over current techniques for CRISPR identification. CRT's approach to detecting repetitive sequences is straightforward. It uses a simple sequential scan of a DNA sequence and detects repeats directly without any major conversion or preprocessing of the input. This leads to a program that is easy to describe and understand; yet it is very accurate, fast and memory efficient, being O(n) in space and O(nm/l) in time.

",
                    Publisher:="Charles Bland* - charles.bland@jsums.edu; <br />
                    Teresa L Ramsey - tramsey@cse.unl.edu; <br />
                    Fareedah Sabree - fareedah.sabree@jsums.edu;<br />
Micheal Lowe - michael.l.lowe@jsums.edu; <br />
                    Kyndall Brown - kyndall.d.brown@jsums.edu; <br />
                    Nikos C Kyrpides - NCKyrpides@lbl.gov;<br />
Philip Hugenholtz - phugenholtz@lbl.gov",
                    Description:="Clustered Regularly Interspersed Short Palindromic Repeats
<br />
<p>These loci consist of direct repeats of around 22-40 bp in length, in between which<br />
are spacer sequences derived from foreign DNA. These spacer sequences are used to<br />
recognize complementary invading DNA so that the CRISPR can target and subsequently <br />
remove the threat. It has been well documented that CRISPRs evolve rapidly in response <br />
to their environment, leading to a wide variation of spacer sequences.
                    <p>
                    Project name: CRISPR Recognition Tool (CRT)<br />
Project home page: http://www.room220.com/crt<br />
Operating system(s): Platform independent<br />
Programming language: Java")>
<Cite(Title:="CRISPR recognition tool (CRT): a tool for automatic detection of clustered regularly interspaced palindromic repeats",
      Keywords:="*Algorithms
*Artificial Intelligence
Base Pairing
Chromosome Mapping/*methods
Interspersed Repetitive Sequences/*genetics
Pattern Recognition, Automated/*methods
Sequence Analysis, DNA/*methods
*Software",
      Authors:="Bland, C.
Ramsey, T. L.
Sabree, F.
Lowe, M.
Brown, K.
Kyrpides, N. C.
Hugenholtz, P.",
      DOI:="10.1186/1471-2105-8-209",
      Abstract:="BACKGROUND: Clustered Regularly Interspaced Palindromic Repeats (CRISPRs) are a novel type of direct repeat found in a wide range of bacteria and archaea. CRISPRs are beginning to attract attention because of their proposed mechanism; that is, defending their hosts against invading extrachromosomal elements such as viruses. 
Existing repeat detection tools do a poor job of identifying CRISPRs due to the presence of unique spacer sequences separating the repeats. In this study, a new tool, CRT, is introduced that rapidly and accurately identifies CRISPRs in large DNA strings, such as genomes and metagenomes. 
<p><p>RESULTS: CRT was compared to CRISPR detection tools, Patscan and Pilercr. In terms of correctness, CRT was shown to be very reliable, demonstrating significant improvements over Patscan for measures precision, recall and quality. 
When compared to Pilercr, CRT showed improved performance for recall and quality. In terms of speed, CRT proved to be a huge improvement over Patscan. Both CRT and Pilercr were comparable in speed, however CRT was faster for genomes containing large numbers of repeats. 
      <p><p>CONCLUSION: In this paper a new tool was introduced for the automatic detection of CRISPR elements. This tool, CRT, showed some important improvements over current techniques for CRISPR identification. 
CRT's approach to detecting repetitive sequences is straightforward. It uses a simple sequential scan of a DNA sequence and detects repeats directly without any major conversion or preprocessing of the input. 
This leads to a program that is easy to describe and understand; yet it is very accurate, fast and memory efficient, being O(n) in space and O(nm/l) in time.",
      Pages:="209",
      AuthorAddress:="Department of Computer Science, Jackson State University, Jackson, MS 39217, USA. charles.bland@jsums.edu",
      Journal:="BMC Bioinformatics",
      ISSN:="1471-2105 (Electronic)
1471-2105 (Linking)",
      Issue:="", Volume:=8, Year:=2007, PubMed:=17577412)>
Public Module ShellScriptAPI

    Dim _innerSeacher As New BoyerMooreAlgorithmSearcher

    <ExportAPI("Boyer_Moore_pattern_exists", Info:="The subject parameter is the pattern that which will be search in the query")>
    Public Function BoyerMoore(query As String, subject As String) As Boolean
        Return _innerSeacher.BoyerMooreSearch(query, pattern:=subject) > -1
    End Function

    <ExportAPI("profile.cirspr")>
    Public Function CRISPR_Profile() As KmerProfile
        Return New KmerProfile With {
            .k = 10,
            .minR = 15,
            .maxR = 72,
            .maxS = 225,
            .minS = 19
        }
    End Function

    <ExportAPI("profile.vntr")>
    Public Function VNTR_Profile() As KmerProfile
        Return New KmerProfile With {
            .k = 5,
            .maxR = 10,
            .minR = 3,
            .minS = 5,
            .maxS = 20
        }
    End Function

    <ExportAPI("Write.Csv.CRISPR")>
    Public Function SaveResult(dat As IEnumerable(Of SearchingModel.CRISPR), saveto$) As Boolean
        Return Export(dat).Save(saveto, False)
    End Function

    ''' <summary>
    ''' 导出来的间隔序列的模型可以用于基因组绘图
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("spacer.export.chromosome_map.loci")>
    Public Function ExportSpacerLoci(model As GenomeScanResult) As SegmentObject()
        Dim LQuery = LinqAPI.Exec(Of SegmentObject) <=
 _
            From site As Output.CRISPR
            In model.Sites
            Let sp_sites = (From sp As Output.Loci
                            In site.SpacerLocis
                            Let loci = New NucleotideLocation(sp.Left, sp.Right, False)
                            Select New SegmentObject(sp.SequenceData, loci))
            Select sp_sites

        Return LQuery
    End Function

    <ExportAPI("repeats.export.chromosome_map.motif_sites")>
    Public Function ExportRepeatsLoci(model As GenomeScanResult) As ComponentModel.Loci.Loci()
        Dim LQuery As ComponentModel.Loci.Loci() =
            LinqAPI.Exec(Of ComponentModel.Loci.Loci) <= From site As Output.CRISPR
                                                         In model.Sites
                                                         Select From rp As Output.Loci
                                                                In site.SpacerLocis
                                                                Select New ComponentModel.Loci.Loci With {
                                                                    .Left = rp.Left,
                                                                    .Right = rp.Right,
                                                                    .TagData = site.ID
                                                                    }
        Return LQuery
    End Function

    <ExportAPI("read.xml.crispr")>
    Public Function ReadCRISPRScanXml(path As String) As GenomeScanResult
        Return path.LoadXml(Of GenomeScanResult)()
    End Function

    <ExportAPI("batch.scan")>
    Public Function BatchScan(source As String, export As String, Optional k As Integer = 8,
                              Optional minR As Integer = 19,
                              Optional maxR As Integer = 38,
                              Optional minS As Integer = 19,
                              Optional maxS As Integer = 48,
                              Optional p As Double = 0.75, Optional MinNumRepeats As Integer = 3) As GenomeScanResult()

        Dim SearchProfile As New KmerProfile With {
            .k = k,
            .maxR = maxR,
            .maxS = maxS,
            .minR = minR,
            .minS = minS
        }
        Return BatchScan(source, export, SearchProfile, p, MinNumRepeats)
    End Function

    <ExportAPI("batch.scan")>
    Public Function BatchScan(source As String,
                              EXPORT As String,
                              profile As KmerProfile,
                              Optional p As Double = 0.75,
                              Optional MinNumRepeats As Integer = 3) As GenomeScanResult()

        Dim SearchProfile As KmerProfile = profile
        Dim Fasta As New FastaFile
        Dim Data As New List(Of GenomeScanResult)

        For Each Path In LoadGbkSource(source).Values
            Dim seq = FastaSeq.LoadNucleotideData(Path.Value)
            If seq Is Nothing Then
                Continue For
            End If
            Dim dat = CRTMotifSearchTool.ExactKMerMatches(
                New NucleicAcid(seq),
                SearchProfile,
                p:=p,
                MinNumberOfRepeats:=MinNumRepeats)
            Call Output.Export(dat).Save(EXPORT & "/" & Path.Name & ".csv", False)
            Dim Xml = GenomeScanResult.CreateObject(seq, Path.Name, dat, SearchProfile)
            Call Xml.GetXml.SaveTo(EXPORT & "/" & Path.Name & ".xml")
            Call Fasta.AddRange(Xml.ExportFasta)
            Call Data.Add(Xml)
        Next

        Call Fasta.Save(EXPORT & "/Motifs.fasta")

        Return Data.ToArray
    End Function

    ''' <summary>
    ''' 分别搜索正链和互补链
    ''' </summary>
    ''' <param name="Sequence"></param>
    ''' <param name="k"></param>
    ''' <param name="minR"></param>
    ''' <param name="maxR"></param>
    ''' <param name="minS"></param>
    ''' <param name="maxS"></param>
    ''' <param name="p"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("-scan.loci")>
    Public Function ScanMotif(Sequence As FastaSeq,
                              Optional k As Integer = 8,
                              Optional minR As Integer = 19,
                              Optional maxR As Integer = 38,
                              Optional minS As Integer = 19,
                              Optional maxS As Integer = 48,
                              Optional p As Double = 0.75,
                              Optional MinNumRepeats As Integer = 3) As SearchingModel.CRISPR()

        Dim profile As New KmerProfile With {
            .k = k,
            .maxR = maxR,
            .maxS = maxS,
            .minR = minR,
            .minS = minS
        }
        Return CRTMotifSearchTool.ExactKMerMatches(New NucleicAcid(Sequence), profile, p:=p, MinNumberOfRepeats:=MinNumRepeats)
    End Function

    ''' <summary>
    ''' 分别搜索正链和互补链
    ''' </summary>
    ''' <param name="Sequence"></param>
    ''' <param name="k"></param>
    ''' <param name="minR"></param>
    ''' <param name="maxR"></param>
    ''' <param name="minS"></param>
    ''' <param name="maxS"></param>
    ''' <param name="p"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <ExportAPI("-scan.loci")>
    Public Function ScanMotif(Sequence As NucleicAcid,
                              Optional k As Integer = 8,
                              Optional minR As Integer = 19,
                              Optional maxR As Integer = 38,
                              Optional minS As Integer = 19,
                              Optional maxS As Integer = 48,
                              Optional p As Double = 0.75, Optional MinNumRepeats As Integer = 3) As SearchingModel.CRISPR()

        Dim profile As New KmerProfile With {
            .k = k,
            .maxR = maxR,
            .maxS = maxS,
            .minR = minR,
            .minS = minS
        }
        Return CRTMotifSearchTool.ExactKMerMatches(Sequence, profile, p:=p, MinNumberOfRepeats:=MinNumRepeats)
    End Function

    <ExportAPI("-scan.loci")>
    Public Function ScanMotif(Sequence As FastaSeq, profile As KmerProfile, Optional p As Double = 0.75, Optional MinNumRepeats As Integer = 3) As SearchingModel.CRISPR()
        Return CRTMotifSearchTool.ExactKMerMatches(New NucleicAcid(Sequence), profile, p:=p, MinNumberOfRepeats:=MinNumRepeats)
    End Function

    <ExportAPI("-scan.loci")>
    Public Function ScanMotif(Sequence As NucleicAcid, profile As KmerProfile, Optional p As Double = 0.75, Optional MinNumRepeats As Integer = 3) As SearchingModel.CRISPR()
        Return CRTMotifSearchTool.ExactKMerMatches(Sequence, profile, p:=p, MinNumberOfRepeats:=MinNumRepeats)
    End Function
End Module
