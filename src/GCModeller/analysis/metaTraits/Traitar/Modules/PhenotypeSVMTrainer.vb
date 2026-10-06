' ============================================================================
' PhenotypeSVMTrainer.vb
'
' 训练引擎：针对 phenotype_traits_and_types.csv 之中的每一种生物表型训练出
' 一个对应的 SVM 模型实例。全部的优化求解工作都委托给底层的 LibSVM 算法库
' （Training.Train / Training.PerformCrossValidation）完成，本模块不再自带
' 任何优化器代码。
'
'   boolean     -> C_SVC        二分类
'   categorical -> C_SVC        多分类
'   numeric     -> EPSILON_SVR  回归
'
' 注意：numeric 型表型不可以走 LibSVM.getSvmModel，因为其内部使用
'       ClassEncoder 会把数值标签重新编码为 0/1/2...，必须在这里手动组装
'       SVMModel，并且 transform 必须是一个 RangeTransform 实例。
' ============================================================================

Imports System.Diagnostics
Imports Microsoft.VisualBasic.DataMining.ComponentModel.Encoder
Imports Microsoft.VisualBasic.MachineLearning.SVM
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure

Namespace metaTraits.Traitar.Modules

    ''' <summary>
    ''' 网格搜索的作用范围
    ''' </summary>
    Public Enum TuneMode
        ''' <summary>不做网格搜索，全部使用默认参数</summary>
        None
        ''' <summary>
        ''' 只对数值型（回归）表型做网格搜索。
        ''' 分类型表型在部分网格点上 SMO 收敛极慢（实测单个表型可达数十秒），
        ''' 因此默认不对其做搜索
        ''' </summary>
        RegressionOnly
        ''' <summary>对全部表型做网格搜索，训练耗时会显著增加</summary>
        All
    End Enum

    ''' <summary>
    ''' 生物表型的 SVM 模型训练引擎
    ''' </summary>
    Public Class PhenotypeSVMTrainer

        ''' <summary>是否在训练过程之中输出日志</summary>
        Public Property verbose As Boolean = True
        ''' <summary>交叉验证的折数</summary>
        Public Property nrfold As Integer = 5
        ''' <summary>核函数类型，默认为 RBF</summary>
        Public Property kernel As KernelType = KernelType.RBF
        ''' <summary>
        ''' 是否在训练每一个表型模型之前先做 C / gamma 的网格搜索。
        ''' 默认为 <see cref="TuneMode.RegressionOnly"/>
        ''' </summary>
        Public Property autoTune As TuneMode = TuneMode.RegressionOnly
        ''' <summary>网格搜索内部使用的交叉验证折数</summary>
        Public Property tuneFolds As Integer = 5
        ''' <summary>网格搜索 C 的最小幂次（2^tuneMinC）</summary>
        Public Property tuneMinC As Double = -5
        ''' <summary>网格搜索 C 的最大幂次（2^tuneMaxC）</summary>
        Public Property tuneMaxC As Double = 7
        ''' <summary>网格搜索 C 的幂次步长</summary>
        Public Property tuneStepC As Double = 1
        ''' <summary>网格搜索 gamma 的最小幂次（2^tuneMinG）</summary>
        Public Property tuneMinG As Double = -11
        ''' <summary>网格搜索 gamma 的最大幂次（2^tuneMaxG）</summary>
        Public Property tuneMaxG As Double = 1
        ''' <summary>网格搜索 gamma 的幂次步长</summary>
        Public Property tuneStepG As Double = 1

        ''' <summary>
        ''' 针对训练数据集之中的全部表型逐一训练模型
        ''' </summary>
        ''' <param name="dataset">由 TraitProblemBuilder 装配出来的训练数据集</param>
        ''' <returns>表型名 -> 该表型所对应的 SVM 模型实例</returns>
        Public Function TrainAll(dataset As TraitTrainingSet) As Dictionary(Of String, Models.PhenotypeModel)
            Dim models As New Dictionary(Of String, Models.PhenotypeModel)

            If dataset Is Nothing OrElse dataset.traits Is Nothing Then
                Return models
            End If

            Dim dims As Integer = If(dataset.embedding Is Nothing, 0, dataset.embedding.GetDimensionCount())
            Dim total As Integer = dataset.traits.Length
            Dim i As Integer = 0

            For Each trait As PhenotypeTrait In dataset.traits
                i += 1

                Dim clock As Stopwatch = Stopwatch.StartNew()
                Dim model As Models.PhenotypeModel = TrainOne(dataset, trait, dims)

                Call clock.Stop()

                models(trait.trait_name) = model

                If verbose Then
                    If model.IsTrained() Then
                        Console.WriteLine($"[{i}/{total}] {model} ({clock.ElapsedMilliseconds}ms)")
                    Else
                        Console.WriteLine($"[{i}/{total}] skip '{trait.trait_name}': {model.ErrorMessage}")
                    End If
                End If
            Next

            Return models
        End Function

        ''' <summary>
        ''' 训练单个表型所对应的 SVM 模型实例
        ''' </summary>
        ''' <param name="dataset">训练数据集</param>
        ''' <param name="trait">目标表型的元数据</param>
        ''' <param name="dims">特征维度数量，用于计算 RBF 核的 gamma 参数</param>
        ''' <returns>
        ''' 训练失败或者样本不足时返回 Status = ``skipped`` 的模型对象，
        ''' 不会抛出异常
        ''' </returns>
        Public Function TrainOne(dataset As TraitTrainingSet, trait As PhenotypeTrait, Optional dims As Integer = 0) As Models.PhenotypeModel
            Dim topic As String = trait.trait_name
            Dim result As New Models.PhenotypeModel With {
                .Trait = trait,
                .Status = "skipped"
            }

            If dataset Is Nothing OrElse dataset.problems Is Nothing OrElse dataset.problems.vectors Is Nothing Then
                result.ErrorMessage = "no training data"
                Return result
            End If
            If dims <= 0 AndAlso dataset.embedding IsNot Nothing Then
                dims = dataset.embedding.GetDimensionCount()
            End If

            ' 只保留在该表型上面具备有效标签的样本
            Dim rows As SupportVector() = dataset.problems _
                .vectors _
                .Where(Function(v) v.labels.ContainsKey(topic)) _
                .ToArray

            result.SampleCount = rows.Length

            If rows.Length < 2 Then
                result.ErrorMessage = $"insufficient labeled samples ({rows.Length})"
                Return result
            End If

            Try
                If trait.GetDataType() = TraitDataType.Unknown Then
                    result.ErrorMessage = $"unknown trait data type '{trait.data_type}'"
                    Return result
                End If

                Dim subTable As New ProblemTable With {
                    .dimensionNames = dataset.problems.dimensionNames,
                    .vectors = rows
                }
                Dim problem As Problem = subTable.GetProblem(topic)

                If trait.IsRegression() Then
                    ' 用真实的数值替换掉 ClassEncoder 所产生的顺序编码，
                    ' 否则 SVR 会把标签当成类别序号来学习
                    Dim y As ColorClass() = New ColorClass(rows.Length - 1) {}

                    For k As Integer = 0 To rows.Length - 1
                        Dim raw As String = rows(k).labels(topic)
                        Dim value As Double = Double.Parse(raw)

                        y(k) = New ColorClass With {
                            .factor = value,
                            .name = raw,
                            .color = "#000000"
                        }
                    Next

                    problem.Y = y
                Else
                    Dim classes As String() = rows _
                        .Select(Function(v) v.labels(topic)) _
                        .Distinct _
                        .ToArray

                    If classes.Length < 2 Then
                        result.ErrorMessage = $"only one class label '{classes(0)}'"
                        Return result
                    End If
                End If

                Dim par As Parameter = trait.CreateParameter(dims, kernel)

                If Not trait.IsRegression() Then
                    ' 显式登记各个类别的惩罚权重（均为 1），避免 LibSVM 输出
                    ' "class label x specified in weight is not found" 的警告
                    For Each factor As Integer In problem.Y _
                        .Select(Function(c) CInt(c.factor)) _
                        .Distinct

                        par.weights(factor) = 1.0
                    Next
                End If

                Dim transform As RangeTransform = RangeTransform.Compute(problem)
                Dim scaled As Problem = transform.Scale(problem)

                Dim doTune As Boolean = kernel = KernelType.RBF AndAlso
                    (autoTune = TuneMode.All OrElse
                    (autoTune = TuneMode.RegressionOnly AndAlso trait.IsRegression()))

                If doTune Then
                    ' 在缩放之后的数据之上做网格搜索，保证与正式训练的数据分布一致
                    Dim tuned As ParameterSearchResult = ParameterSearch.Search(
                        scaled, par,
                        nrfold:=If(tuneFolds < rows.Length, tuneFolds, rows.Length),
                        minC:=tuneMinC, maxC:=tuneMaxC, stepC:=tuneStepC,
                        minG:=tuneMinG, maxG:=tuneMaxG, stepG:=tuneStepG)

                    If tuned IsNot Nothing Then
                        par.c = tuned.C
                        par.gamma = tuned.Gamma
                    End If
                End If

                Dim trained As Microsoft.VisualBasic.MachineLearning.SVM.Model = Training.Train(scaled, par)
                Dim svm As SVMModel

                If trait.IsRegression() Then
                    svm = New SVMModel With {
                        .model = trained,
                        .transform = transform,
                        .factors = New ClassEncoder()
                    }
                Else
                    svm = New SVMModel With {
                        .model = trained,
                        .transform = transform,
                        .factors = New ClassEncoder(scaled.Y)
                    }
                End If

                result.Model = svm
                result.Status = "trained"
                result.C = par.c
                result.gamma = par.gamma

                ' 交叉验证：分类返回准确率，回归返回相关系数
                Dim folds As Integer = nrfold

                If folds > rows.Length Then
                    folds = rows.Length
                End If

                If folds >= 2 Then
                    Try
                        Dim cv As Double = Training.PerformCrossValidation(scaled, par, folds)

                        ' NaN 不是合法的 json 数值（交叉验证得到退化的相关系数时会出现），
                        ' 这里统一写为 0 表示无法给出有效的交叉验证得分
                        result.CVScore = If(Double.IsNaN(cv), 0, cv)
                    Catch ex As Exception
                        result.CVScore = 0
                    End Try
                End If
            Catch ex As Exception
                result.Model = Nothing
                result.Status = "skipped"
                result.ErrorMessage = ex.Message
            End Try

            Return result
        End Function

    End Class
End Namespace
