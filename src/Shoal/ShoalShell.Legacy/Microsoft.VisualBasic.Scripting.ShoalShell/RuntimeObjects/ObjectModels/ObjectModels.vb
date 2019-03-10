Imports System.Text
Imports Microsoft.VisualBasic.Scripting.ShoalShell

Namespace Runtime.Objects.ObjectModels

    ''' <summary>
    ''' The parsed object model result for the target input script text.(对脚本进行解析之后，将会创建出一个脚本的对象模型)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ShellScript : Inherits ShoalShell.Runtime.Objects.ObjectModels.IScriptEngineComponent

        Implements IReadOnlyDictionary(Of String, ObjectModels.ScriptCodeLine)

        Dim _InternalScriptCodeLineHash As IReadOnlyDictionary(Of String, ScriptCodeLine)
        Protected _InternalEXEC_Pointer As Integer = 0

        ''' <summary>
        ''' This flag variable indicated that the "On Error Resume Next" option is setup or not, 
        ''' if TRUE then when an exception occurs, the shoal shell will record the exception 
        ''' information and then trying to ignored the error and continute running the script.
        ''' </summary>
        ''' <remarks></remarks>
        Dim _Flag_OnErrorResumeNext As Boolean
        Dim _ImportsNewNamespace As Boolean = False

#Region "ReadOnly Property"

        Public ReadOnly Property ImportsNewNamespace As Boolean
            Get
                Return ScriptEngine._ImportsNewNamespace
            End Get
        End Property

        Private ReadOnly Property HasImportsNamespaceStatements As Boolean
            Get
                Dim LQuery = (From item In Me._InternalScriptCodeLineHash Where item.Value.PreExecuteType = ScriptCodeLine.PreExecuteTypes.Imports Select item).ToArray
                Return Not LQuery.IsNullOrEmpty
            End Get
        End Property

        Public ReadOnly Property Values As IEnumerable(Of ScriptCodeLine) Implements IReadOnlyDictionary(Of String, ScriptCodeLine).Values
            Get
                Return Me._InternalScriptCodeLineHash.Values
            End Get
        End Property

        Default Public ReadOnly Property Item(key As String) As ScriptCodeLine Implements IReadOnlyDictionary(Of String, ScriptCodeLine).Item
            Get
                If Me._InternalScriptCodeLineHash.ContainsKey(key) Then
                    Return Me._InternalScriptCodeLineHash(key)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public ReadOnly Property Keys As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, ScriptCodeLine).Keys
            Get
                Return Me._InternalScriptCodeLineHash.Keys
            End Get
        End Property

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of KeyValuePair(Of String, ScriptCodeLine)).Count
            Get
                Return Me._InternalScriptCodeLineHash.Count
            End Get
        End Property
#End Region

        Sub New(CodeLines As Generic.IEnumerable(Of ScriptCodeLine), ScriptEngine As ShoalShell.Runtime.Objects.ShellScript)
            Call MyBase.New(ScriptEngine)
            Me._InternalScriptCodeLineHash = CodeLines.ToDictionary(Function(Line) Line.LineFlag)
        End Sub

