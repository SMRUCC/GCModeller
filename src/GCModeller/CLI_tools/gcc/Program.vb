#Region "Microsoft.VisualBasic::d0310a4bfc09dae6a6613f85617128ae, ..\GCModeller\CLI_tools\gcc\Program.vb"

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

Imports Microsoft.VisualBasic.Terminal.stdio

''' <summary>
''' GCModeller模型文件编译工具，主要的工作为将一个MetaCyc数据库编译为一个GCML模型文件，以及根据建模项目文件编译模型文件
''' </summary>
''' <remarks>
''' </remarks>
Module Program

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function

    Sub New()
        Call Settings.Session.Initialize()
        Call FileIO.FileSystem.CreateDirectory(Settings.LogDIR)
    End Sub
End Module

