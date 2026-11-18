Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.Json
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.FuzzyGene.Fitting

Namespace IO

    ''' <summary>
    ''' 神经模糊调控网络模型的压缩包持久化（<c>*.fuzzygene.zip</c>）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 包结构：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description><c>manifest.json</c>：格式版本、拟合路线、基因名、均值/标准差、
    ''' 稳态基线、支持域、配置与带符号边；</description></item>
    ''' <item><description><c>config.json</c>：本次拟合的完整配置（单独一份，方便直接肉眼检查
    ''' <c>maxRegulators</c> / <c>noveltyQuota</c> 等参数，不必在几十 MB 的 manifest 里翻）；</description></item>
    ''' <item><description><c>models/0000.json</c> …：逐靶基因模型（TSK 网络的 JSON + 指标）；</description></item>
    ''' <item><description><c>rules.txt</c> / <c>edges.sif</c> / <c>edges.csv</c>：给人看的可解释性导出。</description></item>
    ''' </list>
    ''' <para>
    ''' 读取后重建的对象是**纯推理对象**：不依赖训练现场，可直接做预测、稳态求解与虚拟扰动。
    ''' 之所以按基因分文件而不是塞进一个大数组：典型场景下每个基因的 TSK 模型都有几 KB，
    ''' 上千个基因时单个 JSON 会到几十 MB，分条目可以流式读取、也便于人工抽查。
    ''' </para>
    ''' <para>
    ''' 写入一律使用 <see cref="ZipArchive"/>（<see cref="ZipArchiveMode.Create"/>），
    ''' 不使用 <c>Microsoft.VisualBasic.ApplicationServices.Zip.ZipStream</c>：
    ''' 后者的文件索引在构造期一次性建立，新增条目不在索引里，二次读取会误判缺失。
    ''' </para>
    ''' </remarks>
    Public Module FuzzyGeneFile

        ''' <summary>模型包推荐的扩展名。</summary>
        Public Const PackageExtension As String = ".fuzzygene.zip"

        ''' <summary>清单条目名。</summary>
        Public Const EntryManifest As String = "manifest.json"

        ''' <summary>配置条目名（单独存一份，便于直接查看参数）。</summary>
        Public Const EntryConfig As String = "config.json"

        ''' <summary>逐基因模型的条目名前缀。</summary>
        Public Const EntryModels As String = "models/"

        ''' <summary>格式版本。</summary>
        Public Const FormatVersion As String = "1.0"

        ''' <summary>把模型写入文件。</summary>
        ''' <param name="network">已拟合的网络</param>
        ''' <param name="path">输出路径（通常以 <c>.fuzzygene.zip</c> 结尾）</param>
        <Extension>
        Public Sub Save(network As FuzzyGeneNetwork, path As String)
            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If
            If String.IsNullOrWhiteSpace(path) Then
                Throw New ArgumentException("the output path is empty", NameOf(path))
            End If

            Dim folder As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path))

            If Not String.IsNullOrEmpty(folder) AndAlso Not Directory.Exists(folder) Then
                Call Directory.CreateDirectory(folder)
            End If

            Using stream As New FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None)
                Call Save(network, stream)
            End Using
        End Sub

        ''' <summary>把模型写入流（流由调用方负责关闭）。</summary>
        <Extension>
        Public Sub Save(network As FuzzyGeneNetwork, stream As Stream)
            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If
            If stream Is Nothing Then
                Throw New ArgumentNullException(NameOf(stream))
            End If

            Using archive As New ZipArchive(stream, ZipArchiveMode.Create, leaveOpen:=True)
                Dim dto As FuzzyGeneNetworkDto = network.ToDto(includeModels:=False)

                dto.formatVersion = FormatVersion
                Call WriteText(archive, EntryManifest, JsonSerializer.Serialize(dto, FuzzyGeneNetwork.JsonOptions(True)))

                ' 配置单独一份：manifest 里虽然也有，但全量模型时 manifest 有几十 MB，
                ' 想确认 noveltyQuota / maxRegulators 之类的参数不该去翻那个文件
                Call WriteText(archive, EntryConfig,
                               JsonSerializer.Serialize(network.Config, FuzzyGeneNetwork.JsonOptions(True)))

                For i As Integer = 0 To network.Models.Length - 1
                    Dim model As GeneRegulationModel = network.Models(i)

                    If model Is Nothing Then
                        Continue For
                    End If

                    Call WriteText(archive, $"{EntryModels}{i.ToString("0000")}.json", model.ToJson(indented:=False))
                Next

                Call WriteText(archive, "rules.txt", String.Join(Environment.NewLine, network.RulesText()))
                Call WriteText(archive, "edges.sif", network.EdgesToSif())
                Call WriteText(archive, "edges.csv", network.EdgesToCsv())
            End Using
        End Sub

        ''' <summary>从文件读取模型。</summary>
        ''' <param name="path">模型包路径</param>
        ''' <returns>可直接推理的网络对象</returns>
        Public Function Load(path As String) As FuzzyGeneNetwork
            If String.IsNullOrWhiteSpace(path) Then
                Throw New ArgumentException("the package path is empty", NameOf(path))
            End If
            If Not System.IO.File.Exists(path) Then
                Throw New FileNotFoundException($"the fuzzy gene model package does not exist: {path}", path)
            End If

            Using stream As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
                Return Load(stream)
            End Using
        End Function

        ''' <summary>从流读取模型（流由调用方负责关闭）。</summary>
        Public Function Load(stream As Stream) As FuzzyGeneNetwork
            If stream Is Nothing Then
                Throw New ArgumentNullException(NameOf(stream))
            End If

            Using archive As New ZipArchive(stream, ZipArchiveMode.Read, leaveOpen:=True)
                Dim manifest As String = ReadText(archive, EntryManifest)

                If manifest Is Nothing Then
                    Throw New InvalidDataException($"not a fuzzy gene model package (missing {EntryManifest})")
                End If

                Dim dto As FuzzyGeneNetworkDto =
                    JsonSerializer.Deserialize(Of FuzzyGeneNetworkDto)(manifest, FuzzyGeneNetwork.JsonOptions(False))

                If dto Is Nothing OrElse dto.geneNames Is Nothing Then
                    Throw New InvalidDataException("the manifest of the fuzzy gene model package is incomplete")
                End If

                ' config.json 优先：它是一份独立、可直接查看的配置；老包里没有该条目时回退到 manifest
                Dim configJson As String = ReadText(archive, EntryConfig)

                If Not String.IsNullOrEmpty(configJson) Then
                    Try
                        Dim cfg As FuzzyGeneConfig = JsonSerializer.Deserialize(Of FuzzyGeneConfig)(
                            configJson, FuzzyGeneNetwork.JsonOptions(False))

                        If cfg IsNot Nothing Then
                            dto.config = cfg
                        End If
                    Catch ex As Exception
                        ' 配置反序列化失败不应让整个模型不可用（manifest 里还有一份）
                    End Try
                End If

                Dim models(dto.geneNames.Length - 1) As GeneModelDto

                For i As Integer = 0 To models.Length - 1
                    Dim text As String = ReadText(archive, $"{EntryModels}{i.ToString("0000")}.json")

                    If Not String.IsNullOrEmpty(text) Then
                        models(i) = JsonSerializer.Deserialize(Of GeneModelDto)(text, GeneRegulationModel.JsonOptions(False))
                    End If
                Next

                dto.models = models

                Return FuzzyGeneNetwork.FromDto(dto)
            End Using
        End Function

        ''' <summary>判断给定文件是否为本模块的模型包（只读清单条目，成本很低）。</summary>
        Public Function IsModelPackage(path As String) As Boolean
            If String.IsNullOrWhiteSpace(path) OrElse Not System.IO.File.Exists(path) Then
                Return False
            End If

            Try
                Using archive As ZipArchive = ZipFile.OpenRead(path)
                    Return archive.GetEntry(EntryManifest) IsNot Nothing
                End Using
            Catch ex As InvalidDataException
                Return False
            End Try
        End Function

        Private Sub WriteText(archive As ZipArchive, entryName As String, text As String)
            Dim entry As ZipArchiveEntry = archive.CreateEntry(entryName, CompressionLevel.Optimal)

            Using stream As Stream = entry.Open()
                Using writer As New StreamWriter(stream, New UTF8Encoding(False))
                    Call writer.Write(If(text, ""))
                End Using
            End Using
        End Sub

        Private Function ReadText(archive As ZipArchive, entryName As String) As String
            Dim entry As ZipArchiveEntry = archive.GetEntry(entryName)

            If entry Is Nothing Then
                Return Nothing
            End If

            Using stream As Stream = entry.Open()
                Using reader As New StreamReader(stream, Encoding.UTF8)
                    Return reader.ReadToEnd()
                End Using
            End Using
        End Function

    End Module
End Namespace
