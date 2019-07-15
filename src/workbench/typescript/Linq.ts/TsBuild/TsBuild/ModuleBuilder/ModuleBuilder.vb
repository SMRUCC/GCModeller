Imports System.Runtime.CompilerServices
Imports System.Text

Module ModuleBuilder

    <Extension>
    Public Function BuildVisualBasicModule(tokens As IEnumerable(Of Token)) As String
        Dim vb As New StringBuilder
        Dim stack As New Stack(Of String)
        Dim moduleKey$ = ""

        For Each t As Token In tokens
            Select Case t.Type
                Case TypeScriptTokens.closeStack
                    Call vb.AppendLine($"End {stack.Pop}")
                Case TypeScriptTokens.openStack
                    Call vb.AppendLine()
                    Call stack.Push(moduleKey)
                Case TypeScriptTokens.typeName
                    Call vb.Append("As " & t.Text)
                Case TypeScriptTokens.funcType
                    Call vb.Append("As " & t.Text)
                    Call vb.AppendLine()
                Case TypeScriptTokens.identifier, TypeScriptTokens.functionName
                    Call vb.Append(t.Text)
                Case TypeScriptTokens.comment
                    Dim comment$ = t.Text _
                        .LineTokens _
                        .Select(Function(s) "' " & s.Trim("/"c, "*"c)) _
                        .JoinBy(vbCrLf)

                    Call vb.AppendLine(comment)
                Case TypeScriptTokens.keyword
                    Select Case t.Text
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
