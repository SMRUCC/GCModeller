Imports Microsoft.VisualBasic.WrapperClassTools.Net

Namespace Runtime.Debugging

    ''' <summary>
    ''' 这个对象是运行于Shoal内部的，用作为调试的客户端的。当IDE启动的时候，会打开调试服务，接着通过命令行启动Shoal程序，将端口号传递给本对象，二者之间通过Tcp协议进行通信
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Debugger : Inherits ShoalShell.Runtime.Objects.ShellScript
        Implements Microsoft.VisualBasic.ConsoleDevice.STDIO__.I_ConsoleDeviceHandle

        ''' <summary>
        ''' 主动向IDE发送调试消息
        ''' </summary>
        ''' <remarks></remarks>
        Dim TcpClient As Microsoft.VisualBasic.WrapperClassTools.Net.AsynchronousClient
        ''' <summary>
        ''' 监听来自于IDE的控制命令
        ''' </summary>
        ''' <remarks></remarks>
        Dim ReadListenerServices As Microsoft.VisualBasic.WrapperClassTools.Net.TcpSynchronizationServicesSocket

        Public Class DebuggerMessage
            Public Property Message As String
            Public Property Pid As Integer
            Public Property MessageType As MessageTypes

            Public Enum MessageTypes
                ''' <summary>
                ''' IDE向Shoal调试程序推送脚本
                ''' </summary>
                ''' <remarks></remarks>
                CTRL_PUSH_SCRIPT
                ''' <summary>
                ''' IDE发送终止脚本调试的信号
                ''' </summary>
                ''' <remarks></remarks>
                CTRL_KILL_SCRIPT
                ''' <summary>
                ''' IDE请求Shoal的变量内容
                ''' </summary>
                ''' <remarks></remarks>
                CTRL_GETS_MEMORY
                ''' <summary>
                ''' IDE修改Shoal内存之中的变量的内容
                ''' </summary>
                ''' <remarks></remarks>
                CTRL_MODIFY_VALUE
                ''' <summary>
                ''' 调试客户端向服务器返回初始化信息
                ''' </summary>
                ''' <remarks></remarks>
                CTRL_DEBUGGER_INIT_INFO

                ''' <summary>
                ''' Shoal向IDE发送一般的消息
                ''' </summary>
                ''' <remarks></remarks>
                OUTPUT_MESSAGE
                ''' <summary>
                ''' Shoal向IDE发送错误消息
                ''' </summary>
                ''' <remarks></remarks>
                OUTPUT_ERROR
                ''' <summary>
                ''' Shoal向IDE发送警告消息
                ''' </summary>
                ''' <remarks></remarks>
                OUTPUT_WARNING
            End Enum
        End Class

        'Dim CurrentConsole As IO.FileStream
        'Dim ConsoleSource As IO.StreamWriter

        Public ReadOnly Property DebuggerExit As Boolean
            Get
                Return _DebuggerExit
            End Get
        End Property

        Dim _DebuggerExit As Boolean = False

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="DebugListenerPort">IDE调试监听器<see cref="ShoalShell.Runtime.Debugging.DebuggerListener"></see>的监听端口号</param>
        ''' <remarks></remarks>
        Sub New(LibraryRegistry As String, DebugListenerPort As Integer, Optional ShowInitializeMessage As Boolean = False)
            Call MyBase.New(LibraryRegistry:=LibraryRegistry, ShowInitializeMessage:=ShowInitializeMessage)

            TcpClient = New WrapperClassTools.Net.AsynchronousClient("127.0.0.1", DebugListenerPort)
            'CurrentConsole = New IO.FileStream(path:=FileIO.FileSystem.GetTempFileName, mode:=IO.FileMode.OpenOrCreate, access:=IO.FileAccess.ReadWrite)
            'ConsoleSource = New IO.StreamWriter(stream:=CurrentConsole)
            'ConsoleSource.AutoFlush = True

            Call Console.WriteLine("[DEBUG] Shoal debugger listeners at    127.0.0.1:", DebugListenerPort)
            'Call Console.WriteLine("[DEBUG] Shoal debugger listeners at   {0} ====> 127.0.0.1:{1}", CurrentConsole.Name, DebugListenerPort)
            Call (Sub() Call InternalStartListening()).BeginInvoke(Nothing, Nothing)
            Call Threading.Thread.Sleep(100)
            Call (Sub() Call TcpClient.SendMessage(New Debugger.DebuggerMessage() With {.Message = ReadListenerServices.LocalPort, .MessageType = DebuggerMessage.MessageTypes.CTRL_DEBUGGER_INIT_INFO}.GetXml)).BeginInvoke(Nothing, Nothing)
            'Console.SetOut(ConsoleSource)
        End Sub

        Private Sub InternalStartListening()
            ReadListenerServices = New TcpSynchronizationServicesSocket(AddressOf InternalProtocol, GetFirstAvailablePort)
            Try
RESTART:        ReadListenerServices.Run()
            Catch ex As Exception
                Call Console.WriteLine(ex.ToString)
                GoTo RESTART
            End Try
        End Sub

        Dim _InternalScriptDebugger As Runtime.Debugging.ShellScriptDebuggerModel

        Protected Overrides Sub InternalExecuteScript(Script As Objects.ObjectModels.ShellScript)
            _InternalScriptDebugger = New Runtime.Debugging.ShellScriptDebuggerModel((From CodeLine In Script.ToArray Select CodeLine.Value).ToArray, ScriptEngine:=Me)
            _InternalScriptDebugger.Execute()
            _InternalScriptDebugger.Free()
        End Sub

        Private Function InternalProtocol(strMessage As String, remoteDevice As Net.IPEndPoint) As String

            'Call Console.WriteLine(strMessage)

            Dim data As DebuggerMessage = strMessage.CreateObjectFromXml(Of DebuggerMessage)(False)

            If data Is Nothing Then
                Return ERROR_PROTOCOL
            End If

            Dim currentProcess = Process.GetCurrentProcess

            If data.Pid <> currentProcess.Id Then

            End If

            If data.MessageType = DebuggerMessage.MessageTypes.OUTPUT_MESSAGE Then
                'Call Console.WriteLine(data.Message)
                Return ""
            End If

            If data.MessageType = DebuggerMessage.MessageTypes.CTRL_PUSH_SCRIPT Then
                'Call Console.WriteLine("[DEBUG] Execute pushed script....")
                Call (Sub() Call InternalExecutePushedScript(data.Message)).BeginInvoke(Nothing, Nothing)

                If Me._RunningScript Then
                    Return New Debugger.DebuggerMessage() With {.MessageType = DebuggerMessage.MessageTypes.OUTPUT_MESSAGE, .Message = "Currently running a script, waiting for the script exit..."}.GetXml
                Else
                    Return EXECUTE_OK
                End If
            End If

            Return ERROR_PROTOCOL
        End Function

        Private Sub InternalExecutePushedScript(Script As String)
            Do While Me._RunningScript
                Threading.Thread.Sleep(100)
            Loop

            Call EXEC(Script)
        End Sub

        Public Function Read() As Integer Implements ConsoleDevice.STDIO__.I_ConsoleDeviceHandle.Read
            Throw New NotImplementedException
        End Function

        Public Function ReadLine() As String Implements ConsoleDevice.STDIO__.I_ConsoleDeviceHandle.ReadLine
            Throw New NotImplementedException
        End Function

        Public Sub WriteLine(s As String) Implements ConsoleDevice.STDIO__.I_ConsoleDeviceHandle.WriteLine
            Call TcpClient.SendMessage(New Debugger.DebuggerMessage() With {.Message = s, .MessageType = DebuggerMessage.MessageTypes.OUTPUT_MESSAGE}.GetXml)
        End Sub

        Public Sub WriteLine(s As String, ParamArray args() As String) Implements ConsoleDevice.STDIO__.I_ConsoleDeviceHandle.WriteLine
            Dim Message As String = String.Format(s, args)
            Call TcpClient.SendMessage(New Debugger.DebuggerMessage() With {.Message = Message, .MessageType = DebuggerMessage.MessageTypes.OUTPUT_MESSAGE}.GetXml)
        End Sub
    End Class
End Namespace