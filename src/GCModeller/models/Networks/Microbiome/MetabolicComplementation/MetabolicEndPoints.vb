Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Model.Network.Microbiome

''' <summary>
''' 一个微生物物种的代谢网络端点数据模型
''' </summary>
Public Class MetabolicEndPoints

    Public Property taxonomy As String

    Public Property uptakes As String()
    Public Property secrete As String()

    Public Overrides Function ToString() As String
        Return taxonomy
    End Function

End Class

''' <summary>
''' 宏基因组测序分析所使用到的微生物物种间的代谢网络端点模型的集合
''' </summary>
Public Class MetabolicEndPointProfiles : Inherits ListOf(Of MetabolicEndPoints)

    <XmlElement>
    Public Property taxonomy As MetabolicEndPoints()

    Protected Overrides Function getSize() As Integer
        Return If(taxonomy Is Nothing, 0, taxonomy.Length)
    End Function

    Protected Overrides Function getCollection() As IEnumerable(Of MetabolicEndPoints)
        Return taxonomy.AsEnumerable
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function CreateProfiles(taxonomy As IEnumerable(Of TaxonomyRef), reactions As ReactionRepository) As MetabolicEndPointProfiles
        Return New MetabolicEndPointProfiles With {
            .taxonomy = taxonomy _
                .SafeQuery _
                .Select(Function(tax)
                            Return tax.DoMetabolicEndPointsAnalysis(reactions)
                        End Function) _
                .Where(Function(tax)
                           Return Not (tax.uptakes.IsNullOrEmpty AndAlso tax.secrete.IsNullOrEmpty)
                       End Function) _
                .ToArray
        }
    End Function
End Class