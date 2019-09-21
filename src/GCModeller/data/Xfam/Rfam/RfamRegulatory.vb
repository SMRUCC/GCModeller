#Region "Microsoft.VisualBasic::7f7cb5692798852f1d5349501f23b4ba, Xfam\Rfam\RfamRegulatory.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class RfamRegulatory
    ' 
    '     Properties: ATGDist, Gene, gStop, site, SiteEvalue
    '                 siteLeft, SiteLocation, SitePvalue, siteStrand, Trace
    ' 
    '     Function: __createObject, __nullRegulatory, __siteCreates, AnalysisRegulatory
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans

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
            Throw New NotImplementedException
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
                                                                                       Group site By site.LocusId Into Group).Select(Function(xSite) xSite.Group.First).ToArray)  ' 因为数据库之中会出现重复比对上的记录，所以这里只需要一条就够了
        Dim LQuery = (From site In mastSites Select RfamId = site.Trace.Split(":"c).First, site)
        Dim Regulatory = (From site In LQuery.AsParallel Select __createObject(Rfam, site.RfamId, site.site)).ToVector
        Return Regulatory
    End Function

    Private Shared Function __createObject(Rfam As Dictionary(Of String, Rfamily()), RfamId As String, site As MastSites) As RfamRegulatory()
        If Not Rfam.ContainsKey(RfamId) Then
            Return {__nullRegulatory(RfamId, site)}
        End If

        Dim lst As Rfamily() = Rfam(RfamId)
        Dim sites = lst.Select(Function(x) __siteCreates(x, site))
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
