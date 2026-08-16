Namespace Core.Message.HttpHeader

    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class ExportAPIAttribute : Inherits Attribute

        Public ReadOnly Property Url As String

        Sub New(url As String)
            _Url = url
        End Sub

    End Class

    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class HttpGet : Inherits ExportAPIAttribute

        Sub New(url As String)
            Call MyBase.New(url)
        End Sub

        Public Overrides Function ToString() As String
            Return $"http-get('{Url}')"
        End Function

    End Class

    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class HttpPost : Inherits ExportAPIAttribute

        Sub New(url As String)
            Call MyBase.New(url)
        End Sub

        Public Overrides Function ToString() As String
            Return $"http-post('{Url}')"
        End Function
    End Class
End Namespace