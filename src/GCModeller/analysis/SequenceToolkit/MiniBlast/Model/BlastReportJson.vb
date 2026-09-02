' ============================================================================
' BlastReportJson.vb — 比对结果的 JSON 序列化 / 落盘 / 回读
' ----------------------------------------------------------------------------
' 原先序列化逻辑是 Program.SerializeReport（Private），自检无法复用导出链路。
' 抽出后「导出比对结果」成为被测代码路径本身，而不是测试内的私有逻辑。
'
' 序列化使用 System.Text.Json（BCL，零第三方依赖）。
' 注意：Model/*.vb 上的 <JsonPropertyName> 是下游 jq / Python 的解析契约，
' 不要改动。
' ============================================================================

Imports System.IO
Imports System.Text.Json
Imports System.Text.Json.Serialization

Namespace Model

    Public Module BlastReportJson

        ''' <summary>默认序列化选项：忽略 null（如 blastn 报告里不输出 matrix）</summary>
        Private ReadOnly Property DefaultOptions As JsonSerializerOptions
            Get
                Return New JsonSerializerOptions With {
                    .WriteIndented = False,
                    .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                }
            End Get
        End Property

        ''' <summary>序列化为 JSON 字符串</summary>
        ''' <param name="pretty">true 时缩进输出，便于人工查看</param>
        Public Function ToJson(report As BlastReport, Optional pretty As Boolean = False) As String
            Dim o = DefaultOptions
            If pretty Then o = New JsonSerializerOptions(o) With {.WriteIndented = True}
            Return JsonSerializer.Serialize(report, o)
        End Function

        ''' <summary>序列化并写入文件（自动创建目录），返回写入的绝对路径</summary>
        ''' <remarks>形参名不能叫 path：VB 标识符大小写不敏感，会遮蔽 System.IO.Path</remarks>
        Public Function Save(report As BlastReport, outputFile As String,
                             Optional pretty As Boolean = True) As String
            Dim full = System.IO.Path.GetFullPath(outputFile)
            Dim dir = System.IO.Path.GetDirectoryName(full)
            If Not String.IsNullOrEmpty(dir) AndAlso Not System.IO.Directory.Exists(dir) Then
                System.IO.Directory.CreateDirectory(dir)
            End If
            File.WriteAllText(full, ToJson(report, pretty))
            Return full
        End Function

        ''' <summary>从 JSON 文件回读（用于验证导出链路完整性）</summary>
        Public Function Load(jsonFile As String) As BlastReport
            Return JsonSerializer.Deserialize(Of BlastReport)(File.ReadAllText(jsonFile), DefaultOptions)
        End Function

        ''' <summary>从 JSON 字符串回读</summary>
        Public Function Parse(json As String) As BlastReport
            Return JsonSerializer.Deserialize(Of BlastReport)(json, DefaultOptions)
        End Function

    End Module

End Namespace
