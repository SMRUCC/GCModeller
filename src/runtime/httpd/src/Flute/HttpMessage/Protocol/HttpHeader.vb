#Region "Microsoft.VisualBasic::4c9d006feafc57b5c1735f88f7aba42b, G:/GCModeller/src/runtime/httpd/src/Flute//HttpMessage/Protocol/HttpHeader.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 41
    '    Code Lines: 35
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.73 KB


    '     Class RequestHeaders
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class ResponseHeaders
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
