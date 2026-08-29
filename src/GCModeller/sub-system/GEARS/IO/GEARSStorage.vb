Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.IO
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
        ''' zip 包清单，序列化为 <c>manifest.json</c>
        ''' </summary>
        Friend Class Manifest
            ''' <summary>格式版本号</summary>
            ''' <returns>整数版本号</returns>
            Public Property formatVersion As Integer

            ''' <summary>保存时的 UTC 时间（yyyy-MM-dd HH:mm:ss）</summary>
            ''' <returns>时间字符串</returns>
            Public Property savedAt As String

            ''' <summary>基因数量</summary>
            ''' <returns>基因数</returns>
            Public Property nGenes As Integer

            ''' <summary>样本数量</summary>
            ''' <returns>样本数</returns>
            Public Property nSamples As Integer

            ''' <summary>超参配置</summary>
            ''' <returns><see cref="GEARSConfig"/> 实例</returns>
            Public Property config As GEARSConfig

            ''' <summary>基因身份嵌入维度</summary>
            ''' <returns>嵌入维度</returns>
            Public Property embeddingDim As Integer

            ''' <summary>图卷积隐藏层维度</summary>
            ''' <returns>隐藏维度</returns>
            Public Property hiddenDim As Integer

            ''' <summary>图卷积层数</summary>
            ''' <returns>层数</returns>
            Public Property numLayers As Integer

            ''' <summary>每个 epoch 的平均损失</summary>
            ''' <returns>损失曲线</returns>
            Public Property lossCurve As Double()

            ''' <summary>用于估计野生型基线的样本列索引</summary>
            ''' <returns>列索引数组</returns>
            Public Property baselineSamples As Integer()

            ''' <summary>模型可训练参数张量的个数，加载时用于校验</summary>
            ''' <returns>张量个数</returns>
            Public Property nParameters As Integer
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
        ''' <param name="info">清单数据</param>
        Friend Sub WriteManifest(zip As ZipArchive, info As Manifest)
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
        ''' <returns>清单数据</returns>
        Friend Function ReadManifest(zip As ZipArchive) As Manifest
            Dim json As String = String.Join(vbLf, ReadTextLines(zip, EntryManifest))
            Dim info As Manifest = json.LoadJSON(Of Manifest)

            If info Is Nothing Then
                Throw New InvalidDataException("GEARS 模型包的 manifest.json 解析失败")
            End If
            If info.formatVersion <> FormatVersion Then
                Throw New InvalidDataException(
                    $"GEARS 模型包格式版本不匹配：文件为 {info.formatVersion}，当前程序支持 {FormatVersion}")
            End If
            If info.config Is Nothing Then
                Throw New InvalidDataException("GEARS 模型包的 manifest.json 中缺少超参配置")
            End If

            Return info
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
        Friend Function ReadExpression(zip As ZipArchive) As Matrix
            Using fs As Stream = GetEntry(zip, EntryExpression).Open()
                Return BinaryMatrix.LoadStream(fs)
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
