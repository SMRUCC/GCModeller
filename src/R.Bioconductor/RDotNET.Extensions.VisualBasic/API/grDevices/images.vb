#Region "Microsoft.VisualBasic::be01469383aa2196afcc30ee3b0bc42b, RDotNET.Extensions.VisualBasic\API\grDevices\images.vb"

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

    '     Module images
    ' 
    '         Sub: tiff
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.grDevices

Namespace API.grDevices

    Public Module images

        Public Sub tiff(Optional filename As String = "Rplot%03d.tif",
                        Optional width As Integer = 480,
                        Optional height As Integer = 480,
                        Optional units As String = "px",
                        Optional pointsize As Integer = 12,
                        Optional compression As String = "c(""none"", ""rle"", ""lzw"", ""jpeg"", ""zip"", ""lzw+p"", ""zip+p"")",
                        Optional bg As String = "white",
                        Optional res As String = "NA",
                        Optional family As String = "",
                        Optional restoreConsole As Boolean = True,
                        Optional type As String = "c(""windows"", ""cairo"")",
                        Optional antialias As String = NULL)

            Call New tiff With {
                .antialias = antialias,
                .filename = filename,
                .restoreConsole = restoreConsole,
                .family = family,
                .compression = compression,
                .bg = bg,
                .height = height,
                .pointsize = pointsize,
                .res = res,
                .type = type,
                .units = units,
                .width = width
            }.__call
        End Sub
    End Module
End Namespace
