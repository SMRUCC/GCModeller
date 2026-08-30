' ============================================================================
' WordLookup.vb — 查询序列 word 查找表
' ----------------------------------------------------------------------------
' [README §一.2] Word 匹配：BLASTN 精确匹配；BLASTP 邻域词（得分 ≥ T）。
'
' NtWordLookup  — 连续 word 的 base-4 打包编码（W ≤ 28，Long 键）
' DcWordLookup  — dc-megablast 非连续模板种子（Ma, Xu & Altschul 2003），
'                 11/18 模板只对 care 位打包编码，don't-care 位容忍错配
' AaWordLookup  — 蛋白邻域词：对查询每个 word 递归枚举所有得分 ≥ T 的
'                 数据库 word（24 字母空间，按列最大得分上界剪枝）
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace Core

    ''' <summary>word 查找表统一接口（扫描器按此多态调用）</summary>
    Public Interface IWordLookup

        ReadOnly Property WordSize As Integer

        ReadOnly Property Span As Integer

        ''' <summary>从 pos 开始打包 word 键；无法作种子（含歧义等）返回 Long.MinValue</summary>
        Function PackAt(codes As Int32(), pos As Integer) As Long

        Function TryGetPositions(key As Long, ByRef positions As List(Of Integer)) As Boolean

    End Interface

    ''' <summary>核酸连续 word 查找表：wordKey → 查询位置列表</summary>
    Public Class NtWordLookup
        Implements IWordLookup

        Private ReadOnly _table As New Dictionary(Of Long, List(Of Integer))()
        Public ReadOnly Property WordSize As Integer Implements IWordLookup.WordSize

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

        Public ReadOnly Property Span As Integer Implements IWordLookup.Span
            Get
                Return WordSize
            End Get
        End Property

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

        Public ReadOnly Property EntryCount As Integer
            Get
                Return _table.Count
            End Get
        End Property

    End Class

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

            ' 显式逐窗口打包（O(n·Span)，简单正确）：
            ' care 位需非歧义且未遮蔽；don't-care 位任意（容忍错配/歧义/遮蔽）
            For startPos As Integer = 0 To n - Span
                Dim key As Long = 0
                Dim valid = True
                For k As Integer = 0 To Span - 1
                    If _cares(k) Then
                        Dim c = queryCodes(startPos + k)
                        If c > 3 OrElse (mask IsNot Nothing AndAlso mask(startPos + k)) Then
                            valid = False
                            Exit For
                        End If
                        key = (key << 2) Or c
                    End If
                Next
                If valid Then AddToTable(key, startPos)
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

        Public ReadOnly Property Span As Integer Implements IWordLookup.Span
            Get
                Return WordSize
            End Get
        End Property

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

        Public ReadOnly Property EntryCount As Integer
            Get
                Return _table.Count
            End Get
        End Property

    End Class

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

    ''' <summary>
    ''' 蛋白邻域 word 查找表：dbWordKey → 查询位置列表。
    ''' [README §2.2] 对查询每个 word，枚举所有比对得分 ≥ T 的数据库 word。
    ''' </summary>
    Public Class AaWordLookup
        Implements IWordLookup

        Private ReadOnly _table As New Dictionary(Of Int32, List(Of Integer))()
        Public ReadOnly Property WordSize As Integer Implements IWordLookup.WordSize

        Private _scorer As AaScorer
        Private _maxByQ() As Double
        Private _threshold As Integer

        Public Sub New(queryCodes As Int32(), mask() As Boolean, wordSize As Integer,
                       scorer As AaScorer, threshold As Integer)
            Me.WordSize = wordSize
            _scorer = scorer
            _threshold = threshold

            ' maxByQ(q) = 查询残基 q 与任意数据库残基可得的最大得分（剪枝上界）
            Dim maxByQ(19) As Double
            For q As Integer = 0 To 19
                Dim mx = Double.NegativeInfinity
                For c As Integer = 0 To 23
                    mx = Math.Max(mx, scorer.Score(q, c))
                Next
                maxByQ(q) = mx
            Next
            _maxByQ = maxByQ

            Dim key(wordSize - 1) As Int32
            Dim n = queryCodes.Length

            For start As Integer = 0 To n - wordSize
                ' 遮蔽位置或含 B/Z/X/* 的 word 不作种子
                Dim ok = True
                For k As Integer = 0 To wordSize - 1
                    Dim c = queryCodes(start + k)
                    If c > 19 OrElse (mask IsNot Nothing AndAlso mask(start + k)) Then
                        ok = False
                        Exit For
                    End If
                    key(k) = c
                Next
                If Not ok Then Continue For

                ' slot=0 层的剩余最大得分上界：sum_{k>=1} maxByQ(key(k))
                Dim remainingMax As Double = 0
                For k As Integer = 1 To wordSize - 1
                    remainingMax += maxByQ(key(k))
                Next

                ' 递归枚举邻域（含 word 自身：自身得分 >= T 是参数有效性前提）
                ExpandNeighborhood(key, 0, 0.0, remainingMax, start)
            Next
        End Sub

        ''' <summary>
        ''' 递归枚举：word(slot) 替换为任意残基 c，累计得分 acc。
        ''' remainingMax = sum_{k>=slot+1} maxByQ(word(k))，对 k 大于 slot 的项不变，
        ''' 下一层上界 = remainingMax - maxByQ(word(slot+1))。
        ''' </summary>
        Private Sub ExpandNeighborhood(word() As Int32, slot As Integer, acc As Double,
                                       remainingMax As Double, queryPos As Integer)
            If slot = word.Length Then
                Dim packed = 0
                For k As Integer = 0 To word.Length - 1
                    packed = packed * 24 + word(k)
                Next
                AddToTable(CInt(packed), queryPos)
                Return
            End If
            Dim nextRemaining As Double = 0
            If slot + 1 < word.Length Then
                nextRemaining = remainingMax - _maxByQ(word(slot + 1))
            End If
            For c As Integer = 0 To 23
                Dim s = acc + _scorer.Score(word(slot), c)
                If s + remainingMax < _threshold Then Continue For
                Dim saved = word(slot)
                word(slot) = c
                ExpandNeighborhood(word, slot + 1, s, nextRemaining, queryPos)
                word(slot) = saved
            Next
        End Sub

        Private Sub AddToTable(key As Int32, pos As Integer)
            Dim list As List(Of Integer) = Nothing
            If Not _table.TryGetValue(key, list) Then
                list = New List(Of Integer)()
                _table(key) = list
            End If
            list.Add(pos)
        End Sub

        Public Function TryGetPositions(key As Long, ByRef positions As List(Of Integer)) As Boolean Implements IWordLookup.TryGetPositions
            Return _table.TryGetValue(CInt(key), positions)
        End Function

        Public ReadOnly Property Span As Integer Implements IWordLookup.Span
            Get
                Return WordSize
            End Get
        End Property

        Public Function PackAt(codes As Int32(), pos As Integer) As Long Implements IWordLookup.PackAt
            Dim key As Long = 0
            For k As Integer = 0 To WordSize - 1
                key = key * 24 + codes(pos + k)
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
