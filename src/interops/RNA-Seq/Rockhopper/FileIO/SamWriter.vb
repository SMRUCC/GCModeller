' /********************************************************************************/
'
'  Rockhopper —— SAM 格式输出
'
'  原始 Rockhopper 在开启 `-SAM` 时会输出 SAM 比对文件（SamOps.java）。
'  这里将比对结果抽象为格式无关的 <see cref="SamRecord"/>，
'  由 Aligner 把命中转换为该记录，再由本模块按 SAM v1.6 规范写出。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.IO
Imports System.Text

Namespace FileIO

    ''' <summary>
    ''' 一条 SAM 比对记录（字段名遵循 SAM 规范）。
    ''' </summary>
    Public Class SamRecord

        ''' <summary>Read name（QNAME）。</summary>
        Public Property QNAME As String
        ''' <summary>Bitwise FLAG。</summary>
        Public Property FLAG As Integer
        ''' <summary>Reference sequence name（RNAME，未比对为 "*"）。</summary>
        Public Property RNAME As String = "*"
        ''' <summary>1-based leftmost mapping position（0 表示未比对）。</summary>
        Public Property POS As Integer
        ''' <summary>Mapping quality（MAPQ，255 表示不可用）。</summary>
        Public Property MAPQ As Integer = 255
        ''' <summary>CIGAR 字符串（未比对为 "*"）。</summary>
        Public Property CIGAR As String = "*"
        ''' <summary>Mate reference sequence name。</summary>
        Public Property RNEXT As String = "*"
        ''' <summary>Mate 的 1-based 位置。</summary>
        Public Property PNEXT As Integer
        ''' <summary>Template length。</summary>
        Public Property TLEN As Integer
        ''' <summary>序列（未比对为 "*"）。</summary>
        Public Property SEQ As String = "*"
        ''' <summary>质量字符串（未比对为 "*"）。</summary>
        Public Property QUAL As String = "*"

        ''' <summary>是否未比对上参考序列。</summary>
        Public ReadOnly Property IsUnmapped As Boolean
            Get
                Return (FLAG And &H4) <> 0
            End Get
        End Property

        ''' <summary>是否比对到负链。</summary>
        Public ReadOnly Property IsReverse As Boolean
            Get
                Return (FLAG And &H10) <> 0
            End Get
        End Property

        ''' <summary>
        ''' 构造一条未比对记录（FLAG=4）。
        ''' </summary>
        Public Shared Function Unmapped(qname As String, seq As String, qual As String) As SamRecord
            Return New SamRecord With {
                .QNAME = qname,
                .FLAG = 4,
                .SEQ = If(seq, "*"),
                .QUAL = If(qual, "*")
            }
        End Function

        ''' <summary>
        ''' 转为 SAM 文本行（11 个必选列）。
        ''' </summary>
        Public Overrides Function ToString() As String
            Return String.Join(vbTab,
                QNAME, FLAG, RNAME, POS, MAPQ, CIGAR, RNEXT, PNEXT, TLEN,
                If(String.IsNullOrEmpty(SEQ), "*", SEQ),
                If(String.IsNullOrEmpty(QUAL), "*", QUAL))
        End Function

    End Class

    ''' <summary>
    ''' SAM 文件写出器。
    ''' </summary>
    Public Module SamWriter

        ''' <summary>
        ''' 写出 SAM 文件（含 @HD/@SQ 头，若未提供则自动生成最小头）。
        ''' </summary>
        Public Function Write(path As String, records As IEnumerable(Of SamRecord),
                              Optional references As IEnumerable(Of (name As String, length As Integer)) = Nothing,
                              Optional encoding As Encoding = Nothing) As Boolean
            If encoding Is Nothing Then encoding = Encoding.ASCII
            Using writer As New StreamWriter(path, False, encoding)
                writer.WriteLine("@HD" & vbTab & "VN:1.6" & vbTab & "SO:unknown")
                If references IsNot Nothing Then
                    For Each reference In references
                        writer.WriteLine($"@SQ{vbTab}SN:{reference.name}{vbTab}LN:{reference.length}")
                    Next
                End If
                writer.WriteLine($"@PG{vbTab}ID:Rockhopper{vbTab}PN:Rockhopper")

                For Each record As SamRecord In records
                    writer.WriteLine(record.ToString())
                Next
            End Using
            Return True
        End Function

        ''' <summary>
        ''' 读取 SAM 文件为 <see cref="SamRecord"/> 序列。
        ''' </summary>
        Public Iterator Function Read(path As String, Optional encoding As Encoding = Nothing) As IEnumerable(Of SamRecord)
            If encoding Is Nothing Then encoding = Encoding.ASCII

            For Each line As String In File.ReadLines(path, encoding)
                If String.IsNullOrWhiteSpace(line) OrElse line.StartsWith("@") Then Continue For
                Dim t As String() = line.Split(ControlChars.Tab)
                If t.Length < 11 Then Continue For
                Yield New SamRecord With {
                    .QNAME = t(0),
                    .FLAG = CInt(Val(t(1))),
                    .RNAME = t(2),
                    .POS = CInt(Val(t(3))),
                    .MAPQ = CInt(Val(t(4))),
                    .CIGAR = t(5),
                    .RNEXT = t(6),
                    .PNEXT = CInt(Val(t(7))),
                    .TLEN = CInt(Val(t(8))),
                    .SEQ = t(9),
                    .QUAL = t(10)
                }
            Next
        End Function

    End Module

End Namespace
