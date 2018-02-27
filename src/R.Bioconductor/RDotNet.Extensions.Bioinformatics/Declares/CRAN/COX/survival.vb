#Region "Microsoft.VisualBasic::bf934645ee5272a72378c1a1e9057d8f, RDotNet.Extensions.Bioinformatics\Declares\CRAN\COX\survival.vb"

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

    '     Enum SurvTypes
    ' 
    '         counting, interval, interval2, left, mstate
    '         right
    ' 
    '  
    ' 
    ' 
    ' 
    '     Enum ties
    ' 
    '         breslow, efron, exact
    ' 
    '  
    ' 
    ' 
    ' 
    '     Module survival
    ' 
    '         Function: (+2 Overloads) coxph, GetCoefficients, isSurv, (+2 Overloads) Surv
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.RSystem
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace survival

    Public Enum SurvTypes
        right
        left
        interval
        counting
        interval2
        mstate
    End Enum

    Public Enum ties
        efron
        breslow
        exact
    End Enum

    Public Module survival

        ''' <summary>
        ''' Create a survival object, usually used as a response variable in a model formula. 
        ''' Argument matching is special for this function, see Details below.
        ''' </summary>
        ''' <param name="time$">For right censored data, this Is the follow up time. For interval data, 
        ''' the first argument Is the starting time for the interval.</param>
        ''' <param name="time2$">ending time of the interval for interval censored or counting process data only. 
        ''' Intervals are assumed to be open on the left and closed on the right, (start, end]. For 
        ''' counting process data, event indicates whether an event occurred at the end of the interval.</param>
        ''' <param name="event$">The status indicator, normally 0=alive, 1=dead. Other choices are 
        ''' TRUE/FALSE (TRUE = death) or 1/2 (2=death). For interval censored data, the status 
        ''' indicator is 0=right censored, 1=event at time, 2=left censored, 3=interval censored. 
        ''' Although unusual, the event indicator can be omitted, in which case all subjects are 
        ''' assumed to have an event.</param>
        ''' <param name="type">character string specifying the type of censoring. Possible values are 
        ''' "right", "left", "counting", "interval", "interval2" or "mstate".</param>
        ''' <param name="origin#">for counting process data, the hazard function origin. This option 
        ''' was intended to be used in conjunction with a model containing time dependent strata in 
        ''' order to align the subjects properly when they cross over from one strata to another, 
        ''' but it has rarely proven useful.</param>
        ''' <returns></returns>
        Public Function Surv(time$, time2$, event$, Optional type As SurvTypes = SurvTypes.right,
    Optional origin# = 0) As String
            SyncLock R
                With R
                    Dim var$ = App.NextTempName

                    .call = $"{var} <- Surv({time}, {time2}, {event$},
    type={type.Rstring},
    origin={origin});"

                    Return var
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' Create a survival object, usually used as a response variable in a model formula. 
        ''' Argument matching is special for this function, see Details below.
        ''' </summary>
        ''' <param name="time$">For right censored data, this Is the follow up time. For interval data, 
        ''' the first argument Is the starting time for the interval.</param>
        ''' <param name="event$">The status indicator, normally 0=alive, 1=dead. Other choices are 
        ''' TRUE/FALSE (TRUE = death) or 1/2 (2=death). For interval censored data, the status 
        ''' indicator is 0=right censored, 1=event at time, 2=left censored, 3=interval censored. 
        ''' Although unusual, the event indicator can be omitted, in which case all subjects are 
        ''' assumed to have an event.</param>
        ''' <param name="type">character string specifying the type of censoring. Possible values are 
        ''' "right", "left", "counting", "interval", "interval2" or "mstate".</param>
        ''' <param name="origin#">for counting process data, the hazard function origin. This option 
        ''' was intended to be used in conjunction with a model containing time dependent strata in 
        ''' order to align the subjects properly when they cross over from one strata to another, 
        ''' but it has rarely proven useful.</param>
        ''' <returns></returns>
        Public Function Surv(time$, event$, Optional type As SurvTypes = SurvTypes.right,
    Optional origin# = 0) As String
            SyncLock R
                With R
                    Dim var$ = App.NextTempName

                    .call = $"{var} <- Surv({time}, {event$},
    type={type.Rstring},
    origin={origin});"

                    Return var
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' In the case of is.Surv, a logical value TRUE if x inherits from class "Surv", otherwise an FALSE.
        ''' </summary>
        ''' <param name="x$">any R object.</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function isSurv(x$) As Boolean
            SyncLock R
                With R
                    Return .Evaluate($"is.Surv({x})").AsLogical.ToArray.First
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' Fit Proportional Hazards Regression Model
        ''' 
        ''' Fits a Cox proportional hazards regression model. Time dependent variables, time dependent 
        ''' strata, multiple events per subject, and other extensions are incorporated using the 
        ''' counting process formulation of Andersen and Gill.
        ''' </summary>
        ''' <param name="formula$">a formula object, with the response on the left of a ~ operator, 
        ''' and the terms on the right. The response must be a survival object as returned by the Surv 
        ''' function.</param>
        ''' <param name="data$">a data.frame in which to interpret the variables named in the formula, 
        ''' or in the subset and the weights argument.</param>
        ''' <param name="weights$">vector of case weights. For a thorough discussion of these see the 
        ''' book by Therneau and Grambsch.</param>
        ''' <param name="subset$">expression indicating which subset of the rows of data should be used 
        ''' in the fit. All observations are included by default.</param>
        ''' <param name="na_action$">a missing-data filter function. This is applied to the model.frame 
        ''' after any subset argument has been used. Default is options()\$na.action.</param>
        ''' <param name="init$">vector of initial values of the iteration. Default initial value is zero 
        ''' for all variables.</param>
        ''' <param name="control$">Object of class coxph.control specifying iteration limit and other 
        ''' control options. Default is coxph.control(...).</param>
        ''' <param name="ties">a character string specifying the method for tie handling. If there are 
        ''' no tied death times all the methods are equivalent. Nearly all Cox regression programs use 
        ''' the Breslow method by default, but not this one. The Efron approximation is used as the 
        ''' default here, it is more accurate when dealing with tied death times, and is as efficient 
        ''' computationally. The “exact partial likelihood” is equivalent to a conditional logistic 
        ''' model, and is appropriate when the times are a small set of discrete values. See further 
        ''' below.</param>
        ''' <param name="singular_ok">logical value indicating how to handle collinearity in the model 
        ''' matrix. If TRUE, the program will automatically skip over columns of the X matrix that are 
        ''' linear combinations of earlier columns. In this case the coefficients for such columns will 
        ''' be NA, and the variance matrix will contain zeros. For ancillary calculations, such as the 
        ''' linear predictor, the missing coefficients are treated as zeros.</param>
        ''' <param name="robust">this argument has been deprecated, use a cluster term in the model 
        ''' instead. (The two options accomplish the same goal – creation of a robust variance – 
        ''' but the second is more flexible).</param>
        ''' <param name="model">logical value: if TRUE, the model frame is returned in component model.</param>
        ''' <param name="x">logical value: if TRUE, the x matrix is returned in component x.</param>
        ''' <param name="y">logical value: if TRUE, the response vector is returned in component y.</param>
        ''' <param name="tt$">optional list of time-transform functions.</param>
        ''' <param name="method$">alternate name for the ties argument.</param>
        ''' <param name="argumentList">Other arguments will be passed to coxph.control</param>
        ''' <returns>an object of class coxph representing the fit. See coxph.object for details.</returns>
        ''' <remarks>
        ''' The proportional hazards model is usually expressed in terms of a single survival time value 
        ''' for each person, with possible censoring. Andersen and Gill reformulated the same problem 
        ''' as a counting process; as time marches onward we observe the events for a subject, rather 
        ''' like watching a Geiger counter. The data for a subject is presented as multiple rows or 
        ''' "observations", each of which applies to an interval of observation (start, stop].
        ''' 
        ''' The routine internally scales and centers data to avoid overflow in the argument to the 
        ''' exponential function. These actions do not change the result, but lead to more numerical 
        ''' stability. However, arguments to offset are not scaled since there are situations where a 
        ''' large offset value is a purposefully used. Users should not use normally allow large numeric 
        ''' offset values.
        ''' </remarks>
        Public Function coxph(formula$, data$,
                              Optional weights$ = NULL, Optional subset$ = NULL,
                              Optional na_action$ = NULL, Optional init$ = NULL,
                              Optional control$ = NULL,
                              Optional ties As ties = ties.efron,
                              Optional singular_ok As Boolean = True, Optional robust As Boolean = False,
                              Optional model As Boolean = False,
                              Optional x As Boolean = False, Optional y As Boolean = True,
                              Optional tt$ = NULL, Optional method$ = NULL,
                              Optional argumentList As Dictionary(Of String, Object) = Nothing) As String

        End Function

        ''' <summary>
        ''' 更加长使用的是这个方法
        ''' </summary>
        ''' <param name="formula$"></param>
        ''' <param name="data$"></param>
        ''' <returns></returns>
        Public Function coxph(formula$, data$) As String
            SyncLock R
                With R
                    Dim var$ = App.NextTempName
                    .call = $"{var} <- coxph({formula}, data = {data});"
                    Return var
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' Get ``res.cox$coefficients``
        ''' </summary>
        ''' <param name="coxph"></param>
        ''' <returns></returns>
        Public Function GetCoefficients(coxph As String) As Dictionary(Of String, Double)
            SyncLock R
                With R
                    Dim var$ = App.NextTempName

                    .call = $"{var} <- {coxph}$coefficients;"

                    Dim names$() = base.names(var)
                    Dim coeff#() = [as].numeric(var)

                    Return names _
                        .SeqIterator _
                        .ToDictionary(Function(name) name.value,
                                      Function(i) coeff(i))
                End With
            End SyncLock
        End Function
    End Module
End Namespace
