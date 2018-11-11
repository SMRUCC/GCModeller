#Region "Microsoft.VisualBasic::f5da36a39fdb185759a03533ab6eaa85, CLI_tools\gcc\CLI\Project.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.Compiler
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Partial Module CLI

    ''' <summary>
    ''' 这个函数只是将代谢网络数据给写入到模型之中？
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/compile.KEGG")>
    <Usage("/compile.KEGG /in <genome.gb> /KO <ko.assign.csv> /maps <kegg.pathways.repository> /compounds <kegg.compounds.repository> /reactions <kegg.reaction.repository> [/regulations <transcription.regulates.csv> /out <out.model.Xml/xlsx>]")>
    <Argument("/regulations", True, CLITypes.File, PipelineTypes.undefined, AcceptTypes:={GetType(RegulationFootprint)})>
    Public Function CompileKEGG(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim KO$ = args <= "/KO"
        Dim kegg As New RepositoryArguments With {
            .KEGGCompounds = args <= "/compounds",
            .KEGGPathway = args <= "/maps",
            .KEGGReactions = args <= "/reactions"
        }
        Dim out$ = args("/out") Or $"{[in].TrimSuffix}.GCMarkup"
        Dim genome As GBFF.File = GBFF.File.Load(path:=[in])
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
    <Usage("/export.model.graph /model <GCMarkup.xml/table.xlsx> [/degree <default=1> /out <out.dir>]")>
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

        Return model.CreateGraph.Save(out).CLICode
    End Function
End Module