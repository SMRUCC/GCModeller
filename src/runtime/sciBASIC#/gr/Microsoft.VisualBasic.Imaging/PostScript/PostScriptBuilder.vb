Namespace PostScript

    ''' <summary>
    ''' A helper module for convert the postscript object as ASCII script text
    ''' </summary>
    Public Class PostScriptBuilder

        Dim paint As PSElement()

        ''' <summary>
        ''' Get ascii postscript text
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Throw New NotImplementedException
        End Function
    End Class
End Namespace