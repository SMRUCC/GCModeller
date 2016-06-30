Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder.RTypes
Imports RDotNet.Extensions.VisualBasic.RScripts
Imports RDotNet.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNet.Extensions.VisualBasic.base.Control

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