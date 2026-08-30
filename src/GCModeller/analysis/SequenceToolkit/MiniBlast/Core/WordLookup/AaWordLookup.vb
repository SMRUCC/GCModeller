Namespace Core

    ''' <summary>
    ''' 蛋白邻域 word 查找表：dbWordKey → 查询位置列表。
    ''' [README §2.2] 对查询每个 word，枚举所有比对得分 ≥ T 的数据库 word。
    ''' </summary>
    Public Class AaWordLookup : Implements IWordLookup

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