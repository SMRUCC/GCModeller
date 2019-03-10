﻿#Region "Microsoft.VisualBasic::89184f1e43920f7c189ef22d92376a8e, CLI_tools\KEGG\CLI\Pathways.vb"

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
'     Function: Compile, CompoundMapRender, PathwayGeneList
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File

Partial Module CLI

    <ExportAPI("/Compile.Model", Info:="KEGG pathway model compiler",
               Usage:="/Compile.Model /pathway <pathwayDIR> /mods <modulesDIR> /sp <sp_code> [/out <out.Xml>]")>
    Public Function Compile(args As CommandLine) As Integer
        Dim pwyDIR As String = args("/pathway")
        Dim modDIR As String = args("/mods")
        Dim sp As String = args("/sp")
        Dim reactions As String = GCModeller.FileSystem.KEGG.GetReactions
        Dim out As String = args.GetValue("/out", pwyDIR & "." & sp & "_KEGG.xml")
        Dim model As XmlModel = CompilerAPI.Compile(
            KEGGPathways:=pwyDIR,
            KEGGModules:=modDIR,
            KEGGReactions:=reactions,
            speciesCode:=sp)
        Return model.SaveAsXml(out).CLICode
    End Function

    <ExportAPI("/Pathway.geneIDs",
               Usage:="/Pathway.geneIDs /in <pathway.XML> [/out <out.list.txt>]")>
    Public Function PathwayGeneList(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".geneIDs.txt")
        Dim pathway As Pathway = [in].LoadXml(Of Pathway)
        Return pathway.GetPathwayGenes.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Enrichment.Map.Render")>
    <Usage("/Enrichment.Map.Render /url <url> [/repo <pathwayMap.repository> /out <out.png>]")>
    <Description("Rendering kegg pathway map for enrichment analysis result in local.")>
    <Argument("/repo", True, CLITypes.File, Description:="A directory path that contains the KEGG reference pathway map XML model. If this argument value is not presented in the commandline, then the default installed GCModeller KEGG compound repository will be used.")>
    Public Function EnrichmentMapRender(args As CommandLine) As Integer
        Dim url$ = args <= "/url"
        Dim repo$ = args("/repo") Or (GCModeller.FileSystem.FileSystem.RepositoryRoot & "/KEGG/pathwayMap/")
        Dim query = URLEncoder.URLParser(url)
        Dim out$ = args("/out") Or $"./{query.Name}.png"
        Dim render As LocalRender = LocalRender.FromRepository(repo)

        Return render.Rendering(query.Name, query.Value) _
                     .SaveAs(out, ImageFormats.Png) _
                     .CLICode
    End Function

    <ExportAPI("/Compound.Map.Render")>
    <Usage("/Compound.Map.Render /list <csv/txt> [/repo <pathwayMap.repository> /scale <default=1> /color <default=red> /out <out.DIR>]")>
    <Description("Render draw of the KEGG pathway map by using a given KEGG compound id list.")>
    <Argument("/list", False, CLITypes.File, PipelineTypes.std_in,
              AcceptTypes:={GetType(String())},
              Extensions:="*.txt, *.csv",
              Description:="A KEGG compound id list that provides the KEGG pathway map rendering source.")>
    <Argument("/repo", True, CLITypes.File,
              Description:="A directory path that contains the KEGG reference pathway map XML model. If this argument value is not presented in the commandline, then the default installed GCModeller KEGG compound repository will be used.")>
    <Argument("/scale", True, CLITypes.Double,
              Description:="The circle radius size of the KEGG compound that rendering on the output pathway map image. By default is no scale.")>
    <Argument("/color", True, CLITypes.String,
              Description:="The node color that the KEGG compound rendering on the pathway map.")>
    <Argument("/out", True, CLITypes.File,
              Description:="A directory output path that will be using for contains the rendered pathway map image and the summary table file.")>
    Public Function CompoundMapRender(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim repo$ = (args <= "/repo") Or (GCModeller.FileSystem.FileSystem.RepositoryRoot & "/KEGG/pathwayMap/").AsDefault
        Dim scale# = args.GetValue("/scale", 1.0#)
        Dim color$ = (args <= "/color") Or "red".AsDefault
        Dim out$ = (args <= "/out") Or [in].TrimSuffix.AsDefault
        Dim list$()

        If [in].ExtensionSuffix.TextEquals("txt") Then
            list = [in].ReadAllLines
        Else
            list = csv.Load([in]).Columns(0).ToArray
        End If

        Dim summary = PathwayMapRender.RenderMaps(repo, list, out)
        Dim table As New csv

        table += {"Pathway", "Name", "ID.list"}

        For Each pathway In summary
            table += {pathway.Name, pathway.Value, pathway.Description}
        Next

        Return table _
            .Save(out & "/summary.csv", encoding:=Encodings.UTF8) _
            .CLICode
    End Function
End Module
