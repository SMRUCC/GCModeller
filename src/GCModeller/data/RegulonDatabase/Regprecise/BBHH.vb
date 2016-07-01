Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Regprecise

    ''' <summary>
    ''' bbh mappings of the regulators between the RegPrecise database and annotated genome.
    ''' </summary>
    Public Class bbhMappings : Implements IQueryHits

        Public Property Identities As Double Implements IQueryHits.identities
        Public Property Positive As Double
        ''' <summary>
        ''' 在Regprecise数据库之中的进行注释的源
        ''' </summary>
        ''' <returns></returns>
        Public Property query_name As String Implements IBlastHit.locusId
        ''' <summary>
        ''' 在所需要进行注释的基因组之中的蛋白质基因号
        ''' </summary>
        ''' <returns></returns>
        Public Property hit_name As String Implements IBlastHit.Address
        Public Property vimssId As Integer
        Public Property Family As String
        Public Property definition As String

        Public Overrides Function ToString() As String
            Return $"{query_name}  --> {hit_name}"
        End Function

        Public Shared Function GetQueryMaps(source As IEnumerable(Of bbhMappings), hit As String) As bbhMappings
            Dim LQuery = (From x As bbhMappings In source
                          Let hitId As String = x.query_name.Split(":"c).Last
                          Where String.Equals(hit, hitId)
                          Select x).FirstOrDefault
            Return LQuery
        End Function
    End Class

    ''' <summary>
    ''' annotated TF hits on Family
    ''' </summary>
    Public Class FamilyHit
        Public Property QueryName As String
        Public Property Family As String
        Public Property HitName As String
    End Class
End Namespace