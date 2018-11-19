Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.Algebra.LinearProgramming
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports RDotNET.Extensions.Bioinformatics
Imports RDotNET.Extensions.Bioinformatics.lpSolveAPI.APIExtensions
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports SMRUCC.genomics.Analysis.FBA.Core

Namespace v2

    Public Module lpSolveRScript

        <Extension> Public Function Rsolver(matrix As Matrix) As LPPSolution
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

            Using progress As New ProgressBar("Build lpSolve constraints matrix...")
                Dim tick As New ProgressProvider(matrix.Matrix.Length)

                For Each compound As Double() In matrix.Matrix
                    Call addconstraint(lprec, compound, 0, lpSolveAPI.constraintTypes.equals)
                    Call constraintTypes.Add("=")
                    Call progress.SetProgress(tick.StepProgress, compoundNames(tick.Current - 1))
                Next
            End Using

            Call print(lprec)

            Call setbounds(lprec, lower:=base.c(matrix.Flux.Select(Function(f) f.Value.Min)))
            Call setbounds(lprec, upper:=base.c(matrix.Flux.Select(Function(f) f.Value.Max)))

            ' 设置名称，方便进行调试
            Dim rownames = base.c(compoundNames, stringVector:=True)
            Dim colNames = base.c(fluxNames, stringVector:=True)

            dimnames(lprec) = base.list(rownames, colNames)

            Dim error$ = base.solve(lprec)
            Dim result# = getobjective(lprec)
            Dim fluxDistrib#() = getvariables(lprec)

            Return New LPPSolution(fluxDistrib, result, fluxNames, constraintTypes, {}, {}, {}, 0, 0, "", "G5")
        End Function
    End Module
End Namespace