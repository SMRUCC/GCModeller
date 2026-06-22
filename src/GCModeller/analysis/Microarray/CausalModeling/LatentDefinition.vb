Imports Microsoft.VisualBasic.Data.Trinity
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Scripting.Expressions
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Public Class LatentDefinition

    Public Property varName As String

    ''' <summary>
    ''' manifestIndices
    ''' </summary>
    ''' <returns></returns>
    Public Property featureIDs As String()
    Public Property mode As MeasurementModels = MeasurementModels.A

    Public ReadOnly Property size As Integer
        Get
            Return featureIDs.TryCount
        End Get
    End Property

    Sub New(name As String, manifest As IEnumerable(Of String), Optional mode As MeasurementModels = MeasurementModels.A)
        _varName = name
        _featureIDs = manifest.ToArray
        _mode = mode
    End Sub

    Public Overrides Function ToString() As String
        Return $"{varName}({mode.ToString}) - [{size} x manifest_vars, {featureIDs.Concatenate }]"
    End Function

    ''' <summary>
    ''' 执行 MAD初筛 + 冗余剔除 流程，筛选出Top5显变量
    ''' </summary>
    ''' <param name="manifest">某一个潜变量中所有的未经筛选的显变量矩阵</param>
    ''' <param name="corrThreshold">
    ''' 冗余剔除的相关性阈值 (绝对值)
    ''' </param>
    ''' <param name="madPoolSize">
    ''' MAD初筛截取的前K个变量数量
    ''' </param>
    ''' <param name="targetManifests">
    ''' 最终需要获取的变量数量
    ''' </param>
    ''' <returns>筛选出的显变量 geneID 字符串数组</returns>
    ''' <remarks>
    ''' MAD初筛 + 冗余剔除（MAD + Correlation Filter）
    ''' 第一步：MAD排序初筛
    ''' 对 flavone : Flavones (24个) 计算 MAD，按降序排列。
    ''' *不要直接取前5，而是先截取前 8-10 个作为候选池。*
    ''' 第二步：去冗余（解决共线性）
    ''' 计算这前 8-10 个候选变量的相关系数矩阵。
    ''' 从相关系数最高的一对变量中，剔除MAD值稍小的那个。
    ''' 重复此过程，直到剩余变量两两之间的相关系数均低于某个阈值（例如 ∣r∣ &lt; 0.7 或 0.8）。
    ''' 第三步：定稿
    ''' 如果去冗余后剩余变量多于5个，再按MAD值取Top 5；如果少于5个（比如剩3个），那3个就足够了。
    ''' 这样做的好处：选出的Top 5不仅自身变异丰富（MAD大），而且彼此之间相对独立，能从不同角度代表该潜变量，使得潜变量得分（LV scores）的估计更加稳健和具有代表性。
    ''' </remarks>
    Public Shared Function FilterTopManifestVariables(manifest As Matrix,
                                                      Optional targetManifests As Integer = 5,
                                                      Optional corrThreshold As Double = 0.8,
                                                      Optional madPoolSize As Integer = 10) As IEnumerable(Of String)
        ' 边界条件检查
        If manifest Is Nothing OrElse manifest.expression Is Nothing OrElse manifest.expression.Length = 0 Then
            Return New String() {}
        End If

        Dim madList As DataFrameRow() = manifest.expression

        ' 如果总变量数不超过5个，直接返回所有 geneID
        If madList.Length <= 5 Then
            Return From r As DataFrameRow In madList Select r.geneID
        End If

        ' 按MAD降序排序并取前 madPoolSize 个作为候选池
        Dim candidatePool = madList.OrderByDescending(Function(x) x.MAD).Take(madPoolSize).ToList()
        ' === 步骤 2: 冗余剔除 ===
        Dim finalSelectedRows As New List(Of DataFrameRow)()

        For Each candidate In candidatePool
            ' 如果已经选够了 targetCount 个，提前结束
            If finalSelectedRows.Count >= targetManifests Then
                Exit For
            End If

            Dim isRedundant As Boolean = False
            Dim currentExpr = candidate.experiments

            ' 将当前候选变量与已选入的变量逐一比较相关性
            For Each selectedRow In finalSelectedRows
                Dim corr = Correlations.GetPearson(currentExpr, selectedRow.experiments)
                ' 如果相关系数绝对值大于阈值，认为冗余，舍弃
                If std.Abs(corr) > corrThreshold Then
                    isRedundant = True
                    Exit For
                End If
            Next

            ' 如果不与任何已选变量冗余，则加入最终列表
            If Not isRedundant Then
                finalSelectedRows.Add(candidate)
            End If
        Next

        ' === 步骤 3: 返回结果 ===
        ' 注意：如果候选池中变量高度相关，最终选出的数量可能少于5个。
        ' 此处根据之前的讨论，少于5个也是合理的（保留具有代表性的非冗余变量）。
        ' 如果您在数量不足时必须强行补齐5个，可以在这里添加后备逻辑：
        ' 例如放宽阈值重新筛选，或者直接按MAD顺位补齐。
        Return finalSelectedRows.Select(Function(r) r.geneID)
    End Function

End Class
