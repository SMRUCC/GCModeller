' /********************************************************************************/
'
'  Rockhopper —— 测序读段文件读取
'
'  原始 Rockhopper 支持 FASTQ / QSEQ / FASTA / SAM / BAM 五种输入格式（FileOps.java）。
'  这里迁移到 GCModeller 现代序列框架：
'    * FASTQ  → SMRUCC.genomics.SequenceModel.FQ.FastQFile / Stream
'    * FASTA  → SMRUCC.genomics.SequenceModel.FASTA.FastaFile
'    * SAM/BAM→ 自行解析（框架内的 SAM 模型缺少 SEQ 字段访问，BamReader 为演示代码
'               含 Console.ReadKey() 阻塞且只读取第一条记录，均不适合作为库被调用）
'    * QSEQ   → 自行解析
'
'  读段以流式（Iterator）方式产生，避免全量读段驻留内存。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.FQ

Namespace FileIO

    ''' <summary>
    ''' 一条用于比对的测序读段（统一后的轻量视图，屏蔽输入格式差异）。
    ''' </summary>
    Public Class Read

        ''' <summary>读段标识（SAM/BAM 中为 QNAME）。</summary>
        Public Property ID As String
        ''' <summary>碱基序列（已转大写）。</summary>
        Public Property Sequence As String
        ''' <summary>测序质量字符串（FASTQ/QSEQ/SAM 才有，否则为 Nothing）。</summary>
        Public Property Quality As String
        ''' <summary>质量分值（Phred+33），与 <see cref="Sequence"/> 等长；缺失时为 Nothing。</summary>
        Public Property Scores As Integer()

        Public ReadOnly Property Length As Integer
            Get
                Return If(Sequence Is Nothing, 0, Sequence.Length)
            End Get
        End Property

        Public Sub New(id As String, sequence As String, Optional quality As String = Nothing)
            Me.ID = id
            Me.Sequence = If(sequence, "").ToUpperInvariant()
            Me.Quality = quality
            Me.Scores = If(quality Is Nothing OrElse quality.Length = 0, Nothing, FastQ.GetQualityOrder(quality).ToArray())
        End Sub

    End Class

    ''' <summary>
    ''' 支持的读段文件格式。
    ''' </summary>
    Public Enum ReadFileFormat
        Auto
        FastQ
        QSeq
        Fasta
        Sam
        Bam
    End Enum

    ''' <summary>
    ''' 读段文件读取器。
    ''' </summary>
    Public Module ReadsReader

        ''' <summary>
        ''' 根据扩展名推断读段文件格式。
        ''' </summary>
        Public Function DetectFormat(path As String) As ReadFileFormat
            Select Case System.IO.Path.GetExtension(path).ToLowerInvariant
                Case ".fastq", ".fq" : Return ReadFileFormat.FastQ
                Case ".qseq" : Return ReadFileFormat.QSeq
                Case ".fasta", ".fa", ".fna", ".fas" : Return ReadFileFormat.Fasta
                Case ".sam" : Return ReadFileFormat.Sam
                Case ".bam" : Return ReadFileFormat.Bam
                Case Else : Return ReadFileFormat.Auto
            End Select
        End Function

        ''' <summary>
        ''' 读取一个测序文件中的全部读段。
        ''' </summary>
        Public Iterator Function ReadAll(path As String, Optional format As ReadFileFormat = ReadFileFormat.Auto) As IEnumerable(Of Read)
            If format = ReadFileFormat.Auto Then format = DetectFormat(path)

            Select Case format
                Case ReadFileFormat.FastQ
                    For Each read As FastQ In SMRUCC.genomics.SequenceModel.FQ.Stream.ReadAllLines(path)
                        Yield New Read(read.SEQ_ID, read.SequenceData, read.Quality)
                    Next
                Case ReadFileFormat.QSeq
                    For Each line As String In File.ReadLines(path)
                        Dim read As Read = parseQSeqLine(line)
                        If read IsNot Nothing Then Yield read
                    Next
                Case ReadFileFormat.Fasta
                    For Each fa As FastaSeq In FastaFile.Read(path)
                        Yield New Read(fa.Title, fa.SequenceData)
                    Next
                Case ReadFileFormat.Sam
                    For Each read As Read In readSam(path)
                        Yield read
                    Next
                Case ReadFileFormat.Bam
                    For Each read As Read In readBam(path)
                        Yield read
                    Next
                Case Else
                    Throw New NotSupportedException($"无法识别读段文件格式：{path}")
            End Select
        End Function

        ''' <summary>
        ''' 统计读段文件中的读段数。
        ''' </summary>
        Public Function CountReads(path As String, Optional format As ReadFileFormat = ReadFileFormat.Auto) As Long
            Dim n As Long = 0
            For Each read As Read In ReadAll(path, format)
                n += 1
            Next
            Return n
        End Function

        ''' <summary>
        ''' 解析 QSEQ 行（11 列 Tab 分隔，序号 8 为序列，10 为质量）。
        ''' </summary>
        Private Function parseQSeqLine(line As String) As Read
            If String.IsNullOrWhiteSpace(line) Then Return Nothing
            Dim tokens As String() = line.Split(ControlChars.Tab)
            If tokens.Length < 11 Then Return Nothing
            Return New Read(tokens(0), tokens(8), tokens(10))
        End Function

        ''' <summary>
        ''' 解析 SAM 文本（跳过 @ 头行；QNAME=0, SEQ=9, QUAL=10）。
        ''' </summary>
        Private Iterator Function readSam(path As String) As IEnumerable(Of Read)
            For Each line As String In File.ReadLines(path)
                If String.IsNullOrWhiteSpace(line) OrElse line.StartsWith("@") Then Continue For
                Dim tokens As String() = line.Split(ControlChars.Tab)
                If tokens.Length < 11 Then Continue For
                Yield New Read(tokens(0), tokens(9), If(tokens(10) = "*", Nothing, tokens(10)))
            Next
        End Function

        ''' <summary>
        ''' 解析未压缩 BAM（BAM\1，非 BGZF 压缩）。
        ''' 与框架 BamReader 的假设一致：可用 `samtools view -u -h in.sam > uncompressed.bam` 生成。
        ''' </summary>
        Private Iterator Function readBam(path As String) As IEnumerable(Of Read)
            Using fs As New FileStream(path, FileMode.Open, FileAccess.Read)
                Using br As New BinaryReader(fs)
                    Dim magic As String = Encoding.ASCII.GetString(br.ReadBytes(4))
                    If magic <> "BAM" & ChrW(1) Then
                        Throw New InvalidDataException($"不是有效的（未压缩）BAM 文件：{path}")
                    End If

                    ' 头部文本（以 \0 结尾）
                    readCString(br)
                    ' 参考序列字典
                    Dim nRef As Integer = br.ReadInt32()
                    For i As Integer = 0 To nRef - 1
                        Dim lName As Integer = br.ReadInt32()
                        Call br.ReadBytes(lName)
                        Call br.ReadInt32()
                    Next

                    While br.BaseStream.Position < br.BaseStream.Length
                        Dim alignment As Byte() = readBamRecord(br)
                        If alignment Is Nothing Then Exit While
                        Dim read As Read = parseBamRecord(alignment)
                        If read IsNot Nothing Then Yield read
                    End While
                End Using
            End Using
        End Function

        ''' <summary>
        ''' 读取一条 BAM 记录（含 block_size 之后的全部字节）。
        ''' </summary>
        Private Function readBamRecord(br As BinaryReader) As Byte()
            If br.BaseStream.Position + 4 > br.BaseStream.Length Then Return Nothing
            Dim blockSize As Integer = br.ReadInt32()
            If blockSize <= 0 Then Return Nothing
            Return br.ReadBytes(blockSize)
        End Function

        ''' <summary>
        ''' 解析 BAM 记录体：提取 read name、序列与质量。
        ''' 记录体布局：refID(4) pos(4) bin_mq_nl(4) flag_nc(4) l_seq(4) next_refID(4)
        '''             next_pos(4) tlen(4) read_name(l_read_name) cigar(4*n) seq(ceil(l_seq/2)) qual(l_seq)
        ''' </summary>
        Private Function parseBamRecord(buf As Byte()) As Read
            If buf.Length < 32 Then Return Nothing

            Dim l_read_name As Integer = buf(8)
            Dim bin_mq_nl As UInteger = BitConverter.ToUInt32(buf, 8)
            Dim n_cigar_op As Integer = CInt(BitConverter.ToUInt32(buf, 12) And &HFFFFUI)
            Dim l_seq As Integer = BitConverter.ToInt32(buf, 16)
            l_read_name = CInt(bin_mq_nl And &HFFUI)

            Dim offset As Integer = 32
            If offset + l_read_name > buf.Length Then Return Nothing
            Dim readName As String = Encoding.ASCII.GetString(buf, offset, l_read_name).TrimEnd(ChrW(0))
            offset += l_read_name
            offset += n_cigar_op * 4

            Dim seqByteCount As Integer = (l_seq + 1) \ 2
            If offset + seqByteCount > buf.Length Then Return Nothing
            Dim sb As New StringBuilder(l_seq)
            For i As Integer = 0 To seqByteCount - 1
                Dim b As Byte = buf(offset + i)
                sb.Append(bamBaseChar(CInt((b >> 4) And &HF)))
                If sb.Length < l_seq Then sb.Append(bamBaseChar(CInt(b And &HF)))
            Next
            offset += seqByteCount

            Dim quality As String = Nothing
            If offset + l_seq <= buf.Length AndAlso l_seq > 0 Then
                Dim qb As New StringBuilder(l_seq)
                For i As Integer = 0 To l_seq - 1
                    qb.Append(ChrW(CInt(buf(offset + i)) + 33))
                Next
                quality = qb.ToString()
            End If

            Return New Read(readName, sb.ToString(), quality)
        End Function

        Private Function bamBaseChar(val As Integer) As Char
            Select Case val
                Case 1 : Return "A"c
                Case 2 : Return "C"c
                Case 4 : Return "G"c
                Case 8 : Return "T"c
                Case 15 : Return "N"c
                Case Else : Return "."c
            End Select
        End Function

        Private Function readCString(br As BinaryReader) As String
            Dim bytes As New List(Of Byte)()
            While br.BaseStream.Position < br.BaseStream.Length
                Dim b As Byte = br.ReadByte()
                If b = 0 Then Exit While
                bytes.Add(b)
            End While
            Return Encoding.ASCII.GetString(bytes.ToArray())
        End Function

    End Module

End Namespace
