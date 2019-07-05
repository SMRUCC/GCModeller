Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace Pipeline.COG

    ''' <summary>
    ''' Protein cog category using myva database.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MyvaCOG : Implements INamedValue, IFeatureDigest, IQueryHits, ICOGCatalog

        <Column("query_name")>
        Public Property QueryName As String Implements INamedValue.Key, IBlastHit.locusId
        Public Property Length As Integer
        <Column("cog_myva")> Public Property MyvaCOG As String

        ''' <summary>
        ''' COG category
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("COG_category")> Public Property Category As String Implements ICOGCatalog.Catalog
        <Column("COG")> Public Property COG As String Implements IFeatureDigest.Feature, IBlastHit.Address, ICOGCatalog.COG
        <Column("description")> Public Property Description As String

        Public Property Evalue As Double
        Public Property Identities As Double Implements IQueryHits.identities
        Public Property QueryLength As Integer
        Public Property LengthQuery As Integer

        ''' <summary>
        ''' 额外的附件数据
        ''' </summary>
        ''' <returns></returns>
        Public Property DataAsset As Dictionary(Of String, String)

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", COG, QueryName)
        End Function

        ''' <summary>
        ''' ```
        ''' query   -> <see cref="BestHit.QueryName"/>
        ''' myvaCOG -> <see cref="BestHit.HitName"/>
        ''' ```
        ''' </summary>
        ''' <param name="besthit"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function CreateObject(besthit As BestHit) As MyvaCOG
            Return New MyvaCOG With {
                .QueryName = besthit.QueryName,
                .MyvaCOG = besthit.HitName,
                .Length = besthit.query_length,
                .Evalue = besthit.evalue,
                .Identities = besthit.identities,
                .LengthQuery = besthit.length_query,
                .QueryLength = besthit.query_length
            }
        End Function
    End Class
End Namespace