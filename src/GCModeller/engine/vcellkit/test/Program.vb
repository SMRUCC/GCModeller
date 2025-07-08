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
                                    New gene With {.type = RNATypes.mRNA, .locus_tag = "gene1", .protein_id = "protein1", .product = "peptide1", .nucleotide_base = New NumericVector("gene1", 1, 1, 1, 1), .amino_acid = New NumericVector("peptide1", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)},
                                    New gene With {.type = RNATypes.mRNA, .locus_tag = "gene2", .protein_id = "protein2", .product = "peptide2", .nucleotide_base = New NumericVector("gene2", 1, 1, 1, 1), .amino_acid = New NumericVector("peptide2", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)},
                                    New gene With {.type = RNATypes.mRNA, .locus_tag = "gene3", .protein_id = "protein3", .product = "peptide3", .nucleotide_base = New NumericVector("gene3", 1, 1, 1, 1), .amino_acid = New NumericVector("peptide3", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)}
                                }
                            }
                        }
                    }
                }
            },
            .properties = New CompilerServices.[Property] With {.name = "demo1", .authors = {"xieguigang"}, .comment = "simple network demo", .title = .comment},
            .taxonomy = New Taxonomy With {.species = "1"},
            .metabolismStructure = New MetabolismStructure With {
                .compounds = {
                    New Compound("A", "A"),
                    New Compound("B", "B"),
                    New Compound("C", "C")
                },
                .reactions = New ReactionGroup With {
                    .enzymatic = {
                        New Reaction With {.bounds = {10, 10}, .ID = "A->B", .name = "A->B", .substrate = {New CompoundFactor("A", 1)}, .product = {New CompoundFactor("B", 1)}, .ec_number = {}, .is_enzymatic = False},
                        New Reaction With {.bounds = {10, 10}, .ID = "B->C", .name = "B->C", .substrate = {New CompoundFactor("B", 1)}, .product = {New CompoundFactor("C", 1)}, .ec_number = {"1.-"}, .is_enzymatic = True},
                        New Reaction With {.bounds = {10, 10}, .ID = "A->A", .name = "A->A", .substrate = {New CompoundFactor("A", 1, "Extracellular")}, .product = {New CompoundFactor("A", 1)}, .ec_number = {"3.1.-"}, .is_enzymatic = True},
                        New Reaction With {.bounds = {10, 10}, .ID = "C->C", .name = "C->C", .substrate = {New CompoundFactor("C", 1)}, .product = {New CompoundFactor("C", 1, "Extracellular")}, .ec_number = {"3.2.-"}, .is_enzymatic = True}
                    }
                },
                .enzymes = {
                    New Enzyme With {.ECNumber = "2.-", .geneID = "gene3", .catalysis = {New Catalysis With {.reaction = "B->C", .PH = 7.0, .temperature = 37}}},
                    New Enzyme With {.ECNumber = "3.1.-", .geneID = "gene1", .catalysis = {New Catalysis With {.reaction = "A->A", .PH = 7.0, .temperature = 37}}},
                    New Enzyme With {.ECNumber = "3.2.-", .geneID = "gene2", .catalysis = {New Catalysis With {.reaction = "C->C", .PH = 7.0, .temperature = 37}}}
                }
            }
        }

        Call vcellModeller.writeJSON(cell, "./cell1.json", indent:=True)
    End Sub
End Module
