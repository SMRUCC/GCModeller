Namespace Platform.AccessController

    Public Class Uses

        Public Property Usage As Usages = Usages.Api

        Public Overrides Function ToString() As String
            Return $"Using as '{Usage.Description}'"
        End Function

    End Class

    Public Enum Usages
        Api
        View
        Text
        Soap
        Image
        FileDownload
    End Enum
End Namespace