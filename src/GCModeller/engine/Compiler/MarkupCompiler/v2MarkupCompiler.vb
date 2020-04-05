Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.CompilerServices
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports XmlReaction = SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2.Reaction

Namespace MarkupCompiler

    Public Class v2MarkupCompiler : Inherits Compiler(Of VirtualCell)

        ReadOnly model As CellularModule
        ReadOnly genomes As Dictionary(Of String, GBFF.File)
        ReadOnly KEGG As RepositoryArguments
        ReadOnly regulations As RegulationFootprint()
        ReadOnly locationAsLocus_tag As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="genomes"></param>
        ''' <param name="KEGG"></param>
        ''' <param name="regulations">
        ''' 所有的复制子的调控网络应该都是合并在一起通过这个参数传递进来了
        ''' </param>
        ''' <param name="locationAsLocus_tag"></param>
        Sub New(model As CellularModule,
            genomes As Dictionary(Of String, GBFF.File),
            KEGG As RepositoryArguments,
            regulations As RegulationFootprint(),
            locationAsLocus_tag As Boolean)

            Me.model = model
            Me.genomes = genomes
            Me.KEGG = KEGG
            Me.regulations = regulations
            Me.locationAsLocus_tag = locationAsLocus_tag
        End Sub

        Public Overrides Function Compile(Optional args As CommandLine = Nothing) As VirtualCell
            Dim KOgenes As Dictionary(Of String, CentralDogma) = model _
                .Genotype _
                .centralDogmas _
                .Where(Function(process)
                           Return Not process.IsRNAGene AndAlso Not process.orthology.StringEmpty
                       End Function) _
                .ToDictionary(Function(term) term.geneID)
            Dim enzymes As Enzyme() = model.Regulations _
                .Where(Function(process)
                           Return process.type = Processes.MetabolicProcess
                       End Function) _
                .createEnzymes(KOgenes) _
                .ToArray
            Dim KOfunc As Dictionary(Of String, CentralDogma()) = KOgenes.Values _
                .GroupBy(Function(proc) proc.orthology) _
                .ToDictionary(Function(KO) KO.Key,
                              Function(g)
                                  Return g.ToArray
                              End Function)
            Dim allCompounds As CompoundRepository = KEGG.GetCompounds

            Return New VirtualCell With {
                .taxonomy = model.Taxonomy,
                .genome = New Genome With {
                    .replicons = model _
                        .populateReplicons(genomes, locationAsLocus_tag) _
                        .ToArray,
                     .regulations = model _
                        .getTFregulations(regulations, allCompounds.CreateMapping) _
                        .ToArray
                },
                .metabolismStructure = New MetabolismStructure With {
                    .reactions = model _
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
                    .enzymes = enzymes,
                    .compounds = .reactions _
                                 .AsEnumerable _
                                 .getCompounds(allCompounds) _
                                 .ToArray,
                    .maps = KEGG.GetPathways _
                        .PathwayMaps _
                        .createMaps(KOfunc) _
                        .ToArray
                }
            }
        End Function
    End Class
End Namespace