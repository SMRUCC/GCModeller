#Region "Microsoft.VisualBasic::f671f6ab1e32a117a622b8cd578cd6db, ..\R.Bioconductor\RDotNET.Extensions.VisualBasic\API\base\print.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Namespace API

    Partial Public Module base

        ''' <summary>
        ''' print prints its argument and returns it invisibly (via invisible(x)). It is a generic function which means that new printing methods can be easily added for new classes.
        ''' </summary>
        ''' <param name="x">an object used to select a method.</param>
        ''' <param name="additionals">further arguments passed to or from other methods.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' The default method, print.default has its own help page. Use methods("print") to get all the methods for the print generic.
        ''' print.factor allows some customization And Is used for printing ordered factors as well.
        ''' print.table for printing tables allows other customization. As of R 3.0.0, it only prints a description in case of a table with 0-extents (this can happen if a classifier has no valid data).
        ''' See noquote As an example Of a Class whose main purpose Is a specific print method.
        ''' </remarks>
        Public Function print(x As String, ParamArray additionals As String())
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' ## S3 method For Class 'factor'
        ''' </summary>
        ''' <param name="x">an object used to select a method.</param>
        ''' <param name="quote">logical, indicating whether or not strings should be printed with surrounding quotes.</param>
        ''' <param name="maxlevels">integer, indicating how many levels should be printed for a factor; if 0, no extra "Levels" line will be printed. The default, NULL, entails choosing max.levels such that the levels print on one line of width width.</param>
        ''' <param name="width">only used when max.levels is NULL, see above.</param>
        ''' <param name="additionals">further arguments passed to or from other methods.</param>
        ''' <returns></returns>
        Public Function print(x As String,
                              Optional quote As Boolean = False,
                              Optional maxlevels As String = Nothing,
                              Optional width As String = Nothing,
                              Optional additionals As IEnumerable(Of String) = Nothing)
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' ## S3 method For Class 'table'
        ''' </summary>
        ''' <param name="x">an object used to select a method.</param>
        ''' <param name="digits">minimal number of significant digits, see print.default.</param>
        ''' <param name="quote">logical, indicating whether or not strings should be printed with surrounding quotes.</param>
        ''' <param name="naprint">character string (or NULL) indicating NA values in printed output, see print.default.</param>
        ''' <param name="zeroprint">character specifying how zeros (0) should be printed; for sparse tables, using "." can produce more readable results, similar to printing sparse matrices in Matrix.</param>
        ''' <param name="justify">character indicating if strings should left- or right-justified or left alone, passed to format.</param>
        ''' <param name="additionals">further arguments passed to or from other methods.</param>
        ''' <returns></returns>
        Public Function print(x As String,
                              Optional digits As String = Nothing,
                              Optional quote As Boolean = False,
                              Optional naprint As String = "",
                              Optional zeroprint As String = "0",
                              Optional justify As String = "none",
                              Optional additionals As IEnumerable(Of String) = Nothing)
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' ## S3 method For Class 'function'
        ''' </summary>
        ''' <param name="x">an object used to select a method.</param>
        ''' <param name="useSource">logical indicating if internally stored source should be used for printing when present, e.g., if options(keep.source = TRUE) has been in use.</param>
        ''' <param name="additionals">further arguments passed to or from other methods.</param>
        ''' <returns></returns>
        Public Function print(x As String, Optional useSource As Boolean = True, Optional additionals As IEnumerable(Of String) = Nothing)
            Throw New NotImplementedException
        End Function
    End Module
End Namespace
