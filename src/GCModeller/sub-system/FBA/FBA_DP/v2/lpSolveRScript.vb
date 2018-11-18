Imports System.Runtime.CompilerServices
Imports RDotNET.Extensions.Bioinformatics
Imports RDotNET.Extensions.Bioinformatics.lpSolveAPI.APIExtensions
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports SMRUCC.genomics.Analysis.FBA.Core

Namespace v2

    Public Module lpSolveRScript

        <Extension> Public Function Rsolver(matrix As Matrix)
            ' 加载所需要的线性规划的计算程序包
            require(lpSolveAPI.packageName)

            Dim lprec$ = makelp(0, matrix.Flux.Count, verbose:="full")
            Dim direction As New Dictionary(Of String, String) From {
                {"sense", "max".Rstring}
            }

            Call setobjfn(lprec, matrix.GetTargetCoefficients)
            Call lpcontrol(lprec, direction)

            For Each compound As Double() In matrix.Matrix
                Call addconstraint(lprec, compound, 0, lpSolveAPI.constraintTypes.equals)
            Next

            Call setbounds(lprec, lower:=base.c(matrix.Flux.Select(Function(f) f.Value.Min)))
            Call setbounds(lprec, upper:=base.c(matrix.Flux.Select(Function(f) f.Value.Max)))

            Dim lppSolution = solve(lprec)
        End Function
    End Module
End Namespace