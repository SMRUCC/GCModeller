Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.GCModeller
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.Metagenomics
Imports vcellkit

Module Program

    Sub Main(args As String())
        Call test_model1()
    End Sub

    Private Sub test_model1()
        Dim cell As New VirtualCell With {
            .genome = New Genome With {
                .replicons = {
                    New replicon With {
                        .operons = {
                            New TranscriptUnit With {
                                .id = "Gene-123",
                                .genes = {
                                    New gene With {.type = RNATypes.mRNA, .locus_tag = "gene1", .protein_id = "protein1", .product = "peptide1", .nucleotide_base = New NumericVector, .amino_acid = New NumericVector},
                                    New gene With {.type = RNATypes.mRNA, .locus_tag = "gene2", .protein_id = "protein2", .product = "peptide2", .nucleotide_base = New NumericVector, .amino_acid = New NumericVector},
                                    New gene With {.type = RNATypes.mRNA, .locus_tag = "gene3", .protein_id = "protein3", .product = "peptide3", .nucleotide_base = New NumericVector, .amino_acid = New NumericVector}
                                }
                            }
                        }
                    }
                }
            },
            .properties = New CompilerServices.[Property] With {.name = "", .authors = {"xieguigang"}, .comment = ""},
            .taxonomy = New Taxonomy With {.species = "1"},
            .metabolismStructure = New MetabolismStructure With {
                .compounds = {
                    New Compound("A", "A"),
                    New Compound("B", "B"),
                    New Compound("C", "C")
                }
            }
        }

        Call vcellModeller.writeJSON(cell, "./cell1.json", indent:=True)
    End Sub
End Module
