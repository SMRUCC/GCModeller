Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Correlations

Public Module TRN

    <Extension>
    Public Function CorrelationNetwork(expression As IEnumerable(Of DataSet), Optional cutoff As Double = 0.65) As IEnumerable(Of Connection)
        Dim matrix = expression.ToArray
        Dim samples As String() = matrix.PropertyNames

        cutoff = Math.Abs(cutoff)

        Return matrix _
            .correlationImpl(samples, cutoff) _
            .IteratesALL _
            .Where(Function(cnn)
                       Return Math.Abs(cnn.cor) >= cutoff
                   End Function)
    End Function

    <Extension>
    Private Iterator Function correlationImpl(matrix As DataSet(), sampleNames As String(), cutoff As Double) As IEnumerable(Of Connection())
        Dim fpkm As Double()
        Dim links As Connection()

        For Each gene As DataSet In matrix
            fpkm = gene(sampleNames)
            links = matrix _
                .Where(Function(g) g.ID <> gene.ID) _
                .AsParallel _
                .Select(Function(g)
                            Dim fpkm2 As Double() = g(sampleNames)
                            Dim cor As Double = GetPearson(fpkm, fpkm2)

                            If Math.Abs(cor) >= cutoff Then
                                Return New Connection With {
                                    .cor = cor,
                                    .gene1 = gene.ID,
                                    .gene2 = g.ID,
                                    .is_directly = True
                                }
                            Else
                                Return New Connection With {
                                    .cor = Spearman(fpkm, fpkm2),
                                    .gene1 = gene.ID,
                                    .gene2 = g.ID,
                                    .is_directly = False
                                }
                            End If
                        End Function) _
                .ToArray

            Yield links
        Next
    End Function
End Module

Public Class Connection

    Public Property gene1 As String
    Public Property gene2 As String
    Public Property is_directly As Boolean
    Public Property cor As Double

End Class