Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports dataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe

Module UniProtTable

    <Extension>
    Public Function ProteinTable(prot As IEnumerable(Of entry)) As dataframe
        Dim all As entry() = prot.ToArray
        Dim orf As String() = all.Select(Function(p) p.ORF).fill
        Dim uniprotId As String() = all.Select(Function(p) p.accessions(Scan0)).fill
        Dim name As String() = all.Select(Function(p) p.name).fill
        Dim geneName As String() = all.Select(Function(p) p.gene?.Primary.JoinBy("; ")).fill
        Dim fullName As String() = all.Select(Function(p) p.proteinFullName).fill
        Dim organism As String() = all.Select(Function(p) p.OrganismScientificName).fill
        Dim NCBITaxonomyId As String() = all.Select(Function(p) p.NCBITaxonomyId).fill
        Dim ECnumber As String() = all.Select(Function(p) p.ECNumberList.JoinBy("; ")).fill
        Dim GOterms As String() = all.Select(Function(p) p.GO.Select(Function(r) r.id).Distinct.JoinBy("; ")).fill
        Dim EMBL As String() = all.Select(Function(p) p.DbReferenceId("EMBL")).fill
        Dim Ensembl As String() = all.Select(Function(p) p.DbReferenceId("Ensembl")).fill
        Dim Ensembl_protein As String() = all _
            .Select(Function(p)
                        Dim ref = p.xrefs.TryGetValue("Ensembl")?.FirstOrDefault

                        If ref Is Nothing Then
                            Return ""
                        Else
                            Return ref("protein sequence ID")
                        End If
                    End Function) _
            .fill
        Dim Ensembl_geneID As String() = all _
            .Select(Function(p)
                        Dim ref = p.xrefs.TryGetValue("Ensembl")?.FirstOrDefault

                        If ref Is Nothing Then
                            Return ""
                        Else
                            Return ref("gene ID")
                        End If
                    End Function) _
            .fill
        Dim EnsemblPlants As String() = all _
            .Select(Function(p)
                        Dim ref = p.xrefs.TryGetValue("EnsemblPlants")?.FirstOrDefault

                        If ref Is Nothing Then
                            Return ""
                        Else
                            Return ref("gene ID")
                        End If
                    End Function) _
            .fill
        Dim GeneID As String() = all.Select(Function(p) p.DbReferenceId("GeneID")).fill
        Dim Proteomes As String() = all.Select(Function(p) p.DbReferenceId("Proteomes")).fill
        Dim Bgee As String() = all.Select(Function(p) p.DbReferenceId("Bgee")).fill
        Dim eggNOG As String() = all.Select(Function(p) p.DbReferenceId("eggNOG")).fill
        Dim RefSeq As String() = all.Select(Function(p) p.DbReferenceId("RefSeq")).fill
        Dim KEGG As String() = all.Select(Function(p) p.DbReferenceId("KEGG")).fill
        Dim motif As String() = all _
            .Select(Function(p)
                        Return p.GetDomainData _
                            .Select(Function(d) $"{d.DomainId}({d.start}|{d.ends})") _
                            .JoinBy("+")
                    End Function) _
            .fill
        Dim signal_peptide = all _
            .Select(Function(p)
                        Return p.features _
                            .SafeQuery _
                            .Where(Function(f) f.type = "signal peptide") _
                            .Select(Function(f) $"{f.type}({f.location.begin}|{f.location.end})") _
                            .JoinBy("+")
                    End Function) _
            .fill
        Dim region_of_interest = all.Select(Function(p)
                                                Return p.features.SafeQuery.Where(Function(f) f.type = "region of interest").Select(Function(f) $"{f.description}({f.location.begin}|{f.location.end})").JoinBy("; ")
                                            End Function).fill
        Dim active_site = all.Select(Function(p)
                                         Return p.features.SafeQuery.Where(Function(f) f.type = "active site").Select(Function(f) $"{f.description}_{f.location.position}").JoinBy("; ")
                                     End Function).fill
        Dim binding_site = all.Select(Function(p)
                                          Return p.features.SafeQuery.Where(Function(f) f.type = "binding site").Select(Function(f) $"{f.description}_{f.location.position}").JoinBy("; ")
                                      End Function).fill
        Dim chain = all.Select(Function(p)
                                   Return p.features.SafeQuery.Where(Function(f) f.type = "chain").Select(Function(f) $"{f.description}({f.location.begin}|{f.location.end})").JoinBy("; ")
                               End Function).fill

        Dim transmembrane_region = all.Select(Function(p)
                                                  Return p.features.SafeQuery.Where(Function(f) f.type = "transmembrane region").Select(Function(f) $"{f.description}({f.location.begin}|{f.location.end})").JoinBy("; ")
                                              End Function).fill

        Dim locations = all.Select(Function(p)
                                       Return p.SubCellularLocations.JoinBy("; ")
                                   End Function) _
                           .fill

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"uniprotId", uniprotId},
                {"name", name},
                {"orf", orf},
                {"geneName", geneName},
                {"fullName", fullName},
                {"EC_number", ECnumber},
                {"GeneID", GeneID},
                {"GO", GOterms},
                {"EMBL", EMBL},
                {"Ensembl", Ensembl},
                {"Ensembl_protein", Ensembl_protein},
                {"Ensembl_geneID", Ensembl_geneID},
                {"Proteomes", Proteomes},
                {"Bgee", Bgee},
                {"eggNOG", eggNOG},
                {"RefSeq", RefSeq},
                {"KEGG", KEGG},
                {"motif", motif},
                {"chain", chain},
                {"region of interest", region_of_interest},
                {"transmembrane_region", transmembrane_region},
                {"active_site", active_site},
                {"binding_site", binding_site},
                {"signal_peptide", signal_peptide},
                {"subcellularLocation", locations},
                {"NCBI_taxonomyId", NCBITaxonomyId},
                {"organism", organism}
            }
        }
    End Function

    <Extension>
    Private Function fill(col As IEnumerable(Of String), Optional fill_str As String = "-") As String()
        Return col.Select(Function(s) If(s = "", fill_str, s)).ToArray
    End Function
End Module
