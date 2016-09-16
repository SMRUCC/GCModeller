Namespace API

    Partial Module base

        ''' <summary>
        ''' Evaluate an R expression in an environment constructed from data, possibly modifying (a copy of) the original data.
        ''' </summary>
        ''' <param name="data">data to use for constructing an environment. For the default with method this may be an environment, a list, a data frame, or an integer as in sys.call. For within, it can be a list or a data frame.</param>
        ''' <param name="expr">expression to evaluate.</param>
        ''' <param name="additionals">arguments to be passed to future methods.</param>
        ''' <returns>
        ''' with is a generic function that evaluates expr in a local environment constructed from data. The environment has the caller's environment as its parent. This is useful for simplifying calls to modeling functions. (Note: if data is already an environment then this is used with its existing parent.)
        ''' Note that assignments within expr take place In the constructed environment And Not In the user's workspace.
        ''' within Is similar, except that it examines the environment after the evaluation of expr And makes the corresponding modifications to a copy of data (this may fail in the data frame case if objects are created which cannot be stored in a data frame), And returns it. within can be used as an alternative to transform.
        ''' </returns>
        Public Function [with](data As String, expr As String, ParamArray additionals As String()) As String
            Dim tmp As String = App.NextTempName

            Call $"{tmp} <- with({data}, {"{" & vbCrLf &
                                              expr & vbCrLf &
                                          "}"}, {additionals.params})".__call
            Return tmp
        End Function
    End Module
End Namespace