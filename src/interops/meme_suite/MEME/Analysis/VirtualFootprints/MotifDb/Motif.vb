Imports System.Xml.Serialization
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MEME.HTML
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace Analysis.GenomeMotifFootPrints.MotifDb

    Public Class Motif : Implements sIdEnumerable
        Implements IKeyValuePairObject(Of String, String())

        ''' <summary>
        ''' [Motif].[MotifId]，当前的这个属性值可以唯一的标识一个Motif对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property MotifId As String Implements sIdEnumerable.Identifier, IKeyValuePairObject(Of String, String()).Identifier
        <XmlAttribute> Public Property Evalue As Double
        <XmlAttribute> Public Property Width As Integer
        <XmlAttribute> Public Property LogLikelihoodRatio As Double
        <XmlAttribute> Public Property InformationContent As Double
        <XmlAttribute> Public Property RelativeEntropy As Double
        Public Property Family As String

        ''' <summary>
        ''' 这个Motif的序列特征，使用一个正则表达式来表示
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("Signature")> <XmlElement>
        Public Property Signature As String
        Public Property MatchedSites As SiteInfo()
        <XmlAttribute> Public Property RegpreciseRegulators As String() Implements IKeyValuePairObject(Of String, String()).Value

        Public Overrides Function ToString() As String
            Return MotifId
        End Function

        Public Shared Function CopyFrom(data As MEME.HTML.Motif, ObjectId As String) As Motif
            Dim Motif As Motif = New Motif With {
                    .Evalue = data.Evalue,
                    .InformationContent = data.InformationContent, .LogLikelihoodRatio = data.LogLikelihoodRatio,
                    .MatchedSites = data.MatchedSites, .MotifId = data.MotifId(ObjectId),
                    .RelativeEntropy = data.RelativeEntropy,
                    .Signature = data.RegularExpression,
                    .Width = data.Width}
            Return Motif
        End Function
    End Class
End Namespace