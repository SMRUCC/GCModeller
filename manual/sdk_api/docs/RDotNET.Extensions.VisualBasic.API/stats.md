# stats
_namespace: [RDotNET.Extensions.VisualBasic.API](./index.md)_





### Methods

#### arima
```csharp
RDotNET.Extensions.VisualBasic.API.stats.arima(System.String,System.String,System.String,System.String,System.Boolean,System.Boolean,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String)
```
Fit an ARIMA model to a univariate time series.

|Parameter Name|Remarks|
|--------------|-------|
|x|a univariate time series|
|order|A specification of the non-seasonal part of the ARIMA model: the three integer components (p, d, q) are the AR order, the degree of differencing, and the MA order.|
|seasonal|A specification of the seasonal part of the ARIMA model, plus the period (which defaults to frequency(x)). This should be a list with components order and period, but a specification of just a numeric vector of length 3 will be turned into a suitable list with the specification as the order.|
|xreg|Optionally, a vector or matrix of external regressors, which must have the same number of rows as x.|
|includemean|Should the ARMA model include a mean/intercept term? The default is TRUE for undifferenced series, and it is ignored for ARIMA models with differencing.|
|transformpars|logical; if true, the AR parameters are transformed to ensure that they remain in the region of stationarity. Not used for method = "CSS". For method = "ML", it has been advantageous to set transform.pars = FALSE in some cases, see also fixed.|
|fixed|optional numeric vector of the same length as the total number of parameters. If supplied, only NA entries in fixed will be varied. transform.pars = TRUE will be overridden (with a warning) if any AR parameters are fixed. It may be wise to set transform.pars = FALSE when fixing MA parameters, especially near non-invertibility.|
|init|optional numeric vector of initial parameter values. Missing values will be filled in, by zeroes except for regression coefficients. Values already specified in fixed will be ignored.|
|method|fitting method: maximum likelihood or minimize conditional sum-of-squares. The default (unless there are missing values) is to use conditional-sum-of-squares to find starting values, then maximum likelihood. Can be abbreviated.|
|ncond|only used if fitting by conditional-sum-of-squares: the number of initial observations to ignore. It will be ignored if less than the maximum lag of an AR term.|
|SSinit|a string specifying the algorithm to compute the state-space initialization of the likelihood; see KalmanLike for details. Can be abbreviated.|
|optimmethod|The value passed as the method argument to optim.|
|optimcontrol|List of control parameters for optim.|
|kappa|the prior variance (as a multiple of the innovations variance) for the past observations in a differenced model. Do not reduce this.|


_returns: 
 A list of class "Arima" with components:

 coef	
 a vector Of AR, MA And regression coefficients, which can be extracted by the coef method.

 sigma2	
 the MLE Of the innovations variance.

 var.coef	
 the estimated variance matrix Of the coefficients coef, which can be extracted by the vcov method.

 loglik	
 the maximized log-likelihood (Of the differenced data), Or the approximation To it used.

 arma	
 A compact form Of the specification, As a vector giving the number Of AR, MA, seasonal AR And seasonal MA coefficients, plus the period And the number Of non-seasonal And seasonal differences.

 aic	
 the AIC value corresponding To the log-likelihood. Only valid For method = "ML" fits.

 residuals	
 the fitted innovations.

 Call    
 the matched Call.

 series	
 the name Of the series x.

 code	
 the convergence value returned by optim.

 n.cond	
 the number Of initial observations Not used In the fitting.

 nobs	
 the number Of “used” observations For the fitting, can also be extracted via nobs() And Is used by BIC.

 model	
 A list representing the Kalman Filter used In the fitting. See KalmanLike.
 _
