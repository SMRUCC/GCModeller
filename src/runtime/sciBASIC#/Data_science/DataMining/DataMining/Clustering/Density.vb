Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Clustering

    ''' <summary>
    ''' evaluate point density
    ''' </summary>
    Public Class Density

        ''' <summary>
        ''' 密度为到k个最近邻的平均距离的倒数。如果该距离小，则密度高
        ''' </summary>
        ''' <param name="dataset"></param>
        ''' <param name="k"></param>
        ''' <returns>
        ''' the larger of the result value, the higher density value it is
        ''' </returns>
        Public Shared Iterator Function GetDensity(dataset As IEnumerable(Of ClusterEntity), Optional k As Integer = 6) As IEnumerable(Of NamedValue(Of Double))
            Dim raw = dataset.ToArray

            For Each row As ClusterEntity In raw
                Dim d As Double() = raw _
                    .AsParallel _
                    .Select(Function(r)
                                Return KMeans.EuclideanDistance(r.entityVector, row.entityVector)
                            End Function) _
                    .OrderBy(Function(di) di) _
                    .ToArray
                Dim nearest As Double() = d.Take(k).ToArray
                Dim mean As Double = nearest.Average

                Yield New NamedValue(Of Double) With {
                    .Name = row.uid,
                    .Value = 1 / mean,
                    .Description = nearest.GetJson
                }
            Next
        End Function
    End Class
End Namespace