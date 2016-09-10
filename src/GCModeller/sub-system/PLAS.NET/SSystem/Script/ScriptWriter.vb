Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text

Namespace Script

    Public Module ScriptWriter

        ''' <summary>
        ''' Generates the script text from the data model
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        <Extension>
        Public Function WriteScript(model As Model, path As String) As Boolean
            Return model.WriteScript(path.Open)
        End Function

        ''' <summary>
        ''' 向流指针之中写入模型数据
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="s"></param>
        ''' <returns></returns>
        <Extension>
        Public Function WriteScript(model As Model, ByRef s As Stream) As Boolean
            Dim sb As New StreamWriter(s)

            For Each rxn In model.sEquations
                Call sb.WriteLine($"RXN {rxn.x}={rxn.Expression}")
            Next

            Call sb.WriteLine()

            For Each var In model.Vars
                Call sb.WriteLine($"INIT {var.UniqueId}={var.Value}")
            Next
            Call sb.WriteLine()
            Call sb.WriteLine("FINALTIME " & model.FinalTime)
            Call s.Flush()

            Return True
        End Function
    End Module
End Namespace