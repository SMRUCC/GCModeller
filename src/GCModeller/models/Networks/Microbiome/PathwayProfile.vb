Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Metagenomics
Imports RDotNET.Extensions.VisualBasic.API
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Model.Network.KEGG

''' <summary>
''' The microbiome KEGG pathway profile.
''' </summary>
Public Module PathwayProfile

    <Extension>
    Public Function PathwayProfile(metagenome As IEnumerable(Of TaxonomyRef), maps As MapRepository, Optional coverage# = 0.3) As Dictionary(Of String, Double)
        Dim profile As New Dictionary(Of String, Counter)

        For Each genome As TaxonomyRef In metagenome
            Dim KOlist$() = genome.KOTerms
            Dim pathways = maps _
                .QueryMapsByMembers(KOlist) _
                .Where(Function(map)
                           With map.Index.Objects.Where(Function(id) id.IsPattern("KO\d+")).ToArray
                               Return .Intersect(KOlist).Count / .Length >= coverage
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
    Public Function PathwayProfiles(gast As IEnumerable(Of gast.gastOUT),
                                    uniprot As TaxonomyRepository,
                                    ref As MapRepository,
                                    Optional rank As TaxonomyRanks = TaxonomyRanks.Genus) As Dictionary(Of String, (profile#, pvalue#))

        Dim taxonomyGroup = gast.TaxonomyProfile(rank, percentage:=True)
        Dim profiles = taxonomyGroup _
            .Select(Function(tax)
                        Dim name$ = tax.Key
                        Dim taxonomy As New Taxonomy(BIOMTaxonomy.TaxonomyParser(name))
                        Dim profile = taxonomy.PathwayProfiles(uniprot, ref)

                        Return (tax:=name, profile:=profile, pct:=tax.Value)
                    End Function) _
            .ToArray

        ' 转换为每一个mapID对应的pathway按照taxonomy排列的向量
        Dim ZERO#() = Repeats(0R, profiles.Length)
        Dim profileTable = profiles _
            .Select(Function(tax) tax.profile.Keys) _
            .IteratesALL _
            .Distinct _
            .OrderBy(Function(id) id) _
            .ToDictionary(Function(mapID) mapID,
                          Function(mapID)
                              Dim vector#() = profiles _
                                  .Select(Function(tax)
                                              Return If(tax.profile.ContainsKey(mapID), tax.profile(mapID), 0) * tax.pct
                                          End Function) _
                                  .ToArray

                              ' student t test
                              Dim pvalue# = stats.Ttest(vector, ZERO, varEqual:=True).pvalue
                              Dim profile# = vector.Sum

                              Return (profile, pvalue)
                          End Function)

        Return profileTable
    End Function

    <Extension>
    Public Function MicrobiomePathwayNetwork(profiles As Dictionary(Of String, (profile#, pvalue#)), KEGG As MapRepository, Optional cutoff# = 0.05) As NetworkGraph
        Dim idlist = profiles.Where(Function(map) map.Value.pvalue <= cutoff).Keys
        Dim maps = idlist.Select(Function(mapID) KEGG.GetByKey(mapID).Map).ToArray
        Dim network As NetworkGraph = maps.BuildNetwork(
            Sub(mapNode)
                With profiles(mapNode.Label)
                    mapNode.Data!pvalue = -Math.Log10(.pvalue)
                    mapNode.Data!profile = .profile
                End With
            End Sub)

        Return network
    End Function
End Module
