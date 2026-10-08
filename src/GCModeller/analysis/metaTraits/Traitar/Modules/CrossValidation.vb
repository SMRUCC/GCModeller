' ============================================================================
' CrossValidation.vb
'
' 交叉验证：直接委托给底层 LibSVM 算法库的 Training.PerformCrossValidation
' 完成。分类模型返回准确率，回归模型（EPSILON_SVR / NU_SVR）返回相关系数。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.SVM
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure

Namespace Traitar.Modules

    ''' <summary>
    ''' 交叉验证工具（LibSVM Training.PerformCrossValidation 的封装）
    ''' </summary>
    Public Module CrossValidation

        ''' <summary>
        ''' 对给定的 Problem 做 k 折交叉验证
        ''' </summary>
        ''' <param name="problem">已经过 RangeTransform 缩放的训练数据</param>
        ''' <param name="par">LibSVM 参数</param>
        ''' <param name="nrfold">折数，会自动限制为不超过样本数量</param>
        ''' <returns>
        ''' 分类模型返回准确率；回归模型返回预测值与真实值之间的相关系数；
        ''' 样本数量少于 2 时返回 <see cref="Double.NaN"/>
        ''' </returns>
        Public Function CrossValidate(problem As Problem, par As Parameter, Optional nrfold As Integer = 5) As Double
            If problem Is Nothing OrElse par Is Nothing Then
                Return Double.NaN
            End If

            Dim folds As Integer = nrfold

            If folds > problem.count Then
                folds = problem.count
            End If
            If folds < 2 Then
                Return Double.NaN
            End If

            Dim score As Double = Training.PerformCrossValidation(problem, par, folds)

            ' NaN 不是合法的 json 数值，交叉验证退化时统一返回 0
            Return If(Double.IsNaN(score), 0, score)
        End Function

        ''' <summary>
        ''' 直接从训练数据集之中取出某一个表型的样本做交叉验证
        ''' </summary>
        ''' <param name="dataset">训练数据集</param>
        ''' <param name="trait">目标表型</param>
        ''' <param name="nrfold">折数</param>
        ''' <param name="kernel">核函数类型</param>
        Public Function CrossValidateTrait(dataset As TraitTrainingSet,
                                           trait As PhenotypeTrait,
                                           Optional nrfold As Integer = 5,
                                           Optional kernel As KernelType = KernelType.RBF) As Double

            Dim topic As String = trait.trait_name
            Dim rows As SupportVector() = dataset.problems _
                .vectors _
                .Where(Function(v) v.labels.ContainsKey(topic)) _
                .ToArray

            If rows.Length < 2 Then
                Return Double.NaN
            End If

            Dim subTable As New ProblemTable With {
                .dimensionNames = dataset.problems.dimensionNames,
                .vectors = rows
            }
            Dim problem As Problem = subTable.GetProblem(topic)
            Dim par As Parameter = trait.CreateParameter(dataset.embedding.GetDimensionCount(), kernel)
            Dim transform As RangeTransform = RangeTransform.Compute(problem)
            Dim scaled As Problem = transform.Scale(problem)

            Return CrossValidate(scaled, par, nrfold)
        End Function

    End Module
End Namespace
