Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace base

    Public Module DataFrameAPI

        Public Function rownames(x As String, Optional doNULL As Boolean = True, Optional prefix As String = "row") As String
            Return $"rownames({x}, do.NULL = {New RBoolean(doNULL)}, prefix= ""{prefix}"")"
        End Function

        Public Function colnames(x As String, Optional doNULL As Boolean = True, Optional prefix As String = "col") As String
            Return $"colnames({x}, do.NULL = {New RBoolean(doNULL)}, prefix= ""{prefix}"")"
        End Function
    End Module
End Namespace