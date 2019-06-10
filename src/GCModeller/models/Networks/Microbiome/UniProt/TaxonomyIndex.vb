
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.WebServices

Public Class TaxonomyIndex

    <XmlElement(NameOf(ref))>
    Public Property ref As Summary()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="uniprotRef$"><see cref="TaxonomyRepository"/></param>
    ''' <param name="maps$"><see cref="MapRepository"/></param>
    ''' <returns></returns>
    Public Shared Function Summary(uniprotRef$, maps$) As TaxonomyIndex
        Return New TaxonomyIndex With {
            .ref = TaxonomyIndexExtensions _
                .IteratesModels(uniprotRef) _
                .Summary(ref:=maps.LoadXml(Of MapRepository)) _
                .ToArray
        }
    End Function
End Class

''' <summary>
''' 用于加速<see cref="PathwayProfile"/>计算的已经预先计算好的基因组摘要数据
''' </summary>
Public Class Summary

    Public Property ncbi_taxon_id As String
    Public Property scientificName As String
    Public Property lineageGroup As NamedValue()
    Public Property Maps As String()

    Public Overrides Function ToString() As String
        Return scientificName
    End Function
End Class
