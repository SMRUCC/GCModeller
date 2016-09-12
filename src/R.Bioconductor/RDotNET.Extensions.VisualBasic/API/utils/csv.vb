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

        Private Sub __write(func As String, x As String, Optional file As String = "", Optional append As Boolean = False, Optional quote As Boolean = True, Optional sep As String = " ",
                              Optional eol As String = "\n", Optional na As String = "NA", Optional dec As String = ".", Optional rowNames As Boolean = True,
                              Optional colNames As Boolean = True, Optional qmethod As qmethods = qmethods.escape, Optional fileEncoding As String = "")
            Dim R As String =
                $"{func}({x}, file = {Rstring(file.UnixPath)}, append = {Rbool(append)}, quote = {Rbool(quote)}, sep = {Rstring(sep)},
            eol = {Rstring(eol)}, na = {Rstring(na)}, dec = {Rstring(dec)}, row.names = {Rbool(rowNames)},
            col.names = {Rbool(colNames)}, qmethod = {Rstring(qmethod.ToString)},
            fileEncoding = {Rstring(fileEncoding)})"

            Try
                Call R.ζ
            Catch ex As Exception
                ex = New Exception(R, ex)
                Throw ex
            End Try
        End Sub

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