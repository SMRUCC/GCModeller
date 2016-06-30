Imports System.Xml.Serialization
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.FootprintTraceAPI
Imports LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MEME
Imports LANS.SystemsBiology.Assembly.DOOR
Imports LANS.SystemsBiology.DatabaseServices.Regprecise
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Analysis.MotifScans

    ''' <summary>
    ''' 通过MAST程序扫描RegPrecise数据库里面的Motif Fasta所得到的结果
    ''' </summary>
    Public Class MotifSiteHit
        Implements I_PolymerSequenceModel

        ''' <summary>
        ''' 这个属性是记录了这个motif位点的来源信息，这个是用于Csv文档的，Xml文档会被忽略掉
        ''' </summary>
        ''' <returns></returns>
        <XmlIgnore> Public Property source As String()
        <XmlAttribute> Public Property gStart As Integer
        <XmlAttribute> Public Property gStop As Integer
        <XmlAttribute> Public Property Pvalue As Double
        <XmlAttribute> Public Property Family As String
        <XmlAttribute> Public Property Trace As String
        <XmlAttribute> Public ReadOnly Property Length As Integer
        ''' <summary>
        ''' 在RegPrecise数据库之中所匹配上的调控位点的trace信息：locus_tag:position
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property RegPrecise As String
        <XmlAttribute> Public Property Regulators As String()
        <XmlAttribute> Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData

        Sub New()
        End Sub

        Public Function Copy() As MotifSiteHit
            Return DirectCast(Me.MemberwiseClone, MotifSiteHit)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class MatchResult : Inherits ClassObject
        Implements sIdEnumerable

        ''' <summary>
        ''' 来源的文件名
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property MEME As String Implements sIdEnumerable.Identifier
        <XmlElement> Public Property Matches As MotifHits()

        Public Function ToFootprints() As IEnumerable(Of GenomeMotifFootPrints.PredictedRegulationFootprint)
            Return Matches.ToArray(Function(x) x.GetFootprints).MatrixAsIterator
        End Function
    End Class

    ''' <summary>
    ''' MEME结果之中的某一个Motif
    ''' </summary>
    Public Class MotifHits : Implements sIdEnumerable
        ''' <summary>
        ''' 当前的这个MEME Motif的来源位点的集合
        ''' </summary>
        ''' <returns></returns>
        Public Property MEME As LDM.Site()
        ''' <summary>
        ''' File::MotifId
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Trace As String Implements sIdEnumerable.Identifier
        <XmlAttribute> Public Property Evalue As Double
        Public Property MAST As MotifSiteHit()

        Public Function GetFootprints() As GenomeMotifFootPrints.PredictedRegulationFootprint()
            Dim LQuery = (From x As LDM.Site In MEME
                          Let footprints = (From site As MotifSiteHit
                                            In MAST
                                            Select __toFootprints(x, site))
                          Select footprints).MatrixAsIterator.MatrixToVector
            Return LQuery
        End Function

        Private Function __toFootprints(g As LDM.Site, site As MotifSiteHit) As GenomeMotifFootPrints.PredictedRegulationFootprint()
            Return site.Regulators.ToArray(
                Function(reg) New GenomeMotifFootPrints.PredictedRegulationFootprint With {
                    .MotifFamily = site.Family,
                    .MotifId = site.Trace,
                    .MotifTrace = Trace,
                    .ORF = g.Name,
                    .RegulatorTrace = reg,
                    .Sequence = g.Site,
                    .Starts = g.Start,
                    .Ends = g.Right
               })
        End Function
    End Class

    Public Class FootprintTrace : Inherits ClassObject
        <XmlElement> Public Property Footprints As MatchResult()

        Public Function ToFootprints(DOOR As DOOR, maps As IEnumerable(Of bbhMappings)) As IEnumerable(Of GenomeMotifFootPrints.PredictedRegulationFootprint)
            Dim mapsHash As Dictionary(Of String, bbhMappings()) = maps.GetMapHash
            Dim LQuery = (From x As MatchResult In Footprints.AsParallel
                          Select (From site As GenomeMotifFootPrints.PredictedRegulationFootprint
                                  In x.ToFootprints
                                  Select site.FillSites(DOOR, mapsHash))).MatrixToVector
            Return LQuery.MatrixAsIterator
        End Function
    End Class
End Namespace