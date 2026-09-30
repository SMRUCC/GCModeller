Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.Translation

Namespace DeltaSimilarity1998.CAI.XML

    ''' <summary>
    ''' The XML persistable codon adaptation index weight table (the w table built 
    ''' from a reference high-expression gene set H, see 
    ''' <see cref="DeltaSimilarity1998.CAI.CodonWeightTable"/>).
    ''' </summary>
    ''' 
    <XmlRoot("codon-adaptation-index", [Namespace]:="http://gcmodeller.org/analysis/SequenceToolkit/DNA_Comparative/codon-adaptation-index.XML")>
    Public Class CodonAdaptationIndex

        ''' <summary>
        ''' the display name of the reference gene set H
        ''' </summary>
        <XmlElement> Public Property Name As String

        ''' <summary>
        ''' the NCBI transl_table identifier of the genetic code
        ''' </summary>
        <XmlElement> Public Property GeneticCode As GeneticCodes

        ''' <summary>
        ''' number of codons counted in the reference set H
        ''' </summary>
        <XmlElement> Public Property ReferenceCodons As Integer

        ''' <summary>
        ''' the w(codon) weight table: 61 sense codons
        ''' </summary>
        <XmlElement> Public Property Weights As CodonWeight()

        Sub New()
        End Sub

        ''' <summary>
        ''' Build the persistable XML model from the w weight table.
        ''' </summary>
        ''' <param name="table"></param>
        Sub New(table As CodonWeightTable)
            Name = table.Name
            GeneticCode = table.GeneticCode
            Weights = table _
                .GetWeights _
                .Select(Function(w) New CodonWeight(w.Codon, w.AminoAcid, w.Weight)) _
                .ToArray
        End Sub

        ''' <summary>
        ''' Restore the w weight table from this XML model for gene evaluation.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetWeightTable() As CodonWeightTable
            Dim table As New CodonWeightTable(New FastaSeq() {}, GeneticCode, Name)

            For Each w As CodonWeight In Weights
                Call table.SetWeight(w.Codon, w.Weight)
            Next

            Return table
        End Function

        ''' <summary>
        ''' Evaluate the CAI of the given gene against this w table.
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <returns></returns>
        Public Function Evaluate(gene As FastaSeq) As Double
            Return gene.CAI(GetWeightTable())
        End Function

        ''' <summary>
        ''' 对w权重表进行展开: (氨基酸, (密码子, w权重))
        ''' </summary>
        ''' <remarks></remarks>
        Public Function GetCodonBiasList() As KeyValuePair(Of Char, CodonBias)()
            Return Weights _
                .Select(Function(w)
                            Return New KeyValuePair(Of Char, CodonBias)(
                                w.AminoAcid.First,
                                New CodonBias(w.Codon, w.Weight))
                        End Function) _
                .ToArray
        End Function
    End Class

    ''' <summary>
    ''' One w(codon) weight entry of the CAI w table.
    ''' </summary>
    Public Class CodonWeight

        ''' <summary>
        ''' the codon triple string, e.g. ``"ATG"``
        ''' </summary>
        <XmlAttribute> Public Property Codon As String

        ''' <summary>
        ''' the one letter amino acid encoded by this codon
        ''' </summary>
        <XmlAttribute> Public Property AminoAcid As String

        ''' <summary>
        ''' w(codon) = f^H(codon) / max_{synonyms} f^H(.)
        ''' </summary>
        <XmlAttribute> Public Property Weight As Double

        Sub New()
        End Sub

        Sub New(codon As String, aminoAcid As Char, weight As Double)
            Me.Codon = codon
            Me.AminoAcid = aminoAcid
            Me.Weight = weight
        End Sub

        Public Overrides Function ToString() As String
            Return $"{Codon}({AminoAcid}) w={Weight:F4}"
        End Function
    End Class
End Namespace
