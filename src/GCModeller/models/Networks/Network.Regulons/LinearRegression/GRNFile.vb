Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Namespace LinearRegression

    ''' <summary>
    ''' 已经拟合好的基因调控网络(<see cref="GeneRegulatoryNetwork"/>)的持久化存储
    ''' 
    ''' 网络对象会被保存成一个 ``zip`` 压缩包，其中包含如下的条目：
    ''' 
    ''' | 条目 | 内容 |
    ''' |------|------|
    ''' | ``meta.json`` | 格式标识、格式版本、基因/样本数量、交叉验证超参数等元信息 |
    ''' | ``options.json`` | 拟合参数(<see cref="GrnFittingOptions"/>)，用于重建标准化之后的表达向量与候选调控因子池 |
    ''' | ``expression.tsv`` | 原始的输入表达矩阵，每一行是 ``基因编号 [TAB] 样本1 [TAB] 样本2 ...`` |
    ''' | ``models.json`` | 每一个目标基因的线性模型(<see cref="GeneRegressionModel"/>) |
    ''' 
    ''' 表达矩阵与拟合参数被完整地保存下来，因此加载的时候可以重新构造出一个与保存之前
    ''' 完全一致的网络对象：<see cref="GeneRegulatoryNetwork.Vectors"/>、
    ''' <see cref="GeneRegulatoryNetwork.Baseline"/> 这些可以从原始数据推导出来的内容
    ''' 会被重新计算，而 <see cref="GeneRegulatoryNetwork.Models"/>、
    ''' <see cref="GeneRegulatoryNetwork.Regulations"/>、
    ''' <see cref="GeneRegulatoryNetwork.CoefMatrix"/> 这些分析接口可以直接使用，
    ''' 不需要重新做一次拟合。
    ''' </summary>
    ''' <remarks>
    ''' 压缩包是可以移植的：所有的数值都使用 <see cref="CultureInfo.InvariantCulture"/>
    ''' 的 ``R`` 格式写出，因此在任何区域设置之下都可以做到没有精度损失的往返。
    ''' </remarks>
    Public Module GRNFile

        ''' <summary>
        ''' 压缩包的格式标识，用于校验一个压缩包是否为基因调控网络的模型文件
        ''' </summary>
        Public Const FileFormat As String = "GCModeller.GRN.LinearRegression"

        ''' <summary>
        ''' 当前程序所支持的压缩包格式版本，读取到更新版本的模型文件的时候会拒绝加载
        ''' </summary>
        Public Const FileVersion As Integer = 1

        ''' <summary>
        ''' 元信息条目(``meta.json``)的名称
        ''' </summary>
        Public Const MetaEntry As String = "meta.json"

        ''' <summary>
        ''' 拟合参数条目(``options.json``)的名称
        ''' </summary>
        Public Const OptionsEntry As String = "options.json"

        ''' <summary>
        ''' 表达矩阵条目(``expression.tsv``)的名称
        ''' </summary>
        Public Const ExpressionEntry As String = "expression.tsv"

        ''' <summary>
        ''' 逐基因的线性模型条目(``models.json``)的名称
        ''' </summary>
        Public Const ModelEntry As String = "models.json"

        ''' <summary>
        ''' 把已经拟合好的基因调控网络保存到 ``zip`` 流之中
        ''' 
        ''' 传入的表达矩阵、拟合参数、每一个基因的模型(包括拟合失败的模型以及
        ''' 它们的错误信息)都会被写入压缩包，因此可以完整地还原出一个网络对象。
        ''' </summary>
        ''' <param name="model">已经完成拟合的调控网络</param>
        ''' <param name="zip">目标压缩包流，方法返回之后该流仍然保持打开状态</param>
        <Extension>
        Public Sub Save(model As GeneRegulatoryNetwork, zip As Stream)
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If
            If zip Is Nothing Then
                Throw New ArgumentNullException(NameOf(zip))
            End If

            Dim matrix As Matrix = model.Expression
            Dim fitted As GeneRegressionModel() = model.Models.ToArray()
            Dim meta As New GrnFileMeta With {
                .format = FileFormat,
                .version = FileVersion,
                .created = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ssK", CultureInfo.InvariantCulture),
                .method = model.Options.method.ToString(),
                .genes = model.Size,
                .samples = matrix.sampleID.Length,
                .fitted = fitted.Where(Function(m) m IsNot Nothing AndAlso m.IsValid).Count(),
                .expressionTag = matrix.tag,
                .sampleID = matrix.sampleID,
                .cvAlpha = model.CrossValidationAlpha,
                .cvL1Ratio = model.CrossValidationL1Ratio
            }

            Using archive As New ZipArchive(zip, ZipArchiveMode.Create, leaveOpen:=True)
                Call WriteJSON(archive, MetaEntry, meta)
                Call WriteJSON(archive, OptionsEntry, model.Options)
                Call WriteExpression(archive, matrix, meta.samples)
                Call WriteJSON(archive, ModelEntry, fitted)
            End Using
        End Sub

        ''' <summary>
        ''' 把已经拟合好的基因调控网络保存到指定的模型文件(``*.grn.zip``)之中
        ''' </summary>
        ''' <param name="model">已经完成拟合的调控网络</param>
        ''' <param name="path">输出文件的路径，所在的文件夹不存在的时候会被自动创建</param>
        <Extension>
        Public Sub Save(model As GeneRegulatoryNetwork, path As String)
            If String.IsNullOrEmpty(path) Then
                Throw New ArgumentNullException(NameOf(path))
            End If

            ' 参数 path 会遮蔽掉 Path 类型(VB 的标识符不区分大小写)，这里必须写全名
            Dim dir As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path))

            If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            Using file As New FileStream(path, FileMode.Create, FileAccess.Write)
                Call model.Save(file)
            End Using
        End Sub

        ''' <summary>
        ''' 从 ``zip`` 流之中重建一个完整的基因调控网络对象
        ''' 
        ''' 表达矩阵与拟合参数会被用来重新构造网络对象本身(标准化之后的表达向量、
        ''' 基线表达量以及候选调控因子池都会被重新计算)，然后再把压缩包之中
        ''' 保存的拟合结果与交叉验证超参数回填进去。加载之后的网络对象与保存之前的
        ''' 对象在数值上是完全一致的，可以直接用于后续的分析工作。
        ''' </summary>
        ''' <param name="zip">模型压缩包的流，方法返回之后该流仍然保持打开状态</param>
        ''' <returns>已经完成拟合的调控网络</returns>
        ''' <exception cref="InvalidDataException">压缩包的内容不是有效的模型文件</exception>
        Public Function Load(zip As Stream) As GeneRegulatoryNetwork
            If zip Is Nothing Then
                Throw New ArgumentNullException(NameOf(zip))
            End If

            Using archive As New ZipArchive(zip, ZipArchiveMode.Read, leaveOpen:=True)
                Dim meta As GrnFileMeta = ReadJSON(Of GrnFileMeta)(archive, MetaEntry)

                If meta Is Nothing Then
                    Throw New InvalidDataException($"the grn archive contains no '{MetaEntry}' entry data!")
                End If
                If Not String.Equals(meta.format, FileFormat, StringComparison.Ordinal) Then
                    Throw New InvalidDataException($"the given stream is not a '{FileFormat}' archive, the format tag is '{meta.format}'!")
                End If
                If meta.version <= 0 OrElse meta.version > FileVersion Then
                    Throw New InvalidDataException($"the model file version ({meta.version}) is not supported by this program, up to version {FileVersion}!")
                End If

                Dim fittingOptions As GrnFittingOptions = ReadJSON(Of GrnFittingOptions)(archive, OptionsEntry)
                Dim matrix As Matrix = ReadExpression(archive, meta)
                Dim fitted As GeneRegressionModel() = ReadJSON(Of GeneRegressionModel())(archive, ModelEntry)
                Dim model As New GeneRegulatoryNetwork(matrix, If(fittingOptions, New GrnFittingOptions()))

                Call model.RestoreModels(fitted, meta.cvAlpha, meta.cvL1Ratio)

                Return model
            End Using
        End Function

        ''' <summary>
        ''' 从指定的模型文件(``*.grn.zip``)之中重建一个完整的基因调控网络对象
        ''' </summary>
        ''' <param name="path">模型文件的路径</param>
        ''' <returns>已经完成拟合的调控网络</returns>
        ''' <exception cref="InvalidDataException">文件的内容不是有效的模型文件</exception>
        Public Function Load(path As String) As GeneRegulatoryNetwork
            If String.IsNullOrEmpty(path) Then
                Throw New ArgumentNullException(NameOf(path))
            End If
            If Not File.Exists(path) Then
                Throw New FileNotFoundException($"the given grn model file does not exists: {path}", path)
            End If

            Using file As New FileStream(path, FileMode.Open, FileAccess.Read)
                Return Load(DirectCast(file, Stream))
            End Using
        End Function

        ''' <summary>
        ''' 把一个对象以(json)文本的方式写入到压缩包的指定条目之中
        ''' </summary>
        ''' <param name="archive">目标压缩包</param>
        ''' <param name="name">条目的名称</param>
        ''' <param name="data">需要序列化的对象</param>
        Private Sub WriteJSON(archive As ZipArchive, name As String, data As Object)
            Dim entry As ZipArchiveEntry = archive.CreateEntry(name, CompressionLevel.Optimal)

            Using stream As Stream = entry.Open()
                Using writer As New StreamWriter(stream, New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False))
                    Call writer.Write(GetObjectJson(data, indent:=True))
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 从压缩包的指定条目之中反序列化出一个对象
        ''' </summary>
        ''' <typeparam name="T">目标类型</typeparam>
        ''' <param name="archive">目标压缩包</param>
        ''' <param name="name">条目的名称</param>
        ''' <returns></returns>
        ''' <exception cref="InvalidDataException">条目缺失或者内容为空</exception>
        Private Function ReadJSON(Of T)(archive As ZipArchive, name As String) As T
            Dim entry As ZipArchiveEntry = GetEntry(archive, name)

            If entry Is Nothing Then
                Throw New InvalidDataException($"the given stream is not a valid grn archive: the '{name}' entry is missing!")
            End If

            Dim json As String

            Using stream As Stream = entry.Open()
                Using reader As New StreamReader(stream, Encoding.UTF8)
                    json = reader.ReadToEnd()
                End Using
            End Using

            If String.IsNullOrWhiteSpace(json) Then
                Throw New InvalidDataException($"the given stream is not a valid grn archive: the '{name}' entry is empty!")
            End If

            Return CType(json.LoadObject(GetType(T)), T)
        End Function

        ''' <summary>
        ''' 把原始的输入表达矩阵写入到压缩包之中
        ''' 
        ''' 每一行是 ``基因编号 [TAB] 样本1 [TAB] 样本2 ...``，数值使用
        ''' ``R`` 格式(可以精确往返)并且固定使用 <see cref="CultureInfo.InvariantCulture"/>，
        ''' 与运行环境的区域设置无关。
        ''' </summary>
        ''' <param name="archive">目标压缩包</param>
        ''' <param name="matrix">表达矩阵</param>
        ''' <param name="nSamples">样本数量</param>
        Private Sub WriteExpression(archive As ZipArchive, matrix As Matrix, nSamples As Integer)
            Dim entry As ZipArchiveEntry = archive.CreateEntry(ExpressionEntry, CompressionLevel.Optimal)
            Dim rows As DataFrameRow() = matrix.expression

            If rows Is Nothing Then
                Throw New ArgumentException("the expression matrix contains no gene expression data!", NameOf(matrix))
            End If

            Using stream As Stream = entry.Open()
                Using writer As New StreamWriter(stream, New UTF8Encoding(encoderShouldEmitUTF8Identifier:=False))
                    For Each row As DataFrameRow In rows
                        Dim values As Double() = row.experiments

                        If String.IsNullOrEmpty(row.geneID) Then
                            Throw New ArgumentException("the expression matrix contains a gene without any reference id!", NameOf(matrix))
                        End If
                        If row.geneID.IndexOf(vbTab) > -1 OrElse row.geneID.IndexOf(ControlChars.Cr) > -1 OrElse row.geneID.IndexOf(ControlChars.Lf) > -1 Then
                            Throw New ArgumentException(
                                $"the gene id '{row.geneID}' contains a tab or a line break character, which can not be saved as a text based expression matrix!",
                                NameOf(matrix))
                        End If
                        If values Is Nothing OrElse values.Length <> nSamples Then
                            Throw New ArgumentException(
                                $"the expression vector size of gene '{row.geneID}' is mis-matched with the sample size {nSamples}!",
                                NameOf(matrix))
                        End If

                        Call writer.Write(row.geneID)

                        For i As Integer = 0 To values.Length - 1
                            Call writer.Write(vbTab)
                            Call writer.Write(values(i).ToString("R", CultureInfo.InvariantCulture))
                        Next

                        Call writer.WriteLine()
                    Next
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 从压缩包之中读取原始的输入表达矩阵
        ''' </summary>
        ''' <param name="archive">目标压缩包</param>
        ''' <param name="meta">压缩包之中的元信息，样本编号从这里读取</param>
        ''' <returns></returns>
        ''' <exception cref="InvalidDataException">表达矩阵条目的内容不完整或者格式不正确</exception>
        Private Function ReadExpression(archive As ZipArchive, meta As GrnFileMeta) As Matrix
            Dim entry As ZipArchiveEntry = GetEntry(archive, ExpressionEntry)

            If entry Is Nothing Then
                Throw New InvalidDataException($"the given stream is not a valid grn archive: the '{ExpressionEntry}' entry is missing!")
            End If
            If meta.sampleID Is Nothing OrElse meta.sampleID.Length = 0 Then
                Throw New InvalidDataException($"the given stream is not a valid grn archive: no sample id was declared in the '{MetaEntry}' entry!")
            End If

            Dim nSamples As Integer = meta.sampleID.Length
            Dim rows As New List(Of DataFrameRow)
            Dim lineNo As Integer = 0

            Using stream As Stream = entry.Open()
                Using reader As New StreamReader(stream, Encoding.UTF8)
                    Do While Not reader.EndOfStream
                        Dim line As String = reader.ReadLine()

                        lineNo += 1

                        If lineNo = 1 AndAlso line.Length > 0 AndAlso line(0) = ChrW(&HFEFF) Then
                            ' 容忍外部工具重新保存过、带上了 BOM 的文本
                            line = line.Substring(1)
                        End If
                        If String.IsNullOrEmpty(line) Then
                            Continue Do
                        End If

                        Dim values As String() = line.Split(New Char() {ControlChars.Tab})

                        If values.Length <> nSamples + 1 Then
                            Throw New InvalidDataException(
                                $"the {lineNo} line of the '{ExpressionEntry}' entry contains {values.Length - 1} expression value(s), " &
                                $"but {nSamples} sample(s) are declared in the '{MetaEntry}' entry!")
                        End If

                        Dim expression As Double() = New Double(nSamples - 1) {}

                        For i As Integer = 0 To nSamples - 1
                            Dim text As String = values(i + 1)

                            If Not Double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, expression(i)) Then
                                Throw New InvalidDataException(
                                    $"invalid expression value '{text}' was found at the {lineNo} line of the '{ExpressionEntry}' entry!")
                            End If
                        Next

                        Call rows.Add(New DataFrameRow With {
                            .geneID = values(0),
                            .experiments = expression
                        })
                    Loop
                End Using
            End Using

            If rows.Count = 0 Then
                Throw New InvalidDataException($"the '{ExpressionEntry}' entry of the grn archive contains no gene expression data!")
            End If
            If meta.genes > 0 AndAlso rows.Count <> meta.genes Then
                Throw New InvalidDataException(
                    $"the grn archive is damaged: {rows.Count} gene expression vector(s) were found, but {meta.genes} gene(s) are declared!")
            End If

            Return New Matrix With {
                .tag = meta.expressionTag,
                .sampleID = meta.sampleID,
                .expression = rows.ToArray()
            }
        End Function

        ''' <summary>
        ''' 在压缩包之中查找指定名称的条目(不区分大小写)
        ''' </summary>
        ''' <param name="archive">目标压缩包</param>
        ''' <param name="name">条目的名称</param>
        ''' <returns>不存在的时候返回空引用</returns>
        Private Function GetEntry(archive As ZipArchive, name As String) As ZipArchiveEntry
            For Each entry As ZipArchiveEntry In archive.Entries
                If String.Equals(entry.FullName, name, StringComparison.OrdinalIgnoreCase) Then
                    Return entry
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' 模型压缩包之中的元信息(``meta.json``)
        ''' </summary>
        Friend Class GrnFileMeta

            ''' <summary>
            ''' 格式标识，固定为 <see cref="GRNFile.FileFormat"/>
            ''' </summary>
            ''' <returns></returns>
            Public Property format As String

            ''' <summary>
            ''' 压缩包的格式版本
            ''' </summary>
            ''' <returns></returns>
            Public Property version As Integer

            ''' <summary>
            ''' 模型文件的创建时间
            ''' </summary>
            ''' <returns></returns>
            Public Property created As String

            ''' <summary>
            ''' 创建当前模型所使用的拟合方法(<see cref="FittingMethod"/> 的名称)
            ''' </summary>
            ''' <returns></returns>
            Public Property method As String

            ''' <summary>
            ''' 表达矩阵之中的基因数量
            ''' </summary>
            ''' <returns></returns>
            Public Property genes As Integer

            ''' <summary>
            ''' 表达矩阵之中的样本数量
            ''' </summary>
            ''' <returns></returns>
            Public Property samples As Integer

            ''' <summary>
            ''' 拟合成功的基因模型的数量
            ''' </summary>
            ''' <returns></returns>
            Public Property fitted As Integer

            ''' <summary>
            ''' 输入表达矩阵的标签
            ''' </summary>
            ''' <returns></returns>
            Public Property expressionTag As String

            ''' <summary>
            ''' 样本编号列表，列顺序与表达矩阵一致
            ''' </summary>
            ''' <returns></returns>
            Public Property sampleID As String()

            ''' <summary>
            ''' 全局交叉验证模式之下解析出来的共享 alpha，小于等于零表示没有启用该模式
            ''' </summary>
            ''' <returns></returns>
            Public Property cvAlpha As Double

            ''' <summary>
            ''' 全局交叉验证模式之下解析出来的共享 L1 混合比例，小于等于零表示没有启用该模式
            ''' </summary>
            ''' <returns></returns>
            Public Property cvL1Ratio As Double
        End Class
    End Module
End Namespace
