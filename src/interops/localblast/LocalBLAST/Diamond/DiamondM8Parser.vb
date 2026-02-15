Imports System.Globalization
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Language

''' <summary>
''' 用于解析 DIAMOND .m8 格式文件的工具类
''' </summary>
Public Class DiamondM8Parser

    ''' <summary>
    ''' 解析指定的 .m8 文件并返回结果列表
    ''' </summary>
    ''' <param name="filePath">文件的完整路径</param>
    ''' <returns>包含 DiamondAnnotation 对象的列表</returns>
    Public Shared Iterator Function ParseFile(filePath As String) As IEnumerable(Of DiamondAnnotation)
        ' 检查文件是否存在
        If Not File.Exists(filePath) Then
            Call ("无法找到文件: " & filePath).warning
        Else
            Dim lineCounter As Integer = 0
            Dim hit As DiamondAnnotation
            Dim stream As IEnumerable(Of String)

            If filePath.FileLength > 512 * ByteSize.MB Then
                stream = TqdmStreamLoader(filePath)
            Else
                stream = SimpleStreamLoader(filePath)
            End If

            For Each line As String In stream
                lineCounter += 1
                hit = ParseLine(line, lineCounter)

                If Not hit Is Nothing Then
                    Yield hit
                End If
            Next
        End If
    End Function

    Private Shared Iterator Function TqdmStreamLoader(filepath As String) As IEnumerable(Of String)
        Dim s As Stream = filepath.Open(FileMode.Open, doClear:=False, [readOnly]:=True)

        For Each line As String In TextDoc.ReadLines(New StreamReader(s))
            ' 去除首尾空白
            line = line.Trim()

            ' 跳过空行
            If String.IsNullOrEmpty(line) Then
                Continue For
                ' 跳过注释行 (以 # 开头)
            ElseIf line.StartsWith("#") Then
                Continue For
            End If

            Yield line
        Next

        Try
            Call s.Dispose()
        Catch ex As Exception

        End Try
    End Function

    Private Shared Iterator Function SimpleStreamLoader(filepath As String) As IEnumerable(Of String)
        Dim line As Value(Of String) = ""

        Using reader As New StreamReader(filepath)
            ' 逐行读取
            While (line = reader.ReadLine()) IsNot Nothing
                ' 去除首尾空白
                line = line.Trim()

                ' 跳过空行
                If String.IsNullOrEmpty(line) Then
                    Continue While
                    ' 跳过注释行 (以 # 开头)
                ElseIf line.StartsWith("#") Then
                    Continue While
                End If

                Yield line
            End While
        End Using
    End Function

    Private Shared Function ParseLine(line As String, lineNum As Integer) As DiamondAnnotation
        ' 按制表符分割列
        Dim parts As String() = line.Split(ControlChars.Tab)

        ' .m8 标准格式应有 12 列
        If parts.Length <> 12 Then
            ' 可以在这里记录日志，或者直接跳过格式错误的行
            Call $"第 {lineNum} 行格式错误 (列数: {parts.Length})，已跳过。".warning
            Return Nothing
        End If

        ' 创建新对象并赋值
        Dim hit As New DiamondAnnotation()

        hit.QseqId = parts(0)
        hit.SseqId = parts(1)

        ' 使用 InvariantCulture 解析数字，以确保能正确处理小数点 "." 
        ' 以及科学计数法 (例如 2.31e-84)
        hit.Pident = Double.Parse(parts(2), CultureInfo.InvariantCulture)
        hit.Length = Integer.Parse(parts(3), CultureInfo.InvariantCulture)
        hit.Mismatch = Integer.Parse(parts(4), CultureInfo.InvariantCulture)
        hit.GapOpen = Integer.Parse(parts(5), CultureInfo.InvariantCulture)
        hit.QStart = Integer.Parse(parts(6), CultureInfo.InvariantCulture)
        hit.QEnd = Integer.Parse(parts(7), CultureInfo.InvariantCulture)
        hit.SStart = Integer.Parse(parts(8), CultureInfo.InvariantCulture)
        hit.SEnd = Integer.Parse(parts(9), CultureInfo.InvariantCulture)
        hit.EValue = Double.Parse(parts(10), CultureInfo.InvariantCulture)
        hit.BitScore = Double.Parse(parts(11), CultureInfo.InvariantCulture)

        Return hit
    End Function
End Class
