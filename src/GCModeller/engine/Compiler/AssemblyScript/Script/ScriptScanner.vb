#Region "Microsoft.VisualBasic::b908e18b933006bfc61711fce6ac5ac7, engine\Compiler\AssemblyScript\ScriptScanner.vb"

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

'     Class ScriptScanner
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Parser

Namespace AssemblyScript.Script

    ''' <summary>
    ''' assembly script token scanner
    ''' </summary>
    Public Class ScriptScanner

        ReadOnly script As CharPtr

        Dim buf As New CharBuffer
        Dim escapes As New Escaping

        Private Class Escaping
            Public comment As Boolean
            Public [string] As Boolean
        End Class

        Sub New(scriptText As String)
            script = scriptText
        End Sub

        Public Iterator Function GetTokens() As IEnumerable(Of Token)
            Dim token As New Value(Of Token)

            Do While script
                If Not (token = walkChar(++script)) Is Nothing Then
                    Yield token.Value
                End If
            Loop
        End Function

        Private Function walkChar(c As Char) As Token

        End Function
    End Class
End Namespace
