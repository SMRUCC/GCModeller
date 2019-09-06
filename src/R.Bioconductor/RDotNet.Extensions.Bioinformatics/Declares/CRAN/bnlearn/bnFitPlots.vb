#Region "Microsoft.VisualBasic::8a99916aa7b98650a06cfdfb272f9994, RDotNet.Extensions.Bioinformatics\Declares\CRAN\bnlearn\bnFitPlots.vb"

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

    '     Class bnFitPlot
    ' 
    '         Properties: fitted, main, xlab, ylab
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class bnFitQqplot
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class bnFitHistogram
    ' 
    '         Properties: density
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class bnFitxyplot
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class bnFitBarchart
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class bnFitDotplot
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.base.ControlFlow
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.RScripts
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace declares.bnlearn

    ''' <summary>
    ''' Plot fitted Bayesian networks
    ''' Plot functions for the bn.fit, bn.fit.dnode and bn.fit.gnode classes, based on the lattice package.
    ''' </summary>
    Public MustInherit Class bnFitPlot : Inherits bnlearnBase

        ''' <summary>
        ''' an object of class bn.fit, bn.fit.dnode Or bn.fit.gnode.
        ''' </summary>
        ''' <returns></returns>
        Public Property fitted As RExpression
        ''' <summary>
        ''' the label of the x axis, of the y axis, and the plot title.
        ''' </summary>
        ''' <returns></returns>
        Public Property xlab As String
        ''' <summary>
        ''' the label of the x axis, of the y axis, and the plot title.
        ''' </summary>
        ''' <returns></returns>
        Public Property ylab As RExpression
        ''' <summary>
        ''' the label of the x axis, of the y axis, and the plot title.
        ''' </summary>
        ''' <returns></returns>
        Public Property main As String

        Protected Sub New(main As String, x As String, y As String)
            Me.main = main
            xlab = x
            ylab = y
        End Sub
    End Class

    ''' <summary>
    ''' bn.fit.qqplot draws a quantile-quantile plot of the residuals.
    ''' </summary>
    <RFunc("bn.fit.qqplot")> Public Class bnFitQqplot : Inherits bnFitPlot

        Sub New()
            Call MyBase.New("Normal Q-Q Plot", "Theoretical Quantiles", "Sample Quantiles")
        End Sub
    End Class

    ''' <summary>
    ''' bn.fit.histogram draws a histogram of the residuals, using either absolute or relative frequencies.
    ''' </summary>
    <RFunc("bn.fit.histogram")> Public Class bnFitHistogram : Inherits bnFitPlot

        ''' <summary>
        ''' a boolean value. If TRUE the histogram is plotted using relative frequencies, and the matching normal density is added to the plot.
        ''' </summary>
        ''' <returns></returns>
        Public Property density As Boolean = True

        Sub New()
            Call MyBase.New("Histogram of the residuals", "Residuals", ifelse("density", Rstring("Density"), Rstring("")))
        End Sub
    End Class

    ''' <summary>
    ''' bn.fit.xyplot plots the residuals versus the fitted values.
    ''' </summary>
    <RFunc("bn.fit.xyplot")> Public Class bnFitxyplot : Inherits bnFitPlot

        Sub New()
            Call MyBase.New("Residuals vs Fitted", "Fitted values", "Residuals")
        End Sub
    End Class

    ''' <summary>
    ''' bn.fit.barchart and bn.fit.dotplot plot the probabilities in the conditional probability table associated with each node.
    ''' </summary>
    <RFunc("bn.fit.barchart")> Public Class bnFitBarchart : Inherits bnFitPlot

        Sub New()
            Call MyBase.New("Conditional Probabilities", "Probabilities", "Levels")
        End Sub
    End Class

    <RFunc("bn.fit.dotplot")> Public Class bnFitDotplot : Inherits bnFitPlot

        Sub New()
            Call MyBase.New("Conditional Probabilities", "Probabilities", "Levels")
        End Sub
    End Class
End Namespace
