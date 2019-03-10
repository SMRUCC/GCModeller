﻿#Region "Microsoft.VisualBasic::f5da36a39fdb185759a03533ab6eaa85, CLI_tools\gcc\CLI\Project.vb"

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

' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.Compiler
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Partial Module CLI

    <ExportAPI("/summary")>
    <Usage("/summary <model.GCMarkup>")>
    Public Function Summary(args As CommandLine) As Integer
        Call Console.WriteLine(VirtualCell.Summary((args.Tokens(1)).LoadXml(Of VirtualCell)))
        Return 0
    End Function

    ''' <summary>
    ''' 这个函数只是将代谢网络数据给写入到模型之中？
    ''' 
    ''' 如果是包括多个复制子的，例如染色体和质粒等，是可以通过合并在一个gb文件之中传递给这个命令行的
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/compile.KEGG")>
    <Description("Create GCModeller virtual cell data model file.")>
    <Usage("/compile.KEGG /in <genome.gb> /KO <ko.assign.csv> /maps <kegg.pathways.repository> /compounds <kegg.compounds.repository> /reactions <kegg.reaction.repository> [/regulations <transcription.regulates.csv> /out <out.model.Xml/xlsx>]")>
    <Argument("/regulations", True, CLITypes.File, PipelineTypes.undefined, AcceptTypes:={GetType(RegulationFootprint)})>
    <Argument("/in", False, CLITypes.File, PipelineTypes.std_in)>
    Public Function CompileKEGG(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim KO$ = args <= "/KO"
        Dim kegg As New RepositoryArguments With {
            .KEGGCompounds = args <= "/compounds",
            .KEGGPathway = args <= "/maps",
            .KEGGReactions = args <= "/reactions"
        }
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.GCMarkup"
        Dim genome As Dictionary(Of String, GBFF.File) = GBFF.File _
            .LoadDatabase(filePath:=[in]) _
            .ToDictionary(Function(gb)
                              Return gb.Locus.AccessionID
                          End Function)
        Dim geneKO As Dictionary(Of String, String) = EntityObject _
            .LoadDataSet(KO) _
            .ToDictionary(Function(protein) protein.ID,
                          Function(protein) protein!KO)
        Dim regulations = (args <= "/regulations").LoadCsv(Of RegulationFootprint)
        Dim model As CellularModule = genome _
            .AssemblingMetabolicNetwork(geneKO, kegg) _
            .AssemblingRegulationNetwork(regulations)

        If out.IsGCMarkup Then
            Return model.ToMarkup(genome, kegg, regulations) _
                        .GetXml _
                        .SaveTo(out) _
                        .CLICode
        Else
            Return model.ToTabular _
                        .WriteXlsx(out) _
                        .CLICode
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function IsGCMarkup(out As String) As Boolean
        With out.ExtensionSuffix
            Return .TextEquals("GCMarkup") OrElse .TextEquals("Xml")
        End With
    End Function

    ''' <summary>
    ''' 这个命令将模型导出为网络模型
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 这个工具导出来的网络默认是至少具有一个连接点的
    ''' </remarks>
    <ExportAPI("/export.model.graph")>
    <Usage("/export.model.graph /model <GCMarkup.xml/table.xlsx> [/pathway <default=none> /disable.trim /degree <default=1> /out <out.dir>]")>
    <Description("Export cellular module network from virtual cell model file for cytoscape visualization.")>
    <Argument("/pathway", True, CLITypes.String, AcceptTypes:={GetType(String)},
              Description:="Apply a pathway module filter on the network model, only the gene contains in the given pathway list then will be output to user. 
              By default is export all. Pathway id should be a KO pathway id list, like ``ko04146,ko02010``, and id was seperated by comma symbol.")>
    Public Function ExportModelGraph(args As CommandLine) As Integer
        Dim in$ = args <= "/model"
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.network_graph/"
        Dim degree% = args("/degree") Or 1
        Dim model As VirtualCell

        With [in].ExtensionSuffix
            If .TextEquals("gcmarkup") OrElse .TextEquals("xml") Then
                model = [in].LoadXml(Of VirtualCell)
            Else
                Throw New NotImplementedException
            End If
        End With

        Return model.CreateGraph(pathways:=args("/pathway").Split(","c)) _
            .AnalysisDegrees _
            .RemovesByDegree(degree:=degree) _
            .Trim(doNothing:=args("/disable.trim")) _
            .Save(out, Encodings.ASCII) _
            .CLICode
    End Function

    <ExportAPI("/export.model.pathway_graph")>
    <Usage("/export.model.pathway_graph /model <GCMarkup.xml/table.xlsx> [/disable.trim /degree <default=1> /out <out.dir>]")>
    Public Function ExportPathwaysNetwork(args As CommandLine) As Integer
        Dim model$ = args <= "/model"
        Dim disableTrim As Boolean = args("/disable.trim")
        Dim degree% = args("/degree") Or 1
        Dim out$ = args("/out") Or $"{model.TrimSuffix}.pathways/"

        For Each [module] As FunctionalCategory In model.LoadXml(Of VirtualCell).MetabolismStructure.maps
            Dim mapName = [module].category.NormalizePathString

            For Each pathway As Pathway In [module].pathways
                With $"{out.TrimDIR}/{mapName}/{pathway.ID}__{pathway.name.NormalizePathString}/"
                    Call Apps.GCModellerCompiler.ExportModelGraph(
                        model, pathway.ID,
                        degree,
                        .ByRef,
                        disableTrim
                    )

                    If NetworkTables.IsEmptyTables(.ByRef) Then
                        ' 删除这个空的网络导出结果
                        Call .Delete
                        Call $"Pathway: {pathway} no network was found...".Warning
                    Else
                        Call pathway.ToString.__INFO_ECHO
                    End If
                End With
            Next
        Next

        Return 0
    End Function
End Module