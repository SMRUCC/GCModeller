Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Web.Packages

    Public Class Archives

        <Column("Package Source")> Public Property Source As String
        <Column("Windows Binary")> Public Property WindowsBinary As String
        <Column("Mac OS X 10.6 (Snow Leopard)")> Public Property MacSnowLeopard As String
        <Column("Mac OS X 10.9 (Mavericks)")> Public Property MacMavericks As String
        <Column("Subversion source")> Public Property Subversion As String
        <Column("Git source")> Public Property Git As String
        <Column("Package Short Url")> Public Property ShortUrl As String
        <Column("Package Downloads Report")> Public Property DownloadsReport As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace