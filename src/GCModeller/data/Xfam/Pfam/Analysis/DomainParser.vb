#Region "Microsoft.VisualBasic::dd8ff9a2a730cabd6ec417aed8a24aa3, data\Xfam\Pfam\Analysis\DomainParser.vb"

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

' Module DomainParser
' 
'     Function: __domainFilter, __groupAndTrimOverlap, __trimOverlaps, Parser
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.ProteinModel

Public Module DomainParser

    Public Const Evalue1En5 As Double = 10 ^ -3

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="queryIteration"></param>
    ''' <param name="evalue"></param>
    ''' <param name="coverage"></param>
    ''' <param name="identities">暂时无用</param>
    ''' <param name="offset"></param>
    ''' <returns></returns>
    Public Function Parser(queryIteration As BlastPlus.Query,
                           Optional evalue As Double = Evalue1En5,
                           Optional coverage As Double = 0.85,
                           Optional identities As Double = 0.3,
                           Optional offset As Double = 0.1) As DomainModel()

        Dim LQuery = (From Hit As BlastPlus.SubjectHit
                      In queryIteration.SubjectHits
                      Where applyDomainFilter(Hit, evalue, coverage, identities)
                      Select Pfam = PfamFastaComponentModels.PfamFasta.ParseEntry(Hit.Name),
                             Location = New Location(Hit.QueryLocation),
                             Hit.Score.Expect
                      Order By Location.Left Ascending).ToArray   '

        Dim lenOffset As Integer = offset * queryIteration.QueryLength
        Dim ParsedDomains = (From sId As String
                             In (From parsedLDM In LQuery Select parsedLDM.Pfam.PfamCommonName Distinct).ToArray
                             Let ddLoci As Location() = (From parsedLDM In LQuery
                                                         Where String.Equals(parsedLDM.Pfam.PfamCommonName, sId)
                                                         Select parsedLDM.Location).ToArray
                             Let ChunkBuffer = (From loci As Location
                                                In Loci_API.Group(ddLoci, lenOffset)
                                                Select New DomainModel(sId, Location:=loci)).ToArray
                             Select ChunkBuffer).IteratesALL.OrderBy(Function(x) x.Start).ToArray
        Dim Domains As DomainModel() = doGroupingAndTrimOverlap(ParsedDomains, lenOffset)
        Domains = trimOverlaps(Domains, 5) ', evalues)

        Return Domains
    End Function

    ''' <summary>
    ''' 按照长度值将重叠的结构域清除掉，只留下大的结构域，因为在这之前都是经过阈值筛选了的，所以都是满足条件了的，
    ''' 这里只选择比较大的结构域，但是这样子会有什么问题么？有重叠的时候在KEGG上面是首先显示出比较大的结构域的
    ''' </summary>
    ''' <param name="domains"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Private Function trimOverlaps(domains As DomainModel(), lenOffset As Integer) As DomainModel() ', evalues As Dictionary(Of String, Double)
        ''Dim LQuery = (From x As KeyValuePair(Of String, Double)
        ''              In evalues
        ''              Let domain As DomainObject = Pfam.ProteinDomainArchitecture.PfamString.GetDomain(x.Key)
        ''              Select domain,
        ''                  evalue = x.Value          ' 因为可能会在不同的位置上面出现多个相同的结构域
        ''              Group By domain.Identifier Into Group) _
        ''                  .ToDictionary(Function(x) x.Identifier, Function(x) x.Group.ToArray)
        'Dim LQuery = (From x In domains Select domain = x, x.Location.FragmentSize Group By domain.DomainId Into Group).ToDictionary(Function(x) x.DomainId, Function(x) x.Group.ToArray)
        'Dim GetEvalues = (From x As DomainModel In domains
        '                  Let d = LQuery(x.DomainId)
        '                  Select x,
        '                      evalue = (From od In d
        '                                Where x.Location.ContainSite(od.domain.Location.Center)
        '                                Select od.FragmentSize).FirstOrDefault).AsList
        Dim lstDomains As New List(Of DomainModel)
        Dim GetDomains As List(Of DomainModel) = domains.AsList

        Do While GetDomains.Count > 0
            Dim domain As DomainModel = GetDomains.First
            Dim loci As Location = DirectCast(domain, IMotifSite).site
            Dim overlaps As DomainModel() = LinqAPI.Exec(Of DomainModel) <=
 _
                From x As DomainModel
                In GetDomains
                Let location As Location = DirectCast(x, IMotifSite).site
                Where loci.Inside(location, lenOffset) OrElse
                    location.Inside(loci, lenOffset)
                Select x
                Order By DirectCast(x, IMotifSite).site.FragmentSize Descending

            If overlaps.Length <= 1 Then
                Call lstDomains.Add(domain)  ' 这个结构域没有任何重叠
                Call GetDomains.Remove(domain)
            Else
                Call lstDomains.Add(overlaps.First)   ' 当具有重叠的话，将evalue最小的结构域留下来

                For Each x In overlaps
                    Call GetDomains.Remove(x)
                Next
            End If
        Loop

        Return lstDomains.ToArray
    End Function

    ''' <summary>
    ''' Domain只是比对上query序列的一部分，所以不应该要求要覆盖全长，而是能够将hit本身给覆盖住就可以了
    ''' 由于只是比对上一部分，所以identities不可能会很高，一般是在20%到60%之间，
    ''' 也就是说identities可能在这里并不能作为一个判断的标准了，因为query序列越长，则identities则可能越低，但是那个domain还是可能能够真实存在的
    '''
    ''' 一般而言，gaps和evalue是成正比的，gaps值越大，则evalue越大，所以这里使用evalue就可以筛掉了
    ''' </summary>
    ''' <param name="hit"></param>
    ''' <param name="evalue"></param>
    ''' <param name="coverage"></param>
    ''' <param name="identities">暂时无用</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Private Function applyDomainFilter(hit As BlastPlus.SubjectHit,
                                    evalue As Double,
                                    coverage As Double,
                                    identities As Double) As Boolean
        Dim b As Boolean = hit.Score.Expect <= evalue AndAlso
            hit.Length / Val(hit.LengthHit) > coverage AndAlso
            Math.Abs(hit.LengthHit - hit.LengthQuery) < 20
        Return b
    End Function

    ''' <summary>
    ''' 分组并去掉重叠的数据，这个主要是针对Pfam-A的比对数据，对于CDD数据库可能没有什么用途
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="lenOffset"></param>
    ''' <returns></returns>
    Private Function doGroupingAndTrimOverlap(source As IEnumerable(Of DomainModel), lenOffset As Integer) As DomainModel()
        Dim Group = (From domainLDM As IGrouping(Of String, DomainModel)
                     In source.GroupBy(Function(x As ProteinModel.DomainModel) x.DomainId)
                     Let locations As DomainModel() = domainLDM.OrderBy(Function(n) DirectCast(n, IMotifSite).site.Left).ToArray
                     Select DomainId = domainLDM.Key, locations).ToArray
        Dim ClearOverlap = (From lstDomain In Group
                            Let segments As Location() =
                                lstDomain.locations.Select(Function(loci) DirectCast(loci, IMotifSite).site).ToArray
                            Select lstDomain.DomainId,
                                CleanLocations = FragmentAssembly(segments, lenOffset)).ToArray
        Dim domains As DomainModel() =
            LinqAPI.Exec(Of DomainModel) <= From x
                                            In ClearOverlap
                                            Let merge As DomainModel() =
                                                x.CleanLocations.Select(Function(n) New DomainModel(x.DomainId, n)).ToArray
                                            Select merge
        Return domains
    End Function
End Module
