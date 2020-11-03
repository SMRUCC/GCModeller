#Region "Microsoft.VisualBasic::bdf58293f38dac0437a25d3d1b79dc79, CLI_tools\KEGG\CLI\Repository.vb"

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
    '     Function: BuildCompoundsRepository, BuildPathwayMapsRepository, BuildReactionsRepository, CompileGenomePathwayModule, CompoundBriteTable
    '               CompoundNames, Glycan2CompoundId, ReactionToGeneNames
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data

Partial Module CLI

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Compound.Brite.Table")>
    <Usage("/Compound.Brite.Table [/save <table.csv>]")>
    Public Function CompoundBriteTable(args As CommandLine) As Integer
        Dim save$ = args("/save") Or $"./CompoundBrite.csv"
        Dim briteTable As New Dictionary(Of String, EntityObject)

        For Each [class] In CompoundBrite.GetAllCompoundResources
            For Each compound In [class].Value.Where(Function(kegg_cpd) Not kegg_cpd.kegg_id.StringEmpty)
                If Not briteTable.ContainsKey(compound.kegg_id) Then
                    briteTable(compound.kegg_id) = New EntityObject With {
                        .ID = compound.kegg_id,
                        .Properties = New Dictionary(Of String, String) From {
                            {"name", compound.entry.Value},
                            {"brite_class", [class].Name},
                            {"class", compound.class},
                            {"category", compound.category},
                            {"subcategory", compound.subcategory},
                            {"order", compound.order}
                        }
                    }
                End If
            Next
        Next

        Return briteTable.Values _
            .SaveTo(save) _
            .CLICode
    End Function

    <ExportAPI("/Maps.Repository.Build")>
    <Usage("/Maps.Repository.Build /imports <directory> [/out <repository.XML>]")>
    <Description("Union the individual kegg reference pathway map file into one integral database file, usually used for fast loading.")>
    <Group(CLIGroups.Repository_cli)>
    <ArgumentAttribute("/imports", False, CLITypes.File,
              AcceptTypes:={GetType(Map)},
              Extensions:="*.xml",
              Description:="A directory folder path which contains multiple KEGG reference pathway map xml files.")>
    <ArgumentAttribute("/out", True, CLITypes.File, PipelineTypes.std_out,
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
    <ArgumentAttribute("/in", False, CLITypes.File, Description:="A directory that created by ``/Download.Pathway.Maps`` command.")>
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

    <ExportAPI("/compound.names")>
    <Usage("/compound.names /repo <kegg_compounds.directory> [/out <names.json>]")>
    Public Function CompoundNames(args As CommandLine) As Integer
        Dim repo$ = args <= "/repo"
        Dim out$ = args("/out") Or $"{repo.TrimDIR}.commandNames.json"
        Dim names = GetCompoundNames(repo)

        Return names _
            .GetJson _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/Glycan.compoundId")>
    <Usage("/Glycan.compoundId /repo <kegg_compounds.directory> [/out <id_mapping.json>]")>
    Public Function Glycan2CompoundId(args As CommandLine) As Integer
        Dim repo$ = args <= "/repo"
        Dim out$ = args("/out") Or $"{repo.TrimDIR}.glycan.compoundIds.json"
        Dim names = CompoundRepository.ScanRepository(repo, False).Glycan2CompoundId

        Return names _
            .GetJson _
            .SaveTo(out) _
            .CLICode
    End Function

    <ExportAPI("/reaction.geneNames")>
    <Usage("/reaction.geneNames /repo <kegg_reaction.directory> [/out <names.json>]")>
    Public Function ReactionToGeneNames(args As CommandLine) As Integer
        Dim repo$ = args <= "/repo"
        Dim out$ = args("/out") Or $"{repo.TrimDIR}.geneNames.json"
        Dim repository = KEGG.Metabolism.FetchReactionRepository(repo)
        Dim reactions = repository.metabolicNetwork _
            .Where(Function(r) r.Orthology.size > 0) _
            .Select(Function(r)
                        Return r.Orthology _
                            .AsEnumerable _
                            .Select(Function(term)
                                        Return (KO:=term.name, reactionId:=r.ID)
                                    End Function)
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(r) r.reactionId) _
            .ToDictionary(Function(r) r.Key,
                          Function(list)
                              Return list _
                                 .Select(Function(map) map.KO) _
                                 .Distinct _
                                 .ToArray
                          End Function)

        Return reactions _
            .GetJson _
            .SaveTo(out) _
            .CLICode
    End Function
End Module
