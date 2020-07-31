Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.FuzzyCMeans
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace ExpressionPattern

    ''' <summary>
    ''' 表达模式聚类
    ''' </summary>
    Public Class ExpressionPattern

        Public Property Patterns As FuzzyCMeansEntity()
        Public Property sampleNames As String()
        Public Property [dim] As Integer()

        Public Function GetPartitionMatrix() As IEnumerable(Of Matrix())
            Return populatePartitions(Patterns, [dim], sampleNames)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="dim">``[row, columns]``</param>
        ''' <returns></returns>
        ''' 
        Public Shared Function CMeansCluster(matrix As Matrix, [dim] As Integer()) As ExpressionPattern
            Dim nsize As Integer = [dim](Scan0) * [dim](1)
            Dim sampleNames = matrix.sampleID
            Dim clusters = matrix.expression _
                .AsParallel _
                .Select(Function(gene)
                            Dim vector As New List(Of Double)

                            For i As Integer = 0 To sampleNames.Length - 1
                                Call vector.Add(gene.experiments(i))
                            Next

                            Return New FuzzyCMeansEntity With {
                                .uid = gene.geneID,
                                .Memberships = New Dictionary(Of Integer, Double),
                                .entityVector = vector.ToArray
                            }
                        End Function) _
                .FuzzyCMeans(numberOfClusters:=nsize)

            Return New ExpressionPattern With {
                .Patterns = clusters.ToArray,
                .sampleNames = sampleNames,
                .[dim] = [dim]
            }
        End Function

        Private Shared Iterator Function populatePartitions(clusters As IEnumerable(Of FuzzyCMeansEntity), dim%(), sampleNames As String()) As IEnumerable(Of Matrix())
            Dim row As New List(Of Matrix)

            For Each cluster In clusters.GroupBy(Function(c) c.cluster)
                Dim matrix = New Matrix With {
                    .sampleID = sampleNames,
                    .expression = cluster _
                        .Select(Function(a)
                                    Return New DataFrameRow With {
                                        .geneID = a.uid,
                                        .experiments = a.entityVector
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
    End Class
End Namespace