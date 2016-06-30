Imports System.Xml.Serialization
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Infernal.cmsearch

    Public Class SearchSites : Inherits Infernal.STDOUT
        Implements IRfamHits

        Public Property Queries As Query()

        Public Function GetDataFrame() As RfamHit() Implements IRfamHits.GetDataFrame
            Return LinqAPI.Exec(Of RfamHit) <= From query As Query
                                               In Queries
                                               Where Not query Is Nothing
                                               Select data = query.GetDataFrame
        End Function
    End Class

    Public Interface IRfamHits
        Function GetDataFrame() As RfamHit()
    End Interface

    Public Class Query : Implements IRfamHits

        <XmlAttribute> Public Property Query As String
        Public Property Clen As Integer
        Public Property Accession As String
        Public Property hits As Hit()
        ''' <summary>
        ''' ------ inclusion threshold ------
        ''' </summary>
        ''' <returns></returns>
        Public Property Uncertain As Hit()
        Public Property describ As String

        Public Function GetDataFrame() As RfamHit() Implements IRfamHits.GetDataFrame
            Return hits.ToList(Function(x) New RfamHit(x, Me)) + From x As Hit
                                                                 In Uncertain
                                                                 Select New RfamHit(x, Me)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class RfamHit : Inherits Hit

        Public Property Query As String
        Public Property Clen As Integer
        Public Property Accession As String
        Public Property QueryDef As String

        Sub New(hit As Hit, query As Query)
            MyBase.bias = hit.bias
            MyBase.description = hit.description
            MyBase.end = hit.end
            MyBase.Evalue = hit.Evalue
            MyBase.gc = hit.gc
            MyBase.mdl = hit.mdl
            MyBase.rank = hit.rank
            MyBase.score = hit.score
            MyBase.sequence = hit.sequence
            MyBase.start = hit.start
            MyBase.trunc = hit.trunc
            MyBase.strand = hit.strand

            Me.QueryDef = query.describ
            Me.Query = query.Query
            Me.Clen = query.Clen
            Me.Accession = query.Accession
        End Sub
    End Class

    Public Class Hit : Inherits IHit

        <XmlAttribute> Public Property rank As String
        <XmlAttribute> <Column("E-value")>
        Public Property Evalue As Double
        <XmlAttribute> Public Property score As Double
        <XmlAttribute> Public Property bias As Double
        <XmlAttribute> Public Property sequence As String
        <XmlAttribute> Public Property mdl As String
        <XmlAttribute> Public Property trunc As String
        <XmlAttribute> Public Property gc As Double
        <XmlAttribute> Public Property description As String

        Public Overrides Function ToString() As String
            Return MyClass.GetJson
        End Function
    End Class
End Namespace