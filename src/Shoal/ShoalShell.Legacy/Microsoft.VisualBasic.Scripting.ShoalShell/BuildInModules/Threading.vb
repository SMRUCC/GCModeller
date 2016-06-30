Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports System.Runtime.CompilerServices

Namespace BuildInModules.System.Threading

    Public Structure ThreadingHandle

        Implements Microsoft.VisualBasic.ComponentModel.Collection.Generic.IKeyValuePairObject(Of Global.System.IAsyncResult, Global.System.Func(Of Object))

        Public Property ThreadingInvoke As Global.System.IAsyncResult Implements ComponentModel.Collection.Generic.IKeyValuePairObject(Of IAsyncResult, Func(Of Object)).Key
        Public Property [Delegate] As Global.System.Func(Of Object) Implements ComponentModel.Collection.Generic.IKeyValuePairObject(Of IAsyncResult, Func(Of Object)).Value

        Public Overrides Function ToString() As String
            Return [Delegate].ToString
        End Function
    End Structure

    <[Namespace]("System.Threading")>
    Public Module ShoalThreading

        't <- $command -> begin_invoke
        'r <- $t -> end_invoke

        <Command("Invoke.Begin", Info:="Parameter only accept the script text or delegate function which was implement as Func(Of Object)")>
        Public Function BeginInvoke(entry As ShoalShell.Runtime.Objects.ShellScript, Operation As Object) As ThreadingHandle
            Dim [Delegate] As Func(Of Object)

            If TypeOf Operation Is String Then
                [Delegate] = Function() entry.InternalSourceScript(ShellScript:=Operation, parameters:=Nothing)
            ElseIf TypeOf Operation Is Global.System.Action Then
                [Delegate] = Function() As Object
                                 Call DirectCast(Operation, Global.System.Action)
                                 Return True
                             End Function
            Else
                [Delegate] = DirectCast(Operation, Func(Of Object))
            End If

            Return New ThreadingHandle With {.Delegate = [Delegate], .ThreadingInvoke = [Delegate].BeginInvoke(Nothing, Nothing)}
        End Function

        <Command("Invoke.End")>
        Public Function EndInvoke(handle As ThreadingHandle) As Object
            Return handle.Delegate.EndInvoke(handle.ThreadingInvoke)
        End Function

        <Command("Threading.Start")>
        Public Function Threading(entry As ShoalShell.Runtime.Objects.ShellScript, command As String) As Boolean
            Call New Global.System.Threading.Thread(Sub() Call entry.InternalSourceScript(command, Nothing)).Start()
            Return True
        End Function
    End Module
End Namespace

