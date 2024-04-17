#Region "Microsoft.VisualBasic::111227a71ab28dc0b69fb204aeb9e091, WebCloud\SMRUCC.HTTPInternal\Extensions.vb"

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
#If NET_35 Then
Imports Microsoft.VisualBasic.Language
#End If
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

<HideModuleName> Public Module Extensions

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function FaviconZip() As Byte()
        Return My.Resources.favicon
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="path$">The file path of the local file that will be transfer to the client browser.</param>
    ''' <param name="MIMEtype$"><see cref="MIME"/></param>
    ''' <param name="out"></param>
    ''' <param name="buffer_size%"></param>
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

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub SuccessMsg(Of T)(rep As HttpResponse, message As T)
        Call rep.WriteJSON(New JsonResponse(Of T) With {.code = 0, .info = message})
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub FailureMsg(Of T)(rep As HttpResponse, message As T, Optional code& = HTTP_RFC.RFC_UNKNOWN_ERROR)
        Call rep.WriteJSON(New JsonResponse(Of T) With {.code = code, .info = message})
    End Sub
End Module
