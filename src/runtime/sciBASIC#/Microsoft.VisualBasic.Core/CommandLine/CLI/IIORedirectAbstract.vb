﻿#Region "Microsoft.VisualBasic::22e29b8ac29d418051d018487c8a4fd1, Microsoft.VisualBasic.Core\CommandLine\CLI\IIORedirectAbstract.vb"

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

    '     Interface IIORedirectAbstract
    ' 
    '         Properties: Bin, CLIArguments, StandardOutput
    ' 
    '         Function: Run, Start
    ' 
    '     Structure ProcessEx
    ' 
    '         Properties: Bin, CLIArguments, StandardOutput
    ' 
    '         Function: Run, Start, ToString
    ' 
    '         Sub: wait
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace CommandLine

    Public Interface IIORedirectAbstract

        ReadOnly Property Bin As String
        ReadOnly Property CLIArguments As String

        ''' <summary>
        ''' The target invoked process event has been exit with a specific return code.(目标派生子进程已经结束了运行并且返回了一个错误值)
        ''' </summary>
        ''' <param name="exitCode"></param>
        ''' <param name="exitTime"></param>
        ''' <remarks></remarks>
        Event ProcessExit(exitCode As Integer, exitTime As String)

        ''' <summary>
        ''' Gets the standard output for the target invoke process.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property StandardOutput As String

        ''' <summary>
        ''' Start the target process. If the target invoked process is currently on the running state, 
        ''' then this function will returns the -100 value as error code and print the warning 
        ''' information on the system console.(启动目标进程)
        ''' </summary>
        ''' <param name="WaitForExit">Indicate that the program code wait for the target process exit or not?(参数指示应用程序代码是否等待目标进程的结束)</param>
        ''' <returns>当发生错误的时候会返回错误代码，当当前的进程任然处于运行的状态的时候，程序会返回-100错误代码并在终端之上打印出警告信息</returns>
        ''' <remarks></remarks>
        Function Start(Optional WaitForExit As Boolean = False) As Integer
        ''' <summary>
        ''' 启动目标子进程，然后等待执行完毕并返回退出代码(请注意，在进程未执行完毕之前，整个线程会阻塞在这里)
        ''' </summary>
        ''' <returns></returns>
        Function Run() As Integer
    End Interface

    Public Structure ProcessEx : Implements IIORedirectAbstract

        Public Property Bin As String Implements IIORedirectAbstract.Bin
        Public Property CLIArguments As String Implements IIORedirectAbstract.CLIArguments

        Public ReadOnly Property StandardOutput As String Implements IIORedirectAbstract.StandardOutput
            Get
                Throw New NotSupportedException
            End Get
        End Property

        Public Event ProcessExit(exitCode As Integer, exitTime As String) Implements IIORedirectAbstract.ProcessExit

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function Run() As Integer Implements IIORedirectAbstract.Run
            Return Start(True)
        End Function

        Public Function Start(Optional waitForExit As Boolean = False) As Integer Implements IIORedirectAbstract.Start
            Dim proc As New Process

            Try
                proc.StartInfo = New ProcessStartInfo(Bin, CLIArguments)
                proc.Start()
            Catch ex As Exception
                ex = New Exception(Me.GetJson, ex)
                Throw ex
            End Try

            If waitForExit Then
                Call wait(proc)
                Return proc.ExitCode
            Else
                Dim h As Action(Of Process) = AddressOf wait
                Call New Thread(Sub() Call h(proc)).Start()
                Return 0
            End If
        End Function

        Private Sub wait(proc As Process)
            Call proc.WaitForExit()
            RaiseEvent ProcessExit(proc.ExitCode, Now.ToString)
        End Sub
    End Structure
End Namespace
