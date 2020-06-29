Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

''' <summary>
''' 表达模式聚类
''' </summary>
Public Module ExpressionPatterns

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="dim">``[row, columns]``</param>
    ''' <returns></returns>
    Public Function KMeansCluster(matrix As Matrix, [dim] As Integer()) As Matrix()()
        Dim nsize As Integer = [dim](Scan0) * [dim](1)
        Dim sampleNames = matrix.sampleID
        Dim clusters = matrix.expression _
            .AsParallel _
            .Select(Function(gene)
                        Dim vector As New Dictionary(Of String, Double)

                        For i As Integer = 0 To sampleNames.Length - 1
                            Call vector.Add(sampleNames(i), gene.experiments(i))
                        Next

                        Return New DataSet With {.ID = gene.geneID, .Properties = vector}
                    End Function) _
            .ToKMeansModels _
            .Kmeans(expected:=nsize)

        Return clusters.populatePartitions([dim], sampleNames).ToArray
    End Function

    <Extension>
    Private Iterator Function populatePartitions(clusters As IEnumerable(Of EntityClusterModel), dim%(), sampleNames As String()) As IEnumerable(Of Matrix())
        Dim row As New List(Of Matrix)

        For Each cluster In clusters.GroupBy(Function(c) c.Cluster)
            Dim matrix = New Matrix With {
                .sampleID = sampleNames,
                .expression = cluster _
                    .Select(Function(a)
                                Return New DataFrameRow With {
                                    .geneID = a.ID,
                                    .experiments = sampleNames _
                                        .Select(Function(name) a(name)) _
                                        .ToArray
                                }
                            End Function) _
                    .ToArray
            }

            row += matrix

            If row = [dim](1) Then
                Yield row.PopAll
            End If
        Next

        If row > 0 Then
            Yield row.PopAll
        End If
    End Function
End Module
