#Region "Microsoft.VisualBasic::f3c68c65e73bb125467a17377d2abb14, R#\metagenomics_kit\metaTraits.vb"

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
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 108
'    Code Lines: 89 (82.41%)
' Comment Lines: 2 (1.85%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 17 (15.74%)
'     File Size: 5.44 KB


' Module metaTraitsTool
' 
'     Function: load_metatraits, load_traitModels, make_predicts, phenotype_features, result_table
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.SVM
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits.Traitar
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits.Traitar.Models
Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits.Traitar.Modules
Imports SMRUCC.genomics.Data.Xfam.Pfam.PfamString
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' toolkit for the microbial phenotype (Traitar) svm models
''' </summary>
''' <remarks>
''' 该工具集把生物表型的 SVM 训练与预测管线暴露给 R# 环境：
''' 
''' 1. 通过 R# 函数 ``load.phenotype_traits`` 读取表型定义表，
'''    每一种表型按照其数据类型（boolean / categorical / numeric）对应一个
'''    SVM 模型实例（C_SVC 或者 EPSILON_SVR）；
''' 2. 通过 R# 函数 ``pfam.embedding`` 把各微生物基因组所预测的蛋白质组
'''    Pfam 结构域组成嵌入为 per-genome 归一化的特征向量；
''' 3. 通过 R# 函数 ``train.phenotype_models`` 训练出全部表型的模型，
'''    并通过 ``save.trait_models`` 落盘为 json 模型仓库；
''' 4. 通过 R# 函数 ``make_predicts`` 使用模型仓库预测新基因组的全部表型。
''' </remarks>
<Package("metaTraits", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
<RTypeExport("phenotype_trait", GetType(PhenotypeTrait))>
<RTypeExport("trait_annotation", GetType(TraitAnnotation))>
<RTypeExport("pfam_embedding", GetType(PfamEmbedding))>
<RTypeExport("trait_training_set", GetType(TraitTrainingSet))>
<RTypeExport("trait_models", GetType(ModelLoader))>
<RTypeExport("trait_prediction", GetType(TraitPrediction))>
<RTypeExport("trait_report", GetType(ReportJSON))>
Module metaTraitsTool

    Public Sub Main()
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(ReportJSON()), AddressOf result_table)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(TraitPrediction()), AddressOf prediction_table)
        Call RInternal.ConsolePrinter.AttachConsoleFormatter(Of ModelLoader)(AddressOf printModelRepo)
        Call RInternal.ConsolePrinter.AttachConsoleFormatter(Of PfamEmbedding)(AddressOf printEmbedding)
    End Sub

    Private Function printModelRepo(models As ModelLoader) As String
        Return models.ToString
    End Function

    Private Function printEmbedding(embedding As PfamEmbedding) As String
        Return embedding.ToString
    End Function

    <RGenericOverloads("as.data.frame")>
    Public Function result_table(result As ReportJSON(), args As list, env As Environment) As rdataframe
        Dim df As New rdataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = result.Select(Function(a) a.phenotypeId).ToArray
        }

        Call df.add(NameOf(ReportJSON.accession), From r As ReportJSON In result Select r.accession)
        Call df.add(NameOf(ReportJSON.category), From r As ReportJSON In result Select r.category)
        Call df.add(NameOf(ReportJSON.data_type), From r As ReportJSON In result Select r.data_type)
        Call df.add(NameOf(ReportJSON.unit), From r As ReportJSON In result Select r.unit)
        Call df.add(NameOf(ReportJSON.predict), From r As ReportJSON In result Select r.predict)
        Call df.add(NameOf(ReportJSON.confidence), From r As ReportJSON In result Select r.confidence)
        Call df.add(NameOf(ReportJSON.score), From r As ReportJSON In result Select r.score)
        Call df.add(NameOf(ReportJSON.status), From r As ReportJSON In result Select r.status)
        Call df.add(NameOf(ReportJSON.cvScore), From r As ReportJSON In result Select r.cvScore)
        Call df.add(NameOf(ReportJSON.sampleCount), From r As ReportJSON In result Select r.sampleCount)
        Call df.add(NameOf(ReportJSON.votes), From r As ReportJSON In result Select r.votes.SafeQuery.JoinBy("; "))
        Call df.add("keys", From r As ReportJSON
                            In result
                            Select r.KeyFeatures _
                                .SafeQuery _
                                .Select(Function(k) k.PfamId) _
                                .JoinBy(", "))
        Return df
    End Function

    <RGenericOverloads("as.data.frame")>
    Public Function prediction_table(predicts As TraitPrediction(), args As list, env As Environment) As rdataframe
        Dim df As New rdataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = predicts.Select(Function(a) a.trait_name).ToArray
        }

        Call df.add(NameOf(TraitPrediction.data_type), From r In predicts Select r.data_type)
        Call df.add(NameOf(TraitPrediction.unit), From r In predicts Select r.unit)
        Call df.add(NameOf(TraitPrediction.group_1), From r In predicts Select r.group_1)
        Call df.add(NameOf(TraitPrediction.group_2), From r In predicts Select r.group_2)
        Call df.add(NameOf(TraitPrediction.predict), From r In predicts Select r.predict)
        Call df.add(NameOf(TraitPrediction.value), From r In predicts Select r.value)
        Call df.add(NameOf(TraitPrediction.score), From r In predicts Select r.score)
        Call df.add(NameOf(TraitPrediction.confidence), From r In predicts Select r.confidence)
        Call df.add(NameOf(TraitPrediction.status), From r In predicts Select r.status)

        Return df
    End Function

    ''' <summary>
    ''' 读取表型标注数据并按物种组织为表型谱
    ''' </summary>
    ''' <param name="file">ncbi/gtdb species summary tsv 表文件</param>
    <ExportAPI("load.meta_traits")>
    <RApiReturn(GetType(metaTraitData))>
    Public Function load_metatraits(file As String, Optional env As Environment = Nothing) As Object
        Return TraitAnnotation.CreateProfiles(TraitAnnotation.ParseTable(file)).ToArray
    End Function

    ''' <summary>
    ''' 读取表型标注的原始表格（每一行是一个物种的一种表型标注）
    ''' </summary>
    ''' <param name="file">ncbi/gtdb species summary tsv 表文件</param>
    <ExportAPI("load.trait_annotations")>
    <RApiReturn(GetType(TraitAnnotation))>
    Public Function load_trait_annotations(<RRawVectorArgument(TypeCodes.string)> file As Object, Optional env As Environment = Nothing) As Object
        Dim list As New List(Of TraitAnnotation)

        For Each path As String In CLRVector.asCharacter(file)
            Call list.AddRange(TraitAnnotation.ParseTable(path))
        Next

        Return list.ToArray
    End Function

    ''' <summary>
    ''' 读取表型定义表，每一种表型对应一个 SVM 模型实例
    ''' </summary>
    ''' <param name="file">
    ''' phenotype_traits_and_types.csv，其中定义了每一种表型的数据类型
    ''' （boolean / categorical / numeric (continuous)）
    ''' </param>
    <ExportAPI("load.phenotype_traits")>
    <RApiReturn(GetType(PhenotypeTrait))>
    Public Function load_phenotype_traits(file As String, Optional env As Environment = Nothing) As Object
        Return PhenotypeTraits.LoadTable(file)
    End Function

    ''' <summary>
    ''' 读取一个目录之下的全部微生物基因组的蛋白质组 Pfam 结构域注释
    ''' </summary>
    ''' <param name="dir">
    ''' 每一个子目录是一个微生物基因组，目录名即物种名，
    ''' 其中包含一个 Pfam.csv 文件
    ''' </param>
    ''' <returns>一个以基因组名为键的 list，每一个元素是该基因组的蛋白质组 Pfam 注释</returns>
    <ExportAPI("read.pfam_proteomes")>
    <RApiReturn(GetType(PfamString))>
    Public Function read_pfam_proteomes(dir As String, Optional env As Environment = Nothing) As Object
        If Not Directory.Exists(dir) Then
            Return RInternal.debug.stop($"the pfam proteome directory '{dir}' is not exists!", env)
        End If

        Dim pfams As Dictionary(Of String, PfamString()) = LoadPfamSets(dir)
        Dim out As New list With {.slots = New Dictionary(Of String, Object)}

        For Each genome As KeyValuePair(Of String, PfamString()) In pfams
            Call out.add(genome.Key, CObj(genome.Value))
        Next

        Return out
    End Function

    ''' <summary>
    ''' 构建 Pfam 结构域的嵌入配置（词表 + 编码方式）
    ''' </summary>
    ''' <param name="pfams">
    ''' 可以是 R# 函数 ``read.pfam_proteomes`` 的输出，也可以直接是
    ''' 一个包含各微生物基因组 Pfam.csv 的目录路径
    ''' </param>
    ''' <param name="minGenomes">
    ''' 词表剪枝阈值：Pfam 结构域至少要在这么多个基因组之中出现才会被保留，默认为 1（不剪枝）
    ''' </param>
    ''' <param name="encoding">
    ''' 编码方式：``normalized`` 表示按基因组内的最大出现次数做归一化（默认），
    ''' ``binary`` 表示退化为 0/1 的 one-hot 编码
    ''' </param>
    <ExportAPI("pfam.embedding")>
    <RApiReturn(GetType(PfamEmbedding))>
    Public Function pfam_embedding(<RRawVectorArgument> pfams As Object,
                                   Optional minGenomes As Integer = 1,
                                   Optional encoding As String = "normalized",
                                   Optional env As Environment = Nothing) As Object

        Dim sets As Dictionary(Of String, PfamString()) = tryGetPfamSets(pfams, env)

        If sets Is Nothing Then
            Return Nothing
        End If

        Return TraitProblemBuilder.CreateEmbedding(sets, minGenomes, getEncoding(encoding))
    End Function

    ''' <summary>
    ''' 装配表型的训练数据集
    ''' </summary>
    ''' <param name="traits">表型定义表</param>
    ''' <param name="annotations">表型标注数据</param>
    ''' <param name="pfams">各微生物基因组的蛋白质组 Pfam 注释（list 或者目录路径）</param>
    ''' <param name="embedding">Pfam 嵌入配置</param>
    ''' <returns>
    ''' 行 = 微生物基因组样本，topic = 表型名的训练数据集
    ''' </returns>
    <ExportAPI("phenotype.problem")>
    <RApiReturn(GetType(TraitTrainingSet))>
    Public Function phenotype_problem(<RRawVectorArgument> traits As Object,
                                      <RRawVectorArgument> annotations As Object,
                                      <RRawVectorArgument> pfams As Object,
                                      embedding As PfamEmbedding,
                                      Optional env As Environment = Nothing) As Object

        Dim traitList As PhenotypeTrait() = getTraits(traits, env)
        Dim annoList As TraitAnnotation() = getAnnotations(annotations, env)
        Dim sets As Dictionary(Of String, PfamString()) = tryGetPfamSets(pfams, env)

        If traitList Is Nothing OrElse annoList Is Nothing OrElse sets Is Nothing Then
            Return Nothing
        End If
        If embedding Is Nothing Then
            Return RInternal.debug.stop("the required pfam embedding can not be nothing!", env)
        End If

        Return TraitProblemBuilder.Build(annoList, sets, traitList, embedding)
    End Function

    ''' <summary>
    ''' 训练全部表型所对应的 SVM 模型
    ''' </summary>
    ''' <param name="dataset">由 <see cref="phenotype_problem"/> 装配出来的训练数据集</param>
    ''' <param name="kernel">核函数类型：rbf（默认）/ linear / poly / sigmoid</param>
    ''' <param name="nrfold">交叉验证的折数</param>
    ''' <param name="tune">
    ''' 网格搜索的作用范围：``regression`` 表示只对数值型表型做搜索（默认，开销很小），
    ''' ``none`` 表示全部使用默认参数，``all`` 表示对全部表型做搜索（分类型表型
    ''' 在部分网格点上收敛极慢，耗时可能达到分钟级）
    ''' </param>
    ''' <param name="verbose">是否输出训练日志</param>
    <ExportAPI("train.phenotype_models")>
    <RApiReturn(GetType(ModelLoader))>
    Public Function train_phenotype_models(dataset As TraitTrainingSet,
                                           Optional kernel As String = "rbf",
                                           Optional nrfold As Integer = 5,
                                           Optional tune As String = "regression",
                                           Optional verbose As Boolean = True,
                                           Optional env As Environment = Nothing) As Object

        If dataset Is Nothing Then
            Return RInternal.debug.stop("the required training dataset can not be nothing!", env)
        End If

        Dim trainer As New PhenotypeSVMTrainer With {
            .verbose = verbose,
            .nrfold = nrfold,
            .kernel = getKernel(kernel, env),
            .autoTune = getTuneMode(tune)
        }

        Dim models As Dictionary(Of String, PhenotypeModel) = trainer.TrainAll(dataset)

        Return New ModelLoader With {
            .Models = models,
            .Embedding = dataset.embedding,
            .Directory = Nothing
        }
    End Function

    ''' <summary>
    ''' 把训练得到的表型模型仓库保存到指定目录
    ''' </summary>
    ''' <param name="models">模型仓库</param>
    ''' <param name="dir">输出目录，每一种表型会被写为一个独立的 json 文件</param>
    ''' <param name="sampleCount">训练集的样本数量</param>
    <ExportAPI("save.trait_models")>
    <RApiReturn(GetType(ModelLoader))>
    Public Function save_trait_models(models As ModelLoader, dir As String,
                                      Optional sampleCount As Integer = 0,
                                      Optional verbose As Boolean = True,
                                      Optional env As Environment = Nothing) As Object

        If models Is Nothing Then
            Return RInternal.debug.stop("the required model repository can not be nothing!", env)
        End If

        Return ModelLoader.SaveDirectory(models.Models, models.Embedding, dir, sampleCount, verbose)
    End Function

    ''' <summary>
    ''' 从 json 模型仓库目录之中加载全部表型的 SVM 模型
    ''' </summary>
    ''' <param name="repo">模型仓库目录</param>
    <ExportAPI("load.trait_models")>
    <RApiReturn(GetType(ModelLoader))>
    Public Function load_traitModels(repo As String, Optional env As Environment = Nothing) As Object
        If Not Directory.Exists(repo) Then
            Return RInternal.debug.stop($"the trait model repository '{repo}' is not exists!", env)
        End If

        Return ModelLoader.LoadDirectory(repo)
    End Function

    ''' <summary>
    ''' 使用表型模型仓库预测基因组的全部表型
    ''' </summary>
    ''' <param name="models">模型仓库</param>
    ''' <param name="pfams">
    ''' 待预测基因组的蛋白质组 Pfam 注释：
    ''' 传入单个基因组的 PfamString 向量时返回该基因组的预测结果；
    ''' 传入 list（R# 函数 ``read.pfam_proteomes`` 的输出）或者目录路径时，
    ''' 返回以基因组名为键的预测结果列表
    ''' </param>
    <ExportAPI("make_predicts")>
    <RApiReturn(GetType(TraitPrediction))>
    Public Function make_predicts(models As ModelLoader,
                                  <RRawVectorArgument> pfams As Object,
                                  Optional env As Environment = Nothing) As Object

        If models Is Nothing Then
            Return RInternal.debug.stop("the required model repository can not be nothing!", env)
        End If
        If models.Embedding Is Nothing Then
            Return RInternal.debug.stop("the pfam embedding of the given model repository is missing!", env)
        End If

        Dim predictor As New PhenotypePredictor(models)

        If TypeOf pfams Is String Then
            Dim dir As String = DirectCast(pfams, String)

            If Not Directory.Exists(dir) Then
                Return RInternal.debug.stop($"the pfam proteome directory '{dir}' is not exists!", env)
            End If

            Return predictAll(predictor, LoadPfamSets(dir))
        ElseIf TypeOf pfams Is list Then
            Dim slots As list = DirectCast(pfams, list)
            Dim sets As New Dictionary(Of String, PfamString())

            For Each name As String In slots.getNames
                Dim proteome As PfamString() = tryGetProteome(slots.getByName(name), env)

                If proteome Is Nothing Then
                    Return Nothing
                End If

                sets(name) = proteome
            Next

            Return predictAll(predictor, sets)
        Else
            Dim proteome As PfamString() = tryGetProteome(pfams, env)

            If proteome Is Nothing Then
                Return Nothing
            End If

            Return predictor.PredictGenome(proteome).ToArray
        End If
    End Function

    Private Function predictAll(predictor As PhenotypePredictor, sets As Dictionary(Of String, PfamString())) As Object
        Dim out As New list With {.slots = New Dictionary(Of String, Object)}

        For Each genome As KeyValuePair(Of String, PfamString()) In sets
            Call out.add(genome.Key, CObj(predictor.PredictGenome(genome.Value).ToArray))
        Next

        Return out
    End Function

    ''' <summary>
    ''' 把预测结果合并上模型的元数据，生成最终的表型预测报告
    ''' </summary>
    ''' <param name="predicts">预测结果</param>
    ''' <param name="models">模型仓库</param>
    ''' <param name="dataset">
    ''' 训练数据集，提供该参数时会额外计算每一个表型的关键 Pfam 结构域特征
    ''' </param>
    ''' <param name="topN">关键特征的数量上限</param>
    <ExportAPI("phenotype_result")>
    <RApiReturn(GetType(ReportJSON))>
    Public Function phenotype_result(<RRawVectorArgument> predicts As Object,
                                     models As ModelLoader,
                                     <RRawVectorArgument> Optional dataset As Object = Nothing,
                                     Optional topN As Integer = 20,
                                     Optional env As Environment = Nothing) As Object

        Dim predictions As TraitPrediction() = getPredictions(predicts, env)

        If predictions Is Nothing Then
            Return Nothing
        End If
        If models Is Nothing Then
            Return RInternal.debug.stop("the required model repository can not be nothing!", env)
        End If

        If dataset IsNot Nothing AndAlso TypeOf dataset Is TraitTrainingSet Then
            Dim training As TraitTrainingSet = DirectCast(dataset, TraitTrainingSet)
            Dim selector As New FeatureSelection(models)

            For Each pred As TraitPrediction In predictions
                Dim model As PhenotypeModel = models.GetModel(pred.trait_name)

                If model IsNot Nothing AndAlso model.IsTrained() Then
                    model.KeyFeatures = selector _
                        .SelectKeyFeatures(model, training, topN) _
                        .ToArray
                End If
            Next
        End If

        Return predictions.ResultTable(models).ToArray
    End Function

    ''' <summary>
    ''' 对单个表型做超参数（C / gamma）的网格搜索
    ''' </summary>
    ''' <param name="dataset">训练数据集</param>
    ''' <param name="trait">目标表型</param>
    ''' <param name="nrfold">交叉验证折数</param>
    <ExportAPI("tune.trait_model")>
    <RApiReturn(GetType(ParameterSearchResult))>
    Public Function tune_trait_model(dataset As TraitTrainingSet,
                                     trait As PhenotypeTrait,
                                     Optional kernel As String = "rbf",
                                     Optional nrfold As Integer = 5,
                                     Optional env As Environment = Nothing) As Object

        If dataset Is Nothing OrElse trait Is Nothing Then
            Return RInternal.debug.stop("the required training dataset and trait can not be nothing!", env)
        End If

        Return ParameterSearch.SearchTrait(dataset, trait, getKernel(kernel, env), nrfold)
    End Function

    ' ------------------------------------------------------------
    ' 参数与数据转换工具
    ' ------------------------------------------------------------

    Private Function getEncoding(name As String) As PfamEncoding
        If name Is Nothing Then
            Return PfamEncoding.NormalizedCount
        End If

        Select Case name.ToLower
            Case "binary", "onehot", "one-hot", "0/1"
                Return PfamEncoding.Binary
            Case Else
                Return PfamEncoding.NormalizedCount
        End Select
    End Function

    Private Function getKernel(name As String, env As Environment) As KernelType
        If name Is Nothing Then
            Return KernelType.RBF
        End If

        Select Case name.ToLower
            Case "linear" : Return KernelType.LINEAR
            Case "poly", "polynomial" : Return KernelType.POLY
            Case "sigmoid" : Return KernelType.SIGMOID
            Case "rbf", "radial" : Return KernelType.RBF
            Case Else
                If env IsNot Nothing Then
                    Call env.AddMessage($"unknown kernel type '{name}', use the RBF kernel as default.")
                End If

                Return KernelType.RBF
        End Select
    End Function

    Private Function getTuneMode(name As String) As TuneMode
        If name Is Nothing Then
            Return TuneMode.RegressionOnly
        End If

        Select Case name.ToLower
            Case "none", "false", "off" : Return TuneMode.None
            Case "all", "true", "on" : Return TuneMode.All
            Case Else : Return TuneMode.RegressionOnly
        End Select
    End Function

    Private Function LoadPfamSets(dir As String) As Dictionary(Of String, PfamString())
        Return dir _
            .ListDirectory _
            .ToDictionary(Function(d) d.BaseName,
                          Function(d)
                              Return $"{d}/Pfam.csv".LoadCsv(Of PfamString)(mute:=True).ToArray
                          End Function)
    End Function

    ''' <summary>
    ''' 把 R# 端传入的 pfam 数据统一转换为 基因组名 -> 蛋白质组注释 的字典
    ''' </summary>
    Private Function tryGetPfamSets(pfams As Object, env As Environment) As Dictionary(Of String, PfamString())
        If pfams Is Nothing Then
            If env IsNot Nothing Then
                Call RInternal.debug.stop("the required pfam proteome data can not be nothing!", env)
            End If

            Return Nothing
        End If

        If TypeOf pfams Is String Then
            Dim dir As String = DirectCast(pfams, String)

            If Not Directory.Exists(dir) Then
                Call RInternal.debug.stop($"the pfam proteome directory '{dir}' is not exists!", env)
                Return Nothing
            End If

            Return LoadPfamSets(dir)
        ElseIf TypeOf pfams Is list Then
            Dim slots As list = DirectCast(pfams, list)
            Dim sets As New Dictionary(Of String, PfamString())

            For Each name As String In slots.getNames
                Dim proteome As PfamString() = tryGetProteome(slots.getByName(name), env)

                If proteome Is Nothing Then
                    Return Nothing
                End If

                sets(name) = proteome
            Next

            Return sets
        Else
            If env IsNot Nothing Then
                Call RInternal.debug.stop("the pfam data should be a directory path or a list of proteomes!", env)
            End If

            Return Nothing
        End If
    End Function

    Private Function tryGetProteome(data As Object, env As Environment) As PfamString()
        Dim stream As pipeline = pipeline.TryCreatePipeline(Of PfamString)(data, env)

        If stream.isError Then
            If env IsNot Nothing Then
                Call env.AddMessage(stream.getError.ToString)
            End If

            Return Nothing
        End If

        Return stream.populates(Of PfamString)(env).ToArray
    End Function

    Private Function getTraits(data As Object, env As Environment) As PhenotypeTrait()
        Dim stream As pipeline = pipeline.TryCreatePipeline(Of PhenotypeTrait)(data, env)

        If stream.isError Then
            If env IsNot Nothing Then
                Call env.AddMessage(stream.getError.ToString)
            End If

            Return Nothing
        End If

        Return stream.populates(Of PhenotypeTrait)(env).ToArray
    End Function

    Private Function getAnnotations(data As Object, env As Environment) As TraitAnnotation()
        Dim stream As pipeline = pipeline.TryCreatePipeline(Of TraitAnnotation)(data, env)

        If stream.isError Then
            If env IsNot Nothing Then
                Call env.AddMessage(stream.getError.ToString)
            End If

            Return Nothing
        End If

        Return stream.populates(Of TraitAnnotation)(env).ToArray
    End Function

    Private Function getPredictions(data As Object, env As Environment) As TraitPrediction()
        Dim stream As pipeline = pipeline.TryCreatePipeline(Of TraitPrediction)(data, env)

        If stream.isError Then
            If env IsNot Nothing Then
                Call env.AddMessage(stream.getError.ToString)
            End If

            Return Nothing
        End If

        Return stream.populates(Of TraitPrediction)(env).ToArray
    End Function

End Module
