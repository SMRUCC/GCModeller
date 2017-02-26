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
    End Module
End Namespace