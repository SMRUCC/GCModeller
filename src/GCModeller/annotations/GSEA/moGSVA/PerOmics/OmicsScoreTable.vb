Imports System.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace PerOmics

    ''' <summary>
    ''' 单个组学层的「通路 × 样本」富集分数表。
    ''' </summary>
    Public Class OmicsScoreTable

        ''' <summary>所属组学层名称</summary>
        Public Property Omics As String

        ''' <summary>该层是否成功完成打分</summary>
        Public Property Available As Boolean = True

        ''' <summary>打分失败时的错误信息</summary>
        Public Property ErrorMessage As String

        ''' <summary>分数矩阵：行是通路、列是样本</summary>
        Public Property Scores As Matrix

        ''' <summary>pathway → 该通路在本层中匹配到的特征数 |S_k|</summary>
        Public Property Coverage As Dictionary(Of String, Integer) = New Dictionary(Of String, Integer)(StringComparer.Ordinal)

        ''' <summary>该层打分耗时（毫秒）</summary>
        Public Property ElapsedMilliseconds As Long

        Private m_index As Dictionary(Of String, Integer)

        ''' <summary>
        ''' 该层实际得到分数的通路列表。
        ''' </summary>
        Public ReadOnly Property Pathways As String()
            Get
                If Scores Is Nothing Then
                    Return New String() {}
                End If

                Return Scores.rownames
            End Get
        End Property

        Public Overrides Function ToString() As String
            If Available Then
                Return $"{Omics}: {Pathways.Length} pathways x {If(Scores Is Nothing, 0, Scores.sample_count)} samples ({ElapsedMilliseconds} ms)"
            Else
                Return $"{Omics}: unavailable ({ErrorMessage})"
            End If
        End Function

        ''' <summary>
        ''' 该层是否包含指定通路的分数。
        ''' </summary>
        Public Function HasPathway(pathway As String) As Boolean
            Call EnsureIndex()

            Return m_index.ContainsKey(pathway)
        End Function

        ''' <summary>
        ''' 取某条通路在各样本上的分数向量；不存在时返回 Nothing。
        ''' </summary>
        Public Function ScoreVector(pathway As String) As Double()
            Call EnsureIndex()

            Dim i As Integer

            If Not m_index.TryGetValue(pathway, i) Then
                Return Nothing
            End If

            Return Scores.expression(i).experiments
        End Function

        ''' <summary>
        ''' 取某条通路在本层的成员数，缺失时返回 0。
        ''' </summary>
        Public Function MemberCount(pathway As String) As Integer
            Dim n As Integer

            If Coverage IsNot Nothing AndAlso Coverage.TryGetValue(pathway, n) Then
                Return n
            Else
                Return 0
            End If
        End Function

        Private Sub EnsureIndex()
            If m_index IsNot Nothing Then
                Return
            End If

            m_index = New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            If Scores Is Nothing Then
                Return
            End If

            For i As Integer = 0 To Scores.size - 1
                Dim id As String = Scores.expression(i).geneID

                If Not String.IsNullOrEmpty(id) AndAlso Not m_index.ContainsKey(id) Then
                    m_index(id) = i
                End If
            Next
        End Sub

    End Class

    ''' <summary>
    ''' 全部组学层的打分结果集合。
    ''' </summary>
    Public Class OmicsScoreSet

        ''' <summary>逐层打分结果（顺序与数据集中的层顺序一致）</summary>
        Public Property Tables As OmicsScoreTable()

        ''' <summary>跨组学共享的样本轴</summary>
        Public Property SampleID As String()

        ''' <summary>所有可用层中出现过的通路（保持首次出现顺序）</summary>
        Public Property Pathways As String()

        Public ReadOnly Property OmicsNames As String()
            Get
                If Tables Is Nothing Then
                    Return New String() {}
                End If

                Return Tables.Select(Function(t) t.Omics).ToArray
            End Get
        End Property

        Public ReadOnly Property AvailableTables As OmicsScoreTable()
            Get
                If Tables Is Nothing Then
                    Return New OmicsScoreTable() {}
                End If

                Return Tables.Where(Function(t) t.Available).ToArray
            End Get
        End Property

        Public Function Table(omics As String) As OmicsScoreTable
            Return Tables.FirstOrDefault(Function(t) t.Omics.TextEquals(omics, null_equals:=False, empty_equals:=False))
        End Function

        ''' <summary>
        ''' 取某层的分数矩阵（不可用时返回 Nothing）。
        ''' </summary>
        Public Function ScoreMatrix(omics As String) As Matrix
            Dim table As OmicsScoreTable = Me.Table(omics)

            If table Is Nothing Then
                Return Nothing
            Else
                Return table.Scores
            End If
        End Function

        Public Shared Function Create(tables As OmicsScoreTable(), sampleID As String()) As OmicsScoreSet
            Dim pathwayOrder As New List(Of String)
            Dim seen As New HashSet(Of String)(StringComparer.Ordinal)

            For Each table As OmicsScoreTable In tables
                If Not table.Available Then
                    Continue For
                End If

                For Each pathway As String In table.Pathways
                    If seen.Add(pathway) Then
                        pathwayOrder.Add(pathway)
                    End If
                Next
            Next

            Return New OmicsScoreSet With {
                .Tables = tables,
                .SampleID = sampleID,
                .Pathways = pathwayOrder.ToArray
            }
        End Function

    End Class

End Namespace
