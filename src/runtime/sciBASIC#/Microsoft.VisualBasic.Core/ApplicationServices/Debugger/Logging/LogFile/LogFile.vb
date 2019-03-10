﻿#Region "Microsoft.VisualBasic::114172961e80001497cd83b56e46bcad, Microsoft.VisualBasic.Core\ApplicationServices\Debugger\Logging\LogFile\LogFile.vb"

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

    '     Class LogFile
    ' 
    '         Properties: FileName, NowTimeNormalizedString
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __getDefaultPath, Read, ReadLine, Save, SaveLog
    '                   SystemInfo, ToString
    ' 
    '         Sub: Dispose, (+2 Overloads) LogException, (+4 Overloads) WriteLine
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Terminal.STDIO__

Namespace ApplicationServices.Debugging.Logging

    ''' <summary>
    ''' 日志文件记录模块。因为在Linux平台上面，Windows的日志记录API可能不会正常工作，
    ''' 所以需要这个日志模块来接替Windows的日志模块的工作
    ''' </summary>
    ''' <remarks>
    ''' 这个类模块将输入的信息格式化保存到文本文件之中，记录的信息包括信息头，消息文本，以及消息等级
    ''' </remarks>
    Public Class LogFile : Inherits ITextFile
        Implements ISaveHandle
        Implements IDisposable
        Implements I_ConsoleDeviceHandle

        Dim buffer As TextWriter
        Dim counts&

        ''' <summary>
        ''' 没有路径名称和拓展名，仅包含有单独的文件名
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property FileName As String
            Get
                Return MyBase.FilePath.BaseName
            End Get
        End Property

        ''' <summary>
        ''' 将时间字符串里面的":"符号去除之后，剩余的字符串可以用于作为路径来使用
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property NowTimeNormalizedString As String
            Get
                Return $"{Format(Now.Month, "00")}{ Format(Now.Day, "00")}{Format(Now.Hour, "00")}{Format(Now.Minute, "00")}{Format(Now.Second, "00")}"
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Path">This logfile will saved to.</param>
        ''' <remarks></remarks>
        ''' <param name="bufferSize">当日志的记录数目达到这个数目的时候就会将日志数据写入到文件之中</param>
        Public Sub New(Path As String, Optional AutoFlush As Boolean = True, Optional bufferSize As Integer = 1024)
            Dim file As New FileStream(Path, FileMode.Append)

            buffer = New StreamWriter(file, Encoding.UTF8, bufferSize) With {
                .AutoFlush = AutoFlush
            }
            buffer.WriteLine($"//{vbTab}[{Now.ToString}]{vbTab}{New String("=", 25)}  START WRITE LOGGING SECTION  {New String("=", 25)}" & vbCrLf)
            FilePath = FileIO.FileSystem.GetFileInfo(Path).FullName
        End Sub

        Public Sub LogException(Msg As String, <CallerMemberName> Optional Object$ = Nothing)
            Call WriteLine(Msg, [Object], Type:=MSG_TYPES.ERR)
        End Sub

        Public Sub LogException(ex As Exception)
            Call WriteLine(ex.ToString, "", Type:=MSG_TYPES.ERR)
        End Sub

        ''' <summary>
        ''' 向日志文件之中写入数据
        ''' </summary>
        ''' <param name="Msg"></param>
        ''' <param name="[Object]"></param>
        ''' <param name="Type"></param>
        Public Sub WriteLine(Msg As String, [Object] As String, Optional Type As MSG_TYPES = MSG_TYPES.INF)
            Dim LogEntry As New LogEntry With {
                .Msg = Msg,
                .Object = [Object],
                .Time = Now,
                .Type = Type
            }

            buffer.WriteLine(LogEntry.ToString)
            counts += 1
        End Sub

        ''' <summary>
        ''' 会自动拓展已经存在的日志数据
        ''' </summary>
        ''' <remarks></remarks>
        Private Function SaveLog() As Boolean
            Call buffer.WriteLine(vbCrLf & $"//{vbTab}{New String("=", 25)}  END OF LOG FILE  {New String("=", 25)}")
            Call buffer.WriteLine(vbCrLf)
            Call buffer.Flush()

            Return True
        End Function

        Public Overrides Function ToString() As String
            Return $"[{counts} records]'{FilePath.ToFileURL}'"
        End Function

        Public Function ReadLine() As String Implements I_ConsoleDeviceHandle.ReadLine
            Return ""
        End Function

        Public Sub WriteLine(Optional s As String = "") Implements I_ConsoleDeviceHandle.WriteLine
            Call WriteLine(s, Type:=MSG_TYPES.INF, [Object]:="")
        End Sub

        Public Sub WriteLine(s As String())
            Dim str As String = String.Join(vbCrLf, s)
            Call WriteLine(str, Type:=MSG_TYPES.INF, [Object]:="")
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="args">{[Object] As String, Optional Type As MsgType = MsgType.INF, Optional WriteToScreen As Boolean = True}</param>
        ''' <remarks></remarks>
        Public Sub WriteLine(s As String, ParamArray args() As String) Implements I_ConsoleDeviceHandle.WriteLine
            Dim [Object] As String = IIf(String.IsNullOrEmpty(args(0)), "", args(0))
            Call WriteLine(s, Type:=MSG_TYPES.INF, [Object]:=[Object])
        End Sub

        Public Overloads Function Read() As Integer Implements I_ConsoleDeviceHandle.Read
            Return -1
        End Function

        ''' <summary>
        ''' 日志文件在保存的时候默认是追加的方式
        ''' </summary>
        ''' <param name="FilePath"></param>
        ''' <param name="Encoding"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            FilePath = getPath(FilePath)
            Return SaveLog()
        End Function

        ''' <summary>
        ''' 给出用于调试的系统的信息摘要
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function SystemInfo() As String
            Dim sBuilder As New StringBuilder(1024)

            Call sBuilder.AppendLine($"{NameOf(OSVersionInfo.BuildVersion)}:={OSVersionInfo.BuildVersion}")
            Call sBuilder.AppendLine($"{NameOf(OSVersionInfo.Edition)}:={OSVersionInfo.Edition}")
            Call sBuilder.AppendLine($"{NameOf(OSVersionInfo.MajorVersion)}:={OSVersionInfo.MajorVersion}")
            Call sBuilder.AppendLine($"{NameOf(OSVersionInfo.MinorVersion)}:={OSVersionInfo.MinorVersion}")
            Call sBuilder.AppendLine($"{NameOf(OSVersionInfo.WindowsName)}:={OSVersionInfo.WindowsName}")
            Call sBuilder.AppendLine($"{NameOf(OSVersionInfo.OSBits)}:={OSVersionInfo.OSBits}")
            Call sBuilder.AppendLine($"{NameOf(OSVersionInfo.ProcessorBits)}:={OSVersionInfo.ProcessorBits}")
            Call sBuilder.AppendLine($"{NameOf(OSVersionInfo.ProgramBits)}:={OSVersionInfo.ProgramBits}")
            Call sBuilder.AppendLine($"{NameOf(OSVersionInfo.RevisionVersion)}:={OSVersionInfo.RevisionVersion}")
            Call sBuilder.AppendLine($"{NameOf(OSVersionInfo.ServicePack)}:={OSVersionInfo.ServicePack}")
            Call sBuilder.AppendLine($"{NameOf(OSVersionInfo.Version)}:={OSVersionInfo.Version.ToString}")

            Return sBuilder.ToString
        End Function

        Protected Overrides Function __getDefaultPath() As String
            Return FilePath
        End Function

        Protected Overrides Sub Dispose(disposing As Boolean)
            Call SaveLog()
            MyBase.Dispose(disposing)
        End Sub
    End Class
End Namespace
