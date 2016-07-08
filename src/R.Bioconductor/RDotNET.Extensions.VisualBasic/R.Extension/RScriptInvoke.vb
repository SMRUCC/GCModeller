
Imports Microsoft.VisualBasic.Linq
''' <summary>
''' 推荐使用这个对象来执行R脚本
''' </summary>
Public Class RScriptInvoke

    ''' <summary>
    ''' The R script
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property [Call] As String
    ''' <summary>
    ''' R output from the script <see cref="[Call]"/>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property STD_OUT As String()

    ''' <summary>
    ''' Creates a R call from the script text
    ''' </summary>
    ''' <param name="script"></param>
    Sub New(script As String)
        Me.Call = script
    End Sub

    ''' <summary>
    ''' Creates a R call from the script builder
    ''' </summary>
    ''' <param name="script"></param>
    Sub New(script As IRProvider)
        Me.Call = script.RScript
    End Sub

    ''' <summary>
    ''' Display the output on the system console.
    ''' </summary>
    Public Sub PrintSTDOUT()
        For Each s As String In STD_OUT.SafeQuery
            Call Console.WriteLine(s)
        Next
    End Sub

    ''' <summary>
    ''' <see cref="[Call]"/>
    ''' </summary>
    ''' <returns></returns>
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
    ''' The R script <see cref="[Call]"/> should output a S4Object.
    ''' </summary>
    ''' <typeparam name="T">在R之中的类型必须是S4Object对象</typeparam>
    ''' <returns></returns>
    Public Function Invoke(Of T As Class)() As T
        Dim raw As RDotNET.SymbolicExpression = RServer.Evaluate([Call])
        Dim result As T = Serialization.LoadFromStream(Of T)(raw)
        Return result
    End Function
End Class
