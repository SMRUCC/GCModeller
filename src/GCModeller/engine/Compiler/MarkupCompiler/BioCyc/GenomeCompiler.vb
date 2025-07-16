
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.File.FileSystem.FastaObjects
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Vector

Namespace MarkupCompiler.BioCyc

    Public Class GenomeCompiler

        ReadOnly biocyc As Workspace
        ReadOnly geneIndex As Dictionary(Of String, genes)
        ReadOnly proteinIndex As Dictionary(Of String, proteins)
        ReadOnly protSeq As Dictionary(Of String, ProteinSeq)
        ReadOnly geneSeq As Dictionary(Of String, GeneObject)

        Sub New(compiler As v2Compiler)
            biocyc = compiler.biocyc
            geneIndex = biocyc.genes.features.ToDictionary(Function(g) g.uniqueId)
            proteinIndex = biocyc.proteins.features.ToDictionary(Function(a) a.uniqueId)
            protSeq = biocyc.fastaSeq.protseq.ToDictionary(Function(a) a.UniqueId)
            geneSeq = biocyc.fastaSeq.DNAseq.ToDictionary(Function(a) a.UniqueId)
        End Sub

        Public Function CreateReplicon() As replicon
            Return New replicon With {
                .genomeName = biocyc.species.commonName,
                .isPlasmid = False,
                .operons = CreateOperons.ToArray
            }
        End Function

        Private Iterator Function CreateOperons() As IEnumerable(Of TranscriptUnit)
            For Each operon As transunits In biocyc.transunits.features
                Dim genes As gene() = GeneObjects(operon.components).ToArray

                If genes.IsNullOrEmpty Then
                    Continue For
                End If

                Yield New TranscriptUnit With {
                    .id = operon.uniqueId,
                    .name = operon.commonName,
                    .genes = genes,
                    .note = operon.comment
                }
            Next
        End Function

        Private Iterator Function GeneObjects(list As IEnumerable(Of String)) As IEnumerable(Of gene)
            Dim prot_vec As NumericVector
            Dim nucl_vec As NumericVector

            For Each id As String In list
                Dim data As genes = geneIndex.TryGetValue(id)

                If data Is Nothing Then
                    Continue For
                End If

                Dim prot = If(data.product Is Nothing, Nothing, proteinIndex.TryGetValue(data.product))
                Dim rna_type As RNATypes = RNATypes.micsRNA
                Dim gene_seq As GeneObject = geneSeq.TryGetValue(id)

                If Not gene_seq Is Nothing Then
                    nucl_vec = RNAComposition.FromNtSequence(gene_seq.SequenceData, id).CreateVector
                Else
                    nucl_vec = RNAComposition.Blank(id).CreateVector
                End If

                If prot IsNot Nothing Then
                    Dim seq = protSeq.TryGetValue(prot.uniqueId)

                    If Not seq Is Nothing Then
                        prot_vec = ProteinComposition.FromRefSeq(seq.SequenceData, prot.uniqueId).CreateVector
                    Else
                        prot_vec = ProteinComposition.Blank(prot.uniqueId).CreateVector
                    End If

                    rna_type = RNATypes.mRNA
                Else
                    prot_vec = Nothing
                End If

                Yield New gene With {
                    .locus_tag = data.uniqueId,
                    .product = data.product,
                    .left = data.left,
                    .right = data.right,
                    .strand = data.direction.ToString,
                    .type = rna_type,
                    .protein_id = prot?.uniqueId,
                    .amino_acid = prot_vec,
                    .nucleotide_base = nucl_vec
                }
            Next
        End Function
    End Class
End Namespace