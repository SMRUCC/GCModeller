Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports XmlReaction = SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2.Reaction

Namespace MarkupCompiler

    Public Class CompileGenomeWorkflow : Inherits CompilerWorkflow

        Sub New(compiler As v2MarkupCompiler)
            Call MyBase.New(compiler)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="model"></param>
        ''' <param name="genomes">染色体基因组+质粒基因组</param>
        ''' <returns></returns>
        Friend Iterator Function populateReplicons(model As CellularModule, genomes As Dictionary(Of String, GBFF.File), locationAsLocustag As Boolean) As IEnumerable(Of replicon)
            For Each genome In genomes
                Yield New replicon With {
                    .genomeName = genome.Value.Locus.AccessionID,
                    .genes = genome.Value _
                        .getGenes(model, locationAsLocustag) _
                        .ToArray,
                    .RNAs = model _
                        .getRNAs(.genomeName) _
                        .ToArray,
                    .isPlasmid = genome.Value.isPlasmid
                }
            Next
        End Function

        Private Function getRNAs(model As CellularModule, repliconName$) As IEnumerable(Of RNA)
            Return model.Genotype _
                .centralDogmas _
                .Where(Function(proc)
                           Return proc.RNA.Value <> RNATypes.mRNA AndAlso repliconName = proc.replicon
                       End Function) _
                .Select(Function(proc)
                            Return New RNA With {
                                .Type = proc.RNA.Value,
                                .Val = proc.RNA.Description,
                                .gene = proc.geneID
                            }
                        End Function)
        End Function
    End Class
End Namespace