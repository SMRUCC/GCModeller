Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.GEARS.Model
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace IO

    ''' <summary>
    ''' GEARS 模型的 zip 持久化编解码模块
    ''' </summary>
    ''' <remarks>
    ''' 一个 zip 包内包含五个条目：
    ''' <list type="bullet">
    ''' <item><description><c>manifest.json</c> —— 格式版本、超参配置、模型结构、损失曲线、基线样本索引；</description></item>
    ''' <item><description><c>prior.csv</c> —— 先验调控网络（文本，便于人工查看与编辑）；</description></item>
    ''' <item><description><c>expression.bin</c> —— 完整表达矩阵（复用 <see cref="BinaryMatrix"/> 的网络字节序格式）；</description></item>
    ''' <item><description><c>baseline.bin</c> —— 野生型均值与标准差；</description></item>
    ''' <item><description><c>model.bin</c> —— 全部可训练参数张量。</description></item>
    ''' </list>
    '''
    ''' 图结构不单独落盘：<see cref="Graph.GeneRegulatoryGraph"/> 在给定「基因名列表 + 先验网络 + 表达矩阵 +
    ''' 共表达配置」时构建结果完全确定，加载时按同样入参重建即可，既省空间也避免图与数据不同步。
    '''
    ''' 注意：<see cref="BinaryMatrix.Save"/> 与 <see cref="BinaryMatrix.LoadStream"/> 内部都会 dispose
    ''' 传入的流，而 zip 条目的流本来就必须关闭才会落盘，因此二者语义吻合；但这也意味着
    ''' <strong>同一个条目只能写一次</strong>，不可追加。
    ''' </remarks>
    Friend Module GEARSStorage

        ''' <summary>当前 zip 包格式版本号；加载时不一致会直接报错</summary>
        Friend Const FormatVersion As Integer = 1

        ''' <summary>条目名：格式版本与超参清单</summary>
        Friend Const EntryManifest As String = "manifest.json"

        ''' <summary>条目名：先验调控网络 CSV</summary>
        Friend Const EntryPrior As String = "prior.csv"

        ''' <summary>条目名：表达矩阵二进制</summary>
        Friend Const EntryExpression As String = "expression.bin"

        ''' <summary>条目名：野生型基线（均值与标准差）</summary>
        Friend Const EntryBaseline As String = "baseline.bin"

        ''' <summary>条目名：模型可训练参数</summary>
        Friend Const EntryModel As String = "model.bin"

        ''' <summary>
        ''' zip 包清单的字段键名集合
        ''' </summary>
        ''' <remarks>
        ''' 清单以 <c>Dictionary(Of String, String)</c> 的形式序列化为 <c>manifest.json</c>，
        ''' 而不是定义一个专用的清单类型：sciBASIC 的 JSON 序列化底层走
        ''' <c>DataContractJsonSerializer</c>，要求被序列化的类型必须是 public，
        ''' 而本模块属于内部实现细节（Friend），用字典可以完全避免为此暴露公开类型。
        ''' </remarks>
        Friend Class ManifestKeys
            ''' <summary>格式版本号</summary>
            Friend Const formatVersion As String = "formatVersion"
            ''' <summary>保存时间</summary>
            Friend Const savedAt As String = "savedAt"
            ''' <summary>基因数量</summary>
            Friend Const nGenes As String = "nGenes"
            ''' <summary>样本数量</summary>
            Friend Const nSamples As String = "nSamples"
            ''' <summary>超参配置（内嵌 JSON 字符串）</summary>
            Friend Const config As String = "config"
            ''' <summary>基因身份嵌入维度</summary>
            Friend Const embeddingDim As String = "embeddingDim"
            ''' <summary>图卷积隐藏层维度</summary>
            Friend Const hiddenDim As String = "hiddenDim"
            ''' <summary>图卷积层数</summary>
            Friend Const numLayers As String = "numLayers"
            ''' <summary>损失曲线（逗号分隔）</summary>
            Friend Const lossCurve As String = "lossCurve"
            ''' <summary>基线样本列索引（逗号分隔）</summary>
            Friend Const baselineSamples As String = "baselineSamples"
            ''' <summary>模型参数张量个数</summary>
            Friend Const nParameters As String = "nParameters"
        End Class

        ' ==================== 条目读写 ====================

        ''' <summary>
        ''' 创建 zip 条目并打开其写入流
        ''' </summary>
        ''' <param name="zip">目标 zip 归档</param>
        ''' <param name="entryName">条目名</param>
        ''' <returns>条目的写入流；调用方负责 Using 释放</returns>
        Private Function OpenEntry(zip As ZipArchive, entryName As String) As Stream
            Dim entry As ZipArchiveEntry = zip.CreateEntry(entryName, CompressionLevel.Optimal)

            Return entry.Open()
        End Function

        ''' <summary>
        ''' 读取 zip 条目的全部文本内容
        ''' </summary>
        ''' <param name="zip">zip 归档</param>
        ''' <param name="entryName">条目名</param>
        ''' <returns>文本行数组</returns>
        Private Function ReadTextLines(zip As ZipArchive, entryName As String) As String()
            Dim entry As ZipArchiveEntry = GetEntry(zip, entryName)
            Dim lines As New List(Of String)()

            Using reader As New StreamReader(entry.Open(), Encoding.UTF8)
                Do While Not reader.EndOfStream
                    lines.Add(reader.ReadLine())
                Loop
            End Using

            Return lines.ToArray()
        End Function

        ''' <summary>
        ''' 按名称取 zip 条目，不存在时抛出友好异常
        ''' </summary>
        ''' <param name="zip">zip 归档</param>
        ''' <param name="entryName">条目名</param>
        ''' <returns>条目对象</returns>
        Private Function GetEntry(zip As ZipArchive, entryName As String) As ZipArchiveEntry
            Dim entry As ZipArchiveEntry = zip.GetEntry(entryName)

            If entry Is Nothing Then
                Throw New InvalidDataException($"zip 包中缺少必需的条目 ""{entryName}""，该文件可能不是有效的 GEARS 模型包")
            End If

            Return entry
        End Function

        ' ==================== manifest.json ====================

        ''' <summary>
        ''' 写入清单条目
        ''' </summary>
        ''' <param name="zip">目标 zip 归档</param>
        ''' <param name="config">超参配置</param>
        ''' <param name="model">模型（提供结构信息）</param>
        ''' <param name="nGene">基因数量</param>
        ''' <param name="nSample">样本数量</param>
        ''' <param name="lossCurve">损失曲线</param>
        ''' <param name="baselineSamples">基线样本列索引</param>
        ''' <param name="nParameters">模型参数张量个数</param>
        Friend Sub WriteManifest(zip As ZipArchive,
                                 config As GEARSConfig,
                                 model As GEARSModel,
                                 nGene As Integer,
                                 nSample As Integer,
                                 lossCurve As Double(),
                                 baselineSamples As Integer(),
                                 nParameters As Integer)

            Dim info As New Dictionary(Of String, String) From {
                {ManifestKeys.formatVersion, FormatVersion.ToString()},
                {ManifestKeys.savedAt, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")},
                {ManifestKeys.nGenes, nGene.ToString()},
                {ManifestKeys.nSamples, nSample.ToString()},
                {ManifestKeys.config, config.GetJson},
                {ManifestKeys.embeddingDim, model.EmbeddingDim.ToString()},
                {ManifestKeys.hiddenDim, model.HiddenDim.ToString()},
                {ManifestKeys.numLayers, model.NumLayers.ToString()},
                {ManifestKeys.lossCurve, JoinNumbers(lossCurve)},
                {ManifestKeys.baselineSamples, JoinNumbers(baselineSamples)},
                {ManifestKeys.nParameters, nParameters.ToString()}
            }

            Using fs As Stream = OpenEntry(zip, EntryManifest)
                Using writer As New StreamWriter(fs, New UTF8Encoding(False))
                    Call writer.Write(info.GetJson)
                    Call writer.Flush()
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 读取并校验清单条目
        ''' </summary>
        ''' <param name="zip">zip 归档</param>
        ''' <param name="config">返回超参配置</param>
        ''' <param name="lossCurve">返回损失曲线</param>
        ''' <param name="baselineSamples">返回基线样本列索引</param>
        ''' <param name="nParameters">返回模型参数张量个数</param>
        ''' <returns>清单字典，便于调用方读取结构字段</returns>
        Friend Function ReadManifest(zip As ZipArchive,
                                     ByRef config As GEARSConfig,
                                     ByRef lossCurve As Double(),
                                     ByRef baselineSamples As Integer(),
                                     ByRef nParameters As Integer) As Dictionary(Of String, String)

            Dim json As String = String.Join(vbLf, ReadTextLines(zip, EntryManifest))
            Dim info As Dictionary(Of String, String) = json.LoadJSON(Of Dictionary(Of String, String))

            If info Is Nothing Then
                Throw New InvalidDataException("GEARS 模型包的 manifest.json 解析失败")
            End If

            Dim version As Integer = ReadInt(info, ManifestKeys.formatVersion)

            If version <> FormatVersion Then
                Throw New InvalidDataException(
                    $"GEARS 模型包格式版本不匹配：文件为 {version}，当前程序支持 {FormatVersion}")
            End If

            Dim configJson As String = ReadString(info, ManifestKeys.config)
            config = configJson.LoadJSON(Of GEARSConfig)

            If config Is Nothing Then
                Throw New InvalidDataException("GEARS 模型包的 manifest.json 中缺少或无法解析超参配置")
            End If

            lossCurve = ParseDoubles(ReadString(info, ManifestKeys.lossCurve))
            baselineSamples = ParseIntegers(ReadString(info, ManifestKeys.baselineSamples))
            nParameters = ReadInt(info, ManifestKeys.nParameters)

            Return info
        End Function

        ''' <summary>
        ''' 从清单字典中读取必填字符串字段
        ''' </summary>
        ''' <param name="info">清单字典</param>
        ''' <param name="key">字段键名</param>
        ''' <returns>字段值</returns>
        Private Function ReadString(info As Dictionary(Of String, String), key As String) As String
            Dim value As String = Nothing

            If info.TryGetValue(key, value) Then
                Return value
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' 从清单字典中读取必填整数字段
        ''' </summary>
        ''' <param name="info">清单字典</param>
        ''' <param name="key">字段键名</param>
        ''' <returns>字段值</returns>
        Private Function ReadInt(info As Dictionary(Of String, String), key As String) As Integer
            Dim text As String = ReadString(info, key)
            Dim value As Integer

            If Integer.TryParse(text, value) Then
                Return value
            End If

            Throw New InvalidDataException($"GEARS 模型包的 manifest.json 中字段 ""{key}"" 缺失或不是整数")
        End Function

        ''' <summary>
        ''' 把双精度数组拼成逗号分隔的字符串（固定使用不变文化，避免区域设置影响小数点）
        ''' </summary>
        ''' <param name="values">数值数组</param>
        ''' <returns>逗号分隔文本；空数组返回空串</returns>
        Private Function JoinNumbers(values As Double()) As String
            If values.IsNullOrEmpty Then
                Return ""
            End If

            Dim list As New List(Of String)()

            For Each v As Double In values
                list.Add(v.ToString("R", System.Globalization.CultureInfo.InvariantCulture))
            Next

            Return String.Join(",", list)
        End Function

        ''' <summary>
        ''' 把整数数组拼成逗号分隔的字符串
        ''' </summary>
        ''' <param name="values">整数数组</param>
        ''' <returns>逗号分隔文本；空数组返回空串</returns>
        Private Function JoinNumbers(values As Integer()) As String
            If values.IsNullOrEmpty Then
                Return ""
            End If

            Dim list As New List(Of String)()

            For Each v As Integer In values
                list.Add(v.ToString(System.Globalization.CultureInfo.InvariantCulture))
            Next

            Return String.Join(",", list)
        End Function

        ''' <summary>
        ''' 解析逗号分隔的双精度数组
        ''' </summary>
        ''' <param name="text">逗号分隔文本</param>
        ''' <returns>双精度数组；文本为空时返回空数组</returns>
        Private Function ParseDoubles(text As String) As Double()
            If String.IsNullOrWhiteSpace(text) Then
                Return New Double() {}
            End If

            Dim tokens As String() = text.Split(","c)
            Dim result As New List(Of Double)()

            For Each token As String In tokens
                Dim v As Double

                If Double.TryParse(token,
                                   System.Globalization.NumberStyles.Float,
                                   System.Globalization.CultureInfo.InvariantCulture, v) Then
                    result.Add(v)
                End If
            Next

            Return result.ToArray()
        End Function

        ''' <summary>
        ''' 解析逗号分隔的整数数组
        ''' </summary>
        ''' <param name="text">逗号分隔文本</param>
        ''' <returns>整数数组；文本为空时返回空数组</returns>
        Private Function ParseIntegers(text As String) As Integer()
            If String.IsNullOrWhiteSpace(text) Then
                Return New Integer() {}
            End If

            Dim tokens As String() = text.Split(","c)
            Dim result As New List(Of Integer)()

            For Each token As String In tokens
                Dim v As Integer

                If Integer.TryParse(token, v) Then
                    result.Add(v)
                End If
            Next

            Return result.ToArray()
        End Function

        ' ==================== prior.csv ====================

        ''' <summary>
        ''' 写入先验调控网络
        ''' </summary>
        ''' <param name="zip">目标 zip 归档</param>
        ''' <param name="prior">先验调控网络</param>
        Friend Sub WritePrior(zip As ZipArchive, prior As PriorNetwork)
            Using fs As Stream = OpenEntry(zip, EntryPrior)
                Using writer As New StreamWriter(fs, New UTF8Encoding(False))
                    For Each line As String In PriorNetworkIO.PriorNetworkToCsv(prior)
                        Call writer.WriteLine(line)
                    Next

                    Call writer.Flush()
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 读取先验调控网络
        ''' </summary>
        ''' <param name="zip">zip 归档</param>
        ''' <returns>先验调控网络</returns>
        Friend Function ReadPrior(zip As ZipArchive) As PriorNetwork
            Dim lines As String() = ReadTextLines(zip, EntryPrior)

            Return BnIO.ReadPriorNetwork(PriorNetworkIO.ParseRegulatoryEdges(lines))
        End Function

        ' ==================== expression.bin ====================

        ''' <summary>
        ''' 写入完整表达矩阵
        ''' </summary>
        ''' <param name="zip">目标 zip 归档</param>
        ''' <param name="expr">表达矩阵（行=基因，列=样本）</param>
        Friend Sub WriteExpression(zip As ZipArchive, expr As Matrix)
            Using fs As Stream = OpenEntry(zip, EntryExpression)
                Call BinaryMatrix.Save(expr, fs)
            End Using
        End Sub

        ''' <summary>
        ''' 读取完整表达矩阵
        ''' </summary>
        ''' <param name="zip">zip 归档</param>
        ''' <returns>表达矩阵</returns>
        ''' <remarks>
        ''' <see cref="BinaryMatrix.LoadStream"/> 内部依赖 <c>Stream.Length</c> 与 <c>Stream.Position</c>
        ''' 判断数据块是否读完，而 zip 条目的解压流（<c>DeflateStream</c>）不支持取长度，
        ''' 直接传入会抛 <see cref="NotSupportedException"/>。因此这里先把条目内容缓冲到
        ''' <see cref="MemoryStream"/> 再解码——这样既复用了既有的二进制格式，又不必改动共享运行时代码。
        ''' </remarks>
        Friend Function ReadExpression(zip As ZipArchive) As Matrix
            Using raw As Stream = GetEntry(zip, EntryExpression).Open()
                Using buffer As New MemoryStream()
                    Call raw.CopyTo(buffer)
                    Call buffer.Seek(0, SeekOrigin.Begin)

                    Return BinaryMatrix.LoadStream(buffer)
                End Using
            End Using
        End Function

        ' ==================== baseline.bin ====================

        ''' <summary>
        ''' 写入一个双精度向量（长度 + 数据）
        ''' </summary>
        ''' <param name="zip">目标 zip 归档</param>
        ''' <param name="entryName">条目名</param>
        ''' <param name="x">待写入的向量</param>
        Friend Sub WriteVector(zip As ZipArchive, entryName As String, x As Double())
            Using fs As Stream = OpenEntry(zip, entryName)
                Using writer As New BinaryWriter(fs, Encoding.UTF8, leaveOpen:=True)
                    Call writer.Write(x.Length)

                    For Each v As Double In x
                        Call writer.Write(v)
                    Next

                    Call writer.Flush()
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 读取一个双精度向量
        ''' </summary>
        ''' <param name="zip">zip 归档</param>
        ''' <param name="entryName">条目名</param>
        ''' <returns>向量</returns>
        Friend Function ReadVector(zip As ZipArchive, entryName As String) As Double()
            Using fs As Stream = GetEntry(zip, entryName).Open()
                Using reader As New BinaryReader(fs, Encoding.UTF8, leaveOpen:=True)
                    Dim n As Integer = reader.ReadInt32()
                    Dim x As Double() = New Double(n - 1) {}

                    For i As Integer = 0 To n - 1
                        x(i) = reader.ReadDouble()
                    Next

                    Return x
                End Using
            End Using
        End Function

        ' ==================== model.bin ====================

        ''' <summary>
        ''' 写入模型的全部可训练参数张量
        ''' </summary>
        ''' <param name="zip">目标 zip 归档</param>
        ''' <param name="parameters">参数张量列表（顺序即恢复顺序）</param>
        Friend Sub WriteTensors(zip As ZipArchive, parameters As List(Of Tensor))
            Using fs As Stream = OpenEntry(zip, EntryModel)
                Using writer As New BinaryWriter(fs, Encoding.UTF8, leaveOpen:=True)
                    Call writer.Write(parameters.Count)

                    For Each t As Tensor In parameters
                        Call writer.Write(t.Rank)

                        For Each d As Integer In t.Shape
                            Call writer.Write(d)
                        Next

                        For Each v As Double In t.Data
                            Call writer.Write(v)
                        Next
                    Next

                    Call writer.Flush()
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 读取参数张量并原地写回目标张量
        ''' </summary>
        ''' <param name="zip">zip 归档</param>
        ''' <param name="parameters">
        ''' 目标张量列表；其个数与每个张量的形状必须与保存时完全一致，
        ''' 否则抛出 <see cref="InvalidDataException"/>
        ''' </param>
        Friend Sub ReadTensors(zip As ZipArchive, parameters As List(Of Tensor))
            Using fs As Stream = GetEntry(zip, EntryModel).Open()
                Using reader As New BinaryReader(fs, Encoding.UTF8, leaveOpen:=True)
                    Dim n As Integer = reader.ReadInt32()

                    If n <> parameters.Count Then
                        Throw New InvalidDataException(
                            $"模型参数张量数量不匹配：zip 中有 {n} 个，当前模型结构期望 {parameters.Count} 个")
                    End If

                    For i As Integer = 0 To n - 1
                        Dim rank As Integer = reader.ReadInt32()
                        Dim shape As Integer() = New Integer(rank - 1) {}

                        For k As Integer = 0 To rank - 1
                            shape(k) = reader.ReadInt32()
                        Next

                        Dim target As Tensor = parameters(i)

                        If target.Rank <> rank OrElse Not target.Shape.SequenceEqual(shape) Then
                            Throw New InvalidDataException(
                                $"第 {i} 个参数张量形状不匹配：zip 中为 [{String.Join(",", shape)}]，" &
                                $"当前模型结构期望 [{String.Join(",", target.Shape)}]")
                        End If

                        Dim data As Double() = target.Data

                        For k As Integer = 0 To data.Length - 1
                            data(k) = reader.ReadDouble()
                        Next
                    Next
                End Using
            End Using
        End Sub
    End Module
End Namespace
