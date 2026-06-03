' ============================================================================
' ProdigalUtils.vb - Prodigal VB.NET 基因预测程序 工具类
' 包含：FASTA读取、DNA序列操作、密码子表、模型序列化
' ============================================================================

Imports System.Text

''' <summary>
''' DNA序列操作工具类
''' </summary>
Public Class SequenceUtils

    ''' <summary>互补碱基映射表</summary>
    Private Shared ReadOnly ComplementMap As New Dictionary(Of Char, Char) From {
        {"A"c, "T"c}, {"T"c, "A"c}, {"G"c, "C"c}, {"C"c, "G"c},
        {"N"c, "N"c}, {"R"c, "Y"c}, {"Y"c, "R"c}, {"M"c, "K"c},
        {"K"c, "M"c}, {"S"c, "S"c}, {"W"c, "W"c}, {"H"c, "D"c},
        {"D"c, "H"c}, {"B"c, "V"c}, {"V"c, "B"c}
    }

    ''' <summary>标准遗传密码表</summary>
    Private Shared ReadOnly CodonTable As Dictionary(Of String, String) = InitCodonTable()

    ''' <summary>起始密码子集合</summary>
    Public Shared ReadOnly StartCodons As New HashSet(Of String) From {"ATG", "GTG", "TTG"}

    ''' <summary>终止密码子集合</summary>
    Public Shared ReadOnly StopCodons As New HashSet(Of String) From {"TAA", "TAG", "TGA"}

    Private Shared Function InitCodonTable() As Dictionary(Of String, String)
        Dim table As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        ' 标准遗传密码表（细菌）
        Dim entries = {
            "TTT", "F", "TTC", "F", "TTA", "L", "TTG", "L",
            "CTT", "L", "CTC", "L", "CTA", "L", "CTG", "L",
            "ATT", "I", "ATC", "I", "ATA", "I", "ATG", "M",
            "GTT", "V", "GTC", "V", "GTA", "V", "GTG", "V",
            "TCT", "S", "TCC", "S", "TCA", "S", "TCG", "S",
            "CCT", "P", "CCC", "P", "CCA", "P", "CCG", "P",
            "ACT", "T", "ACC", "T", "ACA", "T", "ACG", "T",
            "GCT", "A", "GCC", "A", "GCA", "A", "GCG", "A",
            "TAT", "Y", "TAC", "Y", "TAA", "*", "TAG", "*",
            "CAT", "H", "CAC", "H", "CAA", "Q", "CAG", "Q",
            "AAT", "N", "AAC", "N", "AAA", "K", "AAG", "K",
            "GAT", "D", "GAC", "D", "GAA", "E", "GAG", "E",
            "TGT", "C", "TGC", "C", "TGA", "*", "TGG", "W",
            "CGT", "R", "CGC", "R", "CGA", "R", "CGG", "R",
            "AGT", "S", "AGC", "S", "AGA", "R", "AGG", "R",
            "GGT", "G", "GGC", "G", "GGA", "G", "GGG", "G"
        }
        For i As Integer = 0 To entries.Length - 2 Step 2
            table(entries(i)) = entries(i + 1)
        Next
        Return table
    End Function

    ''' <summary>
    ''' 计算反向互补链
    ''' </summary>
    Public Shared Function ReverseComplement(seq As String) As String
        Dim rc As New StringBuilder(seq.Length)
        For i As Integer = seq.Length - 1 To 0 Step -1
            Dim c = seq(i)
            If ComplementMap.ContainsKey(c) Then
                rc.Append(ComplementMap(c))
            Else
                rc.Append("N"c)
            End If
        Next
        Return rc.ToString()
    End Function

    ''' <summary>
    ''' 翻译核苷酸序列为氨基酸序列
    ''' </summary>
    Public Shared Function Translate(nucSeq As String) As String
        If String.IsNullOrEmpty(nucSeq) Then Return ""
        Dim protein As New StringBuilder()
        For i As Integer = 0 To nucSeq.Length - 3 Step 3
            If i + 2 < nucSeq.Length Then
                Dim codon = nucSeq.Substring(i, 3)
                If CodonTable.ContainsKey(codon) Then
                    protein.Append(CodonTable(codon))
                Else
                    protein.Append("X"c)
                End If
            End If
        Next
        Return protein.ToString()
    End Function

    ''' <summary>
    ''' 判断是否为起始密码子
    ''' </summary>
    Public Shared Function IsStartCodon(codon As String) As Boolean
        Return StartCodons.Contains(codon.ToUpper())
    End Function

    ''' <summary>
    ''' 判断是否为终止密码子
    ''' </summary>
    Public Shared Function IsStopCodon(codon As String) As Boolean
        Return StopCodons.Contains(codon.ToUpper())
    End Function

    ''' <summary>
    ''' 计算GC含量
    ''' </summary>
    Public Shared Function ComputeGcContent(seq As String) As Double
        If String.IsNullOrEmpty(seq) Then Return 0.0
        Dim gc As Integer = 0
        For Each c In seq
            If c = "G"c OrElse c = "C"c Then gc += 1
        Next
        Return gc / CDbl(seq.Length)
    End Function

    ''' <summary>
    ''' 将六聚体转换为0-4095的索引值
    ''' A=0, C=1, G=2, T=3
    ''' </summary>
    Public Shared Function HexamerToIndex(hexamer As String) As Integer
        If hexamer Is Nothing OrElse hexamer.Length <> 6 Then Return -1
        Dim index As Integer = 0
        For i As Integer = 0 To 5
            Select Case Char.ToUpper(hexamer(i))
                Case "A"c : index = index * 4 + 0
                Case "C"c : index = index * 4 + 1
                Case "G"c : index = index * 4 + 2
                Case "T"c : index = index * 4 + 3
                Case Else : Return -1  ' 含N等非标准碱基
            End Select
        Next
        Return index
    End Function

    ''' <summary>
    ''' 将索引值转换回六聚体字符串
    ''' </summary>
    Public Shared Function IndexToHexamer(index As Integer) As String
        Dim bases = {"A"c, "C"c, "G"c, "T"c}
        Dim chars(5) As Char
        For i As Integer = 5 To 0 Step -1
            chars(i) = bases(index Mod 4)
            index \= 4
        Next
        Return New String(chars)
    End Function

    ''' <summary>
    ''' 获取序列中指定位置的密码子
    ''' </summary>
    Public Shared Function GetCodon(seq As String, position As Integer) As String
        If position + 2 < seq.Length AndAlso position >= 0 Then
            Return seq.Substring(position, 3)
        End If
        Return ""
    End Function

    ''' <summary>
    ''' 获取指定位置上游的序列（用于RBS检测）
    ''' </summary>
    Public Shared Function GetUpstreamSequence(seq As String, startPos As Integer, upstreamLen As Integer) As String
        Dim start = Math.Max(0, startPos - upstreamLen)
        Dim len = startPos - start
        If len <= 0 Then Return ""
        Return seq.Substring(start, len)
    End Function

End Class

