#Region "Microsoft.VisualBasic::f9b5df0ded0c618c54bfd67ac9af789b, pipeline\Program.vb"

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

    ' Module Program
    ' 
    '     Function: [Stop], Dispose, Main, Register, Start
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection

<CLI> Module Program

    Const defaultPort8833 As Integer = 8833

    Const ServicesController$ = "Pipeline Services Controller"
    Const ResourceController$ = "Pipeline Resource Controller"

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    ''' <summary>
    ''' Start the IPC pipeline host services
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/start")>
    <Description("Start the IPC pipeline host services")>
    <Usage("/start [/port <default=8833>]")>
    <Group(Program.ServicesController)>
    Public Function Start(args As CommandLine) As Integer
        Return New IPCHost(port:=args("/port") Or defaultPort8833).Run
    End Function

    <ExportAPI("/stop")>
    <Description("Send a stop signal to the IPC host to shutdown the running services instance.")>
    <Usage("/stop [/port <default=8833>]")>
    <Group(Program.ServicesController)>
    Public Function [Stop](args As CommandLine) As Integer
        Dim port% = args("/port") Or defaultPort8833
        Throw New NotImplementedException
    End Function

    <ExportAPI("/dispose")>
    <Usage("/dispose /resource <resource_name>")>
    <Description("Delete an exists memory mapping file resource.")>
    <Group(Program.ResourceController)>
    Public Function Dispose(args As CommandLine) As Integer
        Throw New NotImplementedException
    End Function

    <ExportAPI("/register")>
    <Usage("/register /resource <resource_name> /size <size_in_bytes> /type <meta_base64>")>
    <Description("Allocate a new memory mapping file resource for save the temp data for cli pipeline scripting")>
    <Group(Program.ResourceController)>
    Public Function Register(args As CommandLine) As Integer
        Dim name$ = args <= "/resource"
        Dim size& = args("/size")
        Dim type$ = args <= "/type"



        Return 0
    End Function
End Module
