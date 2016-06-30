Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace utils.read.table

    ''' <summary>
    ''' Reads a file in table format and creates a data frame from it, with cases corresponding to lines and variables to fields in the file.
    ''' </summary>
    Public MustInherit Class readTableAPI : Inherits IRToken

        ''' <summary>
        ''' The name Of the file which the data are To be read from. Each row Of the table appears As one line Of the file. 
        ''' If it does Not contain an absolute path, the file name Is relative To the current working directory, getwd(). 
        ''' Tilde-expansion Is performed where supported. This can be a compressed file (see file).
        ''' Alternatively, file can be a readable text-mode connection (which will be opened for reading if necessary, And if so closed (And hence destroyed) at the end of the function call). 
        ''' (If stdin() Is used, the prompts for lines may be somewhat confusing. Terminate input with a blank line Or an EOF signal, Ctrl-D on Unix And Ctrl-Z on Windows. Any pushback on stdin() will be cleared before return.)
        ''' file can also be a complete URL. (For the supported URL schemes, see the 'URLs’ section of the help for url.)
        ''' </summary>
        ''' <returns></returns>

        <Parameter("file", ValueTypes.Path)>
        Public Property file As String
        ''' <summary>
        ''' a logical value indicating whether the file contains the names of the variables as its first line. 
        ''' If missing, the value is determined from the file format: header is set to TRUE if and only if the first row contains one fewer field than the number of columns.
        ''' </summary>
        ''' <returns></returns>
        Public Property header As Boolean = False
    End Class

    ''' <summary>
    ''' [read.table]
    ''' Reads a file in table format and creates a data frame from it, with cases corresponding to lines and variables to fields in the file.
    ''' </summary>
    ''' 
    <RFunc("read.table")>
    Public Class readTable : Inherits readTableAPI

        ''' <summary>
        ''' the field separator character. Values on each line of the file are separated by this character. If sep = "" (the default for read.table) the separator is ‘white space’, that is one or more spaces, tabs, newlines or carriage returns.
        ''' </summary>
        ''' <returns></returns>
        Public Property sep As String = ""
        ''' <summary>
        ''' the set of quoting characters. To disable quoting altogether, use quote = "". See scan for the behaviour on quotes embedded in quotes. Quoting is only considered for columns read as character, which is all of them unless colClasses is specified.
        ''' </summary>
        ''' <returns></returns>
        Public Property quote As String = "\""'"
        ''' <summary>
        ''' the character used In the file For Decimal points.
        ''' </summary>
        ''' <returns></returns>
        Public Property dec As String = "."
        ''' <summary>
        ''' string indicating how to convert numbers whose conversion to double precision would lose accuracy, see type.convert. Can be abbreviated. (Applies also to complex-number inputs.)
        ''' </summary>
        ''' <returns></returns>
        Public Property numerals As RExpression = c("allow.loss", "warn.loss", "no.loss")
        ''' <summary>
        ''' a vector of row names. This can be a vector giving the actual row names, or a single number giving the column of the table which contains the row names, or character string giving the name of the table column containing the row names.
        ''' If there Is a header And the first row contains one fewer field than the number Of columns, the first column In the input Is used For the row names. Otherwise If row.names Is missing, the rows are numbered.
        ''' Using row.names = NULL forces row numbering. Missing Or NULL row.names generate row names that are considered To be 'automatic’ (and not preserved by as.matrix).
        ''' </summary>
        ''' <returns></returns>
        <Parameter("row.names")> Public Property rowNames As String
        ''' <summary>
        ''' a vector of optional names for the variables. The default is to use "V" followed by the column number.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("col.names")> Public Property colNames As String
        ''' <summary>
        ''' the default behavior of read.table is to convert character variables (which are not converted to logical, numeric or complex) to factors. 
        ''' The variable as.is controls the conversion of columns not otherwise specified by colClasses. 
        ''' Its value is either a vector of logicals (values are recycled if necessary), or a vector of numeric or character indices which specify which columns should not be converted to factors.
        ''' Note: to suppress all conversions including those of numeric columns, set colClasses = "character".
        ''' Note that As.Is Is specified per column (Not per variable) And so includes the column of row names (if any) And any columns to be skipped.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("as.is")> Public Property asIs As RExpression = "!stringsAsFactors"
        ''' <summary>
        ''' a character vector of strings which are to be interpreted as NA values. Blank fields are also considered to be missing values in logical, integer, numeric and complex fields.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("na.strings")> Public Property naStrings As String = NA
        ''' <summary>
        ''' character. A vector of classes to be assumed for the columns. Recycled as necessary. If named and shorter than required, names are matched to the column names with unspecified values are taken to be NA.
        ''' Possible values are NA (the Default, When type.convert Is used), "NULL" (When the column Is skipped), one Of the atomic vector classes (logical, Integer, numeric, complex, character, raw), Or "factor", "Date" Or "POSIXct". Otherwise there needs To be an As method (from package methods) For conversion from "character" To the specified formal Class.
        ''' Note that colClasses Is specified per column (Not per variable) And so includes the column Of row names (If any).
        ''' </summary>
        ''' <returns></returns>
        Public Property colClasses As RExpression = NA
        ''' <summary>
        ''' integer: the maximum number of rows to read in. Negative and other invalid values are ignored.
        ''' </summary>
        ''' <returns></returns>
        Public Property nrows As Integer = -1
        ''' <summary>
        ''' integer: the number of lines of the data file to skip before beginning to read data.
        ''' </summary>
        ''' <returns></returns>
        Public Property skip As Integer = 0
        ''' <summary>
        ''' logical. If TRUE then the names of the variables in the data frame are checked to ensure that they are syntactically valid variable names. 
        ''' If necessary they are adjusted (by make.names) so that they are, and also to ensure that there are no duplicates.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("check.names")> Public Property checkNames As Boolean = True
        ''' <summary>
        ''' logical. If TRUE then in case the rows have unequal length, blank fields are implicitly added. See ‘Details’.
        ''' </summary>
        ''' <returns></returns>
        Public Property fill As RExpression = "!blank.lines.skip"
        ''' <summary>
        ''' logical. Used only when sep has been specified, and allows the stripping of leading and trailing white space from unquoted character fields (numeric fields are always stripped). 
        ''' See scan for further details (including the exact meaning of ‘white space’), remembering that the columns may include the row names.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("strip.white")> Public Property stripWhite As Boolean = False
        ''' <summary>
        ''' logical: if TRUE blank lines in the input are ignored.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("blank.lines.skip")> Public Property blankLinesSkip As Boolean = True
        ''' <summary>
        ''' character: a character vector of length one containing a single character or an empty string. Use "" to turn off the interpretation of comments altogether.
        ''' </summary>
        ''' <returns></returns>
        <Parameter("comment.char")> Public Property commentChar As String = "#"
        ''' <summary>
        ''' logical. Should C-style escapes such as \n be processed Or read verbatim (the default)? 
        ''' Note that if Not within quotes these could be interpreted as a delimiter (but Not as a comment character). For more details see scan.
        ''' </summary>
        ''' <returns></returns>
        Public Property allowEscapes As Boolean = False
        ''' <summary>
        ''' logical: if TRUE, scan will flush to the end of the line after reading the last of the fields requested. This allows putting comments after the last field.
        ''' </summary>
        ''' <returns></returns>
        Public Property flush As Boolean = False
        ''' <summary>
        ''' logical: should character vectors be converted to factors? Note that this is overridden by as.is and colClasses, both of which allow finer control.
        ''' </summary>
        ''' <returns></returns>
        Public Property stringsAsFactors As RExpression = "default.stringsAsFactors()"
        ''' <summary>
        ''' character string: if non-empty declares the encoding used on a file (not a connection) so the character data can be re-encoded. 
        ''' See the ‘Encoding’ section of the help for file, the ‘R Data Import/Export Manual’ and ‘Note’.
        ''' </summary>
        ''' <returns></returns>
        Public Property fileEncoding As String = ""
        ''' <summary>
        ''' encoding to be assumed for input strings. 
        ''' It is used to mark character strings as known to be in Latin-1 or UTF-8 (see Encoding): it is not used to re-encode the input, but allows R to handle encoded strings in their native encoding (if one of those two). See ‘Value’ and ‘Note’.
        ''' </summary>
        ''' <returns></returns>
        Public Property encoding As String = "unknown"
        ''' <summary>
        ''' character string: if file is not supplied and this is, then data are read from the value of text via a text connection. 
        ''' Notice that a literal string can be used to include (small) data sets within R code.
        ''' </summary>
        ''' <returns></returns>
        Public Property text As String
        ''' <summary>
        ''' logical: should nuls be skipped?
        ''' </summary>
        ''' <returns></returns>
        Public Property skipNul As Boolean = False
    End Class

    <RFunc("read.csv")>
    Public Class readcsv : Inherits readTableAPI

        ''' <summary>
        ''' the field separator character. Values on each line of the file are separated by this character. If sep = "" (the default for read.table) the separator is ‘white space’, that is one or more spaces, tabs, newlines or carriage returns.
        ''' </summary>
        ''' <returns></returns>
        Public Property sep As String = ","
        ''' <summary>
        ''' the set of quoting characters. To disable quoting altogether, use quote = "". See scan for the behaviour on quotes embedded in quotes. Quoting is only considered for columns read as character, which is all of them unless colClasses is specified.
        ''' </summary>
        ''' <returns></returns>
        Public Property quote As String = "\"""
        ''' <summary>
        ''' the character used In the file For Decimal points.
        ''' </summary>
        ''' <returns></returns>
        Public Property dec As String = "."
        Public Property fill As Boolean = True
        <Parameter("comment.char")> Public Property commentChar As String = ""

        Sub New()
            header = True  ' 为False的时候，画heatmap的时候标签会消失，所以在这里默认为True
        End Sub

        Sub New(file As String)
            Call Me.New
            Me.file = file
        End Sub

        Public Function ReadCsv() As File
            Return [Imports](file, sep)
        End Function
    End Class

    <RFunc("read.csv2")>
    Public Class readcsv2 : Inherits readTableAPI

        ''' <summary>
        ''' the field separator character. Values on each line of the file are separated by this character. If sep = "" (the default for read.table) the separator is ‘white space’, that is one or more spaces, tabs, newlines or carriage returns.
        ''' </summary>
        ''' <returns></returns>
        Public Property sep As String = ";"
        ''' <summary>
        ''' the set of quoting characters. To disable quoting altogether, use quote = "". See scan for the behaviour on quotes embedded in quotes. Quoting is only considered for columns read as character, which is all of them unless colClasses is specified.
        ''' </summary>
        ''' <returns></returns>
        Public Property quote As String = "\"""
        ''' <summary>
        ''' the character used In the file For Decimal points.
        ''' </summary>
        ''' <returns></returns>
        Public Property dec As String = ","
        Public Property fill As Boolean = True
        <Parameter("comment.char")> Public Property commentChar As String = ""

        Public Function ReadCsv() As File
            Return [Imports](file, sep)
        End Function
    End Class

    <RFunc("read.delim")>
    Public Class readdelim : Inherits readTableAPI

        ''' <summary>
        ''' the field separator character. Values on each line of the file are separated by this character. If sep = "" (the default for read.table) the separator is ‘white space’, that is one or more spaces, tabs, newlines or carriage returns.
        ''' </summary>
        ''' <returns></returns>
        Public Property sep As String = "\t"
        ''' <summary>
        ''' the set of quoting characters. To disable quoting altogether, use quote = "". See scan for the behaviour on quotes embedded in quotes. Quoting is only considered for columns read as character, which is all of them unless colClasses is specified.
        ''' </summary>
        ''' <returns></returns>
        Public Property quote As String = "\"""
        ''' <summary>
        ''' the character used In the file For Decimal points.
        ''' </summary>
        ''' <returns></returns>
        Public Property dec As String = "."
        Public Property fill As Boolean = True
        <Parameter("comment.char")> Public Property commentChar As String = ""
    End Class

    <RFunc("read.delim2")>
    Public Class readdelim2 : Inherits readTableAPI

        ''' <summary>
        ''' the field separator character. Values on each line of the file are separated by this character. If sep = "" (the default for read.table) the separator is ‘white space’, that is one or more spaces, tabs, newlines or carriage returns.
        ''' </summary>
        ''' <returns></returns>
        Public Property sep As String = "\t"
        ''' <summary>
        ''' the set of quoting characters. To disable quoting altogether, use quote = "". See scan for the behaviour on quotes embedded in quotes. Quoting is only considered for columns read as character, which is all of them unless colClasses is specified.
        ''' </summary>
        ''' <returns></returns>
        Public Property quote As String = "\"""
        ''' <summary>
        ''' the character used In the file For Decimal points.
        ''' </summary>
        ''' <returns></returns>
        Public Property dec As String = ","
        Public Property fill As Boolean = True
        <Parameter("comment.char")> Public Property commentChar As String = ""
    End Class
End Namespace