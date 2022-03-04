
Namespace Tamir.SharpSsh.java.lang
    ''' <summary>
    ''' Summary description for Thread.
    ''' </summary>
    Public Class Thread
        Private t As Global.System.Threading.Thread

        Public Sub New(ByVal t As Global.System.Threading.Thread)
            Me.t = t
        End Sub

        Public Sub New(ByVal ts As Global.System.Threading.ThreadStart)
            Me.New(New Global.System.Threading.Thread(ts))
        End Sub

        Public Sub New(ByVal r As Runnable)
            Me.New(New Global.System.Threading.ThreadStart(AddressOf r.run))
        End Sub

        Public Sub setName(ByVal name As String)
            t.Name = name
        End Sub

        Public Sub start()
            t.Start()
        End Sub

        Public Function isAlive() As Boolean
            Return t.IsAlive
        End Function

        Public Sub yield()
        End Sub

        Public Sub interrupt()
            Try
                t.Interrupt()
            Catch
            End Try
        End Sub

        Public Sub notifyAll()
            Global.System.Threading.Monitor.PulseAll(Me)
        End Sub

        Public Shared Sub Sleep(ByVal t As Integer)
            Global.System.Threading.Thread.Sleep(t)
        End Sub

        Public Shared Sub sleepMethod(ByVal t As Integer)
            Sleep(t)
        End Sub

        Public Shared Function currentThread() As Thread
            Return New Thread(Global.System.Threading.Thread.CurrentThread)
        End Function
    End Class
End Namespace
