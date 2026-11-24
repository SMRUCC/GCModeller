Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Reflection
Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.Squiiff.Model
Imports SMRUCC.genomics.Analysis.Squiiff.NN

Namespace IO

    ''' <summary>模型存档的读取结果。</summary>
    Public Class SquiiffArchive

        Public Property Model As DiffusionAutoEncoder
        Public Property GeneNames As String()
        Public Property Config As SquiiffConfig
    End Class

    ''' <summary>
    ''' 模型存档（单个 zip 容器）。
    '''
    ''' 容器内容（全部为文本，便于人工检查与跨平台迁移）：
    ''' <list type="bullet">
    ''' <item><c>config.tsv</c> —— 超参数（属性名 + 不变文化字符串）；</item>
    ''' <item><c>genes.tsv</c> —— 基因名清单（行序即模型输出维度的顺序）；</item>
    ''' <item><c>parameters.tsv</c> —— 每行一个参数：名称 + 元素数 + 扁平化数值。</item>
    ''' </list>
    ''' 超参数与参数均按**名称**匹配，因此新增字段不会破坏旧存档（缺失字段沿用默认值）。
    ''' </summary>
    Public Module SquiiffStorage

        Private Const ConfigEntry As String = "config.tsv"
        Private Const GeneEntry As String = "genes.tsv"
        Private Const ParameterEntry As String = "parameters.tsv"

        ''' <summary>
        ''' 批归一化滑动统计量条目。
        ''' 推理默认走滑动统计量，若只存档可训练参数，Load 回来的模型会给出不同的输出。
        ''' 该条目为**可选**：旧存档中没有它时静默跳过（沿用初始 mean=0 / var=1）。
        ''' </summary>
        Private Const StatisticsEntry As String = "statistics.tsv"

        ''' <summary>保存模型与基因名清单。</summary>
        Public Sub Save(model As DiffusionAutoEncoder, geneNames As String(), path As String)
            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))

            Dim parent = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path))
            If Not String.IsNullOrEmpty(parent) AndAlso Not System.IO.Directory.Exists(parent) Then
                Call System.IO.Directory.CreateDirectory(parent)
            End If

            If File.Exists(path) Then File.Delete(path)

            Using archive As ZipArchive = ZipFile.Open(path, ZipArchiveMode.Create)
                Call WriteEntry(archive, ConfigEntry, SerializeConfig(model.Config))
                Call WriteEntry(archive, GeneEntry, If(geneNames, New String() {}))
                Call WriteEntry(archive, ParameterEntry, SerializeParameters(model))
                Call WriteEntry(archive, StatisticsEntry, SerializeStatistics(model))
            End Using
        End Sub

        ''' <summary>读取模型存档。</summary>
        Public Function Load(path As String) As SquiiffArchive
            If Not File.Exists(path) Then Throw New FileNotFoundException($"模型存档不存在: {path}", path)

            Dim config As New SquiiffConfig()
            Dim geneNames As String() = Nothing
            Dim parameterLines As String() = Nothing
            Dim statisticsLines As String() = Nothing

            Using archive As ZipArchive = ZipFile.OpenRead(path)
                Dim configLines = ReadEntry(archive, ConfigEntry)
                If configLines IsNot Nothing Then ApplyConfig(config, configLines)

                geneNames = ReadEntry(archive, GeneEntry)
                parameterLines = ReadEntry(archive, ParameterEntry)
                statisticsLines = ReadEntry(archive, StatisticsEntry)
            End Using

            If geneNames Is Nothing OrElse geneNames.Length = 0 Then
                Throw New InvalidDataException($"模型存档缺少基因名清单（{GeneEntry}）: {path}")
            End If

            Dim model As New DiffusionAutoEncoder(config, geneNames.Length, config.Seed)
            Call RestoreParameters(model, parameterLines)
            Call RestoreStatistics(model, statisticsLines)

            Return New SquiiffArchive With {
                .Model = model,
                .GeneNames = geneNames,
                .Config = config
            }
        End Function

#Region "配置序列化"

        Private Function SerializeConfig(config As SquiiffConfig) As String()
            Dim lines As New List(Of String)
            For Each p In ConfigProperties()
                lines.Add($"{p.Name}{ControlChars.Tab}{Convert.ToString(p.GetValue(config), CultureInfo.InvariantCulture)}")
            Next
            Return lines.ToArray()
        End Function

        Private Sub ApplyConfig(config As SquiiffConfig, lines As String())
            Dim map = ConfigProperties().ToDictionary(Function(p) p.Name, Function(p) p, StringComparer.OrdinalIgnoreCase)

            For Each line In lines
                If String.IsNullOrWhiteSpace(line) Then Continue For

                Dim parts = line.Split(ControlChars.Tab)
                If parts.Length < 2 Then Continue For

                Dim propertyInfo As PropertyInfo = Nothing
                If Not map.TryGetValue(parts(0).Trim(), propertyInfo) Then Continue For

                Try
                    Dim text = parts(1).Trim()
                    Dim targetType = propertyInfo.PropertyType

                    Dim value As Object
                    If targetType.IsEnum Then
                        value = [Enum].Parse(targetType, text, ignoreCase:=True)
                    Else
                        value = Convert.ChangeType(text, targetType, CultureInfo.InvariantCulture)
                    End If

                    propertyInfo.SetValue(config, value)
                Catch ex As Exception
                    ' 单个字段解析失败不影响其它字段，沿用默认值
                End Try
            Next
        End Sub

        Private Function ConfigProperties() As PropertyInfo()
            Return GetType(SquiiffConfig) _
                .GetProperties(BindingFlags.Public Or BindingFlags.Instance) _
                .Where(Function(p) p.CanRead AndAlso p.CanWrite AndAlso IsSerializable(p.PropertyType)) _
                .ToArray()
        End Function

        Private Function IsSerializable(t As Type) As Boolean
            Return t.IsPrimitive OrElse t.IsEnum OrElse t = GetType(String) OrElse t = GetType(Decimal)
        End Function

#End Region

#Region "参数序列化"

        Private Function SerializeParameters(model As DiffusionAutoEncoder) As String()
            Dim lines As New List(Of String)
            For Each p In model.Parameters
                Dim data = p.Value.Data
                Dim cells As New List(Of String)(data.Length + 2)
                cells.Add(p.Name)
                cells.Add(data.Length.ToString(CultureInfo.InvariantCulture))

                For i As Integer = 0 To data.Length - 1
                    cells.Add(data(i).ToString("R", CultureInfo.InvariantCulture))
                Next

                lines.Add(String.Join(ControlChars.Tab, cells))
            Next

            Return lines.ToArray()
        End Function

        Private Sub RestoreParameters(model As DiffusionAutoEncoder, lines As String())
            If lines Is Nothing Then Return

            Dim map = model.Parameters.ToDictionary(Function(p) p.Name, Function(p) p, StringComparer.Ordinal)
            Dim restored As Integer = 0

            For Each line In lines
                If String.IsNullOrWhiteSpace(line) Then Continue For

                Dim parts = line.Split(ControlChars.Tab)
                If parts.Length < 2 Then Continue For

                Dim target As Parameter = Nothing
                If Not map.TryGetValue(parts(0), target) Then Continue For

                Dim data = target.Value.Data
                Dim count = data.Length
                If parts.Length - 2 <> count Then
                    Throw New InvalidDataException($"参数 {target.Name} 的元素数不匹配：存档 {parts.Length - 2}，模型 {count}")
                End If

                For i As Integer = 0 To count - 1
                    data(i) = Double.Parse(parts(i + 2), CultureInfo.InvariantCulture)
                Next

                Call target.Value.MarkHostModified()
                restored += 1
            Next

            If restored <> map.Count Then
                Throw New InvalidDataException($"参数还原不完整：存档 {restored} 个，模型 {map.Count} 个")
            End If
        End Sub

#End Region

#Region "滑动统计量序列化"

        ''' <summary>
        ''' 每行一条：层名 + 均值元素数 + 均值数值 + 方差元素数 + 方差数值。
        ''' 均值与方差形状必然一致，但仍各自记录元素数以便校验。
        ''' </summary>
        Private Function SerializeStatistics(model As DiffusionAutoEncoder) As String()
            Dim lines As New List(Of String)

            For Each norm In model.Normalizations
                Dim meanData = norm.RunningMean.Data
                Dim varData = norm.RunningVariance.Data
                Dim cells As New List(Of String)(meanData.Length + varData.Length + 3)

                cells.Add(norm.Name)
                cells.Add(meanData.Length.ToString(CultureInfo.InvariantCulture))
                For i As Integer = 0 To meanData.Length - 1
                    cells.Add(meanData(i).ToString("R", CultureInfo.InvariantCulture))
                Next

                cells.Add(varData.Length.ToString(CultureInfo.InvariantCulture))
                For i As Integer = 0 To varData.Length - 1
                    cells.Add(varData(i).ToString("R", CultureInfo.InvariantCulture))
                Next

                lines.Add(String.Join(ControlChars.Tab, cells))
            Next

            Return lines.ToArray()
        End Function

        ''' <summary>
        ''' 还原滑动统计量。按层名匹配；条目缺失（旧存档）或个别层缺失时静默跳过，
        ''' 沿用初始的 mean=0 / var=1，不影响其余部分的还原。
        ''' </summary>
        Private Sub RestoreStatistics(model As DiffusionAutoEncoder, lines As String())
            If lines Is Nothing Then Return

            Dim map = model.Normalizations.ToDictionary(Function(n) n.Name, Function(n) n, StringComparer.Ordinal)

            For Each line In lines
                If String.IsNullOrWhiteSpace(line) Then Continue For

                Dim parts = line.Split(ControlChars.Tab)
                If parts.Length < 3 Then Continue For

                Dim target As IRunningStatistics = Nothing
                If Not map.TryGetValue(parts(0), target) Then Continue For

                Dim meanCount As Integer
                If Not Integer.TryParse(parts(1), meanCount) Then Continue For
                If parts.Length < meanCount + 4 Then Continue For

                Dim varCount As Integer
                If Not Integer.TryParse(parts(2 + meanCount), varCount) Then Continue For
                If parts.Length <> meanCount + varCount + 3 Then Continue For

                Dim meanData = target.RunningMean.Data
                Dim varData = target.RunningVariance.Data
                If meanData.Length <> meanCount OrElse varData.Length <> varCount Then Continue For

                For i As Integer = 0 To meanCount - 1
                    meanData(i) = Double.Parse(parts(2 + i), CultureInfo.InvariantCulture)
                Next
                For i As Integer = 0 To varCount - 1
                    varData(i) = Double.Parse(parts(3 + meanCount + i), CultureInfo.InvariantCulture)
                Next

                Call target.RunningMean.MarkHostModified()
                Call target.RunningVariance.MarkHostModified()
            Next
        End Sub

#End Region

        Private Sub WriteEntry(archive As ZipArchive, name As String, lines As String())
            Dim entry = archive.CreateEntry(name, CompressionLevel.Optimal)
            Using writer As New StreamWriter(entry.Open(), New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False))
                For Each line In lines
                    writer.WriteLine(line)
                Next
            End Using
        End Sub

        Private Function ReadEntry(archive As ZipArchive, name As String) As String()
            Dim entry = archive.GetEntry(name)
            If entry Is Nothing Then Return Nothing

            Dim lines As New List(Of String)
            Using reader As New StreamReader(entry.Open())
                While Not reader.EndOfStream
                    lines.Add(reader.ReadLine())
                End While
            End Using

            Return lines.ToArray()
        End Function
    End Module
End Namespace
