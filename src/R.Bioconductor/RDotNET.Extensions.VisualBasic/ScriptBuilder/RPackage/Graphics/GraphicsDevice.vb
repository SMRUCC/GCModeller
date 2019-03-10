﻿#Region "Microsoft.VisualBasic::8318e6e10b12c64556f6da4779c16c8d, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\Graphics\GraphicsDevice.vb"

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

    '     Module GraphicsDevice
    ' 
    '         Function: bmp, jpeg, png, tiff, WriteImage
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace SymbolBuilder.packages.Graphics

    ''' <summary>
    ''' Graphics devices for BMP, JPEG, PNG and TIFF format bitmap files. 
    ''' </summary>
    ''' 
    <Package("grDevices", Description:="Graphics devices for BMP, JPEG, PNG and TIFF format bitmap files.  
<p><strong>NOTE:  please notice that this package just generates the R language statement for write a image file from a specific plot statement.
Once you have done the function from this namespace, then you can using the Shoal Shell hybrid scripting feature to draw the image and save to a file.
</strong>"， Publisher:=“amethyst.asuka@gcmodeller.org”)>
    Public Module GraphicsDevice

        Const NA As String = "NA"

        ''' <summary>
        ''' Execute the statement that comes from the function <see cref="GraphicsDevice.bmp(String, String, Integer, Integer, String, Integer, String, String, String, Boolean, String)"/>,
        ''' <see cref="GraphicsDevice.jpeg(String, String, Integer, Integer, String, Integer, Integer, String, String, String, Boolean, String)"/>,
        ''' <see cref="GraphicsDevice.png(String, String, Integer, Integer, String, Integer, String, String, String, Boolean, String)"/>,
        ''' <see cref="GraphicsDevice.tiff(String, String, Integer, Integer, String, Integer, String, String, String, String, Boolean, String)"/>
        ''' </summary>
        ''' <param name="plot"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Write")>
        Public Function WriteImage(plot As String) As String()
            Dim STD As String() = R.WriteLine(plot)
            Return STD
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="plot">画图的语句</param>
        ''' <param name="filename">the name Of the output file, up To 511 characters. The page number Is substituted If a C Integer format Is included In the character String, As In the Default, And tilde-expansion Is performed (see path.expand). (The result must be less than 600 characters Long. See postscript For further details.) </param>
        ''' <param name="width"></param>
        ''' <param name="height"></param>
        ''' <param name="units"></param>
        ''' <param name="pointsize"></param>
        ''' <param name="bg"></param>
        ''' <param name="res"></param>
        ''' <param name="family"></param>
        ''' <param name="restoreConsole"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("bmp")>
        Public Function bmp(plot As String,
                        <MetaData.Parameter("filename", "the name of the output file, up to 511 characters. The page number is substituted if a C integer format is included in the character string, as in the default, and tilde-expansion is performed (see path.expand). 
                        (The result must be less than 600 characters long. See postscript for further details.) ")> filename As String,
                        <MetaData.Parameter("width", "the width of the device.")> Optional width As Integer = 480,
                        <MetaData.Parameter("height", "the height of the device.")> Optional height As Integer = 480,
                        <MetaData.Parameter("units", "The units in which height and width are given. Can be px (pixels, the default), in (inches), cm or mm.")> Optional units As String = "px",
                        <MetaData.Parameter("pointsize", "the default pointsize of plotted text, interpreted as big points (1/72 inch) at res ppi.")> Optional pointsize As Integer = 12,
                        <MetaData.Parameter("bg", "the initial background colour: can be overridden by setting par(""bg"").")> Optional bg As String = "white",
                        <MetaData.Parameter("res", "The nominal resolution in ppi which will be recorded in the bitmap file, if a positive integer. Also used for units other than the default. 
                        If not specified, taken as 72 ppi to set the size of text and line widths.")> Optional res As String = NA,
                        <MetaData.Parameter("family", "A length-one character vector specifying the default font family. 
                        The default means to use the font numbers on the Windows GDI versions and ""sans"" on the cairographics versions.")> Optional family As String = "",
                        <MetaData.Parameter("restoreConsole", "See the ‘Details’ section of windows. For type == ""windows"" only.")> Optional restoreConsole As Boolean = True,
                        <MetaData.Parameter("type", "Should be plotting be done using Windows GDI or cairographics?")> Optional type As String = "c(""windows"", ""cairo"")") As String

            Return $"bmp(filename = ""{filename.Replace("\", "/")}"", width = {width}, height = {height}, units = ""{units}"", pointsize = {pointsize},    
                                                                                    bg = ""{bg}"", res = {res}, family = ""{family}"", restoreConsole = {If(restoreConsole, "T", "F")}, type = {type})
            {plot}
            dev.off()"
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="plot">画图的语句</param>
        ''' <param name="filename"></param>
        ''' <param name="width"></param>
        ''' <param name="height"></param>
        ''' <param name="units"></param>
        ''' <param name="pointsize"></param>
        ''' <param name="quality"></param>
        ''' <param name="bg"></param>
        ''' <param name="res"></param>
        ''' <param name="family"></param>
        ''' <param name="restoreConsole"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("jpeg")>
        Public Function jpeg(plot As String,
                         <MetaData.Parameter("filename", "the name of the output file, up to 511 characters. The page number is substituted if a C integer format is included in the character string, as in the default, and tilde-expansion is performed (see path.expand). 
                        (The result must be less than 600 characters long. See postscript for further details.) ")> filename As String,
                         <MetaData.Parameter("width", "the width of the device.")> Optional width As Integer = 480,
                         <MetaData.Parameter("height", "the height of the device.")> Optional height As Integer = 480,
                         <MetaData.Parameter("units", "The units in which height and width are given. Can be px (pixels, the default), in (inches), cm or mm.")> Optional units As String = "px",
                         <MetaData.Parameter("pointsize", "the default pointsize of plotted text, interpreted as big points (1/72 inch) at res ppi.")> Optional pointsize As Integer = 12,
                         <MetaData.Parameter("quality", "the ‘quality’ of the JPEG image, as a percentage. Smaller values will give more compression but also more degradation of the image.")> Optional quality As Integer = 75,
                          <MetaData.Parameter("bg", "the initial background colour: can be overridden by setting par(""bg"").")> Optional bg As String = "white",
                         <MetaData.Parameter("res", "The nominal resolution in ppi which will be recorded in the bitmap file, if a positive integer. Also used for units other than the default. 
                        If not specified, taken as 72 ppi to set the size of text and line widths.")> Optional res As String = NA,
                         <MetaData.Parameter("family", "A length-one character vector specifying the default font family. 
                        The default means to use the font numbers on the Windows GDI versions and ""sans"" on the cairographics versions.")> Optional family As String = "",
                         <MetaData.Parameter("restoreConsole", "See the ‘Details’ section of windows. For type == ""windows"" only.")> Optional restoreConsole As Boolean = True,
                         <MetaData.Parameter("type", "Should be plotting be done using Windows GDI or cairographics?")> Optional type As String = "c(""windows"", ""cairo"")") As String

            Return $"jpeg(filename = ""{filename.Replace("\", "/")}"",
     width = {width}, height = {height}, units = ""{units}"", pointsize = {pointsize},
     quality = {quality},
     bg = ""{bg}"", res = {res}, family = ""{family}"", restoreConsole = {If(restoreConsole, "T", "F")},
     type = {type})
     {plot}
     dev.off()"
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="plot">画图的语句</param>
        ''' <param name="filename"></param>
        ''' <param name="width"></param>
        ''' <param name="height"></param>
        ''' <param name="units"></param>
        ''' <param name="pointsize"></param>
        ''' <param name="bg"></param>
        ''' <param name="res"></param>
        ''' <param name="family"></param>
        ''' <param name="restoreConsole"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("png")>
        Public Function png(plot As String,
                           <MetaData.Parameter("filename", "the name of the output file, up to 511 characters. The page number is substituted if a C integer format is included in the character string, as in the default, and tilde-expansion is performed (see path.expand). 
                        (The result must be less than 600 characters long. See postscript for further details.) ")> filename As String,
                           <MetaData.Parameter("width", "the width of the device.")> Optional width As Integer = 480,
                         <MetaData.Parameter("height", "the height of the device.")> Optional height As Integer = 480,
                       <MetaData.Parameter("units", "The units in which height and width are given. Can be px (pixels, the default), in (inches), cm or mm.")> Optional units As String = "px",
                        <MetaData.Parameter("pointsize", "the default pointsize of plotted text, interpreted as big points (1/72 inch) at res ppi.")> Optional pointsize As Integer = 12,
                         <MetaData.Parameter("bg", "the initial background colour: can be overridden by setting par(""bg"").")> Optional bg As String = "white",
                        <MetaData.Parameter("res", "The nominal resolution in ppi which will be recorded in the bitmap file, if a positive integer. Also used for units other than the default. 
                        If not specified, taken as 72 ppi to set the size of text and line widths.")> Optional res As String = NA,
                        <MetaData.Parameter("family", "A length-one character vector specifying the default font family. 
                        The default means to use the font numbers on the Windows GDI versions and ""sans"" on the cairographics versions.")> Optional family As String = "",
                      <MetaData.Parameter("restoreConsole", "See the ‘Details’ section of windows. For type == ""windows"" only.")> Optional restoreConsole As Boolean = True,
                        <MetaData.Parameter("type", "Should be plotting be done using Windows GDI or cairographics?")> Optional type As String = "c(""windows"", ""cairo"", ""cairo-png"")") As String

            Return $"png(filename = ""{filename.Replace("\", "/")}"",
    width = {width}, height = {height}, units = ""{units}"", pointsize = {pointsize},
    bg = ""{bg}"", res = {res}, family = ""{family}"", restoreConsole = {If(restoreConsole, "T", "F")},
    type = {type})
    {plot}
    dev.off()"
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="plot">画图的语句</param>
        ''' <param name="filename"></param>
        ''' <param name="width"></param>
        ''' <param name="height"></param>
        ''' <param name="units"></param>
        ''' <param name="pointsize"></param>
        ''' <param name="bg"></param>
        ''' <param name="res"></param>
        ''' <param name="family"></param>
        ''' <param name="restoreConsole"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("tiff")>
        Public Function tiff(plot As String,
                             <MetaData.Parameter("filename", "the name of the output file, up to 511 characters. The page number is substituted if a C integer format is included in the character string, as in the default, and tilde-expansion is performed (see path.expand). 
                        (The result must be less than 600 characters long. See postscript for further details.) ")> filename As String,
                             <MetaData.Parameter("width", "the width of the device.")> Optional width As Integer = 480,
                             <MetaData.Parameter("height", "the height of the device.")> Optional height As Integer = 480,
                             <MetaData.Parameter("units", "The units in which height and width are given. Can be px (pixels, the default), in (inches), cm or mm.")> Optional units As String = "px",
                             <MetaData.Parameter("pointsize", "the default pointsize of plotted text, interpreted as big points (1/72 inch) at res ppi.")> Optional pointsize As Integer = 12,
                             <MetaData.Parameter("compression", "the type of compression to be used.")> Optional compression As String = "c(""none"", ""rle"", ""lzw"", ""jpeg"", ""zip"", ""lzw+p"", ""zip+p"")",
                             <MetaData.Parameter("bg", "the initial background colour: can be overridden by setting par(""bg"").")> Optional bg As String = "white",
                             <MetaData.Parameter("res", "The nominal resolution in ppi which will be recorded in the bitmap file, if a positive integer. Also used for units other than the default. 
                          If not specified, taken as 72 ppi to set the size of text and line widths.")> Optional res As String = NA,
                             <MetaData.Parameter("family", "A length-one character vector specifying the default font family. 
                        The default means to use the font numbers on the Windows GDI versions and ""sans"" on the cairographics versions.")> Optional family As String = "",
                             <MetaData.Parameter("restoreConsole", "See the ‘Details’ section of windows. For type == ""windows"" only.")> Optional restoreConsole As Boolean = True,
                             <MetaData.Parameter("type", "Should be plotting be done using Windows GDI or cairographics?")> Optional type As String = "c(""windows"", ""cairo"")") As String

            Return $"tiff(filename = ""{filename.Replace("\", "/")}"",
     width = {width}, height = {height}, units = ""{units}"", pointsize = {pointsize},
     compression = {compression},
     bg = ""{bg}"", res = {res}, family = ""{family}"", restoreConsole = {If(restoreConsole, "T", "F")},
     type = {type})
     {plot}
     dev.off()"
        End Function

    End Module
End Namespace
