#Region "Microsoft.VisualBasic::afb8aee8670e7ce02e3c21c3159c29d8, R#\seqtoolkit\models\UniProtTable.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 141
    '    Code Lines: 131 (92.91%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (7.09%)
    '     File Size: 7.46 KB


    ' Module UniProtTable
    ' 
    '     Function: annotationIDs, fill, hmmerProfiles, ProteinTable
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports dataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe

Module UniProtTable

    <Extension>
    Private Iterator Function annotationIDs(all As entry(), db_xref As String, key As String) As IEnumerable(Of String)
        For Each p As entry In all
            Dim ref = p.xrefs.TryGetValue(db_xref)?.FirstOrDefault

            If ref Is Nothing Then
                Yield ""
            Else
                Yield ref(key)
            End If
        Next
    End Function

    <Extension>
    Private Iterator Function hmmerProfiles(prot As IEnumerable(Of entry), name As String) As IEnumerable(Of String)
        For Each p As entry In prot
            Dim hmmer = p.xrefs.TryGetValue(name)

            If hmmer.IsNullOrEmpty Then
                Yield ""
            Else
                Yield hmmer _
                    .Select(Function(a)
                                Return $"{a.id}({a("entry name")})"
                            End Function) _
                    .JoinBy("; ")
            End If
        Next
    End Function

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
        Dim Ensembl_protein As String() = all.annotationIDs("Ensembl", "protein sequence ID").fill
        Dim Ensembl_geneID As String() = all.annotationIDs("Ensembl", "gene ID").fill
        Dim EnsemblPlants As String() = all.annotationIDs("EnsemblPlants", "gene ID").fill
        Dim GeneID As String() = all.Select(Function(p) p.DbReferenceId("GeneID")).fill
        Dim Proteomes As String() = all.Select(Function(p) p.DbReferenceId("Proteomes")).fill
        Dim Bgee As String() = all.Select(Function(p) p.DbReferenceId("Bgee")).fill
        Dim eggNOG As String() = all.Select(Function(p) p.DbReferenceId("eggNOG")).fill
        Dim RefSeq As String() = all.Select(Function(p) p.DbReferenceId("RefSeq")).fill
        Dim KEGG As String() = all.Select(Function(p) p.DbReferenceId("KEGG")).fill
        Dim motif As String() = all _
            .Select(Function(p)
                        Return p.GetDomainData _
                            .Select(Function(d) $"{d.ID}:{d.name}({d.start}|{d.ends})") _
                            .JoinBy("+")
                    End Function) _
            .fill
        Dim pfam As String() = all.hmmerProfiles("Pfam").fill
        Dim interpro As String() = all.hmmerProfiles("InterPro").fill
        Dim SUPFAM As String() = all.hmmerProfiles("SUPFAM").fill
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

        Dim locations = all.Select(Function(p) p.SubCellularLocations.JoinBy("; ")).fill

        Return New dataframe With {
            .columns = New Dictionary(Of String, Array) From {
                {"name", name},
                {"orf", orf},
                {"geneName", geneName},
                {"fullName", fullName},
                {"EC_number", ECnumber},
                {"GeneID", GeneID},
                {"GO", GOterms},
                {"EMBL", EMBL},
                {"Ensembl", Ensembl},
                {"EnsemblPlants", EnsemblPlants},
                {"Ensembl_protein", Ensembl_protein},
                {"Ensembl_geneID", Ensembl_geneID},
                {"Proteomes", Proteomes},
                {"Bgee", Bgee},
                {"eggNOG", eggNOG},
                {"RefSeq", RefSeq},
                {"KEGG", KEGG},
                {"motif", motif},
                {"Pfam", pfam},
                {"InterPro", interpro},
                {"SUPFAM", SUPFAM},
                {"chain", chain},
                {"region of interest", region_of_interest},
                {"transmembrane_region", transmembrane_region},
                {"active_site", active_site},
                {"binding_site", binding_site},
                {"signal_peptide", signal_peptide},
                {"subcellularLocation", locations},
                {"NCBI_taxonomyId", NCBITaxonomyId},
                {"organism", organism}
            },
            .rownames = uniprotId
        }
    End Function

    <Extension>
    Private Function fill(col As IEnumerable(Of String), Optional fill_str As String = "-") As String()
        Return col.Select(Function(s) If(s = "", fill_str, s)).ToArray
    End Function
End Module

