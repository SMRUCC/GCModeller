Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative
Imports SMRUCC.genomics.SequenceModel.FASTA

<Package("sigma_difference",
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
Module SigmaDifference

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
        Return ToolsAPI.GenomeSigmaDifference_p(genome, compare, windowsSize)
    End Function
End Module
