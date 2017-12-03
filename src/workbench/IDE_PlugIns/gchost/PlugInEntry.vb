#Region "Microsoft.VisualBasic::40edc612139eb78c06f5ff6e1014e139, ..\workbench\IDE_PlugIns\gchost\PlugInEntry.vb"

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

Imports Microsoft.VisualBasic
Imports PlugIn

<PlugIn.PlugInEntry(name:="测试", description:="")>
Module PlugInEntry

    Public IDEProxy As LANS.SystemsBiology.GCModeller.Workbench.MultipleTabpageProxy
    Public IDEInstance As LANS.SystemsBiology.GCModeller.Workbench.IDEInstance

    <PlugIn.PlugInCommand(name:="运行")>
    Public Function Run() As Integer
        Throw New NotImplementedException
    End Function

    <PlugIn.PlugInCommand(name:="打开控制面板")>
    Public Function OpenPanel() As Integer
        Call IDEProxy.AddTabPage(Of Global.gchost.GcHostControlPanel)("GCModeller Host")
        Return 0
    End Function

    <PlugIn.EntryFlag(entrytype:=EntryFlag.EntryTypes.Initialize)>
    Public Function Initialize(Target As Global.LANS.SystemsBiology.GCModeller.Workbench.FormMain) As Integer
        PlugInEntry.IDEProxy = Target.TabpageProxy
        PlugInEntry.IDEInstance = Target.IDEInstance
        Return 0
    End Function
End Module
