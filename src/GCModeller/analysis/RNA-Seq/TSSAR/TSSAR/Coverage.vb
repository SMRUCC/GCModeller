Imports System.IO
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.SAM
Imports std = System.Math

''' <summary>
''' 双库（[+] / [-]）双链的逐位置 read 起始覆盖度。
''' </summary>
''' <remarks>
''' 四个覆盖度向量的命名与原始 TSSAR Perl/R 脚本保持一致：
''' 
''' - <see cref="PlusPlus"/>   = library_P_1（[+] 库、正链）
''' - <see cref="PlusMinus"/>  = library_P_0（[+] 库、负链）
''' - <see cref="MinusPlus"/>  = library_M_1（[-] 库、正链）
''' - <see cref="MinusMinus"/> = library_M_0（[-] 库、负链）
''' 
''' 所有数组长度为 ``genomeSize + 1``，下标 ``1..genomeSize`` 有效。
''' </remarks>
Public Class CoverageMap

    Public Property GenomeSize As Integer
    ''' <summary>所有 read 之中到达的最远位置。</summary>
    Public Property MaxPosition As Integer
    Public Property PlusPlus As Double()
    Public Property PlusMinus As Double()
    Public Property MinusPlus As Double()
    Public Property MinusMinus As Double()
    ''' <summary>[+] 库的全部 read 起始总数。</summary>
    Public Property SumPlus As Double
    ''' <summary>[-] 库的全部 read 起始总数。</summary>
    Public Property SumMinus As Double

    ''' <summary>
    ''' 把较大的 [+] 文库缩放到较小文库规模所用的归一化因子。
    ''' </summary>
    Public ReadOnly Property NormalizePlus As Double
        Get
            If SumPlus <= 0 Then Return 1
            Return If(SumPlus >= SumMinus, SumMinus / SumPlus, 1.0)
        End Get
    End Property

    ''' <summary>
    ''' 把较大的 [-] 文库缩放到较小文库规模所用的归一化因子。
    ''' </summary>
    Public ReadOnly Property NormalizeMinus As Double
        Get
            If SumMinus <= 0 Then Return 1
            Return If(SumMinus >= SumPlus, SumPlus / SumMinus, 1.0)
        End Get
    End Property
End Class

''' <summary>
''' 从 SAM 比对文件统计双库双链的逐位置 read 起始覆盖度。
''' </summary>
Public Module ReadsCoverage

    ''' <summary>
    ''' 从 [+] / [-] 两个 SAM 文件统计 read 起始覆盖度。
    ''' </summary>
    ''' <param name="plusSam">[+] 文库的 SAM 文件路径。</param>
    ''' <param name="minusSam">[-] 文库的 SAM 文件路径。</param>
    ''' <param name="genomeSize">基因组长度。</param>
    ''' <param name="prorata">是否按 SAM 的 ``NH`` 标签对多重比对 read 执行 ``1/n`` 计数。</param>
    ''' <returns>覆盖度统计结果。</returns>
    Public Function FromSam(plusSam As String,
                            minusSam As String,
                            genomeSize As Integer,
                            Optional prorata As Boolean = False) As CoverageMap

        Dim pp As Double() = New Double(genomeSize) {}
        Dim pm As Double() = New Double(genomeSize) {}
        Dim mp As Double() = New Double(genomeSize) {}
        Dim mm As Double() = New Double(genomeSize) {}
        Dim maxPos As Integer = 0

        Accumulate(plusSam, pp, pm, prorata, maxPos)
        Accumulate(minusSam, mp, mm, prorata, maxPos)

        ' 与 TSSAR.pl 保持一致：覆盖度四舍五入为整数（int(value + 0.5)）
        RoundInPlace(pp)
        RoundInPlace(pm)
        RoundInPlace(mp)
        RoundInPlace(mm)

        Return New CoverageMap With {
            .GenomeSize = genomeSize,
            .MaxPosition = maxPos,
            .PlusPlus = pp,
            .PlusMinus = pm,
            .MinusPlus = mp,
            .MinusMinus = mm,
            .SumPlus = Sum(pp) + Sum(pm),
            .SumMinus = Sum(mp) + Sum(mm)
        }
    End Function

    ''' <summary>
    ''' 解析参考基因组 fasta 文件，得到基因组总长度与第一条序列的名称。
    ''' </summary>
    ''' <param name="fasta">fasta 文件路径。</param>
    ''' <param name="chromosome">输出：第一条序列的名称。</param>
    ''' <returns>所有序列的碱基总数。</returns>
    ''' <remarks>
    ''' 与 TSSAR.pl 一致：把 fasta 中的全部序列首尾相连作为基因组长度，
    ''' 并使用第一条序列的名称作为染色体标识。
    ''' </remarks>
    Public Function GetGenomeSize(fasta As String, ByRef chromosome As String) As Integer
        Dim length As Integer = 0
        Dim name As String = ""

        For Each line As String In File.ReadLines(fasta)
            If line.StartsWith(">"c) Then
                If String.IsNullOrEmpty(name) Then
                    name = line.Substring(1).Trim().Split(" "c, ControlChars.Tab)(0)
                End If
            Else
                length += line.Trim().Length
            End If
        Next

        chromosome = name
        Return length
    End Function

    Private Sub Accumulate(samFile As String,
                           forward As Double(),
                           reversed As Double(),
                           prorata As Boolean,
                           ByRef maxPos As Integer)

        Dim sam As SAM = SAM.Load(samFile)
        Dim lastIndex As Integer = forward.Length - 1

        For Each read As AlignmentReads In sam.AlignmentsReads
            If read Is Nothing Then
                Continue For
            End If
            If read.IsUnmappedReads Then
                Continue For
            End If
            If String.IsNullOrEmpty(read.RNAME) OrElse read.RNAME = "*" Then
                Continue For
            End If

            Dim span As Integer = read.ReferenceSpan()

            If span <= 0 Then
                If read.CIGAR = "*" AndAlso
                   Not String.IsNullOrEmpty(read.SequenceData) AndAlso
                   read.SequenceData <> "*" Then
                    span = read.SequenceData.Length
                Else
                    Continue For
                End If
            End If

            Dim strand As Strands = read.Strand
            Dim left As Integer = read.POS
            Dim start As Integer
            Dim [stop] As Integer

            If strand = Strands.Forward Then
                start = left
                [stop] = left + span - 1
            ElseIf strand = Strands.Reverse Then
                start = left + span - 1
                [stop] = left
            Else
                Continue For
            End If

            If start <= 0 OrElse start > lastIndex Then
                Continue For
            End If

            Dim weight As Double = 1.0

            If prorata Then
                Dim nh As Integer = 0

                If read.OptionalTable IsNot Nothing AndAlso read.OptionalTable.ContainsKey("NH") Then
                    Integer.TryParse(read.OptionalTable("NH").Value, nh)
                End If
                If nh <= 0 Then
                    nh = 1
                End If

                weight = 1.0 / nh
            End If

            If strand = Strands.Forward Then
                forward(start) += weight
            Else
                reversed(start) += weight
            End If

            If [stop] > maxPos Then
                maxPos = [stop]
            End If
        Next
    End Sub

    Private Sub RoundInPlace(values As Double())
        For i As Integer = 1 To values.Length - 1
            values(i) = std.Floor(values(i) + 0.5)
        Next
    End Sub

    Private Function Sum(values As Double()) As Double
        Dim s As Double = 0

        For i As Integer = 1 To values.Length - 1
            s += values(i)
        Next

        Return s
    End Function
End Module
