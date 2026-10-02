' ============================================================================
' ModelLoader.vb
'
' 表型模型仓库：把每一个表型所对应的 SVM 模型实例序列化为一个独立的 json
' 文件，并使用一个目录级的 index.json 记录全部表型的元数据与训练指标，
' 同时把 Pfam 词表与编码配置写入 pfam_vocabulary.json。
'
'   {index}.json                 目录索引（表型元数据 + 训练指标 + 模型文件名）
'   {nnnnn}_{sanitized}.json     单个表型模型（SvmModelJSON）
'   pfam_vocabulary.json         Pfam 词表与编码配置（预测端必须还原）
' ============================================================================

Imports System.IO
Imports Microsoft.VisualBasic.MachineLearning.SVM
Imports Microsoft.VisualBasic.MachineLearning.SVM.StorageProcedure
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace metaTraits.Traitar

    ''' <summary>
    ''' index.json 之中的单条表型记录
    ''' </summary>
    Public Class PhenotypeModelIndex

        Public Property trait_name As String
        Public Property data_type As String
        Public Property unit As String
        Public Property group_1_category As String
        Public Property group_2_subcategory As String
        ''' <summary>模型状态：trained / skipped</summary>
        Public Property status As String
        ''' <summary>参与训练的样本数量</summary>
        Public Property sampleCount As Integer
        ''' <summary>交叉验证得分</summary>
        Public Property cvScore As Double
        ''' <summary>被跳过（或失败）的原因</summary>
        Public Property errorMessage As String
        ''' <summary>该表型模型所对应的 json 文件名，skipped 时为空</summary>
        Public Property modelFile As String

    End Class

    ''' <summary>
    ''' 模型仓库的目录索引
    ''' </summary>
    Public Class ModelStoreIndex

        ''' <summary>模型仓库的创建时间</summary>
        Public Property createdTime As String
        ''' <summary>特征维度数量（Pfam 词表长度）</summary>
        Public Property dimensions As Integer
        ''' <summary>Pfam 编码方式</summary>
        Public Property encoding As PfamEncoding
        ''' <summary>Pfam 词表剪枝阈值</summary>
        Public Property minGenomes As Integer
        ''' <summary>样本（基因组）数量</summary>
        Public Property sampleCount As Integer
        ''' <summary>全部表型的索引记录</summary>
        Public Property models As PhenotypeModelIndex()

    End Class

    ''' <summary>
    ''' 表型 SVM 模型仓库：负责把训练得到的模型集落盘以及从磁盘还原
    ''' </summary>
    Public Class ModelLoader

        ''' <summary>表型名 -> 该表型所对应的 SVM 模型实例</summary>
        Public Property Models As New Dictionary(Of String, Models.PhenotypeModel)()
        ''' <summary>Pfam 词表与编码配置</summary>
        Public Property Embedding As PfamEmbedding
        ''' <summary>模型仓库所在的目录</summary>
        Public Property Directory As String

        Public Const INDEX_FILE As String = "index.json"
        Public Const VOCABULARY_FILE As String = "pfam_vocabulary.json"

        ''' <summary>已经训练成功的模型数量</summary>
        Public ReadOnly Property TrainedCount As Integer
            Get
                Return Models.Values.Count(Function(m) m.IsTrained())
            End Get
        End Property

        ''' <summary>表型模型总数</summary>
        Public ReadOnly Property PhenotypeCount As Integer
            Get
                Return Models.Count
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{TrainedCount}/{PhenotypeCount} trained models @ {Directory}"
        End Function

        ''' <summary>
        ''' 把表型名转换为安全的 json 文件名
        ''' </summary>
        Public Shared Function SafeFileName(trait_name As String, Optional index As Integer = 0) As String
            Dim name As String = If(trait_name, "")

            For Each c As Char In Path.GetInvalidFileNameChars()
                name = name.Replace(c, "_"c)
            Next

            name = name.Trim()

            If name.Length > 80 Then
                name = name.Substring(0, 80).Trim()
            End If
            If name.Length = 0 Then
                name = "trait"
            End If

            Return $"{index:D5}_{name}.json"
        End Function

        ''' <summary>
        ''' 把全部表型模型保存到目标目录之中
        ''' </summary>
        ''' <param name="models">表型名 -> SVM 模型实例</param>
        ''' <param name="embedding">Pfam 嵌入配置</param>
        ''' <param name="dir">模型仓库目录</param>
        ''' <param name="sampleCount">训练集之中的样本数量</param>
        Public Shared Function SaveDirectory(models As IDictionary(Of String, Models.PhenotypeModel),
                                             embedding As PfamEmbedding,
                                             dir As String,
                                             Optional sampleCount As Integer = 0,
                                             Optional verbose As Boolean = True) As ModelLoader

            If Not System.IO.Directory.Exists(dir) Then
                Call System.IO.Directory.CreateDirectory(dir)
            End If

            Dim index As New List(Of PhenotypeModelIndex)
            Dim i As Integer = 0

            For Each kvp As KeyValuePair(Of String, Models.PhenotypeModel) In models
                Dim model As Models.PhenotypeModel = kvp.Value
                Dim trait As PhenotypeTrait = model.Trait
                Dim modelFile As String = ""

                If model.IsTrained() Then
                    If Not TypeOf model.Model.transform Is RangeTransform Then
                        ' SvmModelJSON 在还原时要求 rangeTransform 不为空
                        model.Status = "skipped"
                        model.ErrorMessage = "the range transform of this model is not a RangeTransform"
                        model.Model = Nothing
                    Else
                        modelFile = SafeFileName(kvp.Key, i)

                        Dim json As String = SvmModelJSON.CreateJSONModel(model.Model).GetJson

                        Call System.IO.File.WriteAllText(Path.Combine(dir, modelFile), json)
                    End If
                End If

                If trait Is Nothing Then
                    index.Add(New PhenotypeModelIndex With {
                        .trait_name = kvp.Key,
                        .status = model.Status,
                        .sampleCount = model.SampleCount,
                        .cvScore = model.CVScore,
                        .errorMessage = model.ErrorMessage,
                        .modelFile = modelFile
                    })
                Else
                    index.Add(New PhenotypeModelIndex With {
                        .trait_name = trait.trait_name,
                        .data_type = trait.data_type,
                        .unit = trait.unit,
                        .group_1_category = trait.group_1_category,
                        .group_2_subcategory = trait.group_2_subcategory,
                        .status = model.Status,
                        .sampleCount = model.SampleCount,
                        .cvScore = model.CVScore,
                        .errorMessage = model.ErrorMessage,
                        .modelFile = modelFile
                    })
                End If

                i += 1
            Next

            Dim store As New ModelStoreIndex With {
                .createdTime = Now.ToString("yyyy-MM-dd HH:mm:ss"),
                .dimensions = If(embedding Is Nothing, 0, embedding.GetDimensionCount()),
                .encoding = If(embedding Is Nothing, PfamEncoding.NormalizedCount, embedding.encoding),
                .minGenomes = If(embedding Is Nothing, 1, embedding.minGenomes),
                .sampleCount = sampleCount,
                .models = index.ToArray
            }

            Call System.IO.File.WriteAllText(Path.Combine(dir, INDEX_FILE), store.GetJson)

            If embedding IsNot Nothing Then
                Call embedding.Save(Path.Combine(dir, VOCABULARY_FILE))
            End If

            If verbose Then
                Console.WriteLine($"[ModelLoader] saved {index.Count(Function(m) m.modelFile.Length > 0)} models to '{dir}'")
            End If

            Return New ModelLoader With {
                .Models = New Dictionary(Of String, Models.PhenotypeModel)(models),
                .Embedding = embedding,
                .Directory = dir
            }
        End Function

        ''' <summary>
        ''' 从模型仓库目录之中还原全部表型模型
        ''' </summary>
        ''' <param name="dir">模型仓库目录</param>
        Public Shared Function LoadDirectory(dir As String, Optional verbose As Boolean = True) As ModelLoader
            Dim loader As New ModelLoader With {.Directory = dir}
            Dim indexFile As String = Path.Combine(dir, INDEX_FILE)
            Dim vocabularyFile As String = Path.Combine(dir, VOCABULARY_FILE)

            If System.IO.File.Exists(vocabularyFile) Then
                loader.Embedding = PfamEmbedding.Load(vocabularyFile)
            End If

            If Not System.IO.File.Exists(indexFile) Then
                Throw New FileNotFoundException($"the model index file '{indexFile}' is not exists!", indexFile)
            End If

            Dim store As ModelStoreIndex = System.IO.File _
                .ReadAllText(indexFile) _
                .LoadJSON(Of ModelStoreIndex)()
            Dim n As Integer = 0

            For Each item As PhenotypeModelIndex In store.models
                Dim trait As New PhenotypeTrait With {
                    .trait_name = item.trait_name,
                    .data_type = item.data_type,
                    .unit = item.unit,
                    .group_1_category = item.group_1_category,
                    .group_2_subcategory = item.group_2_subcategory
                }
                Dim model As New Models.PhenotypeModel With {
                    .Trait = trait,
                    .SampleCount = item.sampleCount,
                    .CVScore = item.cvScore,
                    .Status = item.status,
                    .ErrorMessage = item.errorMessage
                }

                If item.modelFile IsNot Nothing AndAlso item.modelFile.Length > 0 Then
                    Dim path As String = Path.Combine(dir, item.modelFile)

                    If System.IO.File.Exists(path) Then
                        model.Model = System.IO.File _
                            .ReadAllText(path) _
                            .LoadJSON(Of SvmModelJSON)() _
                            .CreateSVMModel()

                        n += 1
                    End If
                End If

                loader.Models(item.trait_name) = model
            Next

            If verbose Then
                Console.WriteLine($"[ModelLoader] loaded {n} trained models from '{dir}'")
            End If

            Return loader
        End Function

        ''' <summary>
        ''' 获取指定表型所对应的模型实例，不存在时返回 Nothing
        ''' </summary>
        Public Function GetModel(trait_name As String) As Models.PhenotypeModel
            Dim model As Models.PhenotypeModel = Nothing

            If Models.TryGetValue(trait_name, model) Then
                Return model
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 获取全部已经训练成功的表型模型
        ''' </summary>
        Public Iterator Function GetTrainedModels() As IEnumerable(Of Models.PhenotypeModel)
            For Each model As Models.PhenotypeModel In Models.Values
                If model.IsTrained() Then
                    Yield model
                End If
            Next
        End Function

    End Class
End Namespace
