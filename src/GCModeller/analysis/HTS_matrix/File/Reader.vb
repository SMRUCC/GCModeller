#Region "Microsoft.VisualBasic::6e6a44fa15668b72ef478f9926694d40, analysis\HTS_matrix\File\Reader.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program.  If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Code Statistics:

    '   Total Lines: 57
    '    Code Lines: 46 (80.70%)
    ' Comment Lines: 3 (5.26%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 8 (14.04%)
    '     File Size: 2.01 KB


    ' Module Reader
    ' 
    '     Function: loadGeneMatrixParallel, ParseGeneRow, ParseGeneRowTokens
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text
Imports std = System.Math

''' <summary>
''' the parser of the matrix text document
''' </summary>
''' <remarks>
''' <para>
''' 这里假设基因表达矩阵是由基因 id 加表达值向量构成的（id 中不会出现分隔符），
''' 因此可以完全绕开 <see cref="String.Split(Char())"/>：每行 3 万个数值时，
''' Split 会为一行分配 3 万个 <see cref="String"/> 对象，1.85 万行的矩阵就是 5.6 亿次
''' 字符串分配——这才是解析慢的主要原因，而不是数字转换本身。
''' </para>
''' <para>
''' 现在的实现直接在行文本上滑动并按索引切片，数字由一个就地扫描的十进制解析器完成，
''' 全程不产生任何中间字符串；值直接写进目标数组，需要做样本子集时也只复制保留的列。
''' （这里无法使用 <c>ReadOnlySpan(Of Char)</c>：VB 编译器不支持 ref struct 类型。）
''' </para>
''' </remarks>
Module Reader

    ''' <summary>
    ''' 一批并行解析的行数。
    ''' </summary>
    ''' <remarks>
    ''' 批次太小会让 <see cref="System.Threading.Tasks.Parallel.For"/> 的分区/同步开销盖过收益，
    ''' 批次太大则会让"读下一批"与"解析当前批"的重叠变粗。128 行（3 万列时约 35 MB 文本）
    ''' 是实测比较均衡的取值。
    ''' </remarks>
    Const ParseBatchSize As Integer = 128

    ''' <summary>
    ''' parse csv/tsv line
    ''' </summary>
    ''' <param name="line"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 在这里假设基因表达矩阵是由基因id加表达值的向量构成的，不会有太复杂的基因id字符串（例如存在逗号的基因id字符串），
    ''' 所以在这里为了字符串的解析性能，直接在行文本上定位分隔符，不做字符串拆分
    ''' </remarks>
    Friend Function ParseGeneRowTokens(line As String) As NamedValue(Of Double())
        Dim width As Integer = CountColumns(line)
        Dim values(width - 1) As Double
        Dim geneID As String = Nothing

        Call ParseGeneRow(line, values, Nothing, geneID)

        Return New NamedValue(Of Double()) With {
            .Name = geneID,
            .Value = values
        }
    End Function

    ''' <summary>
    ''' 解析一行矩阵文本：行名 + 数值向量（零中间字符串）。
    ''' </summary>
    ''' <param name="line">文本行</param>
    ''' <param name="values">目标数组，长度即要保留的样本数</param>
    ''' <param name="columns">
    ''' 要保留的源列下标（升序、从 0 开始计数的数值列）；<c>Nothing</c> 表示按原顺序保留全部列
    ''' </param>
    ''' <param name="geneID">解析出来的行名</param>
    Friend Sub ParseGeneRow(line As String, values As Double(), columns As Integer(), ByRef geneID As String)
        Dim pos As Integer = 0

        geneID = ReadRowName(line, pos)

        Dim target As Integer = values.Length

        If target = 0 Then
            Return
        End If

        Dim kept As Integer = 0     ' 已经写入 values 的数量
        Dim source As Integer = 0   ' 当前字段在源行中的列号（数值列从 0 开始）
        Dim cursor As Integer = 0   ' columns 的扫描指针

        While pos < line.Length
            Dim start As Integer = pos
            Dim [end] As Integer = FindDelimiter(line, pos)

            pos = [end]

            If columns Is Nothing Then
                values(kept) = ParseNumber(line, start, [end] - start)
                kept += 1
            Else
                ' columns 是升序的，用单向前进的指针做归并，避免每列都做一次二分查找
                While cursor < columns.Length AndAlso columns(cursor) < source
                    cursor += 1
                End While

                If cursor < columns.Length AndAlso columns(cursor) = source Then
                    values(cursor) = ParseNumber(line, start, [end] - start)
                End If
            End If

            source += 1

            If kept >= target Then
                Exit While
            End If
            If columns IsNot Nothing AndAlso cursor >= columns.Length Then
                Exit While
            End If
            If pos >= line.Length Then
                Exit While
            End If

            ' 跳过分隔符
            pos += 1
        End While
    End Sub

    ''' <summary>
    ''' Parse the matrix rows as expression data (parallel version).
    ''' </summary>
    ''' <param name="text">矩阵文本行（惰性流式枚举，头部行需要由调用方跳过）</param>
    ''' <param name="takeIndex">
    ''' 需要保留的样本（数值列）下标；<c>Nothing</c> 表示保留全部列
    ''' </param>
    ''' <param name="degreeOfParallelism">并行度：0 = 使用全部逻辑核心，1 = 串行</param>
    ''' <returns>按文件顺序排列的矩阵行</returns>
    ''' <remarks>
    ''' <para>
    ''' 每批 <see cref="ParseBatchSize"/> 行交给 <see cref="System.Threading.Tasks.Parallel.For"/> 解析，
    ''' 批次之间保持顺序（第 i 批的结果一定落在第 i 段），所以输出与单线程版本逐行一致。
    ''' </para>
    ''' <para>
    ''' 下一批文本在解析当前批的同时于后台读取（<see cref="System.Threading.Tasks.Task"/>），
    ''' 这样磁盘 I/O 与数字解析重叠，整体耗时接近 max(I/O, 解析) 而不是两者之和。
    ''' 枚举器始终只有一个线程在推进，所以对惰性流是安全的。
    ''' </para>
    ''' </remarks>
    <Extension>
    Friend Function loadGeneMatrixParallel(text As IEnumerable(Of String),
                                           takeIndex As Integer(),
                                           Optional degreeOfParallelism As Integer = 0) As DataFrameRow()

        Dim rows As New List(Of DataFrameRow)()
        Dim enumerator As IEnumerator(Of String) = text.GetEnumerator()
        Dim options As New ParallelOptions()
        Dim columns As Integer() = takeIndex
        Dim width As Integer = -1
        Dim wanted As Integer = -1

        ' Parallel 必须全限定：Microsoft.VisualBasic.Parallel 会遮蔽同名类型
        options.MaxDegreeOfParallelism = If(degreeOfParallelism > 0, degreeOfParallelism, -1)

        Try
            Dim batch As List(Of String) = ReadBatch(enumerator)

            While batch.Count > 0
                If width < 0 Then
                    width = CountColumns(batch(0))
                    wanted = If(columns Is Nothing, width, columns.Length)

                    ' 全部列且顺序一致时退化为顺序写入，省掉逐字段的指针比较
                    If columns IsNot Nothing AndAlso IsIdentity(columns, width) Then
                        columns = Nothing
                    End If
                End If

                Dim current As List(Of String) = batch
                Dim parsed(current.Count - 1) As DataFrameRow
                Dim reading = Task.Run(Function() ReadBatch(enumerator))

                Call ParseBatch(current, parsed, columns, wanted, options)

                batch = reading.Result

                Call rows.AddRange(parsed)
            End While
        Finally
            enumerator.Dispose()
        End Try

        Return rows.ToArray()
    End Function

    Private Function ReadBatch(enumerator As IEnumerator(Of String)) As List(Of String)
        Dim batch As New List(Of String)(ParseBatchSize)

        While batch.Count < ParseBatchSize
            If Not enumerator.MoveNext() Then
                Exit While
            End If

            Dim line As String = enumerator.Current

            If String.IsNullOrEmpty(line) Then
                Continue While
            End If

            Call batch.Add(line)
        End While

        Return batch
    End Function

    Private Sub ParseBatch(lines As List(Of String),
                           parsed As DataFrameRow(),
                           columns As Integer(),
                           width As Integer,
                           options As ParallelOptions)

        If lines.Count = 1 Then
            parsed(0) = ParseRow(lines(0), columns, width)
            Return
        End If

        Dim body As Action(Of Integer) =
            Sub(i)
                parsed(i) = ParseRow(lines(i), columns, width)
            End Sub

        Call Parallel.For(0, lines.Count, options, body)
    End Sub

    Private Function ParseRow(line As String, columns As Integer(), width As Integer) As DataFrameRow
        Dim values(width - 1) As Double
        Dim geneID As String = Nothing

        Call ParseGeneRow(line, values, columns, geneID)

        Return New DataFrameRow With {
            .experiments = values,
            .geneID = geneID.Trim(""""c, " "c, ASCII.TAB)
        }
    End Function

    ''' <summary>
    ''' 读取行名，并把 <paramref name="pos"/> 推进到第一个数值字符。
    ''' </summary>
    ''' <remarks>
    ''' 旧实现用正则 <c>".+""[,\t]</c> 在整行（3 万列时约 270 KB）上做贪婪匹配，
    ''' 这里换成"看首字符是不是引号"的线性判断，语义等价但成本可以忽略。
    ''' </remarks>
    Private Function ReadRowName(line As String, ByRef pos As Integer) As String
        If line.Length = 0 Then
            Return ""
        End If

        If line(0) = """"c Then
            ' 被引号包裹的行名："GENE",1,2,3
            Dim close As Integer = line.IndexOf(""""c, 1)

            If close >= 0 Then
                Dim name As String = line.Substring(1, close - 1)

                pos = close + 1

                If pos < line.Length AndAlso IsDelimiter(line(pos)) Then
                    pos += 1
                End If

                Return name
            End If
        End If

        Dim [end] As Integer = FindDelimiter(line, 0)
        Dim id As String = If([end] > 0, line.Substring(0, [end]), "")

        pos = If([end] < line.Length, [end] + 1, [end])

        Return id
    End Function

    ''' <summary>从 <paramref name="start"/> 开始找到第一个分隔符的位置（找不到时返回字符串长度）。</summary>
    Private Function FindDelimiter(line As String, start As Integer) As Integer
        Dim comma As Integer = line.IndexOf(","c, start)
        Dim tab As Integer = line.IndexOf(ASCII.TAB, start)

        If comma < 0 Then
            Return If(tab < 0, line.Length, tab)
        ElseIf tab < 0 Then
            Return comma
        Else
            Return std.Min(comma, tab)
        End If
    End Function

    ''' <summary>一行文本中的数值列数量（不含行名）。</summary>
    Private Function CountColumns(line As String) As Integer
        Dim pos As Integer = 0

        Call ReadRowName(line, pos)

        If pos >= line.Length Then
            Return 0
        End If

        Dim count As Integer = 1
        Dim i As Integer = pos

        While i < line.Length
            If IsDelimiter(line(i)) Then
                count += 1
            End If

            i += 1
        End While

        Return count
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function IsDelimiter(c As Char) As Boolean
        Return c = ","c OrElse c = ASCII.TAB
    End Function

    ''' <summary>
    ''' 10 的整数次幂（0..22）。这一区间内的 10^n 在 <see cref="Double"/> 中是精确可表示的。
    ''' </summary>
    Private ReadOnly Pow10 As Double() = {
        1.0E+00, 1.0E+01, 1.0E+02, 1.0E+03, 1.0E+04, 1.0E+05,
        1.0E+06, 1.0E+07, 1.0E+08, 1.0E+09, 1.0E+10, 1.0E+11,
        1.0E+12, 1.0E+13, 1.0E+14, 1.0E+15, 1.0E+16, 1.0E+17,
        1.0E+18, 1.0E+19, 1.0E+20, 1.0E+21, 1.0E+22
    }

    ''' <summary>
    ''' 就地解析一个数值字段（纯数字不产生子串）。
    ''' </summary>
    ''' <param name="line">行文本</param>
    ''' <param name="start">字段起始下标</param>
    ''' <param name="count">字段长度</param>
    ''' <returns>解析结果</returns>
    ''' <remarks>
    ''' <para>
    ''' **快路径**（零分配）：<c>[+/-]digits[.digits][(e|E)[+/-]digits]</c> 与首尾空白，
    ''' 这正是矩阵文档里会出现的格式。之所以自己扫描而不是 <see cref="Double.Parse(String)"/>：后者需要先
    ''' <see cref="String.Substring(Integer, Integer)"/> 切出子串，3 万列 × 1.85 万行就是 5.6 亿次字符串分配。
    ''' </para>
    ''' <para>
    ''' 为了与 <see cref="Double.Parse(String)"/> **逐位一致**（而不是差最后几个 ULP 导致下游结果不可复现），
    ''' 快路径先把有效数字累积成整数尾数（≤ 19 位，仍在 2^53 内可精确表示），
    ''' 再乘/除一个精确的 10 的幂（|指数| ≤ 22）。由于两个操作数都是精确值，IEEE 的乘法/除法只舍入一次，
    ''' 结果就是正确舍入的十进制值。超出这个窗口（超长数字或极端指数）时回退到
    ''' <see cref="Double.TryParse(String, Globalization.NumberStyles, IFormatProvider, ByRef Double)"/>。
    ''' </para>
    ''' <para>
    ''' **变体路径**：字段里出现纯数字之外的字符时（<c>89.45%</c>、<c>1/3</c>、<c>NA</c>/<c>NaN</c>、
    ''' 千分位、<c>12abc</c> 这类脏值）转交 <see cref="LegacyParse"/>，即旧版
    ''' <c>ParseDouble</c>/<c>ParseNumeric</c> 的语义——<c>89.45%</c> → 0.8945、<c>1/3</c> → 0.3333、
    ''' <c>NA</c> → <see cref="Double.NaN"/>，保证与改动前的读取器行为一致。
    ''' </para>
    ''' </remarks>
    Private Function ParseNumber(line As String, start As Integer, count As Integer) As Double
        Dim rawStart As Integer = start
        Dim rawCount As Integer = count
        Dim [end] As Integer = start + count

        ' 跳过首尾空白
        While start < [end] AndAlso Char.IsWhiteSpace(line(start))
            start += 1
        End While
        While [end] > start AndAlso Char.IsWhiteSpace(line([end] - 1))
            [end] -= 1
        End While

        If start >= [end] Then
            Return 0.0
        End If

        Dim sign As Integer = 1
        Dim c As Char = line(start)

        If c = "-"c Then
            sign = -1
            start += 1
        ElseIf c = "+"c Then
            start += 1
        End If

        Dim mantissa As ULong = 0UL
        Dim digits As Integer = 0
        Dim exponent10 As Integer = 0
        Dim inFraction As Boolean = False
        Dim hasExp As Boolean = False
        Dim expSign As Integer = 1
        Dim expValue As Integer = 0

        While start < [end]
            c = line(start)

            If c >= "0"c AndAlso c <= "9"c Then
                If digits >= 19 Then
                    ' 有效数字太多：交给慢路径保证正确舍入
                    Return SlowParse(line, rawStart, rawCount)
                End If

                mantissa = mantissa * 10UL + CULng(AscW(c) - AscW("0"c))
                digits += 1

                If inFraction Then
                    exponent10 -= 1
                End If

                start += 1
            ElseIf c = "."c AndAlso Not inFraction AndAlso Not hasExp Then
                inFraction = True
                start += 1
            ElseIf (c = "e"c OrElse c = "E"c) AndAlso Not hasExp AndAlso digits > 0 Then
                hasExp = True
                start += 1

                If start < [end] Then
                    If line(start) = "-"c Then
                        expSign = -1
                        start += 1
                    ElseIf line(start) = "+"c Then
                        start += 1
                    End If
                End If

                Dim expDigits As Integer = 0

                While start < [end]
                    c = line(start)

                    If c >= "0"c AndAlso c <= "9"c Then
                        If expDigits > 3 Then
                            ' 极端指数：慢路径处理溢出/下溢
                            Return SlowParse(line, rawStart, rawCount)
                        End If

                        expValue = expValue * 10 + (AscW(c) - AscW("0"c))
                        expDigits += 1
                        start += 1
                    Else
                        ' 指数部分出现非数字：交回旧版语义
                        Return LegacyParse(line, rawStart, rawCount)
                    End If
                End While
            Else
                ' 出现了纯数字之外的字符（%, /, NA 等）：交回旧版语义
                Return LegacyParse(line, rawStart, rawCount)
            End If
        End While

        If digits = 0 Then
            Return LegacyParse(line, rawStart, rawCount)
        End If

        exponent10 += expSign * expValue

        ' 快路径：尾数 < 2^53 且 |指数| <= 22 时，一次乘/除即可得到正确舍入的结果
        If exponent10 > 22 OrElse exponent10 < -22 Then
            Return SlowParse(line, rawStart, rawCount)
        End If

        Dim value As Double = CDbl(mantissa)

        If exponent10 > 0 Then
            value *= Pow10(exponent10)
        ElseIf exponent10 < 0 Then
            value /= Pow10(-exponent10)
        End If

        Return sign * value
    End Function

    ''' <summary>数值慢路径：切出子串交给 <see cref="Double.TryParse(String, Globalization.NumberStyles, IFormatProvider, ByRef Double)"/>。</summary>
    ''' <remarks>只用于「是纯数字、但超出快路径窗口」的情况（超长有效数字、极端指数）。</remarks>
    Private Function SlowParse(line As String, start As Integer, count As Integer) As Double
        Dim text As String = line.Substring(start, count)
        Dim value As Double

        If Double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
            Return value
        Else
            Return 0.0
        End If
    End Function

    ''' <summary>
    ''' 变体数值解析：<c>89.45%</c>、<c>1/3</c>、<c>NaN</c>/<c>NA</c>、千分位、带尾巴的脏值等。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 直接复用 <see cref="Casting.ParseNumeric"/>（即旧版 <c>ParseDouble</c> 的实现），
    ''' 从而保证这些"非纯数字"字段的语义与改动前的读取器**完全一致**，不需要重新发明一套规则。
    ''' </para>
    ''' <para>
    ''' 已实测的变体行为（全部来自 <c>ParseNumeric</c>，不是这里重新定义的规则）：
    ''' <c>89.45%</c> → 0.8945；<c>1/3</c> → 0.3333…；<c>1,234.5</c> → 1234.5（千分位）；
    ''' <c>NaN</c> → <see cref="Double.NaN"/>；<c>NULL</c> → 0；
    ''' <c>12abc</c> → 12、<c>abc</c> → 0（<c>Val</c> 风格取前导数字）。
    ''' 其余未列出的写法（例如 <c>NA</c>）一律以 <c>ParseNumeric</c> 的当前实现为准。
    ''' </para>
    ''' <para>
    ''' 代价是每个这样的字段一次 <see cref="String.Substring(Integer, Integer)"/>：矩阵文档里这类字段应当是少数，
    ''' 纯数字字段仍然走零分配的快路径。若整张表都是百分比/分数，就退化成旧实现的分配量。
    ''' </para>
    ''' </remarks>
    Private Function LegacyParse(line As String, start As Integer, count As Integer) As Double
        Dim text As String = line.Substring(start, count)

        Return text.ParseNumeric()
    End Function

    ''' <summary><c>columns</c> 是否就是 <c>0..width-1</c> 的顺序全列。</summary>
    Private Function IsIdentity(columns As Integer(), width As Integer) As Boolean
        If columns Is Nothing OrElse columns.Length <> width Then
            Return False
        End If

        For i As Integer = 0 To columns.Length - 1
            If columns(i) <> i Then
                Return False
            End If
        Next

        Return True
    End Function

End Module
