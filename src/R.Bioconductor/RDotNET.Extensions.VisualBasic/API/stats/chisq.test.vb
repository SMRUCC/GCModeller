#Region "Microsoft.VisualBasic::cf94ecf142e14dd8c6348dd5e9d0df41, RDotNET.Extensions.VisualBasic\API\stats\chisq.test.vb"

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

    '     Module stats
    ' 
    '         Function: chisqTest
    ' 
    '     Class chisqTestResult
    ' 
    '         Properties: dataName, expected, method, observed, parameter
    '                     pvalue, residuals, statistic, stdres
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace API

    Partial Public Module stats

        ''' <summary>
        ''' **Pearson's Chi-squared Test for Count Data**
        ''' 
        ''' ``chisq.test`` performs chi-squared contingency table tests and goodness-of-fit tests.
        ''' </summary>
        ''' <param name="x">R对象名称或者表达式</param>
        ''' <param name="y"></param>
        ''' <param name="correct"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 卡方检验试用条件
        ''' 1. 随机样本数据； 
        ''' 2. 卡方检验的理论频数不能太小. 
        ''' 两个独立样本比较可以分以下3种情况：
        ''' 1. 所有的理论数T≥5并且总样本量n≥40,用Pearson卡方进行检验. 
        ''' 2. 如果理论数``T&lt;5``但T≥1,并且n≥40,用连续性校正的卡方进行检验. 
        ''' 3. 如果有理论数T&lt;1或n&lt;40,则用Fisher's检验. 
        ''' 上述是适用于四格表.
        ''' R×C表卡方检验应用条件 
        ''' 1. R×C表中理论数小于5的格子不能超过1/5； 
        ''' 2. 不能有小于1的理论数.我的实验中也不符合R×C表的卡方检验.可以通过增加样本数、列合并来实现.
        ''' 统计专业研究生工作室为您服务，需要专业数据分析可以找我
        ''' </remarks>
        <ExportAPI("chisq.test")>
        Public Function chisqTest(x As String, Optional y As String = NULL, Optional correct As Boolean = True) As chisqTestResult
            Dim out As SymbolicExpression() =
                $"chisq.test({x},y={y},correct={correct.λ})".__call.AsList.ToArray
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
        ''' [X-squared] the value the chi-squared test statistic.
        ''' </summary>
        ''' <returns></returns>
        Public Property statistic As Double
        ''' <summary>
        ''' [df] the degrees Of freedom Of the approximate chi-squared distribution Of the test statistic, NA If the p-value Is computed by Monte Carlo simulation.
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
        ''' [data] a character String giving the name(s) Of the data.
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
