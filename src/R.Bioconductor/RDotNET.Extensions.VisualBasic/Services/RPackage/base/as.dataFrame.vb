Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace base.as.data

    ''' <summary>
    ''' Functions to check if an object is a data frame, or coerce it if possible.
    ''' </summary>
    Public Module DataFrameAPI

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="x">any R object.</param>
        ''' <param name="rowNames">NULL or a character vector giving the row names for the data frame. Missing values are not allowed.</param>
        ''' <param name="[optional]">logical. If TRUE, setting row names and converting column names (to syntactic names: see make.names) is optional.</param>
        ''' <returns></returns>
        Public Function frame(x As String, Optional rowNames As String = NULL, Optional [optional] As Boolean = False) As RExpression
            Return $"as.data.frame({x}, row.names={rowNames}, optional={New RBoolean([optional])})"
        End Function
    End Module
End Namespace