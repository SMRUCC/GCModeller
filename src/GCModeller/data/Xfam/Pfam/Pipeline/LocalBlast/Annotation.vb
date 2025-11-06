#Region "Microsoft.VisualBasic::13ebe91404e36db1b7b5a4cda49d97eb, data\Xfam\Pfam\Pipeline\LocalBlast\Annotation.vb"

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

    '   Total Lines: 283
    '    Code Lines: 201 (71.02%)
    ' Comment Lines: 47 (16.61%)
    '    - Xml Docs: 74.47%
    ' 
    '   Blank Lines: 35 (12.37%)
    '     File Size: 13.25 KB


    '     Module Annotation
    ' 
    '         Function: AnnotatedFromHitsGroup, ApplyDomainFilter, CreatePfamStringAnnotation, doGroupingAndTrimOverlap, DoHitsGrouping
    '                   getOverlaps, parseDomains, ParserRaw, trimOverlaps
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data.Xfam.Pfam.Pipeline.Database
Imports SMRUCC.genomics.Data.Xfam.Pfam.Pipeline.LocalBlast
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.ProteinModel

Namespace Pipeline.LocalBlast

    Public Module Annotation

        <Extension>
        Public Iterator Function DoHitsGrouping(hits As IEnumerable(Of PfamHit)) As IEnumerable(Of NamedCollection(Of PfamHit))
            Dim proteinGroups As New Dictionary(Of String, List(Of PfamHit))

            Call "Create protein groups...".debug

            For Each hit As PfamHit In hits
                If Not proteinGroups.ContainsKey(hit.QueryName) Then
                    proteinGroups(hit.QueryName) = New List(Of PfamHit)
                    Call $"{hit.QueryName}: {hit.description}".debug
                End If

                proteinGroups(hit.QueryName) += hit
            Next

            For Each proteinGroup In proteinGroups
                Yield New NamedCollection(Of PfamHit) With {
                    .name = proteinGroup.Key,
                    .description = proteinGroup.Value(Scan0).description,
                    .value = proteinGroup.Value
                }
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Iterator Public Function CreateAnnotations(source As IEnumerable(Of NamedCollection(Of PfamHit))) As IEnumerable(Of PfamString.PfamString)
            Dim protein As PfamString.PfamString
            Dim padLen% = 80

            For Each query As NamedCollection(Of PfamHit) In source
                protein = query.CreatePfamStringAnnotation
                Console.WriteLine(query.name & vbTab & Mid(query.description, 1, padLen) & If(padLen > query.description.Length, New String(" "c, padLen - query.description.Length), "") & vbTab & protein.PfamString.JoinBy("+"))

                Yield protein
            Next
        End Function

        <Extension>
        Public Function CreatePfamStringAnnotation(query As NamedCollection(Of PfamHit)) As PfamString.PfamString
            Dim domains As NamedCollection(Of DomainModel) = query.AnnotatedFromHitsGroup()
            Dim idTable As Dictionary(Of String, String) = query _
                .Select(Function(p) p.Pfam) _
                .GroupBy(Function(p) p.CommonName) _
                .ToDictionary(Function(p) p.Key,
                              Function(p)
                                  Return p.First.PfamId
                              End Function)

            If domains.IsEmpty Then
                Return New PfamString.PfamString With {
                    .ProteinId = query.name,
                    .Description = query.description,
                    .Length = query(Scan0).query_length
                }
            End If

            Dim domainIDs$() = (From d As DomainModel In domains Select $"{idTable(d.DomainId)}:{d.DomainId}" Distinct).ToArray
            Dim pfamString$() = domains _
                .OrderBy(Function(d) d.start) _
                .Select(Function(x)
                            Return $"{x.DomainId}({x.start}|{x.ends})"
                        End Function) _
                .Distinct _
                .ToArray
            Dim protein As New PfamString.PfamString With {
                .ProteinId = query.name,
                .Description = query.description,
                .Length = query(Scan0).query_length,
                .Domains = domainIDs,
                .PfamString = pfamString
            }

            Return protein
        End Function

        Public Const Evalue1En5 As Double = 10 ^ -3

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="evalue"></param>
        ''' <param name="coverage"></param>
        ''' <param name="identities">暂时无用</param>
        ''' <param name="offset"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ParserRaw(query As BlastPlus.Query,
                                  Optional evalue As Double = Evalue1En5,
                                  Optional coverage As Double = 0.85,
                                  Optional identities As Double = 0.3,
                                  Optional offset As Double = 0.1) As NamedCollection(Of DomainModel)
            Return query _
                .ParseProteinQuery _
                .AnnotatedFromHitsGroup(evalue, coverage, identities, offset)
        End Function

        ''' <summary>
        ''' 从一组对相同的目标蛋白做的比较结果中进行结构域的注释
        ''' </summary>
        ''' <param name="hits">query都是相同的蛋白序列</param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function AnnotatedFromHitsGroup(hits As IEnumerable(Of PfamHit),
                                               Optional evalue As Double = Evalue1En5,
                                               Optional coverage As Double = 0.85,
                                               Optional identities As Double = 0.3,
                                               Optional offset As Double = 0.1) As NamedCollection(Of DomainModel)

            Dim hitsArray As PfamHit() = hits _
                .Where(Function(hit) hit.ApplyDomainFilter(evalue, coverage, identities)) _
                .ToArray

            If hitsArray.Length = 0 Then
                Return Nothing
            End If

            ' 因为query是相同的一个对象，所以下面的两个属性都是相同的值
            ' 直接取出第一个
            Dim queryId$ = hitsArray(Scan0).QueryName
            Dim description$ = hitsArray(Scan0).description
            Dim queryLength% = hitsArray(Scan0).query_length
            Dim hitsGroup = hitsArray _
                .Select(Function(hit) (hit:=hit, Pfam:=hit.Pfam)) _
                .GroupBy(Function(g)
                             Return g.Pfam.CommonName
                         End Function) _
                .ToArray
            Dim lenOffset As Integer = offset * queryLength
            Dim domains As DomainModel() = hitsGroup _
                .parseDomains(lenOffset) _
                .doGroupingAndTrimOverlap(lenOffset) _
                .IteratesALL _
                .trimOverlaps(5) _
                .ToArray
            Dim annotation As New NamedCollection(Of DomainModel) With {
                .name = queryId,
                .description = description,
                .value = domains
            }

            Return annotation
        End Function

        <Extension>
        Private Iterator Function parseDomains(hitsGroup As IGrouping(Of String, (hit As PfamHit, pfam As PfamEntryHeader))(), lenOffset%) As IEnumerable(Of DomainModel)
            For Each domain In hitsGroup
                ' 因为相同的编号的结构与可能在蛋白序列上存在多个重复
                ' 所以会需要在这里按照位点合并分组
                Dim locations As Location() = domain _
                   .Select(Function(hit)
                               Return New Location(hit.hit.start, hit.hit.ends)
                           End Function) _
                   .DoCall(Function(locis)
                               Return LociAPI.Group(locis, lenOffset).ToArray
                           End Function)

                For Each loci As Location In locations
                    Yield New DomainModel(domain.Key, loci)
                Next
            Next
        End Function

        <Extension>
        Private Function getOverlaps(domainHits As IEnumerable(Of DomainModel), loci As Location, lenOffset%) As DomainModel()
            Return LinqAPI.Exec(Of DomainModel) <=
 _
                From x As DomainModel
                In domainHits
                Let location As Location = DirectCast(x, IMotifSite).site
                Where loci.Inside(location, lenOffset) OrElse location.Inside(loci, lenOffset)
                Select x
                Order By DirectCast(x, IMotifSite).site.FragmentSize Descending

        End Function

        ''' <summary>
        ''' 按照长度值将重叠的结构域清除掉，只留下大的结构域，因为在这之前都是经过阈值筛选了的，所以都是满足条件了的，
        ''' 这里只选择比较大的结构域，但是这样子会有什么问题么？有重叠的时候在KEGG上面是首先显示出比较大的结构域的
        ''' </summary>
        ''' <param name="domains"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Private Iterator Function trimOverlaps(domains As IEnumerable(Of DomainModel), lenOffset As Integer) As IEnumerable(Of DomainModel)
            Dim domainHits As List(Of DomainModel) = domains.AsList
            Dim clean As DomainModel

            Do While domainHits > 0
                Dim domain As DomainModel = domainHits.First
                Dim loci As Location = DirectCast(domain, IMotifSite).site
                Dim overlaps As DomainModel() = domainHits.getOverlaps(loci, lenOffset)

                If overlaps.Length <= 1 Then
                    ' 这个结构域没有任何重叠
                    domainHits -= domain
                    clean = domain
                Else
                    ' 当具有重叠的话，将evalue最小的结构域留下来
                    clean = overlaps.First

                    For Each x As DomainModel In overlaps
                        Call domainHits.Remove(x)
                    Next
                End If

                Yield clean
            Loop
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
        ''' <returns></returns>
        <Extension>
        Public Function ApplyDomainFilter(hit As PfamHit, evalue#, coverage#, identities#) As Boolean
            Dim b As Boolean = hit.evalue <= evalue AndAlso
                (hit.identities >= identities) AndAlso
                (hit.hit_length / hit.length_hit) >= coverage AndAlso
                (hit.length_hit - hit.length_query).DoCall(AddressOf Math.Abs) <= 16

            Return b
        End Function

        ''' <summary>
        ''' 分组并去掉重叠的数据，这个主要是针对Pfam-A的比对数据，对于CDD数据库可能没有什么用途
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="lenOffset"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Private Iterator Function doGroupingAndTrimOverlap(source As IEnumerable(Of DomainModel), lenOffset As Integer) As IEnumerable(Of DomainModel())
            Dim group = (From domain As IGrouping(Of String, DomainModel)
                         In source.GroupBy(Function(d) d.DomainId)
                         Let locations As DomainModel() = domain _
                             .OrderBy(Function(n) DirectCast(n, IMotifSite).site.left) _
                             .ToArray
                         Select DomainId = domain.Key, locations).ToArray
            Dim clearOverlap = (From list
                                In group
                                Let segments As Location() = list.locations _
                                    .Select(Function(loci) DirectCast(loci, IMotifSite).site) _
                                    .ToArray
                                Select list.DomainId,
                                    cleanLocations = segments.FragmentAssembly(lenOffset)).ToArray
            Dim result As DomainModel()

            For Each clear In clearOverlap
                result = clear.cleanLocations _
                    .Select(Function(n) New DomainModel(clear.DomainId, n)) _
                    .ToArray

                Yield result
            Next
        End Function
    End Module
End Namespace
