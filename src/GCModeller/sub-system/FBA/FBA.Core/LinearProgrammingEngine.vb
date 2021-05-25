#Region "Microsoft.VisualBasic::85f28e20ce0fe523d3a16b03b1f08ad9, sub-system\FBA\FBA.Core\LinearProgrammingEngine.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:

' Class LinearProgrammingEngine
' 
'     Function: CreateMatrix, (+2 Overloads) Run
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular

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
    Public Overridable Function CreateMatrix(model As CellularModule, targets$()) As Matrix
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
                .ToDictionary(Function(flux) flux.ID,
                              Function(flux)
                                  Return New DoubleRange(flux.bounds)
                              End Function),
            .Targets = targets
        }
    End Function

    Public Function Run(matrix As Matrix) As LPPSolution
        Dim engine As New LPP(
            OptimizationType.MAX,
            matrix.Flux.Keys.ToArray,
            matrix.GetTargetCoefficients,
            matrix.GetMatrix,
            "=".Replicate(matrix.NumOfCompounds).ToArray,
            0.0.Replicate(matrix.NumOfCompounds).ToArray
        )

        Return engine.solve(showProgress:=True)
    End Function

End Class
