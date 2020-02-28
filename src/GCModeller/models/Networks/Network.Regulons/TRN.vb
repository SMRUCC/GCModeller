Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.Microarray

Public Module TRN

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="expression"></param>
    ''' <param name="cutoff">An absolute value for the correlation cutoff.</param>
    ''' <returns></returns>
    <Extension>
    Public Function CorrelationNetwork(expression As IEnumerable(Of DataSet), Optional cutoff As Double = 0.65) As IEnumerable(Of Connection)
        Dim matrix As DataSet() = expression.ToArray
        Dim samples As String() = matrix.PropertyNames

        Return matrix _
            .Select(Function(gene)
                        Return gene.CorrelationImpl(matrix, samples, isSelfComparison:=True, skipIndirect:=False, cutoff:=cutoff)
                    End Function) _
            .IteratesALL _
            .Where(Function(cnn)
                       Return Math.Abs(cnn.cor) >= cutoff
                   End Function)
    End Function
End Module