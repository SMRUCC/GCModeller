
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SSystem.Kernel
Imports SMRUCC.genomics.Analysis.SSystem.Script
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Invokes
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("S.system")>
Module SSystemKit

    <ExportAPI("S.script")>
    Public Function script(Optional title$ = "unnamed model", Optional description$ = "") As Model
        Return New Model With {
            .Title = title,
            .Comment = description
        }
    End Function

    <ExportAPI("kernel")>
    Public Function createKernel(snapshot As DataSnapshot, Optional model As Model = Nothing) As Kernel
        If model Is Nothing Then
            model = SSystemKit.script()
        End If

        Dim dataDriver As New DataAcquisition(AddressOf snapshot.Cache)
        Dim kernel As New Kernel(model, dataDriver)

        Call dataDriver.loadKernel(kernel)

        Return kernel
    End Function

    <ExportAPI("environment")>
    Public Function ConfigEnvironment(kernel As Kernel, <RListObjectArgument> symbols As Object, Optional env As Environment = Nothing) As Kernel
        Dim data As list = base.Rlist(symbols, env)
        Dim value As Double

        For Each symbolName As String In data.slots.Keys
            value = asVector(Of Double)(data.getByName(symbolName)).GetValue(Scan0)
            kernel.SetMathSymbol(symbolName, value)
        Next

        Return kernel
    End Function

    <ExportAPI("s.system")>
    Public Function ConfigSSystem(kernel As Kernel, ssystem As Array, Optional env As Environment = Nothing) As Kernel

        Return kernel
    End Function

    <ExportAPI("run")>
    Public Function RunKernel(kernel As Kernel, Optional ticks As Integer = 100, Optional resolution As Double = 0.01) As Kernel
        kernel.finalTime = ticks
        kernel.precision = resolution
        kernel.Run()

        Return kernel
    End Function

    <ExportAPI("snapshot")>
    Public Function GetSnapshotsDriver(Optional file As String = Nothing, Optional symbols As String() = Nothing) As DataSnapshot
        If file.StringEmpty Then
            Return New MemoryCacheSnapshot
        Else
            Return New SnapshotStream(file, symbols)
        End If
    End Function
End Module
