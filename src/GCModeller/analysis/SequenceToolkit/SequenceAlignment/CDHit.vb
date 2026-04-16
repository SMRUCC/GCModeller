#Region "Microsoft.VisualBasic::3d5eeb6d7f89ea72e7dadd845e5f10c5, analysis\SequenceToolkit\SequenceAlignment\CDHit.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 159
    '    Code Lines: 107 (67.30%)
    ' Comment Lines: 25 (15.72%)
    '    - Xml Docs: 56.00%
    ' 
    '   Blank Lines: 27 (16.98%)
    '     File Size: 5.78 KB


    ' Class CDHit
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: FindSimilar, NrSeqs, Setup, SimilarGraph
    ' 
    ' Class SimilarHit
    ' 
    '     Properties: IsUniqued, SeqID, Similar, Size
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.HashMaps.MinHash
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class CDHit

    ReadOnly k As Integer = 31

    ''' <summary>
    ''' sort by sequence length in desc order
    ''' </summary>
    Dim seqPool As FastaSeq()
    Dim minHash As SequenceItem()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="k">
    ''' protein - k=5aa
    ''' nucleotide - k=12nt
    ''' genomics - k=31nt
    ''' </param>
    Sub New(Optional k As Integer = 12)
        Me.k = k
    End Sub

    Public Function Setup(seqs As IEnumerable(Of FastaSeq)) As CDHit
        seqPool = (From seq As FastaSeq In seqs Order By seq.Length Descending).ToArray
        minHash = seqPool.SeqIterator.ToArray _
            .AsParallel _
            .Select(Function(s)
                        ' MinHash.CreateSequenceData
                        Return KSeq _
                            .KmerSpans(s.value.SequenceData, k) _
                            .CreateSequenceData(id:=s.i)
                    End Function) _
            .ToArray

        Return Me
    End Function

    Public Iterator Function SimilarGraph() As IEnumerable(Of SimilarHit)
        Dim similars As New Dictionary(Of Integer, SimilarHit)

        For Each result As SimilarityIndex In LSH.FindSimilarItems(minHash, produceUniqueHit:=True)
            If result.IsUniqueHit Then
                Yield New SimilarHit With {.SeqID = seqPool(result.U).Title}
            Else
                If Not similars.ContainsKey(result.U) Then
                    Call similars.Add(result.U, New SimilarHit With {.SeqID = seqPool(result.U).Title})
                End If

                Call similars(result.U).Similar.Add(seqPool(result.V).Title, result.Similarity)
            End If
        Next

        For Each similar As SimilarHit In similars.Values
            Yield similar
        Next
    End Function

    ''' <summary>
    ''' implements of the CD-hit liked sequence similarity clustering
    ''' </summary>
    ''' <returns></returns>
    Public Iterator Function FindSimilar(Optional threshold As Double = 0.8) As IEnumerable(Of SimilarHit)
        ' 提前计算所有相似对，构建图结构
        Dim adjList As New Dictionary(Of Integer, Dictionary(Of Integer, Double))()
        Dim jaccardTh = LSHParameterEstimator.GetThresholdFromIdentity(threshold, k)

        For Each result As SimilarityIndex In LSH.FindSimilarItems(minHash, produceUniqueHit:=False)
            If result.Similarity >= jaccardTh Then
                ' 构建邻接表：u -> v 和 v -> u
                If Not adjList.ContainsKey(result.U) Then adjList(result.U) = New Dictionary(Of Integer, Double)()
                If Not adjList.ContainsKey(result.V) Then adjList(result.V) = New Dictionary(Of Integer, Double)()

                adjList(result.U).Add(result.V, result.Similarity)
                adjList(result.V).Add(result.U, result.Similarity)
            End If
        Next

        ' 2. CD-HIT 核心：贪婪聚类
        Dim isClustered(seqPool.Length - 1) As Boolean ' 标记是否已被归入某个簇
        Dim cluster As SimilarHit

        For i As Integer = 0 To seqPool.Length - 1
            If isClustered(i) Then
                Continue For ' 如果已经被归簇，跳过
            Else
                ' i 作为代表序列
                cluster = New SimilarHit With {
                    .SeqID = seqPool(i).Title
                }
            End If

            ' 遍历所有与 i 相似的邻居
            If adjList.ContainsKey(i) Then
                For Each neighbor In adjList(i).Keys
                    If Not isClustered(neighbor) Then
                        ' 这里可以加上阈值的二次确认，虽然 LSH 已经筛选过了
                        ' CD-HIT 逻辑：将邻居标记为已归簇
                        isClustered(neighbor) = True
                        ' 记录相似度信息 (需要从之前的计算中获取，或重新计算)
                        ' 为了简化示例，这里假设记录 ID 即可
                        cluster.Similar.Add(seqPool(neighbor).Title, adjList(i)(neighbor))
                    End If
                Next
            End If

            Yield cluster
        Next
    End Function

    Public Iterator Function NrSeqs(Optional threshold As Double = 0.8) As IEnumerable(Of FastaSeq)
        Dim seqIndex = seqPool.ToDictionary(Function(s) s.Title)

        For Each cluster As SimilarHit In FindSimilar(threshold)
            Dim nr_rep = seqIndex(cluster.SeqID)

            If cluster.IsUniqued Then
                Yield nr_rep
            Else
                Yield New FastaSeq(nr_rep.SequenceData) With {
                    .Headers = {
                        cluster.SeqID,
                        $"{cluster.Size} cluster members",
                        cluster.Similar.Keys.GetJson
                    }
                }
            End If
        Next
    End Function

End Class

Public Class SimilarHit

    Public Property SeqID As String
    Public Property Similar As Dictionary(Of String, Double)

    Public ReadOnly Property IsUniqued As Boolean
        Get
            Return Similar.IsNullOrEmpty
        End Get
    End Property

    Public ReadOnly Property Size As Integer
        Get
            Return Similar.TryCount
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return SeqID
    End Function

End Class

