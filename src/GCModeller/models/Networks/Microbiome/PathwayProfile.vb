Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports RDotNET.Extensions.VisualBasic.API
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.Model.Network.KEGG
Imports Numeric = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

''' <summary>
''' The microbiome KEGG pathway profile.
''' </summary>
Public Module PathwayProfile

    <Extension>
    Public Function PathwayProfile(metagenome As IEnumerable(Of TaxonomyRef), maps As MapRepository, Optional coverage# = 0.3) As Dictionary(Of String, Double)
        Dim profile As New Dictionary(Of String, Counter)

        For Each genome As TaxonomyRef In metagenome
            Dim KOlist$() = genome.KOTerms
            ' query KEGG map这里是主要的限速步骤
            Dim pathways = maps _
                .QueryMapsByMembers(KOlist) _
                .Where(Function(map)
                           With map.KOIndex
                               Return .Intersect(KOlist).Count / .Count >= coverage
                           End With
                       End Function) _
                .ToArray

            For Each map In pathways
                If Not profile.ContainsKey(map.MapID) Then
                    Call profile.Add(map.MapID, New Counter)
                End If

                Call profile(map.MapID).Hit()
            Next
        Next

        Return profile.AsNumeric
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function PathwayProfiles(taxonomy As Taxonomy, uniprot As TaxonomyRepository, ref As MapRepository) As Dictionary(Of String, Double)
        Return uniprot.PopulateModels({taxonomy}, distinct:=True).PathwayProfile(ref)
    End Function

    <Extension>
    Public Function CreateProfile(gast As IEnumerable(Of gast.gastOUT),
                                  uniprot As TaxonomyRepository,
                                  ref As MapRepository,
                                  Optional rank As TaxonomyRanks = TaxonomyRanks.Genus) As Dictionary(Of String, Profile())

        Dim taxonomyGroup As gast.gastOUT() = gast.ToArray
        Dim ALL = taxonomyGroup.Select(Function(tax) tax.counts).Sum
        Dim profiles = taxonomyGroup _
            .AsParallel _
            .Select(Function(tax)
                        Dim name$ = tax.taxonomy
                        Dim taxonomy As New Taxonomy(BIOMTaxonomy.TaxonomyParser(name))
                        Dim profile = taxonomy.PathwayProfiles(uniprot, ref)

                        Return New Profile(
                            tax:=taxonomy,
                            profile:=profile,
                            pct:=tax.counts / ALL
                        ) With {
                            .RankGroup = taxonomy.TaxonomyRankString(rank)
                        }
                    End Function) _
            .ToArray

        ' 下面按照rank进行数据分组
        Dim profileGroup = profiles _
            .GroupBy(Function(tax) tax.RankGroup) _
            .ToDictionary(Function(g) g.Key,
                          Function(profile) profile.ToArray)

        Return profileGroup
    End Function

    Public Class Profile
        Public Property Taxonomy As Taxonomy
        Public Property Profile As Dictionary(Of String, Double)
        Public Property pct As Double
        Public Property RankGroup As String

        Sub New()
        End Sub

        Sub New(tax As Taxonomy, profile As Dictionary(Of String, Double), pct#)
            Me.Taxonomy = tax
            Me.Profile = profile
            Me.pct = pct
        End Sub
    End Class

    <Extension>
    Public Function ProfileEnrichment(profileData As IEnumerable(Of Profile)) As Dictionary(Of String, (profile#, pvalue#))
        ' 转换为每一个mapID对应的pathway按照taxonomy排列的向量
        Dim profiles = profileData.ToArray
        Dim profileTable = profiles _
            .Select(Function(tax) tax.Profile.Keys) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(id) id) _
            .ToDictionary(Function(mapID) mapID,
                          Function(mapID)
                              Return profiles.EnrichmentTestInternal(mapID)
                          End Function)

        Return profileTable
    End Function

    <Extension>
    Private Function EnrichmentTestInternal(profiles As IEnumerable(Of Profile), mapID$) As (profile#, pvalue#)
        Dim vector#() = profiles _
            .Where(Function(tax) tax.Profile.ContainsKey(mapID)) _
            .Where(Function(tax) tax.Profile(mapID) > 0R) _
            .Select(Function(tax) tax.Profile(mapID) * tax.pct) _
            .ToArray
        Dim ZERO#() = Repeats(0R, vector.Length)

        Dim profile# = vector.Sum
        ' student t test
        Dim pvalue#
        Dim x0 = vector.FirstOrDefault

        If vector.Length < 3 Then
            pvalue = 1
        ElseIf vector.All(Function(x) x = x0) Then
            If x0 = 0R Then
                pvalue = 1
            Else
                pvalue = 0
            End If
        Else
            ' 可能有很多零
            pvalue = stats.Ttest(vector, ZERO, varEqual:=True).pvalue
        End If

        Return (profile, pvalue)
    End Function

    Public Class EnrichmentProfiles
        Public Property RankGroup As String
        Public Property pathway As String
        Public Property profile As Double
        Public Property pvalue As Double
    End Class

    <Extension>
    Public Function PathwayProfiles(gast As IEnumerable(Of gast.gastOUT),
                                    uniprot As TaxonomyRepository,
                                    ref As MapRepository,
                                    Optional rank As TaxonomyRanks = TaxonomyRanks.Genus) As EnrichmentProfiles()

        Dim profiles = gast.CreateProfile(uniprot, ref, rank)
        Dim profileTable = profiles _
            .Select(Function(group)
                        Dim tax = group.Key
                        Dim enrichment = group.Value.ProfileEnrichment

                        Return enrichment _
                            .Select(Function(pathway)
                                        Return New EnrichmentProfiles With {
                                            .pathway = pathway.Key,
                                            .profile = pathway.Value.profile,
                                            .pvalue = pathway.Value.pvalue,
                                            .RankGroup = tax
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray

        Return profileTable
    End Function

    <Extension>
    Public Function MicrobiomePathwayNetwork(profiles As EnrichmentProfiles(), KEGG As MapRepository, Optional cutoff# = 0.05) As NetworkGraph
        Dim idlist = profiles _
            .Where(Function(map) map.pvalue <= cutoff) _
            .Select(Function(map) map.pathway) _
            .ToArray
        Dim mapGroup = profiles _
            .GroupBy(Function(map) map.pathway) _
            .ToDictionary(Function(g) g.Key,
                          Function(mapProfiles) mapProfiles.ToArray)
        Dim maps = idlist _
            .Select(Function(mapID) KEGG.GetByKey(mapID).Map) _
            .ToArray
        Dim network As NetworkGraph = maps.BuildNetwork(
            Sub(mapNode)
                With mapGroup(mapNode.Label).Shadows
                    mapNode.Data!pvalue = (-Numeric.Log10(!pvalue)).Average
                    mapNode.Data!profile = !profile.Average
                End With
            End Sub)

        Return network
    End Function
End Module
