
Imports Microsoft.VisualBasic.Linq
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
                .RNAs = RNAObjects().ToArray,
                .operons = CreateOperons(.RNAs).ToArray
            }
        End Function

        Private Iterator Function CreateOperons(rnas As RNA()) As IEnumerable(Of TranscriptUnit)
            Dim rna_genes As Dictionary(Of String, RNA()) = rnas _
                .Where(Function(r) Not r.gene.StringEmpty(, True)) _
                .GroupBy(Function(r) r.gene) _
                .ToDictionary(Function(a)
                                  Return a.Key
                              End Function,
                              Function(a)
                                  Return a.ToArray
                              End Function)

            For Each operon As transunits In biocyc.transunits.features
                Dim genes As gene() = GeneObjects(operon.components, rna_genes).ToArray
                Dim sites = operon.components _
                    .Where(Function(id) Not geneIndex.ContainsKey(id)) _
                    .ToArray

                If genes.IsNullOrEmpty Then
                    Continue For
                End If

                Yield New TranscriptUnit With {
                    .id = operon.uniqueId,
                    .name = operon.commonName,
                    .genes = genes,
                    .note = operon.comment,
                    .sites = sites
                }
            Next
        End Function

        Public Iterator Function RNAObjects() As IEnumerable(Of RNA)
            For Each rna_mol As rnas In biocyc.rnas.features
                Dim type As RNATypes
                Dim value As String = Nothing

                Select Case rna_mol.types(0)
                    Case "Small-RNAs", "Regulatory-RNAs" : type = RNATypes.sRNAs
                    Case "16S-rRNAs", "23S-rRNAs", "5S-rRNAs"
                        type = RNATypes.ribosomalRNA
                        value = rna_mol.types(0).Split("-"c).First.ToLower
                    Case Else
                        If rna_mol.types.Any(Function(t) t.EndsWith("-tRNAs")) Then
                            If rna_mol.types.Any(Function(t) t.StartsWith("Charged")) Then
                                type = RNATypes.chargedtRNA
                            Else
                                type = RNATypes.tRNA
                            End If

                            value = rna_mol.types(0) _
                                .Replace("-tRNAs", "") _
                                .Replace("Charged", "") _
                                .ToLower
                        Else
                            type = RNATypes.micsRNA
                        End If
                End Select

                Yield New RNA With {
                    .gene = rna_mol.gene,
                    .id = rna_mol.uniqueId,
                    .note = rna_mol.comment,
                    .type = type,
                    .val = value
                }
            Next
        End Function

        Private Iterator Function GeneObjects(list As IEnumerable(Of String), rna_genes As Dictionary(Of String, RNA())) As IEnumerable(Of gene)
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

                If prot IsNot Nothing Then
                    Dim seq = protSeq.TryGetValue(prot.uniqueId)

                    If Not seq Is Nothing Then
                        prot_vec = ProteinComposition.FromRefSeq(seq.SequenceData, prot.uniqueId).CreateVector
                    Else
                        prot_vec = ProteinComposition.Blank(prot.uniqueId).CreateVector
                    End If

                    If Not gene_seq Is Nothing Then
                        nucl_vec = RNAComposition.FromNtSequence(gene_seq.SequenceData, id).CreateVector
                    Else
                        nucl_vec = RNAComposition.Blank(id).CreateVector
                    End If

                    rna_type = RNATypes.mRNA
                Else
                    Dim rna As RNA = rna_genes.TryGetValue(id).DefaultFirst

                    rna_type = If(rna Is Nothing, RNATypes.micsRNA, rna.type)
                    prot_vec = Nothing

                    If Not gene_seq Is Nothing Then
                        nucl_vec = RNAComposition.FromNtSequence(gene_seq.SequenceData, If(rna Is Nothing, id, rna.id)).CreateVector
                    Else
                        nucl_vec = RNAComposition.Blank(If(rna Is Nothing, id, rna.id)).CreateVector
                    End If
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