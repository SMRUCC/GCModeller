Namespace Summary

    ''' <summary>
    ''' 训练摘要的目录：按固定顺序排列的全部摘要行，以及主视图与文件顺序记录。
    ''' </summary>
    ''' <remarks>
    ''' 池化的累加顺序就是 <see cref="Rows"/> 的顺序（浮点加法不可交换），
    ''' 因此这里不允许在算法内部重排；需要"按视图取子集"时用 <see cref="ByView"/>，
    ''' 它保持原有相对顺序。
    ''' </remarks>
    Public Class SummaryCatalog

        ''' <summary>全部摘要行（顺序即池化顺序）。</summary>
        Public Property Rows As SummaryRow()

        ''' <summary>主视图名称（用于基因并集、相似度与控制剖面），默认 <c>view_0</c>。</summary>
        Public Property PrimaryView As String = "view_0"

        ''' <summary>
        ''' 显式记录的文件顺序（键如 <c>effect_files</c> / <c>control_files</c>），
        ''' 由上游装配时填入；为空时退化为 <see cref="Rows"/> 顺序。
        ''' </summary>
        Public Property Orders As Dictionary(Of String, String()) = New Dictionary(Of String, String())(StringComparer.Ordinal)

        ''' <summary>行数。</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return If(Rows Is Nothing, 0, Rows.Length)
            End Get
        End Property

        ''' <summary>按视图取子集（保持相对顺序）。</summary>
        Public Function ByView(view As String) As SummaryRow()
            Dim result As New List(Of SummaryRow)

            For i As Integer = 0 To Count - 1
                If String.Equals(Rows(i).View, view, StringComparison.Ordinal) Then
                    Call result.Add(Rows(i))
                End If
            Next

            Return result.ToArray
        End Function

        ''' <summary>取某细胞系在主视图上的主记录；不存在时抛异常。</summary>
        Public Function PrimaryRow(cellLine As String) As SummaryRow
            For i As Integer = 0 To Count - 1
                Dim row As SummaryRow = Rows(i)

                If String.Equals(row.CellLine, cellLine, StringComparison.Ordinal) AndAlso
                    String.Equals(row.View, PrimaryView, StringComparison.Ordinal) AndAlso
                    Not row.IsAuxiliary Then

                    Return row
                End If
            Next

            Throw New KeyNotFoundException($"no primary summary for cell line '{cellLine}'")
        End Function

        ''' <summary>全部细胞系（去重，保持出现顺序）。</summary>
        Public Function CellLines() As String()
            Dim seen As New HashSet(Of String)(StringComparer.Ordinal)
            Dim result As New List(Of String)

            For i As Integer = 0 To Count - 1
                If seen.Add(Rows(i).CellLine) Then
                    Call result.Add(Rows(i).CellLine)
                End If
            Next

            Return result.ToArray
        End Function

        ''' <summary>取显式记录的顺序；未记录时返回 <c>Nothing</c>。</summary>
        Public Function RecordedOrder(name As String) As String()
            Dim order As String() = Nothing

            If Orders IsNot Nothing AndAlso Orders.TryGetValue(name, order) Then
                Return order
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>校验所有行，并把缺失的主视图补齐为默认值。</summary>
        Public Sub Validate()
            If Rows Is Nothing OrElse Rows.Length = 0 Then
                Throw New ArgumentException("the summary catalog contains no row", NameOf(Rows))
            End If

            For i As Integer = 0 To Rows.Length - 1
                Call Rows(i).Validate()
            Next
        End Sub

        Public Overrides Function ToString() As String
            Return $"summary catalog: {Count} row(s), primary view '{PrimaryView}'"
        End Function

    End Class
End Namespace
