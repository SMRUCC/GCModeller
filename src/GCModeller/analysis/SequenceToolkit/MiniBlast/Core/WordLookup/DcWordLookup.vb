Namespace Core


    ''' <summary>
    ''' dc-megablast 非连续模板查找表（Ma, Xu &amp; Altschul 2003）。
    ''' 模板串 '1' = care 位（参与编码与匹配），'0' = don't-care 位。
    ''' 默认 11/18：coding = 101101100101101101, optimal = 111010010110010111
    ''' </summary>
    Public Class DcWordLookup : Implements IWordLookup

        ReadOnly _table As New Dictionary(Of Long, List(Of Integer))()
        ReadOnly _cares() As Boolean

        Public ReadOnly Property Span As Integer Implements IWordLookup.Span
        Public ReadOnly Property Weight As Integer
        Public Property WordSize As Integer Implements IWordLookup.WordSize

        Public ReadOnly Property EntryCount As Integer
            Get
                Return _table.Count
            End Get
        End Property

        Public Sub New(queryCodes As Int32(), mask() As Boolean, template As String)
            Me.Span = template.Length
            Dim w As Integer = 0
            ReDim _cares(template.Length - 1)
            For i As Integer = 0 To template.Length - 1
                _cares(i) = (template(i) = "1"c)
                If _cares(i) Then w += 1
            Next
            Me.Weight = w

            ' WordSize 取模板 weight：扫描器用它判断两-hit 的「非重叠」距离。
            ' 此前未赋值（恒为 0），会使 d >= ws 退化成恒真。
            Me.WordSize = w

            Dim n = queryCodes.Length
            If n < Span Then Return

            ' 按窗口起点逐个窗口重算 care 位打包键。
            ' （原实现的滚动更新有两处耦合错误：key 未屏蔽超出 weight 的高位，
            '   且 caresFilled 在 AddToTable 后与出窗时双重扣减，
            '   导致首窗之后所有窗口的键都错、且后续窗口不再成键。
            '   整窗重算语义直观且可验证，代价 O(n·Span)，对本规模数据可忽略。）
            For start As Integer = 0 To n - Span
                ' 窗口内任一 care 位被遮蔽或含歧义 → 该窗不作种子
                ' （don't-care 位不参与编码，其遮蔽/歧义不影响种子）
                Dim ok = True
                For k As Integer = 0 To Span - 1
                    If Not _cares(k) Then Continue For
                    Dim c = queryCodes(start + k)
                    If c > 3 OrElse (mask IsNot Nothing AndAlso mask(start + k)) Then
                        ok = False
                        Exit For
                    End If
                Next
                If Not ok Then Continue For

                AddToTable(PackWindow(queryCodes, start), start)
            Next
        End Sub

        ''' <summary>按模板 care 位顺序打包窗口 [start, start+Span) 的键</summary>
        Private Function PackWindow(codes As Int32(), start As Integer) As Long
            Dim key As Long = 0
            For k As Integer = 0 To Span - 1
                If _cares(k) Then
                    key = (key << 2) Or codes(start + k)
                End If
            Next
            Return key
        End Function

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

        ''' <summary>care 位打包；care 位含歧义返回 Long.MinValue（don't-care 位容忍歧义）</summary>
        Public Function PackAt(codes As Int32(), pos As Integer) As Long Implements IWordLookup.PackAt
            Dim key As Long = 0
            For k As Integer = 0 To Span - 1
                If _cares(k) Then
                    Dim c = codes(pos + k)
                    If c > 3 Then Return Long.MinValue
                    key = (key << 2) Or c
                End If
            Next
            Return key
        End Function
    End Class
End Namespace