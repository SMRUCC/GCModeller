#Region "Microsoft.VisualBasic::e7a3ee4af2843d370c048711d45e8675, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\Graphics\grDevices\API.vb"

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

    '     Module API
    ' 
    '         Function: rgb
    ' 
    '     Class rgb
    ' 
    '         Properties: alpha, blue, green, maxColorValue, names
    '                     red
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract

Namespace SymbolBuilder.packages.grDevices

    Public Module API

        ''' <summary>
        ''' RGB Color Specification
        ''' 
        ''' This function creates colors corresponding to the given intensities (between 0 and max) of the red, green and blue primaries. The colour specification refers to the standard sRGB colorspace (IEC standard 61966).
        ''' An alpha transparency value can also be specified (As an opacity, so 0 means fully transparent And max means opaque). If alpha Is Not specified, an opaque colour Is generated.
        ''' The names argument may be used To provide names For the colors.
        ''' The values returned by these functions can be used With a col= specification In graphics functions Or In par.
        ''' </summary>
        ''' <param name="red">numeric vectors with values in [0, M] where M is maxColorValue. When this is 255, the red, blue, green, and alpha values are coerced to integers in 0:255 and the result is computed most efficiently.</param>
        ''' <param name="green">numeric vectors with values in [0, M] where M is maxColorValue. When this is 255, the red, blue, green, and alpha values are coerced to integers in 0:255 and the result is computed most efficiently.</param>
        ''' <param name="blue">numeric vectors with values in [0, M] where M is maxColorValue. When this is 255, the red, blue, green, and alpha values are coerced to integers in 0:255 and the result is computed most efficiently.</param>
        ''' <param name="alpha">numeric vectors with values in [0, M] where M is maxColorValue. When this is 255, the red, blue, green, and alpha values are coerced to integers in 0:255 and the result is computed most efficiently.</param>
        ''' <param name="names">character vector. The names for the resulting vector.</param>
        ''' <param name="maxColorValue">number giving the maximum of the color values range, see above.</param>
        ''' <returns>A character vector with elements of 7 or 9 characters, "#" followed by the red, blue, green and optionally alpha values in hexadecimal (after rescaling to 0 ... 255). The optional alpha values range from 0 (fully transparent) to 255 (opaque).
        ''' R does Not use 'premultiplied alpha’.</returns>
        ''' <remarks>
        ''' The colors may be specified by passing a matrix or data frame as argument red, and leaving blue and green missing. In this case the first three columns of red are taken to be the red, green and blue values.
        ''' Semi-transparent colors (0 &lt; alpha &lt; 1) are supported only on some devices: at the time Of writing On the pdf, windows, quartz And X11(type = "cairo") devices And associated bitmap devices (jpeg, png, bmp, tiff And bitmap). They are supported by several third-party devices such As those In packages Cairo, cairoDevice And JavaGD. Only some Of these devices support semi-transparent backgrounds.
        ''' Most other graphics devices plot semi-transparent colors As fully transparent, usually With a warning When first encountered.
        ''' NA values are Not allowed For any Of red, blue, green Or alpha.
        ''' </remarks>
        Public Function rgb(red As Double, green As Double, blue As Double, alpha As Double, Optional names As String = NULL, Optional maxColorValue As Integer = 1) As String
            Return $"rgb({red}, {green}, {blue}, {alpha}, names = {names}, maxColorValue = {maxColorValue})"
        End Function
    End Module

    ''' <summary>
    ''' RGB Color Specification
    ''' 
    ''' This function creates colors corresponding to the given intensities (between 0 and max) of the red, green and blue primaries. The colour specification refers to the standard sRGB colorspace (IEC standard 61966).
    ''' An alpha transparency value can also be specified (As an opacity, so 0 means fully transparent And max means opaque). If alpha Is Not specified, an opaque colour Is generated.
    ''' The names argument may be used To provide names For the colors.
    ''' The values returned by these functions can be used With a col= specification In graphics functions Or In par.
    ''' </summary>
    ''' <remarks>
    ''' The colors may be specified by passing a matrix or data frame as argument red, and leaving blue and green missing. In this case the first three columns of red are taken to be the red, green and blue values.
    ''' Semi-transparent colors (0 &lt; alpha &lt; 1) are supported only on some devices: at the time Of writing On the pdf, windows, quartz And X11(type = "cairo") devices And associated bitmap devices (jpeg, png, bmp, tiff And bitmap). They are supported by several third-party devices such As those In packages Cairo, cairoDevice And JavaGD. Only some Of these devices support semi-transparent backgrounds.
    ''' Most other graphics devices plot semi-transparent colors As fully transparent, usually With a warning When first encountered.
    ''' NA values are Not allowed For any Of red, blue, green Or alpha.
    ''' </remarks>
    Public Class rgb : Inherits IRToken

        ''' <summary>
        ''' numeric vectors with values in [0, M] where M is maxColorValue. When this is 255, the red, blue, green, and alpha values are coerced to integers in 0:255 and the result is computed most efficiently.
        ''' </summary>
        ''' <returns></returns>
        Public Property red As Double
        ''' <summary>
        ''' numeric vectors with values in [0, M] where M is maxColorValue. When this is 255, the red, blue, green, and alpha values are coerced to integers in 0:255 and the result is computed most efficiently.
        ''' </summary>
        ''' <returns></returns>
        Public Property green As Double
        ''' <summary>
        ''' numeric vectors with values in [0, M] where M is maxColorValue. When this is 255, the red, blue, green, and alpha values are coerced to integers in 0:255 and the result is computed most efficiently.
        ''' </summary>
        ''' <returns></returns>
        Public Property blue As Double
        ''' <summary>
        ''' numeric vectors with values in [0, M] where M is maxColorValue. When this is 255, the red, blue, green, and alpha values are coerced to integers in 0:255 and the result is computed most efficiently.
        ''' </summary>
        ''' <returns></returns>
        Public Property alpha As Double
        ''' <summary>
        ''' character vector. The names for the resulting vector.
        ''' </summary>
        ''' <returns></returns>
        Public Property names As String = NULL
        ''' <summary>
        ''' number giving the maximum of the color values range, see above.
        ''' </summary>
        ''' <returns></returns>
        Public Property maxColorValue As Integer = 1
    End Class
End Namespace
