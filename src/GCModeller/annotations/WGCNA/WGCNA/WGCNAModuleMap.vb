Imports System.IO
Imports System.IO.Compression
Imports System.Security.Cryptography
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' WGCNA 模块映射的持久化缓存（边车文件）
''' </summary>
''' <remarks>
''' bicor 相关矩阵存储（<see cref="CorrelationMatrixStore"/>）缓存了原始的 N×N 带符号
''' 相关矩阵之后，先验调控网络构建（如 CellPhenote 的 <c>ExpressionGRNBuilder</c>
''' 存储驱动重载）从 WGCNA 结果中只消费 <c>Result.modules</c> 模块映射表——
''' hub 截断用连接度现算、跨模块边不构建，因此 <c>TOM</c>（N=5 万时约 10GB）与
''' <c>network</c> 图等大对象不需要、也不应该被缓存。
'''
''' 缓存的正确性依据：blockwise 预聚类的 <c>randomSeed</c> 默认固定（12345），
''' 同一份数据 + 同一套 WGCNA 配置 = 确定性的模块划分。
''' <see cref="fingerprint"/> 携带基因 ID 行序与 WGCNA 关键配置的哈希，
''' <see cref="Validate"/> 不匹配时调用方应放弃缓存并重新计算。
'''
''' 文件格式：Brotli 压缩的 JSON，原子写入（临时文件 + <see cref="File.Replace"/>）。
''' 推荐命名约定：与 bicor 相关矩阵存储配套，即 <c>{storeFile}.modules</c>；
''' 或在 R# <c>write_bicor(repo)</c> 约定下为 <c>{repo}/modules.dat</c>。
''' </remarks>
Public Class WGCNAModuleMap

    ''' <summary>
    ''' 基因 ID 表（行序与 bicor 相关矩阵存储的行序一致，指纹校验的依据之一）
    ''' </summary>
    ''' <returns></returns>
    Public Property genes As String() = {}

    ''' <summary>
    ''' WGCNA 模块划分结果：模块名（颜色）→ 基因 ID 列表
    ''' </summary>
    ''' <returns></returns>
    Public Property modules As Dictionary(Of String, String()) = New Dictionary(Of String, String())

    ''' <summary>
    ''' 缓存指纹（基因 ID 行序 + WGCNA 配置的哈希）
    ''' </summary>
    ''' <returns></returns>
    Public Property fingerprint As String = ""

    ''' <summary>缓存创建时间</summary>
    ''' <returns></returns>
    Public Property createdAt As Date = Now

    ''' <summary>
    ''' 从 WGCNA 分析结果提取模块映射
    ''' </summary>
    ''' <param name="wgcna">WGCNA 分析结果（要求 <see cref="Result.modules"/> 非空）</param>
    ''' <param name="fingerprint">由 <see cref="ComputeFingerprint"/> 生成的缓存指纹</param>
    ''' <returns></returns>
    Public Shared Function FromWGCNA(wgcna As Result, fingerprint As String) As WGCNAModuleMap
        If wgcna Is Nothing Then
            Throw New ArgumentNullException(NameOf(wgcna), "WGCNA 分析结果不能为空")
        End If
        If wgcna.modules Is Nothing OrElse wgcna.modules.Count = 0 Then
            Throw New InvalidOperationException("WGCNA 结果不包含模块划分信息（Result.modules 为空）")
        End If

        Return New WGCNAModuleMap With {
            .genes = wgcna.modules _
                .Values _
                .IteratesALL _
                .Distinct(StringComparer.OrdinalIgnoreCase) _
                .ToArray(),
            .modules = New Dictionary(Of String, String())(wgcna.modules, StringComparer.OrdinalIgnoreCase),
            .fingerprint = fingerprint,
            .createdAt = Now
        }
    End Function

    ''' <summary>
    ''' 校验缓存的基因表与指纹是否与当前数据匹配
    ''' </summary>
    ''' <param name="geneIds">当前数据的基因 ID 行序（例如 bicor store 的 <c>genes</c>）</param>
    ''' <param name="fingerprint">当前数据 + WGCNA 配置生成的指纹</param>
    ''' <returns>True 表示缓存可用</returns>
    Public Function Validate(geneIds As IEnumerable(Of String), fingerprint As String) As Boolean
        If String.IsNullOrEmpty(Me.fingerprint) OrElse String.IsNullOrEmpty(fingerprint) Then
            Return False
        End If
        If Not String.Equals(Me.fingerprint, fingerprint, StringComparison.Ordinal) Then
            Return False
        End If
        If Me.genes Is Nothing OrElse geneIds Is Nothing Then
            Return False
        End If
        If Me.genes.Length = 0 Then
            Return False
        End If

        ' 基因表必须逐行一致（行序参与指纹哈希，这里做二次显式校验）
        Dim current As String() = geneIds.ToArray()

        If current.Length <> Me.genes.Length Then
            Return False
        End If

        For i As Integer = 0 To current.Length - 1
            If Not String.Equals(current(i), Me.genes(i), StringComparison.OrdinalIgnoreCase) Then
                Return False
            End If
        Next

        Return modules IsNot Nothing AndAlso modules.Count > 0
    End Function

    ''' <summary>
    ''' 持久化缓存（Brotli 压缩 JSON，原子写）
    ''' </summary>
    ''' <param name="path">缓存文件路径</param>
    Public Sub Save(path As String)
        If String.IsNullOrEmpty(path) Then
            Throw New ArgumentException("缓存文件路径不能为空", NameOf(path))
        End If

        Dim json As String = Me.GetJson
        Dim tmp As String = path & ".tmp"
        Dim dir As String = IO.Path.GetDirectoryName(IO.Path.GetFullPath(path))

        If Not String.IsNullOrEmpty(dir) Then
            Call dir.MakeDir
        End If

        Using raw As New MemoryStream(System.Text.Encoding.UTF8.GetBytes(json))
            Using file As New FileStream(tmp, FileMode.Create, FileAccess.Write)
                Using brotli As New BrotliStream(file, CompressionLevel.Optimal)
                    Call raw.CopyTo(brotli)
                End Using
            End Using
        End Using

        ' 原子替换：防止写入中断留下损坏缓存
        If IO.File.Exists(path) Then
            Call IO.File.Replace(tmp, path, Nothing)
        Else
            Call IO.File.Move(tmp, path)
        End If

        Call $"module map cached: {modules.Count} modules, {genes.Length} genes -> '{path}'".info
    End Sub

    ''' <summary>
    ''' 读取缓存文件
    ''' </summary>
    ''' <param name="path">由 <see cref="Save"/> 写出的缓存文件路径</param>
    ''' <returns>缓存不存在或损坏时返回 Nothing（调用方据此回退重算）</returns>
    Public Shared Function Load(path As String) As WGCNAModuleMap
        If String.IsNullOrEmpty(path) OrElse Not IO.File.Exists(path) Then
            Return Nothing
        End If

        Try
            Using file As New FileStream(path, FileMode.Open, FileAccess.Read)
                Using brotli As New BrotliStream(file, CompressionMode.Decompress)
                    Using buf As New MemoryStream
                        Call brotli.CopyTo(buf)
                        Call buf.Seek(0, SeekOrigin.Begin)

                        Dim json As String = System.Text.Encoding.UTF8.GetString(buf.ToArray())
                        Dim map As WGCNAModuleMap = json.LoadJSON(Of WGCNAModuleMap)

                        If map Is Nothing OrElse map.modules Is Nothing OrElse map.modules.Count = 0 Then
                            Return Nothing
                        End If

                        Return map
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Call $"failed to load module map cache '{path}': {ex.Message} (cache will be rebuilt)".warning
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' 计算缓存指纹：基因 ID 行序 + WGCNA 关键配置的 SHA256
    ''' </summary>
    ''' <param name="geneIds">基因 ID（行序敏感）</param>
    ''' <param name="config">WGCNA 分析配置；为 Nothing 时按默认配置计算</param>
    ''' <returns>32 字符的十六进制指纹</returns>
    Public Shared Function ComputeFingerprint(geneIds As IEnumerable(Of String), config As WGCNAConfig) As String
        If config Is Nothing Then
            config = New WGCNAConfig()
        End If

        ' 影响模块划分结果的关键配置字段（新增影响划分的配置字段时需要同步到这里）
        Dim configText As String =
            $"power={config.power};minModuleSize={config.minModuleSize};deepSplit={config.deepSplit};" &
            $"mergeCutHeight={config.mergeCutHeight};maxBlockSize={config.maxBlockSize};randomSeed={config.randomSeed};" &
            $"treeCut={config.treeCut};adjacency={config.adjacency};cutHeight={config.cutHeight};" &
            $"pamStage={config.pamStage};distCut={config.distCut}"

        Dim sb As New System.Text.StringBuilder

        For Each gid As String In geneIds
            Call sb.Append(gid).Append("|"c)
        Next

        Call sb.Append("#"c).Append(configText)

        Dim hash As Byte() = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(sb.ToString()))
        Dim hex As New System.Text.StringBuilder(hash.Length * 2)

        For Each b As Byte In hash
            Call hex.Append(b.ToString("x2"))
        Next

        Return hex.ToString()
    End Function

    Public Overrides Function ToString() As String
        Return $"{If(String.IsNullOrEmpty(fingerprint), "", fingerprint.Substring(0, Math.Min(8, fingerprint.Length)))}...: " &
               $"{modules.Count} modules over {genes.Length} genes ({createdAt:yyyy-MM-dd HH:mm})"
    End Function
End Class
