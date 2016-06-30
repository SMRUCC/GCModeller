Imports System.Text

Namespace HTML

    ''' <summary>
    ''' Internal wiki system queriable object.(这个对象是可以接受wiki查询操作的)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IWikiHandle

        Function GenerateDescription() As String
        ''' <summary>
        ''' 模糊匹配并返回匹配结果，当返回空字符串的时候，则说明没有被匹配上
        ''' </summary>
        ''' <param name="keyword"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Match(keyword As String) As String
    End Interface

    Public Class Wiki : Inherits Runtime.SCOM.RuntimeComponent

        Sub New(ScriptEngine As Runtime.ScriptEngine)
            Call MyBase.New(ScriptEngine)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj">所需要搜索帮助信息的对象的名称</param>
        ''' <returns></returns>
        Public Function WikiSearchView(obj As String) As String()
            Dim Result = HandleWikiSearch(obj)
            Return (From element In Result Select element.GenerateDescription).ToArray
        End Function

        Public Function HandleWikiSearch(obj As String) As IWikiHandle()

        End Function

        ''' <summary>
        ''' 不带任何参数的wiki命令，显示概览信息
        ''' </summary>
        ''' <returns></returns>
        Public Function WikiHelp() As String
            Return __listAllImportsAPI()
        End Function

        ''' <summary>
        ''' 打印出所有导入的API的信息
        ''' </summary>
        ''' <returns></returns>
        Private Function __listAllImportsAPI() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Dim ImportsCommands = ScriptEngine.Interpreter.EPMDevice.ImportedAPI
            Dim CommandNameMaxLength As Integer = (From strKey As String In ImportsCommands.Keys Select Len(strKey)).ToArray.Max

            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine("System Basically Command & Currently Imported Commands" & vbCrLf)
            Call sBuilder.AppendLine(vbCrLf & String.Format("     {0} Command(s)", ImportsCommands.Count))
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine(String.Format("  MethodEntry{0}    Return Type", New String(" "c, CommandNameMaxLength)))
            Call sBuilder.AppendLine(String.Format("+---{0}+----------------------------------------------------", New String("-"c, 1.5 * CommandNameMaxLength - 3)))

            For Each MethodEntryPoint In ImportsCommands
                Call sBuilder.AppendLine(String.Format("  {0}{1} {2} {3}", MethodEntryPoint.Key, New String(" "c, 1.5 * CommandNameMaxLength - Len(MethodEntryPoint.Key)),
                                                                           MethodEntryPoint.Value.First.EntryPoint.EntryPoint.ReturnType.FullName,
                                                                           If(MethodEntryPoint.Value.IsOverloaded, String.Format("(+ {0} overloads)", MethodEntryPoint.Value.OverloadsNumber), "")))
            Next

            Dim TempShell = ScriptEngine.Interpreter.EPMDevice.AnonymousDelegate.TempDelegate

            If Not TempShell.IsNullOrEmpty Then

                Call sBuilder.AppendLine()
                Call sBuilder.AppendLine(String.Format("    {0} Temp Shell Command(s) available in current work directory" & vbCrLf, TempShell.Count))
                Call sBuilder.AppendLine("-Name---------------------------File----------------------")

                CommandNameMaxLength = (From strKey As String In TempShell.Keys Select Len(strKey)).ToArray.Max

                For Each ShellEntry In TempShell
                    Call sBuilder.AppendLine(String.Format(" {0}  {1} {2}", ShellEntry.Key, New String(" "c, CommandNameMaxLength - Len(ShellEntry.Key)), ShellEntry.Value.FilePath))
                Next
            End If

            Call Console.WriteLine(sBuilder.ToString)

            Return sBuilder.ToString
        End Function

    End Class
End Namespace