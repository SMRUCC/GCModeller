Imports Microsoft.VisualBasic.Math.Matrix
Imports std = System.Math

''' <summary>
''' Correlation between the molecules across two omics matrix data, such as gene expression and protein abundance
''' </summary>
Public Class CrossOmicsCorrelation

    Protected ReadOnly cor As New NamedSparseMatrix
    Protected ReadOnly pval As New NamedSparseMatrix

    Public Overridable ReadOnly Property omics1 As String()
    Public Overridable ReadOnly Property omics2 As String()

    Public Property methodName As String = "correlation"

    Sub New()
    End Sub

    Sub New(cor As NamedSparseMatrix, pval As NamedSparseMatrix, omics1 As IEnumerable(Of String), omics2 As IEnumerable(Of String))
        Me.cor = cor
        Me.pval = pval
        Me.omics1 = omics1.ToArray
        Me.omics2 = omics2.ToArray
    End Sub

    ''' <summary>
    ''' 计算跨组学分子的相关性
    ''' </summary>
    ''' <returns></returns>
    Public Function Correlation(i As Integer, j As Integer) As (cor As Double, pval As Double)
        Return Correlation(omics1(i), omics2(j))
    End Function

    ''' <summary>
    ''' 计算跨组学分子的相关性
    ''' </summary>
    ''' <param name="entity1">组学1中的分子名称 (如 Gene ID)</param>
    ''' <param name="entity2">组学2中的分子名称 (如 Protein ID)</param>
    ''' <returns></returns>
    Public Overridable Function Correlation(entity1 As String, entity2 As String) As (cor As Double, pval As Double)
        If Not cor.CheckElement(entity1, entity2) Then
            Return Nothing
        Else
            Return (cor(entity1, entity2), pval(entity1, entity2))
        End If
    End Function

    Public Function CorrelationEdge(entity1 As String, entity2 As String) As Connection
        Dim link = Correlation(entity1, entity2)
        Dim edge As New Connection With {
            .cor = link.cor,
            .gene1 = entity1,
            .gene2 = entity2,
            .interaction = methodName,
            .is_directly = True,
            .pval = link.pval
        }

        Return edge
    End Function

    Public Iterator Function Network(Optional pval_cutoff As Double = 0.05, Optional cor_cutoff As Double = 0.6) As IEnumerable(Of Connection)
        For Each id1 As String In omics1
            For Each id2 As String In omics2
                Dim edge As Connection = CorrelationEdge(id1, id2)

                If edge.pval < pval_cutoff AndAlso std.Abs(edge.cor) > cor_cutoff Then
                    Yield edge
                End If
            Next
        Next
    End Function
End Class
