#Region "Microsoft.VisualBasic::ae8cad440786c5e512cce346af634d7d, ..\httpd\HTTPServer\HTTP_SERVER\HTTP_SERVER\Program.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic.CommandLine.Reflection

Module Program

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function
End Module

Module CLI

    ''' <summary>
    ''' Run the http server
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/start", Usage:="/start [/port <default:=80> /root <./wwwroot>]",
               Info:="Start the simple http server.",
               Example:="/start /root ~/.server/wwwroot/ /port 412")>
    <ParameterInfo("/port", True,
                   Description:="The data port for this http server to bind.")>
    <ParameterInfo("/root", True,
                   Description:="The wwwroot directory for your http html files, default location is the wwwroot directory in your App HOME directory.")>
    Public Function Start(args As CommandLine.CommandLine) As Integer
        Dim port As Integer = args.GetValue("/port", 80)
        Dim root As String = args.GetValue("/root", App.HOME & "/wwwroot")
        Return New HttpInternal.HttpFileSystem(port, root, True).Run
    End Function
End Module

