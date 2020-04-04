Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels

Namespace Kernel

    Public Module KernelLoader

        <Extension>
        Public Function loadModel(kernel As Kernel, script As Script.Model) As Kernel
            Dim channels = script.sEquations.Select(Function(x) New Equation(x)).ToArray
            Dim vars = LinqAPI.Exec(Of var) <=
 _
                From v As var
                In script.Vars
                Select v
                Order By Len(v.Id) Descending

            For Each declares In script.UserFunc.SafeQuery
                Call kernel.mathEngine.SetFunction(declares.Declaration)
            Next
            For Each __const__ In script.Constant.SafeQuery
                Call kernel.mathEngine.SetSymbol(__const__.Name, __const__.Value)
            Next

            For Each x As var In vars
                kernel.mathEngine(x.Id) = x.Value
            Next

            For i As Integer = 0 To channels.Length - 1
                channels(i).Set(kernel)
            Next

            kernel.kicks = New Kicks(kernel)
            kernel.Channels = channels
            kernel.Vars = vars

            Return kernel
        End Function
    End Module
End Namespace