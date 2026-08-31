Imports System.Globalization
Imports System.IO
Imports Microsoft.VisualBasic.Language

Namespace siRNAHit

    Public Class BlastnMapTable

        Public Property qseqid As String ' 0
        Public Property sseqid As String ' 1
        Public Property sstart As Integer ' 2
        Public Property send As Integer '3
        Public Property qstart As Integer '4
        Public Property qend As Integer '5
        Public Property sstrand As String '6
        Public Property qseq As String '7
        Public Property sseq As String '8
        Public Property length As Integer '9
        Public Property evalue As Double '10
        Public Property bitscore As Double '11

        ''' <summary>
        ''' 该 HSP 是否命中在 subject 的负链上（BLASTN 的 sstrand = "minus"）。
        ''' </summary>
        ''' <remarks>
        ''' 小RNA 靶位点 = revcomp(miRNA) 出现在 mRNA 正义链上，因此只有 minus 链命中
        ''' 才是生物学上有意义的靶位点；plus 链命中表示 mRNA 含有与 miRNA 同向的序列，
        ''' 无法反向互补结合。
        ''' </remarks>
        Public ReadOnly Property IsMinus As Boolean
            Get
                Return "minus".Equals(sstrand, StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        ''' <summary>
        ''' 靶位点在 mRNA 正义链上的起始坐标（1-based，恒小于等于 <see cref="SiteEnd"/>）。
        ''' </summary>
        ''' <remarks>
        ''' minus 链命中时 BLASTN 输出 sstart &gt; send，这里统一归一化成从小到大，
        ''' 避免出现负长度。
        ''' </remarks>
        Public ReadOnly Property SiteStart As Integer
            Get
                Return Math.Min(sstart, send)
            End Get
        End Property

        ''' <summary>靶位点在 mRNA 正义链上的结束坐标（1-based，恒大于等于 <see cref="SiteStart"/>）。</summary>
        Public ReadOnly Property SiteEnd As Integer
            Get
                Return Math.Max(sstart, send)
            End Get
        End Property

        ''' <summary>靶位点在 mRNA 正义链上的跨度（nt）。</summary>
        Public ReadOnly Property SiteLength As Integer
            Get
                Return Math.Abs(send - sstart) + 1
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{qseqid} -> {sseqid} [{SiteStart},{SiteEnd}] {sstrand} len={length} evalue={evalue}"
        End Function

        ''' <summary>
        ''' 解析 NCBI blastn 的 outfmt 6 表格流。
        ''' </summary>
        ''' <param name="s">
        ''' 表格文件流。列序必须与下面命令行中的 -outfmt 完全一致：
        ''' <code>
        ''' -outfmt "6 qseqid sseqid sstart send qstart qend sstrand qseq sseq length evalue bitscore"
        ''' </code>
        ''' </param>
        ''' <returns>逐行流式产出，列数不足 12 的行被跳过。</returns>
        Public Shared Iterator Function Parse(s As Stream) As IEnumerable(Of BlastnMapTable)
            Using reader As New StreamReader(s)
                Dim line As String = reader.ReadLine()

                Do While line IsNot Nothing
                    line = line.Trim()

                    If line.Length > 0 Then
                        Dim map As BlastnMapTable = TryParseRow(line.Split(vbTab))

                        If map IsNot Nothing Then
                            Yield map
                        End If
                    End If

                    line = reader.ReadLine()
                Loop
            End Using
        End Function

        ''' <summary>
        ''' 解析单行 HSP 记录；列数不足 12 时返回 Nothing。
        ''' </summary>
        Private Shared Function TryParseRow(cols As String()) As BlastnMapTable
            If cols.Length < 12 Then
                Return Nothing
            End If

            ' E-value 解析：BLASTN 常输出科学计数法（如 2e-07），
            ' 用 InvariantCulture 避免系统区域设置（如德语逗号小数点）干扰。
            ' 解析失败不丢弃整行，退化为 +∞ 交由 e-value 阈值自然淘汰，
            ' 避免因为单列格式问题静默丢掉真实的比对结果。
            Dim evalue As Double

            If Not Double.TryParse(cols(10).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, evalue) Then
                evalue = Double.MaxValue
            End If

            Return New BlastnMapTable With {
                .evalue = evalue,
                .length = cols(9).ParseInteger,
                .bitscore = cols(11).Trim().ParseDouble,
                .qend = cols(5).ParseInteger,
                .sstrand = cols(6),
                .qseq = cols(7),
                .qseqid = cols(0),
                .qstart = cols(4).ParseInteger,
                .send = cols(3).ParseInteger,
                .sseq = cols(8),
                .sseqid = cols(1),
                .sstart = cols(2).ParseInteger
            }
        End Function

    End Class
End Namespace