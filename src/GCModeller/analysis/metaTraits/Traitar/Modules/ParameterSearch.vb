' ============================================================================
' ParameterSearch.vb
'
' 超参数网格搜索：基于底层 LibSVM 的 ParameterSelection.Grid，在 C 与 gamma
' 两个维度上做对数网格搜索（2 的幂），以交叉验证得分最优者为最终参数。
'
'   - 分类模型（C_SVC）：交叉验证得分为准确率，越大越好
'   - 回归模型（EPSILON_SVR）：交叉验证得分为相关系数，越大越好
'
' 网格搜索只对 C 与 gamma 生效，其余参数（svmType / kernelType / P 等）
' 由传入的 Parameter 模板决定。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.SVM
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure

Namespace metaTraits.Traitar.Modules

    ''' <summary>
    ''' 网格搜索的结果
    ''' </summary>
    Public Class ParameterSearchResult

        ''' <summary>搜索得到的最优 C 值</summary>
        Public Property C As Double
        ''' <summary>搜索得到的最优 gamma 值</summary>
        Public Property Gamma As Double
        ''' <summary>最优参数所对应的交叉验证得分</summary>
        Public Property Score As Double
        ''' <summary>网格的规模（组合数量）</summary>
        Public Property GridSize As Integer
        ''' <summary>全部网格点及其得分</summary>
        Public Property Squares As GridSquare()

        Public Overrides Function ToString() As String
            Return $"C={C:G4}, gamma={Gamma:G4}, cv={Score:F4} ({GridSize} grid points)"
        End Function

    End Class

    ''' <summary>
    ''' SVM 超参数（C / gamma）网格搜索
    ''' </summary>
    Public Module ParameterSearch

        ''' <summary>
        ''' 在 C 与 gamma 的 2 的幂网格之上搜索最优参数
        ''' </summary>
        ''' <param name="problem">已经过 RangeTransform 缩放的训练数据</param>
        ''' <param name="template">
        ''' 参数模板：svmType / kernelType / P / weights 等会被保留，
        ''' 只有 C 与 gamma 会被网格搜索覆盖
        ''' </param>
        ''' <param name="nrfold">网格搜索内部使用的交叉验证折数，会限制为不超过样本数</param>
        ''' <param name="minC">C 的最小幂次（2^minC）</param>
        ''' <param name="maxC">C 的最大幂次（2^maxC）</param>
        ''' <param name="stepC">C 的幂次步长</param>
        ''' <param name="minG">gamma 的最小幂次（2^minG）</param>
        ''' <param name="maxG">gamma 的最大幂次（2^maxG）</param>
        ''' <param name="stepG">gamma 的幂次步长</param>
        ''' <param name="report">每一个网格点完成之后的回调，可用于输出进度</param>
        ''' <param name="threads">并行度，小于等于 0 时使用 CPU 核心数</param>
        Public Function Search(problem As Problem,
                               template As Parameter,
                               Optional nrfold As Integer = 5,
                               Optional minC As Double = -5,
                               Optional maxC As Double = 7,
                               Optional stepC As Double = 2,
                               Optional minG As Double = -11,
                               Optional maxG As Double = 1,
                               Optional stepG As Double = 2,
                               Optional report As Action(Of GridSquare) = Nothing,
                               Optional threads As Integer = 0) As ParameterSearchResult

            If problem Is Nothing OrElse template Is Nothing Then
                Return Nothing
            End If

            Dim folds As Integer = nrfold

            If folds > problem.count Then
                folds = problem.count
            End If
            If folds < 2 Then
                Return Nothing
            End If

            Dim factory As Func(Of Parameter) = Function() DirectCast(template.Clone(), Parameter)
            Dim bestC As Double = 0
            Dim bestGamma As Double = 0

            If threads > 0 Then
                ParameterSelection.Threads = threads
            End If

            Dim squares As List(Of GridSquare) = ParameterSelection.Grid(
                problem,
                factory,
                ParameterSelection.GetList(minC, maxC, stepC),
                ParameterSelection.GetList(minG, maxG, stepG),
                report,
                folds,
                bestC,
                bestGamma
            )

            Dim score As Double = 0

            For Each square As GridSquare In squares
                If square.C = bestC AndAlso square.Gamma = bestGamma Then
                    score = square.Score
                    Exit For
                End If
            Next

            ' NaN 不是合法的 json 数值：全部网格点都退化（如标签方差为 0）时统一写 0
            Return New ParameterSearchResult With {
                .C = bestC,
                .Gamma = bestGamma,
                .Score = If(Double.IsNaN(score), 0, score),
                .GridSize = squares.Count,
                .Squares = squares.ToArray
            }
        End Function

        ''' <summary>
        ''' 针对训练数据集之中的某一个表型做超参数网格搜索
        ''' </summary>
        ''' <param name="dataset">训练数据集</param>
        ''' <param name="trait">目标表型</param>
        ''' <param name="kernel">核函数类型，网格搜索只对 RBF 核有意义</param>
        ''' <param name="nrfold">交叉验证折数</param>
        Public Function SearchTrait(dataset As TraitTrainingSet,
                                    trait As PhenotypeTrait,
                                    Optional kernel As KernelType = KernelType.RBF,
                                    Optional nrfold As Integer = 5,
                                    Optional minC As Double = -5,
                                    Optional maxC As Double = 7,
                                    Optional stepC As Double = 2,
                                    Optional minG As Double = -11,
                                    Optional maxG As Double = 1,
                                    Optional stepG As Double = 2,
                                    Optional report As Action(Of GridSquare) = Nothing,
                                    Optional threads As Integer = 0) As ParameterSearchResult

            If dataset Is Nothing OrElse dataset.problems Is Nothing OrElse dataset.embedding Is Nothing Then
                Return Nothing
            End If

            Dim topic As String = trait.trait_name
            Dim rows As SupportVector() = dataset.problems _
                .vectors _
                .Where(Function(v) v.labels.ContainsKey(topic)) _
                .ToArray

            If rows.Length < 2 Then
                Return Nothing
            End If

            Dim dims As Integer = dataset.embedding.GetDimensionCount()
            Dim subTable As New ProblemTable With {
                .dimensionNames = dataset.problems.dimensionNames,
                .vectors = rows
            }
            Dim problem As Problem = subTable.GetProblem(topic)

            If trait.IsRegression() Then
                Dim y As Microsoft.VisualBasic.DataMining.ComponentModel.Encoder.ColorClass() =
                    New Microsoft.VisualBasic.DataMining.ComponentModel.Encoder.ColorClass(rows.Length - 1) {}

                For k As Integer = 0 To rows.Length - 1
                    Dim raw As String = rows(k).labels(topic)

                    y(k) = New Microsoft.VisualBasic.DataMining.ComponentModel.Encoder.ColorClass With {
                        .factor = Double.Parse(raw),
                        .name = raw,
                        .color = "#000000"
                    }
                Next

                problem.Y = y
            End If

            Dim transform As RangeTransform = RangeTransform.Compute(problem)
            Dim scaled As Problem = transform.Scale(problem)
            Dim template As Parameter = trait.CreateParameter(dims, kernel)

            Return Search(scaled, template, nrfold, minC, maxC, stepC, minG, maxG, stepG, report, threads)
        End Function

    End Module
End Namespace
