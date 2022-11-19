#Region "Microsoft.VisualBasic::393cb946f8d7397d0d2f35660349d33f, GCModeller\engine\Compiler\KEGG\PathwayCompiler.vb"

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

    '   Total Lines: 99
    '    Code Lines: 85
    ' Comment Lines: 4
    '   Blank Lines: 10
    '     File Size: 4.35 KB


    ' Module PathwayCompiler
    ' 
    '     Function: CompileOrganism, CreateMaps, ToMarkup
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.Compiler.MarkupCompiler
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Molecule
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Process
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' Create virtual cell xml file model from KEGG pathway data
''' </summary>
Public Module PathwayCompiler

    <Extension>
    Public Function CompileOrganism(replicons As Dictionary(Of String, GBFF.File), keggModel As OrganismModel, locationAsLocustag As Boolean) As VirtualCell
        Dim taxonomy As Taxonomy = replicons.getTaxonomy
        Dim Kofunction As Dictionary(Of String, String) = keggModel.KoFunction
        Dim genotype As New Genotype With {
            .centralDogmas = replicons _
                .GetCentralDogmas(Kofunction, locationAsLocustag) _
                .ToArray
        }
        Dim cell As New CellularModule With {
            .Taxonomy = taxonomy,
            .Genotype = genotype
        }

        Return cell.ToMarkup(replicons, keggModel, locationAsLocustag)
    End Function

    <Extension>
    Private Function ToMarkup(cell As CellularModule,
                              genomes As Dictionary(Of String, GBFF.File),
                              kegg As OrganismModel,
                              locationAsLocustag As Boolean) As VirtualCell

        Dim KOgenes As Dictionary(Of String, CentralDogma) = cell _
            .Genotype _
            .centralDogmas _
            .Where(Function(process)
                       Return Not process.IsRNAGene AndAlso Not process.orthology.StringEmpty
                   End Function) _
            .ToDictionary(Function(term) term.geneID)
        Dim maps As FunctionalCategory() = kegg.CreateMaps.ToArray
        Dim compiler As New v2KEGGCompiler(cell, genomes, Nothing, {}, locationAsLocustag)
        ' Dim genomeCompiler As New CompileGenomeWorkflow(compiler)
        Dim genomeCompiler As CompileGenomeWorkflow

        Return New VirtualCell With {
            .taxonomy = cell.Taxonomy,
            .genome = New Genome With {
                .replicons = genomeCompiler _
                    .populateReplicons(genomes) _
                    .ToArray
            },
            .metabolismStructure = New MetabolismStructure With {
                .maps = maps
            }
        }
    End Function

    <Extension>
    Private Iterator Function CreateMaps(kegg As OrganismModel) As IEnumerable(Of FunctionalCategory)
        Dim pathwayCategory = BriteHEntry.Pathway.LoadFromResource
        Dim pathwayIndex = kegg.genome.ToDictionary(Function(map) map.briteID)

        For Each category As IGrouping(Of String, BriteHEntry.Pathway) In pathwayCategory.GroupBy(Function(pathway) pathway.class)
            Dim pathways As Pathway() = category _
                .Where(Function(entry) pathwayIndex.ContainsKey(entry.EntryId)) _
                .Select(Function(entry)
                            Dim map = pathwayIndex(entry.EntryId)

                            Return New Pathway With {
                                .ID = map.EntryId,
                                .name = map.name,
                                .enzymes = map.genes _
                                    .Select(Function(gene)
                                                Return New [Property] With {
                                                    .name = gene.geneName,
                                                    .comment = gene.description,
                                                    .value = gene.description
                                                }
                                            End Function) _
                                    .ToArray
                            }
                        End Function) _
                .ToArray

            Yield New FunctionalCategory With {
                .category = category.Key,
                .pathways = pathways
            }
        Next
    End Function
End Module
