#Region "Microsoft.VisualBasic::f0dafbac95c99e9538037d4160a76651, sub-system\FBA\FBA.Core\LinearProgrammingEngine.vb"

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

    '   Total Lines: 117
    '    Code Lines: 66 (56.41%)
    ' Comment Lines: 40 (34.19%)
    '    - Xml Docs: 87.50%
    ' 
    '   Blank Lines: 11 (9.40%)
    '     File Size: 5.33 KB


    ' Class LinearProgrammingEngine
    ' 
    '     Function: CreateMatrix, (+2 Overloads) Run, ToLppModel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process

''' <summary>
''' FBA LPP solver
''' </summary>
''' <remarks>
''' #### FBA代谢流平衡分析的计算原理
''' 
''' FBA的核心思想是： 在一个假设的稳态下， 计算一个细胞代谢网络中所有反应的流量（通量）分布， 使得某个生物学目标（如生长速率）达到最优。
''' 
''' 它建立在几个关键假设之上：
''' 
''' 稳态假设： 这是FBA的基石。它假设在所研究的时间尺度内，细胞内每个代谢物的浓度保持不变。这意味着，对于任何一个代谢物，其生成总速率等于消耗总速率。
''' 数学表达：d[Metabolite_i]/dt = 0
''' 质量守恒： 代谢网络中的物质是守恒的。每个反应都遵循化学计量关系。
''' 目标驱动： 细胞（尤其是微生物）的代谢行为是为了实现某个优化目标，最常见的就是最大化自身的生长速率（即最大化生物量合成反应的通量）。
''' </remarks>
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
    ''' <param name="targets">需要流量最大化的目标代谢反应的ID集合</param>
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
        Dim fluxList = model.Phenotype.fluxes.ToArray
        Dim compoundIndex As Dictionary(Of String, Integer) = allCompounds _
            .Select(Function(id, i) (id, i)) _
            .ToDictionary(Function(t) t.id, Function(t) t.i)
        Dim rows As New List(Of Integer)(fluxList.Length * 8)
        Dim cols As New List(Of Integer)(fluxList.Length * 8)
        Dim vals As New List(Of Double)(fluxList.Length * 8)

        For j As Integer = 0 To fluxList.Length - 1
            Dim flux = fluxList(j)

            For Each compound As String In flux.AllCompounds
                Dim row As Integer = -1

                If Not compoundIndex.TryGetValue(compound, row) Then
                    Continue For
                End If

                Dim coefficient# = flux.GetCoefficient(compound)

                If coefficient <> 0.0 Then
                    rows.Add(row)
                    cols.Add(j)
                    vals.Add(coefficient)
                End If
            Next
        Next

        If targets.IsNullOrEmpty Then
            targets = model.Phenotype.fluxes _
                .Select(Function(r) r.ID) _
                .ToArray
        End If

        Return New Matrix With {
            .Stoichiometry = LpSparseMatrix.FromTriplets(
                rows:=allCompounds.Length,
                columns:=fluxList.Length,
                rowIdx:=rows.ToArray,
                colIdx:=cols.ToArray,
                vals:=vals.ToArray),
            .Compounds = allCompounds,
            .Flux = fluxList _
                .ToDictionary(Function(flux) flux.ID,
                              Function(flux)
                                  Return New DoubleRange(flux.bounds)
                              End Function),
            .Targets = targets
        }
    End Function

    ''' <summary>
    ''' 导出 FBA 问题的线性规划模型描述
    ''' </summary>
    ''' <remarks>
    ''' 仅用于模型导出与展示（R# 等上层接口），**不参与求解**：
    ''' 求解路径已切换到内点法引擎，见 <see cref="Run(Matrix, OptimizationType)"/>。
    ''' </remarks>
    Public Shared Function ToLppModel(fbaMat As Matrix, name As String, Optional description As String = "n/a", Optional opt As OptimizationType = OptimizationType.MAX) As LPPModel
        Dim types As String() = "=".Replicate(fbaMat.NumOfCompounds).ToArray
        Dim constraints As Double() = 0.0.Replicate(fbaMat.NumOfCompounds).ToArray

        Return New LPPModel(fbaMat.Matrix, types, constraints, fbaMat.Compounds) With {
            .objectiveFunctionType = opt.Description,
            .objectiveFunctionValue = 0
        }.ConfigSymbols(
            names:=fbaMat.Flux.Keys.ToArray,
            value:=fbaMat.GetTargetCoefficients
        ).ConfigModelName(name, description)
    End Function

    ''' <summary>
    ''' FBA solver based on the interior point method(IPM) solver
    ''' </summary>
    ''' <param name="fbaMat"></param>
    ''' <param name="opt"></param>
    ''' <returns>
    ''' + the objective function value is the bio-mass value
    ''' + the lpp solution is the reaction flux value
    ''' </returns>
    ''' <remarks>
    ''' 本函数原先使用单纯形法求解器 <c>LPP</c>，在基因组规模（万级代谢物 × 万级反应）
    ''' 上会退化：既无法在可接受时间内收敛，结果也会严重越界。
    ''' 现已切换为 IPMCrossover 内点法引擎（Mehrotra 预测-校正 + 稀疏正规方程），
    ''' 并通过 <see cref="IpmFbaAdapter"/> 完成 FBA 问题 ⇄ 标准形的稀疏转换
    ''' （下界平移消掉 lb、上界由内点法原生支持，规模不膨胀）。
    '''
    ''' the flux bounds is required by the FBA problem, without the flux
    ''' bounds the FBA linear programming problem will be degenerated at
    ''' the zero flux point (the right hand side of the mass balance 
    ''' constraint is always zero), result in a full zero solution.
    ''' </remarks>
    Public Function Run(fbaMat As Matrix, Optional opt As OptimizationType = OptimizationType.MAX) As LPPSolution
        Return IpmFbaAdapter.Run(fbaMat, opt)
    End Function

End Class
