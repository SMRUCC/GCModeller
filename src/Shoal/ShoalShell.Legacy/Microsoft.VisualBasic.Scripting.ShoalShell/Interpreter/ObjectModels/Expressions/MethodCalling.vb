Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.ObjectModels.Tokens

Namespace Interpreter.ObjectModels.Statements

    ''' <summary>
    ''' 只是调用方法，函数的返回值直接返回给系统变量$
    ''' </summary>
    Public Class MethodCalling : Inherits Statement

        Public Overrides ReadOnly Property TypeID As Types
            Get
                Return Types.MethodCalling
            End Get
        End Property

        ''' <summary>
        ''' 对于<see cref="Expression"/>类型而言，其不仅仅调用一个方法，而且还将方法的返回值赋值给一个指定的左端变量，
        ''' 由于变量之间赋值传递的情况也可能存在，故而这个属性也可能是引用一个内存地址，当找不到方法的时候，就会通过这个参数来查找内存变量
        ''' </summary>
        ''' <returns></returns>
        Public Property EntryPoint As EntryPoint

        ''' <summary>
        ''' 解析得到的顺序应该和原始的脚本语句是一致的
        ''' </summary>
        ''' <returns></returns>
        Public Property Parameters As Dictionary(Of ParameterName, InternalExpression)
        Public Property BooleanSwitches As String()

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub

        Public Overridable Function Execute(ScriptEngine As ShoalShell.Runtime.Objects.ShellScript) As Object
            Return MethodCalling.Execute(EntryPoint, Parameters, ScriptEngine)
        End Function

        ''' <summary>
        ''' 由于所有的对象都要是动态指针来代替的，所以在这里需要计算参数名以及参数值的实际的位置
        ''' </summary>
        ''' <param name="EntryPoint"></param>
        ''' <param name="Parameters"></param>
        ''' <param name="ScriptEngine"></param>
        ''' <returns></returns>
        Public Shared Function Execute(EntryPoint As EntryPoint, Parameters As Dictionary(Of ParameterName, InternalExpression), ScriptEngine As ShoalShell.Runtime.Objects.ShellScript) As Object
            Dim ParameterValue As Dictionary(Of String, Object) = New Dictionary(Of String, Object)  '计算过后所得到的参数名和参数值
            For Each Parameter In Parameters
                Dim Name As String
                If Parameter.Key.Type <> ParameterName.ParameterType.Normal Then
                    Name = Parameter.Key.Type.ToString
                Else
                    Name = Parameter.Key.GetValue(ScriptEngine)
                End If

                Dim value As Object = Parameter.Value.GetValue(ScriptEngine)
                Call ParameterValue.Add(Name, value)
            Next

            Dim [Function] = EntryPoint.TryGetEntryPoint(ScriptEngine)
            Dim Handle = ScriptEngine._Interpreter._InternalMethodInvoker.FindOverloadsMethod([Function], ParameterValue, ScriptEngine._EngineMemoryDevice, "")
            Dim params = GenerateParameters(ParameterValue, Handle.MethodEntryPoint)
            Dim Result = Handle.Invoke(params, Handle.InvokeOnObject, True)
            Return Result
        End Function

        ''' <summary>
        ''' 生成可选参数以及开关参数
        ''' </summary>
        ''' <returns></returns>
        Private Shared Function GenerateParameters(ParameterValue As Dictionary(Of String, Object), Method As System.Reflection.MethodInfo) As Object()
            Dim List As New List(Of Object)
            Dim offset As Integer = 1

            If ParameterValue.ContainsKey(ShoalShell.Interpreter.ObjectModels.Tokens.ParameterName.s_ExtensionMethodCaller) Then
                Call List.Add(ParameterValue(ShoalShell.Interpreter.ObjectModels.Tokens.ParameterName.s_ExtensionMethodCaller))
                Call ParameterValue.Remove(ShoalShell.Interpreter.ObjectModels.Tokens.ParameterName.s_ExtensionMethodCaller)

                If ParameterValue.ContainsKey(ShoalShell.Interpreter.ObjectModels.Tokens.ParameterName.s_EXtensionSingleParameter) Then
                    Call List.Add(ParameterValue(ShoalShell.Interpreter.ObjectModels.Tokens.ParameterName.s_EXtensionSingleParameter))
                    Call ParameterValue.Remove(ShoalShell.Interpreter.ObjectModels.Tokens.ParameterName.s_EXtensionSingleParameter)

                    offset = 2
                End If

            Else
                If ParameterValue.ContainsKey(ShoalShell.Interpreter.ObjectModels.Tokens.ParameterName.s_SingleParameter) Then
                    Call List.Add(ParameterValue(ShoalShell.Interpreter.ObjectModels.Tokens.ParameterName.s_SingleParameter))
                    Call ParameterValue.Remove(ShoalShell.Interpreter.ObjectModels.Tokens.ParameterName.s_SingleParameter)
                End If
            End If

            For Each parameter In Method.GetParameters.Skip(offset)
                If ParameterValue.ContainsKey(parameter.Name) Then
                    Call List.Add(ParameterValue(parameter.Name))
                    Call ParameterValue.Remove(parameter.Name)
                Else

                    '但是为可选参数或者为逻辑值
                    If parameter.IsOptional Then
                        Call List.Add(parameter.DefaultValue)
                    ElseIf parameter.ParameterType.Equals(GetType(Boolean))
                        Call List.Add(False)  '开关参数默认为False
                    Else

                        '语法错误
                        Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.SyntaxErrorException($"Could not found the parameter value for ""{parameter.Name} As {parameter.ParameterType.FullName}""!")

                    End If
                End If
            Next

            Return List.ToArray
        End Function
    End Class

    ' a <- $b
    ' a = $b
    ' a = $b -> method
    ' call $a -> method


    ''' <summary>
    ''' 本表达式对象仅仅解析出词元对象
    ''' </summary>
    Public Class Expression : Inherits MethodCalling

        Public Property LeftAssignedVariable As LeftAssignedVariable
        Public Property [Operator] As [Operator]
        Public Property ExtensionVariable As LeftAssignedVariable

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub

        ''' <summary>
        ''' 只有左端引用表达式不为空，其他的元素都为空
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsVariable As Boolean
            Get
                If Not [Operator] Is Nothing Then
                    Return False
                End If
                If Not EntryPoint Is Nothing Then
                    Return False
                End If
                If Not Parameters.IsNullOrEmpty Then
                    Return False
                End If

                Return Not LeftAssignedVariable Is Nothing
            End Get
        End Property

        Public Overrides ReadOnly Property TypeID As Types
            Get
                Return Types.Expression
            End Get
        End Property

        Public Overrides Function Execute(ScriptEngine As ShoalShell.Runtime.Objects.ShellScript) As Object

            If IsVariable Then
                Return LeftAssignedVariable.GetValue(ScriptEngine.ScriptEngineMemoryDevice)
            End If

            Dim hash = MyBase.Parameters.CopyTypeDef
            If Not ExtensionVariable Is Nothing Then
                Call hash.Add(New ParameterName(ParameterName.ParameterType.ExtensionMethodCaller, ""), New InternalExpression(ExtensionVariable.GetTokenValue))
            End If


            Call hash.AddRange(MyBase.Parameters)

            Dim value = MethodCalling.Execute(EntryPoint, hash, ScriptEngine) '得到计算值，然后需要进行赋值

            ''在这里需要根据运算符的不同来选择不同的处理方法
            'If [Operator].Type = [Operator].Operators.ValueAssign OrElse [Operator].Type = [Operator].Operators.HybridsScript OrElse [Operator].Type = [Operator].Operators.SelfCast Then
            '    Call LeftAssignedVariable.WriteMemory(value, ScriptEngine.ScriptEngineMemoryDevice)
            'End If

            Return value
        End Function

    End Class

End Namespace