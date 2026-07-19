Imports tcp = Microsoft.VisualBasic.Net.Tcp

Public Class HttpServices : Implements IDisposable

    Public ReadOnly Property port As Integer

    Private disposedValue As Boolean

    Dim web As String
    Dim WithEvents background As Process

    Sub New(wwwroot As String)
        web = wwwroot
    End Sub

    Public Function StartHttp() As HttpServices
        Dim http = Interop.CreateServer
        Dim service As Integer = tcp.GetFirstAvailablePort(BEGIN_PORT:=-1)
        Dim args As String = http.GetlistenCommandLine(web, port:=service)
        Dim task = http.CreateSlave(args, workdir:=App.HOME)

        _port = service
        background = task.Start()

        Return Me
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Try
                    Call background.Kill()
                    Call background.Dispose()
                Catch ex As Exception
                    Call App.LogException(ex)
                End Try
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
