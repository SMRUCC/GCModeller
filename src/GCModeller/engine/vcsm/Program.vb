#Region "Microsoft.VisualBasic::fc59cc90e92e20520727193c9fcc0238, engine\vcsm\Program.vb"

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
'     Properties: ExternalModuleRegistry, Settings
' 
'     Function: Main
' 
' /********************************************************************************/

#End Region

Imports GCModeller_vcell.Settings
Imports SMRUCC.genomics.GCModeller

''' <summary>
''' Virtual cell engine program main entry.
''' </summary>
Module Program

    Public ReadOnly Property Settings As Settings.File = Session.Initialize

    ''' <summary>
    ''' 计算框架的外部系统模块的注册表
    ''' </summary>
    ''' <remarks></remarks>
    Public ReadOnly Property ExternalModuleRegistry As ModellingEngine.PlugIns.ModuleRegistry =
        ModellingEngine.PlugIns.ModuleRegistry.Load(ModellingEngine.PlugIns.ModuleRegistry.XmlFile)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Function Main() As Integer
        Return GetType(CommandLines).RunCLI(App.CommandLine)
    End Function
End Module
