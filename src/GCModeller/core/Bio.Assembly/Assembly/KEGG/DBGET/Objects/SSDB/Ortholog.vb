Imports System.Data.Linq.Mapping
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.KEGG.DBGET.bGetObject.SSDB

    ''' <summary>
    ''' http://www.kegg.jp/ssdb-bin/ssdb_best?org_gene=sp:locus_tag
    ''' </summary>
    Public Class Ortholog

        Public Property sp As String
        ''' <summary>
        ''' query_name
        ''' </summary>
        ''' <returns></returns>
        Public Property LocusId As String
        <Column(Name:="KEGG_Entry")> Public Property hit_name As String
        Public Property Definition As String
        Public Property KO As String
        Public Property query_length As Integer
        <Column(Name:="Length")> Public Property len As Integer
        <Column(Name:="SW-score")> Public Property SW As Double
        <Column(Name:="Margin")> Public Property margin As Integer
        Public Property bits As Double
        Public Property identity As Double
        Public Property overlap As Double
        <Column(Name:="best(all)")> Public Property bestAll As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function CreateObjects(result As SSDB.OrthologREST) As Ortholog()
            Return result.Orthologs.ToArray(Function(x) __createObject(result, x))
        End Function

        Private Shared Function __createObject(result As SSDB.OrthologREST, hit As SShit) As Ortholog
            Return New Ortholog With {
                .bestAll = $"{hit.Best.Key} {hit.Best.Value}",
                .bits = Val(hit.Bits),
                .Definition = hit.Entry.Description,
                .hit_name = hit.Entry.LocusId,
                .LocusId = result.KEGG_ID,
                .identity = Val(hit.Identity),
                .KO = hit.KO.Key,
                .len = Val(hit.Length),
                .margin = Val(hit.Margin),
                .overlap = Val(hit.Overlap),
                .sp = hit.Entry.SpeciesId,
                .SW = Val(hit.SWScore),
                .query_length = Strings.Len(result.Sequence)
            }
        End Function
    End Class
End Namespace