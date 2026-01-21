Imports System.IO
Imports System.Globalization
Imports System.Collections.Generic

''' <summary>
''' 用于解析 DIAMOND .m8 格式文件的工具类
''' </summary>
Public Class DiamondM8Parser
    ''' <summary>
    ''' 解析指定的 .m8 文件并返回结果列表
    ''' </summary>
    ''' <param name="filePath">文件的完整路径</param>
    ''' <returns>包含 DiamondAnnotation 对象的列表</returns>
    Public Shared Function ParseFile(ByVal filePath As String) As List(Of DiamondAnnotation)
        Dim results As New List(Of DiamondAnnotation)()

        ' 检查文件是否存在
        If Not File.Exists(filePath) Then
            Throw New FileNotFoundException("无法找到文件: " & filePath)
        End If

        Try
            ' 使用 StreamReader 读取文件
            Using reader As New StreamReader(filePath)
                Dim line As String
                Dim lineCounter As Integer = 0

                ' 逐行读取
                While (InlineAssignHelper(line, reader.ReadLine())) IsNot Nothing
                    lineCounter += 1

                    ' 去除首尾空白
                    line = line.Trim()

                    ' 跳过空行
                    If String.IsNullOrEmpty(line) Then Continue While

                    ' 跳过注释行 (以 # 开头)
                    If line.StartsWith("#") Then Continue While

                    ' 按制表符分割列
                    Dim parts As String() = line.Split(ControlChars.Tab)

                    ' .m8 标准格式应有 12 列
                    If parts.Length <> 12 Then
                        ' 可以在这里记录日志，或者直接跳过格式错误的行
                        debug.WriteLine($"第 {lineCounter} 行格式错误 (列数: {parts.Length})，已跳过。")
                        Continue While
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

                    results.Add(hit)
                End While
            End Using

        Catch ex As Exception
            ' 捕获 IO 异常或格式转换异常
            Throw New ApplicationException("解析文件时发生错误: " & ex.Message, ex)
        End Try

        Return results
    End Function

    ' VB.NET 辅助函数，用于在 While 循环中赋值
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function
End Class
