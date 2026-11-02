Imports System.Text

Namespace Bayesian.Statistics

    ''' <summary>
    ''' 蛋白质序列的残基字母表。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Burger &amp; van Nimwegen (2008) 的贝叶斯网络方法把比对列当作离散随机变量处理，
    ''' 其字母表包含 20 种标准氨基酸再加上比对空位(同一个符号计数，共 <b>21</b> 个符号)。
    ''' 本类默认就使用这 21 个符号，并且保持一个固定的排列顺序，因为所有的计数数组、
    ''' 因子表和条件概率表都以这个顺序作为下标。
    ''' </para>
    ''' <para>
    ''' 旧实现(<c>SequenceAssembler.AlignmentColumn.ProteinAlphabetDictionary</c>)使用 23 个符号，
    ''' 额外包含了 <c>O</c>(吡咯赖氨酸)与 <c>U</c>(硒代半胱氨酸)。为了让历史数据仍然可以读入，
    ''' 打开 <see cref="IncludeNonStandard"/> 之后这两个符号会被映射到 <c>K</c> 与 <c>C</c>。
    ''' </para>
    ''' </remarks>
    Public Class ResidueAlphabet

        ''' <summary>
        ''' 论文使用的 21 符号字母表：20 种标准氨基酸 + 空位
        ''' </summary>
        Public Const StandardSymbols As String = "ARNDCQEGHILKMFPSTWYV-"

        Private ReadOnly _symbols As Char()
        Private ReadOnly _index As Dictionary(Of Char, Integer)

        ''' <summary>
        ''' 是否把非标准残基(O、U、B、Z、X)映射到最接近的标准残基，而不是直接报错
        ''' </summary>
        Public Property IncludeNonStandard As Boolean

        ''' <summary>
        ''' 使用论文标准的 21 符号字母表
        ''' </summary>
        Public Sub New()
            Me.New(StandardSymbols, includeNonStandard:=True)
        End Sub

        ''' <summary>
        ''' 使用自定义的符号集合
        ''' </summary>
        ''' <param name="symbols">大写字母组成的符号表，必须包含空位符号 '-'</param>
        ''' <param name="includeNonStandard">是否容忍非标准残基</param>
        Public Sub New(symbols As String, Optional includeNonStandard As Boolean = True)
            If String.IsNullOrEmpty(symbols) Then
                Throw New ArgumentException("the alphabet can not be empty", NameOf(symbols))
            End If
            If symbols.IndexOf("-"c) < 0 Then
                Throw New ArgumentException("the alphabet must contain the gap symbol '-'", NameOf(symbols))
            End If

            _symbols = symbols.ToCharArray()
            _index = New Dictionary(Of Char, Integer)

            For i As Integer = 0 To _symbols.Length - 1
                Dim symbol As Char = Char.ToUpperInvariant(_symbols(i))

                If _index.ContainsKey(symbol) Then
                    Throw New ArgumentException($"duplicated symbol '{symbol}' in the alphabet", NameOf(symbols))
                End If

                _index(symbol) = i
            Next

            Me.IncludeNonStandard = includeNonStandard
        End Sub

        ''' <summary>
        ''' 论文标准的 21 符号字母表(共享实例，只读使用)
        ''' </summary>
        Public Shared ReadOnly Property Standard21 As ResidueAlphabet
            Get
                Return _standard21
            End Get
        End Property

        Private Shared ReadOnly _standard21 As New ResidueAlphabet()

        ''' <summary>
        ''' 字母表之中的符号，下标即为编码值
        ''' </summary>
        Public ReadOnly Property Symbols As Char()
            Get
                Return _symbols
            End Get
        End Property

        ''' <summary>
        ''' 符号的个数
        ''' </summary>
        Public ReadOnly Property Size As Integer
            Get
                Return _symbols.Length
            End Get
        End Property

        ''' <summary>
        ''' 空位符号的下标
        ''' </summary>
        Public ReadOnly Property GapIndex As Integer
            Get
                Return _index("-"c)
            End Get
        End Property

        ''' <summary>
        ''' 查找符号对应的编码，非标准符号按照 <see cref="IncludeNonStandard"/> 的设定做映射
        ''' </summary>
        ''' <returns>符号的编码；无法识别的时候返回 -1</returns>
        Public Function IndexOf(symbol As Char) As Integer
            Dim upper As Char = Char.ToUpperInvariant(symbol)
            Dim code As Integer

            If _index.TryGetValue(upper, code) Then
                Return code
            End If

            If Not IncludeNonStandard Then
                Return -1
            End If

            Select Case upper
                Case "O"c, "B"c, "Z"c, "X"c
                    Return IndexOf("K"c)
                Case "U"c
                    Return IndexOf("C"c)
                Case "."c, "~"c
                    Return GapIndex
                Case Else
                    Return -1
            End Select
        End Function

        ''' <summary>
        ''' 获取编码对应的符号
        ''' </summary>
        Public Function Symbol(code As Integer) As Char
            Return _symbols(code)
        End Function

        ''' <summary>
        ''' 把一条序列编码为整数数组
        ''' </summary>
        ''' <exception cref="ArgumentException">序列之中出现无法识别的符号</exception>
        Public Function Encode(sequence As String, Optional name As String = Nothing) As Integer()
            Dim codes As Integer() = New Integer(sequence.Length - 1) {}

            For i As Integer = 0 To sequence.Length - 1
                Dim code As Integer = IndexOf(sequence(i))

                If code < 0 Then
                    Throw New ArgumentException(
                        $"invalid residue symbol '{sequence(i)}' at position {i + 1} of sequence '{If(name, "<anonymous>")}'")
                End If

                codes(i) = code
            Next

            Return codes
        End Function

        ''' <summary>
        ''' 把整数数组还原为序列文本
        ''' </summary>
        Public Function Decode(codes As Integer(), Optional count As Integer = -1) As String
            Dim n As Integer = If(count < 0, codes.Length, count)
            Dim sb As New StringBuilder(n)

            For i As Integer = 0 To n - 1
                Call sb.Append(_symbols(codes(i)))
            Next

            Return sb.ToString()
        End Function

        Public Overrides Function ToString() As String
            Return $"{Size} symbols: {New String(_symbols)}"
        End Function
    End Class
End Namespace
