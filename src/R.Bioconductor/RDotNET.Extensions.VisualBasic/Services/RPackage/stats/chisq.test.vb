Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace stats

    Public Module API

        ''' <summary>
        ''' **Pearson's Chi-squared Test for Count Data**
        ''' 
        ''' ``chisq.test`` performs chi-squared contingency table tests and goodness-of-fit tests.
        ''' </summary>
        ''' <param name="x">R对象名称或者表达式</param>
        ''' <param name="y"></param>
        ''' <param name="correct"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("chisq.test")>
        Public Function chisqTest(x As String, Optional y As String = NULL, Optional correct As String = [TRUE]) As chisqTestResult
            Dim R As String = $"chisq.test({x},y={y},correct={correct})"
            Dim out = RServer.Evaluate(R).AsList.ToArray
            Dim i As New Pointer
            Dim result As New chisqTestResult With {
                .statistic = out(++i).AsNumeric.ToArray.First,
                .parameter = out(++i).AsInteger.ToArray.First,
                .pvalue = out(++i).AsNumeric.ToArray.First,
                .method = out(++i).AsCharacter.ToArray.First,
                .dataName = out(++i).AsCharacter.ToArray.First,
                .observed = out(++i).AsNumeric.ToArray,
                .expected = out(++i).AsNumeric.ToArray,
                .residuals = out(++i).AsNumeric.ToArray,
                .stdres = out(++i).AsNumeric.ToArray
            }

            Return result
        End Function
    End Module

    Public Class chisqTestResult

        ''' <summary>
        ''' the value the chi-squared test statistic.
        ''' </summary>
        ''' <returns></returns>
        Public Property statistic As Double
        ''' <summary>
        ''' the degrees Of freedom Of the approximate chi-squared distribution Of the test statistic, NA If the p-value Is computed by Monte Carlo simulation.
        ''' </summary>
        ''' <returns></returns>
        Public Property parameter As Integer
        ''' <summary>
        ''' the p-value For the test.
        ''' </summary>
        ''' <returns></returns>
        Public Property pvalue As Double
        ''' <summary>
        ''' a character String indicating the type Of test performed, And whether Monte Carlo simulation Or continuity correction was used.
        ''' </summary>
        ''' <returns></returns>
        Public Property method As String
        ''' <summary>
        ''' a character String giving the name(s) Of the data.
        ''' </summary>
        ''' <returns></returns>
        Public Property dataName As String
        ''' <summary>
        ''' the observed counts.
        ''' </summary>
        ''' <returns></returns>
        Public Property observed As Double()
        ''' <summary>
        ''' the expected counts under the null hypothesis.
        ''' </summary>
        ''' <returns></returns>
        Public Property expected As Double()
        ''' <summary>
        ''' the Pearson residuals, (observed - expected) / sqrt(expected).
        ''' </summary>
        ''' <returns></returns>
        Public Property residuals As Double()
        ''' <summary>
        ''' standardized residuals, (observed - expected) / sqrt(V), where V Is the residual cell variance (Agresti, 2007, section 2.4.5 For the Case where x Is a matrix, n * p * (1 - p) otherwise).
        ''' </summary>
        ''' <returns></returns>
        Public Property stdres As Double()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace