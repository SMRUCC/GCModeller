Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.KEGG.Metabolism
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.Metagenomics

Public Module Workflow

    ''' <summary>
    ''' 输出Model，然后再从Model写出模型文件
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="KOfunction"></param>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <Extension>
    Public Function AssemblingModel(genome As GBFF.File, KOfunction As Dictionary(Of String, String), repo As RepositoryArguments) As CellularModule
        Dim taxonomy As Taxonomy = genome.Source.GetTaxonomy
        Dim genotype As New Genotype With {
            .CentralDogmas = genome _
                .GetCentralDogmas _
                .ToArray
        }
        Dim phenotype As New Phenotype With {
            .fluxes = repo _
                .KEGGReactions _
                .FetchReactionRepository _
                .BuildReactions _
                .ToArray
        }

        Return New CellularModule With {
            .Taxonomy = taxonomy,
            .Genotype = genotype,
            .Phenotype = phenotype
        }
    End Function

    <Extension>
    Private Iterator Function BuildReactions(repo As ReactionRepository) As IEnumerable(Of Reaction)
        For Each reaction In repo.MetabolicNetwork
            Dim model As Equation = reaction.ReactionModel
            Dim enzymes$() = {}

            If Not reaction.Orthology.Terms Is Nothing Then
                enzymes = reaction _
                    .Orthology _
                    .Terms _
                    .Select(Function(t) t.name) _
                    .ToArray
            End If

            Yield New Reaction With {
                .enzyme = enzymes,
                .ID = reaction.ID,
                .substrates = model.Reactants.converts,
                .products = model.Products.converts,
                .name = reaction _
                    .CommonNames _
                    .ElementAtOrDefault(0, reaction.Definition)
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Private Function converts(compounds As CompoundSpecieReference()) As FactorString(Of Double)()
        Return compounds _
            .Select(Function(r)
                        Return New FactorString(Of Double) With {
                            .Factor = r.StoiChiometry,
                            .text = r.ID
                        }
                    End Function) _
            .ToArray
    End Function

    ReadOnly centralDogmaComponents As Index(Of String) = {"gene", "CDS", "tRNA", "rRNA"}

    <Extension>
    Private Iterator Function GetCentralDogmas(genome As GBFF.File) As IEnumerable(Of CentralDogma)
        Dim centralDogmaFeatures = genome _
            .Features _
            .Where(Function(feature)
                       Return feature _
                           .KeyName _
                           .IsOneOfA(centralDogmaComponents)
                   End Function) _
            .GroupBy(Function(feature)
                         Return feature.Query("locus_tag")
                     End Function)

        For Each feature As IGrouping(Of String, Feature) In centralDogmaFeatures
            Dim gene As Feature = feature.First(Function(component) component.KeyName = "gene")
            Dim RNA As Feature = feature _
                .FirstOrDefault(Function(component)
                                    Return component.KeyName = "tRNA" OrElse component.KeyName = "rRNA"
                                End Function)
            Dim CDS As Feature = feature _
                .FirstOrDefault(Function(component)
                                    Return component.KeyName = "CDS"
                                End Function)
            Dim locus_tag$ = feature.Key
            Dim rnaType As RNATypes = RNATypes.mRNA
            Dim proteinId As String = Nothing

            If Not RNA Is Nothing Then
                If RNA.KeyName = "tRNA" Then
                    rnaType = RNATypes.tRNA
                Else
                    rnaType = RNATypes.ribosomalRNA
                End If
            ElseIf Not CDS Is Nothing Then
                proteinId = CDS.Query("protein_id")

                If proteinId.StringEmpty Then
                    proteinId = CDS.Query("db_xref")
                End If
                If proteinId.StringEmpty Then
                    proteinId = $"{locus_tag}::peptide"
                End If
            Else
                ' 既没有RNA也没有CDS，这个可能是其他的类型的feature
                ' 例如移动原件之类的
                ' 跳过这些
                Call $"Skip invalid locus_tag: {feature.Key}".Warning

                Continue For
            End If

            Yield New CentralDogma With {
                .geneID = locus_tag,
                .RNA = New NamedValue(Of RNATypes) With {
                    .Name = locus_tag,
                    .Value = rnaType
                },
                .polypeptide = proteinId
            }
        Next
    End Function
End Module
