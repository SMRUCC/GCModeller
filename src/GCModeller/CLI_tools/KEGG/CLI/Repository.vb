#Region "Microsoft.VisualBasic::c0c53c99b3a29fb1ffee294adca8de97, CLI_tools\KEGG\CLI\Repository.vb"

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

' Module CLI
' 
'     Function: BuildCompoundsRepository, BuildPathwayMapsRepository, BuildReactionsRepository, CompileGenomePathwayModule
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data

Partial Module CLI

    <ExportAPI("/Maps.Repository.Build")>
    <Usage("/Maps.Repository.Build /imports <directory> [/out <repository.XML>]")>
    <Description("Union the individual kegg reference pathway map file into one integral database file, usually used for fast loading.")>
    <Group(CLIGroups.Repository_cli)>
    <Argument("/imports", False, CLITypes.File,
              AcceptTypes:={GetType(Map)},
              Extensions:="*.xml",
              Description:="A directory folder path which contains multiple KEGG reference pathway map xml files.")>
    <Argument("/out", True, CLITypes.File, PipelineTypes.std_out,
              Extensions:="*.xml",
              Description:="An integral database xml file.")>
    <LastUpdated("2019-06-10 00:00:01")>
    Public Function BuildPathwayMapsRepository(args As CommandLine) As Integer
        Dim imports$ = args("/imports")
        Dim out$ = args("/out") Or $"{[imports].TrimDIR}.repository.Xml"

        Return MapRepository _
            .BuildRepository(directory:=[imports]) _
            .GetXml _
            .SaveTo(out, TextEncodings.UTF8WithoutBOM) _
            .CLICode
    End Function

    ''' <summary>
    ''' 将分散的小文件打包成一个大数据文件, 可以提升数据的加载效率
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Build.Reactions.Repository")>
    <Usage("/Build.Reactions.Repository /in <directory> [/out <repository.XML>]")>
    <Description("Package all of the single reaction model file into one data file for make improvements on the data loading.")>
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
    <Usage("/Pathway.Modules.Build /in <directory> [/batch /out <out.Xml>]")>
    <Group(CLIGroups.Repository_cli)>
    <Argument("/in", False, CLITypes.File, Description:="A directory that created by ``/Download.Pathway.Maps`` command.")>
    Public Function CompileGenomePathwayModule(args As CommandLine) As Integer
        Dim in$ = args <= "/in"

        If args.IsTrue("/batch") Then
            For Each dir As String In ls - l - lsDIR <= [in]
                Call Apps.KEGG_tools.CompileGenomePathwayModule([in]:=dir, batch:=False)
            Next

            Return 0
        Else
            Dim out = args("/out")
            Dim model As OrganismModel = OrganismModel.CreateModel(handle:=[in])
            Dim name$ = model _
                .organism _
                .FullName _
                .NormalizePathString

            Return model _
                .GetXml _
                .SaveTo(out Or $"{[in].ParentPath}/{name}.Xml", TextEncodings.UTF8WithoutBOM) _
                .CLICode
        End If
    End Function
End Module
