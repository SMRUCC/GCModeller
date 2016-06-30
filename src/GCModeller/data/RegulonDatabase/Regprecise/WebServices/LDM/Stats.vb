Imports System.ComponentModel
Imports System.Xml.Serialization

Namespace Regprecise.WebServices.JSONLDM

    ''' <summary>
    ''' http://regprecise.lbl.gov/Services/rest/genomeStats
    ''' </summary>
    <Description("http://regprecise.lbl.gov/Services/rest/genomeStats")> Public Class genomeStat
        ''' <summary>
        ''' genome identifier
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property genomeId As Integer
        ''' <summary>
        ''' genome name
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' total number of RNA-controlled regulons reconstructed in genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property rnaRegulonCount As Integer
        ''' <summary>
        ''' total number of RNA regulatory sites in genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property rnaSiteCount As Integer
        ''' <summary>
        ''' NCBI taxonomy id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property taxonomyId As Integer
        ''' <summary>
        ''' total number of TF-controlled regulons reconstructed in genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property tfRegulonCount As Integer
        ''' <summary>
        ''' total number of TF binding sites in genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property tfSiteCount As Integer

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class

    ''' <summary>
    ''' /searchExtRegulons?taxonomyId={taxonomyId}&amp;locusTags={locusTags}
    ''' </summary>
    Public Class searchExtRegulons

        ''' <summary>
        ''' identifier Of regulon
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulonId As Integer
        ''' <summary>
        ''' name Of genome
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property genomeName As String
        ''' <summary>
        ''' the name Of regulator
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property regulatorName As String
        ''' <summary>
        ''' found Object type 
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property foundObjType As String
        ''' <summary>
        ''' found Object name (Or locusTag)
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property foundObjName As String
    End Class

    ''' <summary>
    ''' /regulogCollectionStats?collectionType={type}
    ''' </summary>
    Public Class regulogCollectionStat
        ''' <summary>
        ''' name of collection class
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property className As String
        ''' <summary>
        ''' identifier of collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property collectionId As Integer
        ''' <summary>
        ''' type of collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property collectionType As String
        ''' <summary>
        ''' collection name
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' number of RNA families in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property rnaCount As Integer
        ''' <summary>
        ''' number of RNA-controlled regulogs in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property rnaRegulogCount As Integer
        ''' <summary>
        ''' number of RNA regulatory sites in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property rnaSiteCount As Integer
        ''' <summary>
        ''' number of different transcription factors in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property tfCount As Integer
        ''' <summary>
        ''' number of TF-controlled regulogs in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property tfRegulogCount As Integer
        ''' <summary>
        ''' number of TF binding sites in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property tfSiteCount As Integer
        ''' <summary>
        ''' total number of genomes that have at least one regulon in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property totalGenomeCount As Integer
        ''' <summary>
        ''' total number of regulogs in collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property totalRegulogCount As Integer
    End Class

    ''' <summary>
    ''' http://regprecise.lbl.gov/Services/rest/genomes
    ''' </summary>
    ''' 
    <Description("http://regprecise.lbl.gov/Services/rest/genomes")>
    <XmlType("bacterial.genome", [Namespace]:="http://regprecise.lbl.gov/Services/rest/genomes")> Public Class genome
        ''' <summary>
        ''' genome identifier
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property genomeId As Integer
        ''' <summary>
        ''' genome name
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' NCBI taxonomy id
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property taxonomyId As Integer

        Public Overrides Function ToString() As String
            Return name
        End Function
    End Class

    ''' <summary>
    ''' /regulogCollections?collectionType={type}
    ''' </summary>
    Public Class regulogCollection
        ''' <summary>
        ''' type of regulog collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property collectionType As String
        ''' <summary>
        ''' identifier of collection
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property collectionId As String
        ''' <summary>
        ''' collection name
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' name of collection class 
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property className As String
    End Class
End Namespace