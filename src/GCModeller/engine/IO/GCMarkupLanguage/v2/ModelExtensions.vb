Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model

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
                        .ToArray
                },
                .Phenotype = model.createPhenotype,
                .Regulations = model.exportRegulations
            }
        End Function

        <Extension>
        Private Iterator Function createGenotype(model As VirtualCell) As IEnumerable(Of CentralDogma)
            Dim genomeName$
            Dim enzymes As Dictionary(Of String, Enzyme) = model.MetabolismStructure _
                .Enzymes _
                .ToDictionary(Function(enzyme) enzyme.geneID)

            For Each replicon In model.genome.replicons
                genomeName = replicon.genomeName

                For Each gene As gene In replicon.genes
                    Yield New CentralDogma With {
                        .replicon = genomeName,
                        .polypeptide = gene.protein_id,
                        .geneID = gene.locus_tag,
                        .orthology = enzymes.TryGetValue(.geneID)?.KO,
                        .RNA = New NamedValue(Of RNATypes)
                    }
                Next
            Next
        End Function

        <Extension>
        Private Function istRNA(gene As gene) As Boolean

        End Function

        <Extension>
        Private Function createPhenotype(model As VirtualCell) As Phenotype

        End Function

        <Extension>
        Private Iterator Function exportRegulations(model As VirtualCell) As IEnumerable(Of Regulation)

        End Function
    End Module
End Namespace