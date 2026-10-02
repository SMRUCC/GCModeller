Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation

Namespace DeltaSimilarity1998.CAI

    ''' <summary>
    ''' CODON ADAPTATION INDEX (CAI, Sharp &amp; Li) weight table.
    ''' 
    ''' This specifies a reference set of genes, ``H``, chosen from among 
    ''' "highly expressed genes". Defining 
    ''' 
    ''' ```
    '''    w(codon) = f^H(codon) / max_{synonyms} f^H(.)
    ''' ```
    ''' 
    ''' as the ratio of the frequency of the codon to the maximal codon frequency in 
    ''' ``H`` for the same amino acid, the CAI of a gene of length L is taken as 
    ''' 
    ''' ```
    '''    CAI(gene) = (w1 * w2 * ... * wL)^(1/L)
    ''' ```
    ''' 
    ''' (the geometric mean, computed numerically through the log-average).
    ''' 
    ''' This is a two-step API: first build the w table from the reference gene set 
    ''' ``H`` (this class), then evaluate the target genes 
    ''' (see <see cref="CodonAdaptationIndexEvaluation.CAI(FastaSeq, CodonWeightTable)"/>).
    ''' 
    ''' (compatible rename of the legacy ``RelativeCodonBiases`` class, which 
    ''' mistakenly used each single ORF as its own reference set and thus always 
    ''' produced CAI values close to 1)
    ''' </summary>
    Public Class CodonWeightTable

        ''' <summary>
        ''' the display name of the reference gene set H
        ''' </summary>
        Public Property Name As String
        Public Property GeneticCode As GeneticCodes

        ''' <summary>
        ''' w values indexed by the packed codon index 
        ''' ``((b1*4 + b2)*4 + b3)`` (base index A=0, G=1, C=2, T=3)
        ''' </summary>
        Friend ReadOnly weights As Double() = New Double(63) {}
        Friend ReadOnly codonHash As Codon() = Codon.CreateHashTable
        Friend totalCodonCount As Integer

        ''' <summary>
        ''' Total number of sense codons counted in the reference set H.
        ''' </summary>
        Public ReadOnly Property TotalCodons As Integer
            Get
                Return totalCodonCount
            End Get
        End Property

        ''' <summary>
        ''' Pseudocount assigned to the codons which are absent in the reference set H, 
        ''' expressed as an absolute count. Prevents the geometric mean from being 
        ''' zeroed out by a single unused codon.
        ''' </summary>
        Public ReadOnly Property PseudoCount As Double

        Sub New(referenceGenes As IEnumerable(Of FastaSeq),
                Optional code As GeneticCodes = GeneticCodes.StandardCode,
                Optional name As String = Nothing,
                Optional pseudoCount As Double = 0.5)

            Me.GeneticCode = code
            Me.Name = If(name, "CAI-reference-set")
            Me.PseudoCount = pseudoCount
            Call Build(referenceGenes, code)
        End Sub

        Private Sub Build(referenceGenes As IEnumerable(Of FastaSeq), code As GeneticCodes)
            Dim table As TranslTable = TranslTable.GetTable(code)
            Dim counts As Double() = New Double(63) {}

            ' 1. count the codon usage f^H(codon) of the reference gene set H
            For Each gene As FastaSeq In referenceGenes
                Dim nt As DNA() = New NucleotideModels.NucleicAcid(gene, strict:=False).ToArray
                Dim n As Integer = nt.Length - nt.Length Mod 3

                For i As Integer = 0 To n - 1 Step 3
                    Dim b1 As Integer = NucleicAcid.BaseIndex(nt(i))
                    Dim b2 As Integer = NucleicAcid.BaseIndex(nt(i + 1))
                    Dim b3 As Integer = NucleicAcid.BaseIndex(nt(i + 2))

                    If b1 < 0 OrElse b2 < 0 OrElse b3 < 0 Then
                        Continue For
                    End If

                    Dim hashCode As Integer = (b1 + 1) * 1000 + (b2 + 1) * 100 + (b3 + 1) * 10000

                    ' stop codons are excluded from the reference set
                    If Array.IndexOf(table.StopCodons, hashCode) > -1 Then
                        Continue For
                    End If

                    counts((b1 * 4 + b2) * 4 + b3) += 1
                Next
            Next

            totalCodonCount = CInt(counts.Sum)

            ' 2. w(codon) = f^H(codon) / max_{synonyms} f^H(.)
            Dim codonHash As Codon() = Codon.CreateHashTable
            Dim stopCodons As Integer() = table.StopCodons
            Dim senseCodons As Codon() = codonHash _
                .Where(Function(c) Array.IndexOf(stopCodons, c.TranslHashCode) = -1) _
                .ToArray

            For Each codon As Codon In senseCodons
                Dim idx As Integer = CodonUsageProfile.CodonIndex(codon)

                If idx < 0 Then
                    Continue For
                End If

                Dim aa As Char = TranslationTable.Translate(codon, code)
                Dim synonyms As Codon() = senseCodons _
                    .Where(Function(c) TranslationTable.Translate(c, code) = aa) _
                    .ToArray
                Dim max As Double = synonyms _
                    .Select(Function(c) counts(CodonUsageProfile.CodonIndex(c))) _
                    .Max

                If max <= 0 Then
                    ' the whole synonymous group is absent in H: no preference known
                    weights(idx) = 0
                ElseIf counts(idx) <= 0 Then
                    ' codon absent in H: assign the pseudocount floor
                    weights(idx) = PseudoCount / max
                Else
                    weights(idx) = counts(idx) / max
                End If
            Next
        End Sub

        ''' <summary>
        ''' The w(codon) weight value of the given codon.
        ''' </summary>
        ''' <param name="codon"></param>
        ''' <returns></returns>
        Public Function Weight(codon As Codon) As Double
            Dim idx As Integer = CodonUsageProfile.CodonIndex(codon)

            If idx < 0 Then
                Return 0
            Else
                Return weights(idx)
            End If
        End Function

        ''' <summary>
        ''' The w(codon) weight value of the given codon triple string, e.g. ``"ATG"``.
        ''' </summary>
        ''' <param name="codon"></param>
        ''' <returns></returns>
        Public Function Weight(codon As String) As Double
            If codon.Length <> 3 Then
                Return 0
            End If

            Dim b1 As Integer = BaseIndexOf(codon(0))
            Dim b2 As Integer = BaseIndexOf(codon(1))
            Dim b3 As Integer = BaseIndexOf(codon(2))

            If b1 < 0 OrElse b2 < 0 OrElse b3 < 0 Then
                Return 0
            Else
                Return weights((b1 * 4 + b2) * 4 + b3)
            End If
        End Function

        Private Shared Function BaseIndexOf(c As Char) As Integer
            Select Case Char.ToUpper(c)
                Case "A"c : Return 0
                Case "G"c : Return 1
                Case "C"c : Return 2
                Case "T"c, "U"c : Return 3
                Case Else : Return -1
            End Select
        End Function

        ''' <summary>
        ''' Enumerate the w weight entries of all 61 sense codons.
        ''' </summary>
        ''' <returns></returns>
        Friend Function GetWeights() As (Codon As String, AminoAcid As Char, Weight As Double)()
            Dim list As New List(Of (Codon As String, AminoAcid As Char, Weight As Double))

            For Each codon As Codon In codonHash
                Dim idx As Integer = CodonUsageProfile.CodonIndex(codon)

                If idx < 0 OrElse weights(idx) <= 0 Then
                    Continue For
                End If

                Call list.Add((codon.CodonValue,
                               TranslationTable.Translate(codon, GeneticCode),
                               weights(idx)))
            Next

            Return list.ToArray
        End Function

        ''' <summary>
        ''' Set/override the w weight of the given codon triple string 
        ''' (used by the XML model restoration).
        ''' </summary>
        ''' <param name="codon"></param>
        ''' <param name="weight"></param>
        Friend Sub SetWeight(codon As String, weight As Double)
            For Each c As Codon In codonHash
                If c.CodonValue = codon.ToUpper Then
                    Dim idx As Integer = CodonUsageProfile.CodonIndex(c)

                    If idx >= 0 Then
                        weights(idx) = weight
                    End If

                    Return
                End If
            Next
        End Sub

        ''' <summary>
        ''' Build the w weight table from a fasta file of the reference 
        ''' high-expression gene set H.
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="code"></param>
        ''' <param name="name"></param>
        ''' <param name="pseudoCount"></param>
        ''' <returns></returns>
        Public Shared Function FromFasta(path As String,
                                         Optional code As GeneticCodes = GeneticCodes.StandardCode,
                                         Optional name As String = Nothing,
                                         Optional pseudoCount As Double = 0.5) As CodonWeightTable
            Dim file As FastaFile = FastaFile.Read(path)
            Return New CodonWeightTable(file, code, name, pseudoCount)
        End Function

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

    ''' <summary>
    ''' Evaluation of the codon adaptation index against a 
    ''' <see cref="CodonWeightTable"/> reference set.
    ''' </summary>
    Public Module CodonAdaptationIndexEvaluation

        ''' <summary>
        ''' CAI(gene) = (w1 * w2 * ... * wL)^(1/L), the geometric mean of the 
        ''' w weights of all codons in the gene. Stop codons (and any trailing 
        ''' incomplete codon) are excluded from the average.
        ''' </summary>
        ''' <param name="gene">the CDS nucleotide sequence of the target gene</param>
        ''' <param name="w">the w weight table built from the reference gene set H</param>
        ''' <returns>the CAI value in the range [0, 1]; -1 when the gene contains no valid codon</returns>
        <Extension>
        Public Function CAI(gene As FastaSeq, w As CodonWeightTable) As Double
            Dim nt As DNA() = New NucleotideModels.NucleicAcid(gene, strict:=False).ToArray
            Dim n As Integer = nt.Length - nt.Length Mod 3
            Dim sumLog As Double = 0
            Dim count As Integer = 0

            For i As Integer = 0 To n - 1 Step 3
                Dim b1 As Integer = NucleicAcid.BaseIndex(nt(i))
                Dim b2 As Integer = NucleicAcid.BaseIndex(nt(i + 1))
                Dim b3 As Integer = NucleicAcid.BaseIndex(nt(i + 2))

                If b1 < 0 OrElse b2 < 0 OrElse b3 < 0 Then
                    Continue For
                End If

                Dim wi As Double = w.weights((b1 * 4 + b2) * 4 + b3)

                If wi <= 0 Then
                    ' codon with no known preference (absent synonymous group): 
                    ' excluded from the geometric average
                    Continue For
                End If

                sumLog += Math.Log(wi)
                count += 1
            Next

            If count = 0 Then
                Return -1
            Else
                Return Math.Exp(sumLog / count)
            End If
        End Function

        ''' <summary>
        ''' CAI of every gene in the fasta file.
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="w"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CAIProfile(genes As FastaFile, w As CodonWeightTable) As IEnumerable(Of NamedValue(Of Double))
            Return From gene As FastaSeq
                   In genes.AsParallel
                   Select New NamedValue(Of Double) With {
                       .Name = gene.Title,
                       .Value = gene.CAI(w)
                   }
        End Function
    End Module
End Namespace
