#Region "Microsoft.VisualBasic::b3aaac2a6ab40d0dabe52230c0ca865b, ..\workbench\IDE_PlugIns\ModelEditor\PlugInEntry.vb"

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
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports PlugIn

<[PackageNamespace]("GCModeller.Workbench.Plugins.ModelEditor", Publisher:="xie.guigang@gcmodeller.org")>
<PlugIn.PlugInEntry(Name:="Model Editor", Description:="")>
Public Module PlugInEntry

    <ExportAPI("Open.ModelEditor")>
    <PlugIn.PlugInCommand(Name:="Open Model Editor")>
    Public Function OpenEditor() As Integer
        Dim Editor = New ModelEditor()
        If Editor.OpenModel() = True Then
            Call Editor.ShowDialog()
        End If

        Return 0
    End Function
End Module
