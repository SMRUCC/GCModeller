﻿Imports Microsoft.VisualBasic.WrapperClassTools.Net

Namespace Runtime.Debugging

    ''' <summary>
    ''' 这个对象为服务器对象，运行在IDE模块之中
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DebuggerListener : Implements System.IDisposable

        Dim _DebuggerListener As Microsoft.VisualBasic.WrapperClassTools.Net.TcpSynchronizationServicesSocket
        Dim _InternalMessageSender As AsynchronousClient
        Dim pid As Integer
        Dim DebuggerProcess As Microsoft.VisualBasic.CommandLine.IORedirect
        Dim LocalPort As Integer = GetFirstAvailablePort()
        Dim DebuggerStarted As Boolean = False

        ''' <summary>
        ''' /debug listener_port &lt;listen_port> [-work &lt;working_Dir>]
        ''' </summary>
        ''' <param name="debugger"></param>
        ''' <param name="WorkDir"></param>
        ''' <remarks></remarks>
        Sub New(Debugger As String, WorkDir As String)
            Call (Sub() Call InternalStartListener()).BeginInvoke(Nothing, Nothing)
            Call Threading.Thread.Sleep(100)

            Dim DebuggerArgvs As String = "/debug listener_port " & LocalPort
            If Not String.IsNullOrEmpty(WorkDir) Then DebuggerArgvs = DebuggerArgvs & " -work """ & WorkDir & """"
            Debugger = FileIO.FileSystem.GetFileInfo(Debugger).FullName
            DebuggerProcess = New Microsoft.VisualBasic.CommandLine.IORedirect(Debugger, DebuggerArgvs, _disp_debug:=True)
            Call DebuggerProcess.Start(_DISP_DEBUG_INFO:=True)
            Call InternalWaitForDebuggerStart()
            Call (Sub() SendHelloWorld()).BeginInvoke(Nothing, Nothing)
        End Sub

        Private Sub InternalStartListener()
            Try
RESTART:        _DebuggerListener = New TcpSynchronizationServicesSocket(AddressOf InternalProtocol, LocalPort)
                Call _DebuggerListener.Run()
            Catch ex As Exception
                Call Console.WriteLine(ex.ToString)
                GoTo RESTART
            End Try
        End Sub

        Private Sub InternalWaitForDebuggerStart()
            Do While Not DebuggerStarted
                Call Threading.Thread.Sleep(10)
            Loop
        End Sub

        Private Function InternalProtocol(str As String, remote As Net.IPEndPoint) As String
            Call Console.WriteLine(str)

            Dim Message As Debugger.DebuggerMessage = str.CreateObjectFromXml(Of Debugger.DebuggerMessage)(False)

            If Message Is Nothing Then
                Return ERROR_PROTOCOL
            End If

            If Message.MessageType = Debugger.DebuggerMessage.MessageTypes.CTRL_DEBUGGER_INIT_INFO AndAlso Not DebuggerStarted Then
                DebuggerStarted = True
                _InternalMessageSender = New AsynchronousClient("127.0.0.1", Val(Message.Message))
                Return EXECUTE_OK
            End If

            Return ERROR_PROTOCOL
        End Function

        Private Sub SendHelloWorld()
            Call Threading.Thread.Sleep(1500)
            pid = DebuggerProcess.ProcessInfo.Id
            Dim Message As String = SendMessage("[DEBUGGING] Hello World!")
            Call Console.WriteLine(Message)
        End Sub

        Public Function PushScript(Script As String) As String
            Dim Message = New Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Debugging.Debugger.DebuggerMessage() With
                          {
                              .Pid = pid, .MessageType = Debugger.DebuggerMessage.MessageTypes.CTRL_PUSH_SCRIPT, .Message = Script}
            Dim s_Message As String = _InternalMessageSender.SendMessage(Message.GetXml)
            Call Console.WriteLine(s_Message)
            Return s_Message
        End Function

        Public Function SendMessage(s As String) As String
            Return _InternalMessageSender.SendMessage(New Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Debugging.Debugger.DebuggerMessage() With
                                                      {
                                                          .Pid = pid,
                                                          .Message = s,
                                                          .MessageType = Debugger.DebuggerMessage.MessageTypes.OUTPUT_MESSAGE}.GetXml)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call DebuggerProcess.ProcessInfo.Kill()
                    Call DebuggerProcess.Free()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace