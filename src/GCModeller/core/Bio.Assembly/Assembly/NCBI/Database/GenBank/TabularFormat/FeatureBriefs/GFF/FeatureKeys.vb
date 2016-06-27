Imports System.Text.RegularExpressions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports System.Text
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Module FeatureKeys

        Public Const tRNA As String = "tRNA"
        Public Const CDS As String = "CDS"
        Public Const exon As String = "exon"
        Public Const gene As String = "gene"
        Public Const tmRNA As String = "tmRNA"
        Public Const rRNA As String = "rRNA"
        Public Const region As String = "region"

        Public Enum Features As Integer
            UnDefine = -1
            CDS
            gene
            tRNA
            exon
            tmRNA
            rRNA
            region
        End Enum

        <Extension>
        Public Function [GetFeatureType](x As Feature) As Features
            If String.IsNullOrEmpty(x.Feature) OrElse
                Not FeatureKeys.FeaturesHash.ContainsKey(x.Feature) Then
                Return Features.UnDefine
            Else
                Return FeatureKeys.FeaturesHash(x.Feature)
            End If
        End Function

        <Extension>
        Public Function GetsAllFeatures(source As IEnumerable(Of Feature), type As Features) As Feature()
            Return LinqAPI.Exec(Of Feature) <= From x As Feature
                                               In source
                                               Where type = x.GetFeatureType
                                               Select x
        End Function

        <Extension>
        Public Function GetsAllFeatures(gff As GFF, type As Features) As Feature()
            Return gff.Features.GetsAllFeatures(type)
        End Function

        Public ReadOnly Property FeaturesHash As IReadOnlyDictionary(Of String, Features) =
            New Dictionary(Of String, Features) From {
 _
            {FeatureKeys.CDS, Features.CDS},
            {FeatureKeys.exon, Features.exon},
            {FeatureKeys.gene, Features.gene},
            {FeatureKeys.region, Features.region},
            {FeatureKeys.rRNA, Features.rRNA},
            {FeatureKeys.tmRNA, Features.tmRNA},
            {FeatureKeys.tRNA, Features.tRNA}
        }
    End Module
End Namespace