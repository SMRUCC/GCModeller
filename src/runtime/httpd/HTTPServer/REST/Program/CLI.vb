#Region "Microsoft.VisualBasic::ffc5785f0d51027613d1cfff3dd738b0, ..\httpd\HTTPServer\REST\Program\CLI.vb"

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
Imports SMRUCC.HTTPInternal.Platform

Module CLI

    <ExportAPI("/start", Usage:="/start [/port 80 /root <wwwroot_DIR>]")>
    Public Function Start(args As CommandLine.CommandLine) As Integer
        Dim cfg As Configs = Configs.LoadDefault
        Dim port As Integer = args.GetValue("/port", cfg.Portal)
        Dim HOME As String = args.GetValue("/root", cfg.WWWroot)
        cfg.Portal = port
        cfg.WWWroot = HOME
        cfg.Save()
        Return New PlatformEngine(HOME, port, True).Run
    End Function

    <ExportAPI("/run", Usage:="/run /dll <app.dll> [/port <80> /root <wwwroot_DIR>]")>
    Public Function RunApp(args As CommandLine.CommandLine) As Integer
        Dim cfg As Configs = Configs.LoadDefault
        Dim port As Integer = args.GetValue("/port", cfg.Portal)
        Dim HOME As String = args.GetValue("/root", cfg.WWWroot)
        Dim dll As String = args.GetValue("/dll", cfg.App)
        cfg.App = dll
        cfg.Portal = port
        cfg.WWWroot = HOME
        cfg.Save()
        Return New PlatformEngine(HOME, port, True, dll).Run
    End Function
End Module

