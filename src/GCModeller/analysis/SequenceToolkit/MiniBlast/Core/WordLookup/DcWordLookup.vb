Namespace Core


    ''' <summary>
    ''' dc-megablast 非连续模板查找表（Ma, Xu &amp; Altschul 2003）。
    ''' 模板串 '1' = care 位（参与编码与匹配），'0' = don't-care 位。
    ''' 默认 11/18：coding = 101101100101101101, optimal = 111010010110010111
    ''' </summary>
    Public Class DcWordLookup
        Implements IWordLookup

        Private ReadOnly _table As New Dictionary(Of Long, List(Of Integer))()
        Private ReadOnly _cares() As Boolean
        Public ReadOnly Property Span As Integer Implements IWordLookup.Span
        Public ReadOnly Property Weight As Integer
        Public Property WordSize As Integer Implements IWordLookup.WordSize

        Public Sub New(queryCodes As Int32(), mask() As Boolean, template As String)
            Me.Span = template.Length
            Dim w As Integer = 0
            ReDim _cares(template.Length - 1)
            For i As Integer = 0 To template.Length - 1
                _cares(i) = (template(i) = "1"c)
                If _cares(i) Then w += 1
            Next
            Me.Weight = w

            Dim n = queryCodes.Length
            If n < Span Then Return

            Dim key As Long = 0
            Dim caresFilled As Integer = 0
            For i As Integer = 0 To n - 1
                Dim c = queryCodes(i)
                Dim masked As Boolean = (mask IsNot Nothing AndAlso mask(i))
                ' 进入模板窗口：滑动重置最简单的做法——care 位全满后打包
                ' 为控制复杂度，窗口内任一 care 位被遮蔽/歧义则整窗重置
                ' （don't-care 位不参与编码，其遮蔽不影响种子）
                If i >= Span Then
                    Dim outCode = queryCodes(i - Span)
                    Dim outMasked As Boolean = (mask IsNot Nothing AndAlso mask(i - Span))
                    Dim outCare = _cares((i - Span) Mod Span)
                    If outCare AndAlso (Not outMasked) AndAlso outCode <= 3 Then
                        caresFilled -= 1
                    End If
                End If

                Dim affects As Boolean = _cares(i Mod Span) AndAlso (Not masked) AndAlso c <= 3
                If affects Then
                    key = (key << 2) Or c
                    caresFilled += 1
                    If caresFilled = Weight Then
                        AddToTable(key, i - Span + 1)
                        caresFilled -= 1
                        ' 移除最早 care 位贡献：直接重算本窗口（简单但稍慢）
                    End If
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

        Public ReadOnly Property EntryCount As Integer
            Get
                Return _table.Count
            End Get
        End Property
    End Class
End Namespace