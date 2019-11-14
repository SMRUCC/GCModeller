#Region "Microsoft.VisualBasic::047de2211b7dcf5822269e57f4ca74ea, engine\Compiler\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: createEnzymes, createMaps, getCompounds, getGenes, getRNAs
    '               getTFregulations, populateReplicons, ToMarkup, ToTabular
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports Excel = Microsoft.VisualBasic.MIME.Office.Excel.File
Imports XmlReaction = SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2.Reaction

<HideModuleName>
Public Module Extensions

    <Extension>
    Friend Iterator Function populateReplicons(model As CellularModule, genomes As Dictionary(Of String, GBFF.File)) As IEnumerable(Of replicon)
        For Each genome In genomes
            Yield New replicon With {
                .genomeName = genome.Value.Locus.AccessionID,
                .genes = genome.Value _
                    .getGenes _
                    .ToArray,
                .RNAs = model _
                    .getRNAs(.genomeName) _
                    .ToArray,
                .isPlasmid = genome.Value.IsPlasmidSource
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension> Private Function getRNAs(model As CellularModule, repliconName$) As IEnumerable(Of RNA)
        Return model.Genotype _
            .centralDogmas _
            .Where(Function(proc)
                       Return proc.RNA.Value <> RNATypes.mRNA AndAlso repliconName = proc.replicon
                   End Function) _
            .Select(Function(proc)
                        Return New RNA With {
                            .type = proc.RNA.Value,
                            .val = proc.RNA.Description,
                            .gene = proc.geneID
                        }
                    End Function)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="genomes"></param>
    ''' <param name="KEGG"></param>
    ''' <param name="regulations">所有的复制子的调控网络应该都是合并在一起通过这个参数传递进来了</param>
    ''' <returns></returns>
    <Extension> Public Function ToMarkup(model As CellularModule,
                                         genomes As Dictionary(Of String, GBFF.File),
                                         KEGG As RepositoryArguments,
                                         regulations As RegulationFootprint()) As VirtualCell

        Dim KOgenes As Dictionary(Of String, CentralDogma) = model _
            .Genotype _
            .centralDogmas _
            .Where(Function(process)
                       Return Not process.IsRNAGene AndAlso Not process.orthology.StringEmpty
                   End Function) _
            .ToDictionary(Function(term) term.geneID)
        Dim enzymes As Enzyme() = model _
            .Regulations _
            .Where(Function(process)
                       Return process.type = Processes.MetabolicProcess
                   End Function) _
            .createEnzymes(KOgenes) _
            .ToArray
        Dim KOfunc As Dictionary(Of String, CentralDogma()) = KOgenes _
            .Values _
            .GroupBy(Function(proc) proc.orthology) _
            .ToDictionary(Function(KO) KO.Key,
                          Function(g) g.ToArray)

        Return New VirtualCell With {
            .taxonomy = model.Taxonomy,
            .genome = New Genome With {
                .replicons = model _
                    .populateReplicons(genomes) _
                    .ToArray,
                 .regulations = model _
                    .getTFregulations(regulations) _
                    .ToArray
            },
            .MetabolismStructure = New MetabolismStructure With {
                .Reactions = model _
                    .Phenotype _
                    .fluxes _
                    .Select(Function(r)
                                Return New XmlReaction With {
                                    .ID = r.ID,
                                    .name = r.name,
                                    .Equation = r.GetEquationString,
                                    .is_enzymatic = r.is_enzymatic
                                }
                            End Function) _
                    .ToArray,
                .Enzymes = enzymes,
                .Compounds = .Reactions _
                             .getCompounds(KEGG.GetCompounds) _
                             .ToArray,
                .maps = KEGG.GetPathways _
                    .PathwayMaps _
                    .createMaps(KOfunc) _
                    .ToArray
            }
        }
    End Function

    <Extension>
    Private Iterator Function createMaps(pathwayMaps As bGetObject.PathwayMap(), KOfunc As Dictionary(Of String, CentralDogma())) As IEnumerable(Of FunctionalCategory)
        Dim mapgroups = pathwayMaps _
            .Where(Function(map) Not map.brite Is Nothing) _
            .GroupBy(Function(map) map.brite.class)

        For Each category As IGrouping(Of String, bGetObject.PathwayMap) In mapgroups
            Dim maps As New List(Of Pathway)

            For Each map As bGetObject.PathwayMap In category
                Dim enzymeUnits = map.KEGGOrthology _
                    .Terms _
                    .SafeQuery _
                    .Where(Function(term)
                               Return KOfunc.ContainsKey(term.name)
                           End Function) _
                    .Select(Function(term)
                                Dim enzymeUnit = KOfunc(term.name) _
                                    .Select(Function(protein)
                                                Return New [Property] With {
                                                    .name = protein.polypeptide,
                                                    .Comment = protein.geneID,
                                                    .value = term.name
                                                }
                                            End Function) _
                                    .ToArray
                                Return enzymeUnit
                            End Function) _
                    .IteratesALL _
                    .ToArray

                If Not enzymeUnits.IsNullOrEmpty Then
                    maps += New Pathway With {
                        .ID = map.KOpathway,
                        .name = map.name,
                        .enzymes = enzymeUnits
                    }
                End If
            Next

            If Not maps = 0 Then
                Yield New FunctionalCategory With {
                    .category = category.Key,
                    .pathways = maps
                }
            End If
        Next
    End Function

    <Extension>
    Private Iterator Function getGenes(genome As GBFF.File) As IEnumerable(Of Gene)
        For Each gene As GeneBrief In genome.GbffToPTT(ORF:=False).GeneObjects
            Yield New Gene With {
                .left = gene.Location.Left,
                .right = gene.Location.Right,
                .locus_tag = gene.Synonym,
                .product = gene.Product,
                .protein_id = gene.PID,
                .strand = gene.Location.Strand.GetBriefCode
            }
        Next
    End Function

    <Extension>
    Private Iterator Function getTFregulations(model As CellularModule, regulations As RegulationFootprint()) As IEnumerable(Of transcription)
        Dim centralDogmas = model.Genotype.centralDogmas.ToDictionary(Function(d) d.geneID)

        For Each reg As RegulationFootprint In regulations
            Dim process As CentralDogma = centralDogmas.TryGetValue(reg.regulated)

            If process.geneID.StringEmpty Then
                Call $"{reg.regulated} process not found!".Warning
            End If

            Yield New transcription With {
                .biological_process = reg.biological_process,
                .effector = reg.effector,
                .mode = reg.mode,
                .regulator = reg.regulator,
                .target = reg.regulated,
                .motif = New Motif With {
                    .family = reg.family,
                    .left = reg.motif.Left,
                    .right = reg.motif.Right,
                    .strand = reg.motif.Strand.GetBriefCode,
                    .sequence = reg.sequenceData,
                    .distance = reg.distance
                },
                .centralDogma = process.ToString
            }
        Next
    End Function

    <Extension>
    Private Iterator Function getCompounds(reactions As IEnumerable(Of XmlReaction), compounds As CompoundRepository) As IEnumerable(Of Compound)
        Dim allCompoundId$() = reactions _
            .Select(Function(r)
                        Return Equation.TryParse(r.Equation) _
                                       .GetMetabolites _
                                       .Select(Function(compound) compound.ID)
                    End Function) _
            .IteratesALL _
            .Distinct _
            .ToArray

        For Each id As String In allCompoundId.Where(Function(cid) compounds.Exists(cid))
            Dim keggModel = compounds.GetByKey(id).Entity

            Yield New Compound With {
                .ID = id,
                .name = keggModel _
                    .CommonNames _
                    .ElementAtOrDefault(0, keggModel.Formula),
                .otherNames = keggModel _
                    .CommonNames _
                    .SafeQuery _
                    .Skip(1) _
                    .ToArray
            }
        Next
    End Function

    <Extension>
    Private Iterator Function createEnzymes(metabolicProcess As IEnumerable(Of Regulation), KOgenes As Dictionary(Of String, CentralDogma)) As IEnumerable(Of Enzyme)
        For Each catalysis As IGrouping(Of String, Regulation) In metabolicProcess.GroupBy(Function(c) c.regulator)
            Yield New Enzyme With {
                .geneID = catalysis.Key,
                .catalysis = catalysis _
                    .Select(Function(reg)
                                Return New Catalysis With {
                                    .coefficient = reg.effects,
                                    .reaction = reg.process,
                                    .comment = reg.name
                                }
                            End Function) _
                    .ToArray,
                .KO = KOgenes.TryGetValue(.geneID).orthology
            }
        Next
    End Function

    <Extension> Public Function ToTabular(model As CellularModule) As Excel

    End Function
End Module
