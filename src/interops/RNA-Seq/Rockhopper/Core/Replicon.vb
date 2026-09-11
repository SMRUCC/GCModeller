' /********************************************************************************/
'
'  Rockhopper —— Replicon 序列模型
'
'  原始 Rockhopper 的 Replicon 同时承担"序列容器 + BWT 索引"两个职责。
'  重写后按分层原则拆分为：
'    * Core.Replicon      —— 仅保存规范化后的序列与元数据（本文件）。
'    * Alignment.FMIndex  —— BWT/FM-index 的构建与查询（见 Alignment 模块）。
'
'  序列规范化规则复刻自原实现的 readInFastaFile / replaceAmbiguousCharacters：
'    1) 转大写；2) U → T；3) 非 A/C/G/T 字符替换为 '^'（ASCII 大于 A/C/G/T，
'       以便在 BWT 排序时排在末尾，模拟原实现使用的哨兵/歧义字符策略）。
'
' /********************************************************************************/

Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Core

    ''' <summary>
    ''' 单个复制子（染色体/质粒）的核酸序列模型。
    ''' </summary>
    Public Class Replicon

        ''' <summary>复制子名称（FASTA 头行）。</summary>
        Public ReadOnly Property Name As String

        ''' <summary>规范化后的 DNA 序列（大写、U→T、歧义字符→'^'）。</summary>
        Public ReadOnly Property SequenceData As String

        ''' <summary>序列长度。</summary>
        Public ReadOnly Property Length As Integer
            Get
                Return SequenceData.Length
            End Get
        End Property

        ''' <summary>
        ''' 使用给定名称与序列构造 Replicon（序列会被规范化）。
        ''' </summary>
        Public Sub New(name As String, sequence As String)
            Me.Name = name
            Me.SequenceData = normalize(sequence)
        End Sub

        ''' <summary>
        ''' 从 FASTA 文件读取单个复制子。
        ''' </summary>
        Public Shared Function Load(fastaFile As String) As Replicon
            Dim fasta As FastaSeq = FastaSeq.Load(fastaFile)
            Return New Replicon(fasta.Title, fasta.SequenceData)
        End Function

        ''' <summary>
        ''' 从 FASTA 对象构造。
        ''' </summary>
        Public Shared Function FromFasta(fasta As FastaSeq) As Replicon
            Return New Replicon(fasta.Title, fasta.SequenceData)
        End Function

        ''' <summary>
        ''' 序列规范化：大写化、U→T、歧义字符替换为 '^'。
        ''' </summary>
        Private Shared Function normalize(sequence As String) As String
            Dim seq As Char() = sequence.ToUpperInvariant().ToCharArray()
            For i As Integer = 0 To seq.Length - 1
                If seq(i) = "U"c Then seq(i) = "T"c
                If seq(i) <> "A"c AndAlso seq(i) <> "C"c AndAlso seq(i) <> "G"c AndAlso seq(i) <> "T"c Then
                    seq(i) = "^"c
                End If
            Next
            Return New String(seq)
        End Function

        Public Overrides Function ToString() As String
            Return Name
        End Function

    End Class

End Namespace
