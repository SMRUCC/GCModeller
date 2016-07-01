Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.DatabaseServices.Regprecise.WebServices
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

Namespace Regprecise

    Public Class BacteriaGenome

        ''' <summary>
        ''' {GenomeName, Url}
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property BacteriaGenome As JSONLDM.genome
        <XmlElement> Public Property Regulons As Regulon

        ''' <summary>
        ''' 这个基因组里面的Regulon的数目
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property NumOfRegulons As Integer
            Get
                If Regulons Is Nothing OrElse
                    Regulons.Regulators.IsNullOrEmpty Then
                    Return 0
                Else
                    Return Regulons.Regulators.Length
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return BacteriaGenome.ToString
        End Function

        ''' <summary>
        ''' Listing all TF type regulators in this genome.
        ''' </summary>
        ''' <returns></returns>
        Public Function ListRegulators() As String()
            Dim list As String() = (From x As Regulator In Regulons.Regulators
                                    Where x.Type = Regulator.Types.TF
                                    Select x.LocusTag.Key
                                    Distinct).ToArray
            Return list
        End Function

        ''' <summary>
        ''' Listing all regulated genes in this genome.
        ''' </summary>
        ''' <returns></returns>
        Public Function ListRegulatedGenes() As String()
            Dim list As List(Of String) = (From x As Regulator
                                           In Regulons.Regulators
                                           Select x.lstOperon.ToArray(Function(o) o.Members.ToArray(Function(g) g.LocusId))).ToArray.MatrixToList.MatrixToList
            Dim dlist As String() = list.Distinct.ToArray
            Return dlist
        End Function

        ''' <summary>
        ''' 创建用于从KEGG数据库之中下载蛋白质序列的查询数据集合
        ''' </summary>
        ''' <returns></returns>
        Public Function CreateKEGGQuery() As KEGG.WebServices.QuerySource
            Dim lstId As String() = ListRegulatedGenes.Join(ListRegulators).ToArray

            Return New KEGG.WebServices.QuerySource With {
                .genome = BacteriaGenome.name,
                .locusId = lstId.Distinct.ToArray
            }
        End Function
    End Class
End Namespace