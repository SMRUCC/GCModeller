Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

Public Module Utils

    <Extension>
    Public Function FromGenBank(genbank As GBFF.File) As Project
        Dim meta = genbank.createMetadata
        Dim proteins As New Dictionary(Of String, ProteinAnnotation)

        For Each prot As ProteinAnnotation In genbank.populateProteins
            If prot.sequence.StringEmpty Then
                Continue For
            End If

            Call proteins.Add(prot.geneId, prot)
        Next

        Return New Project With {
            .metadata = meta,
            .proteins = New ProteinSet With {
                .proteins = proteins
            }
        }
    End Function

    <Extension>
    Private Iterator Function populateProteins(genbank As GBFF.File) As IEnumerable(Of ProteinAnnotation)
        For Each feature As Feature In From f As Feature
                                       In genbank.Features
                                       Where f.KeyName.TextEquals("CDS")

            Yield New ProteinAnnotation With {
                .locus_id = feature("locus_tag"),
                .geneId = feature("protein_id"),
                .geneName = feature("gene"),
                .description = feature("product"),
                .sequence = feature("translation")
            }
        Next
    End Function

    <Extension>
    Private Function createMetadata(genbank As GBFF.File) As Sys_set
        Return New Sys_set With {
            .sysid = New sysid With {
                .bsid = genbank.Locus.AccessionID,
                .version = genbank.Locus.AccessionID & "." & genbank.Locus.UpdateTime
            },
            .names = {genbank.Definition.Value}
        }
    End Function
End Module
