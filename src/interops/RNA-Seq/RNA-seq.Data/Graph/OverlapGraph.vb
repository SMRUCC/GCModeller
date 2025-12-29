Imports System.Buffers
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.SequenceModel.FQ
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Graph

    Public Class OverlapGraph : Inherits Builder

        Public Sub New(reads As IEnumerable(Of FastQ))
            MyBase.New(reads)
        End Sub

        Protected Overrides Sub ProcessReads(reads As IEnumerable(Of FQ.FastQ))
            Dim edge As Edge = Nothing

            For Each read As FastQ In reads
                g.CreateNode(read.SEQ_ID).data("reads") = read.SequenceData
            Next

            For Each u As Node In TqdmWrapper.Wrap(g.vertex.ToArray)
                Dim reads_u As String = u!reads

                For Each v As Node In g.vertex
                    If u Is v Then
                        Continue For
                    End If

                    ' 返回一个对象包含: OverlapLength (长度), Identity (相似度)
                    Dim overlapInfo As OverlapResult = CalculateOverlap(reads_u, v!reads)

                Next
            Next
        End Sub

        ' ==========================================
        ' 2. 核心计算逻辑
        ' ==========================================
        Public Function CalculateOverlap(readA As String, readB As String) As OverlapResult
            ' 参数设置：
            ' MIN_OVERLAP: 允许的最小重叠长度（例如 20bp），太短没有意义且计算量大
            ' MIN_IDENTITY: 允许的最小相似度（例如 0.8 或 80%）
            Const MIN_OVERLAP As Integer = 20
            Const MIN_IDENTITY As Double = 0.8

            Dim seqA As String = readA.ToUpper()
            Dim seqB As String = readB.ToUpper()

            ' --- 尝试 1: 正向重叠 ---
            ' 逻辑：Read A 的末端 与 Read B 的前端 重叠
            Dim resultForward As OverlapResult = TryFindSuffixPrefixOverlap(seqA, seqB, MIN_OVERLAP, MIN_IDENTITY, isReverse:=False)
            If resultForward.IsValid Then Return resultForward

            ' --- 尝试 2: 反向互补重叠 ---
            ' 逻辑：Read A 的末端 与 Read B 的反向互补序列的前端 重叠 (这在双端测序或无参组装中很常见)
            Dim revSeqB As String = NucleicAcid.Complement(seqB).Reverse.CharString
            Dim resultReverse As OverlapResult = TryFindSuffixPrefixOverlap(seqA, revSeqB, MIN_OVERLAP, MIN_IDENTITY, isReverse:=True)

            Return resultReverse
        End Function

        ' ==========================================
        ' 3. 辅助算法：尝试寻找 A末端-B前端 的最佳重叠
        ' ==========================================
        Private Function TryFindSuffixPrefixOverlap(seqA As String, seqB As String, minLen As Integer, minIdent As Double, isReverse As Boolean) As OverlapResult
            Dim lenA As Integer = seqA.Length
            Dim lenB As Integer = seqB.Length

            ' 确定最大可能的重叠长度 (不能超过任一序列的长度)
            Dim maxPossibleOverlap As Integer = Math.Min(lenA, lenB)

            ' 策略：从“最长可能的重叠”开始向下搜索。
            ' 这样找到的第一个满足条件的重叠，通常就是最佳重叠。
            For overlapLen As Integer = maxPossibleOverlap To minLen Step -1

                ' 提取 A 的后缀
                Dim suffixA As String = seqA.Substring(lenA - overlapLen, overlapLen)
                ' 提取 B 的前缀
                Dim prefixB As String = seqB.Substring(0, overlapLen)

                ' 计算这两段字符串的一致性
                Dim identity As Double = CalculateIdentity(suffixA, prefixB)

                If identity >= minIdent Then
                    ' 找到了满足条件的重叠！
                    Return New OverlapResult(overlapLen, identity, isReverse)
                End If
            Next

            ' 如果循环结束还没找到，返回无效结果
            Return OverlapResult.Empty
        End Function

        ' ==========================================
        ' 4. 辅助算法：计算两个等长字符串的序列一致性
        ' ==========================================
        Private Function CalculateIdentity(s1 As String, s2 As String) As Double
            If s1.Length <> s2.Length Then Return 0.0

            Dim matches As Integer = 0
            Dim len As Integer = s1.Length

            For i As Integer = 0 To len - 1
                ' 简单的字符比对，也可以处理 'N' 等模糊碱基
                If s1(i) = s2(i) Then
                    matches += 1
                End If
            Next

            Return CDbl(matches) / CDbl(len)
        End Function
    End Class

    ' ==========================================
    ' 1. 定义重叠结果的数据结构
    ' ==========================================
    Public Class OverlapResult
        Public Property Length As Integer          ' 重叠区域的长度
        Public Property Identity As Double          ' 序列一致性 (0.0 到 1.0)
        Public Property IsReverseComplement As Boolean ' 是否为反向互补重叠
        Public Property IsValid As Boolean          ' 是否找到了满足条件的重叠
        Public Property OffsetA As Integer          ' Read A 中重叠开始的位置
        Public Property OffsetB As Integer          ' Read B 中重叠开始的位置

        Public Sub New(len As Integer, ident As Double, isRev As Boolean)
            Me.Length = len
            Me.Identity = ident
            Me.IsReverseComplement = isRev
            Me.IsValid = True
            ' 简单的偏移量计算逻辑（假设是典型的末端重叠）
            Me.OffsetA = len - len ' 末端重叠，这里仅作示意
            Me.OffsetB = 0
        End Sub

        ' 用于返回无效结果的构造函数
        Public Shared ReadOnly Property Empty As OverlapResult
            Get
                Return New OverlapResult(0, 0, False) With {.IsValid = False}
            End Get
        End Property
    End Class
End Namespace