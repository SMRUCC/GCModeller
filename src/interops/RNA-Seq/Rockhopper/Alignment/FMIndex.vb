' /********************************************************************************/
'
'  Rockhopper —— FM-index（BWT + C 表 + Occ 表）
'
'  严格对应论文中"Burrows-Wheeler 变换 + 全文后缀数组索引"的比对第一步：精确匹配。
'
'  实现采用**显式哨兵**（explicit sentinel）形式，这是可证明正确的标准写法：
'    1) 在参考序列 T 末尾追加唯一且字典序最小的哨兵 '$'，得到 T$（长度 n+1）；
'    2) 对 T$ 建立后缀数组 SA（长度 n+1）；
'    3) BWT[i] = T$[SA[i] - 1]（SA[i] = 0 时取哨兵 '$'）；
'    4) C[c] = T$ 中字典序严格小于 c 的字符数（因此 C['$'] = 0）；
'    5) Occ(c, i) = BWT 前 i 个字符中 c 的个数（采用检查点存储以节省内存）。
'
'  反向搜索（backward search）在行区间 [0, n+1) 上进行：
'      left'  = C[c] + Occ(c, left)
'      right' = C[c] + Occ(c, right)
'
'  注意：早期版本使用"隐式哨兵"（直接对 T 建 SA，并把 SA[i]=0 的行记为 '$'），
'  这会使 C 表与后缀数组的排序不一致（被 '$' 替换掉的字符计数少 1），
'  在部分输入上导致精确匹配失败；本实现已修正为显式哨兵。
'
' /********************************************************************************/

Imports System.Collections.Generic

