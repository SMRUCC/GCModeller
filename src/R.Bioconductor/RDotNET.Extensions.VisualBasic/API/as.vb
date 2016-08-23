Namespace API.as

    Public Module stats

        ''' <summary>
        ''' as.ts and is.ts coerce an object to a time-series and test whether an object is a time series.
        ''' </summary>
        ''' <param name="x">an arbitrary R object.</param>
        ''' <param name="additionals">arguments passed to methods (unused for the default method).</param>
        ''' <returns>as.ts is generic. Its default method will use the tsp attribute of the object if it has one to set the start and end times and frequency.</returns>
        Public Function ts(x As String, ParamArray additionals As String()) As String
            Dim out As String = App.NextTempName
            Call $"{out} <- as.ts({x}, {String.Join(",", additionals)})".ζ
            Return out
        End Function
    End Module
End Namespace