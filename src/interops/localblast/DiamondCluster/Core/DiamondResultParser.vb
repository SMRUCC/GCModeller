' ============================================================================
' DiamondResultParser.vb — DIAMOND 比对结果流式解析器
' ----------------------------------------------------------------------------
' 逐行读取 DIAMOND tabular 输出（--outfmt 6），解析字段并应用阈值过滤。
'
' 输出格式（每行 TAB 分隔）：
'   qseqid  sseqid  pident  qcovhsp  scovhsp
'   (int)   (int)   (float) (float)  (float)
'
' 过滤规则：
'   1. pident ≥ min_identity
'   2. qcovhsp ≥ min_coverage（查询序列覆盖度）
'   3. scovhsp ≥ min_coverage（目标序列覆盖度）
'   4. 跳过自比对（qseqid == sseqid）
'
' 流式处理：逐行读取，不加载整个文件到内存
' ============================================================================

Imports System.IO

Namespace Core

    ''' <summary>单条比对记录</summary>
    Public Structure AlignmentRecord

        ''' <summary>查询序列 ID</summary>
        Public Property QueryId As Integer

        ''' <summary>目标序列 ID</summary>
        Public Property SubjectId As Integer

        ''' <summary>序列相似性百分比</summary>
        Public Property PercentIdentity As Double

        ''' <summary>查询序列覆盖度百分比</summary>
        Public Property QueryCoverage As Double

        ''' <summary>目标序列覆盖度百分比</summary>
        Public Property SubjectCoverage As Double

        Public Overrides Function ToString() As String
            Return $"Q={QueryId} S={SubjectId} ID={PercentIdentity:F1}% QCov={QueryCoverage:F1}% SCov={SubjectCoverage:F1}%"
        End Function
    End Structure

    ''' <summary>
    ''' DIAMOND tabular 结果流式解析器。
    ''' 逐行读取，应用阈值过滤，生成比对记录。
    ''' </summary>
    Public Class DiamondResultParser
        Implements IDisposable

        Private ReadOnly _reader As StreamReader
        Private ReadOnly _minIdentity As Double
        Private ReadOnly _minCoverage As Double
        Private _disposed As Boolean = False

        ''' <summary>已解析的行数（含被过滤的）</summary>
        Private _totalLines As Long = 0

        ''' <summary>通过阈值的行数</summary>
        Private _passedLines As Long = 0

        ''' <summary>已解析的行数</summary>
        Public ReadOnly Property TotalLines As Long
            Get
                Return _totalLines
            End Get
        End Property

        ''' <summary>通过阈值的行数</summary>
        Public ReadOnly Property PassedLines As Long
            Get
                Return _passedLines
            End Get
        End Property

        ''' <summary>
        ''' 构造函数
        ''' </summary>
        ''' <param name="filePath">DIAMOND TSV 输出文件路径</param>
        ''' <param name="minIdentity">最小序列相似性（如 90.0 表示 90%）</param>
        ''' <param name="minCoverage">最小覆盖度（如 80.0 表示 80%）</param>
        Public Sub New(filePath As String, minIdentity As Double, minCoverage As Double)
            If Not File.Exists(filePath) Then
                Throw New FileNotFoundException($"DIAMOND 结果文件不存在: {filePath}")
            End If
            _reader = New StreamReader(filePath)
            _minIdentity = minIdentity
            _minCoverage = minCoverage
        End Sub

        ''' <summary>
        ''' 读取下一条通过阈值过滤的比对记录。
        ''' 自动跳过自比对和不符合阈值的行。
        ''' </summary>
        ''' <returns>AlignmentRecord 或 Nothing（文件结束）</returns>
        Public Function ReadNext() As AlignmentRecord?
            Dim line As String
            Do
                line = _reader.ReadLine()
                If line Is Nothing Then Return Nothing ' 文件结束
                _totalLines += 1

                ' 跳过空行
                If line.Length = 0 Then Continue Do

                ' 解析 TAB 分隔字段
                Dim fields = line.Split(ControlChars.Tab)
                If fields.Length < 5 Then Continue Do

                ' 解析整数 ID
                Dim qid, sid As Integer
                If Not Integer.TryParse(fields(0), qid) Then Continue Do
                If Not Integer.TryParse(fields(1), sid) Then Continue Do

                ' 跳过自比对
                If qid = sid Then Continue Do

                ' 解析浮点数值
                Dim pident, qcov, scov As Double
                If Not Double.TryParse(fields(2), pident) Then Continue Do
                If Not Double.TryParse(fields(3), qcov) Then Continue Do
                If Not Double.TryParse(fields(4), scov) Then Continue Do

                ' 应用阈值过滤
                ' 双重保险：DIAMOND 已用 --id 和 --query-cover 预过滤，
                ' 此处再检查 pident, qcov, scov 三项
                If pident < _minIdentity Then Continue Do
                If qcov < _minCoverage Then Continue Do
                If scov < _minCoverage Then Continue Do

                ' 通过所有过滤
                _passedLines += 1
                Return New AlignmentRecord With {
                    .QueryId = qid,
                    .SubjectId = sid,
                    .PercentIdentity = pident,
                    .QueryCoverage = qcov,
                    .SubjectCoverage = scov
                }
            Loop
        End Function

        ''' <summary>释放资源</summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            If _disposed Then Return
            _reader?.Dispose()
            _disposed = True
        End Sub

    End Class

End Namespace
