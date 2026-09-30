Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation

Namespace DeltaSimilarity1998

    ''' <summary>
    ''' CODON BIAS DIFFERENCE MEASURE B(F|C) (Karlin, Campbell &amp; Mrazek, 1998, eq. [1])
    ''' 
    ''' A measure of the difference in codon usage between a gene family F and a 
    ''' reference class C:
    ''' 
    ''' ```
    '''    B(F|C) = SUM_a p_a(F) * SUM_{(x,y,z)->a} | f(x, y, z) - c(x, y, z) |
    ''' ```
    ''' 
    ''' where ``p_a(F)`` is the frequency of the amino acid ``a`` in F, ``f(x,y,z)`` 
    ''' the codon frequency in F and ``c(x,y,z)`` the codon frequency in C. 
    ''' The measure is asymmetric: F is the "query" object and C the "reference ruler".
    ''' 
    ''' The reference class C can be built by: (1) functional/cellular categories; 
    ''' (2) genes aggregated on genomic contigs; or (3) k-clustering in the 
    ''' 61-dim codon usage space (paper section 6).
    ''' </summary>
    <Package("Codon.Bias.Measure",
             Description:="B(F|C) inter-class codon usage bias difference measure (Karlin 1998 eq.[1])")>
    Public Module CodonBiasMeasure

        ''' <summary>
        ''' ``B(F|C) = SUM_a p_a(F) * SUM_{(x,y,z)->a} | f(x,y,z) - c(x,y,z) |``
        ''' </summary>
        ''' <param name="f">the query codon usage profile (gene family F)</param>
        ''' <param name="c">the reference codon usage profile (class C)</param>
        ''' <returns></returns>
        <ExportAPI("B.FC")>
        Public Function BiasDifference(f As CodonUsageProfile, c As CodonUsageProfile) As Double
            Dim table As TranslTable = TranslTable.GetTable(f.code)
            Dim aaWeightedSum As Double = 0
            Dim codonHash As Codon() = Codon.CreateHashTable
            ' group the sense codons by their translated amino acid
            Dim groups As IGrouping(Of Char, Codon)() = codonHash _
                .Where(Function(codon)
                           Dim hash = codon.TranslHashCode
                           Return Array.IndexOf(table.StopCodons, hash) = -1
                       End Function) _
                .GroupBy(Function(codon) TranslationTable.Translate(codon, f.code)) _
                .ToArray

            For Each aaGroup In groups
                Dim pa As Double = f.AminoAcidFrequency(aaGroup.Key)

                If pa <= 0 Then
                    ' amino acid not used in F: contributes nothing to the weighted sum
                    Continue For
                End If

                Dim aaSum As Double = 0

                For Each codon As Codon In aaGroup
                    aaSum += Math.Abs(f.Relative(codon) - c.Relative(codon))
                Next

                aaWeightedSum += pa * aaSum
            Next

            Return aaWeightedSum
        End Function

        ''' <summary>
        ''' ``B(g|C)``: the codon usage bias difference of one single gene g 
        ''' against the reference class C.
        ''' </summary>
        ''' <param name="gene">the CDS nucleotide sequence of the query gene g</param>
        ''' <param name="reference">the reference class C codon usage profile</param>
        ''' <returns></returns>
        <Extension>
        <ExportAPI("B.Gene")>
        Public Function BiasDifference(gene As FastaSeq, reference As CodonUsageProfile) As Double
            Dim f As CodonUsageProfile = CodonUsageProfile.FromGenes(
                {gene}, reference.code, name:=$"gene:{gene.Title}")
            Return BiasDifference(f, reference)
        End Function

        ''' <summary>
        ''' ``B(g|C)`` for a batch of genes. The per-gene codon counting is a single 
        ''' pass over each gene sequence, so the batch is parallelized at the gene level.
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="reference"></param>
        ''' <returns></returns>
        <Extension>
        Public Function BiasDifference(genes As FastaFile, reference As CodonUsageProfile) As NamedValue(Of Double)()
            Return genes _
                .AsParallel() _
                .Select(Function(gene)
                            Return New NamedValue(Of Double) With {
                                .Name = gene.Title,
                                .Value = gene.BiasDifference(reference)
                            }
                        End Function) _
                .ToArray
        End Function
    End Module
End Namespace
