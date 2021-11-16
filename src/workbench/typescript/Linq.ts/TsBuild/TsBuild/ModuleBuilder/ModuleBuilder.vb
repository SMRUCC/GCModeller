#Region "Microsoft.VisualBasic::30604aa27749b80aacd3786b108f784a, typescript\Linq.ts\TsBuild\TsBuild\ModuleBuilder\ModuleBuilder.vb"

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

    ' Module ModuleBuilder
    ' 
    '     Function: BuildVisualBasicModule
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text

Module ModuleBuilder

    <Extension>
    Public Function BuildVisualBasicModule(tokens As IEnumerable(Of Token)) As String
        Dim vb As New StringBuilder
        Dim stack As New Stack(Of String)
        Dim moduleKey$ = ""

        For Each t As Token In tokens
            Select Case t.type
                Case TypeScriptTokens.closeStack
                    Call vb.AppendLine($"End {stack.Pop}")
                Case TypeScriptTokens.openStack
                    Call vb.AppendLine()
                    Call stack.Push(moduleKey)
                Case TypeScriptTokens.typeName
                    Call vb.Append("As " & t.text)
                Case TypeScriptTokens.funcType
                    Call vb.Append("As " & t.text)
                    Call vb.AppendLine()
                Case TypeScriptTokens.identifier, TypeScriptTokens.functionName
                    Call vb.Append(t.text)
                Case TypeScriptTokens.comment
                    Dim comment$ = t.text _
                        .LineTokens _
                        .Select(Function(s) "' " & s.Trim("/"c, "*"c)) _
                        .JoinBy(vbCrLf)

                    Call vb.AppendLine(comment)
                Case TypeScriptTokens.keyword
                    Select Case t.text
                        Case "function"
                            moduleKey = "Function"
                        Case "namespace"
                            moduleKey = "Namespace"
                        Case "module"
                            moduleKey = "Module"
                        Case "declare"
                            ' ignores this typescript keyword
                        Case "interface"
                            moduleKey = "Interface"
                        Case "class"
                            moduleKey = "Class"
                        Case "protected"
                            Call vb.Append("Protected")
                        Case "public"
                            Call vb.Append("Public")
                        Case "private"
                            Call vb.Append("private")
                        Case "static"
                            Call vb.Append("Shared")
                        Case "extends"
                            Call vb.AppendLine()
                            Call vb.Append("Inherits")
                        Case Else
                            Throw New NotImplementedException
                    End Select
                Case Else
                    Throw New NotImplementedException
            End Select

            Call vb.Append(" ")
        Next

        Return vb.ToString
    End Function
End Module
