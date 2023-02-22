#Region "Microsoft.VisualBasic::310e9901e55bd5ee81673623af435fef, GCModeller\annotations\GSEA\GSEA.KnowledgeBase.Extensions\Metabolism\EnrichmentNetwork.vb"

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


' Code Statistics:

'   Total Lines: 6
'    Code Lines: 2
' Comment Lines: 3
'   Blank Lines: 1
'     File Size: 122 B


' Class EnrichmentNetwork
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

''' <summary>
''' Model of network based enrichment analysis(KEGG pathway targetted)
''' </summary>
''' <remarks>
''' factors for the enrichment data analysis includes the network 
''' topology impact factors, example likes: degree centroid/relative 
''' betweeness, etc
''' </remarks>
Public Module EnrichmentNetwork

    ''' <summary>
    ''' 直接基于已有的物种KEGG数据信息进行富集计算数据集的创建
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/kegg_models")>
    <Usage("/kegg_models /maps <repository_directory> /reactions <br08201.csv> [/ko_ref /out <gsea.rda>]")>
    Public Function KEGGModels(args As CommandLine) As Integer
        Dim in$ = args <= "/maps"
        Dim isKo_ref As Boolean = args("/ko_ref")
        Dim out$ = args("/out") Or $"{If(isKo_ref, [in].ParentPath & "/ko", [in].TrimDIR)}.rda"
        Dim models As Pathway()
        Dim reactions As Dictionary(Of String, ReactionTable()) = ReactionTable.Load(args <= "/reactions").CreateIndex
        Dim orgName As String = $"{[in]}/index.txt".ReadFirstLine

        If isKo_ref Then
            models = (ls - l - r - "*.xml" <= [in]).Select(Function(file) file.LoadXml(Of PathwayMap).ToPathway).ToArray
        Else
            models = (ls - l - r - "*.xml" <= [in]).Select(Function(file) file.LoadXml(Of Pathway)).ToArray
        End If

        If Not orgName.StringEmpty Then
            For Each pathway As Pathway In models
                pathway.name = pathway.name.Replace(orgName, "").Trim(" "c, "-"c)
            Next
        End If

        Return models.buildModels(If(isKo_ref, "map", ""), reactions).save(out).CLICode
    End Function

    <Extension>
    Private Function buildModels(models As Pathway(), keggId As String, reactions As Dictionary(Of String, ReactionTable())) As metpa
        Dim pathIds As New pathIds With {
            .ids = models.Select(Function(m) If(keggId.StringEmpty, m.EntryId, keggId & m.briteID)).ToArray,
            .pathwayNames = models.Select(Function(m) m.name.Replace(" - Reference pathway", "")).ToArray
        }
        Dim uniqueCompounds As Integer = models _
            .Select(Function(a) a.compound) _
            .IteratesALL _
            .GroupBy(Function(a) a.name) _
            .Count

        Dim msetList As New msetList With {
            .list = models _
                 .Select(Function(a)
                             Dim nameId As String = If(keggId.StringEmpty, a.EntryId, keggId & a.briteID)
                             Dim mset As New mset With {
                                .kegg_id = a.compound.Select(Function(c) c.name).ToArray,
                                .metaboliteNames = a.compound.Select(Function(c) c.text).ToArray
                             }

                             Return New NamedValue(Of mset)(nameId, mset)
                         End Function) _
                 .ToArray
        }

        VBDebugger.Mute = True

        Dim graphs As NamedValue(Of NetworkGraph)() = models _
            .Populate(Not VBDebugger.debugMode) _
            .Select(Function(model)
                        Return model.createPathwayNetworkGraph(keggId, reactions)
                    End Function) _
            .ToArray

        With pathIds.ids.Indexing
            graphs = graphs.OrderBy(Function(g) .IndexOf(g.Name)).ToArray
        End With

        Dim rbc As New rbcList With {.list = graphs.Select(AddressOf calcRbc).ToArray}
        Dim dgr As New dgrList With {.pathways = graphs.Select(AddressOf calcDgr).ToArray}

        Return New metpa() _
            .write(pathIds) _
            .write(uniqueCompounds) _
            .write(msetList) _
            .write(rbc) _
            .write(dgr)
    End Function

    <ExportAPI("/gsea_set")>
    <Usage("/gsea_set /ptf <taxonomy.ptf> /maps <br08901_pathwayMaps> /reactions <br08201.csv> /compounds <compoundNames.json> [/class <reactionclasstable.csv> /out <gsea.rda>]")>
    <Description("reconstruct of the kegg pathway and then create gsea background dataset for biodeep package.")>
    Public Function CreateGSEASet(args As CommandLine) As Integer
        Dim ptf As String = args <= "/ptf"
        Dim proteins = PtfFile.Load(ptf)

        Call proteins.ToString.__DEBUG_ECHO

        Dim keggId As String = proteins.AsEnumerable.Where(Function(a) a.attributes.ContainsKey("kegg")).First!kegg.Split(":"c).First
        Dim out$ = args("/out") Or $"{ptf.ParentPath}/{keggId}.rda"
        Dim classTable As Dictionary(Of String, ReactionClassTable()) = args("/class") _
            .LoadCsv(Of ReactionClassTable) _
            .DoCall(AddressOf ReactionClassTable.ReactionIndex)
        Dim compounds As Dictionary(Of String, String) = args("/compounds").LoadJson(Of Dictionary(Of String, String))
        Dim reactions = ReactionTable.Load(args <= "/reactions").CreateIndex
        Dim models As Pathway() = MapRepository _
            .GetMapsAuto(args <= "/maps") _
            .KEGGReconstruction(proteins.AsEnumerable, 0) _
            .Select(Function(pathway)
                        Return pathway.AssignCompounds(
                            reactions:=reactions,
                            names:=compounds,
                            classes:=classTable
                        )
                    End Function) _
            .Where(Function(a) Not a.compound.IsNullOrEmpty) _
            .OrderByDescending(Function(a)
                                   Return a.compound.Length
                               End Function) _
            .ToArray

        Call models _
            .EvaluateCompoundUniqueRank _
            .Transpose _
            .ToArray _
            .SaveTo($"{out.TrimSuffix}.compound_unique.csv", metaBlank:="0")

        models = models.UniquePathwayCompounds.ToArray

        Return models.buildModels(keggId, reactions).save(out).CLICode
    End Function

    <Extension>
    Private Function createPathwayNetworkGraph(model As Pathway, keggId$, reactions As Dictionary(Of String, ReactionTable())) As NamedValue(Of NetworkGraph)
        Dim allCompoundNames = model.compound _
            .Select(Function(a) New NamedValue(Of String)(a.name, a.text)) _
            .ToArray
        Dim g As NetworkGraph = model _
            .GetReactions(reactions) _
            .BuildModel(
                compounds:=allCompoundNames,
                extended:=False,
                enzymes:=Nothing,
                enzymaticRelated:=False,
                filterByEnzymes:=False,
                ignoresCommonList:=False,
                enzymeBridged:=False,
                strictReactionNetwork:=True
            )
        Dim nameId As String = keggId & model.briteID

        Call g.ComputeBetweennessCentrality(base:=1)
        Call g.ComputeNodeDegrees(base:=1)

        Return New NamedValue(Of NetworkGraph) With {
            .Name = nameId,
            .Value = g
        }
    End Function

    Private Function calcDgr(a As NamedValue(Of NetworkGraph)) As NamedValue(Of dgr)
        Dim cnodes As Node() = a.Value.vertex _
            .Where(Function(c)
                       Return c.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "KEGG Compound"
                   End Function) _
            .ToArray
        Dim cid = cnodes.Select(Function(c) c.label).ToArray
        Dim rbcVal As Double() = cnodes _
            .Select(Function(c)
                        Return c.data(NamesOf.REFLECTION_ID_MAPPING_RELATIVE_DEGREE_CENTRALITY)
                    End Function) _
            .Select(AddressOf Val) _
            .ToArray
        Dim dgr As New dgr With {
            .dgr = rbcVal,
            .kegg_id = cid
        }

        Return New NamedValue(Of dgr)(a.Name, dgr)
    End Function

    Private Function calcRbc(a As NamedValue(Of NetworkGraph)) As NamedValue(Of rbc)
        Dim cnodes As Node() = a.Value.vertex _
            .Where(Function(c)
                       Return c.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "KEGG Compound"
                   End Function) _
            .ToArray
        Dim cid = cnodes.Select(Function(c) c.label).ToArray
        Dim rbcVal As Double() = cnodes _
            .Select(Function(c)
                        Return c.data(NamesOf.REFLECTION_ID_MAPPING_RELATIVE_BETWEENESS_CENTRALITY)
                    End Function) _
            .Select(AddressOf Val) _
            .ToArray
        Dim rbc As New rbc With {
            .data = rbcVal,
            .kegg_id = cid
        }

        Return New NamedValue(Of rbc)(a.Name, rbc)
    End Function

End Module
