Namespace Core.Message.HttpHeader

    Public NotInheritable Class RequestHeaders

        Private Sub New()
        End Sub

        Public Const Accept As String = "Accept"
        Public Const AcceptEncoding As String = "Accept-Encoding"
        Public Const AcceptLanguage As String = "Accept-Language"
        Public Const CacheControl As String = "Cache-Control"
        Public Const Connection As String = "Connection"
        Public Const Cookie As String = "Cookie"
        Public Const Host As String = "Host"
        Public Const Pragma As String = "Pragma"
        Public Const UpgradeInsecureRequests As String = "Upgrade-Insecure-Requests"
        Public Const ContentType As String = "Content-Type"
        Public Const UserAgent As String = "User-Agent"
    End Class

    Public NotInheritable Class ResponseHeaders

        Private Sub New()
        End Sub

        Public Const CacheControl As String = "Cache-Control"
        Public Const Connection As String = "Connection"
        Public Const ContentEncoding As String = "Content-Encoding"
        Public Const ContentLength As String = "Content-Length"
        Public Const ContentType As String = "Content-Type"
        Public Const [Date] As String = "Date"
        Public Const Expires As String = "Expires"
        Public Const KeepAlive As String = "Keep-Alive"
        Public Const Pragma As String = "Pragma"
        Public Const Server As String = "Server"
        Public Const SetCookie As String = "Set-Cookie"
        Public Const Vary As String = "Vary"
        Public Const XFrameOptions As String = "X-Frame-Options"
        Public Const XPoweredBy As String = "X-Powered-By"
    End Class
End Namespace