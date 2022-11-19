#Region "Microsoft.VisualBasic::89286e5526e4568afd3d5fe461c58d1a, GCModeller\data\RegulonDatabase\Regprecise\WebServices\Regulations.vb"

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


    ' Code Statistics:

    '   Total Lines: 235
    '    Code Lines: 167
    ' Comment Lines: 41
    '   Blank Lines: 27
    '     File Size: 10.25 KB


    '     Class Regulation
    ' 
    '         Properties: Family, Regulator, Regulon, Site, Type
    ' 
    '     Class Regulations
    ' 
    '         Properties: Regulations, Regulators, Sites
    ' 
    '         Function: GetMotifFamily, GetRegulatesSites, (+2 Overloads) GetRegulations, (+2 Overloads) GetRegulator, GetRegulators
    '                   GetSite, IsRegulates, listRegulators, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Regprecise.WebServices

    Public Class Regulation

        ''' <summary>
        ''' <see cref="JSON.regulator.vimssId"/>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Regulator As Integer
        ''' <summary>
        ''' <see cref="JSON.site.geneLocusTag"/>:<see cref="JSON.site.position"/>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Site As String
        <XmlAttribute> Public Property Type As Types
        <XmlAttribute> Public Property Family As String
        ''' <summary>
        ''' <see cref="JSON.regulator.regulonId"/>
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Regulon As Integer
    End Class

    ''' <summary>
    ''' %Repository%/Regprecise/MEME/regulations.xml
    ''' (在进行了MEME分析之后，使用这个模块来生成所预测的调控关系)
    ''' </summary>
    Public Class Regulations

        Public Property Regulations As Regulation()
            Get
                Return _regulations
            End Get
            Set(value As Regulation())
                _regulations = value
                If _regulations Is Nothing Then
                    _regulations = New Regulation() {}
                End If

                _siteRegulations = (From site As Regulation In _regulations
                                    Select site
                                    Group site By site.Site Into Group) _
                                         .ToDictionary(Function(obj) obj.Site,
                                                       Function(obj) obj.Group.ToArray)
                _regulatorRegulations = (From regulator As Regulation In _regulations
                                         Select regulator
                                         Group regulator By regulator.Regulator Into Group) _
                                              .ToDictionary(Function(obj) obj.Regulator,
                                                            Function(obj) obj.Group.ToArray)

                Call $"{value.Length} {NameOf(Regulations)} rules...".__DEBUG_ECHO
                Call $"{_siteRegulations.Count} {NameOf(_siteRegulations)} rules...".__DEBUG_ECHO
                Call $"{_regulatorRegulations.Count} {NameOf(_regulatorRegulations)} rules...".__DEBUG_ECHO

#If DEBUG Then
                Dim LQuery = (From site In _siteRegulations.AsParallel Where site.Value.Length > 1 Select site.Key).ToArray
                If Not LQuery.IsNullOrEmpty Then
                    Call $"{String.Join("; ", LQuery)} have more than one  duplicated records....".__DEBUG_ECHO
                End If
