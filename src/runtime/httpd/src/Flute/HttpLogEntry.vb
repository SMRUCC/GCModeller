Imports System.Globalization
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language

Public Class HttpLogEntry

    Public Property RemoteIp As String
    Public Property Ident As String
    Public Property RemoteUser As String
    Public Property LogTime As DateTimeOffset?
    Public Property HttpMethod As String
    Public Property RequestUrl As String
    Public Property HttpProtocol As String
    Public Property StatusCode As Short
    Public Property ResponseBytes As Long?       ' Nothing = 日志中的 '-'
    Public Property Referer As String
    Public Property UserAgent As String
    Public Property RawLog As String

    ' ========== 1. 正则模式：Apache Combined Log Format ==========
    ' 说明：VB.NET 中 "" 表示一个双引号字符
    Public Const LogPattern As String =
        "^(?<ip>\S+)\s" &
        "(?<ident>\S+)\s" &
        "(?<user>\S+)\s" &
        "\[(?<time>[^\]]+)\]\s" &
        """(?<request>[^""]*)""\s" &
        "(?<status>\d{3})\s" &
        "(?<size>\d+|-)\s" &
        """(?<referer>[^""]*)""\s" &
        """(?<ua>[^""]*)"""

    ' 预编译 Regex（线程安全，可复用）
    Shared ReadOnly LogRegex As New Regex(LogPattern, RegexOptions.Compiled)

    ' ========== 3. 解析单行日志 ==========
    Public Shared Function ParseLine(line As String) As HttpLogEntry
        If String.IsNullOrWhiteSpace(line) Then Return Nothing

        Dim m As Match = LogRegex.Match(line)
        If Not m.Success Then Return Nothing   ' 格式不符，跳过

        Dim entry As New HttpLogEntry With {
            .RemoteIp = m.Groups("ip").Value,
            .Ident = m.Groups("ident").Value,
            .RemoteUser = m.Groups("user").Value,
            .StatusCode = Short.Parse(m.Groups("status").Value),
            .Referer = If(m.Groups("referer").Value = "-", Nothing, m.Groups("referer").Value),
            .UserAgent = If(m.Groups("ua").Value = "-", Nothing, m.Groups("ua").Value),
            .RawLog = line
        }

        ' 时间戳解析（处理时区偏移 +0000 → +00:00）
        entry.LogTime = ParseApacheTimestamp(m.Groups("time").Value)

        ' 响应字节数（'-' → Nothing，对应数据库 NULL）
        Dim sizeStr = m.Groups("size").Value
        If sizeStr <> "-" Then
            Dim b As Long
            If Long.TryParse(sizeStr, b) Then entry.ResponseBytes = b
        End If

        ' 拆分请求行：METHOD URL PROTOCOL
        Dim reqParts = m.Groups("request").Value.Split(" "c)
        Select Case reqParts.Length
            Case >= 3
                entry.HttpMethod = reqParts(0)
                ' URL 中可能含空格（极少见），取中间所有部分
                entry.RequestUrl = String.Join(" "c, reqParts, 1, reqParts.Length - 2)
                entry.HttpProtocol = reqParts(reqParts.Length - 1)
            Case 2
                entry.HttpMethod = reqParts(0)
                entry.RequestUrl = reqParts(1)
            Case 1
                entry.RequestUrl = reqParts(0)
        End Select

        Return entry
    End Function

    ' ========== 4. 时间戳解析（关键：Apache 时区无冒号，.NET 需要有冒号） ==========
    Private Shared Function ParseApacheTimestamp(raw As String) As DateTimeOffset?
        If String.IsNullOrWhiteSpace(raw) Then Return Nothing

        ' 将 "+0000" 规范为 "+00:00"
        Dim normalized = raw
        Dim lastSpace = raw.LastIndexOf(" "c)
        If lastSpace > 0 AndAlso raw.Length - lastSpace - 1 = 5 Then
            Dim off = raw.Substring(lastSpace + 1)
            If off(0) = "+"c OrElse off(0) = "-"c Then
                normalized = raw.Substring(0, lastSpace + 1) &
                             off.Substring(0, 3) & ":" & off.Substring(3)
            End If
        End If

        Dim dto As DateTimeOffset
        If DateTimeOffset.TryParseExact(
            normalized,
            "dd/MMM/yyyy:HH:mm:ss zzz",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            dto) Then
            Return dto
        End If
        Return Nothing
    End Function

    ' ========== 5. 批量解析文件 ==========
    Public Shared Iterator Function ParseApacheLogFile(logFilePath As String) As IEnumerable(Of HttpLogEntry)
        Dim failedCount As Integer = 0

        Using sr As New IO.StreamReader(logFilePath, Text.Encoding.UTF8)
            Dim lineNo As Integer = 0
            Dim line As Value(Of String) = ""

            Do While Not (line = sr.ReadLine) Is Nothing
                lineNo += 1

                Try
                    Dim e = ParseLine(line)

                    If e IsNot Nothing Then
                        Yield e
                    Else
                        failedCount += 1
                        Call $"[SKIP] 第 {lineNo} 行格式异常: {CStr(line).Substring(0, Math.Min(80, CStr(line).Length))}...".debug
                    End If
                Catch ex As Exception
                    failedCount += 1
                    Call $"[ERR ] 第 {lineNo} 行解析异常: {ex.Message}".error
                End Try
            Loop
        End Using

        Call $"解析完成: 成功 {list.Count} 条，跳过/失败 {failedCount} 条".info
    End Function
End Class
