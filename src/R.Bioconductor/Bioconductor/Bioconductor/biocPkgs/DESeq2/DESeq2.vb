#Region "Microsoft.VisualBasic::2e0a363f360fe245519d87b5fc6f37f5, Bioconductor\Bioconductor\biocPkgs\DESeq2\DESeq2.vb"

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

    '     Class DESeq2
    ' 
    '         Sub: New
    ' 
    '  
    ' 
    '     Properties: betaSD, dispMeanRel, interceptMean, interceptSD, m
    '                 n, sizeFactors
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace DESeq2

    <RFunc(NameOf(DESeq2))>
    Public MustInherit Class DESeq2 : Inherits IRToken

        Sub New()
            MyBase.Requires = {NameOf(DESeq2)}
        End Sub
    End Class

    ''' <summary>
    ''' Constructs a simulated dataset of Negative Binomial data from two conditions.
    ''' By default, there are no fold changes between the two conditions, but this can be adjusted with the betaSD argument.
    ''' </summary>
    ''' <remarks>
    ''' a DESeqDataSet with true dispersion, intercept and beta values in the metadata columns.
    ''' Note that the true betas are provided on the log2 scale.
    ''' </remarks>
    <RFunc(NameOf(makeExampleDESeqDataSet))> Public Class makeExampleDESeqDataSet : Inherits DESeq2
        ''' <summary>
        ''' number of rows
        ''' </summary>
        ''' <returns></returns>
        Public Property n As Integer = 1000
        ''' <summary>
        ''' number of columns
        ''' </summary>
        ''' <returns></returns>
        Public Property m As Integer = 12
        ''' <summary>
        ''' the standard deviation for non-intercept betas, i.e. beta ~ N(0,betaSD)
        ''' </summary>
        ''' <returns></returns>
        Public Property betaSD As Integer = 0
        ''' <summary>
        ''' the mean of the intercept betas (log2 scale)
        ''' </summary>
        ''' <returns></returns>
        Public Property interceptMean As Integer = 4
        ''' <summary>
        ''' the standard deviation of the intercept betas (log2 scale)
        ''' </summary>
        ''' <returns></returns>
        Public Property interceptSD As Integer = 2
        ''' <summary>
        ''' a function specifying the relationship of the dispersions on 2^trueIntercept
        ''' </summary>
        ''' <returns></returns>
        Public Property dispMeanRel As RExpression = "function(x) 4/x + 0.1"
        ''' <summary>
        ''' multiplicative factors for each sample
        ''' </summary>
        ''' <returns></returns>
        Public Property sizeFactors As RExpression = "rep(1, m)"
    End Class
End Namespace
