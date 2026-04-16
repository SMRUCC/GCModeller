Imports System.IO
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Class LazyCrossOmicsCorrelation

    ''' <summary>
    ''' the normalized expression matrix data of Omics 1
    ''' </summary>
    ReadOnly expr1 As Matrix

    ''' <summary>
    ''' the normalized expression matrix data of Omics 2
    ''' </summary>
    ReadOnly expr2 As Matrix

    ReadOnly cor As New NamedSparseMatrix
    ReadOnly pval As New NamedSparseMatrix

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="expr1">组学1的表达矩阵</param>
    ''' <param name="expr2">组学2的表达矩阵</param>
    Sub New(expr1 As Matrix, expr2 As Matrix, Optional strict As Boolean = True)
        Me.expr1 = expr1
        Me.expr2 = expr2

        ' 跨组学计算相关性要求两个矩阵的样本（实验/列）必须是对齐的！
        ' 即 expr1 的第 i 列和 expr2 的第 i 列必须是同一个样本。
        Dim intersects As String() = expr1.sampleID.Intersect(expr2.sampleID).ToArray

        If intersects.Length <> expr1.sampleID.Length Then
            If strict Then
                Throw New InvalidDataException($"sample dimension of omics data 1 is not equals to the sample dimension of the omics data 2!")
            Else
                expr1 = expr1.Project(intersects)
                expr2 = expr2.Project(intersects)
            End If
        Else
            If Not expr1.sampleID.SequenceEqual(expr2.sampleID) Then
                expr1 = expr1.Project(intersects)
                expr2 = expr2.Project(intersects)
            End If
        End If
    End Sub

    ''' <summary>
    ''' 计算跨组学分子的相关性
    ''' </summary>
    ''' <param name="entity1">组学1中的分子名称 (如 Gene ID)</param>
    ''' <param name="entity2">组学2中的分子名称 (如 Protein ID)</param>
    ''' <returns></returns>
    Public Function Correlation(entity1 As String, entity2 As String) As (cor As Double, pval As Double)
        ' 检查缓存中是否已经计算过 (组学1分子在前，组学2分子在后)
        If Not cor.CheckElement(entity1, entity2) Then
            Dim c As Double, p As Double
            Dim v1 = expr1(entity1)
            Dim v2 = expr2(entity2)

            ' no correlation result for missing data
            If v1 Is Nothing OrElse v2 Is Nothing Then
                Return (0, 1)
            End If

            ' 计算皮尔逊相关性
            c = Correlations.GetPearson(v1.experiments, v2.experiments, p, throwMaxIterError:=False)

            ' 跨组学矩阵是不对称的，不需要像单组学那样同时写入反向 [entity2, entity1]
            ' 除非希望能够通过 Protein -> Gene 反向查询，如果内存允许，也可以加上反向缓存：
            ' Call cor.SetValue(entity2, entity1, c)
            ' Call pval.SetValue(entity2, entity1, p)

            Call cor.SetValue(entity1, entity2, c)
            Call pval.SetValue(entity1, entity2, p)
        End If

        Return (cor(entity1, entity2), pval(entity1, entity2))
    End Function
End Class
