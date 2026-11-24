Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Text
Imports SMRUCC.genomics.BioSystem

''' <summary>门面存档解析结果。</summary>
Friend Class VAGeneArchive

    Public Property Model As ConditionalVAE

    Public Property GeneNames As String()

    Public Property ConditionNames As String()

    Public Property Population As Double()()

    Public Property Registry As Dictionary(Of String, Integer)

    Public Property ControlCondition As Integer

    Public Property AlignmentMode As ContextAlignmentMode

End Class

''' <summary>
''' <see cref="VAGeneExperiment"/> 的单文件存档。
'''
''' 采用与 <c>SquiiffStorage</c> 一致的条目化 ZIP：
''' <list type="bullet">
''' <item><c>manifest.tsv</c>：维度、对照条件与对齐模式；</item>
''' <item><c>model.bin</c>：<see cref="ConditionalVAE"/> 的模型参数；</item>
''' <item><c>genes.txt</c> / <c>conditions.txt</c>：基因名与条件名；</item>
''' <item><c>population.tsv</c>：逐基因训练种群（分位对齐所需）；</item>
''' <item><c>registry.tsv</c>：显式扰动登记表。</item>
''' </list>
''' </summary>
Public Module VAGeneStorage

    Private Const ManifestEntry As String = "manifest.tsv"
    Private Const ModelEntry As String = "model.bin"
    Private Const GeneEntry As String = "genes.txt"
    Private Const ConditionEntry As String = "conditions.txt"
    Private Const PopulationEntry As String = "population.tsv"
    Private Const RegistryEntry As String = "registry.tsv"

    ''' <summary>把门面状态写入单文件存档。</summary>
    Public Sub Save(model As ConditionalVAE, geneNames As String(), conditionNames As String(),
                    population As Double()(), registry As Dictionary(Of String, Integer),
                    controlCondition As Integer, mode As ContextAlignmentMode, path As String)
        If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
        If geneNames Is Nothing OrElse geneNames.Length = 0 Then Throw New ArgumentException("基因名清单不能为空", NameOf(geneNames))
        If conditionNames Is Nothing OrElse conditionNames.Length = 0 Then Throw New ArgumentException("条件名清单不能为空", NameOf(conditionNames))
        If String.IsNullOrWhiteSpace(path) Then Throw New ArgumentException("存档路径不能为空", NameOf(path))

        Dim directory As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path))

        If Not String.IsNullOrEmpty(directory) Then
            Call System.IO.Directory.CreateDirectory(directory)
        End If

        Dim temp As String = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "vagene-" & Guid.NewGuid().ToString("N") & ".bin")

        Try
            Call model.Save(temp)

            If System.IO.File.Exists(path) Then System.IO.File.Delete(path)

            Using archive As ZipArchive = ZipFile.Open(path, ZipArchiveMode.Create)
                Call writeText(archive, ManifestEntry, serializeManifest(model, controlCondition, mode))
                Call writeText(archive, GeneEntry, String.Join(ControlChars.CrLf, geneNames))
                Call writeText(archive, ConditionEntry, String.Join(ControlChars.CrLf, conditionNames))
                Call writeText(archive, PopulationEntry, serializePopulation(geneNames, population))
                Call writeText(archive, RegistryEntry, serializeRegistry(registry))
                Call archive.CreateEntryFromFile(temp, ModelEntry, CompressionLevel.Optimal)
            End Using
        Finally
            If File.Exists(temp) Then File.Delete(temp)
        End Try
    End Sub

    ''' <summary>读取单文件存档并重建底层模型（仅供门面内部使用）。</summary>
    Friend Function Load(path As String) As VAGeneArchive
        If Not System.IO.File.Exists(path) Then Throw New FileNotFoundException($"模型存档不存在: {path}", path)

        Dim manifest As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        Dim genes As String() = Nothing
        Dim conditions As String() = Nothing
        Dim populationLines As String() = Nothing
        Dim registryLines As String() = Nothing
        Dim temp As String = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "vagene-" & Guid.NewGuid().ToString("N") & ".bin")

        Try
            Using archive As ZipArchive = ZipFile.OpenRead(path)
                Call readManifest(archive, ManifestEntry, manifest)

                genes = cleanLines(readLines(archive, GeneEntry))
                conditions = cleanLines(readLines(archive, ConditionEntry))
                populationLines = readLines(archive, PopulationEntry)
                registryLines = readLines(archive, RegistryEntry)

                Dim modelFileEntry As ZipArchiveEntry = archive.GetEntry(ModelEntry)

                If modelFileEntry Is Nothing Then
                    Throw New InvalidDataException($"模型存档缺少参数条目: {ModelEntry}")
                End If

                Call modelFileEntry.ExtractToFile(temp, True)
            End Using

            If genes Is Nothing OrElse genes.Length = 0 Then
                Throw New InvalidDataException("模型存档缺少基因名清单")
            End If
            If conditions Is Nothing OrElse conditions.Length = 0 Then
                Throw New InvalidDataException("模型存档缺少条件名清单")
            End If

            Dim geneCount As Integer = parseInt(manifest, "geneCount", genes.Length)
            Dim conditionCount As Integer = parseInt(manifest, "conditionCount", conditions.Length)
            Dim latentDim As Integer = parseInt(manifest, "latentDim", 16)

            If geneCount <> genes.Length Then
                Throw New InvalidDataException($"存档基因数 {geneCount} 与基因名清单长度 {genes.Length} 不一致")
            End If
            If conditionCount <> conditions.Length Then
                Throw New InvalidDataException($"存档条件数 {conditionCount} 与条件名清单长度 {conditions.Length} 不一致")
            End If
            Dim hiddenDim As Integer = parseInt(manifest, "hiddenDim", 64)
            Dim controlCondition As Integer = parseInt(manifest, "controlCondition", 0)
            Dim mode As ContextAlignmentMode = parseMode(manifestValue(manifest, "alignmentMode", Nothing))

            Dim model As New ConditionalVAE(geneCount, conditionCount, latentDim, hiddenDim)
            Dim errorMessage As String = Nothing

            If Not model.TryLoad(temp, errorMessage) Then
                Throw New InvalidDataException($"模型参数加载失败: {errorMessage}")
            End If

            Return New VAGeneArchive With {
                .Model = model,
                .GeneNames = genes,
                .ConditionNames = conditions,
                .Population = parsePopulation(genes, populationLines),
                .Registry = parseRegistry(registryLines),
                .ControlCondition = controlCondition,
                .AlignmentMode = mode
            }
        Finally
            If System.IO.File.Exists(temp) Then System.IO.File.Delete(temp)
        End Try
    End Function

#Region "清单"

    Private Function serializeManifest(model As ConditionalVAE, controlCondition As Integer,
                                       mode As ContextAlignmentMode) As String
        Dim lines As New List(Of String) From {
            "schemaVersion" & ControlChars.Tab & "1",
            "geneCount" & ControlChars.Tab & model.GeneCount.ToString(CultureInfo.InvariantCulture),
            "conditionCount" & ControlChars.Tab & model.ConditionCount.ToString(CultureInfo.InvariantCulture),
            "latentDim" & ControlChars.Tab & model.LatentDim.ToString(CultureInfo.InvariantCulture),
            "hiddenDim" & ControlChars.Tab & model.HiddenDim.ToString(CultureInfo.InvariantCulture),
            "meanLibrarySize" & ControlChars.Tab & model.MeanLibrarySize.ToString("R", CultureInfo.InvariantCulture),
            "controlCondition" & ControlChars.Tab & controlCondition.ToString(CultureInfo.InvariantCulture),
            "alignmentMode" & ControlChars.Tab & mode.ToString()
        }

        Return String.Join(ControlChars.CrLf, lines)
    End Function

    Private Sub readManifest(archive As ZipArchive, entryName As String, manifest As Dictionary(Of String, String))
        Dim lines As String() = readLines(archive, entryName)

        If lines Is Nothing Then Return

        For Each line As String In lines
            If String.IsNullOrWhiteSpace(line) Then Continue For

            Dim parts As String() = line.Split(ControlChars.Tab)

            If parts.Length < 2 Then Continue For

            manifest(parts(0).Trim()) = parts(1).Trim()
        Next
    End Sub

    Private Function manifestValue(manifest As Dictionary(Of String, String), key As String, fallback As String) As String
        Dim value As String = Nothing

        If manifest.TryGetValue(key, value) AndAlso Not String.IsNullOrWhiteSpace(value) Then
            Return value
        End If

        Return fallback
    End Function

    Private Function parseInt(manifest As Dictionary(Of String, String), key As String, fallback As Integer) As Integer
        Dim text As String = manifestValue(manifest, key, Nothing)
        Dim value As Integer

        If Not String.IsNullOrWhiteSpace(text) AndAlso
           Integer.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, value) Then
            Return value
        End If

        Return fallback
    End Function

    Private Function parseMode(text As String) As ContextAlignmentMode
        Dim mode As ContextAlignmentMode

        If Not String.IsNullOrWhiteSpace(text) AndAlso [Enum].TryParse(text, True, mode) Then
            Return mode
        End If

        Return ContextAlignmentMode.Quantile
    End Function

#End Region

#Region "种群与登记表"

    Private Function serializePopulation(geneNames As String(), population As Double()()) As String
        Dim lines As New List(Of String)(geneNames.Length)

        For i As Integer = 0 To geneNames.Length - 1
            Dim values As Double() = If(population IsNot Nothing AndAlso i < population.Length, population(i), Nothing)
            Dim cells As New List(Of String)()

            If values IsNot Nothing Then
                For Each value As Double In values
                    cells.Add(value.ToString("R", CultureInfo.InvariantCulture))
                Next
            End If

            cells.Insert(0, geneNames(i))
            lines.Add(String.Join(ControlChars.Tab, cells))
        Next

        Return String.Join(ControlChars.CrLf, lines)
    End Function

    Private Function parsePopulation(geneNames As String(), lines As String()) As Double()()
        Dim result(geneNames.Length - 1)() As Double

        If lines Is Nothing Then Return result

        Dim map As New Dictionary(Of String, Double())(StringComparer.OrdinalIgnoreCase)

        For Each line As String In lines
            If String.IsNullOrWhiteSpace(line) Then Continue For

            Dim parts As String() = line.Split(ControlChars.Tab)

            If parts.Length = 0 Then Continue For

            Dim values As Double() = If(parts.Length > 1, New Double(parts.Length - 2) {}, Array.Empty(Of Double)())

            For i As Integer = 1 To parts.Length - 1
                Dim value As Double

                If Double.TryParse(parts(i), NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                    values(i - 1) = value
                End If
            Next

            map(parts(0).Trim()) = values
        Next

        For i As Integer = 0 To geneNames.Length - 1
            Dim values As Double() = Nothing

            If map.TryGetValue(geneNames(i), values) Then
                result(i) = values
            End If
        Next

        Return result
    End Function

    Private Function serializeRegistry(registry As Dictionary(Of String, Integer)) As String
        If registry Is Nothing OrElse registry.Count = 0 Then Return String.Empty

        Dim lines As New List(Of String)(registry.Count)

        For Each item As KeyValuePair(Of String, Integer) In registry
            lines.Add(item.Key & ControlChars.Tab & item.Value.ToString(CultureInfo.InvariantCulture))
        Next

        Return String.Join(ControlChars.CrLf, lines)
    End Function

    Private Function parseRegistry(lines As String()) As Dictionary(Of String, Integer)
        Dim result As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        If lines Is Nothing Then Return result

        For Each line As String In lines
            If String.IsNullOrWhiteSpace(line) Then Continue For

            Dim parts As String() = line.Split(ControlChars.Tab)

            If parts.Length < 2 Then Continue For

            Dim index As Integer

            If Integer.TryParse(parts(1).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, index) Then
                result(parts(0).Trim()) = index
            End If
        Next

        Return result
    End Function

#End Region

#Region "ZIP 读写"

    Private Sub writeText(archive As ZipArchive, entryName As String, content As String)
        Dim entry As ZipArchiveEntry = archive.CreateEntry(entryName, CompressionLevel.Optimal)

        Using writer As New StreamWriter(entry.Open(), New UTF8Encoding(False))
            writer.Write(content)
        End Using
    End Sub

    Private Function readLines(archive As ZipArchive, entryName As String) As String()
        Dim entry As ZipArchiveEntry = archive.GetEntry(entryName)

        If entry Is Nothing Then Return Nothing

        Using reader As New StreamReader(entry.Open(), Encoding.UTF8)
            Dim text As String = reader.ReadToEnd()

            Return text.Replace(ControlChars.CrLf, ControlChars.Lf).Split(ControlChars.Lf)
        End Using
    End Function

    Private Function cleanLines(lines As String()) As String()
        If lines Is Nothing Then Return Nothing

        Return lines _
            .Where(Function(line) Not String.IsNullOrWhiteSpace(line)) _
            .Select(Function(line) line.Trim()) _
            .ToArray()
    End Function

#End Region

End Module
