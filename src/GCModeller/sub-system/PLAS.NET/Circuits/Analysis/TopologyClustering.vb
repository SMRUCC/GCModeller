Imports Microsoft.VisualBasic.DataMining.Evaluation
Imports Microsoft.VisualBasic.DataMining.Hamming

Namespace Analysis

    ''' <summary>
    ''' The Hamming clustering of the functional topologies: the topologies are
    ''' clustered by the number of link slots in which their sign vectors
    ''' differ, using an agglomerative average linkage clustering.
    ''' (功能拓扑的 Hamming 聚类：以符号向量不同位数为距离的凝聚式平均连接聚类)
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' A topology is a length 9 sign vector with the values -1, 0 and +1, so
    ''' the Hamming distance between two topologies is the number of link slots
    ''' in which they disagree, computed by the <see cref="HammingDistance"/>
    ''' helper of the data mining library. Two topologies that differ in a
    ''' single sign are structural neighbours, and the clustering groups the
    ''' functional topologies into the families of similar adaptation networks
    ''' that the paper shows in its Figure 4C and 4D.
    ''' </para>
    ''' <para>
    ''' The average linkage update rule is the Lance Williams form
    ''' ``d(i∪j, k) = (n_i·d(i,k) + n_j·d(j,k)) / (n_i + n_j)``, and the merging
    ''' stops as soon as the closest pair of clusters is further apart than the
    ''' cutoff distance, which is the equivalent of the ``max_dist`` cutoff of
    ''' the hierarchical clustering of the paper.
    ''' </para>
    ''' </remarks>
    Public Class TopologyClustering

        ''' <summary>
        ''' The pairwise Hamming distance matrix of a topology collection.
        ''' (拓扑集合的两两 Hamming 距离矩阵)
        ''' </summary>
        ''' <param name="topologies">the topology collection</param>
        Public Shared Function DistanceMatrix(topologies As Model.NetworkTopology()) As Double(,)
            If topologies Is Nothing Then
                Throw New ArgumentNullException(NameOf(topologies))
            End If

            Dim n As Integer = topologies.Length
            Dim matrix As Double(,) = New Double(n - 1, n - 1) {}
            Dim vectors As Integer()() = New Integer(n - 1)() {}

            For i As Integer = 0 To n - 1
                vectors(i) = topologies(i).ToIntegerVector()
            Next

            For i As Integer = 0 To n - 1
                For j As Integer = i + 1 To n - 1
                    Dim d As Double = HammingDistance.Between(vectors(i), vectors(j))

                    matrix(i, j) = d
                    matrix(j, i) = d
                Next
            Next

            Return matrix
        End Function

        ''' <summary>
        ''' Cluster a topology collection with the agglomerative average linkage
        ''' method, merging clusters while their distance does not exceed the
        ''' cutoff.
        ''' (用凝聚式平均连接法聚类拓扑集合，距离不超过截断值的簇对持续合并)
        ''' </summary>
        ''' <param name="topologies">the topology collection to cluster</param>
        ''' <param name="maxDistance">the cutoff distance of the merging, 3 by the paper</param>
        ''' <returns>
        ''' the cluster label of every topology and the number of clusters; the
        ''' labels are the dense indices 0 .. clusterCount-1 of the final clusters
        ''' </returns>
        Public Shared Function Cluster(topologies As Model.NetworkTopology(), maxDistance As Double) As (labels As Integer(), clusterCount As Integer)
            If topologies Is Nothing Then
                Throw New ArgumentNullException(NameOf(topologies))
            End If

            Dim n As Integer = topologies.Length
            Dim labels As Integer() = New Integer(n - 1) {}

            If n = 0 Then
                Return (labels, 0)
            End If
            If n = 1 Then
                labels(0) = 0
                Return (labels, 1)
            End If

            Dim dist As Double(,) = DistanceMatrix(topologies)
            Dim cd As Double(,) = CType(dist.Clone(), Double(,))
            Dim active As Boolean() = New Boolean(n - 1) {}
            Dim size As Integer() = New Integer(n - 1) {}
            Dim members As List(Of Integer)() = New List(Of Integer)(n - 1) {}

            For i As Integer = 0 To n - 1
                active(i) = True
                size(i) = 1
                members(i) = New List(Of Integer)()
                members(i).Add(i)
            Next

            Do
                Dim bestI As Integer = -1
                Dim bestJ As Integer = -1
                Dim bestD As Double = maxDistance

                For i As Integer = 0 To n - 1
                    If Not active(i) Then
                        Continue For
                    End If

                    For j As Integer = i + 1 To n - 1
                        If Not active(j) Then
                            Continue For
                        End If

                        If cd(i, j) <= bestD Then
                            bestI = i
                            bestJ = j
                            bestD = cd(i, j)
                        End If
                    Next
                Next

                If bestI < 0 Then
                    Exit Do
                End If

                ' average linkage: the merged distance is the size weighted mean
                For k As Integer = 0 To n - 1
                    If Not active(k) OrElse k = bestI OrElse k = bestJ Then
                        Continue For
                    End If

                    Dim merged As Double = (size(bestI) * cd(bestI, k) + size(bestJ) * cd(bestJ, k)) / (size(bestI) + size(bestJ))

                    cd(bestI, k) = merged
                    cd(k, bestI) = merged
                Next

                size(bestI) += size(bestJ)
                members(bestI).AddRange(members(bestJ))
                active(bestJ) = False
            Loop

            Dim label As Integer = 0
            Dim clusterCount As Integer = 0

            For i As Integer = 0 To n - 1
                If Not active(i) Then
                    Continue For
                End If

                For Each m As Integer In members(i)
                    labels(m) = label
                Next

                label += 1
                clusterCount += 1
            Next

            Return (labels, clusterCount)
        End Function

        ''' <summary>
        ''' The silhouette score of a topology clustering, evaluated by the
        ''' clustering indices of the data mining library on the sign vectors.
        ''' <para>
        ''' The silhouette of the library is computed with the Euclidean metric,
        ''' which relates to the Hamming distance of the sign vectors through
        ''' the monotone map ``sqrt(2·d_Hamming)``, so the score is used as a
        ''' quality diagnostic of the clustering and not as an exact Hamming
        ''' silhouette.
        ''' </para>
        ''' (拓扑聚类的轮廓系数，由数据挖掘库在符号向量上评估，作为聚类质量诊断)
        ''' </summary>
        ''' <param name="topologies">the clustered topology collection</param>
        ''' <param name="labels">the cluster label of every topology</param>
        Public Shared Function Silhouette(topologies As Model.NetworkTopology(), labels As Integer()) As Double
            If topologies Is Nothing OrElse labels Is Nothing OrElse topologies.Length <> labels.Length Then
                Return 0
            End If
            If topologies.Length = 0 Then
                Return 0
            End If

            Dim features As Double()() = New Double(topologies.Length - 1)() {}

            For i As Integer = 0 To topologies.Length - 1
                features(i) = topologies(i).ToIntegerVector().Select(Function(s) CDbl(s)).ToArray()
            Next

            Return ClusteringIndices.Silhouette(features, labels)
        End Function

    End Class

End Namespace
