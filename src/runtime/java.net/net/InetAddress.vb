
Namespace Tamir.SharpSsh.java.net
    ''' <summary>
    ''' Summary description for InetAddress.
    ''' </summary>
    Public Class InetAddress
        Friend addr As System.Net.IPAddress

        Public Sub New(ByVal addr As String)
            Me.addr = System.Net.IPAddress.Parse(addr)
        End Sub

        Public Sub New(ByVal addr As System.Net.IPAddress)
            Me.addr = addr
        End Sub

        Public Function isAnyLocalAddress() As Boolean
            Return System.Net.IPAddress.IsLoopback(addr)
        End Function

        Public Function equals(ByVal addr As InetAddress) As Boolean
            Return addr.ToStringMethod().Equals(addr.ToStringMethod())
        End Function

        Public Function equals(ByVal addr As String) As Boolean
            Return addr.ToString().Equals(addr.ToString())
        End Function

        Public Overrides Function ToStringMethod() As String
            Return addr.ToString()
        End Function

        Public Overrides Function EqualsMethod(ByVal obj As Object) As Boolean
            Return equals(obj.ToString())
        End Function

        Public Function getHostAddress() As String
            Return ToStringMethod()
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return MyBase.GetHashCode()
        End Function

        Public Shared Function getByName(ByVal name As String) As InetAddress
            Return New InetAddress(System.Net.Dns.GetHostByName(name).AddressList(0))
        End Function
    End Class
End Namespace
