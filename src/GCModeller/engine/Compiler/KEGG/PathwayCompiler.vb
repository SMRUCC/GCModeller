Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' Create virtual cell xml file model from KEGG pathway data
''' </summary>
Public Module PathwayCompiler

    <Extension>
    Public Function CompileOrganism(replicons As Dictionary(Of String, GBFF.File), keggModel As OrganismModel) As VirtualCell
        Dim taxonomy As Taxonomy = replicons.getTaxonomy
        Dim Kofunction As Dictionary(Of String, String) = keggModel.KoFunction
        Dim genotype As New Genotype With {
            .centralDogmas = replicons _
                .GetCentralDogmas(Kofunction) _
                .ToArray
        }
        Dim cell As New CellularModule With {
            .Taxonomy = taxonomy,
            .Genotype = genotype
        }

        Return cell.ToMarkup(replicons, keggModel)
    End Function

    <Extension>
    Private Function ToMarkup(cell As CellularModule, genomes As Dictionary(Of String, GBFF.File), kegg As OrganismModel) As VirtualCell
        Dim KOgenes As Dictionary(Of String, CentralDogma) = cell _
            .Genotype _
            .centralDogmas _
            .Where(Function(process)
                       Return Not process.IsRNAGene AndAlso Not process.orthology.StringEmpty
                   End Function) _
            .ToDictionary(Function(term) term.geneID)
        Dim pathwayIndex = kegg.genome.ToDictionary(Function(map) map.briteID)
        Dim maps As FunctionalCategory() = kegg _
            .genome _
            .Select(Function(pathway)
                        Return New FunctionalCategory
                    End Function) _
            .ToArray

        Return New VirtualCell With {
            .taxonomy = cell.Taxonomy,
            .genome = New Genome With {
                .replicons = cell _
                    .populateReplicons(genomes) _
                    .ToArray
            },
            .MetabolismStructure = New MetabolismStructure With {
                .maps = maps
            }
        }
    End Function
End Module
