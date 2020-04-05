Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

Namespace MarkupCompiler

    Public Class CompileGenomeWorkflow : Inherits CompilerWorkflow

        Sub New(compiler As v2MarkupCompiler)
            Call MyBase.New(compiler)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="genomes">染色体基因组+质粒基因组</param>
        ''' <returns></returns>
        Friend Iterator Function populateReplicons(genomes As Dictionary(Of String, GBFF.File)) As IEnumerable(Of replicon)
            Dim genePopulator As New CompileGeneModelWorkflow(compiler)

            For Each genome In genomes
                Yield New replicon With {
                    .genomeName = genome.Value.Locus.AccessionID,
                    .genes = genePopulator.getGenes(genome.Value).ToArray,
                    .RNAs = getRNAs(.genomeName).ToArray,
                    .isPlasmid = genome.Value.isPlasmid
                }
            Next
        End Function

        Private Function getRNAs(repliconName$) As IEnumerable(Of RNA)
            Dim cdProcess As CentralDogma() = compiler.model _
                .Genotype _
                .centralDogmas

            Return cdProcess _
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
    End Class
End Namespace