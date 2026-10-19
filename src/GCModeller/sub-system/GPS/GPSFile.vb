Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.Json
Imports Microsoft.VisualBasic.FileIO.Path
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.GPS.Config
Imports SMRUCC.genomics.Analysis.GPS.Model
Imports SMRUCC.genomics.Analysis.GPS.Training

''' <summary>
''' GPS 模型持久化包的清单信息（<c>manifest.json</c>）。
''' </summary>
''' <remarks>
''' 清单是"包的自描述"：给出格式版本、创建时间与模型规模，
''' 便于调用方在不加载全部参数的情况下判断兼容性。
''' </remarks>
Public Class GPSPackageManifest

    ''' <summary>包格式标识（固定为 <c>GCModeller.GPS</c>）</summary>
    Public Property Format As String = GPSFile.PackageFormat

    ''' <summary>包格式版本</summary>
    Public Property Version As Integer = GPSFile.PackageVersion

    ''' <summary>创建时间（UTC，ISO 8601）</summary>
    Public Property Created As String

    ''' <summary>建模基因数</summary>
    Public Property NGenes As Integer

    ''' <summary>样本数</summary>
    Public Property NSamples As Integer

    ''' <summary>时间点数</summary>
    Public Property NTimePoints As Integer

    ''' <summary>滑窗长度</summary>
    Public Property Window As Integer

    ''' <summary>模型隐藏维度</summary>
    Public Property DModel As Integer

    ''' <summary>GPS 融合层数</summary>
    Public Property NLayers As Integer

    ''' <summary>是否已完成训练</summary>
    Public Property IsTrained As Boolean

    ''' <summary>微调轮数</summary>
    Public Property TrainingEpochs As Integer

    ''' <summary>最优验证损失（无验证集时为 +∞）</summary>
    Public Property BestValidationLoss As Double

    ''' <summary>全局参数更新步数（Adam 偏置校正依据；无训练状态时为 0）</summary>
    Public Property StepIndex As Integer

    ''' <summary>中断时所处阶段（0=预训练, 1=微调；无训练状态时为 0）</summary>
    Public Property Phase As Integer

    ''' <summary>预训练已完成的 epoch 数</summary>
    Public Property PretrainEpochsDone As Integer

    ''' <summary>微调已完成的 epoch 数</summary>
    Public Property FinetuneEpochsDone As Integer

    ''' <summary>备注（自由文本）</summary>
    Public Property Notes As String

    Public Overrides Function ToString() As String
        Return $"GPS v{Version} · {NGenes} 基因 × {NSamples} 样本 · window={Window} d={DModel} L={NLayers}" &
               $" · trained={IsTrained}"
    End Function

End Class

