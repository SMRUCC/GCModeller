Namespace Framework.DynamicCode

    Public Interface ICodeProvider
        ''' <summary>
        ''' Closure token code
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Code As String
    End Interface

    Public Interface IProjectProvider

        ''' <summary>
        ''' Data projects code
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Projects As String()
    End Interface
End Namespace