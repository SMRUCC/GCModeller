Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports FluxModel = SMRUCC.genomics.GCModeller.ModellingEngine.Model.Reaction

Namespace v2

    Public Module ModelExtensions

        ''' <summary>
        ''' 将所加载的XML模型文件转换为统一的数据模型
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        <Extension> Public Function CreateModel(model As VirtualCell) As CellularModule
            Return New CellularModule With {
                .Taxonomy = model.taxonomy,
                .Genotype = New Genotype With {
                    .centralDogmas = model _
                        .createGenotype _
                        .OrderByDescending(Function(gene) gene.RNA.Value) _
                        .ToArray
                },
                .Phenotype = model.createPhenotype,
                .Regulations = model.exportRegulations.ToArray
            }
        End Function

        <Extension>
        Private Iterator Function createGenotype(model As VirtualCell) As IEnumerable(Of CentralDogma)
            Dim genomeName$
            Dim enzymes As Dictionary(Of String, Enzyme) = model.MetabolismStructure _
                .Enzymes _
                .ToDictionary(Function(enzyme) enzyme.geneID)
            Dim rnaTable As Dictionary(Of String, NamedValue(Of RNATypes))
            Dim RNA As NamedValue(Of RNATypes)
            Dim proteinId$

            For Each replicon In model.genome.replicons
                genomeName = replicon.genomeName
                rnaTable = replicon _
                    .RNAs _
                    .ToDictionary(Function(r) r.gene,
                                  Function(r)
                                      Return New NamedValue(Of RNATypes) With {
                                          .Name = r.gene,
                                          .Value = r.type,
                                          .Description = r.val
                                      }
                                  End Function)

                For Each gene As gene In replicon.genes
                    If rnaTable.ContainsKey(gene.locus_tag) Then
                        RNA = rnaTable(gene.locus_tag)
                        proteinId = Nothing
                    Else
                        ' 枚举的默认值为mRNA
                        RNA = New NamedValue(Of RNATypes) With {
                            .Name = gene.locus_tag
                        }
                        proteinId = gene.protein_id Or $"{gene.locus_tag}::peptide".AsDefault
                    End If

                    Yield New CentralDogma With {
                        .replicon = genomeName,
                        .geneID = gene.locus_tag,
                        .polypeptide = proteinId,
                        .orthology = enzymes.TryGetValue(.geneID)?.KO,
                        .RNA = RNA
                    }
                Next
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Private Function createPhenotype(model As VirtualCell) As Phenotype
            Return New Phenotype With {
                .fluxes = model.createFluxes _
                    .OrderByDescending(Function(r) r.enzyme.SafeQuery.Count) _
                    .ToArray
            }
        End Function

        <Extension>
        Private Iterator Function createFluxes(model As VirtualCell) As IEnumerable(Of FluxModel)
            Dim equation As Equation
            Dim enzymes = model.MetabolismStructure _
                .Enzymes _
                .Select(Function(enz)
                            Return enz _
                                .catalysis _
                                .Select(Function(ec)
                                            Return (rID:=ec.reaction, enz:=enz.KO)
                                        End Function)
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(r) r.rID) _
                .ToDictionary(Function(r) r.Key,
                              Function(g)
                                  Return g.Select(Function(r) r.enz) _
                                          .Distinct _
                                          .ToArray
                              End Function)

            For Each reaction In model.MetabolismStructure.Reactions
                equation = Equation.TryParse(reaction.Equation)

                Yield New FluxModel With {
                    .ID = reaction.ID,
                    .name = reaction.name,
                    .substrates = equation.Reactants _
                        .Select(Function(c) c.AsFactor) _
                        .ToArray,
                    .products = equation.Products _
                        .Select(Function(c) c.AsFactor) _
                        .ToArray,
                    .enzyme = enzymes.TryGetValue(.ID)
                }
            Next
        End Function

        <Extension>
        Private Iterator Function exportRegulations(model As VirtualCell) As IEnumerable(Of Regulation)
            For Each reg As transcription In model.genome.regulations
                Yield New Regulation With {
                    .effects = reg.mode.EvalEffects,
                    .name = reg.biological_process,
                    .process = reg.centralDogma,
                    .regulator = reg.regulator,
                    .type = Processes.Transcription
                }
            Next
        End Function
    End Module
End Namespace