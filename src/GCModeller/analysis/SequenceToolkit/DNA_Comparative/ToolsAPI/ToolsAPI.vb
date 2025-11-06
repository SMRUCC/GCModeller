#Region "Microsoft.VisualBasic::5c2e2b35b8b8c9436de97b628f85e42a, analysis\SequenceToolkit\DNA_Comparative\ToolsAPI\ToolsAPI.vb"

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

    '   Total Lines: 232
    '    Code Lines: 188 (81.03%)
    ' Comment Lines: 21 (9.05%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 23 (9.91%)
    '     File Size: 12.55 KB


    ' Module ToolsAPI
    ' 
    '     Function: __compileCAIBIASCalculationThread, __createTable, __echo, __regionMetaParser, CAI
    '               CompileCAIBIASCalculationThread_p, CreateSimplePartition, GenomeSigmaDifference_p, PartitioningDataFromFasta, SaveCAI
    ' 
    ' Structure Cache
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI.XML
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

<Package("ComparativeGenomics.Sigma-Difference",
                    Description:="Calculates the nucleotide sequence Delta similarity to measure how closed between the two sequence.",
                    Cites:="Karlin, S., et al. (1998). ""Comparative DNA analysis across diverse genomes."" Annu Rev Genet 32: 185-225.
	We review concepts and methods for comparative analysis of complete genomes including assessments of genomic compositional contrasts based on dinucleotide and tetranucleotide relative abundance values, identifications of rare and frequent oligonucleotides, evaluations and interpretations of codon biases in several large prokaryotic genomes, and characterizations of compositional asymmetry between the two DNA strands in certain bacterial genomes. The discussion also covers means for identifying alien (e.g. laterally transferred) genes and detecting potential specialization islands in bacterial genomes.

", Publisher:="xie.guigang@gcmodeller.org")>
<Cite(Title:="Comparative DNA analysis across diverse genomes",
      Journal:="Annu Rev Genet", PubMed:=9928479,
      Pages:="185-225",
      AuthorAddress:="Department of Mathematics, Stanford University, California 94305-2125, USA.",
      Abstract:="We review concepts and methods for comparative analysis of complete genomes including assessments of genomic compositional contrasts based on dinucleotide and tetranucleotide relative abundance values, identifications of rare and frequent oligonucleotides, evaluations and interpretations of codon biases in several large prokaryotic genomes, and characterizations of compositional asymmetry between the two DNA strands in certain bacterial genomes. 
The discussion also covers means for identifying alien (e.g. laterally transferred) genes and detecting potential specialization islands in bacterial genomes.",
      Authors:="Karlin, S.
Campbell, A. M.
Mrazek, J.",
      Keywords:="Animals
Bacteria/genetics
Base Composition
Base Sequence
Codon/genetics
DNA/chemistry/*genetics
DNA, Bacterial/genetics
Eukaryotic Cells
*Genome
Genome, Bacterial
Prokaryotic Cells
Species Specificity",
      DOI:="10.1146/annurev.genet.32.1.185", ISSN:="0066-4197 (Print)
0066-4197 (Linking)", Issue:="", Volume:=32, Year:=1998)>
Public Module ToolsAPI

    <ExportAPI("Simple.Partition.Create")>
    Public Function CreateSimplePartition(genbank As GBFF.File, data As IEnumerable(Of ChromosomePartitioningEntry)) As PartitioningData()
        Dim Reader As IPolymerSequenceModel = genbank.Origin.ToFasta
        Dim dGroup = From x As ChromosomePartitioningEntry
                     In data
                     Select x
                     Group By x.PartitioningTag Into Group
        Dim Partitions = (From part
                          In dGroup
                          Let id As String() = part.Group.Select(Function(x) x.ORF).ToArray
                          Select part.PartitioningTag,
                              id).ToArray
        Dim ORFPartitions = From ORF As GeneBrief
                            In genbank.GbffToPTT(ORF:=True).GeneObjects
                            Let InternalGetPTag = (From p
                                                   In Partitions
                                                   Where Array.IndexOf(p.id, ORF.Synonym) > -1
                                                   Select p.PartitioningTag).FirstOrDefault
                            Select ORF,
                                 InternalGetPTag,
                                 SequenceData = Reader.CutSequenceLinear(ORF.Location)
                            Group By InternalGetPTag Into Group
        Dim LQuery As PartitioningData() =
            LinqAPI.Exec(Of PartitioningData) <= From pInfo
                                                 In ORFPartitions
                                                 Let Loci = New IntRange((From ORF As GeneBrief
                                                                          In pInfo.Group.Select(Function(x) x.ORF)
                                                                          Let pt As NucleotideLocation = ORF.Location
                                                                          Select {pt.left, pt.right}).IteratesALL)
                                                 Let St As Integer = Loci.Max
                                                 Let SP As Integer = Loci.Min
                                                 Let Sequence As String = Reader.CutSequenceLinear(SP, St).SequenceData
                                                 Select New PartitioningData With {
                                                     .GenomeID = genbank.Accession.AccessionId,
                                                     .LociLeft = SP,
                                                     .LociRight = St,
                                                     .ORFList = (From ORF In pInfo.Group Select ORF.ORF.Synonym).ToArray,
                                                     .PartitioningTag = pInfo.InternalGetPTag,
                                                     .SequenceData = Sequence
                                                 }
        Return LQuery
    End Function

    ''' <summary>
    ''' >Region1(1492-6218)
    ''' </summary>
    ''' <param name="Fasta"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("PartitioningData.From.Fasta")>
    Public Function PartitioningDataFromFasta(<Parameter("Path.Nt.Fasta")> Fasta As String) As PartitioningData()
        Dim FastaFile As FastaFile = FastaFile.Read(Fasta)
        Dim LQuery = (From FastaObject In FastaFile.AsParallel Select __regionMetaParser(FastaObject)).ToArray
        Return LQuery
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Fasta">>Region1(1492-6218)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function __regionMetaParser(Fasta As FastaSeq) As PartitioningData
        Dim Loci As Integer() = (From m As Match
                                 In Regex.Matches(Regex.Match(Fasta.Title, "\(\d+[-]\d+\)").Value, "\d+")
                                 Select CInt(Val(m.Value))).ToArray
        Dim Left = Loci.First
        Dim Right = Loci.Last
        Dim pData As New PartitioningData With {
            .PartitioningTag = Fasta.Title,
            .LociLeft = Left,
            .LociRight = Right,
            .SequenceData = Fasta.SequenceData
        }
        Return pData
    End Function

    <ExportAPI("CAI")>
    Public Function CAI(ORF As FastaSeq) As CodonAdaptationIndex
        Return New CodonAdaptationIndex(New RelativeCodonBiases(ORF))
    End Function

    <ExportAPI("write.xml.cai")>
    Public Function SaveCAI(dat As CodonAdaptationIndex, saveXml As String) As Boolean
        Return dat.GetXml.SaveTo(saveXml)
    End Function

    Public Function __compileCAIBIASCalculationThread(gene_source As FastaFile, WorkTemp As String, InternalID As String) As KeyValuePair(Of String, CodonAdaptationIndex)()
        Dim ResultList = New List(Of KeyValuePair(Of String, CodonAdaptationIndex))

        For i As Integer = 0 To gene_source.Count - 1
            Dim Sequence As FastaSeq = gene_source(i)
            Dim Path As String = String.Format("({0}){1}", InternalID, Sequence.Headers.First.NormalizePathString)
            Dim SeqID As String = Path
            Dim CAIData As CodonAdaptationIndex

            Path = WorkTemp & "/" & Path & ".xml"

            If FileIO.FileSystem.FileExists(Path) Then
                CAIData = Path.LoadXml(Of CodonAdaptationIndex)()
            Else
                CAIData = New CodonAdaptationIndex(New RelativeCodonBiases(Sequence))
                Call CAIData.GetXml.SaveTo(Path)
            End If

            Call ResultList.Add(New KeyValuePair(Of String, CodonAdaptationIndex)(SeqID, CAIData))
            Call Console.Write("{0}  ==>{1}%", SeqID, i / gene_source.Count * 100)
        Next

        Return ResultList.ToArray
    End Function

    Public Function CompileCAIBIASCalculationThread_p(gene_source As FastaFile, WorkTemp As String, InternalID As String) As KeyValuePair(Of String, CodonAdaptationIndex)()
        Dim ResultList = (From Sequence As FastaSeq
                          In gene_source.AsParallel
                          Let Path As String = String.Format("({0}){1}", InternalID, Sequence.Headers.First.NormalizePathString)
                          Let SeqID As String = Path
                          Let CAIData As CodonAdaptationIndex = __createTable(WorkTemp, Path, Sequence, SeqID)
                          Select New KeyValuePair(Of String, CodonAdaptationIndex)(SeqID, CAIData)).ToArray
        Return ResultList.ToArray
    End Function

    Private Function __createTable(workTMP As String, path As String, Sequence As FastaSeq, seqId As String) As CodonAdaptationIndex
        Dim XMLPath = workTMP & "/" & path & ".xml"
        Dim da As CodonAdaptationIndex

        If FileIO.FileSystem.FileExists(XMLPath) Then
            da = XMLPath.LoadXml(Of CodonAdaptationIndex)()
        Else
            da = New CodonAdaptationIndex(New RelativeCodonBiases(Sequence))
            Call da.GetXml.SaveTo(XMLPath)
        End If

        Call $"""{seqId}"" job done!!!".debug

        Return da
    End Function

    ''' <summary>
    ''' 并行版本的计算函数
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="windowsSize">默认为1kb的长度</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("genome.sigma_diff")>
    Public Function GenomeSigmaDifference_p(genome As FastaSeq, compare As FastaSeq, Optional windowsSize As Integer = 1000) As SiteSigma()
        Call Console.WriteLine("Start to create slide window for difference calculation.")
        Dim Windows = New NucleotideModels.NucleicAcid(genome).ToArray.CreateSlideWindows(windowsSize)
        Call Console.WriteLine("Creation job done!")
        Call Console.WriteLine("Start to create the sigma data collection...")

        Using pb As New CBusyIndicator(start:=True)
            Dim LQuery = (From segment In Windows.AsParallel
                          Let x = New NucleotideModels.NucleicAcid(segment.Items)
                          Let y = New NucleotideModels.NucleicAcid(compare)
                          Let Sigma = DifferenceMeasurement.Sigma(x, y)
                          Let p = New SiteSigma With {
                              .Site = __echo(segment, Windows.Count),
                              .Sigma = Sigma,
                              .Similarity = DifferenceMeasurement.SimilarDescription(Sigma)
                          }
                          Select p
                          Order By p.Site Ascending).ToArray
            Call Console.WriteLine("[JOB DONE!] Generating output document...")
            Return LQuery
        End Using
    End Function

    Private Function __echo(segment As SlideWindow(Of DNA), numWins As Integer) As Integer
        Call $"{100 * segment.Index / numWins}%".debug
        Return segment.Index
    End Function

End Module

Public Structure Cache
    Dim SlideWindow As SlideWindow(Of DNA)
    Dim Cache As DeltaSimilarity1998.NucleicAcid
End Structure
