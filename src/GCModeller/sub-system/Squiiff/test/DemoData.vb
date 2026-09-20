Imports System.IO
Imports SMRUCC.genomics.Analysis.Squiiff.IO
Imports std = System.Math

''' <summary>
''' 合成单细胞扰动基准数据集。
'''
''' ### 为什么用合成数据
''' SquiDiff 需要「细胞 × 基因表达矩阵 + 扰动标签」，而仓库内的现有数据都是**批量时序**表达谱
''' （<c>demo/TestData1</c> 等），没有扰动标签。合成基准可以在"扰动方向已知"的前提下给出
''' **精确的定量真值**，从而直接评估建模效果。
'''
''' ### 生成模型（隐因子模型）
''' <code>
''' 每个"基因程序" k 有一组载荷 L[k][g]
''' 细胞类型 t 有基线程序激活  b[t][k]
''' 扰动 p 有程序空间效应向量 e[p][k]（并按细胞类型有不同强度）
''' 单细胞激活： a = b[t] + strength[t]·e[p] + 细胞噪声
''' 表达量：     y_g = Σ_k a_k·L[k][g]·scale + 基线_g
''' 计数：       counts_g = softplus(y_g)·文库大小·倍数 + 基因噪声
''' </code>
'''
''' ### 关键设计
''' <list type="bullet">
''' <item><b>三种细胞类型</b>各激活一组互不重叠的基因程序，形成可分群结构；</item>
''' <item><b>两种单基因敲除</b>（<c>ko_kinA</c> / <c>ko_kinB</c>）作用在**不同**的程序上
'''       → 二者若可加，组合扰动应当等于两者之和；</item>
''' <item><b>组合扰动</b>（<c>ko_kinA+ko_kinB</c>）额外叠加一个**相互作用项**（非可加），
'''       因此"Δz 向量相加"的预测必然有系统性偏差 —— 这正是 README 强调的
'''       "非可加性基因扰动"场景，用来检验模型解码端的非线性表达能力；</item>
''' <item><b>扰动强度随细胞类型变化</b>（效应方向相同、幅度不同），
'''       用于检验 <c>Δz_sem</c> 的跨细胞类型方向一致性；</item>
''' <item>计数经 <c>softplus</c> + 文库大小缩放，接近真实 scRNA 的非负重尾分布，
'''       后续走"log1p 归一化 → 高变基因 → 逐基因 z-score"的真实预处理链。</item>
''' </list>
''' </summary>
Public Module DemoData

    ''' <summary>对照条件名。</summary>
    Public Const ControlName As String = "ctrl"

    ''' <summary>组合扰动名（用于检验非可加性）。</summary>
    Public Const CombinationName As String = "ko_kinA+ko_kinB"

    ''' <summary>两个单基因敲除扰动名。</summary>
    Public Const KnockoutA As String = "ko_kinA"
    Public Const KnockoutB As String = "ko_kinB"

    ''' <summary>
    ''' 生成合成基准数据。
    ''' </summary>
    ''' <param name="seed">随机种子（保证可复现）。</param>
    ''' <param name="geneCount">基因数。</param>
    ''' <param name="programCount">基因程序（隐因子）个数。</param>
    ''' <param name="cellsPerCondition">每个（细胞类型 × 条件）组合的细胞数。</param>
    Public Function Generate(Optional seed As Integer = 20240920,
                             Optional geneCount As Integer = 200,
                             Optional programCount As Integer = 12,
                             Optional cellsPerCondition As Integer = 50) As SyntheticBenchmark
        Dim rng As New Random(seed)

        Dim geneNames(geneCount - 1) As String
        For g As Integer = 0 To geneCount - 1
            geneNames(g) = $"gene{g + 1:D3}"
        Next

        ' 1) 基因程序载荷：每个程序有一批高载荷基因 + 弱背景
        Dim loading As Double()() = New Double(programCount - 1)() {}
        For k As Integer = 0 To programCount - 1
            Dim row(geneCount - 1) As Double
            Dim targets = 18 + rng.Next(18)
            For t As Integer = 0 To targets - 1
                Dim g = rng.Next(geneCount)
                row(g) = (2.6 + rng.NextDouble() * 1.6) * If(rng.NextDouble() < 0.5, -1.0, 1.0)
            Next
            For g As Integer = 0 To geneCount - 1
                row(g) += (rng.NextDouble() - 0.5) * 0.12
            Next
            loading(k) = row
        Next

        ' 2) 细胞类型基线激活：每种类型激活一组互不重叠的程序
        Dim cellTypes = New String() {"T_alpha", "T_beta", "T_gamma"}
        Dim baseline As Double()() = New Double(cellTypes.Length - 1)() {}
        For t As Integer = 0 To cellTypes.Length - 1
            Dim row(programCount - 1) As Double
            Dim activeFrom = t * (programCount \ cellTypes.Length)
            For k As Integer = 0 To programCount - 1
                If k >= activeFrom AndAlso k < activeFrom + (programCount \ cellTypes.Length) Then
                    row(k) = 1.5 + (rng.NextDouble() - 0.5) * 0.5
                Else
                    row(k) = (rng.NextDouble() - 0.5) * 0.2
                End If
            Next
            baseline(t) = row
        Next

        ' 3) 扰动在程序空间的效应（方向固定、强度随细胞类型变化）
        Dim knockOutA(programCount - 1) As Double
        Dim knockOutB(programCount - 1) As Double
        Dim interaction(programCount - 1) As Double

        Call SetProgramEffect(knockOutA, {2, 5, 9}, {1.2, -1.0, 0.8})
        Call SetProgramEffect(knockOutB, {1, 6, 10}, {-1.1, 1.3, -0.7})
        ' 非可加相互作用项：只在组合扰动中出现
        Call SetProgramEffect(interaction, {3, 11}, {1.6, -1.4})

        Dim strengths = New Double() {1.0, 0.75, 1.25}

        ' 4) 生成单细胞
        Dim conditions = New String() {ControlName, KnockoutA, KnockoutB, CombinationName}
        Dim totalCells = cellTypes.Length * conditions.Length * cellsPerCondition

        Dim counts(totalCells - 1, geneCount - 1) As Double
        Dim cellNames(totalCells - 1) As String
        Dim cellTypeLabels(totalCells - 1) As String
        Dim conditionLabels(totalCells - 1) As String

        Dim libraryBaseline(geneCount - 1) As Double
        For g As Integer = 0 To geneCount - 1
            libraryBaseline(g) = 0.3 + rng.NextDouble() * 0.4
        Next

        Dim cursor As Integer = 0
        For t As Integer = 0 To cellTypes.Length - 1
            For c As Integer = 0 To conditions.Length - 1
                For cell As Integer = 0 To cellsPerCondition - 1
                    ' 单细胞程序激活
                    Dim activation(programCount - 1) As Double
                    For k As Integer = 0 To programCount - 1
                        activation(k) = baseline(t)(k) + CellNoise(rng, 0.3)
                    Next

                    Select Case conditions(c)
                        Case KnockoutA
                            AddScaled(activation, knockOutA, strengths(t))
                        Case KnockoutB
                            AddScaled(activation, knockOutB, strengths(t))
                        Case CombinationName
                            ' 组合 = A + B + 相互作用（非可加）
                            AddScaled(activation, knockOutA, strengths(t))
                            AddScaled(activation, knockOutB, strengths(t))
                            AddScaled(activation, interaction, strengths(t))
                    End Select

                    ' 文库大小（测序深度）差异
                    Dim librarySize = std.Exp(CellNoise(rng, 0.22))

                    For g As Integer = 0 To geneCount - 1
                        Dim y As Double = libraryBaseline(g)
                        For k As Integer = 0 To programCount - 1
                            y += activation(k) * loading(k)(g) * 0.5
                        Next

                        Dim value = SoftPlus(y) * librarySize * 3.0 + 0.05
                        value *= std.Exp(CellNoise(rng, 0.08))        ' 基因级噪声
                        counts(cursor, g) = value
                    Next

                    cellNames(cursor) = $"{cellTypes(t)}_{conditions(c)}_{cell + 1:D3}"
                    cellTypeLabels(cursor) = cellTypes(t)
                    conditionLabels(cursor) = conditions(c)
                    cursor += 1
                Next
            Next
        Next

        Return New SyntheticBenchmark With {
            .GeneNames = geneNames,
            .CellNames = cellNames,
            .CellTypes = cellTypes,
            .Conditions = conditions,
            .CellTypeLabels = cellTypeLabels,
            .ConditionLabels = conditionLabels,
            .Counts = counts,
            .Seed = seed
        }
    End Function

    Private Function CellNoise(rng As Random, scale As Double) As Double
        ' Box-Muller
        Dim u1 As Double = 1.0 - rng.NextDouble()
        Dim u2 As Double = 1.0 - rng.NextDouble()
        Return scale * std.Sqrt(-2.0 * std.Log(u1)) * std.Sin(2.0 * std.PI * u2)
    End Function

    Private Sub SetProgramEffect(target As Double(), programs As Integer(), weights As Double())
        For i As Integer = 0 To programs.Length - 1
            target(programs(i)) = weights(i)
        Next
    End Sub

    Private Sub AddScaled(activation As Double(), effect As Double(), scale As Double)
        For k As Integer = 0 To activation.Length - 1
            activation(k) += effect(k) * scale
        Next
    End Sub

    Private Function SoftPlus(x As Double) As Double
        If x > 30.0 Then Return x
        If x < -30.0 Then Return 0.0
        Return std.Log(1.0 + std.Exp(x))
    End Function