''' <summary>
''' GPS 模型的持久化：保存为 zip 压缩包，以及从压缩包重建模型对象。
''' </summary>
''' <remarks>
''' <para>
''' 包内结构（<c>*.gps.zip</c>）：
''' <code>
''' manifest.json      包格式/版本/规模/训练概要
''' config.json        全部超参数（按属性名反射写出）
''' expression.json    表达矩阵元数据（基因名、样本名、时间点、尺寸）
''' expression.matrix.bin  表达矩阵本体（v3 起；行优先 Double，流式压缩）
''' prior.json         先验调控网络（PriorNetwork 的 JSON 形式）
''' parameters.tsv     全部可训练参数（名称 + 形状 + 数值，流式写出）
''' optimizer.tsv      Adam 一阶/二阶矩（v2 起，流式写出）
''' training.json      训练损失曲线与最优 epoch
''' </code>
''' </para>
''' <para>
''' <b>只保存输入与参数、不保存派生结构</b>：预处理结果、调控图、SPD/LapPE 编码、
''' 滑窗切分都由 <c>config + expression + prior</c> 确定性重算。
''' 这样既避免为派生结构维护序列化格式，也保证加载端与训练端走同一条构建路径，
''' 不会出现"包里的图编码与重新训练时不一致"的隐患。
''' </para>
''' <para>
''' 参数以 <c>G17</c> 格式文本保存，保证 double 往返无损；
''' 加载时逐个校验参数名与元素个数，形状或名称不匹配立即报错而不是静默错位。
''' </para>
''' </remarks>
Public Module GPSFile

    ''' <summary>推荐的文件扩展名</summary>
    Public Const FileExtension As String = ".gps.zip"

    ''' <summary>包格式标识</summary>
    Public Const PackageFormat As String = "GCModeller.GPS"

    ''' <summary>当前包格式版本</summary>
    ''' <remarks>
    ''' v2 起包内可携带训练状态：<c>optimizer.tsv</c>（Adam 一阶/二阶矩）与
    ''' <c>training.json</c> 内嵌的 <c>state</c> 对象（步计数/阶段进度/早停/最优快照/曲线前缀）。
    ''' 读取端对缺失条目按 v1 降级处理，v1 包仍可正常加载。
    ''' </remarks>
    Public Const PackageVersion As Integer = 3

    ' 包内条目名
    Private Const EntryManifest As String = "manifest.json"
    Private Const EntryConfig As String = "config.json"
    Private Const EntryExpression As String = "expression.json"
    Private Const EntryPrior As String = "prior.json"
    Private Const EntryParameters As String = "parameters.tsv"
    Private Const EntryTraining As String = "training.json"
    Private Const EntryOptimizer As String = "optimizer.tsv"

    ''' <summary>
    ''' v3 起表达矩阵改为独立的二进制条目（行优先 <c>Double</c>，小端）。
    ''' </summary>
    ''' <remarks>
    ''' 全基因组规模下 JSON 文本形式的矩阵会有上亿个数值：既让 <c>writeBin</c>
    ''' 需要数 GB 的临时字符串/字节数组，也让加载端必须先把整段文本读进内存。
    ''' 二进制条目配合流式读写，峰值内存只与分块缓冲区有关。
    ''' </remarks>
    Private Const EntryMatrix As String = "expression.matrix.bin"

    ''' <summary>
    ''' v3 起最优参数快照单独存为流式 TSV（<c>名称 \t 数值列表</c>），
    ''' 不再内嵌进 <c>training.json</c>。
    ''' </summary>
    Private Const EntryBestParameters As String = "state.best.tsv"

    ''' <summary>
    ''' 把训练好的模型保存为 zip 压缩包。
    ''' </summary>
    ''' <param name="model">待保存的模型</param>
    ''' <param name="filePath">输出路径（建议以 <see cref="FileExtension"/> 结尾）</param>
    ''' <remarks>
    ''' 先写入同目录的临时文件，成功后再原子替换目标文件，
    ''' 避免写入中途失败留下半截的包。
    ''' 模型此前训练过（<see cref="GPSModel.TrainerState"/> 非空）时，
    ''' 包内会附带优化器动量与训练状态，可直接用于续训。
    ''' </remarks>
    '''
    <Extension>
    Public Sub Save(model As GPSModel, filePath As String)
        If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
        If String.IsNullOrWhiteSpace(filePath) Then
            Throw New ArgumentException("输出路径不能为空", NameOf(filePath))
        End If

        Call SaveAtomic(filePath,
                        Sub(stream) Call Save(model, stream, model.TrainerState))
    End Sub

    ''' <summary>
    ''' 把模型与训练状态一起保存为 checkpoint 包（原子写入，覆盖式滚动更新）。
    ''' </summary>
    ''' <param name="model">模型</param>
    ''' <param name="filePath">输出路径</param>
    ''' <param name="state">训练器状态；Nothing 时退化为普通 <see cref="Save(GPSModel, String)"/></param>
    ''' <remarks>
    ''' 与 <see cref="Save(GPSModel, String)"/> 的差别仅在于：即使模型尚未标记
    ''' "已训练"（例如中断在第一个 epoch 的 checkpoint 回调里），也会写入
    ''' <paramref name="state"/> 与优化器动量，保证中断现场完整。
    ''' </remarks>
    <Extension>
    Public Sub SaveCheckpoint(model As GPSModel, filePath As String, state As GPSTrainingState)
        If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
        If String.IsNullOrWhiteSpace(filePath) Then
            Throw New ArgumentException("输出路径不能为空", NameOf(filePath))
        End If

        Call SaveAtomic(filePath,
                        Sub(stream) Call Save(model, stream, state))
    End Sub

    ''' <summary>原子写辅助：临时文件 + 替换，失败不留半截包。</summary>
    ''' <param name="filePath">目标路径</param>
    ''' <param name="write">写包体委托（在临时文件的流上执行）</param>
    Private Sub SaveAtomic(filePath As String, write As Action(Of Stream))
        Dim directory As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(filePath))

        If Not String.IsNullOrEmpty(directory) AndAlso Not System.IO.Directory.Exists(directory) Then
            Call System.IO.Directory.CreateDirectory(directory)
        End If

        Dim tempFile As String = filePath & ".tmp"

        If System.IO.File.Exists(tempFile) Then
            Call System.IO.File.Delete(tempFile)
        End If

        Using stream As New FileStream(tempFile, FileMode.CreateNew, FileAccess.Write)
            Call write(stream)
        End Using

        If System.IO.File.Exists(filePath) Then
            Call System.IO.File.Delete(filePath)
        End If

        Call System.IO.File.Move(tempFile, filePath)
    End Sub

    ''' <summary>
    ''' 把模型（可选附带训练状态）写入 zip 流。
    ''' </summary>
    ''' <param name="model">模型</param>
    ''' <param name="stream">输出流</param>
    ''' <param name="state">训练器状态；Nothing 时只写权重与曲线（v1 等价内容）</param>
    ''' <remarks>
    ''' this function will close the zip file stream after all model data has been save in to the zip archive 
    ''' </remarks>
    <Extension>
    Public Sub Save(model As GPSModel, stream As Stream, Optional state As GPSTrainingState = Nothing)
        Using archive As New ZipArchive(stream, ZipArchiveMode.Create)
            Call WriteTextEntry(archive, EntryManifest, BuildManifestJson(model, state))
            Call WriteTextEntry(archive, EntryConfig, ConfigToJson(model.Config))
            Call WriteExpressionEntry(archive, model.Expression)
            Call WriteTextEntry(archive, EntryPrior, model.Prior.ToJson(indent:=False))
            Call WriteParametersEntry(archive, model.Model.Parameters)

            If state IsNot Nothing Then
                Call WriteMomentaEntry(archive, model.Model.Parameters)
            End If

            Call WriteTextEntry(archive, EntryTraining, TrainingToJson(model.LastTrainingResult, state))

            If state IsNot Nothing AndAlso state.BestParameters IsNot Nothing AndAlso
               state.BestParameters.Count > 0 Then

                Call WriteBestParametersEntry(archive, state)
            End If
        End Using
    End Sub

    Public Function Load(zip As Stream) As GPSModel
        Dim filePath As String = zip.ToString

        If TypeOf zip Is FileStream Then
            filePath = DirectCast(zip, FileStream).Name
        End If

        Using archive As New ZipArchive(zip, ZipArchiveMode.Read)
            Dim manifestText As String = ReadTextEntry(archive, EntryManifest)

            If manifestText Is Nothing Then
                Throw New InvalidDataException($"不是有效的 GPS 模型包（缺少 {EntryManifest}）：{FilePath}")
            End If

            Dim manifest As GPSPackageManifest = ParseManifest(manifestText)

            If Not String.Equals(manifest.Format, PackageFormat, StringComparison.Ordinal) Then
                Throw New InvalidDataException($"包格式不受支持：{manifest.Format}")
            End If

            If manifest.Version > PackageVersion Then
                Throw New InvalidDataException(
                    $"包版本 {manifest.Version} 高于当前支持的 {PackageVersion}，请升级 GPS 库")
            End If

            Dim config As GPSConfig = ConfigFromJson(ReadRequiredEntry(archive, EntryConfig))
            Dim expressionText As String = ReadRequiredEntry(archive, EntryExpression)
            Dim matrixEntry As ZipArchiveEntry = archive.GetEntry(EntryMatrix)
            Dim expression As GeneExpressionData

            If matrixEntry Is Nothing Then
                ' v1/v2 包：矩阵内嵌在 expression.json 里（旧格式，仅用于向后兼容）
                expression = ExpressionFromJson(expressionText)
            Else
                expression = ExpressionFromJsonMeta(expressionText, matrixEntry)
            End If

            Dim prior As PriorNetwork = PriorNetwork.ParseJson(ReadRequiredEntry(archive, EntryPrior))
            Dim model As New GPSModel(expression, prior, config)

            Call ApplyParametersEntry(archive, model.Model.Parameters)

            ' v2 起携带 Adam 动量；条目缺失（v1 包）时按零动量降级，
            ' 表现为"权重热启动、优化器冷启动"的旧语义
            Dim optimizerEntry As ZipArchiveEntry = archive.GetEntry(EntryOptimizer)

            If optimizerEntry IsNot Nothing Then
                Using stream As Stream = optimizerEntry.Open()
                    Using reader As New StreamReader(stream, Encoding.UTF8)
                        Call model.Model.Parameters.RestoreMomentaFrom(reader)
                    End Using
                End Using
            End If

            Dim trainingText As String = ReadTextEntry(archive, EntryTraining)

            If Not String.IsNullOrWhiteSpace(trainingText) Then
                Dim result As GPSTrainingResult = TrainingFromJson(trainingText)
                Dim state As GPSTrainingState = StateFromTrainingJson(trainingText)
                Dim bestEntry As ZipArchiveEntry = archive.GetEntry(EntryBestParameters)

                ' v3 起最优参数快照是独立条目；旧包仍从 training.json 内嵌对象读取
                If bestEntry IsNot Nothing Then
                    Call ReadBestParametersEntry(bestEntry, state)
                End If

                Call model.AttachTrainingResult(result, state)
            End If

            Return model
        End Using
    End Function

    ''' <summary>
    ''' 从 zip 压缩包重建模型对象。
    ''' </summary>
    ''' <param name="filePath">包路径</param>
    ''' <returns>重建的模型（已恢复全部参数与训练状态）</returns>
    ''' <remarks>
    ''' 重建过程：读取表达矩阵与先验网络 → 用保存的超参数重新走一遍
    ''' 预处理 / 建图 / 图编码 / 滑窗切分 → 把包内参数逐项写回模型。
    ''' 因此返回对象的派生结构（图编码、数据集、扰动引擎）都是现成可用的。
    ''' </remarks>
    Public Function Load(filePath As String) As GPSModel
        If String.IsNullOrWhiteSpace(filePath) Then
            Throw New ArgumentException("包路径不能为空", NameOf(filePath))
        End If
        If Not System.IO.File.Exists(filePath) Then
            Throw New FileNotFoundException($"GPS 模型包不存在：{filePath}", filePath)
        End If

        Using archive As Stream = filePath.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return Load(archive)
        End Using
    End Function

    ''' <summary>
    ''' 读取包清单（不加载参数，用于快速检查兼容性与模型规模）。
    ''' </summary>
    ''' <param name="filePath">包路径</param>
    ''' <returns>清单对象</returns>
    Public Function ReadManifest(filePath As String) As GPSPackageManifest
        If Not System.IO.File.Exists(filePath) Then
            Throw New FileNotFoundException($"GPS 模型包不存在：{filePath}", filePath)
        End If

        Using archive As ZipArchive = ZipFile.OpenRead(filePath)
            Return ParseManifest(ReadRequiredEntry(archive, EntryManifest))
        End Using
    End Function

    ''' <summary>
    ''' 判断文件是否为一个 GPS 模型包（检查 zip 内是否含清单）。
    ''' </summary>
    ''' <param name="filePath">候选文件</param>
    ''' <returns>是模型包时返回 True</returns>
    Public Function IsPackage(filePath As String) As Boolean
        If String.IsNullOrWhiteSpace(filePath) OrElse Not System.IO.File.Exists(filePath) Then
            Return False
        End If

        Try
            Using archive As ZipArchive = ZipFile.OpenRead(filePath)
                Return archive.GetEntry(EntryManifest) IsNot Nothing
            End Using
        Catch ex As InvalidDataException
            Return False
        End Try
    End Function

    ' ==================== 清单 ====================

    ''' <summary>构造清单 JSON。</summary>
    ''' <param name="model">模型</param>
    ''' <returns>JSON 文本</returns>
    Private Function BuildManifestJson(model As GPSModel, state As GPSTrainingState) As String
        Dim manifest As New GPSPackageManifest With {
            .Format = PackageFormat,
            .Version = PackageVersion,
            .Created = Date.UtcNow.ToString("o", CultureInfo.InvariantCulture),
            .NGenes = model.GeneNames.Length,
            .NSamples = model.Expression.NSample,
            .NTimePoints = model.Dataset.NTimePoint,
            .Window = model.Dataset.WindowSize,
            .DModel = model.Config.DModel,
            .NLayers = model.Config.NLayers,
            .IsTrained = model.IsTrained,
            .TrainingEpochs = If(model.LastTrainingResult Is Nothing, 0, model.LastTrainingResult.FinetuneEpochs),
            .BestValidationLoss = If(model.LastTrainingResult Is Nothing,
                                     Double.PositiveInfinity,
                                     model.LastTrainingResult.BestValidationLoss),
            .StepIndex = If(state Is Nothing, 0, state.StepIndex),
            .Phase = If(state Is Nothing, 0, state.Phase),
            .PretrainEpochsDone = If(state Is Nothing, 0, state.PretrainEpochsDone),
            .FinetuneEpochsDone = If(state Is Nothing, 0, state.FinetuneEpochsDone),
            .Notes = "generated by SMRUCC.genomics.Analysis.GPS.GPSFile"
        }

        Return Serialize(
            Sub(writer)
                writer.WriteStartObject()
                writer.WriteString(NameOf(GPSPackageManifest.Format), manifest.Format)
                writer.WriteNumber(NameOf(GPSPackageManifest.Version), manifest.Version)
                writer.WriteString(NameOf(GPSPackageManifest.Created), manifest.Created)
                writer.WriteNumber(NameOf(GPSPackageManifest.NGenes), manifest.NGenes)
                writer.WriteNumber(NameOf(GPSPackageManifest.NSamples), manifest.NSamples)
                writer.WriteNumber(NameOf(GPSPackageManifest.NTimePoints), manifest.NTimePoints)
                writer.WriteNumber(NameOf(GPSPackageManifest.Window), manifest.Window)
                writer.WriteNumber(NameOf(GPSPackageManifest.DModel), manifest.DModel)
                writer.WriteNumber(NameOf(GPSPackageManifest.NLayers), manifest.NLayers)
                writer.WriteBoolean(NameOf(GPSPackageManifest.IsTrained), manifest.IsTrained)
                writer.WriteNumber(NameOf(GPSPackageManifest.TrainingEpochs), manifest.TrainingEpochs)

                ' +∞ 无法用 JSON number 表示，用 null 承载
                If Double.IsNaN(manifest.BestValidationLoss) OrElse Double.IsInfinity(manifest.BestValidationLoss) Then
                    writer.WriteNull(NameOf(GPSPackageManifest.BestValidationLoss))
                Else
                    writer.WriteNumber(NameOf(GPSPackageManifest.BestValidationLoss), manifest.BestValidationLoss)
                End If

                writer.WriteNumber(NameOf(GPSPackageManifest.StepIndex), manifest.StepIndex)
                writer.WriteNumber(NameOf(GPSPackageManifest.Phase), manifest.Phase)
                writer.WriteNumber(NameOf(GPSPackageManifest.PretrainEpochsDone), manifest.PretrainEpochsDone)
                writer.WriteNumber(NameOf(GPSPackageManifest.FinetuneEpochsDone), manifest.FinetuneEpochsDone)
                writer.WriteString(NameOf(GPSPackageManifest.Notes), manifest.Notes)
                writer.WriteEndObject()
            End Sub)
    End Function

    ''' <summary>解析清单 JSON。</summary>
    ''' <param name="json">JSON 文本</param>
    ''' <returns>清单对象</returns>
    Private Function ParseManifest(json As String) As GPSPackageManifest
        Dim manifest As New GPSPackageManifest()

        Using document As JsonDocument = JsonDocument.Parse(json)
            Dim root As JsonElement = document.RootElement
            Dim value As JsonElement

            If root.TryGetProperty(NameOf(GPSPackageManifest.Format), value) Then manifest.Format = value.GetString()
            If root.TryGetProperty(NameOf(GPSPackageManifest.Version), value) Then manifest.Version = value.GetInt32()
            If root.TryGetProperty(NameOf(GPSPackageManifest.Created), value) Then manifest.Created = value.GetString()
            If root.TryGetProperty(NameOf(GPSPackageManifest.NGenes), value) Then manifest.NGenes = value.GetInt32()
            If root.TryGetProperty(NameOf(GPSPackageManifest.NSamples), value) Then manifest.NSamples = value.GetInt32()
            If root.TryGetProperty(NameOf(GPSPackageManifest.NTimePoints), value) Then manifest.NTimePoints = value.GetInt32()
            If root.TryGetProperty(NameOf(GPSPackageManifest.Window), value) Then manifest.Window = value.GetInt32()
            If root.TryGetProperty(NameOf(GPSPackageManifest.DModel), value) Then manifest.DModel = value.GetInt32()
            If root.TryGetProperty(NameOf(GPSPackageManifest.NLayers), value) Then manifest.NLayers = value.GetInt32()
            If root.TryGetProperty(NameOf(GPSPackageManifest.IsTrained), value) Then manifest.IsTrained = value.GetBoolean()
            If root.TryGetProperty(NameOf(GPSPackageManifest.TrainingEpochs), value) Then manifest.TrainingEpochs = value.GetInt32()

            If root.TryGetProperty(NameOf(GPSPackageManifest.BestValidationLoss), value) Then
                manifest.BestValidationLoss = If(value.ValueKind = JsonValueKind.Null,
                                                 Double.PositiveInfinity,
                                                 value.GetDouble())
            End If

            ' v2 起的 checkpoint 元信息；v1 包缺失时保留默认值
            If root.TryGetProperty(NameOf(GPSPackageManifest.StepIndex), value) Then manifest.StepIndex = value.GetInt32()
            If root.TryGetProperty(NameOf(GPSPackageManifest.Phase), value) Then manifest.Phase = value.GetInt32()
            If root.TryGetProperty(NameOf(GPSPackageManifest.PretrainEpochsDone), value) Then manifest.PretrainEpochsDone = value.GetInt32()
            If root.TryGetProperty(NameOf(GPSPackageManifest.FinetuneEpochsDone), value) Then manifest.FinetuneEpochsDone = value.GetInt32()

            If root.TryGetProperty(NameOf(GPSPackageManifest.Notes), value) Then manifest.Notes = value.GetString()
        End Using

        Return manifest
    End Function

    ' ==================== 超参数 ====================

    ''' <summary>把配置按公共可读写属性反射成 JSON。</summary>
    ''' <param name="config">配置对象</param>
    ''' <returns>JSON 文本</returns>
    Private Function ConfigToJson(config As GPSConfig) As String
        Dim properties As PropertyInfo() = ConfigProperties()

        Return Serialize(
            Sub(writer)
                writer.WriteStartObject()

                For Each [property] As PropertyInfo In properties
                    Dim value As Object = [property].GetValue(config)

                    Select Case [property].PropertyType
                        Case GetType(Integer)
                            writer.WriteNumber([property].Name, CInt(value))
                        Case GetType(Double)
                            writer.WriteNumber([property].Name, CDbl(value))
                        Case GetType(Boolean)
                            writer.WriteBoolean([property].Name, CBool(value))
                        Case GetType(String)
                            writer.WriteString([property].Name, CStr(value))
                        Case Else
                            ' 其它类型当前不存在；出现时跳过而不是写出无法还原的字段
                    End Select
                Next

                writer.WriteEndObject()
            End Sub)
    End Function

    ''' <summary>从 JSON 还原配置对象（未出现的字段保留默认值）。</summary>
    ''' <param name="json">JSON 文本</param>
    ''' <returns>配置对象</returns>
    Private Function ConfigFromJson(json As String) As GPSConfig
        Dim config As New GPSConfig()
        Dim properties As PropertyInfo() = ConfigProperties()

        Using document As JsonDocument = JsonDocument.Parse(json)
            For Each item As JsonProperty In document.RootElement.EnumerateObject()
                Dim [property] As PropertyInfo = properties.
                    FirstOrDefault(Function(p) String.Equals(p.Name, item.Name, StringComparison.Ordinal))

                If [property] Is Nothing Then Continue For

                Select Case [property].PropertyType
                    Case GetType(Integer)
                        [property].SetValue(config, item.Value.GetInt32())
                    Case GetType(Double)
                        [property].SetValue(config, item.Value.GetDouble())
                    Case GetType(Boolean)
                        [property].SetValue(config, item.Value.GetBoolean())
                    Case GetType(String)
                        [property].SetValue(config, item.Value.GetString())
                End Select
            Next
        End Using

        Return config
    End Function

    ''' <summary>配置类型的公共可读写属性（按名称排序，保证输出稳定）。</summary>
    ''' <returns>属性列表</returns>
    Private Function ConfigProperties() As PropertyInfo()
        Return GetType(GPSConfig).
            GetProperties(BindingFlags.Public Or BindingFlags.Instance).
            Where(Function(p) p.CanRead AndAlso p.CanWrite).
            OrderBy(Function(p) p.Name, StringComparer.Ordinal).
            ToArray()
    End Function

    ' ==================== 表达矩阵 ====================

    ''' <summary>写出表达矩阵：元数据（JSON）+ 矩阵（二进制条目）。</summary>
    ''' <param name="archive">zip 归档</param>
    ''' <param name="expression">表达矩阵</param>
    Private Sub WriteExpressionEntry(archive As ZipArchive, expression As GeneExpressionData)
        Call WriteTextEntry(archive, EntryExpression, ExpressionMetaToJson(expression))

        If expression.Matrix IsNot Nothing Then
            Call WriteMatrixEntry(archive, expression.Matrix)
        End If
    End Sub

    ''' <summary>把表达矩阵的元数据写成 JSON（不含矩阵本体）。</summary>
    ''' <param name="expression">表达矩阵</param>
    ''' <returns>JSON 文本</returns>
    Private Function ExpressionMetaToJson(expression As GeneExpressionData) As String
        Return Serialize(
            Sub(writer)
                writer.WriteStartObject()
                writer.WriteNumber("nGenes", expression.NGene)
                writer.WriteNumber("nSamples", expression.NSample)

                writer.WriteStartArray("geneNames")

                For Each gene As String In expression.GeneNames
                    writer.WriteStringValue(gene)
                Next

                writer.WriteEndArray()
                writer.WriteStartArray("sampleNames")

                For Each sample As String In expression.SampleNames
                    writer.WriteStringValue(sample)
                Next

                writer.WriteEndArray()
                writer.WriteStartArray("timePoints")

                For Each time As Double In expression.TimePoints
                    writer.WriteNumberValue(time)
                Next

                writer.WriteEndArray()
                writer.WriteEndObject()
            End Sub)
    End Function

    ''' <summary>把表达矩阵以行优先的 <c>Double</c> 二进制块写入 zip 条目。</summary>
    ''' <param name="archive">zip 归档</param>
    ''' <param name="matrix">表达矩阵</param>
    ''' <remarks>
    ''' 以 1 MB 为一块搬运：<c>Buffer.BlockCopy</c> 直接按字节偏移作用于二维数组
    ''' （.NET 的多维数组按行优先连续存放），因此不需要把整个矩阵展开成一维副本。
    ''' </remarks>
    Private Sub WriteMatrixEntry(archive As ZipArchive, matrix As Double(,))
        Dim rows As Integer = matrix.GetLength(0)
        Dim cols As Integer = matrix.GetLength(1)

        If rows <= 0 OrElse cols <= 0 Then Return

        Dim entry As ZipArchiveEntry = archive.CreateEntry(EntryMatrix, CompressionLevel.Optimal)
        Dim rowBytes As Integer = cols * 8
        Dim rowsPerChunk As Integer = System.Math.Max(1, (1024 * 1024) \ rowBytes)
        Dim buffer As Byte() = New Byte(rowsPerChunk * rowBytes - 1) {}

        Using stream As Stream = entry.Open()
            Dim row As Integer = 0

            While row < rows
                Dim count As Integer = System.Math.Min(rowsPerChunk, rows - row)
                Dim bytes As Integer = count * rowBytes

                Call System.Buffer.BlockCopy(matrix, row * rowBytes, buffer, 0, bytes)
                Call stream.Write(buffer, 0, bytes)

                row += count
            End While
        End Using
    End Sub

    ''' <summary>从二进制条目读取表达矩阵（必须与元数据声明的尺寸一致）。</summary>
    ''' <param name="entry">矩阵条目</param>
    ''' <param name="matrix">目标矩阵（尺寸由调用方按元数据分配）</param>
    Private Sub ReadMatrixEntry(entry As ZipArchiveEntry, matrix As Double(,))
        Dim rows As Integer = matrix.GetLength(0)
        Dim cols As Integer = matrix.GetLength(1)

        If rows <= 0 OrElse cols <= 0 Then Return

        Dim rowBytes As Integer = cols * 8
        Dim rowsPerChunk As Integer = System.Math.Max(1, (1024 * 1024) \ rowBytes)
        Dim buffer As Byte() = New Byte(rowsPerChunk * rowBytes - 1) {}

        Using stream As Stream = entry.Open()
            Dim row As Integer = 0

            While row < rows
                Dim count As Integer = System.Math.Min(rowsPerChunk, rows - row)
                Dim bytes As Integer = count * rowBytes
                Dim filled As Integer = 0

                While filled < bytes
                    Dim read As Integer = stream.Read(buffer, filled, bytes - filled)

                    If read <= 0 Then
                        Throw New InvalidDataException(
                            $"表达矩阵二进制条目不完整：期望 {rows}×{cols}，在第 {row} 行中断")
                    End If

                    filled += read
                End While

                Call System.Buffer.BlockCopy(buffer, 0, matrix, row * rowBytes, bytes)

                row += count
            End While
        End Using
    End Sub

    ''' <summary>由元数据 JSON + 二进制矩阵条目重建表达矩阵（v3 包）。</summary>
    ''' <param name="json">元数据 JSON</param>
    ''' <param name="matrixEntry">矩阵条目</param>
    ''' <returns>表达矩阵</returns>
    Private Function ExpressionFromJsonMeta(json As String, matrixEntry As ZipArchiveEntry) As GeneExpressionData
        Using document As JsonDocument = JsonDocument.Parse(json)
            Dim root As JsonElement = document.RootElement
            Dim nGenes As Integer = root.GetProperty("nGenes").GetInt32()
            Dim nSamples As Integer = root.GetProperty("nSamples").GetInt32()

            Dim geneNames As String() = ReadStringArray(root.GetProperty("geneNames"))
            Dim sampleNames As String() = ReadStringArray(root.GetProperty("sampleNames"))
            Dim timeElement As JsonElement
            Dim timePoints As Double() = If(root.TryGetProperty("timePoints", timeElement),
                                            ReadDoubleArray(timeElement),
                                            New Double() {})

            If geneNames.Length <> nGenes OrElse sampleNames.Length <> nSamples Then
                Throw New InvalidDataException("表达矩阵的基因/样本数量与声明不一致")
            End If

            Dim matrix As Double(,) = New Double(nGenes - 1, nSamples - 1) {}

            Call ReadMatrixEntry(matrixEntry, matrix)

            Return New GeneExpressionData With {
                .GeneNames = geneNames,
                .SampleNames = sampleNames,
                .TimePoints = timePoints,
                .Matrix = matrix
            }
        End Using
    End Function

    ''' <summary>
    ''' 从 JSON 还原表达矩阵（v1/v2 包：矩阵内嵌在 <c>expression.json</c> 中）。
    ''' </summary>
    ''' <param name="json">JSON 文本</param>
    ''' <returns>表达矩阵</returns>
    ''' <remarks>
    ''' 仅为向后兼容保留：整段文本 + 一维副本 + 二维矩阵三份同时存在，
    ''' 全基因组规模下内存开销很大，新包一律走二进制条目。
    ''' </remarks>
    Private Function ExpressionFromJson(json As String) As GeneExpressionData
        Using document As JsonDocument = JsonDocument.Parse(json)
            Dim root As JsonElement = document.RootElement
            Dim nGenes As Integer = root.GetProperty("nGenes").GetInt32()
            Dim nSamples As Integer = root.GetProperty("nSamples").GetInt32()

            Dim geneNames As String() = ReadStringArray(root.GetProperty("geneNames"))
            Dim sampleNames As String() = ReadStringArray(root.GetProperty("sampleNames"))
            Dim timePoints As Double() = ReadDoubleArray(root.GetProperty("timePoints"))
            Dim flat As Double() = ReadDoubleArray(root.GetProperty("matrix"))

            If geneNames.Length <> nGenes OrElse sampleNames.Length <> nSamples Then
                Throw New InvalidDataException("表达矩阵的基因/样本数量与声明不一致")
            End If

            If flat.Length <> nGenes * nSamples Then
                Throw New InvalidDataException(
                    $"表达矩阵元素数 {flat.Length} 与 {nGenes}×{nSamples} 不一致")
            End If

            Dim matrix As Double(,) = New Double(nGenes - 1, nSamples - 1) {}
            Dim offset As Integer = 0

            For g As Integer = 0 To nGenes - 1
                For s As Integer = 0 To nSamples - 1
                    matrix(g, s) = flat(offset)
                    offset += 1
                Next
            Next

            Return New GeneExpressionData With {
                .GeneNames = geneNames,
                .SampleNames = sampleNames,
                .TimePoints = timePoints,
                .Matrix = matrix
            }
        End Using
    End Function

    ' ==================== 参数 ====================

    ''' <summary>把参数表逐行写入 zip 条目（<c>名称 \t 形状 \t 数值列表</c>）。</summary>
    ''' <param name="archive">zip 归档</param>
    ''' <param name="store">参数登记表</param>
    ''' <remarks>
    ''' 全基因组模型的单个参数（解码器权重）就有 770 万个数。旧实现先在内存里拼出
    ''' 整个 TSV 字符串（约 300 MB），再转成 UTF8 字节数组压缩，加上 StringBuilder
    ''' 的扩容副本，峰值轻松超过数 GB —— 实测 6 万基因训练时 checkpoint 直接抛
    ''' <c>Insufficient memory</c>。改为逐数值流式写出后，峰值只与一行的长度有关。
    ''' </remarks>
    Private Sub WriteParametersEntry(archive As ZipArchive, store As ParameterStore)
        Dim entry As ZipArchiveEntry = archive.CreateEntry(EntryParameters, CompressionLevel.Optimal)

        Using stream As Stream = entry.Open()
            Using writer As New StreamWriter(stream, New UTF8Encoding(False))
                Call writer.WriteLine($"# gps-parameters v{PackageVersion}")
                Call writer.WriteLine("# name<TAB>shape<TAB>values")

                For Each parameter As ParameterEntry In store.Entries
                    Call writer.Write(parameter.Name)
                    Call writer.Write(vbTab)
                    Call writer.Write(String.Join(",", parameter.Value.Shape))
                    Call writer.Write(vbTab)
                    Call WriteValues(writer, parameter.Value.Data)
                    Call writer.WriteLine()
                Next
            End Using
        End Using
    End Sub

    ''' <summary>把优化器动量逐行写入 zip 条目（与参数表同样流式）。</summary>
    ''' <param name="archive">zip 归档</param>
    ''' <param name="store">参数登记表</param>
    Private Sub WriteMomentaEntry(archive As ZipArchive, store As ParameterStore)
        Dim entry As ZipArchiveEntry = archive.CreateEntry(EntryOptimizer, CompressionLevel.Optimal)

        Using stream As Stream = entry.Open()
            Using writer As New StreamWriter(stream, New UTF8Encoding(False))
                Call store.WriteMomenta(writer)
            End Using
        End Using
    End Sub

    ''' <summary>把"验证集最优参数快照"逐行写入 zip 条目（<c>名称 \t 数值列表</c>）。</summary>
    ''' <param name="archive">zip 归档</param>
    ''' <param name="state">训练状态（提供 <see cref="GPSTrainingState.BestParameters"/>）</param>
    Private Sub WriteBestParametersEntry(archive As ZipArchive, state As GPSTrainingState)
        Dim entry As ZipArchiveEntry = archive.CreateEntry(EntryBestParameters, CompressionLevel.Optimal)

        Using stream As Stream = entry.Open()
            Using writer As New StreamWriter(stream, New UTF8Encoding(False))
                For Each parameter As KeyValuePair(Of String, Double()) In state.BestParameters
                    Call writer.Write(parameter.Key)
                    Call writer.Write(vbTab)
                    Call WriteValues(writer, parameter.Value)
                    Call writer.WriteLine()
                Next
            End Using
        End Using
    End Sub

    ''' <summary>流式读回最优参数快照。</summary>
    ''' <param name="entry">快照条目</param>
    ''' <param name="state">目标训练状态</param>
    Private Sub ReadBestParametersEntry(entry As ZipArchiveEntry, state As GPSTrainingState)
        Using stream As Stream = entry.Open()
            Using reader As New StreamReader(stream, Encoding.UTF8)
                Dim line As String = reader.ReadLine()

                While line IsNot Nothing
                    Dim text As String = line.Trim()

                    If text.Length > 0 Then
                        Dim fields As String() = text.Split(vbTab)

                        If fields.Length >= 2 Then
                            state.BestParameters(fields(0)) = ParseValuesText(fields(1))
                        End If
                    End If

                    line = reader.ReadLine()
                End While
            End Using
        End Using
    End Sub

    ''' <summary>写出逗号分隔的 <c>G17</c> 数值（不构造中间字符串）。</summary>
    ''' <param name="writer">目标写入器</param>
    ''' <param name="values">数值数组</param>
    Private Sub WriteValues(writer As TextWriter, values As Double())
        For i As Integer = 0 To values.Length - 1
            If i > 0 Then Call writer.Write(","c)

            Call writer.Write(values(i).ToString("G17", CultureInfo.InvariantCulture))
        Next
    End Sub

    ''' <summary>解析一行逗号分隔的数值文本（span 切片，不产生子串）。</summary>
    ''' <param name="text">数值文本</param>
    ''' <returns>数值数组</returns>
    Private Function ParseValuesText(text As String) As Double()
        If String.IsNullOrEmpty(text) Then Return New Double() {}

        Dim count As Integer = 1

        For i As Integer = 0 To text.Length - 1
            If text(i) = ","c Then count += 1
        Next

        Dim parsed As Double() = New Double(count - 1) {}
        Dim start As Integer = 0
        Dim index As Integer = 0

        For i As Integer = 0 To text.Length
            If i = text.Length OrElse text(i) = ","c Then
                If i > start Then
                    parsed(index) = Double.Parse(
                        text.AsSpan(start, i - start), NumberStyles.Float, CultureInfo.InvariantCulture)
                End If

                index += 1
                start = i + 1
            End If
        Next

        Return parsed
    End Function

    ''' <summary>
    ''' 逐行读取参数条目并写回模型，同时校验名称与元素个数。
    ''' </summary>
    ''' <param name="archive">zip 归档</param>
    ''' <param name="store">模型参数登记表（结构已由配置重建）</param>
    ''' <remarks>
    ''' 流式版本的 <c>parameters.tsv</c> 读取：一次只在内存中保留一行，
    ''' 避免把 300 MB 的 TSV 整段读入再 <c>Split</c> 成上千万个子串
    ''' （全基因组模型下这是必然 OOM 的写法）。
    ''' </remarks>
    Private Sub ApplyParametersEntry(archive As ZipArchive, store As ParameterStore)
        Dim entry As ZipArchiveEntry = archive.GetEntry(EntryParameters)

        If entry Is Nothing Then
            Throw New InvalidDataException($"GPS 模型包缺少必需条目：{EntryParameters}")
        End If

        Dim seen As New HashSet(Of String)(StringComparer.Ordinal)
        Dim mismatched As New List(Of String)()

        Using stream As Stream = entry.Open()
            Using reader As New StreamReader(stream, Encoding.UTF8)
                Dim line As String = reader.ReadLine()

                While line IsNot Nothing
                    Dim text As String = line.Trim()

                    If text.Length > 0 AndAlso Not text.StartsWith("#", StringComparison.Ordinal) Then
                        Dim fields As String() = text.Split(vbTab)

                        If fields.Length < 3 Then
                            Throw New InvalidDataException($"参数行格式错误（应有 3 列）：{Shorten(text)}")
                        End If

                        Dim parameter As ParameterEntry = store.Find(fields(0))

                        If parameter Is Nothing Then
                            Throw New InvalidDataException($"包内参数 {fields(0)} 在当前模型中不存在")
                        End If

                        Dim values As Double() = ParseValuesText(fields(2))

                        If values.Length <> parameter.Value.Data.Length Then
                            Call mismatched.Add(
                                $"{parameter.Name}（包内 {values.Length} / 模型 {parameter.Value.Data.Length}）")
                        Else
                            Call Array.Copy(values, parameter.Value.Data, values.Length)
                            Call parameter.Value.MarkHostModified()
                            Call seen.Add(parameter.Name)
                        End If
                    End If

                    line = reader.ReadLine()
                End While
            End Using
        End Using

        Dim missing As New List(Of String)()

        For Each parameter As ParameterEntry In store.Entries
            If Not seen.Contains(parameter.Name) Then Call missing.Add(parameter.Name)
        Next

        If missing.Count > 0 OrElse mismatched.Count > 0 Then
            Throw New InvalidDataException(
                "参数与当前模型结构不匹配：" &
                If(missing.Count > 0, $"缺少 [{String.Join(", ", missing.Take(8))}]；", "") &
                If(mismatched.Count > 0, $"长度不一致 [{String.Join("; ", mismatched.Take(8))}]", ""))
        End If
    End Sub

    ' ==================== 训练结果 ====================

    ''' <summary>
    ''' 把训练结果（与可选的训练状态）写成 JSON；两者都为空时返回空字符串。
    ''' </summary>
    ''' <param name="result">训练结果</param>
    ''' <param name="state">训练器状态；非空时以 <c>state</c> 子对象内嵌</param>
    ''' <returns>JSON 文本或空字符串</returns>
    Private Function TrainingToJson(result As GPSTrainingResult, state As GPSTrainingState) As String
        If result Is Nothing AndAlso state Is Nothing Then Return ""

        Return Serialize(
            Sub(writer)
                writer.WriteStartObject()

                If result IsNot Nothing Then
                    WriteNumberArray(writer, "pretrainLoss", result.PretrainLoss)
                    WriteNumberArray(writer, "trainLoss", result.TrainLoss)
                    WriteNumberArray(writer, "validationLoss", result.ValidationLoss)
                    writer.WriteNumber("bestEpoch", result.BestEpoch)

                    If Double.IsNaN(result.BestValidationLoss) OrElse Double.IsInfinity(result.BestValidationLoss) Then
                        writer.WriteNull("bestValidationLoss")
                    Else
                        writer.WriteNumber("bestValidationLoss", result.BestValidationLoss)
                    End If
                End If

                If state IsNot Nothing Then
                    writer.WritePropertyName("state")

                    ' 最优参数快照不内嵌：全基因组规模下它是上千万个数值，
                    ' 由 WriteBestParametersEntry 单独流式写入 state.best.tsv
                    Using stateDocument As JsonDocument = JsonDocument.Parse(
                        state.ToJson(includeBestParameters:=False))

                        Call stateDocument.RootElement.WriteTo(writer)
                    End Using
                End If

                writer.WriteEndObject()
            End Sub)
    End Function

    ''' <summary>
    ''' 从 training.json 中解析内嵌的训练状态；不存在（v1 包）时返回 Nothing。
    ''' </summary>
    ''' <param name="json">training.json 文本</param>
    ''' <returns>训练状态或 Nothing</returns>
    Private Function StateFromTrainingJson(json As String) As GPSTrainingState
        If String.IsNullOrWhiteSpace(json) Then Return Nothing

        Using document As JsonDocument = JsonDocument.Parse(json)
            Dim root As JsonElement = document.RootElement
            Dim value As JsonElement

            If root.TryGetProperty("state", value) Then
                Return GPSTrainingState.FromJson(value.GetRawText())
            End If
        End Using

        Return Nothing
    End Function

    ''' <summary>从 JSON 还原训练结果。</summary>
    ''' <param name="json">JSON 文本</param>
    ''' <returns>训练结果；空文本返回 Nothing</returns>
    Private Function TrainingFromJson(json As String) As GPSTrainingResult
        If String.IsNullOrWhiteSpace(json) Then Return Nothing

        ' 曲线字段缺失时保留空数组而不是 Nothing：中断在训练早期保存的包
        ' 可能只含 state 而没有曲线，调用方读取 FinetuneEpochs 等派生属性不应踩空
        Dim result As New GPSTrainingResult With {
            .PretrainLoss = New Double(-1) {},
            .TrainLoss = New Double(-1) {},
            .ValidationLoss = New Double(-1) {}
        }

        Using document As JsonDocument = JsonDocument.Parse(json)
            Dim root As JsonElement = document.RootElement
            Dim value As JsonElement

            If root.TryGetProperty("pretrainLoss", value) Then
                result.PretrainLoss = ReadDoubleArray(value)
            End If

            If root.TryGetProperty("trainLoss", value) Then
                result.TrainLoss = ReadDoubleArray(value)
            End If

            If root.TryGetProperty("validationLoss", value) Then
                result.ValidationLoss = ReadDoubleArray(value)
            End If

            If root.TryGetProperty("bestEpoch", value) Then result.BestEpoch = value.GetInt32()

            If root.TryGetProperty("bestValidationLoss", value) Then
                result.BestValidationLoss = If(value.ValueKind = JsonValueKind.Null,
                                               Double.PositiveInfinity,
                                               value.GetDouble())
            End If
        End Using

        Return result
    End Function

    ' ==================== 通用工具 ====================

    ''' <summary>用 <see cref="Utf8JsonWriter"/> 生成缩进 JSON 文本。</summary>
    ''' <param name="write">写入委托</param>
    ''' <returns>JSON 文本</returns>
    Private Function Serialize(write As Action(Of Utf8JsonWriter)) As String
        Using stream As New MemoryStream()
            Using writer As New Utf8JsonWriter(stream, New JsonWriterOptions With {.Indented = True})
                Call write(writer)
            End Using

            Return Encoding.UTF8.GetString(stream.ToArray())
        End Using
    End Function

    ''' <summary>写一个数值数组。</summary>
    ''' <param name="writer">JSON 写入器</param>
    ''' <param name="name">字段名</param>
    ''' <param name="values">数值</param>
    Private Sub WriteNumberArray(writer As Utf8JsonWriter, name As String, values As IEnumerable(Of Double))
        writer.WriteStartArray(name)

        For Each value As Double In If(values, Enumerable.Empty(Of Double)())
            If Double.IsNaN(value) OrElse Double.IsInfinity(value) Then
                writer.WriteNullValue()
            Else
                writer.WriteNumberValue(value)
            End If
        Next

        writer.WriteEndArray()
    End Sub

    ''' <summary>读取字符串数组。</summary>
    ''' <param name="element">JSON 数组节点</param>
    ''' <returns>字符串数组</returns>
    Private Function ReadStringArray(element As JsonElement) As String()
        Dim values As New List(Of String)()

        For Each item As JsonElement In element.EnumerateArray()
            Call values.Add(item.GetString())
        Next

        Return values.ToArray()
    End Function

    ''' <summary>读取数值数组（null 记为 NaN）。</summary>
    ''' <param name="element">JSON 数组节点</param>
    ''' <returns>数值数组</returns>
    Private Function ReadDoubleArray(element As JsonElement) As Double()
        Dim values As New List(Of Double)()

        For Each item As JsonElement In element.EnumerateArray()
            Call values.Add(If(item.ValueKind = JsonValueKind.Null, Double.NaN, item.GetDouble()))
        Next

        Return values.ToArray()
    End Function

    ''' <summary>写入一个文本条目。</summary>
    ''' <param name="archive">zip 归档</param>
    ''' <param name="name">条目名</param>
    ''' <param name="content">文本内容</param>
    Private Sub WriteTextEntry(archive As ZipArchive, name As String, content As String)
        Dim entry As ZipArchiveEntry = archive.CreateEntry(name, CompressionLevel.Optimal)

        Using stream As Stream = entry.Open()
            Using writer As New StreamWriter(stream, New UTF8Encoding(False))
                Call writer.Write(content)
            End Using
        End Using
    End Sub

    ''' <summary>读取一个文本条目；不存在时返回 Nothing。</summary>
    ''' <param name="archive">zip 归档</param>
    ''' <param name="name">条目名</param>
    ''' <returns>文本内容</returns>
    Private Function ReadTextEntry(archive As ZipArchive, name As String) As String
        Dim entry As ZipArchiveEntry = archive.GetEntry(name)

        If entry Is Nothing Then Return Nothing

        Using stream As Stream = entry.Open()
            Using reader As New StreamReader(stream, Encoding.UTF8)
                Return reader.ReadToEnd()
            End Using
        End Using
    End Function

    ''' <summary>读取必需条目，缺失时抛出。</summary>
    ''' <param name="archive">zip 归档</param>
    ''' <param name="name">条目名</param>
    ''' <returns>文本内容</returns>
    Private Function ReadRequiredEntry(archive As ZipArchive, name As String) As String
        Dim text As String = ReadTextEntry(archive, name)

        If text Is Nothing Then
            Throw New InvalidDataException($"GPS 模型包缺少必需条目：{name}")
        End If

        Return text
    End Function

    ''' <summary>截断过长文本用于错误消息。</summary>
    ''' <param name="text">原文</param>
    ''' <returns>最多 80 字符</returns>
    Private Function Shorten(text As String) As String
        If text Is Nothing Then Return ""
        If text.Length <= 80 Then Return text

        Return text.Substring(0, 80) & "…"
    End Function

End Module
