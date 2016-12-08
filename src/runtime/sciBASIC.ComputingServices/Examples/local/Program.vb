#Region "Microsoft.VisualBasic::74261a99b8377e239a907217443cd6bf, ..\sciBASIC.ComputingServices\Examples\local\Program.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.IO
Imports Microsoft.VisualBasic.Net
Imports sciBASIC.ComputingServices
Imports sciBASIC.ComputingServices.FileSystem.IO
Imports sciBASIC.ComputingServices.TaskHost

Module Program

    Sub Main()

        Dim nnnn As Integer() = {3, 424, 2324, 88, 2, 54, 46, 7, 57, 5, -1111, 86, 7, 87, 97, 55}
        Dim value As New SharedMemory.HashValue(NameOf(nnnn), nnnn)
        Call value.__DEBUG_ECHO

        Dim a As New SharedMemory.MemoryServices(New IPEndPoint("127.0.0.1", 1234), 3321)
        Call a.DriverRun
        Call Threading.Thread.Sleep(1000)
        Dim b As New SharedMemory.MemoryServices(New IPEndPoint("127.0.0.1", 3321), 1234)
        Call b.DriverRun
        Call Threading.Thread.Sleep(1000)


        Call a.SetValue(NameOf(nnnn), nnnn)
        Call Threading.Thread.Sleep(1000)

        Dim bbb As Integer() = a.GetValue(Of Integer())(NameOf(nnnn))

        Pause()

        Dim remoteMachine As New TaskHost(New IPEndPoint("127.0.0.1", 1234))
        Dim func As Func(Of Stream, String, String()) = AddressOf AnalysisExample.API.LongTest1
        Dim path As String = "E:\Microsoft.VisualBasic.Parallel\trunk\Examples\local\local.vbproj"
        Dim localfile As New RemoteFileStream(path, FileMode.Open, remoteMachine.FileSystem)
        Dim array As String() = remoteMachine.Invoke(func, {localfile, "this is the message from local machine!"})
        ' remote linq

        Call array.Length.__DEBUG_ECHO

        localfile = New RemoteFileStream(path, FileMode.Open, remoteMachine.FileSystem)
        Dim source = remoteMachine.AsLinq(Of String)(func, {localfile, "this is the remote linq example!"})
        Dim array2 = (From s As String In source Where InStr(s, "Include=") > 0 Select s)

        For Each line As String In array2
            Call Console.WriteLine(line)
        Next


        Call Pause()
    End Sub

    Sub test()
        Dim info = GetType(Program).AddressOf(NameOf(Main))
        Dim resulkt = info.Invoke(Nothing)
    End Sub
End Module

