Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Imports System.Text
Imports System.Text.Json
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
''' expression.json    输入时序表达矩阵（基因名、样本名、时间点、矩阵）
''' prior.json         先验调控网络（PriorNetwork 的 JSON 形式）
''' parameters.tsv     全部可训练参数（名称 + 形状 + 数值）
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
    Public Const PackageVersion As Integer = 1

    ' 包内条目名
    Private Const EntryManifest As String = "manifest.json"
    Private Const EntryConfig As String = "config.json"
    Private Const EntryExpression As String = "expression.json"
    Private Const EntryPrior As String = "prior.json"
    Private Const EntryParameters As String = "parameters.tsv"
    Private Const EntryTraining As String = "training.json"

    ''' <summary>
    ''' 把训练好的模型保存为 zip 压缩包。
    ''' </summary>
    ''' <param name="model">待保存的模型</param>
    ''' <param name="filePath">输出路径（建议以 <see cref="FileExtension"/> 结尾）</param>
    ''' <remarks>
    ''' 先写入同目录的临时文件，成功后再原子替换目标文件，
    ''' 避免写入中途失败留下半截的包。
    ''' </remarks>
    Public Sub Save(model As GPSModel, filePath As String)
        If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
        If String.IsNullOrWhiteSpace(filePath) Then
            Throw New ArgumentException("输出路径不能为空", NameOf(filePath))
        End If

        Dim directory As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(filePath))

        If Not String.IsNullOrEmpty(directory) AndAlso Not System.IO.Directory.Exists(directory) Then
            Call System.IO.Directory.CreateDirectory(directory)
        End If

        Dim tempFile As String = filePath & ".tmp"

        If System.IO.File.Exists(tempFile) Then
            Call System.IO.File.Delete(tempFile)
        End If

        Using stream As New FileStream(tempFile, FileMode.CreateNew, FileAccess.Write)
            Using archive As New ZipArchive(stream, ZipArchiveMode.Create)
                Call WriteTextEntry(archive, EntryManifest, BuildManifestJson(model))
                Call WriteTextEntry(archive, EntryConfig, ConfigToJson(model.Config))
                Call WriteTextEntry(archive, EntryExpression, ExpressionToJson(model.Expression))
                Call WriteTextEntry(archive, EntryPrior, model.Prior.ToJson(indent:=False))
                Call WriteTextEntry(archive, EntryParameters, ParametersToTsv(model.Model.Parameters))
                Call WriteTextEntry(archive, EntryTraining, TrainingToJson(model.LastTrainingResult))
            End Using
        End Using

        If System.IO.File.Exists(filePath) Then
            Call System.IO.File.Delete(filePath)
        End If

        Call System.IO.File.Move(tempFile, filePath)
    End Sub

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

        Using archive As ZipArchive = ZipFile.OpenRead(filePath)
            Dim manifestText As String = ReadTextEntry(archive, EntryManifest)

            If manifestText Is Nothing Then
                Throw New InvalidDataException($"不是有效的 GPS 模型包（缺少 {EntryManifest}）：{filePath}")
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
            Dim expression As GeneExpressionData = ExpressionFromJson(ReadRequiredEntry(archive, EntryExpression))
            Dim prior As PriorNetwork = PriorNetwork.ParseJson(ReadRequiredEntry(archive, EntryPrior))
            Dim model As New GPSModel(expression, prior, config)

            Call ApplyParameters(model.Model.Parameters, ReadRequiredEntry(archive, EntryParameters))

            Dim trainingText As String = ReadTextEntry(archive, EntryTraining)

            If Not String.IsNullOrWhiteSpace(trainingText) Then
                Dim result As GPSTrainingResult = TrainingFromJson(trainingText)

                If result IsNot Nothing Then Call model.AttachTrainingResult(result)
            End If

            Return model
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
    Private Function BuildManifestJson(model As GPSModel) As String
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

    ''' <summary>把表达矩阵写成 JSON（矩阵按行优先展开）。</summary>
    ''' <param name="expression">表达矩阵</param>
    ''' <returns>JSON 文本</returns>
    Private Function ExpressionToJson(expression As GeneExpressionData) As String
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
                writer.WriteStartArray("matrix")

                For g As Integer = 0 To expression.NGene - 1
                    For s As Integer = 0 To expression.NSample - 1
                        writer.WriteNumberValue(expression.Matrix(g, s))
                    Next
                Next

                writer.WriteEndArray()
                writer.WriteEndObject()
            End Sub)
    End Function

    ''' <summary>从 JSON 还原表达矩阵。</summary>
    ''' <param name="json">JSON 文本</param>
    ''' <returns>表达矩阵</returns>
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

    ''' <summary>把参数表写成 TSV（<c>名称 \t 形状 \t 数值列表</c>）。</summary>
    ''' <param name="store">参数登记表</param>
    ''' <returns>TSV 文本</returns>
    Private Function ParametersToTsv(store As ParameterStore) As String
        Dim builder As New StringBuilder()

        Call builder.Append("# gps-parameters v").Append(PackageVersion).AppendLine()
        Call builder.AppendLine("# name<TAB>shape<TAB>values")

        For Each entry As ParameterEntry In store.Entries
            Dim values As Double() = entry.Value.Data
            Dim parts As String() = New String(values.Length - 1) {}

            For i As Integer = 0 To values.Length - 1
                parts(i) = values(i).ToString("G17", CultureInfo.InvariantCulture)
            Next

            Call builder.Append(entry.Name).Append(vbTab).
                Append(String.Join(",", entry.Value.Shape)).Append(vbTab).
                Append(String.Join(",", parts)).AppendLine()
        Next

        Return builder.ToString()
    End Function

    ''' <summary>
    ''' 把包内参数写回模型，并校验名称与元素个数。
    ''' </summary>
    ''' <param name="store">模型参数登记表（结构已由配置重建）</param>
    ''' <param name="tsv">TSV 文本</param>
    Private Sub ApplyParameters(store As ParameterStore, tsv As String)
        Dim persisted As New Dictionary(Of String, Double())(StringComparer.Ordinal)
        Dim lines As String() = tsv.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)

        For Each line As String In lines
            Dim text As String = line.Trim()

            If text.Length = 0 OrElse text.StartsWith("#", StringComparison.Ordinal) Then Continue For

            Dim fields As String() = text.Split(vbTab)

            If fields.Length < 3 Then
                Throw New InvalidDataException($"参数行格式错误（应有 3 列）：{Shorten(text)}")
            End If

            Dim values As String() = fields(2).Split(","c)
            Dim parsed As Double() = New Double(values.Length - 1) {}

            For i As Integer = 0 To values.Length - 1
                parsed(i) = Double.Parse(values(i), NumberStyles.Float, CultureInfo.InvariantCulture)
            Next

            persisted(fields(0)) = parsed
        Next

        Dim missing As New List(Of String)()
        Dim mismatched As New List(Of String)()

        For Each entry As ParameterEntry In store.Entries
            Dim values As Double() = Nothing

            If Not persisted.TryGetValue(entry.Name, values) Then
                Call missing.Add(entry.Name)
                Continue For
            End If

            If values.Length <> entry.Value.Data.Length Then
                Call mismatched.Add($"{entry.Name}（包内 {values.Length} / 模型 {entry.Value.Data.Length}）")
                Continue For
            End If

            Call Array.Copy(values, entry.Value.Data, values.Length)
            Call entry.Value.MarkHostModified()
        Next

        If missing.Count > 0 OrElse mismatched.Count > 0 Then
            Throw New InvalidDataException(
                "参数与当前模型结构不匹配：" &
                If(missing.Count > 0, $"缺少 [{String.Join(", ", missing.Take(8))}]；", "") &
                If(mismatched.Count > 0, $"长度不一致 [{String.Join("; ", mismatched.Take(8))}]", ""))
        End If
    End Sub

    ' ==================== 训练结果 ====================

    ''' <summary>把训练结果写成 JSON；未训练时返回空字符串。</summary>
    ''' <param name="result">训练结果</param>
    ''' <returns>JSON 文本或空字符串</returns>
    Private Function TrainingToJson(result As GPSTrainingResult) As String
        If result Is Nothing Then Return ""

        Return Serialize(
            Sub(writer)
                writer.WriteStartObject()
                WriteNumberArray(writer, "pretrainLoss", result.PretrainLoss)
                WriteNumberArray(writer, "trainLoss", result.TrainLoss)
                WriteNumberArray(writer, "validationLoss", result.ValidationLoss)
                writer.WriteNumber("bestEpoch", result.BestEpoch)

                If Double.IsNaN(result.BestValidationLoss) OrElse Double.IsInfinity(result.BestValidationLoss) Then
                    writer.WriteNull("bestValidationLoss")
                Else
                    writer.WriteNumber("bestValidationLoss", result.BestValidationLoss)
                End If

                writer.WriteEndObject()
            End Sub)
    End Function

    ''' <summary>从 JSON 还原训练结果。</summary>
    ''' <param name="json">JSON 文本</param>
    ''' <returns>训练结果；空文本返回 Nothing</returns>
    Private Function TrainingFromJson(json As String) As GPSTrainingResult
        If String.IsNullOrWhiteSpace(json) Then Return Nothing

        Dim result As New GPSTrainingResult()

        Using document As JsonDocument = JsonDocument.Parse(json)
            Dim root As JsonElement = document.RootElement
            Dim value As JsonElement

            If root.TryGetProperty("pretrainLoss", value) Then
                Call result.PretrainLoss.AddRange(ReadDoubleArray(value))
            End If

            If root.TryGetProperty("trainLoss", value) Then
                Call result.TrainLoss.AddRange(ReadDoubleArray(value))
            End If

            If root.TryGetProperty("validationLoss", value) Then
                Call result.ValidationLoss.AddRange(ReadDoubleArray(value))
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