End Module

''' <summary>合成基准数据集。</summary>
Public Class SyntheticBenchmark

    Public Property GeneNames As String()
    Public Property CellNames As String()
    Public Property CellTypes As String()
    Public Property Conditions As String()

    ''' <summary>与 <see cref="CellNames"/> 等长的细胞类型标签。</summary>
    Public Property CellTypeLabels As String()

    ''' <summary>与 <see cref="CellNames"/> 等长的扰动条件标签。</summary>
    Public Property ConditionLabels As String()

    ''' <summary>原始计数 <c>[cell, gene]</c>。</summary>
    Public Property Counts As Double(,)

    Public Property Seed As Integer

    Public ReadOnly Property NCell As Integer
        Get
            Return CellNames.Length
        End Get
    End Property

    Public ReadOnly Property NGene As Integer
        Get
            Return GeneNames.Length
        End Get
    End Property

    ''' <summary>取满足指定细胞类型与扰动条件的细胞行下标。</summary>
    Public Function Indices(cellType As String, condition As String) As Integer()
        Dim result As New List(Of Integer)
        For i As Integer = 0 To NCell - 1
            If String.Equals(CellTypeLabels(i), cellType, StringComparison.OrdinalIgnoreCase) AndAlso
               String.Equals(ConditionLabels(i), condition, StringComparison.OrdinalIgnoreCase) Then
                result.Add(i)
            End If
        Next
        Return result.ToArray()
    End Function

    ''' <summary>全部对照细胞的行下标。</summary>
    Public Function ControlIndices() As Integer()
        Dim result As New List(Of Integer)
        For i As Integer = 0 To NCell - 1
            If String.Equals(ConditionLabels(i), DemoData.ControlName, StringComparison.OrdinalIgnoreCase) Then
                result.Add(i)
            End If
        Next
        Return result.ToArray()
    End Function

    ''' <summary>用于训练的条件集合（组合扰动刻意不出现在训练集中）。</summary>
    Public Function TrainingConditions() As String()
        Return New String() {DemoData.ControlName, DemoData.KnockoutA, DemoData.KnockoutB}
    End Function

    ''' <summary>取出指定行子集构成的基准（仅保留给定行）。</summary>
    Public Function Subset(rowIndices As Integer()) As SyntheticBenchmark
        Dim geneCount = NGene
        Dim counts(rowIndices.Length - 1, geneCount - 1) As Double
        Dim names(rowIndices.Length - 1) As String
        Dim types(rowIndices.Length - 1) As String
        Dim conditions(rowIndices.Length - 1) As String

        For i As Integer = 0 To rowIndices.Length - 1
            names(i) = CellNames(rowIndices(i))
            types(i) = CellTypeLabels(rowIndices(i))
            conditions(i) = ConditionLabels(rowIndices(i))

            For g As Integer = 0 To geneCount - 1
                counts(i, g) = Counts(rowIndices(i), g)
            Next
        Next

        Return New SyntheticBenchmark With {
            .GeneNames = GeneNames,
            .CellNames = names,
            .CellTypes = CellTypes,
            .Conditions = Conditions,
            .CellTypeLabels = types,
            .ConditionLabels = conditions,
            .Counts = counts,
            .Seed = Seed
        }
    End Function

    ''' <summary>
    ''' 导出为 CSV：
    ''' <list type="bullet">
    ''' <item><c>single_cell_counts.csv</c> —— **基因 × 细胞** 宽表（首列基因名、首行细胞名），
    '''       即通用加载链 <c>Matrix.LoadData</c> 期望的格式；</item>
    ''' <item><c>cell_metadata.csv</c> —— 细胞 → 细胞类型 / 扰动条件的标签表。</item>
    ''' </list>
    ''' </summary>
    Public Sub SaveCsv(directory As String)
        Call Directory.CreateDirectory(directory)

        ' 基因 × 细胞 宽表
        Dim rows As New List(Of Double())
        For g As Integer = 0 To NGene - 1
            Dim values(NCell - 1) As Double
            For i As Integer = 0 To NCell - 1
                values(i) = Counts(i, g)
            Next
            rows.Add(values)
        Next

        Call ResultWriter.WriteNumericTable(Path.Combine(directory, "single_cell_counts.csv"),
                                            CellNames, rows, GeneNames)

        ' 细胞元数据
        Dim metadata As New List(Of String())
        For i As Integer = 0 To NCell - 1
            metadata.Add(New String() {CellTypeLabels(i), ConditionLabels(i)})
        Next

        Call ResultWriter.WriteTextTable(Path.Combine(directory, "cell_metadata.csv"),
                                         New String() {"cell_type", "condition"},
                                         metadata, CellNames)
    End Sub

    Public Overrides Function ToString() As String
        Return $"合成基准 {NCell} 细胞 × {NGene} 基因（{CellTypes.Length} 细胞类型 × {Conditions.Length} 条件）"
    End Function
End Class
