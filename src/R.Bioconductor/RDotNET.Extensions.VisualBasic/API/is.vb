Namespace API.is

    Public Module stats

        ''' <summary>
        ''' as.ts and is.ts coerce an object to a time-series and test whether an object is a time series.
        ''' </summary>
        ''' <param name="x">an arbitrary R object.</param>
        ''' <returns>is.ts tests if an object is a time series. It is generic: you can write methods to handle specific classes of objects, see InternalMethods.</returns>
        Public Function ts(x As String) As Boolean
            Return $"is.ts({x})".丶.AsBoolean
        End Function
    End Module
End Namespace