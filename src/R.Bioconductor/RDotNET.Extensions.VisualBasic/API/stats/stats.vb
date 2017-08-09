#Region "Microsoft.VisualBasic::4cef575fbe73c4ecf9b4de361c50956d, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\API\stats\stats.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace API

    Public Module stats

        ''' <summary>
        ''' Fit an ARIMA model to a univariate time series.
        ''' </summary>
        ''' <param name="x">a univariate time series</param>
        ''' <param name="order">A specification of the non-seasonal part of the ARIMA model: the three integer components (p, d, q) are the AR order, the degree of differencing, and the MA order.</param>
        ''' <param name="seasonal">A specification of the seasonal part of the ARIMA model, plus the period (which defaults to frequency(x)). This should be a list with components order and period, but a specification of just a numeric vector of length 3 will be turned into a suitable list with the specification as the order.</param>
        ''' <param name="xreg">Optionally, a vector or matrix of external regressors, which must have the same number of rows as x.</param>
        ''' <param name="includemean">Should the ARMA model include a mean/intercept term? The default is TRUE for undifferenced series, and it is ignored for ARIMA models with differencing.</param>
        ''' <param name="transformpars">logical; if true, the AR parameters are transformed to ensure that they remain in the region of stationarity. Not used for method = "CSS". For method = "ML", it has been advantageous to set transform.pars = FALSE in some cases, see also fixed.</param>
        ''' <param name="fixed">optional numeric vector of the same length as the total number of parameters. If supplied, only NA entries in fixed will be varied. transform.pars = TRUE will be overridden (with a warning) if any AR parameters are fixed. It may be wise to set transform.pars = FALSE when fixing MA parameters, especially near non-invertibility.</param>
        ''' <param name="init">optional numeric vector of initial parameter values. Missing values will be filled in, by zeroes except for regression coefficients. Values already specified in fixed will be ignored.</param>
        ''' <param name="method">fitting method: maximum likelihood or minimize conditional sum-of-squares. The default (unless there are missing values) is to use conditional-sum-of-squares to find starting values, then maximum likelihood. Can be abbreviated.</param>
        ''' <param name="ncond">only used if fitting by conditional-sum-of-squares: the number of initial observations to ignore. It will be ignored if less than the maximum lag of an AR term.</param>
        ''' <param name="SSinit">a string specifying the algorithm to compute the state-space initialization of the likelihood; see KalmanLike for details. Can be abbreviated.</param>
        ''' <param name="optimmethod">The value passed as the method argument to optim.</param>
        ''' <param name="optimcontrol">List of control parameters for optim.</param>
        ''' <param name="kappa">the prior variance (as a multiple of the innovations variance) for the past observations in a differenced model. Do not reduce this.</param>
        ''' <returns>
        ''' A list of class "Arima" with components:
        '''
        ''' coef	
        ''' a vector Of AR, MA And regression coefficients, which can be extracted by the coef method.
        '''
        ''' sigma2	
        ''' the MLE Of the innovations variance.
        '''
        ''' var.coef	
        ''' the estimated variance matrix Of the coefficients coef, which can be extracted by the vcov method.
        '''
        ''' loglik	
        ''' the maximized log-likelihood (Of the differenced data), Or the approximation To it used.
        '''
        ''' arma	
        ''' A compact form Of the specification, As a vector giving the number Of AR, MA, seasonal AR And seasonal MA coefficients, plus the period And the number Of non-seasonal And seasonal differences.
        '''
        ''' aic	
        ''' the AIC value corresponding To the log-likelihood. Only valid For method = "ML" fits.
        '''
        ''' residuals	
        ''' the fitted innovations.
        '''
        ''' Call    
        ''' the matched Call.
        '''
        ''' series	
        ''' the name Of the series x.
        '''
        ''' code	
        ''' the convergence value returned by optim.
        '''
        ''' n.cond	
        ''' the number Of initial observations Not used In the fitting.
        '''
        ''' nobs	
        ''' the number Of “used” observations For the fitting, can also be extracted via nobs() And Is used by BIC.
        '''
        ''' model	
        ''' A list representing the Kalman Filter used In the fitting. See KalmanLike.
        ''' </returns>
        ''' <remarks>
        ''' Different definitions of ARMA models have different signs for the AR and/or MA coefficients. The definition used here has
        ''' ```
        ''' X[t] = a[1]X[t-1] + … + a[p]X[t-p] + e[t] + b[1]e[t-1] + … + b[q]e[t-q]
        ''' ```
        ''' And so the MA coefficients differ in sign from those of S-PLUS. Further, if include.mean Is true (the default for an ARMA model), this formula applies to X - m rather than X. 
        ''' For ARIMA models with differencing, the differenced series follows a zero-mean ARMA model. If am xreg term Is included, a linear regression (with a constant term if include.mean 
        ''' Is true And there Is no differencing) Is fitted with an ARMA model for the error term.
        ''' The variance matrix Of the estimates Is found from the Hessian Of the log-likelihood, And so may only be a rough guide.
        ''' Optimization Is done by optim. It will work best if the columns in xreg are roughly scaled to zero mean And unit variance, but does attempt to estimate suitable scalings.
        ''' </remarks>
        Public Function arima(x As String,
                              Optional order As String = "c(0L, 0L, 0L)",
                              Optional seasonal As String = "list(order = c(0L, 0L, 0L), period = NA)",
                              Optional xreg As String = NULL,
                              Optional includemean As Boolean = True,
                              Optional transformpars As Boolean = True,
                              Optional fixed As String = NULL,
                              Optional init As String = NULL,
                              Optional method As String = "c(""CSS-ML"", ""ML"", ""CSS"")",
                              Optional ncond As String = NULL,
                              Optional SSinit As String = "c(""Gardner1980"", ""Rossignol2011"")",
                              Optional optimmethod As String = "BFGS",
                              Optional optimcontrol As String = "list()",
                              Optional kappa As String = "1e6") As String

            Dim out As String = App.NextTempName

            Call $"{out} <- arima({x}, order = {order},
      seasonal = {seasonal},
      xreg = {xreg}, include.mean = {includemean.λ},
      transform.pars = {transformpars.λ},
      fixed = {fixed}, init = {init},
      method = {method}, n.cond = {ncond},
      SSinit = {SSinit},
      optim.method = {Rstring(optimmethod)},
      optim.control = {optimcontrol}, kappa = {kappa})".__call

            Return out
        End Function

        ''' <summary>
        ''' Time-Series Objects, The function ts is used to create time-series objects.
        ''' </summary>
        ''' <param name="data">a vector or matrix of the observed time-series values. A data frame will be coerced to a numeric matrix via data.matrix. (See also ‘Details’.)</param>
        ''' <param name="start">the time of the first observation. Either a single number or a vector of two integers, which specify a natural time unit and a (1-based) number of samples into the time unit. See the examples for the use of the second form.</param>
        ''' <param name="[end]">the time of the last observation, specified in the same way as start.</param>
        ''' <param name="frequency">the number of observations per unit of time.</param>
        ''' <param name="deltat">the fraction of the sampling period between successive observations; e.g., 1/12 for monthly data. Only one of frequency or deltat should be provided.</param>
        ''' <param name="tseps">time series comparison tolerance. Frequencies are considered equal if their absolute difference is less than ts.eps.</param>
        ''' <param name="[class]">class to be given to the result, or none if NULL or "none". The default is "ts" for a single series, c("mts", "ts", "matrix") for multiple series.</param>
        ''' <param name="names">a character vector of names for the series in a multiple series: defaults to the colnames of data, or Series 1, Series 2, ....</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' The function ts is used to create time-series objects. These are vector or matrices with class of "ts" (and additional attributes) which represent data which has been sampled at equispaced points in time. In the matrix case, each column of the matrix data is assumed to contain a single (univariate) time series. Time series must have at least one observation, and although they need not be numeric there is very limited support for non-numeric series.
        ''' Class "ts" has a number of methods. In particular arithmetic will attempt to align time axes, And subsetting to extract subsets of series can be used (e.g., EuStockMarkets[, "DAX"]). However, subsetting the first (Or only) dimension will return a matrix Or vector, as will matrix subsetting. Subassignment can be used to replace values but Not to extend a series (see window). There Is a method for t that transposes the series as a matrix (a one-column matrix if a vector) And hence returns a result that does Not inherit from class "ts".
        ''' The value Of argument frequency Is used When the series Is sampled an integral number Of times In Each unit time interval. For example, one could use a value Of 7 For frequency When the data are sampled daily, And the natural time period Is a week, Or 12 When the data are sampled monthly And the natural time period Is a year. Values Of 4 And 12 are assumed In (e.g.) print methods To imply a quarterly And monthly series respectively.
        ''' </remarks>
        Public Function ts(Optional data As String = "NA",
                           Optional start As String = "c(1,1)",
                           Optional [end] As String = Nothing,
                           Optional frequency As Integer = 1,
                           Optional deltat As String = Nothing,
                           Optional tseps As String = Nothing,
                           Optional [class] As String = Nothing,
                           Optional names As String = Nothing) As String

            Dim tmp As String = App.NextTempName
            Dim func As New packages.stats.ts With {
                .data = data,
                .start = start,
                .end = [end],
                .frequency = frequency,
                .class = [class],
                .ts_eps = tseps,
                .names = names,
                .deltat = deltat
            }

            SyncLock R
                With R
                    .call = $"{tmp} <- {func}"
                End With
            End SyncLock

            Return tmp
        End Function

        ''' <summary>
        ''' ``p.adjust.methods``
        ''' </summary>
        Public Enum padjusts
            holm
            hochberg
            hommel
            bonferroni
            BH
            BY
            fdr
            none
        End Enum

        ''' <summary>
        ''' ###### Adjust P-values for Multiple Comparisons
        ''' 
        ''' Given a set of p-values, returns p-values adjusted using one of several methods.
        ''' </summary>
        ''' <param name="p#">numeric vector of p-values (possibly with NAs). Any other R is coerced by as.numeric.</param>
        ''' <param name="method">correction method. Can be abbreviated.</param>
        ''' <param name="n$">
        ''' number of comparisons, must be at least length(p); only set this (to non-default) when you know what you are doing!
        ''' </param>
        ''' <returns>函数返回来的是变量名，这个变量的值类型为一个Double数组向量</returns>
        Public Function padjust(p#(), Optional method As padjusts = padjusts.fdr, Optional n$ = "length(p)") As String
            Dim v$ = base.c(p, recursive:=False)

            SyncLock R
                With R
                    Dim x$ = App.NextTempName

                    .call = $"{x} <- p.adjust({v}, method = {Rstring(method.Description)}, n = length({v}));"

                    Return x
                End With
            End SyncLock
        End Function
    End Module
End Namespace
