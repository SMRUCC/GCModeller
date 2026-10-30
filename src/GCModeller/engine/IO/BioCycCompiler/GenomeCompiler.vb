Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.File.FileSystem.FastaObjects
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Model.Cellular.Vector

''' <summary>
''' 由BioCyc数据库的 ``genes.dat``/``transunits.dat``/``rnas.dat`` 构建虚拟细胞模型之中的
''' 基因组结构：复制子(染色体与质粒)、转录单元(操纵子)以及基因对象。
''' </summary>
Public Class GenomeCompiler

    ReadOnly compiler As v2Compiler
    ReadOnly biocyc As Workspace
    ReadOnly geneIndex As Dictionary(Of String, genes)
    ReadOnly proteinIndex As Dictionary(Of String, proteins)
    ReadOnly protSeq As Dictionary(Of String, ProteinSeq)
    ReadOnly geneSeq As Dictionary(Of String, GeneObject)

    Sub New(compiler As v2Compiler)
        Me.compiler = compiler
        Me.biocyc = compiler.biocyc
        Me.geneIndex = biocyc.genes.features.ToDictionary(Function(g) g.uniqueId)
        Me.proteinIndex = biocyc.proteins.features.ToDictionary(Function(a) a.uniqueId)
        Me.protSeq = biocyc.fastaSeq.protseq.ToDictionary(Function(a) a.UniqueId)
        Me.geneSeq = biocyc.fastaSeq.DNAseq.ToDictionary(Function(a) a.UniqueId)
    End Sub

    ''' <summary>
    ''' 质粒复制子的编号集合，由 ``classes.dat`` 之中的 Replicon 类型推断得到
    ''' </summary>
    ''' <returns></returns>
    Private Function plasmidRepliconIds() As Index(Of String)
        Return biocyc.classes.features _
            .Where(Function(c)
                       Return c.types.SafeQuery.Any(Function(t) t.ToLowerInvariant.Contains("replicon"))
                   End Function) _
            .Select(Function(c) c.uniqueId) _
            .Indexing
    End Function

    ''' <summary>
    ''' 构建基因组之中的全部复制子(染色体以及质粒)
    ''' </summary>
    ''' <returns></returns>
    Public Function CreateReplicons() As replicon()
        Dim plasmidIds As Index(Of String) = plasmidRepliconIds()
        Dim rnas As RNA() = RNAObjects().ToArray
        Dim operons As TranscriptUnit() = CreateOperons(rnas).ToArray
        Dim geneReplicon As Dictionary(Of String, String) = geneIndex.Values _
            .Where(Function(g) Not g.uniqueId.StringEmpty(, True)) _
            .GroupBy(Function(g) g.uniqueId) _
            .ToDictionary(Function(g) g.Key,
                          Function(g)
                              Return repliconName(g.First, plasmidIds)
                          End Function)

        Dim groups = biocyc.genes.features _
            .GroupBy(Function(g) geneReplicon.TryGetValue(g.uniqueId)) _
            .ToArray

        Return groups _
            .Select(Function(group)
                        Return createReplicon(
                            name:=group.Key,
                            genes:=group.ToArray,
                            operons:=operons,
                            rnas:=rnas)
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' 构建基因组之中的主复制子(染色体)
    ''' </summary>
    ''' <returns></returns>
    Public Function CreateReplicon() As replicon
        Return CreateReplicons() _
            .OrderBy(Function(r) If(r.isPlasmid, 1, 0)) _
            .FirstOrDefault
    End Function

    Private Function repliconName(gene As genes, plasmidIds As Index(Of String)) As String
        Dim plasmid As String = gene.componentOf _
            .SafeQuery _
            .Where(Function(id) id Like plasmidIds) _
            .FirstOrDefault

        If Not plasmid.StringEmpty(, True) Then
            Return plasmid
        End If

        Return If(biocyc.species.Genome, biocyc.species.commonName)
    End Function

    Private Function createReplicon(name As String, genes As genes(), operons As TranscriptUnit(), rnas As RNA()) As replicon
        Dim chromosome As String = If(biocyc.species.Genome, biocyc.species.commonName)
        Dim isPlasmid As Boolean = Not name.StringEmpty(, True) AndAlso name <> chromosome
        Dim geneIds As Index(Of String) = genes _
            .Select(Function(g) g.uniqueId) _
            .Indexing
        Dim repliconOperons As TranscriptUnit() = operons _
            .Where(Function(o)
                       Return o.genes.SafeQuery.Any(Function(g) g.locus_tag Like geneIds)
                   End Function) _
            .ToArray
        Dim repliconRNAs As RNA() = rnas _
            .Where(Function(r) r.gene Like geneIds) _
            .ToArray
        Dim left As Long = genes.Select(Function(g) g.left).DefaultIfEmpty(0).Min
        Dim right As Long = genes.Select(Function(g) g.right).DefaultIfEmpty(0).Max

        Return New replicon With {
            .genomeName = name,
            .isPlasmid = isPlasmid,
            .topology = "circular",
            .length = If(right > left, right - left + 1, 0),
            .gcContent = gcContent(geneIds),
            .copyNumber = If(isPlasmid, 1, 1),
            .RNAs = repliconRNAs,
            .operons = repliconOperons
        }
    End Function

    ''' <summary>
    ''' 由基因序列计算复制子的GC含量
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <returns></returns>
    Private Function gcContent(genes As Index(Of String)) As Double
        Dim gc As Long = 0
        Dim total As Long = 0

        For Each gene As GeneObject In geneSeq.Values
            If Not gene.UniqueId Like genes Then
                Continue For
            End If

            Dim seq As String = gene.SequenceData

            If seq.StringEmpty(, True) Then
                Continue For
            End If

            For Each ch As Char In seq.ToUpperInvariant
                Select Case ch
                    Case "G"c, "C"c
                        gc += 1
                        total += 1
                    Case "A"c, "T"c, "U"c, "N"c
                        total += 1
                End Select
            Next
        Next

        If total = 0 Then
            Return 0
        Else
            Return gc / CDbl(total)
        End If
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
                .sites = sites,
                .direction = operon.direction,
                .left = CInt(operon.left),
                .right = CInt(operon.right),
                .regulatedBy = operon.regulatedBy
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
                Case "rRNAs"
                    type = RNATypes.ribosomalRNA
                    value = rna_mol.commonName.Split(" "c).First.ToLower
                Case Else
                    If rna_mol.types.Any(Function(t) t.EndsWith("-tRNAs")) OrElse
                        rna_mol.types.Any(Function(t) t = "Initiation-tRNAmet") OrElse
                        rna_mol.types.Any(Function(t) t.StartsWith("tRNA-")) Then

                        If rna_mol.types.Any(Function(t) t.StartsWith("Charged")) Then
                            ' skip of the charged t-RNA
                            Continue For
                        Else
                            type = RNATypes.tRNA
                        End If

                        value = rna_mol.types(0) _
                            .Replace("-tRNAs", "") _
                            .Replace("Charged", "") _
                            .Replace("tRNA-", "") _
                            .Replace("Initiation-tRNA", "") _
                            .ToLower _
                            .Trim("-"c)

                        If value = "" Then
                            Call VBDebugger.EchoLine($"found an invalid t-RNA object: {rna_mol.uniqueId}")
                            type = RNATypes.micsRNA
                        End If
                    Else
                        type = RNATypes.micsRNA
                    End If
            End Select

            Yield New RNA With {
                .gene = rna_mol.gene,
                .id = rna_mol.uniqueId,
                .note = rna_mol.comment,
                .type = type,
                .val = value,
                .components = rna_mol.components.SafeQuery.ToArray,
                .componentOf = rna_mol.componentOf,
                .endPosition = CInt(rna_mol.right),
                .dbLinks = rna_mol.dbLinks
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

            Dim prot As proteins = If(data.product Is Nothing, Nothing, getTranslation(data.product))
            Dim rna_type As RNATypes = RNATypes.micsRNA
            Dim gene_seq As GeneObject = geneSeq.TryGetValue(id)
            Dim rna As RNA = rna_genes.TryGetValue(id).DefaultFirst

            rna_type = If(rna Is Nothing, RNATypes.micsRNA, rna.type)
            prot_vec = Nothing

            If Not gene_seq Is Nothing Then
                nucl_vec = RNAComposition.FromNtSequence(gene_seq.SequenceData, If(rna Is Nothing, id, rna.id)).CreateVector
            Else
                nucl_vec = RNAComposition.Blank(If(rna Is Nothing, id, rna.id)).CreateVector
            End If

            If prot IsNot Nothing Then
                Dim seq = protSeq.TryGetValue(prot.uniqueId)

                If Not seq Is Nothing Then
                    prot_vec = ProteinComposition.FromRefSeq(seq.SequenceData, prot.uniqueId).CreateVector
                Else
                    prot_vec = ProteinComposition.Blank(prot.uniqueId).CreateVector
                End If

                rna_type = RNATypes.mRNA
            End If

            Yield New gene With {
                .locus_tag = data.uniqueId,
                .name = data.commonName,
                .synonyms = data.synonyms,
                .accession = data.accession1,
                .dbLinks = data.db_xrefs,
                .goTerms = data.goTerms,
                .centisomePosition = data.centisomePosition,
                .componentOf = data.componentOf.SafeQuery.FirstOrDefault,
                .characterization = data.characterization,
                .evidence = compiler.evidence.GetEvidenceIds(data.citations),
                .product = If(prot Is Nothing, data.product.JoinBy("; "), prot.uniqueId),
                .left = CInt(data.left),
                .right = CInt(data.right),
                .strand = data.direction.ToString,
                .type = rna_type,
                .protein_id = If(rna_type = RNATypes.mRNA, data.product, Nothing),
                .amino_acid = prot_vec,
                .nucleotide_base = nucl_vec
            }
        Next
    End Function

    Private Function getTranslation(products As IEnumerable(Of String)) As proteins
        For Each id As String In products
            If proteinIndex.ContainsKey(id) AndAlso protSeq.ContainsKey(proteinIndex(id).uniqueId) Then
                Return proteinIndex(id)
            End If
        Next

        Return Nothing
    End Function
End Class
