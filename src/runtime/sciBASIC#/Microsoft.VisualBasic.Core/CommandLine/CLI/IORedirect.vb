﻿#Region "Microsoft.VisualBasic::78417a1280c39aa92aa5d39f87b1219e, Microsoft.VisualBasic.Core\CommandLine\CLI\IORedirect.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class IORedirect
    ' 
    '         Properties: Bin, CLIArguments, ExitCode, HasExited, PID
    '                     StandardOutput
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Delegate Function
    ' 
    ' 
    '         Delegate Sub
    ' 
    '             Function: GetError, Read, ReadLine, Run, Shell
    '                       (+3 Overloads) Start, ToString, WaitError, waitForExit, WaitForExit
    '                       WaitOutput
    ' 
    '             Sub: (+2 Overloads) Dispose, errorHandler, Kill, outputHandler, Write
    '                  (+2 Overloads) WriteLine
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Terminal.STDIO__

Namespace CommandLine

    ''' <summary>
    ''' A communication fundation class type for the commandline program interop.
    ''' (一个简单的用于从当前进程派生子进程的Wrapper对象，假若需要folk出来的子进程对象
    ''' 不需要终端交互功能，则更加推荐使用<see cref="IORedirectFile"/>对象来进行调用)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class IORedirect : Implements I_ConsoleDeviceHandle
        Implements IDisposable, IIORedirectAbstract

        ''' <summary>
        ''' 当前的这个进程实例是否处于运行的状态
        ''' </summary>
        ''' <remarks></remarks>
        Dim processIsRunning As Boolean = False
        Dim outputWaitHandle As New AutoResetEvent(False)
        Dim errorWaitHandle As New AutoResetEvent(False)

        ''' <summary>
        ''' The target invoked process event has been exit with a specific return code.
        ''' (目标派生子进程已经结束了运行并且返回了一个错误值)
        ''' </summary>
        ''' <param name="exitCode"></param>
        ''' <param name="exitTime"></param>
        ''' <remarks></remarks>
        Public Event ProcessExit(exitCode As Integer, exitTime As String) Implements IIORedirectAbstract.ProcessExit
        Public Event PrintOutput(output As String)

        ''' <summary>
        ''' The process invoke interface of current I/O redirect operation.
        ''' </summary>
        ''' <remarks></remarks>
        Dim WithEvents processInfo As Process

        ''' <summary>
        ''' Gets the standard output for the target invoke process.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property StandardOutput As String Implements IIORedirectAbstract.StandardOutput
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return output.ToString
            End Get
        End Property

        Public ReadOnly Property Bin As String Implements IIORedirectAbstract.Bin
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return processInfo.StartInfo.FileName
            End Get
        End Property

        Public ReadOnly Property CLIArguments As String Implements IIORedirectAbstract.CLIArguments
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return processInfo.StartInfo.Arguments
            End Get
        End Property

        Public ReadOnly Property ExitCode As Integer
            Get
                Return processInfo.ExitCode
            End Get
        End Property

        Public ReadOnly Property HasExited As Boolean
            Get
                Return processInfo.HasExited
            End Get
        End Property

        Public ReadOnly Property PID As Integer
            Get
                Return processInfo.Id
            End Get
        End Property

        Dim input As StreamWriter
        Dim output As New StringBuilder(1024)
        Dim [error] As New StringBuilder()
        Dim IOredirect As Boolean
        Dim displayOutput As Boolean = False

        ''' <summary>
        ''' Creates a <see cref="System.Diagnostics.Process"/> wrapper for the CLI program operations.
        ''' (在服务器上面可能会有一些线程方面的兼容性BUG的问题，不太清楚为什么会导致这样)
        ''' </summary>
        ''' <param name="Exe">The file path of the executable file.</param>
        ''' <param name="args">
        ''' The CLI arguments for the folked program.
        ''' 
        ''' (程序会自动将这个参数之中的换行符替换为空格.)
        ''' </param>
        ''' <param name="ENV">Set up the environment variable for the target invoked child process.</param>
        ''' <param name="displayDebug"></param>
        ''' <param name="displayStdOut">是否显示目标被调用的外部程序的标准输出</param>
        ''' <remarks></remarks>
        Public Sub New(exe$, Optional args$ = "",
                       Optional ENV As IEnumerable(Of KeyValuePair(Of String, String)) = Nothing,
                       Optional IOredirect As Boolean = True,
                       Optional displayStdOut As Boolean = True,
                       Optional displayDebug As Boolean = False)

            Dim program$ = exe.GetString(""""c)
            Dim pInfo As New ProcessStartInfo(program, args.TrimNewLine.Trim) With {
                .UseShellExecute = False
            }

            If IOredirect Then  '只是重定向输出设备流
                pInfo.RedirectStandardOutput = True
                pInfo.RedirectStandardError = True
                pInfo.RedirectStandardInput = True
            End If

            pInfo.RedirectStandardInput = True
            pInfo.ErrorDialog = False
            pInfo.WindowStyle = ProcessWindowStyle.Hidden
            pInfo.CreateNoWindow = True

            If Not ENV Is Nothing Then
                For Each para As KeyValuePair(Of String, String) In ENV
                    Call pInfo.EnvironmentVariables.Add(para.Key, para.Value)
                Next
            End If

            Me.IOredirect = IOredirect
            Me.displayOutput = displayStdOut
            Me.processInfo = New Process With {
                .EnableRaisingEvents = True,
                .StartInfo = pInfo
            }

            If displayDebug Then
                Call $"""{exe}"" {args}".__DEBUG_ECHO
                Call $"disp_STDOUT -> ${  "Yes!" Or "NO!".When(Not Me.displayOutput) }".__DEBUG_ECHO
            End If
        End Sub

        Public Delegate Function ProcessAyHandle(WaitForExit As Boolean, PushingData As String(), _DISP_DEBUG_INFO As Boolean) As Integer

        ''' <summary>
        ''' A function pointer for process the events when the target invoked child process was terminated and exit.
        ''' (当目标进程退出的时候所调用的过程)
        ''' </summary>
        ''' <param name="exitCode">The exit code for the target sub invoke process.进程的退出代码</param>
        ''' <param name="exitTime">The exit time for the target sub invoke process.(进程的退出时间)</param>
        ''' <remarks></remarks>
        Public Delegate Sub ProcessExitCallback(exitCode As Integer, exitTime As String)

        Private Sub outputHandler(sender As Object, e As DataReceivedEventArgs) Handles processInfo.OutputDataReceived
            If e.Data Is Nothing Then
                Call outputWaitHandle.[Set]()
            Else
                Call output.AppendLine(e.Data)
            End If
        End Sub

        Private Sub errorHandler(sender As Object, e As DataReceivedEventArgs) Handles processInfo.ErrorDataReceived
            If e.Data Is Nothing Then
                Call errorWaitHandle.[Set]()
            Else
                Call [error].AppendLine(e.Data)
            End If
        End Sub

        Public Sub Kill()
            Call processInfo.Kill()
        End Sub

        ''' <summary>
        ''' Gets a <see cref="String"/> used to read the error output of the application.
        ''' </summary>
        ''' <returns>A <see cref="String"/> text value that read from the std_error of <see cref="StreamReader"/> 
        ''' that can be used to read the standard error stream of the application.</returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetError() As String
            Return [error].ToString
        End Function

        ''' <summary>
        ''' Start the target process. If the target invoked process is currently on the running state, 
        ''' then this function will returns the -100 value as error code and print the warning 
        ''' information on the system console.(启动目标进程)
        ''' </summary>
        ''' <param name="WaitForExit">
        ''' Indicate that the program code wait for the target process exit or not?
        ''' (参数指示应用程序代码是否等待目标进程的结束)
        ''' </param>
        ''' <returns>
        ''' 当发生错误的时候会返回错误代码，当当前的进程任然处于运行的状态的时候，程序会返回-100错误代码并在终端之上打印出警告信息
        ''' </returns>
        ''' <remarks></remarks>
        Public Function Start(Optional waitForExit As Boolean = False,
                              Optional pushingData As String() = Nothing,
                              Optional displaDebug As Boolean = False) As Integer

            If processIsRunning Then
                Dim msg As String = $"Target process ""{processInfo.StartInfo.FileName.ToFileURL}"" is currently in the running state, Operation abort!"
                Call VBDebugger.Warning(msg)
                Return -100
            End If

            If displaDebug Then
                Dim Exe As String = FileIO.FileSystem.GetFileInfo(processInfo.StartInfo.FileName).FullName.Replace("\", "/")
                Dim argvs As String = (" " & processInfo.StartInfo.Arguments).Trim

                Call Console.WriteLine("   --> system(""file:""{0}""{1})", Exe, argvs)
            End If

            Try
                Call processInfo.Start()

                If IOredirect Then
                    Call processInfo.BeginOutputReadLine()
                    Call processInfo.BeginErrorReadLine()
                End If
            Catch ex As Exception
                Call printf("FATAL_ERROR::%s", ex.ToString)
                Call Console.WriteLine("  Exe ==> " & processInfo.StartInfo.FileName)
                Call Console.WriteLine("argvs ==> " & processInfo.StartInfo.Arguments)

                ' Return ex.HResul '4.5
                Return -1
            End Try

            processIsRunning = True
            input = processInfo.StandardInput

            If Not pushingData.IsNullOrEmpty Then
                For Each line As String In pushingData
                    Call input.WriteLine(line)
                    Call input.Flush()

                    Call Console.WriteLine("  >>>   " & line)
                Next
            End If

            If waitForExit Then
                ' 请注意这个函数名称是和当前的这个函数参数是一样的
                ' 会需要me来区别引用
                Return Me.waitForExit
            Else
                Call RunTask(AddressOf Me.waitForExit)
                Return 0
            End If
        End Function

        Private Function waitForExit() As Integer
            Dim exitCode%

            Call processInfo.WaitForExit()

            Try
                ' process exit and raise events
                exitCode = processInfo.ExitCode
                RaiseEvent ProcessExit(exitCode, Process.ExitTime)
            Catch ex As Exception

            End Try

            Return exitCode
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function WaitOutput(timeout As Integer) As Boolean
            Return outputWaitHandle.WaitOne(timeout)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function WaitError(timeout As Integer) As Boolean
            Return errorWaitHandle.WaitOne(timeout)
        End Function

        ''' <summary>
        ''' With a given timeout in milliseconds unit
        ''' </summary>
        ''' <param name="timeout"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function WaitForExit(timeout As Integer) As Boolean
            Return processInfo.WaitForExit(timeout)
        End Function

        ''' <summary>
        ''' Start the target process.(启动目标进程)
        ''' </summary>
        ''' <returns>当发生错误的时候会返回错误代码</returns>
        ''' <remarks></remarks>
        Public Function Start(processExitCallback As ProcessExitCallback,
                              Optional pushData As String() = Nothing,
                              Optional displayDebug As Boolean = False) As Integer

            AddHandler ProcessExit, Sub(exitCode As Integer, exitTime As String)
                                        Call processExitCallback(exitCode, exitTime)
                                    End Sub
            Return Start(
                waitForExit:=False,
                pushingData:=pushData,
                displaDebug:=displayDebug
            )
        End Function

        ''' <summary>
        ''' Gets the value that the associated process specified when it terminated.
        ''' </summary>
        ''' <param name="WaitForExit"></param>
        ''' <returns>The code that the associated process specified when it terminated.</returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Start(Optional waitForExit As Boolean = False) As Integer Implements IIORedirectAbstract.Start
            Return Start(waitForExit, Nothing, True)
        End Function

        ''' <summary>
        ''' 线程会被阻塞在这里，直到外部应用程序执行完毕
        ''' </summary>
        ''' <returns></returns>
        Public Function Run() As Integer Implements IIORedirectAbstract.Run
            Return Start(waitForExit:=True)
        End Function

        Public Sub WriteLine(Optional s As String = "") Implements I_ConsoleDeviceHandle.WriteLine
            If s.StringEmpty Then
                Call input.WriteLine()
            Else
                Call input.WriteLine(s)
            End If

            Call input.Flush()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Write(buffer As Byte())
            Call input.BaseStream.Write(buffer, Scan0, buffer.Length)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"{Bin} {CLIArguments}"
        End Function

        ''' <summary>
        ''' 在进行隐士转换的时候，假若可执行文件的文件路径之中含有空格，则这个时候应该要特别的小心
        ''' </summary>
        ''' <param name="CLI"></param>
        ''' <returns></returns>
        Public Shared Widening Operator CType(CLI As String) As IORedirect
            Dim tokens As String() = Regex.Split(CLI, SPLIT_REGX_EXPRESSION)
            Dim EXE As String = tokens.First
            Dim args As String = Mid$(CLI, Len(EXE) + 1)

            Return New IORedirect(EXE, args)
        End Operator

        Public Shared Function Shell(commandLine As String) As IORedirect
            Return CType(commandLine, IORedirect)
        End Function

        Private Function Read() As Integer Implements I_ConsoleDeviceHandle.Read
            Return output.Length
        End Function

        Private Function ReadLine() As String Implements I_ConsoleDeviceHandle.ReadLine
            Return ""
        End Function

        Public Sub WriteLine(s$, ParamArray args() As String) Implements I_ConsoleDeviceHandle.WriteLine
            Call input.WriteLine(String.Format(s, args))
            Call input.Flush()
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call processInfo.Close()
                    Call processInfo.Dispose()
                    Call outputWaitHandle.Dispose()
                    Call errorWaitHandle.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(      disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(      disposing As Boolean) above.
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
