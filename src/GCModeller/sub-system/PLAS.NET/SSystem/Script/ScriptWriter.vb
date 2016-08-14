Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Script

    Public Module ScriptWriter

        <Extension>
        Public Function WriteScript(model As Model, path As String) As Boolean
            Dim sb As New StringBuilder

            For Each rxn In model.sEquations
                Call sb.AppendLine($"RXN {rxn.x}={rxn.Expression}")
            Next
            Call sb.AppendLine()
            For Each var In model.Vars
                Call sb.AppendLine($"INIT {var.UniqueId}={var.Value}")
            Next
            Call sb.AppendLine()
            Call sb.AppendLine("FINALTIME " & model.FinalTime)

            Return sb.SaveTo(path)
        End Function
    End Module
End Namespace