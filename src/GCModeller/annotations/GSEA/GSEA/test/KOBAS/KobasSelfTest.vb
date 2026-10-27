Imports System.Globalization
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.DataFrame.Formats
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KOBAS
Imports std = System.Math

Namespace Global.KOBAS.test

    ''' <summary>
    ''' 轻量断言框架与合成数据构造工具。
    ''' </summary>
    Public Module KobasSelfTest

        Private m_passed As Integer
        Private m_failures As Integer
        Private m_skips As Integer

        ''' <summary>通过的断言数量。</summary>
        Public ReadOnly Property Passed As Integer
            Get
                Return m_passed
            End Get
        End Property

        ''' <summary>失败的断言数量。</summary>
        Public ReadOnly Property Failures As Integer
            Get
                Return m_failures
            End Get
        End Property

        ''' <summary>跳过的断言数量。</summary>
        Public ReadOnly Property Skips As Integer
            Get
                Return m_skips
            End Get
        End Property

        ''' <summary>
        ''' 断言一个布尔条件。
        ''' </summary>
        ''' <param name="name">断言名称。</param>
        ''' <param name="ok">条件是否成立。</param>
        ''' <param name="detail">附加说明。</param>
        Public Sub Check(name As String, ok As Boolean, Optional detail As String = "")
            If ok Then
                m_passed += 1
                Console.WriteLine($"  [PASS] {name}  {detail}")
            Else
                m_failures += 1
                Console.WriteLine($"  [FAIL] {name}  {detail}")
            End If
        End Sub

        ''' <summary>
        ''' 断言两个浮点数在给定容差内相等。
        ''' </summary>
        ''' <param name="name">断言名称。</param>
        ''' <param name="expected">期望值。</param>
        ''' <param name="actual">实际值。</param>
        ''' <param name="tolerance">绝对容差。</param>
        ''' <param name="detail">附加说明。</param>
        Public Sub CheckClose(name As String, expected As Double, actual As Double, tolerance As Double, Optional detail As String = "")
            Dim ok As Boolean

            If Double.IsNaN(expected) AndAlso Double.IsNaN(actual) Then
                ok = True
            ElseIf Double.IsInfinity(expected) OrElse Double.IsInfinity(actual) Then
                ok = expected = actual
            Else
                ok = std.Abs(expected - actual) <= tolerance
            End If

            Call Check(name, ok, $"{detail} expected={expected.ToString("G10", CultureInfo.InvariantCulture)} actual={actual.ToString("G10", CultureInfo.InvariantCulture)} |diff|={std.Abs(expected - actual).ToString("G3", CultureInfo.InvariantCulture)}")
        End Sub

        ''' <summary>
        ''' 记录一条因为环境缺失而跳过的断言。
        ''' </summary>
        ''' <param name="name">断言名称。</param>
        ''' <param name="reason">跳过原因。</param>
        Public Sub Skip(name As String, reason As String)
            m_skips += 1
            Console.WriteLine($"  [SKIP] {name}  {reason}")
        End Sub

        ''' <summary>
        ''' 构造一个只有两个表型分组的 CLS 对象。
        ''' </summary>
        ''' <param name="labels">每个样本的分组标签（0/1）。</param>
        ''' <returns>与之对应的表型对象。</returns>
        Public Function MakePhenotype(labels As Integer()) As phenotype
            ' 注意：phenotype 会用 classNameMaps 的 Value 与 sampleClass 的标签字符串比较，
            ' 因此这里的 Key/Value 必须一致（与 CLS.ParseFile 的行为保持一致）
            Dim names As New Dictionary(Of String, String)(StringComparer.Ordinal) From {
                {"N", "N"},
                {"T", "T"}
            }
            Dim classes As String() = labels _
                .Select(Function(label) If(label = 0, "N", "T")) _
                .ToArray
            Dim cls As New CLS With {
                .numOfSamples = labels.Length,
                .numOfClasses = 2,
                .classNameMaps = names,
                .sampleClass = classes
            }

            Return New phenotype(cls)
        End Function

        ''' <summary>
        ''' 构造一个表达矩阵。
        ''' </summary>
        ''' <param name="genes">基因编号。</param>
        ''' <param name="samples">样本编号。</param>
        ''' <param name="values">按行存放的表达值。</param>
        ''' <returns></returns>
        Public Function MakeMatrix(genes As String(), samples As String(), values As Double()()) As Matrix
            Return New Matrix With {
                .tag = "synthetic",
                .sampleID = samples,
                .expression = genes _
                    .Select(Function(gene, i)
                                Return New DataFrameRow With {
                                    .geneID = gene,
                                    .experiments = values(i)
                                }
                            End Function) _
                    .ToArray
            }
        End Function

        ''' <summary>
        ''' 构造一个基因集背景模型。
        ''' </summary>
        ''' <param name="sets">基因集编号 → 成员基因。</param>
        ''' <returns></returns>
        Public Function MakeBackground(sets As Dictionary(Of String, String())) As Background
            Dim clusters As Cluster() = sets _
                .Select(Function(item)
                            Return New Cluster With {
                                .ID = item.Key,
                                .names = item.Key,
                                .description = item.Key,
                                .members = item.Value.Select(Function(gene) New BackgroundGene(gene)).ToArray
                            }
                        End Function) _
                .ToArray

            Return New Background With {
                .name = "synthetic",
                .comments = "KOBAS self-test",
                .build = Now,
                .clusters = clusters,
                .size = clusters.SelectMany(Function(cluster) cluster.memberIds).Distinct().Count()
            }
        End Function

        ''' <summary>
        ''' 输出断言统计信息。
        ''' </summary>
        ''' <returns>文本摘要。</returns>
        Public Function SummaryText() As String
            Return $"通过 {m_passed} 项断言，失败 {m_failures} 项，跳过 {m_skips} 项"
        End Function

        ''' <summary>
        ''' 进程退出码：有失败时返回 1。
        ''' </summary>
        ''' <returns></returns>
        Public Function ExitCode() As Integer
            Return If(m_failures = 0, 0, 1)
        End Function

        ''' <summary>
        ''' 生成一个确定性的伪随机序列（避免依赖 System.Random 的版本差异）。
        ''' </summary>
        ''' <param name="seed">随机种子。</param>
        ''' <param name="count">序列长度。</param>
        ''' <returns>取值在 <c>[0, 1)</c> 之间的序列。</returns>
        Public Function DeterministicUniform(seed As Integer, count As Integer) As Double()
            Dim rng As New Random(seed)
            Dim values As Double() = New Double(count - 1) {}

            For i As Integer = 0 To count - 1
                values(i) = rng.NextDouble()
            Next

            Return values
        End Function
    End Module
End Namespace
