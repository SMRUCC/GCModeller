#Region "Microsoft.VisualBasic::b5b0d2b8d0351f564213092bf8573f13, src\Flute\Extensions.vb"

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

    '   Total Lines: 66
    '    Code Lines: 48 (72.73%)
    ' Comment Lines: 8 (12.12%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 10 (15.15%)
    '     File Size: 2.29 KB


    ' Module Extensions
    ' 
    '     Function: FaviconZip
    ' 
    '     Sub: FailureMsg, SuccessMsg, TransferBinary
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Flute.Http.AppEngine
Imports Flute.Http.Core.Message
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

#If NET_35 Then
Imports Microsoft.VisualBasic.Language
#End If

<HideModuleName> Public Module Extensions

    ''' <summary>
    ''' get the compiled favicon binary resource that is served at
    ''' <c>/favicon.ico</c> by the http server.
    ''' </summary>
    ''' <returns>the raw favicon bytes embedded in the assembly resources.</returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function FaviconZip() As Byte()
        Return My.Resources.favicon
    End Function

    ''' <summary>
    ''' stream a local file to the client as a binary http response with the
    ''' given mime type, sending it in chunks of <paramref name="buffer_size"/>.
    ''' </summary>
    ''' <param name="path$">The file path of the local file that will be transfer to the client browser.</param>
    ''' <param name="MIMEtype$"><see cref="MIME"/></param>
    ''' <param name="out">the http response that the binary stream is written to.</param>
    ''' <param name="buffer_size%">the read/write buffer size (in bytes) used while streaming.</param>
    <Extension>
    Public Sub TransferBinary(path$, MIMEtype$, ByRef out As HttpResponse, Optional buffer_size% = 4096)
        Dim buffer As Byte() = New Byte(buffer_size) {}

        Using reader As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
#If NET_35 Then
            ' .NET <= v3.5

            Dim read As Value(Of Integer) = 0

            Do While (read = reader.Read(buffer, Scan0, buffer.Length)) > 0
                Call out.Write(buffer, 0, read)
            Loop
#Else
            Call New Content With {
                .type = MIMEtype,
                .length = reader.Length,
                .attachment = path.FileName
            }.DoCall(AddressOf out.WriteHttp)

            Call reader.CopyTo(
                destination:=out.response.BaseStream
            )
            Call out.response.Flush()
#End If
        End Using
    End Sub

    ''' <summary>
    ''' write a success json response with <c>code = 0</c> and the given message
    ''' payload to the client.
    ''' </summary>
    ''' <typeparam name="T">the type of the message payload.</typeparam>
    ''' <param name="rep">the http response to write to.</param>
    ''' <param name="message">the success message payload.</param>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub SuccessMsg(Of T)(rep As HttpResponse, message As T)
        Call rep.WriteJSON(New JsonResponse(Of T) With {.code = 0, .info = message})
    End Sub

    ''' <summary>
    ''' write a failure json response with the given error code and message
    ''' payload to the client.
    ''' </summary>
    ''' <typeparam name="T">the type of the message payload.</typeparam>
    ''' <param name="rep">the http response to write to.</param>
    ''' <param name="message">the failure message payload.</param>
    ''' <param name="code">the application level error code; defaults to <see cref="HTTP_RFC.RFC_UNKNOWN_ERROR"/>.</param>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub FailureMsg(Of T)(rep As HttpResponse, message As T, Optional code& = HTTP_RFC.RFC_UNKNOWN_ERROR)
        Call rep.WriteJSON(New JsonResponse(Of T) With {.code = code, .info = message})
    End Sub
End Module
