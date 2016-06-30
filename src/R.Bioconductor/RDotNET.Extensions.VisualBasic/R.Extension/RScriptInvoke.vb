
''' <summary>
''' 推荐使用这个对象来执行R脚本
''' </summary>
Public Class RScriptInvoke

    Public ReadOnly Property [Call] As String
    Public ReadOnly Property STD_OUT As String()

    Sub New(script As String)
        Me.Call = script
    End Sub

    Sub New(script As IRProvider)
        Me.Call = script.RScript
    End Sub

    Public Sub PrintSTDOUT()
        If Not STD_OUT.IsNullOrEmpty Then
            Dim s As String = String.Join(vbCrLf, STD_OUT)
            Call Console.WriteLine(s)
        End If
    End Sub

    Public Overrides Function ToString() As String
        Return [Call]
    End Function

    Public Function Invoke() As String()
        Return RServer.WriteLine([Call])
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="parser">提供了R数据输出解析的方法</param>
    ''' <returns></returns>
    Public Function Invoke(Of T)(parser As Func(Of String(), T)) As T
        Return parser(Invoke())
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T">在R之中的类型必须是S4Object对象</typeparam>
    ''' <returns></returns>
    Public Function Invoke(Of T As Class)() As T
        Dim raw As RDotNET.SymbolicExpression = RServer.Evaluate([Call])
        Dim result As T = Serialization.LoadFromStream(Of T)(raw)
        Return result
    End Function
End Class