#End If
            End Set
        End Property
        Public Property Sites As JSON.site()
            Get
                Return _sites
            End Get
            Set(value As JSON.site())
                _sites = value
                If _sites Is Nothing Then
                    _sites = New JSON.site() {}
                End If

                Dim source = _sites.RemoveDuplicates(Function(site) site.geneVIMSSId)

                _sitesHash = source.ToDictionary(Function(site) site.geneVIMSSId)
                _sitesLocusHash = (From site As JSON.site
                                   In _sites.AsParallel
                                   Select uid = $"{site.geneLocusTag}:{site.position}".ToLower, site
                                   Group By uid Into Group).ToDictionary(Function(site) site.uid,
                                                                         Function(site) site.Group.First.site)

                Call $"{_sites.Length} {NameOf(Sites)}...".__DEBUG_ECHO
                Call $"{_sitesHash.Count} {NameOf(_sitesHash)}....".__DEBUG_ECHO
                Call $"{_sitesLocusHash.Count} {NameOf(_sitesLocusHash)}...".__DEBUG_ECHO
            End Set
        End Property
        Public Property Regulators As JSON.regulator()
            Get
                Return _regulators
            End Get
            Set(value As JSON.regulator())
                _regulators = value
                If _regulators Is Nothing Then
                    _regulators = New JSON.regulator() {}
                End If
                _regulatorsHash = (From regulator As JSON.regulator
                                   In _regulators
                                   Select regulator
                                   Group regulator By regulator.vimssId Into Group) _
                                        .ToDictionary(Function(regulator) regulator.Group.First.vimssId,
                                                      Function(obj) obj.Group.First)
                _regulatorsDict = (From regulator As JSON.regulator
                                   In _regulators
                                   Where Not String.IsNullOrEmpty(regulator.locusTag)
                                   Select regulator
                                   Group regulator By regulator.locusTag Into Group) _
                                        .ToDictionary(Function(prot) prot.locusTag,
                                                      Function(prot) prot.Group.First)

                Call $"{_regulators.Length} {NameOf(Regulators)}....".__DEBUG_ECHO
                Call $"{_regulatorsHash.Count} {NameOf(_regulatorsHash)}...".__DEBUG_ECHO
            End Set
        End Property

        Dim _siteRegulations As Dictionary(Of String, Regulation())
        Dim _regulatorRegulations As Dictionary(Of Integer, Regulation())
        Dim _regulations As Regulation()
        Dim _sites As JSON.site()
        Dim _sitesHash As Dictionary(Of Integer, JSON.site)
        Dim _regulators As JSON.regulator()
        Dim _regulatorsHash As Dictionary(Of Integer, JSON.regulator)
        Dim _regulatorsDict As Dictionary(Of String, JSON.regulator)

        Public Overloads Function ToString(regulation As Regulation) As String
            Return $"[{regulation.Family}]  {_regulatorsHash(regulation.Regulator).ToString} ==> {_sitesHash(regulation.Site).ToString}"
        End Function

        ''' <summary>
        ''' 从调控因子的编号获取得到他的调控因子的数据模型
        ''' </summary>
        ''' <param name="locusId"></param>
        ''' <returns></returns>
        Public Function GetRegulator(locusId As String) As JSON.regulator
            If _regulatorsDict.ContainsKey(locusId) Then
                Return _regulatorsDict(locusId)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 通过vimssid编号来获取得到调控因子的数据模型
        ''' </summary>
        ''' <param name="geneVIMSSId"></param>
        ''' <returns></returns>
        Public Function GetRegulator(geneVIMSSId As Integer) As JSON.regulator
            If _regulatorsHash.ContainsKey(geneVIMSSId) Then
                Return _regulatorsHash(geneVIMSSId)
            Else
                Return Nothing
            End If
        End Function

        Public Function GetRegulations(site As JSON.site) As Regulation()
            If site Is Nothing Then
                Return Nothing
            End If

            Dim uid As String = $"{site.geneLocusTag}:{site.position}"
            Return GetRegulations(uid)
        End Function

        ''' <summary>
        ''' {locus_tag:position}
        ''' </summary>
        ''' <param name="uid"></param>
        ''' <returns></returns>
        Public Function GetRegulations(uid As String) As Regulation()
            If _siteRegulations.ContainsKey(uid) Then '(site.geneVIMSSId) Then
                Return _siteRegulations(uid) '(site.geneVIMSSId)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="site">$"{site.geneLocusTag}:{site.position}"</param>
        ''' <returns></returns>
        Public Function GetMotifFamily(site As String) As String
            If _siteRegulations.ContainsKey(site) Then
                Return _siteRegulations(site).First.Family
            Else
                Return ""
            End If
        End Function

        Dim _sitesLocusHash As Dictionary(Of String, JSON.site)

        ''' <summary>
        ''' $"{<see cref="JSON.site.geneLocusTag"/>}:{<see cref="JSON.site.position"/>}"
        ''' </summary>
        ''' <param name="geneLocusTag"></param>
        ''' <returns></returns>
        Public Function GetSite(geneLocusTag As String) As JSON.site
            Dim key As String = geneLocusTag.ToLower

            If _sitesLocusHash.ContainsKey(key) Then
                Return _sitesLocusHash(key)
            Else
                Return Nothing
            End If
        End Function

        Public Function IsRegulates(regulator As String, site As String) As Boolean
            Dim regulatorLDM = Me._regulatorsDict(regulator)
            Dim id As Integer = regulatorLDM.vimssId
            Dim regulations = Me._regulatorRegulations(id)
            Dim LQuery = (From x In regulations Where String.Equals(x.Site, site, StringComparison.OrdinalIgnoreCase) Select x).FirstOrDefault
            Return Not LQuery Is Nothing
        End Function

        Public Function GetRegulators(site As String) As String()
            Dim regulations As Regulation() = GetRegulations(site)
            Dim TF = regulations.Select(Function(x) GetRegulator(x.Regulator))
            Dim locus As String() = (From x In TF Where Not x Is Nothing Select x.locusTag).ToArray
            Return locus
        End Function

        Public Function GetRegulatesSites(regulator As String) As String()
            Dim regulatorLDM = Me._regulatorsDict(regulator)
            Dim regulations = Me._regulatorRegulations(regulatorLDM.vimssId)
            Return regulations.Select(Function(x) x.Site)
        End Function

        Public Function listRegulators() As String()
            Dim LQuery = (From x In Me.Regulators Select x.locusTag).ToArray
            Return LQuery
        End Function
    End Class
End Namespace
