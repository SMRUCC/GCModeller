
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM.Expressions

Namespace Interpreter

    ''' <summary>
    ''' 匿名函数
    ''' </summary>
    Public Class AnonymousDelegate : Inherits Runtime.SCOM.RuntimeComponent
        Implements IReadOnlyDictionary(Of String, LDM.SyntaxModel)

        Public ReadOnly Delegates As SortedDictionary(Of String, LDM.SyntaxModel) =
            New SortedDictionary(Of String, SyntaxModel)
        Public ReadOnly Property TempDelegate As SortedDictionary(Of String, LDM.SyntaxModel) =
            New SortedDictionary(Of String, SyntaxModel)


        Sub New(ScriptEngine As Runtime.ScriptEngine)
            Call MyBase.New(ScriptEngine)
        End Sub

        ''' <summary>
        ''' 当切换目录之后扫描当前目录之下的所有的临时命令脚本
        ''' </summary>
        Public Sub CdTemp()
            Dim Files As Dictionary(Of String, String) =
                FileIO.FileSystem.CurrentDirectory _
               .LoadSourceEntryList(ScriptEngine.Config.GetExtensionList, True)

            Call _TempDelegate.Clear()

            If Files.IsNullOrEmpty Then
                Return
            End If

            Dim LQuery = (From path In Files.AsParallel
                          Let script = ___load(path.Value)
                          Where Not script Is Nothing
                          Select path.Key.ToLower,
                              script).ToArray

            For Each script In LQuery
                Call _TempDelegate.Add(script.ToLower, script.script)
            Next
        End Sub

        Const _1MB As Long = 1024 * 1024

        Private Function ___load(path As String) As LDM.SyntaxModel
            Try
                If FileIO.FileSystem.GetFileInfo(path).Length > _1MB Then  ' 文件太大了，可能不是脚本文件而仅仅是普通的文本类型的数据文件，放弃解析，否则程序会被卡在这里
                    Return Nothing
                End If
                Return LDM.SyntaxModel.LoadFile(path)
            Catch ex As Exception
                Return Nothing  '不是脚本文件，则不必被加入临时命令之中了
            End Try
        End Function

        Public Sub [Declare](Name As String, Func As LDM.SyntaxModel)
            Name = Name.ToLower
            If Delegates.ContainsKey(Name) Then
                Call Delegates.Remove(Name) '新的会替换掉旧的
            End If
            Call Delegates.Add(Name, Func)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Expr"></param>
        ''' <param name="success">这个变量主要是用来指示是否找到了相应的函数入口点</param>
        ''' <returns></returns>
        Public Function Exec(ExecName As String, Expr As LDM.Expressions.FunctionCalls, ByRef success As Boolean) As Object
            Dim Func = Me(ExecName)
            success = Not Func Is Nothing

            If Not success Then Return Nothing

            Dim params = ScriptEngine.ExecuteModel.ArgumentsLinker.GetParameters(Expr)

            For Each arg In params
                Call ScriptEngine.MMUDevice.Update(arg.Key, arg.Value)
            Next

            Dim value As Object = New Runtime.FSMMachine(Me.ScriptEngine, Func).Execute
            Return value
        End Function

#Region "Implements IReadOnlyDictionary(Of String, LDM.SyntaxModel)"

        Public ReadOnly Property NumbersOfDelegate As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of String, SyntaxModel)).Count
            Get
                Return Delegates.Count
            End Get
        End Property

        ''' <summary>
        ''' 会在两个字典之中进行查找，由于可能会因为不小心重名的缘故会造成临时脚本被执行，产生无法察觉的BUG，所以在这里临时脚本命令的优先级是最低的
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property GetScriptDelegate(key As String) As SyntaxModel Implements IReadOnlyDictionary(Of String, SyntaxModel).Item
            Get
                If String.IsNullOrEmpty(key) Then
                    Return Nothing
                End If

                key = key.ToLower
                If Delegates.ContainsKey(key) Then
                    Return Delegates(key)
                Else
                    If _TempDelegate.ContainsKey(key) Then
                        Return _TempDelegate(key)
                    Else
                        Return Nothing
                    End If
                End If
            End Get
        End Property

        Public ReadOnly Property Keys As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, SyntaxModel).Keys
            Get
                Return Delegates.Keys
            End Get
        End Property

        Public ReadOnly Property Values As IEnumerable(Of SyntaxModel) Implements IReadOnlyDictionary(Of String, SyntaxModel).Values
            Get
                Return Delegates.Values
            End Get
        End Property

        Public Function ContainsKey(key As String) As Boolean Implements IReadOnlyDictionary(Of String, SyntaxModel).HaveOperon
            Return Delegates.ContainsKey(key.ToLower)
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, SyntaxModel)) Implements IEnumerable(Of KeyValuePair(Of String, SyntaxModel)).GetEnumerator
            For Each __func In Delegates
                Yield __func
            Next
        End Function

        Public Function TryGetValue(key As String, ByRef value As SyntaxModel) As Boolean Implements IReadOnlyDictionary(Of String, SyntaxModel).TryGetValue
            Return Delegates.TryGetValue(key.ToLower, value)
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region
    End Class
End Namespace