Imports Microsoft.VisualBasic.Scripting.ShoalShell.Configuration
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Protocols
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Terminal.STDIO__
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM
Imports Microsoft.VisualBasic.Net.Http

Namespace Runtime.Debugging

    ''' <summary>
    ''' 这个对象是运行于Shoal内部的，用作为调试的客户端的。当IDE启动的时候，会打开调试服务，接着通过命令行启动Shoal程序，将端口号传递给本对象，二者之间通过Tcp协议进行通信
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Debugger : Inherits ScriptEngine
        Implements I_ConsoleDeviceHandle

        ''' <summary>
        ''' 主动向IDE发送调试消息
        ''' </summary>
        ''' <remarks></remarks>
        Dim __tcpClient As AsynInvoke
        ''' <summary>
        ''' 监听来自于IDE的控制命令
        ''' </summary>
        ''' <remarks></remarks>
        Dim ReadListenerServices As TcpSynchronizationServicesSocket

        Public ReadOnly Property DebuggerExit As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ScriptEngine"></param>
        ''' <param name="DebugListenerPort">IDE调试监听器<see cref="ShoalShell.Runtime.Debugging.DebuggerListener"></see>的监听端口号</param>
        Sub New(ScriptEngine As ShoalShell.Runtime.ScriptEngine, DebugListenerPort As Integer)
            Call MyBase.New(Config.Default.SettingsData)
            __tcpClient = New Net.AsynInvoke("127.0.0.1", DebugListenerPort)
            Call $"Shoal debugger listeners at  127.0.0.1:{ DebugListenerPort}".__DEBUG_ECHO
            Call RunTask(AddressOf __startListen)
            Call Threading.Thread.Sleep(100)
            Call __sendMessage(ReadListenerServices.LocalPort, DebuggerMessage.MessageTypes.CTRL_DEBUGGER_INIT_INFO)
        End Sub

        Private Sub __sendMessage(Message As String, Type As DebuggerMessage.MessageTypes)
            Message = New DebuggerMessage() With {.Message = Message, .MessageType = Type}.GetXml
            Call RunTask(Sub() Call __tcpClient.SendMessage(Message))
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Config"></param>
        ''' <param name="DebugListenerPort">IDE调试监听器<see cref="DebuggerListener"></see>的监听端口号</param>
        Sub New(Config As Config, DebugListenerPort As Integer)
            Call Me.New(New ScriptEngine(Config), DebugListenerPort)
        End Sub

        Private Sub __startListen()
            ReadListenerServices = New TcpSynchronizationServicesSocket(AddressOf __internalProtocol, GetFirstAvailablePort)
            Try
RESTART:        ReadListenerServices.Run()
            Catch ex As Exception
                Call App.LogException(ex, NameOf(Debugger) & "::" & NameOf(__startListen))
                GoTo RESTART
            End Try
        End Sub

        Dim _InternalScriptDebugger As Runtime.Debugging.ShellScriptDebuggerModel
        Dim _RunningScript As Boolean = False

        Protected Sub InternalExecuteScript(Script As SyntaxModel)
            _InternalScriptDebugger = New ShellScriptDebuggerModel(Script, ScriptEngine:=Me)
            _RunningScript = True
            _InternalScriptDebugger.Execute()
            _InternalScriptDebugger.Free()
            _RunningScript = False
        End Sub

        Private Function __internalProtocol(uid As Long, request As RequestStream, remoteDevice As System.Net.IPEndPoint) As RequestStream
            Dim strMessage As String = request.GetUTF8String
            Dim data As DebuggerMessage = strMessage.CreateObjectFromXml(Of DebuggerMessage)(False)

            If data Is Nothing Then
                Return NetResponse.RFC_TOKEN_INVALID
            End If

            Dim currentProcess = Process.GetCurrentProcess

            If data.Pid <> currentProcess.Id Then

            End If

            If data.MessageType = DebuggerMessage.MessageTypes.OUTPUT_MESSAGE Then
                data.Message.__DEBUG_ECHO
                Return NetResponse.RFC_OK
            End If

            If data.MessageType = DebuggerMessage.MessageTypes.CTRL_PUSH_SCRIPT Then
                Call "Execute pushed script....".__DEBUG_ECHO
                Call (Sub() Call __execuPushedScript(data.Message)).BeginInvoke(Nothing, Nothing)

                If Me._RunningScript Then
                    strMessage = New DebuggerMessage() With {
                        .MessageType = DebuggerMessage.MessageTypes.OUTPUT_MESSAGE,
                        .Message = "Currently running a script, waiting for the script exit..."
                    }.GetXml
                    Return New RequestStream(0, 0, strMessage)
                Else
                    Return NetResponse.RFC_OK
                End If
            End If

            Return NetResponse.RFC_NOT_FOUND
        End Function

        Private Sub __execuPushedScript(Script As String)
            Do While Me._RunningScript
                Threading.Thread.Sleep(100)
            Loop

            Call Exec(Script)
        End Sub

        Public Function Read() As Integer Implements I_ConsoleDeviceHandle.Read
            Throw New NotImplementedException
        End Function

        Public Function ReadLine() As String Implements I_ConsoleDeviceHandle.ReadLine
            Throw New NotImplementedException
        End Function

        Public Sub WriteLine(s As String) Implements I_ConsoleDeviceHandle.WriteLine
            Call __sendMessage(s, DebuggerMessage.MessageTypes.OUTPUT_MESSAGE)
        End Sub

        Public Sub WriteLine(s As String, ParamArray args() As String) Implements I_ConsoleDeviceHandle.WriteLine
            Dim Message As String = String.Format(s, args)
            Call __sendMessage(Message, DebuggerMessage.MessageTypes.OUTPUT_MESSAGE)
        End Sub
    End Class
End Namespace