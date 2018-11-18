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

Imports System.ComponentModel
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract

Namespace lpSolveAPI

    ''' <summary>
    ''' R Interface to 'lp_solve' Version 5.5.2.0
    ''' 
    ''' The lpSolveAPI package provides an R interface to 'lp_solve',
    ''' a Mixed Integer Linear Programming (MILP) solver With support For pure
    ''' linear, (mixed) integer/binary, semi-continuous And special ordered sets
    ''' (SOS) models.
    ''' </summary>
    Public Module APIExtensions

        ''' <summary>
        ''' Set the objective function in an lpSolve linear program model object.
        ''' </summary>
        ''' <param name="lprec">an lpSolve linear program model object.</param>
        ''' <param name="obj">a numeric vector of length n (where n is the number of decision variables in lprec) 
        ''' containing the coefficients of the objective function. Alternatively, if indices is also provided, 
        ''' a numeric vector of the same length as indices containing only the nonzero coefficients.</param>
        ''' <param name="indices">optional for sparse obj. A numeric vector the same length as obj of unique values 
        ''' from the set {1, ..., n} where n is the number of decision variables in lprec; obj[i] is entered into 
        ''' column indices[i] in objective function. The coefficients for the columns not in indices are set to zero. 
        ''' This argument should be omitted when length(obj) == n.</param>
        Public Sub setobjfn(lprec$, obj As IEnumerable(Of Integer), Optional indices As IEnumerable(Of Integer) = Nothing)
            SyncLock R
                With R
                    If indices Is Nothing Then
                        .call = $"set.objfn({lprec}, {c(obj)});"
                    Else
                        .call = $"set.objfn({lprec}, {c(obj)}, indices = {c(indices)});"
                    End If
                End With
            End SyncLock
        End Sub

        ''' <summary>
        ''' Set control parameters in an lpSolve linear program model object.
        ''' </summary>
        ''' <param name="lprec">an lpSolve linear program model object.</param>
        ''' <param name="arguments">control arguments to bet set in lprec.</param>
        ''' <param name="reset">a logical value. If TRUE all control parameters are reset to their default values</param>
        ''' <returns>a list containing all of the control parameters as set internally in lprec.</returns>
        Public Function lpcontrol(lprec$, arguments As Dictionary(Of String, String), Optional reset As Boolean = False) As Dictionary(Of String, String)
            Dim list_args$ = arguments.Select(Function(tuple) $"{tuple.Key}={tuple.Value}").JoinBy(", ")

            SyncLock R
                With R
                    .call = $"lp.control({lprec}, {list_args}, reset = {reset.λ})"
                End With
            End SyncLock

            Return arguments
        End Function

        ''' <summary>
        ''' Add a constraint to an lpSolve linear program model object.
        ''' </summary>
        ''' <param name="lprec$">an lpSolve linear program model object.</param>
        ''' <param name="xt">a numeric vector containing the constraint coefficients (only the nonzero coefficients if indices is also 
        ''' given). The length of xt must be equal to the number of decision variables in lprec unless indices is provided.</param>
        ''' <param name="type">a numeric or character value from the set {1 = "&lt;=", 2 = ">=", 3 = "="} specifying the type of the constraint.</param>
        ''' <param name="rhs$">a single numeric value specifying the right-hand-side of the constraint.</param>
        ''' <param name="indices">optional for sparse xt. A numeric vector the same length as xt of unique values from the set {1, ..., n} where n 
        ''' is the number of decision variables in lprec; xt[i] is entered into column indices[i] in the added constraint. The coefficients for the 
        ''' columns not in indices are set to zero. This argument should be omitted when length(xt) == n.</param>
        ''' <param name="lhs$">optional. A single numeric value specifying the left-hand-side of the constraint.</param>
        Public Sub addconstraint(lprec$, xt As IEnumerable(Of Double), rhs As Double,
                                 Optional type As constraintTypes = constraintTypes.ltOrEquals,
                                 Optional indices As IEnumerable(Of Integer) = Nothing,
                                 Optional lhs As Double? = Nothing)
            SyncLock R
                With R
                    Dim Rscript$ = New addconstraint With {
                        .lprec = lprec,
                        .indices = indices,
                        .lhs = lhs,
                        .rhs = rhs,
                        .type = type,
                        .xt = xt
                    }

                    .call = Rscript
                End With
            End SyncLock
        End Sub
    End Module

    <RFunc("add.constraint")>
    Public Class addconstraint : Inherits IRToken

        Public Property lprec As String
        Public Property xt As IEnumerable(Of Double)
        Public Property type As constraintTypes = constraintTypes.ltOrEquals
        Public Property rhs As Double
        Public Property indices As IEnumerable(Of Integer)
        Public Property lhs As Double?

    End Class

    Public Enum constraintTypes
        <Description("<=")> ltOrEquals
        <Description("=")> equals
        <Description(">=")> gtOrEquals
    End Enum
End Namespace
