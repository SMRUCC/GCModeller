Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Interops.NBCR.Extensions.MEME_Suite.Analysis.MotifScans

''' <summary>
''' 在这里添加被调控的位点的数据就可以构成完整的RNA调控数据了
''' </summary>
Public Class RfamRegulatory : Inherits Rfamily

    Public Property SiteEvalue As Double
    Public Property SitePvalue As Double
    Public Property SiteLocation As String
    ''' <summary>
    ''' 被调控的位点上面的序列
    ''' </summary>
    ''' <returns></returns>
    Public Property site As String
    Public Property siteStrand As Char
    <Column("gStart")> Public Property siteLeft As Integer
    Public ReadOnly Property gStop As Integer
        Get

        End Get
    End Property

    Public Property Gene As String
    Public Property ATGDist As Integer
    Public Property Trace As String

    Public Shared Function AnalysisRegulatory(RfamSites As Rfamily(), mastSites As MastSites()) As RfamRegulatory()
        Dim Rfam As Dictionary(Of String, Rfamily()) = (From site As Rfamily
                                                        In RfamSites
                                                        Select site
                                                        Group site By Name = site.Rfam Into Group) _
                                                            .ToDictionary(Function(x) x.Name,
                                                                          Function(x) (From site As Rfamily In x.Group
                                                                                       Select site
                                                                                       Group site By site.LocusId Into Group).ToArray(Function(xSite) xSite.Group.First))  ' 因为数据库之中会出现重复比对上的记录，所以这里只需要一条就够了
        Dim LQuery = (From site In mastSites Select RfamId = site.Trace.Split(":"c).First, site)
        Dim Regulatory = (From site In LQuery.AsParallel Select __createObject(Rfam, site.RfamId, site.site)).MatrixToVector
        Return Regulatory
    End Function

    Private Shared Function __createObject(Rfam As Dictionary(Of String, Rfamily()), RfamId As String, site As MastSites) As RfamRegulatory()
        If Not Rfam.ContainsKey(RfamId) Then
            Return {__nullRegulatory(RfamId, site)}
        End If

        Dim lst As Rfamily() = Rfam(RfamId)
        Dim sites = lst.ToArray(Function(x) __siteCreates(x, site))
        Return sites
    End Function

    Private Shared Function __siteCreates(Rfam As Rfamily, mastSite As MastSites) As RfamRegulatory
        Dim site As RfamRegulatory = __nullRegulatory(Rfam.Rfam, mastSite)
        Return Rfam.__copyTo(site)
    End Function

    Private Shared Function __nullRegulatory(RfamId As String, site As MastSites) As RfamRegulatory
        Return New RfamRegulatory With {
            .ATGDist = site.ATGDist,
            .SiteEvalue = site.evalue,
            .site = site.SequenceData,
            .Gene = site.Gene,
            .siteLeft = site.Start,
            .siteStrand = site.Strand.GetBriefCode,
            .Trace = site.Trace,
            .SitePvalue = site.pValue,
            .Rfam = RfamId
        }
    End Function
End Class
