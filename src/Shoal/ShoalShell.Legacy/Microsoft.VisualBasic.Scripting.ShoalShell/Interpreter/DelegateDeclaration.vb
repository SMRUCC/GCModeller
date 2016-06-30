Imports System.Text.RegularExpressions
Imports System.Text

Namespace Interpreter

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Module DelegateDeclaration

        ''' <summary>
        ''' 在运行时通过API命令获取一个.NET函数的函数指针作为Delegate的语法:
        ''' 
        ''' Function &lt;- * command  
        ''' 
        ''' 函数会自动解析出参数信息，并且使用等式左端的变量名作为函数名
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Public Function InternalCreateEntryPoint(obj As Object, MemoryDevice As ShoalShell.Runtime.Objects.I_MemoryManagementDevice) As ShoalShell.DelegateHandlers.EntryPointHandlers.SignatureSignedFunctionEntryPoint
            Dim EntryPoint = GetDelegateInvokeEntryPoint(obj) '共享方法

            If EntryPoint Is Nothing Then
                Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.ScriptRunTimeException(String.Format("Target object ""{0}"" is not a function entry pointer!", obj.GetType.FullName), MemoryDevice)      '目标对象不是一个Delegate函数指针，则抛出错误
            Else
                Return ShoalShell.DelegateHandlers.EntryPointHandlers.SignatureSignedFunctionEntryPoint.CreateObject(EntryPoint, Nothing)
            End If
        End Function

        Const DELEGATE_REGEX As String = "^.+ <- \*$"
        Const HYBRID_SCRIPTING As String = "^.+ << \*$"

        '当调用外部脚本的时候的使用语法：
        'var << !script_environment_name $script_name [par1 $parvalue1 par2 $parvalue2] 

        'Delegate语法
        'delegate_name(parameterList) <- * 
        '{
        '}
        '假若不需求参数，则可以不添加括号
        '假若需求参数，则必须要使用括号，并且括号内的参数名之间使用逗号分隔开

        '<string> example(obj as text, obj2 as csv, obj3 as fasta) <- *

        ''' <summary>
        ''' 将Delegate解析出来后，替换掉Delegate的申明字符串为空字符串，脚本行在这里已经过Trim处理，并且移除了注释行
        ''' </summary>
        ''' <param name="ShellScript"></param>
        ''' <remarks></remarks>
        Public Function TryParse(ShellScript As String, EntryPointHandler As ShoalShell.DelegateHandlers.EntryPointHandlers.ImportsEntryPointManager) As String()
            Dim Tokens As List(Of String) = (From Token As String In Strings.Split(ShellScript.Replace(vbCr, ""), vbLf) Let s As String = Token.Trim Where Not String.IsNullOrEmpty(s) AndAlso s.First <> "#"c Select s).ToList '将脚本行进行分解
            Dim Declarations = (From Line As String In Tokens Where Regex.Match(Line, DELEGATE_REGEX, RegexOptions.Multiline).Success Select Line).ToArray

            If Declarations.IsNullOrEmpty Then
                Return Tokens.ToArray
            End If

            Dim Delegates As KeyValuePair(Of String, String)() = TryParseFunctionBody(Source:=Tokens, Declarations:=Declarations)

            For Each DeclaredDelegate As KeyValuePair(Of String, String) In Delegates
                Dim DelegateName As String = DeclaredDelegate.Key
                Dim [Delegate] As Reflection.Delegate = Reflection.Delegate.InternalCreateObject(DelegateName, DeclaredDelegate.Value, Nothing, Nothing, AddressOf EntryPointHandler.ScriptEngine.InternalSourceScript)

                Call EntryPointHandler.DeclaresDelegate([Delegate])
            Next

            Return TryParseHybridScript(Tokens, EntryPointHandler)
        End Function

        ''' <summary>
        ''' 解析混合编程的脚本块
        ''' </summary>
        ''' <param name="Source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function TryParseHybridScript(ByRef Source As List(Of String), EntryPointHandler As ShoalShell.DelegateHandlers.EntryPointHandlers.ImportsEntryPointManager) As String()
            Dim Declarations = (From Line As String In Source Where Regex.Match(Line, HYBRID_SCRIPTING, RegexOptions.Multiline).Success Select Line).ToArray

            If Declarations.IsNullOrEmpty Then
                Return Source.ToArray
            End If

            Dim HybridScriptBodies As KeyValuePair(Of String, String)() = TryParseFunctionBody(Source:=Source, Declarations:=Declarations)

            For Each DeclaredScript As KeyValuePair(Of String, String) In HybridScriptBodies
                Call EntryPointHandler.DeclaresHybridScripting(DeclaredScript.Key, DeclaredScript.Value)
            Next

            Return Source.ToArray
        End Function

        ''' <summary>
        ''' 返回{对象名, 对象脚本文本}
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function TryParseFunctionBody(ByRef Source As List(Of String), Declarations As String()) As KeyValuePair(Of String, String)()
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Dim ChunkBuffer As List(Of KeyValuePair(Of String, String)) = New Generic.List(Of KeyValuePair(Of String, String))

            For Each DeclaredHead As String In Declarations
                Dim p As Integer = Source.IndexOf(DeclaredHead)
                Dim strLine As String = Source(p + 1)

                If Not String.Equals(strLine, "{") Then
                    Throw New SyntaxErrorException(String.Format("Syntax error at declare delegate ""{0}""", DeclaredHead))
                End If

                Call Source.Remove(DeclaredHead)
                Call p.MoveNext()

                Dim TempTokensList As List(Of String) = New List(Of String)

                Do While Not String.Equals(strLine, "}")
                    If p = Source.Count Then
                        Dim Err As String = String.Format("Syntax error at declare delegate ""{0}"":" & vbCrLf & "Delegate bracket is not closed!", DeclaredHead)
                        Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.SyntaxErrorException(Err)
                    End If
                    strLine = Source(p.MoveNext)
                    Call TempTokensList.Add(strLine)
                Loop

                Call Source.RemoveAt(p - TempTokensList.Count - 1)
                For Each strLine In TempTokensList
                    Call Source.Remove(strLine)
                Next
                Call TempTokensList.Remove("}")

                Dim DelegateBuilder As StringBuilder = New StringBuilder(1024)
                For Each strLine In TempTokensList
                    Call DelegateBuilder.AppendLine(strLine)
                Next

                Dim DelegateName As String = Regex.Replace(DeclaredHead, "<-\s+\*", "").Trim

                Call ChunkBuffer.Add(New KeyValuePair(Of String, String)(DelegateName, DelegateBuilder.ToString))
            Next

            Return ChunkBuffer.ToArray
        End Function
    End Module
End Namespace