Namespace Alignment

    ''' <summary>
    ''' FM-index 接口：精确匹配统计与定位，供种子-延伸使用。
    ''' </summary>
    Public Interface IFmIndex

        ''' <summary>模式串在参考序列中出现的次数。</summary>
        Function Count(pattern As String) As Integer

        ''' <summary>定位模式串（返回 1-based 起始坐标），最多返回 maxHits 个。</summary>
        Function Locate(pattern As String, Optional maxHits As Integer = 1) As Integer()

        ''' <summary>反向搜索得到的后缀数组区间 [left, right)。不存在时返回 (0, 0)。</summary>
        Function BackSearch(pattern As String) As (left As Integer, right As Integer)

    End Interface

    ''' <summary>
    ''' 基于 Burrows-Wheeler 变换的 FM-index（显式哨兵实现）。
    ''' </summary>
    Public Class FMIndex
        Implements IFmIndex

        ''' <summary>字母表（按 ASCII 升序；'$' 为哨兵，'^' 为歧义字符，均排在 A/C/G/T 之外）。</summary>
        Public Shared ReadOnly Alphabet As Char() = {"$"c, "A"c, "C"c, "G"c, "T"c, "^"c}

        Private Const OCC_INTERVAL As Integer = 64
        ''' <summary>哨兵字符。</summary>
        Public Const SENTINEL As Char = "$"c

        Private ReadOnly augmented As String
        Private ReadOnly saTable As Integer()
        Private ReadOnly bwt As String
        Private ReadOnly cTable As Integer()
        ''' <summary>检查点 Occ 表：occCheckpoint(alphaIndex)(block) 表示 BWT 前 block*INTERVAL 个字符中该字符的个数。</summary>
        Private ReadOnly occCheckpoint As Integer()()
        Private ReadOnly charIndex As Integer()

        ''' <summary>参考序列长度（不含哨兵）。</summary>
        Public ReadOnly Property Length As Integer

        ''' <summary>BWT 行数（= Length + 1）。</summary>
        Public ReadOnly Property Rows As Integer
            Get
                Return Length + 1
            End Get
        End Property

        Public Sub New(sequence As String)
            Dim text As String = If(sequence, "")
            Me.Length = text.Length
            Me.augmented = text & SENTINEL

            ' 对 T$ 建立后缀数组
            Me.saTable = SuffixArray.Build(Me.augmented)

            ' BWT
            Dim rows As Integer = Me.augmented.Length
            Dim bwtChars As Char() = New Char(rows - 1) {}
            For i As Integer = 0 To rows - 1
                Dim p As Integer = saTable(i)
                bwtChars(i) = If(p = 0, SENTINEL, Me.augmented(p - 1))
            Next
            Me.bwt = New String(bwtChars)

            ' 字符 → 字母表下标
            Me.charIndex = New Integer(127) {}
            For i As Integer = 0 To charIndex.Length - 1
                charIndex(i) = -1
            Next
            For i As Integer = 0 To Alphabet.Length - 1
                charIndex(AscW(Alphabet(i))) = i
            Next

            ' C 表：T$ 中严格小于 c 的字符总数
            Dim counts As Integer() = New Integer(Alphabet.Length - 1) {}
            For i As Integer = 0 To Me.augmented.Length - 1
                Dim a As Integer = indexOf(Me.augmented(i))
                If a >= 0 Then counts(a) += 1
            Next
            Me.cTable = New Integer(Alphabet.Length - 1) {}
            Dim running As Integer = 0
            For i As Integer = 0 To Alphabet.Length - 1
                cTable(i) = running
                running += counts(i)
            Next

            ' Occ 检查点
            Dim blocks As Integer = (rows \ OCC_INTERVAL) + 1
            Me.occCheckpoint = New Integer(Alphabet.Length - 1)() {}
            For a As Integer = 0 To Alphabet.Length - 1
                occCheckpoint(a) = New Integer(blocks) {}
            Next
            Dim cum As Integer() = New Integer(Alphabet.Length - 1) {}
            For i As Integer = 0 To rows - 1
                If i Mod OCC_INTERVAL = 0 Then
                    Dim block As Integer = i \ OCC_INTERVAL
                    For a As Integer = 0 To Alphabet.Length - 1
                        occCheckpoint(a)(block) = cum(a)
                    Next
                End If
                Dim ai As Integer = indexOf(bwt(i))
                If ai >= 0 Then cum(ai) += 1
            Next
            If rows Mod OCC_INTERVAL = 0 Then
                Dim lastBlock As Integer = rows \ OCC_INTERVAL
                For a As Integer = 0 To Alphabet.Length - 1
                    occCheckpoint(a)(lastBlock) = cum(a)
                Next
            End If
        End Sub

        Private Function indexOf(c As Char) As Integer
            Dim code As Integer = AscW(c)
            If code >= 0 AndAlso code < charIndex.Length Then Return charIndex(code)
            Return -1
        End Function

        ''' <summary>Occ(ch, i)：BWT 前 i 个字符（[0, i)）中字符 ch 的个数。</summary>
        Public Function Occ(ch As Char, i As Integer) As Integer
            If i <= 0 Then Return 0
            If i > Rows Then i = Rows

            Dim a As Integer = indexOf(ch)
            If a < 0 Then Return 0

            Dim block As Integer = i \ OCC_INTERVAL
            Dim count As Integer = occCheckpoint(a)(block)
            For j As Integer = block * OCC_INTERVAL To i - 1
                If bwt(j) = ch Then count += 1
            Next
            Return count
        End Function

        ''' <summary>GetC(ch)：字典序严格小于 ch 的字符总数。</summary>
        Public Function GetC(ch As Char) As Integer
            Dim a As Integer = indexOf(ch)
            If a < 0 Then Return 0
            Return cTable(a)
        End Function

        ''' <summary>反向搜索得到的后缀数组区间 [left, right)。</summary>
        Public Function BackSearch(pattern As String) As (left As Integer, right As Integer) Implements IFmIndex.BackSearch
            If String.IsNullOrEmpty(pattern) Then Return (0, 0)

            Dim left As Integer = 0
            Dim right As Integer = Rows
            For i As Integer = pattern.Length - 1 To 0 Step -1
                Dim c As Char = pattern(i)
                If indexOf(c) < 0 Then Return (0, 0) ' 含有字母表外字符，精确匹配失败
                left = GetC(c) + Occ(c, left)
                right = GetC(c) + Occ(c, right)
                If right <= left Then Return (0, 0)
            Next
            Return (left, right)
        End Function

        ''' <summary>模式串出现次数。</summary>
        Public Function Count(pattern As String) As Integer Implements IFmIndex.Count
            Dim range = BackSearch(pattern)
            Return range.right - range.left
        End Function

        ''' <summary>
        ''' 定位模式串（1-based 起始坐标）。
        ''' 哨兵对应的行（SA 值为 Length）会被跳过。
        ''' </summary>
        Public Function Locate(pattern As String, Optional maxHits As Integer = 1) As Integer() Implements IFmIndex.Locate
            Dim range = BackSearch(pattern)
            Dim hits As New List(Of Integer)()
            For i As Integer = range.left To range.right - 1
                Dim position As Integer = saTable(i)
                If position >= Length Then Continue For ' 哨兵后缀
                hits.Add(position + 1)
                If maxHits > 0 AndAlso hits.Count >= maxHits Then Exit For
            Next
            Return hits.ToArray
        End Function

        ''' <summary>后缀数组（只读，供候选定位使用；长度为 Length + 1）。</summary>
        Public ReadOnly Property Sa As Integer()
            Get
                Return saTable
            End Get
        End Property

    End Class

End Namespace
