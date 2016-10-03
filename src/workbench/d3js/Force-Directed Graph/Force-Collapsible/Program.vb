#Region "Microsoft.VisualBasic::c61da02e866fdd4e9d07adfda710e6a8, ..\workbench\d3js\Force-Directed Graph\Force-Collapsible\Program.vb"

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

Imports Microsoft.VisualBasic.CommandLine

Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine, executeFile:=AddressOf Program.ExecuteFile)
    End Function

    Private Function ExecuteFile(file As String, args As CommandLine) As Integer
        Dim out As String = file.TrimFileExt & ".json"
        Dim json As String = NetworkGenerator.FromRegulations(file)
        Return json.SaveTo(out)
    End Function
End Module

