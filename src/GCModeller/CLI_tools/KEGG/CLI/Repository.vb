#Region "Microsoft.VisualBasic::8cf559e4cb7c359426dda647ef59e9ea, ..\GCModeller\CLI_tools\KEGG\CLI\Repository.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data

Partial Module CLI

    <ExportAPI("/Maps.Repository.Build")>
    <Usage("/Maps.Repository.Build /imports <directory> [/out <repository.XML>]")>
    <Group(CLIGroups.Repository_cli)>
    Public Function BuildPathwayMapsRepository(args As CommandLine) As Integer
        Dim imports$ = args("/imports")
        Dim out$ = args("/out") Or $"{[imports].TrimDIR}.repository.Xml"

        Return MapRepository _
            .BuildRepository(directory:=[imports]) _
            .GetXml _
            .SaveTo(out, TextEncodings.UTF8WithoutBOM) _
            .CLICode
    End Function

    <ExportAPI("/Build.Reactions.Repository")>
    <Usage("/Build.Reactions.Repository /in <directory> [/out <repository.XML>]")>
    <Group(CLIGroups.Repository_cli)>
    Public Function BuildReactionsRepository(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimDIR}.repository.Xml"

        Return ReactionRepository _
            .ScanModel(directory:=[in]) _
            .GetXml _
            .SaveTo(out, TextEncodings.UTF8WithoutBOM) _
            .CLICode
    End Function

    <ExportAPI("/Build.Compounds.Repository")>
    <Usage("/Build.Compounds.Repository /in <directory> [/glycan.ignore /out <repository.XML>]")>
    <Group(CLIGroups.Repository_cli)>
    Public Function BuildCompoundsRepository(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out$ = args("/out") Or $"{[in].TrimDIR}.repository.Xml"
        Dim ignoreGlycan As Boolean = args.IsTrue("/glycan.ignore")

        Return CompoundRepository _
            .ScanModels(directory:=[in], ignoreGlycan:=ignoreGlycan) _
            .GetXml _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/Pathway.Modules.Build")>
    <Usage("/Pathway.Modules.Build /in <directory> [/out <out.Xml>]")>
    <Group(CLIGroups.Repository_cli)>
    <Argument("/in", False, CLITypes.File, Description:="A directory that created by ``/Download.Pathway.Maps`` command.")>
    Public Function CompileGenomePathwayModule(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out = args("/out")
        Dim model As OrganismModel = OrganismModel.CreateModel(directory:=[in])
        Dim name$ = model _
            .organism _
            .FullName _
            .NormalizePathString

        Return model _
            .GetXml _
            .SaveTo(out Or $"{[in].ParentPath}/{name}.Xml", TextEncodings.UTF8WithoutBOM) _
            .CLICode
    End Function
End Module
