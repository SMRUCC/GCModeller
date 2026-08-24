#Region "Microsoft.VisualBasic::1e62b9a9a3bfc44d62dba879b867222b, analysis\OperonMapper\OperonPrediction\GenomeInfo.vb"

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

    '   Total Lines: 66
    '    Code Lines: 36 (54.55%)
    ' Comment Lines: 20 (30.30%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (15.15%)
    '     File Size: 2.46 KB


    '     Class GenomeInfo
    ' 
    '         Properties: GeneCount, GenePositions, GenomeID, Phylum
    ' 
    '         Function: FromGenBank, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
