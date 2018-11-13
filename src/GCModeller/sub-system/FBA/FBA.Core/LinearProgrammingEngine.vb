Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Algebra.LinearProgramming
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Public Class LinearProgrammingEngine

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Run(model As CellularModule, targets$()) As LPPSolution
        Return Run(CreateMatrix(model, targets))
    End Function

    ''' <summary>
    ''' 将细胞之中的代谢网络定义转换为数字矩阵用于后续的计算
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="target$"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 可以将这个函数在继承类之中进行重写，就可以添加诸如调控信息之类的额外的模型信息了
    ''' </remarks>
    Protected Overridable Function CreateMatrix(model As CellularModule, target$()) As Matrix

    End Function

    Public Function Run(matrix As Matrix) As LPPSolution
        Dim engine As New LPP(
            OptimizationType.MAX,
            matrix.Compounds,
            matrix.GetTargetCoefficients,
            matrix.GetMatrix,
            "=".Replicate(matrix.NumOfFlux).ToArray,
            0.0.Replicate(matrix.NumOfFlux).ToArray
        )

        Return engine.solve
    End Function

End Class
