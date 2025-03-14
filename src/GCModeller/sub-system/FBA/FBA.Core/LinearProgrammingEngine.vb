﻿#Region "Microsoft.VisualBasic::8d2ae34cd712d0a91567b5bded30da8d, sub-system\FBA\FBA.Core\LinearProgrammingEngine.vb"

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


    ' Code Statistics:

    '   Total Lines: 92
    '    Code Lines: 66 (71.74%)
    ' Comment Lines: 16 (17.39%)
    '    - Xml Docs: 87.50%
    ' 
    '   Blank Lines: 10 (10.87%)
    '     File Size: 3.87 KB


    ' Class LinearProgrammingEngine
    ' 
    '     Function: CreateMatrix, (+2 Overloads) Run, ToLppModel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

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
    Public Overridable Function CreateMatrix(model As CellularModule, Optional targets$() = Nothing) As Matrix
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

        If targets.IsNullOrEmpty Then
            targets = model.Phenotype.fluxes _
                .Select(Function(r) r.ID) _
                .ToArray
        End If

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

    Public Shared Function ToLppModel(fbaMat As Matrix, name As String, Optional description As String = "n/a") As LPPModel
        Dim types As String() = "=".Replicate(fbaMat.NumOfCompounds).ToArray
        Dim constraints As Double() = 0.0.Replicate(fbaMat.NumOfCompounds).ToArray

        Return New LPPModel(fbaMat.Matrix, types, constraints, fbaMat.Compounds) With {
            .objectiveFunctionType = OptimizationType.MAX.Description,
            .objectiveFunctionValue = 0
        }.ConfigSymbols(
            names:=fbaMat.Flux.Keys.ToArray,
            value:=fbaMat.GetTargetCoefficients
        ).ConfigModelName(name, description)
    End Function

    Public Function Run(fbaMat As Matrix) As LPPSolution
        Dim engine As New LPP(
            objectiveFunctionType:=OptimizationType.MAX.Description,
            variableNames:=fbaMat.Flux.Keys.ToArray,
            objectiveFunctionCoefficients:=fbaMat.GetTargetCoefficients,
            constraintCoefficients:=fbaMat.Matrix,
            constraintTypes:="=".Replicate(fbaMat.NumOfCompounds).ToArray,
            constraintRightHandSides:=0.0.Replicate(fbaMat.NumOfCompounds).ToArray,
            objectiveFunctionValue:=0
        )

        Return engine.solve(showProgress:=True)
    End Function

End Class
