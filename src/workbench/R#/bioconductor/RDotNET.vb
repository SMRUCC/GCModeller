
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports RDotNET.Extensions.VisualBasic.API

<Package("R")>
Module RDotNET

    ''' <summary>
    ''' push any .NET object into R runtime environment
    ''' </summary>
    ''' <param name="any"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("push")>
    Public Function push(any As Object, Optional env As Environment = Nothing) As String
        If any Is Nothing Then
            Return "NULL"
        End If
    End Function
End Module
