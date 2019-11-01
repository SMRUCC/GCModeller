Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Metagenomics

Namespace PathwayProfile

    ''' <summary>
    ''' A profile matrix model
    ''' </summary>
    Public Class Profile

        ''' <summary>
        ''' 物种分类信息
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Column("taxonomy", GetType(BIOMTaxonomyParser))>
        Public Property Taxonomy As Taxonomy
        ''' <summary>
        ''' The profile matrix data row
        ''' 
        ''' (该分类下的所有的具有覆盖度结果的KEGG编号的列表和相对应的覆盖度值)
        ''' </summary>
        ''' <returns></returns>
        Public Property Profile As Dictionary(Of String, Double)
        ''' <summary>
        ''' 这个物种的相对百分比含量
        ''' </summary>
        ''' <returns></returns>
        Public Property pct As Double
        Public Property RankGroup As String

        Sub New()
        End Sub

        Sub New(tax As Taxonomy, profile As Dictionary(Of String, Double), pct#)
            Me.Taxonomy = tax
            Me.Profile = profile
            Me.pct = pct
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{(pct * 100).ToString("F2")}%] {Profile.Where(Function(p) p.Value > 0).ToDictionary.GetJson}"
        End Function
    End Class

End Namespace