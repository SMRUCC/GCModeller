#Region "Microsoft.VisualBasic::b341a253408bab458ce705dcd2067019, RDotNET.Extensions.VisualBasic\API\utils\read.vb"

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

'     Module read
' 
'         Function: csv, table
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace API.utils

    ''' <summary>
    ''' Data Input
    ''' </summary>
    ''' 
    <Package("R.utils.read")>
    Public Module read

        ''' <summary>
        ''' Read a csv file and then returns a temp variable
        ''' </summary>
        ''' <param name="file$"></param>
        ''' <param name="header"></param>
        ''' <param name="sep$"></param>
        ''' <param name="quote$"></param>
        ''' <param name="dec$"></param>
        ''' <param name="fill"></param>
        ''' <param name="commentChar$"></param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("read.csv")>
        Public Function csv(file$,
                            Optional header As Boolean = True,
                            Optional sep$ = ",",
                            Optional quote$ = "\""",
                            Optional dec$ = ".",
                            Optional fill As Boolean = True,
                            Optional commentChar$ = "") As String
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- read.csv({Rstring(file.UnixPath)}, 
header = {Rbool(header)}, 
sep = {Rstring(sep)}, 
quote = {Rstring(quote)},
dec = {Rstring(dec)}, 
fill = {Rbool(fill)}, 
comment.char = {Rstring(commentChar)});"
                End With
            End SyncLock

            Return var
        End Function

        Public Const allow_loss$ = "allow.loss"
        Public Const warn_loss$ = "warn.loss"
        Public Const no_loss$ = "no.loss"

        ''' <summary>
        ''' Reads a file in table format and creates a data frame from it, with cases corresponding to lines and variables to fields in the file.
        ''' </summary>
        ''' <param name="file$">
        ''' the name of the file which the data are to be read from. Each row of the table appears as one line of the file. If it does not contain an absolute path, the file name is relative to the current working directory, getwd(). Tilde-expansion is performed where supported. This can be a compressed file (see file).
        '''
        ''' Alternatively, file can be a readable text-mode connection (which will be opened for reading if necessary, And if so closed (And hence destroyed) at the end of the function call). (If stdin() Is used, the prompts for lines may be somewhat confusing. Terminate input with a blank line Or an EOF signal, Ctrl-D on Unix And Ctrl-Z on Windows. Any pushback on stdin() will be cleared before return.)
        '''
        ''' file can also be a complete URL. (For the supported URL schemes, see the 'URLs’ section of the help for url.)
        ''' </param>
        ''' <param name="header">
        ''' a logical value indicating whether the file contains the names of the variables as its first line. If missing, 
        ''' the value is determined from the file format: header is set to TRUE if and only if the first row contains 
        ''' one fewer field than the number of columns.
        ''' </param>
        ''' <param name="sep$">
        ''' the field separator character. Values on each line of the file are separated by this character. 
        ''' If sep = "" (the default for read.table) the separator is ‘white space’, that is one or more 
        ''' spaces, tabs, newlines or carriage returns.
        ''' </param>
        ''' <param name="quote$"></param>
        ''' <param name="dec$"></param>
        ''' <param name="numerals$">
        ''' 
        ''' + <see cref="allow_loss"/>
        ''' + <see cref="warn_loss"/>
        ''' + <see cref="no_loss"/>
        ''' 
        ''' </param>
        ''' <param name="rowNames$"></param>
        ''' <param name="colNames$"></param>
        ''' <param name="asIs$"></param>
        ''' <param name="NAstrings$"></param>
        ''' <param name="colClasses$"></param>
        ''' <param name="nrows%"></param>
        ''' <param name="skip%"></param>
        ''' <param name="checknames"></param>
        ''' <param name="fill$"></param>
        ''' <param name="stripwhite"></param>
        ''' <param name="blanklinesskip"></param>
        ''' <param name="commentChar$"></param>
        ''' <param name="allowEscapes"></param>
        ''' <param name="flush"></param>
        ''' <param name="stringsAsFactors$"></param>
        ''' <param name="fileEncoding$"></param>
        ''' <param name="encoding$"></param>
        ''' <param name="text$"></param>
        ''' <param name="skipNul"></param>
        ''' <returns></returns>
        Public Function table(file$, Optional header As Boolean = False, Optional sep$ = "", Optional quote$ = "\""'",
                              Optional dec$ = ".", Optional numerals$ = allow_loss,
                              Optional rowNames$ = NULL, Optional colNames$ = NULL, Optional asIs$ = "!stringsAsFactors",
                              Optional NAstrings$ = "NA", Optional colClasses$ = "NA", Optional nrows% = -1,
                              Optional skip% = 0, Optional checknames As Boolean = True, Optional fill$ = "!blank.lines.skip",
                              Optional stripwhite As Boolean = False, Optional blanklinesskip As Boolean = True,
                              Optional commentChar$ = "#",
                              Optional allowEscapes As Boolean = False, Optional flush As Boolean = False,
                              Optional stringsAsFactors$ = "default.stringsAsFactors()",
                              Optional fileEncoding$ = "", Optional encoding$ = "unknown", Optional text$ = NULL, Optional skipNul As Boolean = False) As String
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- read.table({Rstring(file.UnixPath)}, header = {Rbool(header)}, sep = {Rstring(seq)}, quote = {Rstring(quote)},
dec = {Rstring(dec)}, numerals = {Rstring(numerals)},
row.names = {rowNames}, col.names = {colNames}, as.is = {asIs},
na.strings = {Rstring(NAstrings)}, colClasses = {colClasses}, nrows = {nrows},
skip = {skip}, check.names = {Rbool(checknames)}, fill = {fill},
strip.white = {Rbool(stripwhite)}, blank.lines.skip = {Rbool(blanklinesskip)},
comment.char = {Rstring(commentChar)},
allowEscapes = {Rbool(allowEscapes)}, flush = {Rbool(flush)},
stringsAsFactors = {stringsAsFactors},
fileEncoding = {Rstring(fileEncoding)}, encoding = {Rstring(encoding)}, text = {Rstring(text)}, skipNul = {Rbool(skipNul)})"
                End With
            End SyncLock

            Return var
        End Function
    End Module
End Namespace
