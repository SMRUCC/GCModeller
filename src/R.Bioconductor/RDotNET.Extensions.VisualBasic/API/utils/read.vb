Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace API.utils

    Public Module read

        Public Function csv(file$,
                            Optional header As Boolean = True,
                            Optional sep$ = ",",
                            Optional quote$ = "\""",
                            Optional dec$ = ".",
                            Optional fill As Boolean = True,
                            Optional commentChar$ = "") As String
            Dim var$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{var} <- read.csv({file}, 
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
        ''' <param name="file$"></param>
        ''' <param name="header"></param>
        ''' <param name="sep$"></param>
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

        End Function
    End Module
End Namespace