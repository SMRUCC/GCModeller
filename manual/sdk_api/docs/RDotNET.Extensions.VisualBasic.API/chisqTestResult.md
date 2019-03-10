# chisqTestResult
_namespace: [RDotNET.Extensions.VisualBasic.API](./index.md)_






### Properties

#### dataName
[data] a character String giving the name(s) Of the data.
#### expected
the expected counts under the null hypothesis.
#### method
a character String indicating the type Of test performed, And whether Monte Carlo simulation Or continuity correction was used.
#### observed
the observed counts.
#### parameter
[df] the degrees Of freedom Of the approximate chi-squared distribution Of the test statistic, NA If the p-value Is computed by Monte Carlo simulation.
#### pvalue
the p-value For the test.
#### residuals
the Pearson residuals, (observed - expected) / sqrt(expected).
#### statistic
[X-squared] the value the chi-squared test statistic.
#### stdres
standardized residuals, (observed - expected) / sqrt(V), where V Is the residual cell variance (Agresti, 2007, section 2.4.5 For the Case where x Is a matrix, n * p * (1 - p) otherwise).
