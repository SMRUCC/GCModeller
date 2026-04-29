Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Metagenomics

Namespace ContextModel

    ''' <summary>
    ''' 参考基因组信息结构，用于计算邻域保守性和系统发育距离
    ''' </summary>
    Public Class GenomeInfo

        ''' <summary>
        ''' 参考基因组的唯一标识符
        ''' </summary>
        Public Property GenomeID As String
        ''' <summary>
        ''' 参考基因组所属的门
        ''' 用于计算基因在该门中的存在概率 pik
        ''' </summary>
        Public Property Phylum As String
        ''' <summary>
        ''' 参考基因组中的基因总数 Nk
        ''' </summary>
        Public Property GeneCount As Integer
        ''' <summary>
        ''' 基因在基因组中的位置索引字典，键为基因ID，值为位置索引
        ''' 用于计算邻域保守性中的 dk(ij) (两基因间的基因数量)
        ''' </summary>
        ''' <remarks>
        ''' 基因ID -> 位置索引
        ''' </remarks>
        Public Property GenePositions As Dictionary(Of String, Integer)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function FromGenBank(gb As GBFF.File) As GenomeInfo
            Dim genes As Feature() = gb.EnumerateGeneFeatures.ToArray
            Dim tax As Taxonomy = gb.Source.GetTaxonomy
            Dim geneLocs As New Dictionary(Of String, Integer)
            Dim i As Integer = 1
            Dim accId As String = gb.Accession.AccessionId

            Static gene_key As String = FeatureQualifiers.locus_tag.ToString

            For Each gene As Feature In genes
                Dim id As String = If(gene.Query(FeatureQualifiers.locus_tag), $"{accId}{i}")
                Dim loc As NucleotideLocation = gene.Location.ContiguousRegion
                Dim left As Integer = loc.left

                geneLocs(id) = left
            Next

            Return New GenomeInfo With {
                .GeneCount = genes.Length,
                .GenePositions = geneLocs,
                .GenomeID = gb.Source.SpeciesName,
                .Phylum = tax.phylum
            }
        End Function

    End Class
End Namespace