#Const RELEASE = True

        Protected Const EXCEPTION_GOTO_FLAG_MISSING As String = "GOTO_FLAG_MISSING:: Could not found the goto flag ""{0}"":  {1};  Line: {2}"
        Protected Const FLAG_STATEMENT_RETURN_STACK As String = "Return.Stack"

        Public Function GetVariableType(key As String) As String
            Dim LQuery = (From item In Me.Values Where String.Equals(item.VariableAssigned, key, StringComparison.OrdinalIgnoreCase) Select item Order By item.LineNumber Descending).ToArray

            If LQuery.IsNullOrEmpty Then
                Return ""
            Else
                Return LQuery.First.ReturnType
            End If
        End Function

        Protected Function InternalExecuteGoto(CodeLine As ObjectModels.ScriptCodeLine) As Boolean
            If Not CodeLine.IsGotoStatement Then Return False
            If Not CodeLine.GotoCondition() = True Then Return False

            If _InternalScriptCodeLineHash.ContainsKey(CodeLine.GotoLineFlag) Then
                _InternalEXEC_Pointer = _InternalScriptCodeLineHash(CodeLine.GotoLineFlag).LineNumber - 1
            Else '没有找到相应的跳转标签，则抛出错误
                Dim ex As String = String.Format(EXCEPTION_GOTO_FLAG_MISSING,
                                                 CodeLine.GotoLineFlag,
                                                 CodeLine.OrignialScriptLine,
                                                 CodeLine.LineNumber)
                Throw New ArgumentException(ex)
            End If

            Return True
        End Function

        Protected Overridable Sub InternalExecuteScript(CodeLine As ObjectModels.ScriptCodeLine)
            If String.Equals(CodeLine.OrignialScriptLine, FLAG_STATEMENT_RETURN_STACK, StringComparison.OrdinalIgnoreCase) Then
                Call _InternalEXEC_Pointer.MoveNext()
                Return
            ElseIf InternalExecuteGoto(CodeLine) Then
                Return
            End If

            Dim Value As Object = CodeLine.InvokeMethod()
            Dim sys As Objects.I_MemoryManagementDevice = ScriptEngine._EngineMemoryDevice
            Call sys.InsertOrUpdate(CodeLine.VariableAssigned, Value)
        End Sub

        Private Sub InternalExecuteScript(CodeLines As ObjectModels.ScriptCodeLine())
            Do While _InternalEXEC_Pointer < _InternalScriptCodeLineHash.Count

                Dim CodeLine As ScriptCodeLine = CodeLines(_InternalEXEC_Pointer)
                Dim ConsoleTitle As String = String.Format("ShoalShell: @""{0}""", CodeLine.OrignialScriptLine)

                Call _Flag_OnErrorResumeNext.InvokeSet(If(_Flag_OnErrorResumeNext, True, CodeLine.OnErrorResumeNext))
                Call Console.Title.InvokeSet(ConsoleTitle)
                Call Debug.Print(CodeLine.ToString)

#If RELEASE Then
                Try
#End If
                    Call InternalExecuteScript(CodeLine)
                    If ExitScript() Then
                        Return
                    End If
#If RELEASE Then
                Catch ex As Exception
                    If Not _Flag_OnErrorResumeNext Then
                        Throw New Exceptions.ScriptRunTimeException("", ScriptEngine._EngineMemoryDevice) With
                              {
                                  .LineNumber = CodeLine.LineNumber,
                                  .ScriptLine = CodeLine.OrignialScriptLine,
                                  .InnerExceptionValue = ex.ToString
                              }
                    End If
                End Try
#End If
                Call _InternalEXEC_Pointer.MoveNext()
            Loop
        End Sub

        Protected Overridable Function ExitScript() As Boolean
            Return False
        End Function

        ''' <summary>
        ''' 执行脚本对象然后返回脚本的运行结果
        ''' </summary>
        ''' <returns></returns>
        Public Function Execute() As Object
            If Me.HasImportsNamespaceStatements Then
                Call Console.WriteLine()
            End If

            Call InternalExecuteScript(CodeLines:=_InternalScriptCodeLineHash.Values.ToArray)
            Call Console.Title.InvokeSet("Shoal Shell")
            Return ScriptEngine._EngineMemoryDevice("$")
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of KeyValuePair(Of String, ScriptCodeLine)) Implements IEnumerable(Of KeyValuePair(Of String, ScriptCodeLine)).GetEnumerator
            For Each Item As KeyValuePair(Of String, ScriptCodeLine) In Me._InternalScriptCodeLineHash
                Yield Item
            Next
        End Function

        Public Function ContainsKey(key As String) As Boolean Implements IReadOnlyDictionary(Of String, ScriptCodeLine).ContainsKey
            Return Me._InternalScriptCodeLineHash.ContainsKey(key)
        End Function

        Public Function TryGetValue(key As String, ByRef value As ScriptCodeLine) As Boolean Implements IReadOnlyDictionary(Of String, ScriptCodeLine).TryGetValue
            Return Me._InternalScriptCodeLineHash.TryGetValue(key, value)
        End Function

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace