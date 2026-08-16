#Region "Microsoft.VisualBasic::fddfc0953653335ffc01a904e0709560, src\Flute\HttpMessage\Protocol\HttpError.vb"

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
    '    Code Lines: 38 (77.55%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (22.45%)
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

    ''' <summary>
    ''' builds http error response pages from a template that supports a
    ''' <c>{$message}</c> placeholder, and maps <see cref="HTTP_RFC"/> codes to
    ''' their description text.
    ''' </summary>
    Public Class HttpError

        ''' <summary>
        ''' the error page template; the <c>{$message}</c> placeholder is replaced
        ''' with the actual error message when rendering.
        ''' </summary>
        ReadOnly template As String

        ''' <summary>
        ''' create an error page builder from the given template string.
        ''' </summary>
        ''' <param name="template">the template containing a <c>{$message}</c> placeholder.</param>
        Sub New(template As String)
            Me.template = template
        End Sub

        ''' <summary>
        ''' create an error page builder using the default <c>{$message}</c> template.
        ''' </summary>
        Sub New()
            Call Me.New("{$message}")
        End Sub

        ''' <summary>
        ''' render the error page for the given message by substituting the
        ''' <c>{$message}</c> placeholder in the template.
        ''' </summary>
        ''' <param name="message">the error message to embed in the page.</param>
        ''' <returns>the rendered error page string.</returns>
        Public Function GetErrorPage(message As String) As String
            With New ScriptBuilder(template)
                !message = message

                Return .ToString
            End With
        End Function

        ''' <summary>
        ''' the static map of <see cref="HTTP_RFC"/> numeric codes to their
        ''' description text, built once from the enum metadata.
        ''' </summary>
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

        ''' <summary>
        ''' look up the human readable description for an <see cref="HTTP_RFC"/> code.
        ''' </summary>
        ''' <param name="code">the numeric status code as a string.</param>
        ''' <returns>the description text, or "Unknown Status" when not found.</returns>
        Public Shared Function getRFCMessage(code As String) As String
            If httpRFC.ContainsKey(code) Then
                Return httpRFC(code)
            Else
                Return "Unknown Status"
            End If
        End Function

    End Class
End Namespace
