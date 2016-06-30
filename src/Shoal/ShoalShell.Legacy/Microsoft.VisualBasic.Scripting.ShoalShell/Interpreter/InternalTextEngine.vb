Imports System.Text.RegularExpressions

Namespace Interpreter

    ''' <summary>
    ''' 内建的文本处理引擎
    ''' </summary>
    ''' <remarks></remarks>
    Public Module InternalTextEngine

        '使用正则表达式进行文本操作 
        'var <- regex expression <= variable
        'var <- "\d\s{5}" <= $text

        '使用文本引擎进行基本的处理
        'var <- "'tokens | first','mid 3 2'" <= $text

        Public Function SyntaxParser(Command As Microsoft.VisualBasic.CommandLine.CommandLine, MemoryDevice As ShoalShell.Runtime.Objects.I_MemoryManagementDevice) As ShoalShell.Runtime.Objects.ObjectModels.ScriptCodeLine

            If Command.Tokens.Count < 5 Then
                Return Nothing
            End If

            If Not (String.Equals(Command.Tokens(1), "<-") AndAlso String.Equals(Command.Tokens(3), "<=")) Then '不是文本处理命令
                Return Nothing
            End If

            Dim Script As String = Command.Tokens(2)
            Dim ScriptObject = Microsoft.VisualBasic.TextGrepScriptEngine.Compile(Script)
            Dim ScriptLine As ShoalShell.Runtime.Objects.ObjectModels.ScriptCodeLine =
                New Runtime.Objects.ObjectModels.ScriptCodeLine With
                {
                    .OrignialScriptLine = Command.CLICommandArgvs, .ReturnType = GetType(String).FullName, .VariableAssigned = Command.Tokens.First}
            Dim TextSource As String = Command.Tokens(4)

            If ScriptObject Is Nothing Then '正则表达式可能不可用被脚本引擎所正确的编译，则目标操作为正则表达式
                ScriptLine.InvokeMethod = Function() Regex.Match(input:=MemoryDevice.FormatString(TextSource), pattern:=Script).Value
            Else
                '使用内建的脚本引擎进行文本处理
                ScriptLine.InvokeMethod = Function() ScriptObject.Grep(MemoryDevice.FormatString(TextSource))
            End If

            Return ScriptLine
        End Function
    End Module
End Namespace