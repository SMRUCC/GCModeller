#Region "Microsoft.VisualBasic::701285d38e46fc7530fa738fcfc4df82, ..\sciBASIC.ComputingServices\LINQ\LINQ\APP\CLI.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports sciBASIC.ComputingServices.Linq.Framework.Provider.ImportsAPI
Imports sciBASIC.ComputingServices.Linq.Framework.Provider

''' <summary>
''' 框架程序自带的注册模块以及一些配置的管理终端
''' </summary>
Module CLI

    ''' <summary>
    ''' 扫描应用程序文件夹之中可能的插件信息
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/install", Usage:="/install")>
    Public Function InstallPlugins(args As CommandLine.CommandLine) As Integer
        Using registry As TypeRegistry = TypeRegistry.LoadDefault
            Call registry.InstallCurrent()
        End Using
        Using api As APIProvider = APIProvider.LoadDefault
            Call api.Install()
        End Using

        Return 0
    End Function
End Module

