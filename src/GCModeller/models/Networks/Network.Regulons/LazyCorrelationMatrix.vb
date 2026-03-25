Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Class LazyCorrelationMatrix

    ''' <summary>
    ''' the normalized expression matrix data
    ''' </summary>
    ReadOnly expr As Matrix

    ReadOnly cor As New NamedSparseMatrix
    ReadOnly pval As New NamedSparseMatrix

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="expr">
    ''' the normalized expression matrix data
    ''' </param>
    Sub New(expr As Matrix)
        Me.expr = expr
    End Sub

    Public Function Correlation(gene1 As String, gene2 As String) As (cor As Double, pval As Double)
        If Not cor.CheckElement(gene1, gene2) Then
            Dim c As Double, p As Double
            Dim v1 = expr(gene1)
            Dim v2 = expr(gene2)

            ' no correlation result for missing data
            If v1 Is Nothing OrElse v2 Is Nothing Then
                Return (0, 1)
            End If

            c = Correlations.GetPearson(v1.experiments, v2.experiments, p, throwMaxIterError:=False)

            Call cor.SetValue(gene1, gene2, c)
            Call cor.SetValue(gene2, gene1, c)
            Call pval.SetValue(gene1, gene2, p)
            Call pval.SetValue(gene2, gene1, p)
        End If

        Return (cor(gene1, gene2), pval(gene1, gene2))
    End Function
End Class
