#Region "Microsoft.VisualBasic::374faa8a350f8a8254a56d9670b85556, RDotNet.Extensions.Bioinformatics\Declares\CRAN\lpSolve\API.vb"

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

    '     Module API
    ' 
    '         Function: lp
    ' 
    '     Class lpObject
    ' 
    '         Properties: bin_count, binary_vec, compute_sens, const_count, constraints
    '                     dense_col, dense_const_nrow, dense_ctr, dense_val, direction
    '                     duals, duals_from, duals_to, int_count, int_vec
    '                     num_bin_solns, objective, objval, presolve, scale
    '                     sens_coef_from, sens_coef_to, solution, status, tmp
    '                     use_dense, use_rw, x_count
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace lpSolve

    ''' <summary>
    ''' Interface to 'Lp_solve' v. 5.5 to Solve Linear/Integer Programs
    ''' </summary>
    Public Module API

        ''' <summary>
        ''' Interface to lp\_solve linear/integer programming system
        ''' </summary>
        ''' <param name="objective_in$">Numeric vector of coefficients of objective function</param>
        ''' <param name="const_mat$">
        ''' Matrix of numeric constraint coefficients, one row per constraint, one column
        ''' per variable(unless transpose.constraints = False; see below).</param>
        ''' <param name="const_dir$">Vector of character strings giving the direction of the constraint: each value
        ''' should be one of "&lt;," "&lt;=," "=," "==," ">," or ">=". (In each pair the two values are identical.)        ''' </param>
        ''' <param name="const_rhs$">Vector of numeric values for the right-hand sides of the constraints.</param>
        ''' <param name="direction$">Character string giving direction of optimization: "min" (default) or "max."</param>
        ''' <param name="transpose_constraints">By default each constraint occupies a row of const.mat, and that matrix needs to
        ''' be transposed before being passed to the optimizing code. For very large con-
        ''' straint matrices it may be wiser to construct the constraints in a matrix column-
        ''' by-column. In that case set transpose.constraints to FALSE.</param>
        ''' <param name="presolve#">Numeric: presolve? Default 0 (no); any non-zero value means "yes." Currently
        ''' ignored.</param>
        ''' <param name="compute_sens#">Numeric: compute sensitivity? Default 0 (no); any non-zero value means "yes."</param>
        ''' <param name="all_int">Logical: should all variables be integer? Default: FALSE.</param>
        ''' <param name="all_bin">Logical: should all variables be binary? Default: FALSE.</param>
        ''' <param name="scale#">Integer: value for lpSolve scaling. Details can be found in the lpSolve docu-
        ''' mentation. Set to 0 for no scaling. Default: 196</param>
        ''' <param name="num_bin_solns#">Integer: if all.bin=TRUE, the user can request up to num.bin.solns optimal so-
        ''' lutions to be returned.</param>
        ''' <param name="use_rw">Logical: if TRUE and num.bin.solns > 1, write the lp out to a file and read it
        ''' back in for each solution after the first. This is just to defeat a bug somewhere.
        ''' Although the default is FALSE, we recommend you set this to TRUE if you
        ''' need num.bin.solns > 1, until the bug is found.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' This function calls the lp\_solve 5.5 solver. That system has many options not supported here. The
        ''' current version is maintained at http://lpsolve.sourceforge.net/5.5
        ''' Note that every variable is assumed to be >= 0!
        ''' </remarks>
        Public Function lp(objective_in$, const_mat$, const_dir$, const_rhs$, Optional direction$ = "min",
 Optional transpose_constraints As Boolean = True, Optional presolve# = 0, Optional compute_sens# = 0,
 Optional all_int As Boolean = False, Optional all_bin As Boolean = False, Optional scale# = 196, Optional num_bin_solns# = 1, Optional use_rw As Boolean = False) As String
            SyncLock R
                With R

                    Dim var$ = App.NextTempName

                    .call = $"{var} <- lp(direction = {Rstring(direction)}, {objective_in}, {const_mat}, {const_dir}, {const_rhs},
transpose.constraints = {transpose_constraints.λ}, presolve={presolve}, compute.sens={compute_sens},
 all.int={all_int.λ}, all.bin={all_bin.λ}, scale = {scale}, num.bin.solns={num_bin_solns}, use.rw={use_rw.λ});"

                    Return var
                End With
            End SyncLock
        End Function


    End Module

    Public Class lpObject

        ''' <summary>
        ''' Optimization direction, as entered
        ''' </summary>
        ''' <returns></returns>
        Public Property direction As String
        ''' <summary>
        ''' Number of variables in objective function
        ''' </summary>
        ''' <returns></returns>
        Public Property x_count
        ''' <summary>
        ''' Vector of objective function coefficients, as entered
        ''' </summary>
        ''' <returns></returns>
        Public Property objective As Double()
        ''' <summary>
        ''' Number of constraints entered
        ''' </summary>
        ''' <returns></returns>
        Public Property const_count
        ''' <summary>
        ''' Constraint matrix, as entered (not returned by lp.assign or lp.transport)
        ''' </summary>
        ''' <returns></returns>
        Public Property constraints
        ''' <summary>
        ''' Number of integer variables
        ''' </summary>
        ''' <returns></returns>
        Public Property int_count
        ''' <summary>
        ''' Vector of integer variables’ indices, as entered
        ''' </summary>
        ''' <returns></returns>
        Public Property int_vec
        Public Property bin_count
        Public Property binary_vec
        ''' <summary>
        ''' Numeric indicator of number of solutions returned
        ''' </summary>
        ''' <returns></returns>
        Public Property num_bin_solns
        ''' <summary>
        ''' Value of objective function at optimum
        ''' </summary>
        ''' <returns></returns>
        Public Property objval As Double
        ''' <summary>
        ''' Vector of optimal coefficients
        ''' </summary>
        ''' <returns></returns>
        Public Property solution As Double()
        Public Property presolve As Double
        Public Property compute_sens
        Public Property sens_coef_from
        Public Property sens_coef_to
        Public Property duals
        Public Property duals_from
        Public Property duals_to
        Public Property scale As Double
        Public Property use_dense
        Public Property dense_col
        Public Property dense_val
        Public Property dense_const_nrow
        Public Property dense_ctr
        Public Property use_rw
        Public Property tmp
        ''' <summary>
        ''' Numeric indicator: 0 = success, 2 = no feasible solution
        ''' </summary>
        ''' <returns></returns>
        Public Property status As Double

    End Class
End Namespace
