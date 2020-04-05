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

        ReadOnly genomes As Dictionary(Of String, GBFF.File)
        ReadOnly KEGG As RepositoryArguments

        Friend ReadOnly regulations As RegulationFootprint()
        Friend ReadOnly model As CellularModule
        Friend ReadOnly locationAsLocus_tag As Boolean

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="model">中间模型数据</param>
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

        Protected Overrides Function PreCompile(args As CommandLine) As Integer
            m_compiledModel = New VirtualCell With {
                .taxonomy = model.Taxonomy
            }

            Return 0
        End Function

        Protected Overrides Function CompileImpl(args As CommandLine) As Integer
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
            Dim genomeCompiler As New CompileGenomeWorkflow(Me)
            Dim TRNCompiler As New CompileTRNWorkflow(Me)

            m_compiledModel.genome = New Genome With {
                .replicons = genomeCompiler _
                    .populateReplicons(genomes) _
                    .ToArray,
                .regulations = TRNCompiler _
                    .getTFregulations(allCompounds.CreateMapping) _
                    .ToArray
            }
            m_compiledModel.metabolismStructure = New MetabolismStructure With {
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

            Return 0
        End Function
    End Class
End Namespace