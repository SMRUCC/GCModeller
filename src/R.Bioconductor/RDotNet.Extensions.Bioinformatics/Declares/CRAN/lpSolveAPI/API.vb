#Region "Microsoft.VisualBasic::634a486673135c9c113e6d34cbec7094, RDotNet.Extensions.Bioinformatics\Declares\CRAN\lpSolveAPI\API.vb"

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

    '     Module APIExtensions
    ' 
    '         Function: getobjective, getvariables, lpcontrol, makelp
    ' 
    '         Sub: addconstraint, setbounds, setobjfn
    ' 
    '     Class setbounds
    ' 
    '         Properties: columns, lower, lprec, upper
    ' 
    '     Class addconstraint
    ' 
    '         Properties: indices, lhs, lprec, rhs, type
    '                     xt
    ' 
    '     Enum constraintTypes
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.Linq
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

        Public Const packageName$ = "lpSolveAPI"

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
        Public Sub setobjfn(lprec$, obj As IEnumerable(Of Double), Optional indices As IEnumerable(Of Integer) = Nothing)
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
                        .indices = indices?.ToArray,
                        .lhs = lhs,
                        .rhs = rhs,
                        .type = type,
                        .xt = xt.ToArray
                    }

                    .call = Rscript
                End With
            End SyncLock
        End Sub

        ''' <summary>
        ''' Set bounds on the decision variables in an lpSolve linear program model object.
        ''' </summary>
        ''' <param name="lprec$">an lpSolve linear program model object.</param>
        ''' <param name="lower$">a numeric vector of lower bounds to be set on the decision variables specified in columns. 
        ''' If NULL the lower bounds are not changed.</param>
        ''' <param name="upper$">a numeric vector of upper bounds to be set on the decision variables specified in columns. 
        ''' If NULL the upper bounds are not changed.</param>
        ''' <param name="columns">a numeric vector of values from the set ``{1, ..., n}`` specifying the columns to have 
        ''' their bounds set. If NULL all columns are set.</param>
        Public Sub setbounds(lprec$, Optional lower$ = Nothing, Optional upper$ = Nothing, Optional columns As IEnumerable(Of Integer) = Nothing)
            SyncLock R
                With R
                    Dim Rscript$ = New setbounds With {
                        .lprec = lprec,
                        .columns = columns?.ToArray,
                        .lower = lower,
                        .upper = upper
                    }

                    .call = Rscript
                End With
            End SyncLock
        End Sub

        ''' <summary>
        ''' Create a new lpSolve linear program model object.
        ''' </summary>
        ''' <param name="nrow%">a nonnegative integer value specifying the number of constaints in the linear program.</param>
        ''' <param name="ncol%">a nonnegative integer value specifying the number of decision variables in the linear program.</param>
        ''' <param name="verbose$">a character string controlling the level of error reporting. The default value "neutral" is no error reporting. 
        ''' Use "normal" or "full" for more comprehensive error reporting. See the verbose entry in lp.control.options for a complete description 
        ''' of this argument and its possible values.</param>
        ''' <returns>an lpSolve linear program model object. Specifically an R external pointer with class lpExtPtr.</returns>
        Public Function makelp(Optional nrow% = 0, Optional ncol% = 0, Optional verbose$ = "neutral") As String
            Dim var$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{var} <- make.lp(nrow = {nrow}, ncol = {ncol}, verbose = {verbose.Rstring});"
                End With
            End SyncLock

            Return var
        End Function

        ''' <summary>
        ''' Retrieve the value of the objective function from a successfully solved lpSolve linear program 
        ''' model object.
        ''' </summary>
        ''' <param name="lprec">an lpSolve linear program model object.</param>
        ''' <returns>a single numeric value containing the value of the objective function.</returns>
        Public Function getobjective(lprec As String) As Double
            SyncLock R
                With R
                    Dim result = App.NextTempName
                    Dim val#

                    .call = $"{result} <- get.objective({lprec});"
                    val = .Evaluate(result) _
                          .AsNumeric _
                          .First

                    Return val
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' Retrieve the values of the decision variables from a successfully solved lpSolve linear 
        ''' program model object.
        ''' </summary>
        ''' <param name="lprec">an lpSolve linear program model object.</param>
        ''' <returns>
        ''' a numeric vector containing the values of the decision variables corresponding to the 
        ''' optimal solution.
        ''' </returns>
        Public Function getvariables(lprec As String) As Double()
            SyncLock R
                With R
                    Dim result = App.NextTempName
                    Dim val#()

                    .call = $"{result} <- get.variables({lprec});"
                    val = .Evaluate(result) _
                          .AsNumeric _
                          .ToArray

                    Return val
                End With
            End SyncLock
        End Function
    End Module

    <RFunc("set.bounds")>
    Public Class setbounds : Inherits IRToken
        Public Property lprec As String
        Public Property lower As String
        Public Property upper As String
        Public Property columns As Integer()
    End Class

    <RFunc("add.constraint")>
    Public Class addconstraint : Inherits IRToken

        Public Property lprec As String
        Public Property xt As Double()

        <Parameter("type")>
        Public Property type As constraintTypes = constraintTypes.ltOrEquals

        Public Property rhs As Double
        Public Property indices As Integer()
        Public Property lhs As Double?

    End Class

    Public Enum constraintTypes
        <Description("<=")> ltOrEquals
        <Description("=")> equals
        <Description(">=")> gtOrEquals
    End Enum
End Namespace
