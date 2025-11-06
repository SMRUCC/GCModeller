Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.MachineLearning.Transformer
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module EnzymeGenerator

    <Extension>
    Private Sub CreateTrainingSet(enzymes As IEnumerable(Of FastaSeq),
                                  k As Integer,
                                  <Out> ByRef ec_number As List(Of List(Of String)),
                                  <Out> ByRef kmers As List(Of List(Of String)))

        ec_number = New List(Of List(Of String))
        kmers = New List(Of List(Of String))

        For Each seq As FastaSeq In enzymes
            Dim kmer As KSeq() = KSeq.Kmers(seq, k).ToArray

            Call ec_number.Add(seq.Headers(0).Split("."c).AsList)
            Call kmers.Add(kmer.Select(Function(ki) ki.Seq).AsList)
        Next
    End Sub

    <Extension>
    Public Function MakeModel(enzymes As IEnumerable(Of FastaSeq), Optional k As Integer = 3) As TransformerModel
        Dim ec_number As List(Of List(Of String)) = Nothing
        Dim kmers As List(Of List(Of String)) = Nothing

        Call enzymes.CreateTrainingSet(k, ec_number, kmers)

        ' Transformer setup
        Dim batchSize = 2
        Dim embeddingSize = 8
        Dim dk = 4
        Dim dv = 4
        Dim h = 2
        Dim dff = 16
        Dim Nx = 2
        Dim dropout = 0.0
        Dim transformer As New TransformerModel(Nx, embeddingSize, dk, dv, h, dff, batchSize, dropout, ec_number, kmers)
        ' Training
        Dim nrEpochs = 10
        Dim nrTrainingSteps = 10
        Dim learningRate = 0.01

        Call transformer.Train(nrEpochs, nrTrainingSteps, learningRate, batchSize, ec_number, kmers)

        Return transformer
    End Function

    <Extension>
    Public Iterator Function BuildProteinSequence(model As TransformerModel, ec_numbers As IEnumerable(Of String)) As IEnumerable(Of FastaSeq)
        For Each id As String In ec_numbers
            Dim predict = model.Infer(id.Split("."c))
            Dim seq As String

            Yield New FastaSeq With {
                .Headers = {id},
                .SequenceData = seq
            }
        Next
    End Function

End Module
