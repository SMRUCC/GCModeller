Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Serialization.JSON
Imports RDotNet.Extensions.VisualBasic.API
Imports SMRUCC.genomics.Analysis.Metagenome
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.Model.Network.KEGG
Imports Numeric = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

Namespace PathwayProfile

    Public Class Profile

        ''' <summary>
        ''' 物种分类信息
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Column("taxonomy", GetType(BIOMTaxonomyParser))>
        Public Property Taxonomy As Taxonomy
        ''' <summary>
        ''' 该分类下的所有的具有覆盖度结果的KEGG编号的列表和相对应的覆盖度值
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
            Return Taxonomy.family & ": " & Profile.GetJson
        End Function
    End Class

End Namespace