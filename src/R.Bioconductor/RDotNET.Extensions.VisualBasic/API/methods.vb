Namespace API

    Public Module methods

#Region "S4 object"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="x$">either the name of a class (as character string), or a class definition. If given an argument that is neither a character string nor a class definition, slotNames (only) uses class(x) instead.</param>
        ''' <returns></returns>
        Public Function slotNames(x$) As String
            Dim var$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{var} <- slotNames({x});"
                End With
            End SyncLock

            Return var
        End Function
#End Region
    End Module
End Namespace