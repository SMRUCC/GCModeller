#Region "Microsoft.VisualBasic::af8983d1ae71095212e1675c762b27e9, engine\vcsm\CLI\ModuleRegistry.vb"

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

    ' Module CommandLines
    ' 
    '     Function: RegistryModule, UnRegistry
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports SMRUCC.genomics.GCModeller

Partial Module CommandLines

    <ExportAPI("registry", Info:="",
    Usage:="registry <assembly_file>", Example:="resistry /home/xieguigang/gcmodeller/models/plas.dll")>
    Public Function RegistryModule(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Using Registry As ModellingEngine.PlugIns.ModuleRegistry = ModellingEngine.PlugIns.ModuleRegistry.Load(ModellingEngine.PlugIns.ModuleRegistry.XmlFile)
            Dim Entry = Registry.Registry(CommandLine.Parameters.First)
            Call printf("Registry target external module successfully!\nAssembly: %s\n\n%s", CommandLine.Parameters.First, Entry.GetDescription)
        End Using
        Return 0
    End Function

    <ExportAPI("unregistry", Info:="", Usage:="unregistry <assembly_file>", Example:="unregistry ~/gcmodeller/models/plas.dll")>
    Public Function UnRegistry(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Using Registry As ModellingEngine.PlugIns.ModuleRegistry =
            ModellingEngine.PlugIns.ModuleRegistry.Load(ModellingEngine.PlugIns.ModuleRegistry.XmlFile)
            Call Registry.UnRegistry(CommandLine.Parameters.First)
        End Using
        Return 0
    End Function
End Module
