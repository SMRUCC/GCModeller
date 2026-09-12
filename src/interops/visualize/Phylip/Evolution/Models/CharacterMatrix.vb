Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports MSAOutput = SMRUCC.genomics.Analysis.SequenceAlignment.MSA.MSAOutput

Namespace Evolution.Models

    ''' <summary>
    ''' 已比对的多序列字符矩阵（character matrix）：所有序列等长，缺失字符以 <see cref="CharacterSet.Missing"/> 编码。
    ''' </summary>
    ''' <remarks>
    ''' 该类型是最大简约法（MP）、最大似然法（ML）、贝叶斯推断（BI）以及 Bootstrap 重采样的共同输入数据模型。
    ''' 距离法（UPGMA/NJ）则通过 <c>Evolution.Distance.SequenceDistance</c> 将其转换为 <see cref="DistanceMatrix"/>。
    ''' </remarks>
    Public Class CharacterMatrix

        ''' <summary>
        ''' 序列名称向量，与 <see cref="States"/> 的第一维一一对应。
        ''' </summary>
        Public ReadOnly Property Names As String()

        ''' <summary>
        ''' 整数状态编码矩阵：<c>States(sequence)(site)</c>，取值为 0..N-1 或 <see cref="CharacterSet.Missing"/>。
        ''' </summary>
        Public ReadOnly Property States As Integer()()

        ''' <summary>
        ''' 原始的对齐后序列（保留 gap 字符），用于结果输出与调试。
        ''' </summary>
        Public ReadOnly Property AlignedSequences As String()

        Public ReadOnly Property CharacterSet As CharacterSet

        Private Sub New(names As String(), states As Integer()(), aligned As String(), cs As CharacterSet)
            Me.Names = names
            Me.States = states
            Me.AlignedSequences = aligned
            Me.CharacterSet = cs
        End Sub

        ''' <summary>
        ''' 序列的数目（分类单元数目）
        ''' </summary>
        Public ReadOnly Property SequenceCount As Integer
            Get
                Return Names.Length
            End Get
        End Property

        ''' <summary>
        ''' 比对位点的数目（列数）
        ''' </summary>
        Public ReadOnly Property SiteCount As Integer
            Get
                Return If(States.Length = 0, 0, States(0).Length)
            End Get
        End Property

        ''' <summary>
        ''' 访问指定序列、指定位点的状态编码。
        ''' </summary>
        Default Public ReadOnly Property Residue(sequence As Integer, site As Integer) As Integer
            Get
                Return States(sequence)(site)
            End Get
        End Property

        ''' <summary>
        ''' 从等长的对齐序列构建字符矩阵。
        ''' </summary>
        Public Shared Function FromAligned(names As String(), sequences As String(), Optional cs As CharacterSet = Nothing) As CharacterMatrix
            If names Is Nothing OrElse sequences Is Nothing Then
                Throw New ArgumentNullException("names/sequences")
            End If
            If names.Length <> sequences.Length Then
                Throw New ArgumentException($"序列名称的数目({names.Length})与序列的数目({sequences.Length})不一致！")
            End If
            If sequences.Length = 0 Then
                Throw New ArgumentException("输入的多序列集合为空！")
            End If

            Dim length As Integer = sequences(0).Length

            For i As Integer = 0 To sequences.Length - 1
                If sequences(i) Is Nothing OrElse sequences(i).Length <> length Then
                    Throw New ArgumentException(
                        $"序列 '{names(i)}' 的长度({If(sequences(i) Is Nothing, 0, sequences(i).Length)})与第一条序列({length})不一致，" &
                        "请先完成多序列比对（MSA）之后再执行建树操作！")
                End If
            Next

            If cs Is Nothing Then
                cs = CharacterSet.Guess(sequences(0))
            End If

            Dim states(sequences.Length - 1)() As Integer

            For i As Integer = 0 To sequences.Length - 1
                states(i) = New Integer(length - 1) {}

                For j As Integer = 0 To length - 1
                    states(i)(j) = cs.IndexOf(sequences(i)(j))
                Next
            Next

            Return New CharacterMatrix(names, states, sequences, cs)
        End Function

        ''' <summary>
        ''' 从 FASTA 序列集合构建字符矩阵（要求序列已经比对且等长）。
        ''' </summary>
        Public Shared Function FromFasta(fa As FastaFile, Optional cs As CharacterSet = Nothing) As CharacterMatrix
            Dim names As String() = fa.Select(Function(x) x.Title).ToArray
            Dim seqs As String() = fa.Select(Function(x) x.SequenceData).ToArray

            Return FromAligned(names, seqs, cs)
        End Function

        ''' <summary>
        ''' 从多序列比对结果构建字符矩阵。
        ''' </summary>
        Public Shared Function FromMSA(msa As MSAOutput, Optional cs As CharacterSet = Nothing) As CharacterMatrix
            Return FromAligned(msa.names, msa.MSA, cs)
        End Function

        ''' <summary>
        ''' 取指定比对上位点的状态编码列。
        ''' </summary>
        Public Function GetColumn(site As Integer) As Integer()
            Dim col(SequenceCount - 1) As Integer

            For i As Integer = 0 To SequenceCount - 1
                col(i) = States(i)(site)
            Next

            Return col
        End Function

        ''' <summary>
        ''' 判断某个位点是否为最大简约法所需的信息位点（parsimony informative site）：
        ''' 至少存在两种状态，且每种出现的状态至少出现两次。
        ''' </summary>
        Public Function IsInformativeSite(site As Integer) As Boolean
            Dim counts(CharacterSet.Size - 1) As Integer

            For i As Integer = 0 To SequenceCount - 1
                Dim s As Integer = States(i)(site)

                If s >= 0 Then
                    counts(s) += 1
                End If
            Next

            Dim present = counts.Where(Function(x) x > 0).ToArray

            Return present.Length >= 2 AndAlso present.All(Function(x) x >= 2)
        End Function

        ''' <summary>
        ''' 返回所有信息位点的列索引。
        ''' </summary>
        Public Function InformativeSites() As Integer()
            Dim sites As New List(Of Integer)

            For site As Integer = 0 To SiteCount - 1
                If IsInformativeSite(site) Then
                    sites.Add(site)
                End If
            Next

            Return sites.ToArray
        End Function

        ''' <summary>
        ''' 返回全部位点的列索引（0..SiteCount-1）。
        ''' </summary>
        Public Function AllSites() As Integer()
            Return Enumerable.Range(0, SiteCount).ToArray
        End Function

        ''' <summary>
        ''' Bootstrap 重采样支持：按照给定的位点索引序列（可重复）抽取列，构建新的字符矩阵。
        ''' </summary>
        Public Function SubColumns(indices As Integer()) As CharacterMatrix
            Dim states(SequenceCount - 1)() As Integer
            Dim aligned(SequenceCount - 1) As String

            For i As Integer = 0 To SequenceCount - 1
                Dim row(indices.Length - 1) As Integer
                Dim sb As New StringBuilder(indices.Length)

                For j As Integer = 0 To indices.Length - 1
                    Dim site As Integer = indices(j)
                    row(j) = States(i)(site)
                    sb.Append(AlignedSequences(i)(site))
                Next

                states(i) = row
                aligned(i) = sb.ToString()
            Next

            Return New CharacterMatrix(Names.ToArray, states, aligned, CharacterSet)
        End Function

        Public Overrides Function ToString() As String
            Return $"{SequenceCount} sequences x {SiteCount} sites ({CharacterSet.Name})"
        End Function
    End Class

End Namespace
