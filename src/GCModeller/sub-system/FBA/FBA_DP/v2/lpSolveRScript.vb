#Region "Microsoft.VisualBasic::e93628e1e03e92f447ee0a72e2ab06c8, sub-system\FBA\FBA_DP\v2\lpSolveRScript.vb"

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

    '     Module lpSolveRScript
    ' 
    '         Function: Rsolver
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming
Imports RDotNET.Extensions.Bioinformatics
Imports RDotNET.Extensions.Bioinformatics.lpSolveAPI.APIExtensions
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports SMRUCC.genomics.Analysis.FBA.Core

Namespace v2

    Public Module lpSolveRScript

        <Extension> Public Function Rsolver(matrix As Matrix, Optional debug As Boolean = True) As LPPSolution
            ' 加载所需要的线性规划的计算程序包
            require(lpSolveAPI.packageName)

            Dim lprec$ = makelp(0, matrix.Flux.Count, verbose:="full")
            Dim direction As New Dictionary(Of String, String) From {
                {"sense", "max".Rstring}
            }
            Dim fluxNames$() = matrix.Flux.Keys.ToArray
            Dim constraintTypes As New List(Of String)
            Dim compoundNames$() = matrix.Compounds

            Call setobjfn(lprec, matrix.GetTargetCoefficients)
            Call lpcontrol(lprec, direction)

            If matrix.Flux.Count <> matrix.Matrix(Scan0).Length Then
                Throw New Exception("Matrix size not agree with flux counts data!")
            End If

            Using progress As New ProgressBar("Build lpSolve constraints matrix...", Y:=30)
                Dim tick As New ProgressProvider(progress, matrix.Matrix.Length)

                For Each compound As Double() In matrix.Matrix
                    Call addconstraint(lprec, compound, 0, lpSolveAPI.constraintTypes.equals)
                    Call constraintTypes.Add("=")
                    Call progress.SetProgress(tick.StepProgress, compoundNames(tick.Current - 1))
                Next
            End Using

            If debug Then
                ' 设置名称，方便进行调试
                Dim rownames = base.c(compoundNames, stringVector:=True)
                Dim colNames = base.c(fluxNames, stringVector:=True)

                dimnames(lprec) = base.list(rownames, colNames)
                print(lprec)
            End If

            Call setbounds(lprec, lower:=base.c(matrix.Flux.Select(Function(f) f.Value.Min)))
            Call setbounds(lprec, upper:=base.c(matrix.Flux.Select(Function(f) f.Value.Max)))

            Dim error$ = base.solve(lprec)
            Dim result# = getobjective(lprec)
            Dim fluxDistrib#() = getvariables(lprec)
            Dim zeroFill = 0R.Replicate(fluxNames.Length).ToArray

            Return New LPPSolution(fluxDistrib, result, fluxNames, constraintTypes, {}, {}, zeroFill, 0, 0, "", "G5")
        End Function
    End Module
End Namespace
