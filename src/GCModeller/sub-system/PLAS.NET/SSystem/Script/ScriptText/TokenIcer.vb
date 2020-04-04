#Region "Microsoft.VisualBasic::9dae4ed4861309f54dca4a53258b15a3, sub-system\PLAS.NET\SSystem\Script\TokenIcer.vb"

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

'     Module TokenIcer
' 
'         Properties: Tokens
' 
'         Function: __tokenParser, TryParse
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace Script

    Public Module TokenIcer

        ReadOnly tokens As New Dictionary(Of String, ScriptTokens)

        Sub New()
            For Each token As ScriptTokens In Enums(Of ScriptTokens)()
                tokens.Add(token.Description, token)
            Next
        End Sub

        Public Function TryParse(script As IEnumerable(Of String)) As ScriptToken()
            Dim LQuery As ScriptToken() = LinqAPI.Exec(Of ScriptToken) _
 _
                () <= From line As String
                      In script
                      Let token As ScriptToken = line.Trim.tokenParser()
                      Where Not token Is Nothing
                      Select token

            Return LQuery
        End Function

        ''' <summary>
        ''' 会忽略掉注释信息
        ''' </summary>
        ''' <param name="line">类型的前缀应该是已经被切割掉了</param>
        ''' <returns></returns>
        <Extension>
        Private Function tokenParser(line As String) As ScriptToken
            Dim x As String = line.Split.First.ToUpper
            Dim type As ScriptTokens
            Dim text As String

            If Not tokens.ContainsKey(x) Then
                Return Nothing
            Else
                type = tokens(x)
            End If

            If type = Script.ScriptTokens.Comment Then
                Return Nothing
            Else
                text = Mid(line, x.Length + 1).Trim
            End If

            Return New ScriptToken(type, text)
        End Function
    End Module
End Namespace
