Imports Microsoft.VisualBasic.ComponentModel.Settings

Namespace Configuration

    ''' <summary>
    ''' 配置数据的设置引擎
    ''' </summary>
    Public Class Configuration : Inherits ConfigEngine

        Sub New(ScriptEngine As Runtime.ScriptEngine)
            Call MyBase.New(ScriptEngine.Config)
        End Sub

        Public Overrides Function ExistsNode(Name As String) As Boolean
            Dim b = MyBase.ExistsNode(Name)
            Call Console.WriteLine(If(b, "YES", "NO_EXISTS"))
            Return b
        End Function

        Public Overrides Function GetSettings(Name As String) As String
            Dim str As String = MyBase.GetSettings(Name)
            Call Console.WriteLine(str)
            Return str
        End Function

        Public Overloads Function Prints(data As IEnumerable(Of ProfileItem)) As String
            Dim str As String = MyBase.Prints(data)
            Call Console.WriteLine(str)
            Return str
        End Function
    End Class
End Namespace