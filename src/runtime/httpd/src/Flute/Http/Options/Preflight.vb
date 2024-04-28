#Region "Microsoft.VisualBasic::dcb1fb6fc7993b619eef40a99b4dc3ef, G:/GCModeller/src/runtime/httpd/src/Flute//Http/Options/Preflight.vb"

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

    '   Total Lines: 52
    '    Code Lines: 28
    ' Comment Lines: 18
    '   Blank Lines: 6
    '     File Size: 2.49 KB


    '     Module Preflight
    ' 
    '         Function: IsPreflightRequest
    ' 
    '         Sub: HandlePreflightRequest
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports Flute.Http.Core.Message

Namespace Core.HttpOptions

    Module Preflight

        ''' <summary>
        ''' Preflighted requests in CORS
        ''' 
        ''' In CORS, a preflight request is sent with the OPTIONS 
        ''' method so that the server can respond if it is acceptable
        ''' to send the request. In this example, we will request
        ''' permission for these parameters:
        '''
        ''' + The Access-Control-Request-Method header sent In the 
        '''   preflight request tells the server that When the actual 
        '''   request Is sent, it will have a POST request method.
        ''' + The Access-Control-Request-Headers header tells the 
        '''   server that When the actual request Is sent, it will
        '''   have the X-PINGOTHER And Content-Type headers.
        '''   
        ''' </summary>
        ''' <param name="p"></param>
        ''' <returns></returns>
        Public Function IsPreflightRequest(p As HttpProcessor) As Boolean
            Dim cors As Boolean = p.httpHeaders.TryGetValue("Sec-Fetch-Mode").TextEquals("cors")
            Dim testMethod As Boolean = p.httpHeaders.ContainsKey("Access-Control-Request-Method")

            Return cors AndAlso testMethod
        End Function

        Public Sub HandlePreflightRequest(p As HttpProcessor)
            Dim request As New HttpRequest(p)
            Dim response As New HttpResponse(p.outputStream, AddressOf p.writeFailure, p._settings)
            Dim httpStream As StreamWriter = response.response

            Call httpStream.WriteLine($"HTTP/1.1 204 No Content")
            Call httpStream.WriteLine($"Date: {Now.ToString}")
            Call httpStream.WriteLine($"Server: {HttpProcessor.VBS_platform}")
            Call httpStream.WriteLine($"Access-Control-Allow-Origin: *")
            Call httpStream.WriteLine($"Access-Control-Allow-Methods: POST, GET, OPTIONS")
            Call httpStream.WriteLine($"Access-Control-Allow-Headers: X-PINGOTHER, Content-Type")
            Call httpStream.WriteLine($"Access-Control-Max-Age: 86400")
            Call httpStream.WriteLine($"Vary: Accept-Encoding, Origin")
            Call httpStream.WriteLine("Connection: close")
            Call httpStream.WriteLine(HttpProcessor.XPoweredBy & p._settings.x_powered_by)
            Call httpStream.WriteLine()
            Call httpStream.Flush()
        End Sub
    End Module
End Namespace
