Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports CliPipeline = Microsoft.VisualBasic.CommandLine

Public Class IPCHost

    ReadOnly host As TcpServicesSocket
    ReadOnly resources As Dictionary(Of String, Resource)

    Public Sub Register(name$, size&, type As TypeInfo)
        Call CliPipeline.OpenForWrite($"memory:/{name}", size)
    End Sub

    Public Sub Delete(name As String)

    End Sub

End Class
