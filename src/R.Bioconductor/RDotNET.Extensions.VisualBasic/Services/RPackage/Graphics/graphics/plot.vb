Imports Microsoft.VisualBasic.Serialization
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace Graphics

    ''' <summary>
    ''' Generic function for plotting of R objects. For more details about the graphical parameter arguments, see par.
    ''' For simple scatter plots, plot.default will be used. 
    ''' However, there are plot methods for many R objects, including functions, data.frames, density objects, etc. Use methods(plot) And the documentation for these.
    ''' </summary>
    ''' <remarks>
    ''' The two step types differ in their x-y preference: Going from (x1,y1) to (x2,y2) with x1 &lt; x2, type = "s" moves first horizontal, then vertical, whereas type = "S" moves the other way around.
    ''' </remarks>
    <RFunc("plot")> Public Class plot : Inherits IRToken

        Default Public ReadOnly Property plotObject(x As String) As String
            Get
                Dim plot As plot = Me.ShadowsCopy
                plot.x = x
                Return plot.RScript
            End Get
        End Property

        ''' <summary>
        ''' the coordinates Of points In the plot. Alternatively, a Single plotting Structure, Function Or any R Object With a plot method can be provided.
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression

        ''' <summary>
        ''' the y coordinates Of points In the plot, Optional If x Is an appropriate Structure.
        ''' </summary>
        ''' <returns></returns>
        Public Property y As RExpression

        ' Arguments to be passed to methods, such as graphical parameters (see par). Many methods will accept the following arguments:

        ''' <summary>
        ''' what type Of plot should be drawn. Possible types are
        ''' 
        ''' "p" for points,
        ''' "l" for lines,
        ''' "b" for both,
        ''' "c" for the lines part alone of "b",
        ''' "o" for both 'overplotted’,
        ''' "h" for 'histogram’ like (or ‘high-density’) vertical lines,
        ''' "s" for stair steps,
        ''' "S" for other steps, see 'Details’ below,
        ''' "n" for no plotting.
        '''
        ''' All other types give a warning Or an Error; Using, e.g., type = "punkte" being equivalent To type = "p" For S compatibility. 
        ''' Note that some methods, e.g. plot.factor, Do Not accept this.
        ''' </summary>
        ''' <returns></returns>
        Public Property type As String

        ''' <summary>
        ''' an overall title For the plot: see title.
        ''' </summary>
        ''' <returns></returns>
        Public Property main As String

        ''' <summary>
        ''' A Sub() title for the plot: see title.
        ''' </summary>
        ''' <returns></returns>
        Public Property [sub] As String
        ''' <summary>
        ''' A title For the x axis: see title.
        ''' </summary>
        ''' <returns></returns>

        Public Property xlab As String

        ''' <summary>
        ''' A title For the y axis: see title.
        ''' </summary>
        ''' <returns></returns>
        Public Property ylab As String

        ''' <summary>
        ''' the y / x aspect ratio, see plot.window.
        ''' </summary>
        ''' <returns></returns>
        Public Property asp As RExpression

    End Class
End Namespace