Imports System.IO
Imports System.Runtime.CompilerServices

Namespace Clustal

    ''' <summary>
    ''' 替换打分矩阵：字符 -> 编码 + 编码对的扁平打分表(单位 bit)。
    ''' </summary>
    ''' <remarks>
    ''' 与 <c>DIAMOND.AminoAcidScorer</c> 的做法一致，把"两级字典查找"压缩成
    ''' "一次字节表 + 一次扁平数组访问"，这样 profile 发射概率的预计算可以跑满 CPU 缓存。
    ''' </remarks>
    Public Class ScoringMatrix

        ''' <summary>
        ''' 字母表
        ''' </summary>
        Public ReadOnly Property Alphabet As SequenceAlphabet
        ''' <summary>
        ''' 字母表大小
        ''' </summary>
        Public ReadOnly Property Size As Integer
        ''' <summary>
        ''' 通配编码
        ''' </summary>
        Public ReadOnly Property Wildcard As Byte
        ''' <summary>
        ''' 编码对 -> 打分(bit)，扁平的 Size x Size 数组
        ''' </summary>
        Public ReadOnly Property Scores As Double()

        Sub New(alphabet As SequenceAlphabet, scores As Double())
            If scores.Length <> alphabet.Size * alphabet.Size Then
                Throw New ArgumentException($"打分表的长度 {scores.Length} 与字母表大小 {alphabet.Size} 不匹配", NameOf(scores))
            End If

            Me.Alphabet = alphabet
            Me.Size = alphabet.Size
            Me.Wildcard = alphabet.Wildcard
            Me.Scores = scores
        End Sub

        ''' <summary>
        ''' 取两个编码之间的打分(bit)
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Score(a As Byte, b As Byte) As Double
            Return Scores(CInt(a) * Size + CInt(b))
        End Function

        ''' <summary>
        ''' 取两个残基之间的打分(bit)
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Score(a As Char, b As Char) As Double
            Return Score(Alphabet.CodeOf(a), Alphabet.CodeOf(b))
        End Function

        ''' <summary>
        ''' 构造 Gonnet 250 替换矩阵(蛋白质)
        ''' </summary>
        ''' <param name="scoreScale">额外的整体打分缩放系数，默认 1(即表值 x 1/3 bit)</param>
        Public Shared Function Gonnet(Optional scoreScale As Double = 1.0) As ScoringMatrix
            Dim alphabet As SequenceAlphabet = SequenceAlphabet.Protein
            Dim n As Integer = alphabet.Size
            Dim scores As Double() = New Double(n * n - 1) {}

            For a As Integer = 0 To n - 1
                Dim ra As Char = alphabet.Letters(a)

                For b As Integer = 0 To n - 1
                    scores(a * n + b) = Gonnet250.Score(ra, alphabet.Letters(b)) * scoreScale
                Next
            Next

            Return New ScoringMatrix(alphabet, scores)
        End Function

        ''' <summary>
        ''' 构造核酸替换矩阵
        ''' </summary>
        ''' <param name="match">匹配打分(bit)</param>
        ''' <param name="mismatch">错配打分(bit)</param>
        ''' <param name="wildcardScore">通配碱基 N 参与的打分(bit)，默认 0 表示中性</param>
        Public Shared Function DNAMatrix(Optional match As Double = 1.0,
                                         Optional mismatch As Double = -1.0,
                                         Optional wildcardScore As Double = 0.0) As ScoringMatrix

            Dim alphabet As SequenceAlphabet = SequenceAlphabet.DNA
            Dim n As Integer = alphabet.Size
            Dim wild As Integer = alphabet.Wildcard
            Dim scores As Double() = New Double(n * n - 1) {}

            For a As Integer = 0 To n - 1
                For b As Integer = 0 To n - 1
                    If a = wild OrElse b = wild Then
                        scores(a * n + b) = wildcardScore
                    ElseIf a = b Then
                        scores(a * n + b) = match
                    Else
                        scores(a * n + b) = mismatch
                    End If
                Next
            Next

            Return New ScoringMatrix(alphabet, scores)
        End Function

        ''' <summary>
        ''' 从标准的宽表矩阵文件(如 NCBI BLOSUM62、ClustalW gonnet)加载打分矩阵。
        ''' </summary>
        ''' <param name="path">矩阵文件路径</param>
        ''' <param name="alphabet">目标字母表</param>
        ''' <param name="scale">表值到 bit 的换算系数</param>
        ''' <remarks>
        ''' 支持的格式：<c>#</c> 开头的注释行、一行字母表表头、随后每行"残基 + 若干列分值"。
        ''' 文件中缺失的编码(以及未出现在表头里的编码)按通配残基处理。
        ''' </remarks>
        Public Shared Function LoadFile(path As String, Optional alphabet As SequenceAlphabet = Nothing,
                                        Optional scale As Double = 1.0) As ScoringMatrix

            If alphabet Is Nothing Then
                alphabet = SequenceAlphabet.Protein
            End If

            Dim lines As String() = File.ReadAllLines(path)
            Dim header As String() = Nothing
            Dim rows As New Dictionary(Of Char, Double())()

            For Each line As String In lines
                Dim text As String = line.Trim()

                If text.Length = 0 OrElse text.StartsWith("#") Then
                    Continue For
                End If

                Dim tokens As String() = text.Split(New Char() {" "c, vbTab}, StringSplitOptions.RemoveEmptyEntries)

                If header Is Nothing Then
                    header = tokens
                    Continue For
                End If

                Dim key As Char = tokens(0)(0)
                Dim values As Double() = New Double(header.Length - 1) {}

                For i As Integer = 1 To Math.Min(tokens.Length - 1, values.Length)
                    values(i - 1) = Val(tokens(i)) * scale
                Next

                If Not rows.ContainsKey(key) Then
                    Call rows.Add(key, values)
                End If
            Next

            If header Is Nothing Then
                Throw New InvalidDataException($"矩阵文件 '{path}' 的内容为空或者格式不正确")
            End If

            Dim n As Integer = alphabet.Size
            Dim wild As Integer = alphabet.Wildcard
            Dim scores As Double() = New Double(n * n - 1) {}
            Dim headerIndex As New Dictionary(Of Char, Integer)()

            For i As Integer = 0 To header.Length - 1
                If header(i).Length > 0 Then
                    headerIndex(Char.ToUpperInvariant(header(i)(0))) = i
                End If
            Next

            For a As Integer = 0 To n - 1
                Dim ra As Char = Char.ToUpperInvariant(alphabet.Letters(a))

                For b As Integer = 0 To n - 1
                    Dim rb As Char = Char.ToUpperInvariant(alphabet.Letters(b))

                    If a = wild OrElse b = wild OrElse
                        Not headerIndex.ContainsKey(ra) OrElse
                        Not headerIndex.ContainsKey(rb) OrElse
                        Not rows.ContainsKey(ra) Then

                        scores(a * n + b) = 0
                    Else
                        Dim ib As Integer = headerIndex(rb)
                        Dim row As Double() = rows(ra)

                        If ib < row.Length Then
                            scores(a * n + b) = row(ib)
                        Else
                            scores(a * n + b) = 0
                        End If
                    End If
                Next
            Next

            Return New ScoringMatrix(alphabet, scores)
        End Function

        Public Overrides Function ToString() As String
            Return $"ScoringMatrix({Alphabet.Kind}, A={Size})"
        End Function
    End Class
End Namespace
