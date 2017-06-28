Imports RDotNET.Extensions.VisualBasic.API.as

Namespace Custom

    Public Module MyTools

        ''' <summary>
        ''' ```R
        ''' singleColumndf2Vector &lt;- function(df) {
        '''
        '''     df = as.vector(df)
        '''	    df = as.vector(df[,1])
        '''
        '''     return (df)
        ''' }
        ''' ```
        ''' </summary>
        ''' <param name="df$">data frame type in R language</param>
        ''' <returns></returns>
        Public Function SingleColumn2Vector(df$) As String
            df = [as].vector(df)
            df = [as].vector(df)

            Return df
        End Function
    End Module
End Namespace
