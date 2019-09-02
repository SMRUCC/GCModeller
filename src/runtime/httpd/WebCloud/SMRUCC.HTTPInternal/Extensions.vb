#Region "Microsoft.VisualBasic::0816269a2f2f03b5a6e7fdbeb375f84d, WebCloud\SMRUCC.HTTPInternal\Extensions.vb"

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
' Structure JsonResponse
' 
'     Properties: code, message
' 
'     Function: ToString
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine
Imports SMRUCC.WebCloud.HTTPInternal.Core

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
            Call out.WriteHeader(MIMEtype, reader.Length)
            Call reader.CopyTo(destination:=out.response.BaseStream)

            ' .NET <= v3.5

            'Dim read As Value(Of Integer) = 0

            'Do While (read = reader.Read(buffer, Scan0, buffer.Length)) > 0
            '    Call out.Write(buffer, 0, read)
            'Loop
        End Using
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub SuccessMsg(rep As HttpResponse, message$)
        Call rep.WriteJSON(New JsonResponse With {.code = 0, .message = message})
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub FailureMsg(rep As HttpResponse, message$, Optional code& = HTTP_RFC.RFC_UNKNOWN_ERROR)
        Call rep.WriteJSON(New JsonResponse With {.code = code, .message = message})
    End Sub
End Module