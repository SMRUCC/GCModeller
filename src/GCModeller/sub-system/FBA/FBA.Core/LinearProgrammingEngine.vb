Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Algebra.LinearProgramming
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Public Class LinearProgrammingEngine

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="targets">约束的目标，即目标代谢反应的<see cref="Reaction.ID"/>编号集合</param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Run(model As CellularModule, targets$()) As LPPSolution
        Return Run(CreateMatrix(model, targets))
    End Function

    ''' <summary>
    ''' 将细胞之中的代谢网络定义转换为数字矩阵用于后续的计算
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="targets$"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 可以将这个函数在继承类之中进行重写，就可以添加诸如调控信息之类的额外的模型信息了
    ''' </remarks>
    Protected Overridable Function CreateMatrix(model As CellularModule, targets$()) As Matrix
        Dim allCompounds$() = model.Phenotype.fluxes _
            .Select(Function(r) r.AllCompounds) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(name) name) _
            .ToArray
        Dim matrix#()() = allCompounds _
            .Select(Function(compound)
                        ' 按照列取出系数
                        Return model.Phenotype.fluxes _
                            .Select(Function(r) r.GetCoefficient(compound)) _
                            .ToArray
                    End Function) _
            .ToArray

        Return New Matrix With {
            .Matrix = matrix,
            .Compounds = allCompounds,
            .Flux = model.Phenotype _
                .fluxes _
                .Select(Function(flux) flux.ID) _
                .ToArray,
            .Targets = targets
        }
    End Function

    Public Function Run(matrix As Matrix) As LPPSolution
        Dim engine As New LPP(
            OptimizationType.MAX,
            matrix.Flux,
            matrix.GetTargetCoefficients,
            matrix.GetMatrix,
            "=".Replicate(matrix.NumOfCompounds).ToArray,
            0.0.Replicate(matrix.NumOfCompounds).ToArray
        )

        Return engine.solve
    End Function

End Class