> 
>  Different definitions of ARMA models have different signs for the AR and/or MA coefficients. The definition used here has
>  ```
>  X[t] = a[1]X[t-1] + … + a[p]X[t-p] + e[t] + b[1]e[t-1] + … + b[q]e[t-q]
>  ```
>  And so the MA coefficients differ in sign from those of S-PLUS. Further, if include.mean Is true (the default for an ARMA model), this formula applies to X - m rather than X. 
>  For ARIMA models with differencing, the differenced series follows a zero-mean ARMA model. If am xreg term Is included, a linear regression (with a constant term if include.mean 
>  Is true And there Is no differencing) Is fitted with an ARMA model for the error term.
>  The variance matrix Of the estimates Is found from the Hessian Of the log-likelihood, And so may only be a rough guide.
>  Optimization Is done by optim. It will work best if the columns in xreg are roughly scaled to zero mean And unit variance, but does attempt to estimate suitable scalings.
>  

#### chisqTest
```csharp
RDotNET.Extensions.VisualBasic.API.stats.chisqTest(System.String,System.String,System.Boolean)
```
**Pearson's Chi-squared Test for Count Data**
 
 ``chisq.test`` performs chi-squared contingency table tests and goodness-of-fit tests.

|Parameter Name|Remarks|
|--------------|-------|
|x|R对象名称或者表达式|
|y|-|
|correct|-|

> 
>  卡方检验试用条件
>  1. 随机样本数据； 
>  2. 卡方检验的理论频数不能太小. 
>  两个独立样本比较可以分以下3种情况：
>  1. 所有的理论数T≥5并且总样本量n≥40,用Pearson卡方进行检验. 
>  2. 如果理论数``T<5``但T≥1,并且n≥40,用连续性校正的卡方进行检验. 
>  3. 如果有理论数T<1或n<40,则用Fisher's检验. 
>  上述是适用于四格表.
>  R×C表卡方检验应用条件 
>  1. R×C表中理论数小于5的格子不能超过1/5； 
>  2. 不能有小于1的理论数.我的实验中也不符合R×C表的卡方检验.可以通过增加样本数、列合并来实现.
>  统计专业研究生工作室为您服务，需要专业数据分析可以找我
>  

#### ts
```csharp
RDotNET.Extensions.VisualBasic.API.stats.ts(System.String,System.String,System.String,System.Int32,System.String,System.String,System.String,System.String)
```
Time-Series Objects, The function ts is used to create time-series objects.

|Parameter Name|Remarks|
|--------------|-------|
|data|a vector or matrix of the observed time-series values. A data frame will be coerced to a numeric matrix via data.matrix. (See also ‘Details’.)|
|start|the time of the first observation. Either a single number or a vector of two integers, which specify a natural time unit and a (1-based) number of samples into the time unit. See the examples for the use of the second form.|
|[end]|the time of the last observation, specified in the same way as start.|
|frequency|the number of observations per unit of time.|
|deltat|the fraction of the sampling period between successive observations; e.g., 1/12 for monthly data. Only one of frequency or deltat should be provided.|
|tseps|time series comparison tolerance. Frequencies are considered equal if their absolute difference is less than ts.eps.|
|[class]|class to be given to the result, or none if NULL or "none". The default is "ts" for a single series, c("mts", "ts", "matrix") for multiple series.|
|names|a character vector of names for the series in a multiple series: defaults to the colnames of data, or Series 1, Series 2, ....|

> 
>  The function ts is used to create time-series objects. These are vector or matrices with class of "ts" (and additional attributes) which represent data which has been sampled at equispaced points in time. In the matrix case, each column of the matrix data is assumed to contain a single (univariate) time series. Time series must have at least one observation, and although they need not be numeric there is very limited support for non-numeric series.
>  Class "ts" has a number of methods. In particular arithmetic will attempt to align time axes, And subsetting to extract subsets of series can be used (e.g., EuStockMarkets[, "DAX"]). However, subsetting the first (Or only) dimension will return a matrix Or vector, as will matrix subsetting. Subassignment can be used to replace values but Not to extend a series (see window). There Is a method for t that transposes the series as a matrix (a one-column matrix if a vector) And hence returns a result that does Not inherit from class "ts".
>  The value Of argument frequency Is used When the series Is sampled an integral number Of times In Each unit time interval. For example, one could use a value Of 7 For frequency When the data are sampled daily, And the natural time period Is a week, Or 12 When the data are sampled monthly And the natural time period Is a year. Values Of 4 And 12 are assumed In (e.g.) print methods To imply a quarterly And monthly series respectively.
>  


