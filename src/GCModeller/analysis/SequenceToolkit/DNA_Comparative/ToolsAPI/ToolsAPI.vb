Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998.CAI
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation
Imports SMRUCC.genomics.SequenceModel.Slicer

<Package("ComparativeGenomics.DeltaStar",
                    Description:="Calculates the nucleotide sequence delta-similarity to measure how closed between the two sequence.",
                    Cites:="Karlin, S., et al. (1998). ""Comparative DNA analysis across diverse genomes."" Annu Rev Genet 32: 185-225.",
                    Publisher:="xie.guigang@gcmodeller.org")>
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
        Dim reader As IPolymerSequenceModel = genbank.Origin.ToFasta
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
                                 SequenceData = reader.CutSequenceLinear(ORF.Location)
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
                                                 Let Sequence As String = reader.CutSequenceLinear(SP, St).SequenceData
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
        Dim LQuery = (From FastaObject In FastaFile.AsParallel Select regionMetaParser(FastaObject)).ToArray
        Return LQuery
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Fasta">>Region1(1492-6218)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    Private Function regionMetaParser(Fasta As FastaSeq) As PartitioningData
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

    ''' <summary>
    ''' Save the CAI w weight table as an xml document.
    ''' </summary>
    ''' <param name="dat"></param>
    ''' <param name="saveXml"></param>
    ''' <returns></returns>
    <ExportAPI("write.xml.cai")>
    Public Function SaveCAI(dat As CAI.XML.CodonAdaptationIndex, saveXml As String) As Boolean
        Return dat.GetXml.SaveTo(saveXml)
    End Function

    ''' <summary>
    ''' Load the CAI w weight table from the xml document.
    ''' </summary>
    ''' <param name="xmlPath"></param>
    ''' <returns></returns>
    <ExportAPI("read.xml.cai")>
    Public Function LoadCAI(xmlPath As String) As CAI.XML.CodonAdaptationIndex
        Return xmlPath.LoadXml(Of CAI.XML.CodonAdaptationIndex)()
    End Function

    ''' <summary>
    ''' Build the CAI w weight table from a reference gene set H 
    ''' (highly expressed genes, e.g. the ribosomal protein genes).
    ''' </summary>
    ''' <param name="referenceGenes"></param>
    ''' <param name="code"></param>
    ''' <param name="name"></param>
    ''' <returns></returns>
    <ExportAPI("CAI.Reference.Build")>
    Public Function BuildCAIReference(referenceGenes As FastaFile,
                                      Optional code As GeneticCodes = GeneticCodes.StandardCode,
                                      Optional name As String = Nothing) As CodonWeightTable
        Return New CodonWeightTable(referenceGenes, code, name)
    End Function

    ''' <summary>
    ''' Evaluate the codon adaptation index of the given gene against the 
    ''' w weight table.
    ''' </summary>
    ''' <param name="gene"></param>
    ''' <param name="reference">w weight table built from the reference gene set H</param>
    ''' <returns></returns>
    <ExportAPI("CAI.Evaluate")>
    Public Function CAI(gene As FastaSeq, reference As CodonWeightTable) As Double
        Return gene.CAI(reference)
    End Function

    ''' <summary>
    ''' Sliding window delta* profile: the delta-difference between each local 
    ''' window of the genome and the comparison sequence.
    ''' 
    ''' PERFORMANCE: implemented with the incremental window count matrix 
    ''' (O(1) update per slide + O(16) distance per sample), and the comparison 
    ''' sequence signature is built only once. This replaces the legacy 
    ''' ``GenomeSigmaDifference_p`` which rebuilt the window and the comparison 
    ''' sequence caches O(n) times.
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="compare"></param>
    ''' <param name="windowsSize">default 1kb window size</param>
    ''' <returns>profile rows ordered by the site position</returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("genome.delta_star_profile")>
    Public Function GenomeDeltaStarProfile(genome As FastaSeq, compare As FastaSeq, Optional windowsSize As Integer = 1000) As WindowDelta()
        Call Console.WriteLine("Start the sliding window delta* profile calculation...")

        Dim reference As New DeltaSimilarity1998.NucleicAcid(compare)
        Dim profile As WindowDelta() = New DeltaSimilarity1998.NucleicAcid(genome) _
            .DeltaStarProfile(reference, windowSize:=windowsSize, stepSize:=1)
        Dim rows As WindowDelta() = profile _
            .Select(Function(w)
                        Return New WindowDelta With {
                            .site = w.Site,
                            .deltaStar = w.DeltaStar,
                            .level = w.Level
                        }
                    End Function) _
            .ToArray

        Call Console.WriteLine("[JOB DONE!] delta* profile created.")

        Return rows
    End Function
End Module

''' <summary>
''' The sliding window calculation cache. 
''' (kept for the legacy script compatibility)
''' </summary>
Public Structure Cache
    Dim SlideWindow As SlideWindow(Of DNA)
    Dim Cache As DeltaSimilarity1998.NucleicAcid
End Structure
