#Region "Microsoft.VisualBasic::2ab0dea2f33f69ac3aba79d3e200aede, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\Graphics\graphics\plot.vb"

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

    '     Class plot
    ' 
    '         Properties: [sub], asp, main, type, x
    '                     xlab, y, ylab
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.packages.Graphics

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
