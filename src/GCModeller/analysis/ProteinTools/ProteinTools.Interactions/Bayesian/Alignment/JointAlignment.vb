Imports System.Text
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Bayesian.Alignment

    ''' <summary>
    ''' 「配对联合比对」(paired joint multiple alignment)。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这是 Burger &amp; van Nimwegen (2008) 方法的数据入口。联合比对把两个蛋白家族
    ''' 的多重比对在<b>行</b>方向拼接起来：每一行对应一个基因组(物种)，
    ''' 该行的前 <see cref="LengthA"/> 列是家族 A 的比对列，后 <see cref="LengthB"/> 列是家族 B 的比对列。
    ''' 每一行代表一个「假设存在于该基因组之中的互作对」。
    ''' </para>
    ''' <para>
    ''' 论文中的「分配(assignment) a」就是对行的重新配对：在同一基因组之内交换两个成员的
    ''' B 段(或者 A 段)，从而得到一个新的联合比对。因此本类提供了
    ''' <see cref="SwapPartners"/> 与 <see cref="WithPartnerOrder"/> 两个操作来支持 Gibbs 采样。
    ''' </para>
    ''' <para>
    ''' 数据结构上使用一个 <c>Integer(,)</c> 存放编码之后的残基，行优先。相对于字符矩阵，
    ''' 整数编码可以在计数阶段省掉一次查表，并且让计数器的索引计算变成纯整数运算。
    ''' </para>
    ''' </remarks>
    Public Class JointAlignment

        Private ReadOnly _data As Integer(,)
        Private ReadOnly _names As String()
        Private ReadOnly _alphabet As ResidueAlphabet
        Private ReadOnly _lengthA As Integer
        Private ReadOnly _lengthB As Integer

        ''' <summary>
        ''' 由编码之后的残基矩阵构造联合比对
        ''' </summary>
        ''' <param name="data">行优先的残基编码矩阵，尺寸为 (行数, 列数)</param>
        ''' <param name="names">每一行的标签，通常是基因组或者物种的编号</param>
        ''' <param name="lengthA">家族 A 的比对列数</param>
        ''' <param name="lengthB">家族 B 的比对列数</param>
        ''' <param name="alphabet">残基字母表</param>
        Public Sub New(data As Integer(,), names As String(), lengthA As Integer, lengthB As Integer, Optional alphabet As ResidueAlphabet = Nothing)
            If data Is Nothing Then
                Throw New ArgumentNullException(NameOf(data))
            End If
            If lengthA < 0 OrElse lengthB < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(lengthA), "the block length can not be negative")
            End If
            If data.GetLength(1) <> lengthA + lengthB Then
                Throw New ArgumentException(
                    $"the alignment has {data.GetLength(1)} columns but {lengthA + lengthB} were expected (A={lengthA} + B={lengthB})",
                    NameOf(data))
            End If
            If names IsNot Nothing AndAlso names.Length <> data.GetLength(0) Then
                Throw New ArgumentException("the number of row labels must equal the number of alignment rows", NameOf(names))
            End If

            _data = data
            _alphabet = If(alphabet, ResidueAlphabet.Standard21)
            _lengthA = lengthA
            _lengthB = lengthB
            _names = If(names, Enumerable.Range(0, data.GetLength(0)).Select(Function(i) "seq_" & i).ToArray())
        End Sub

        ''' <summary>
        ''' 每一行的标签
        ''' </summary>
        Public ReadOnly Property Names As String()
            Get
                Return _names
            End Get
        End Property

        ''' <summary>
        ''' 每一行家族 A 一侧的成员编号；为 <c>Nothing</c> 的时候等同于 <see cref="Names"/>
        ''' </summary>
        Public Property MemberA As String() = Nothing

        ''' <summary>
        ''' 每一行家族 B 一侧的成员编号；为 <c>Nothing</c> 的时候等同于 <see cref="Names"/>
        ''' </summary>
        Public Property MemberB As String() = Nothing

        ''' <summary>
        ''' 读取家族 A 一侧的成员编号
        ''' </summary>
        Public Function NameOfA(row As Integer) As String
            If MemberA IsNot Nothing AndAlso row < MemberA.Length Then
                Return MemberA(row)
            Else
                Return _names(row)
            End If
        End Function

        ''' <summary>
        ''' 读取家族 B 一侧的成员编号
        ''' </summary>
        Public Function NameOfB(row As Integer) As String
            If MemberB IsNot Nothing AndAlso row < MemberB.Length Then
                Return MemberB(row)
            Else
                Return _names(row)
            End If
        End Function

        ''' <summary>
        ''' 判断两行的家族 A 段是否完全相同(即它们其实是同一个 A 成员)
        ''' </summary>
        Public Function SameBlockA(row1 As Integer, row2 As Integer) As Boolean
            For c As Integer = 0 To _lengthA - 1
                If _data(row1, c) <> _data(row2, c) Then
                    Return False
                End If
            Next

            Return True
        End Function

        ''' <summary>
        ''' 参与比对的序列(基因组)条数，也就是论文之中的样本数
        ''' </summary>
        Public ReadOnly Property Rows As Integer
            Get
                Return _data.GetLength(0)
            End Get
        End Property

        ''' <summary>
        ''' 家族 A 的比对列数
        ''' </summary>
        Public ReadOnly Property LengthA As Integer
            Get
                Return _lengthA
            End Get
        End Property

        ''' <summary>
        ''' 家族 B 的比对列数
        ''' </summary>
        Public ReadOnly Property LengthB As Integer
            Get
                Return _lengthB
            End Get
        End Property

        ''' <summary>
        ''' 联合比对的总列数 <c>L = L_A + L_B</c>，也就是贝叶斯网络之中节点的个数
        ''' </summary>
        Public ReadOnly Property Length As Integer
            Get
                Return _lengthA + _lengthB
            End Get
        End Property

        ''' <summary>
        ''' 残基字母表
        ''' </summary>
        Public ReadOnly Property Alphabet As ResidueAlphabet
            Get
                Return _alphabet
            End Get
        End Property

        ''' <summary>
        ''' 读取某一列的全部编码
        ''' </summary>
        Public Function Column(index As Integer) As Integer()
            Dim values As Integer() = New Integer(Rows - 1) {}

            For r As Integer = 0 To Rows - 1
                values(r) = _data(r, index)
            Next

            Return values
        End Function

        ''' <summary>
        ''' 读取某一行的全部编码
        ''' </summary>
        Public Function Row(index As Integer) As Integer()
            Dim values As Integer() = New Integer(Length - 1) {}

            For c As Integer = 0 To Length - 1
                values(c) = _data(index, c)
            Next

            Return values
        End Function

        ''' <summary>
        ''' 读取某一列某一行上的编码
        ''' </summary>
        Default Public ReadOnly Property Item(row As Integer, column As Integer) As Integer
            Get
                Return _data(row, column)
            End Get
        End Property

        ''' <summary>
        ''' 判断某一列是否属于家族 A
        ''' </summary>
        Public Function IsBlockA(column As Integer) As Boolean
            Return column < _lengthA
        End Function

        ''' <summary>
        ''' 枚举所有「跨家族」的列对 <c>(i, j)</c>，其中 <c>i</c> 属于家族 A、<c>j</c> 属于家族 B。
        ''' </summary>
        ''' <remarks>
        ''' 跨家族的列对是唯一会随着「分配」变化而改变统计量的列对，因此也是 Gibbs 采样器
        ''' 唯一需要重新计算的部分。
        ''' </remarks>
        Public Iterator Function CrossBlockPairs() As IEnumerable(Of (i As Integer, j As Integer))
            For i As Integer = 0 To _lengthA - 1
                For j As Integer = _lengthA To Length - 1
                    Yield (i, j)
                Next
            Next
        End Function

        ''' <summary>
        ''' 枚举全部列对 <c>i &lt; j</c>
        ''' </summary>
        Public Iterator Function AllPairs() As IEnumerable(Of (i As Integer, j As Integer))
            For i As Integer = 0 To Length - 2
                For j As Integer = i + 1 To Length - 1
                    Yield (i, j)
                Next
            Next
        End Function

        ''' <summary>
        ''' 交换两个成员在整个联合比对之中的 B 段，得到一个新的联合比对。
        ''' </summary>
        ''' <param name="row1">第一个成员所在的行</param>
        ''' <param name="row2">第二个成员所在的行</param>
        ''' <remarks>
        ''' 这就是论文 Gibbs 采样器的基本移动：在同一个基因组之内随机取两个蛋白，
        ''' 在「当前分配」与「互换伙伴之后的分配」之间采样。
        ''' </remarks>
        Public Function SwapPartners(row1 As Integer, row2 As Integer) As JointAlignment
            If row1 = row2 Then
                Return Me
            End If

            Dim clone As Integer(,) = DirectCast(_data.Clone(), Integer(,))

            For c As Integer = _lengthA To Length - 1
                Dim tmp As Integer = clone(row1, c)
                clone(row1, c) = clone(row2, c)
                clone(row2, c) = tmp
            Next

            Return New JointAlignment(clone, _names, _lengthA, _lengthB, _alphabet) With {
                .MemberA = MemberA,
                .MemberB = MemberB
            }
        End Function

        ''' <summary>
        ''' 按照给定的 B 段顺序重排联合比对：行 <c>i</c> 的 B 段取自原来的行 <paramref name="sourceRow"/>。
        ''' </summary>
        ''' <param name="sourceRow">新的行到下标的映射，长度必须等于行数</param>
        Public Function WithPartnerOrder(sourceRow As Integer()) As JointAlignment
            If sourceRow Is Nothing OrElse sourceRow.Length <> Rows Then
                Throw New ArgumentException("the mapping must be aligned with the alignment rows", NameOf(sourceRow))
            End If

            Dim clone As Integer(,) = New Integer(Rows - 1, Length - 1) {}

            For r As Integer = 0 To Rows - 1
                For c As Integer = 0 To _lengthA - 1
                    clone(r, c) = _data(r, c)
                Next

                Dim source As Integer = sourceRow(r)

                For c As Integer = _lengthA To Length - 1
                    clone(r, c) = _data(source, c)
                Next
            Next

            Return New JointAlignment(clone, _names, _lengthA, _lengthB, _alphabet) With {
                .MemberA = MemberA,
                .MemberB = MemberB
            }
        End Function

        ''' <summary>
        ''' 复制一份联合比对
        ''' </summary>
        Public Function Clone() As JointAlignment
            Return New JointAlignment(DirectCast(_data.Clone(), Integer(,)), _names.ToArray(), _lengthA, _lengthB, _alphabet) With {
                .MemberA = If(MemberA Is Nothing, Nothing, MemberA.ToArray()),
                .MemberB = If(MemberB Is Nothing, Nothing, MemberB.ToArray())
            }
        End Function

        ''' <summary>
        ''' 只保留指定的行
        ''' </summary>
        Public Function Subset(rows As Integer()) As JointAlignment
            Dim clone As Integer(,) = New Integer(rows.Length - 1, Length - 1) {}
            Dim names As String() = New String(rows.Length - 1) {}
            Dim left As String() = If(MemberA Is Nothing, Nothing, New String(rows.Length - 1) {})
            Dim right As String() = If(MemberB Is Nothing, Nothing, New String(rows.Length - 1) {})

            For i As Integer = 0 To rows.Length - 1
                names(i) = _names(rows(i))

                If left IsNot Nothing Then
                    left(i) = MemberA(rows(i))
                End If
                If right IsNot Nothing Then
                    right(i) = MemberB(rows(i))
                End If

                For c As Integer = 0 To Length - 1
                    clone(i, c) = _data(rows(i), c)
                Next
            Next

            Return New JointAlignment(clone, names, _lengthA, _lengthB, _alphabet) With {
                .MemberA = left,
                .MemberB = right
            }
        End Function

        ''' <summary>
        ''' 删除空位比例过高(或者过低)的列。这是论文与旧实现(<c>DataPreparations.Trim</c>)共有的预处理步骤。
        ''' </summary>
        ''' <param name="maxGapFraction">允许的最大空位比例，超过该比例的列会被删除</param>
        ''' <param name="minOccupancy">允许的最小非空位比例，为 0 的时候不启用该过滤</param>
        Public Function FilterColumns(Optional maxGapFraction As Double = 0.75, Optional minOccupancy As Double = 0.0) As JointAlignment
            Dim keepA As New List(Of Integer)
            Dim keepB As New List(Of Integer)
            Dim gap As Integer = _alphabet.GapIndex

            For c As Integer = 0 To Length - 1
                Dim gaps As Integer = 0

                For r As Integer = 0 To Rows - 1
                    If _data(r, c) = gap Then
                        gaps += 1
                    End If
                Next

                Dim gapFraction As Double = gaps / CDbl(Rows)

                If gapFraction > maxGapFraction Then
                    Continue For
                End If
                If minOccupancy > 0 AndAlso 1.0 - gapFraction < minOccupancy Then
                    Continue For
                End If

                If c < _lengthA Then
                    Call keepA.Add(c)
                Else
                    Call keepB.Add(c)
                End If
            Next

            If keepA.Count = _lengthA AndAlso keepB.Count = _lengthB Then
                Return Me
            End If

            Dim newLengthA As Integer = keepA.Count
            Dim newLengthB As Integer = keepB.Count
            Dim clone As Integer(,) = New Integer(Rows - 1, newLengthA + newLengthB - 1) {}

            For r As Integer = 0 To Rows - 1
                For k As Integer = 0 To keepA.Count - 1
                    clone(r, k) = _data(r, keepA(k))
                Next

                For k As Integer = 0 To keepB.Count - 1
                    clone(r, newLengthA + k) = _data(r, keepB(k))
                Next
            Next

            Return New JointAlignment(clone, _names, newLengthA, newLengthB, _alphabet) With {
                .MemberA = MemberA,
                .MemberB = MemberB
            }
        End Function

#Region "FASTA input/output"

        ''' <summary>
        ''' 从 FASTA 文件读取一个已经配对好的联合比对。
        ''' </summary>
        ''' <param name="path">FASTA 文件路径</param>
        ''' <param name="lengthA">
        ''' 家族 A 的列数。为 -1 的时候尝试从序列标题之中的 <c>A=</c> 标记解析，
        ''' 再失败则按照序列长度的一半处理。
        ''' </param>
        ''' <param name="lengthB">家族 B 的列数，语义与 <paramref name="lengthA"/> 相同</param>
        ''' <param name="alphabet">残基字母表</param>
        Public Shared Function FromFasta(path As String,
                                         Optional lengthA As Integer = -1,
                                         Optional lengthB As Integer = -1,
                                         Optional alphabet As ResidueAlphabet = Nothing) As JointAlignment

            Dim fsa As FastaFile = FastaFile.Read(path)
            Return FromFasta(fsa, lengthA, lengthB, alphabet)
        End Function

        ''' <summary>
        ''' 从 FASTA 记录集合读取联合比对
        ''' </summary>
        Public Shared Function FromFasta(sequences As IEnumerable(Of FastaSeq),
                                        Optional lengthA As Integer = -1,
                                        Optional lengthB As Integer = -1,
                                        Optional alphabet As ResidueAlphabet = Nothing) As JointAlignment

            Dim alphabetUsed As ResidueAlphabet = If(alphabet, ResidueAlphabet.Standard21)
            Dim list As New List(Of FastaSeq)

            For Each seq As FastaSeq In sequences
                Call list.Add(seq)
            Next

            If list.Count = 0 Then
                Throw New ArgumentException("the joint alignment contains no sequence")
            End If

            Dim length As Integer = list(0).SequenceData.Length

            For Each seq As FastaSeq In list
                If seq.SequenceData.Length <> length Then
                    Throw New ArgumentException(
                        $"the joint alignment requires sequences of equal length, but '{seq.Headers.First}' has {seq.SequenceData.Length} residues while '{list(0).Headers.First}' has {length}")
                End If
            Next

            Dim blockA As Integer = lengthA
            Dim blockB As Integer = lengthB

            If blockA < 0 OrElse blockB < 0 Then
                Dim parsed As Boolean = TryParseBlocks(list(0).Headers.First, blockA, blockB)

                If Not parsed Then
                    blockA = length \ 2
                    blockB = length - blockA
                End If
            End If

            Dim data As Integer(,) = New Integer(list.Count - 1, length - 1) {}
            Dim names As String() = New String(list.Count - 1) {}

            For r As Integer = 0 To list.Count - 1
                Dim codes As Integer() = alphabetUsed.Encode(list(r).SequenceData, list(r).Headers.First)
                names(r) = HeaderId(list(r).Headers.First)

                For c As Integer = 0 To length - 1
                    data(r, c) = codes(c)
                Next
            Next

            Return New JointAlignment(data, names, blockA, blockB, alphabetUsed)
        End Function

        Private Shared Function TryParseBlocks(header As String, ByRef lengthA As Integer, ByRef lengthB As Integer) As Boolean
            Dim mA As Match = Regex.Match(header, "A\s*=\s*(\d+)", RegexOptions.IgnoreCase)
            Dim mB As Match = Regex.Match(header, "B\s*=\s*(\d+)", RegexOptions.IgnoreCase)

            If mA.Success AndAlso mB.Success Then
                lengthA = Integer.Parse(mA.Groups(1).Value)
                lengthB = Integer.Parse(mB.Groups(1).Value)
                Return True
            End If

            Return False
        End Function

        Private Shared Function HeaderId(header As String) As String
            Dim p As Integer = header.IndexOf(" "c)

            Return If(p < 0, header, header.Substring(0, p))
        End Function

        ''' <summary>
        ''' 导出为 FASTA 文本，序列标题之中带有 <c>A=</c> / <c>B=</c> 标记，便于再次读入
        ''' </summary>
        Public Function ToFasta() As String
            Dim sb As New StringBuilder(1024 * 64)
            Dim codes As Integer() = New Integer(Length - 1) {}

            For r As Integer = 0 To Rows - 1
                For c As Integer = 0 To Length - 1
                    codes(c) = _data(r, c)
                Next

                Call sb.AppendLine($">{_names(r)} A={_lengthA} B={_lengthB}")
                Call sb.AppendLine(_alphabet.Decode(codes))
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 保存为联合比对的 FASTA 文件
        ''' </summary>
        Public Sub SaveFasta(path As String)
            Call System.IO.File.WriteAllText(path, ToFasta(), Encoding.UTF8)
        End Sub

#End Region

        Public Overrides Function ToString() As String
            Return $"JointAlignment[{Rows} sequences x ({_lengthA} + {_lengthB}) columns]"
        End Function
    End Class
End Namespace
