#Region "Microsoft.VisualBasic::cbff26167b64a330c921c57b6e4eb08e, ..\CLI_tools\c2\CommandLines\ModelExport.vb"

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

Partial Module CommandLines

    <CommandLine.Reflection.ExportAPI("-modelexport",
        info:="export a sbml file format model from a specific metacyc dabase.",
        usage:="-modelexport -i <metacyc_dir> -o <output_model_file>",
        Example:="")>
    Public Function ModelExport(CommandLine As Microsoft.VisualBasic.CommandLine.CommandLine) As Integer
        Dim MetaCyc As String = CommandLine("-i")
        Dim SbmlFile As String = CommandLine("-o")

        Using Export As c2.NetworkVisualization.SBML =
            New NetworkVisualization.SBML(MetaCyc:=LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder.CreateInstance(MetaCyc))
            Call Export.Export.GetXml.SaveTo(Path:=SbmlFile)
        End Using

        Return 0
    End Function
End Module
