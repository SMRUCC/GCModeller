#Region "Microsoft.VisualBasic::fddfc0953653335ffc01a904e0709560, G:/GCModeller/src/runtime/httpd/src/Flute//HttpMessage/Protocol/HttpError.vb"

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

    '   Total Lines: 49
    '    Code Lines: 38
    ' Comment Lines: 0
    '   Blank Lines: 11
    '     File Size: 1.46 KB


    '     Class HttpError
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: GetErrorPage, getRFCMessage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.SymbolBuilder

Namespace Core.Message.HttpHeader

    Public Class HttpError

        ReadOnly template As String

        Sub New(template As String)
            Me.template = template
        End Sub

        Sub New()
            Call Me.New("{$message}")
        End Sub

        Public Function GetErrorPage(message As String) As String
            With New ScriptBuilder(template)
                !message = message

                Return .ToString
            End With
        End Function

        Shared ReadOnly httpRFC As Dictionary(Of String, String)

        Shared Sub New()
            httpRFC = Enums(Of HTTP_RFC)() _
                .Select(Function(a) (a.Description, CLng(a).ToString)) _
                .Where(Function(a)
                           Return Not a.Description.StringEmpty
                       End Function) _
                .ToDictionary(Function(a) a.Item2,
                              Function(a)
                                  Return a.Description
                              End Function)
        End Sub

        Public Shared Function getRFCMessage(code As String) As String
            If httpRFC.ContainsKey(code) Then
                Return httpRFC(code)
            Else
                Return "Unknown Status"
            End If
        End Function

    End Class
End Namespace
