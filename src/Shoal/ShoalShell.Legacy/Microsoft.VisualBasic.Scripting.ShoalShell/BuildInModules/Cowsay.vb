Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.EntryPointMetaData

Namespace BuildInModules.System

    <[Namespace]("System")>
    Module System

        <Command("GetType")>
        Public Function [GetType](<ParameterAlias("obj", "The object instance of the type schema source, if the value of the object is null then function will returns nothing.")> [object] As Object) As Global.System.Type
            Try
                Return [object].GetType
            Catch ex As Exception
                Return GetType(Object)
            End Try
        End Function
    End Module

    Public Module CowsayTricks

        Public ReadOnly NormalCow As String =
    <COW>          |
          |    ^__^
           --  (oo)\_______
               (__)\       )\/\
                   ||----W |
                   ||     ||
</COW>

        Public ReadOnly DeadCow As String =
    <COW>          |
          |    ^__^
           --  (XX)\_______
               (__)\       )\/\
                   ||----W |
                   ||     ||
</COW>

        Public Function Cowsay(msg As String, argvs As CommandLine.CommandLine) As String
            Select Case argvs("-t").ToLower
                Case "dead"
                    msg = Msgbox(msg) & DeadCow
                Case Else
                    msg = Msgbox(msg) & NormalCow
            End Select

            Call Console.WriteLine(msg)

            Return msg
        End Function

        Private Function Msgbox(msg As String) As String
            Dim l = Len(msg)
            Dim offset As String = New String(" ", 8)
            Dim sBuilder As StringBuilder = New StringBuilder(vbCrLf, 1024)
            Call sBuilder.AppendLine(offset & " " & New String("_", l + 4) & " ")
            Call sBuilder.AppendLine(offset & String.Format("<  {0}  >", msg))
            Call sBuilder.AppendLine(offset & " " & New String("-", l + 4) & " ")

            Return sBuilder.ToString
        End Function
    End Module
End Namespace