Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices.Debugging

''' <summary>
''' 测试输出的目录定位与报告落盘。
''' </summary>
Friend Module Report

    ''' <summary>
    ''' 定位输出目录：从程序集所在目录向上查找 <c>test.vbproj</c>，输出到其下的 <c>output</c> 目录。
    ''' </summary>
    Public Function ResolveOutputDirectory() As String
        Dim folder As New DirectoryInfo(AppContext.BaseDirectory)

        While folder IsNot Nothing
            If File.Exists(Path.Combine(folder.FullName, "test.vbproj")) Then
                Return ensure(Path.Combine(folder.FullName, "output"))
            End If

            folder = folder.Parent
        End While

        Return ensure(Path.Combine(AppContext.BaseDirectory, "output"))
    End Function

    Private Function ensure(folder As String) As String
        If Not Directory.Exists(folder) Then Call Directory.CreateDirectory(folder)

        Return folder
    End Function

    ''' <summary>
    ''' 把审计报告写入文本与 JSON 两种格式，并在控制台打印摘要。
    ''' </summary>
    ''' <param name="folder">输出目录。</param>
    Public Function Save(folder As String) As (text As String, json As String)
        Dim text As String = DebugAudit.Dump(includeRecords:=True)
        Dim json As String = DebugAudit.ToJson(indent:=True)
        Dim textPath As String = Path.Combine(folder, "audit_report.txt")
        Dim jsonPath As String = Path.Combine(folder, "audit_report.json")

        Call File.WriteAllText(textPath, text)
        Call File.WriteAllText(jsonPath, json)

        Return (textPath, jsonPath)
    End Function

    ''' <summary>
    ''' 在控制台打印一行带对齐的分节标题。
    ''' </summary>
    ''' <param name="title">标题文本。</param>
    Public Sub Section(title As String)
        Console.WriteLine()
        Console.WriteLine(New String("-"c, 72))
        Console.WriteLine($" {title}")
        Console.WriteLine(New String("-"c, 72))
    End Sub

End Module
