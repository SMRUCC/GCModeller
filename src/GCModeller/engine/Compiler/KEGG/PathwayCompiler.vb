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
            .Genotype = genotype,'.Phenotype = Phenotype,
            .Regulations = Kofunction _
                .createMetabolicProcess(repo.GetReactions) _
                .ToArray
        }

    End Function
End Module
