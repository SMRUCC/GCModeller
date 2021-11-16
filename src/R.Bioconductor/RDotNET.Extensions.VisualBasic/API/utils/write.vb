#Region "Microsoft.VisualBasic::78cdbf9e076a3a273c93a5b3717e5e58, RDotNET.Extensions.VisualBasic\API\utils\write.vb"

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

    '     Module write
    ' 
    ' 
    '         Enum qmethods
    ' 
    '             [double], escape
    ' 
    ' 
    ' 
    '  
    ' 
    '     Function: csv, csv2, table
    ' 
    '     Sub: __write
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace API.utils

    ''' <summary>
    ''' Data Output
    ''' </summary>
    Public Module write

        Public Enum qmethods
            escape
            [double]
        End Enum

        ''' <summary>
        ''' ``write.table`` prints its required argument x (after converting it to a data frame if it is not one nor a matrix) to a file or connection.
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="file"></param>
        ''' <param name="append"></param>
        ''' <param name="quote"></param>
        ''' <param name="sep"></param>
        ''' <param name="eol"></param>
        ''' <param name="na"></param>
        ''' <param name="dec"></param>
        ''' <param name="rowNames"></param>
        ''' <param name="colNames"></param>
        ''' <param name="qmethod"></param>
        ''' <param name="fileEncoding"></param>
        ''' <returns></returns>
        Public Function table(x As String, Optional file As String = "", Optional append As Boolean = False, Optional quote As Boolean = True, Optional sep As String = " ",
                              Optional eol As String = "\n", Optional na As String = "NA", Optional dec As String = ".", Optional rowNames As Boolean = True,
                              Optional colNames As Boolean = True, Optional qmethod As qmethods = qmethods.escape, Optional fileEncoding As String = "") As Boolean

            Try
                Call __write("write.table", x, file, append, quote, sep, eol, na, dec, rowNames, colNames, qmethod, fileEncoding)
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function

        ''' <summary>
        ''' 这个函数会自动创建文件夹的
        ''' </summary>
        ''' <param name="func"></param>
        ''' <param name="x"></param>
        ''' <param name="file"></param>
        ''' <param name="append"></param>
        ''' <param name="quote"></param>
        ''' <param name="sep"></param>
        ''' <param name="eol"></param>
        ''' <param name="na"></param>
        ''' <param name="dec"></param>
        ''' <param name="rowNames"></param>
        ''' <param name="colNames"></param>
        ''' <param name="qmethod"></param>
        ''' <param name="fileEncoding"></param>
        Private Sub __write(func As String, x As String, Optional file As String = "", Optional append As Boolean = False, Optional quote As Boolean = True, Optional sep As String = " ",
                              Optional eol As String = "\n", Optional na As String = "NA", Optional dec As String = ".", Optional rowNames As Boolean = True,
                              Optional colNames As Boolean = True, Optional qmethod As qmethods = qmethods.escape, Optional fileEncoding As String = "")
            Dim R As String =
                $"{func}({x}, file = {Rstring(file.UnixPath)}, append = {Rbool(append)}, quote = {Rbool(quote)}, sep = {Rstring(sep)},
            eol = {Rstring(eol)}, na = {Rstring(na)}, dec = {Rstring(dec)}, row.names = {Rbool(rowNames)},
            col.names = {Rbool(colNames)}, qmethod = {Rstring(qmethod.ToString)},
            fileEncoding = {Rstring(fileEncoding)})"

            Call file.ParentPath.MakeDir

            Try
                Call R.__call
            Catch ex As Exception
                ex = New Exception(R, ex)
                Throw ex
            End Try
        End Sub

        ''' <summary>
        ''' ``write.table`` prints its required argument x (after converting it to a data frame if it is not one nor a matrix) to a file or connection.
        ''' </summary>
        ''' <param name="x">the object to be written, preferably a matrix or data frame. If not, it is attempted to coerce x to a data frame.</param>
        ''' <param name="file"></param>
        ''' <param name="append"></param>
        ''' <param name="quote"></param>
        ''' <param name="sep"></param>
        ''' <param name="eol"></param>
        ''' <param name="na"></param>
        ''' <param name="dec"></param>
        ''' <param name="rowNames"></param>
        ''' <param name="colNames"></param>
        ''' <param name="qmethod"></param>
        ''' <param name="fileEncoding"></param>
        ''' <returns></returns>
        Public Function csv(x As String, Optional file As String = "", Optional append As Boolean = False, Optional quote As Boolean = True, Optional sep As String = " ",
                       Optional eol As String = "\n", Optional na As String = "NA", Optional dec As String = ".", Optional rowNames As Boolean = True,
                       Optional colNames As Boolean = True, Optional qmethod As qmethods = qmethods.escape, Optional fileEncoding As String = "") As Boolean
            Try
                Call __write("write.csv", x, file, append, quote, sep, eol, na, dec, rowNames, colNames, qmethod, fileEncoding)
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function

        Public Function csv2(x As String, Optional file As String = "", Optional append As Boolean = False, Optional quote As Boolean = True, Optional sep As String = " ",
               Optional eol As String = "\n", Optional na As String = "NA", Optional dec As String = ".", Optional rowNames As Boolean = True,
               Optional colNames As Boolean = True, Optional qmethod As qmethods = qmethods.escape, Optional fileEncoding As String = "") As Boolean
            Try
                Call __write("write.csv2", x, file, append, quote, sep, eol, na, dec, rowNames, colNames, qmethod, fileEncoding)
            Catch ex As Exception
                Call App.LogException(ex)
                Return False
            End Try

            Return True
        End Function
    End Module
End Namespace
