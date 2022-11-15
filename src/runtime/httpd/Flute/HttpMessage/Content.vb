#Region "Microsoft.VisualBasic::6a716c355f3c64c36861a20f445ee3f2, WebCloud\SMRUCC.HTTPInternal\Core\Content.vb"

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

'     Structure Content
' 
'         Properties: attachment, Length, Type
' 
'         Function: ToString
' 
'         Sub: WriteHeader
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Core.Message

    ''' <summary>
    ''' for data content response, includes content type and content length
    ''' </summary>
    Public Structure Content

        ''' <summary>
        ''' the content length in byte size
        ''' </summary>
        ''' <returns></returns>
        Public Property length As Integer

        ''' <summary>
        ''' the mime content type, usually in format like: ``text/html``.
        ''' (不需要在这里写入http头部)
        ''' </summary>
        ''' <returns></returns>
        Public Property type As String
        Public Property attachment As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Sub WriteHeader(outputStream As StreamWriter)
            If length > 0 Then
                Call outputStream.WriteLine("Content-Length: " & length)
            End If
            If Not String.IsNullOrEmpty(attachment) Then
                Call outputStream.WriteLine($"Content-Disposition: attachment;filename=""{attachment}""")
                Call outputStream.WriteLine("Accept-Ranges: bytes")
            End If
        End Sub
    End Structure
End Namespace
