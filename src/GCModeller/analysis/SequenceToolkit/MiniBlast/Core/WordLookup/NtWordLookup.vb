Namespace Core


    ''' <summary>核酸连续 word 查找表：wordKey → 查询位置列表</summary>
    Public Class NtWordLookup : Implements IWordLookup

        ReadOnly _table As New Dictionary(Of Long, List(Of Integer))()

        Public ReadOnly Property WordSize As Integer Implements IWordLookup.WordSize

        Public ReadOnly Property EntryCount As Integer
            Get
                Return _table.Count
            End Get
        End Property

        Public ReadOnly Property Span As Integer Implements IWordLookup.Span
            Get
                Return WordSize
            End Get
        End Property

        Public Sub New(queryCodes As Int32(), mask() As Boolean, wordSize As Integer)
            Me.WordSize = wordSize
            Dim n = queryCodes.Length
            If n < wordSize Then Return

            Dim key As Long = 0
            Dim filled As Integer = 0
            For i As Integer = 0 To n - 1
                Dim c = queryCodes(i)
                Dim masked As Boolean = (mask IsNot Nothing AndAlso mask(i))

                If masked OrElse c > 3 Then
                    ' 歧义/遮蔽：word 窗口重置
                    key = 0
                    filled = 0
                    Continue For
                End If

                key = (key << 2) Or c
                filled += 1
                If filled > wordSize Then
                    key = key And ((1L << (2 * wordSize)) - 1L)
                    filled = wordSize
                End If
                If filled = wordSize Then
                    AddToTable(key, i - wordSize + 1)
                End If
            Next
        End Sub

        Private Sub AddToTable(key As Long, pos As Integer)
            Dim list As List(Of Integer) = Nothing
            If Not _table.TryGetValue(key, list) Then
                list = New List(Of Integer)()
                _table(key) = list
            End If
            list.Add(pos)
        End Sub

        Public Function TryGetPositions(key As Long, ByRef positions As List(Of Integer)) As Boolean Implements IWordLookup.TryGetPositions
            Return _table.TryGetValue(key, positions)
        End Function

        ''' <summary>数据库侧打包 word 键；含歧义字符返回 Long.MinValue</summary>
        Public Function PackAt(codes As Int32(), pos As Integer) As Long Implements IWordLookup.PackAt
            Dim key As Long = 0
            For k As Integer = 0 To WordSize - 1
                Dim c = codes(pos + k)
                If c > 3 Then Return Long.MinValue
                key = (key << 2) Or c
            Next
            Return key
        End Function
    End Class


End Namespace