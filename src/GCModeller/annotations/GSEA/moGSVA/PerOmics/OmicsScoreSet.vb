Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace PerOmics

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