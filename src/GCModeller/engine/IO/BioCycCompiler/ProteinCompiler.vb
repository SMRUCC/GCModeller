Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

''' <summary>
''' 由BioCyc数据库的 ``proteins.dat``/``protligandcplxes.dat`` 构建虚拟细胞模型之中的
''' 蛋白质以及蛋白质复合物对象
''' </summary>
Public Class ProteinCompiler

    ReadOnly biocyc As Workspace
    ReadOnly peptides As Index(Of String)

    Sub New(biocyc As Workspace)
        Me.biocyc = biocyc
        Me.peptides = biocyc.proteins.features _
            .Select(Function(a) a.uniqueId) _
            .Indexing
    End Sub

    Private Iterator Function proteinFeatures() As IEnumerable(Of String)
        For Each gene As genes In biocyc.genes.features
            For Each gene_id As String In gene.product.SafeQuery
                Yield gene_id
            Next
        Next

        For Each prot As proteins In biocyc.proteins.features
            Yield prot.uniqueId
        Next

        For Each cplx As protligandcplxes In biocyc.protligandcplxes.features
            Yield cplx.uniqueId
        Next
    End Function

    Public Iterator Function CreateProteins() As IEnumerable(Of protein)
        Dim pid As New Index(Of String)
        Dim translates As Index(Of String) = proteinFeatures.Distinct.Indexing

        For Each cplx As protligandcplxes In biocyc.protligandcplxes.features
            Dim model As New protein With {
                .ligand = cplx.components.Where(Function(id) Not id Like peptides).ToArray,
                .peptide_chains = cplx.components.Where(Function(id) id Like peptides).ToArray,
                .name = cplx.commonName,
                .note = cplx.comment,
                .protein_id = cplx.uniqueId
            }

            Yield model

            pid += cplx.uniqueId
        Next

        For Each prot As proteins In biocyc.proteins.features
            If prot.uniqueId Like pid Then
                Continue For
            End If

            Dim model As protein

            If Not prot.components.IsNullOrEmpty Then
                model = New protein With {
                    .name = prot.commonName,
                    .note = prot.comment,
                    .protein_id = prot.uniqueId,
                    .complexId = prot.uniqueId,
                    .ligand = prot.components.Where(Function(c) Not c Like translates).ToArray,
                    .peptide_chains = prot.components.Where(Function(c) c Like translates).ToArray
                }
            ElseIf Not prot.unmodified_form.StringEmpty(, True) Then
                model = New protein With {
                    .name = prot.commonName,
                    .note = prot.comment,
                    .protein_id = prot.uniqueId,
                    .peptide_chains = {prot.unmodified_form}
                }
            ElseIf Not prot.gene.StringEmpty(, True) Then
                model = New protein With {
                    .protein_id = prot.uniqueId,
                    .name = prot.commonName,
                    .note = prot.comment,
                    .peptide_chains = {prot.uniqueId}
                }
            Else
                ' this protein object is broken
                ' unsure its source 
                Call VBDebugger.warning($"Unsure how to processing the protein data: {prot.ToString}")

                model = New protein With {
                    .protein_id = prot.uniqueId,
                    .name = prot.commonName,
                    .note = prot.comment,
                    .peptide_chains = {prot.uniqueId}
                }
            End If

            Call applyMetadata(model, prot)

            Yield model
        Next
    End Function

    ''' <summary>
    ''' 将蛋白质的物理化学属性以及功能注释写入模型对象
    ''' </summary>
    ''' <param name="model"></param>
    ''' <param name="prot"></param>
    Private Shared Sub applyMetadata(model As protein, prot As proteins)
        model.cellular_location = prot.locations.SafeQuery.FirstOrDefault
        model.locations = prot.locations
        model.molecularWeight = getMolecularWeight(prot)
        model.pi = prot.pi
        model.goTerms = prot.goTerms
        model.catalyzes = prot.catalyzes
        model.evidence = prot.citations
    End Sub

    ''' <summary>
    ''' 依次尝试使用平均分子量、道尔顿分子量以及由序列计算得到的分子量
    ''' </summary>
    ''' <param name="prot"></param>
    ''' <returns></returns>
    Private Shared Function getMolecularWeight(prot As proteins) As Double
        If prot.molecularWeight > 0 Then
            Return prot.molecularWeight
        ElseIf prot.molecularWeightKD > 0 Then
            Return prot.molecularWeightKD * 1000
        ElseIf prot.molecularWeightSeq > 0 Then
            Return prot.molecularWeightSeq
        ElseIf prot.molecularWeightExp > 0 Then
            Return prot.molecularWeightExp
        Else
            Return 0
        End If
    End Function

End Class
