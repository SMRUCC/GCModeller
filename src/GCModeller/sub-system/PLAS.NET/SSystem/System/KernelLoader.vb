Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels

Namespace Kernel

    Public Module KernelLoader

        ''' <summary>
        ''' 初始化变量集合（模拟环境）
        ''' </summary>
        ''' <param name="kernel"></param>
        ''' <param name="script"></param>
        ''' <returns></returns>
        <Extension>
        Private Function initializeEnvironment(kernel As Kernel, script As Script.Model) As var()
            Dim vars As var()

            For Each declares In script.UserFunc.SafeQuery
                Call kernel.mathEngine.SetFunction(declares.Declaration)
            Next
            For Each __const__ In script.Constant.SafeQuery
                Call kernel.mathEngine.SetSymbol(__const__.Name, __const__.Value)
            Next

            vars = LinqAPI.Exec(Of var) _
 _
                () <= From v As var
                      In script.Vars
                      Select v
                      Order By Len(v.Id) Descending

            For Each x As var In vars
                kernel.mathEngine(x.Id) = x.Value
            Next

            Return vars
        End Function

        <Extension>
        Public Function loadModel(kernel As Kernel, script As Script.Model) As Kernel
            kernel.Vars = kernel.initializeEnvironment(script)
            kernel.kicks = New Kicks(kernel, script)
            kernel.Channels = script.sEquations _
                .Select(Function(x)
                            ' 在初始化动力学方程组之前必须要先初始化变量集合
                            Return New Equation(x, kernel)
                        End Function) _
                .ToArray

            Return kernel
        End Function
    End Module
End Namespace