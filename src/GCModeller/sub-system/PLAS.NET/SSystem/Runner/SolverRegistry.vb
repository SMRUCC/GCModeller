Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics

Namespace Runner

    ''' <summary>
    ''' The ODEs solver engine registry for the S-system simulation.
    ''' (S-system 仿真所支持的常微分方程组求解引擎注册表)
    ''' </summary>
    ''' <remarks>
    ''' 这里仅仅包含可以在 <see cref="GenericODEs"/> 之上直接使用的积分引擎；
    ''' 辛几何算法要求模型必须继承自 <c>SymplecticODEs</c>，S-system 模型不满足该约束，故被排除。
    ''' </remarks>
    Public Module SolverRegistry

        ''' <summary>
        ''' The solver name to solver method mapping.(求解器名称大小写不敏感的映射表)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Methods As IReadOnlyDictionary(Of String, ODESolverMethod) =
            New HashDictionary(Of ODESolverMethod) From {
 _
                {"euler", ODESolverMethod.Euler},
                {"rk2", ODESolverMethod.RK2},
                {"gill", ODESolverMethod.Gill},
                {"rk4", ODESolverMethod.RK4},
                {"abm4", ODESolverMethod.ABM4},
                {"backwardeuler", ODESolverMethod.BackwardEuler},
                {"trapezoidal", ODESolverMethod.Trapezoidal},
                {"bdf2", ODESolverMethod.BDF2},
                {"rosenbrockros2", ODESolverMethod.RosenbrockROS2},
                {"rk23", ODESolverMethod.RK23},
                {"rk45", ODESolverMethod.RK45}
            }

        ''' <summary>
        ''' Gets all of the supported solver names.(获取全部可用的求解器名称)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SupportedNames As String() = Methods.Keys.ToArray

        ''' <summary>
        ''' Parse the solver engine from a user specific name string.
        ''' (从用户给定的名称字符串之中解析出目标积分引擎)
        ''' </summary>
        ''' <param name="solverName">
        ''' 大小写不敏感的求解器名称，为空的时候会返回 <paramref name="defaultMethod"/> 默认值
        ''' </param>
        ''' <param name="defaultMethod"></param>
        ''' <returns></returns>
        Public Function ParseSolver(solverName As String,
                                    Optional defaultMethod As ODESolverMethod = ODESolverMethod.RK4) As ODESolverMethod

            Dim method As ODESolverMethod = defaultMethod

            If String.IsNullOrWhiteSpace(solverName) Then
                Return defaultMethod
            End If
            If TryGetByName(solverName.Trim.ToLower, method) Then
                Return method
            End If

            Throw New ArgumentException(
                $"Unsupported ODEs solver engine ""{solverName}"", available solver engines are: {AvailableList()}",
                NameOf(solverName))
        End Function

        ''' <summary>
        ''' Try parse the solver engine by name, this function will never throw exception.
        ''' (按照名称尝试解析出积分引擎，本函数不会抛出任何异常)
        ''' </summary>
        ''' <param name="solverName"></param>
        ''' <param name="method"></param>
        ''' <returns></returns>
        Public Function TryGetByName(solverName As String, ByRef method As ODESolverMethod) As Boolean
            If String.IsNullOrWhiteSpace(solverName) Then
                Return False
            End If

            Dim key As String = solverName.Trim.ToLower

            If Methods.ContainsKey(key) Then
                method = Methods(key)
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Gets the help text of all of the supported solver engines.
        ''' (获取全部可用的求解引擎的帮助文本)
        ''' </summary>
        ''' <returns></returns>
        Public Function AvailableList() As String
            Return String.Join(", ", SupportedNames)
        End Function

        ''' <summary>
        ''' Check the given solver engine can be applied on the S-system model or not.
        ''' (检查给定的积分引擎是否可以应用于 S-system 模型之上)
        ''' </summary>
        ''' <param name="method"></param>
        ''' <returns></returns>
        Public Function IsSupported(method As ODESolverMethod) As Boolean
            For Each pair As KeyValuePair(Of String, ODESolverMethod) In Methods
                If pair.Value = method Then
                    Return True
                End If
            Next

            Return False
        End Function
    End Module
End Namespace
