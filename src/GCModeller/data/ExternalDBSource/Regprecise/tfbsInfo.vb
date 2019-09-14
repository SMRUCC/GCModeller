#Region "Microsoft.VisualBasic::824f2cc6c5e9c286bc87b013de3cae07, ExternalDBSource\Regprecise\tfbsInfo.vb"

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

    '     Class Tfbs_Info
    ' 
    '         Properties: Motif, Regulator, Regulog, siteId, tfbsId
    ' 
    '         Function: CreateObject, ToString
    ' 
    '     Class BidrBhRegulator
    ' 
    '         Properties: BiologicalProcess, Cog, description, Effectors, Family
    '                     Length, Matched, Pfam, Regulator, TfbsIds
    ' 
    '         Function: Convert, GetRegpreciseRegulator, (+3 Overloads) Match, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions

Imports Microsoft.VisualBasic.DataVisualization.DocumentFormat.Csv.Reflection
Imports Microsoft.VisualBasic.DataVisualization.DocumentFormat.Extensions

Namespace Regprecise

    Public Class Tfbs_Info
        <Column("tfbs_Id")> Public Property tfbsId As String
        <Column("site_Id")> Public Property siteId As String
        <Column("regulator")> Public Property Regulator As String
        <Column("regulog")> Public Property Regulog As String
        <Column("motif")> Public Property Motif As String

        Public Overrides Function ToString() As String
            Return tfbsId
        End Function

        Public Shared Function CreateObject(FASTA As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile) As Tfbs_Info()
            Dim LQuery As Regprecise.Tfbs_Info() = (From FsaObject As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject
                                                   In FASTA
                                                   Let attrs = FsaObject.Attributes.First
                                                   Let siteId As String = Regex.Match(attrs, "gene=[^]]+").Value.Split("=").Last
                                                   Let regulator As String = Regex.Match(attrs, "regulator=[^]]+").Value.Split("=").Last
                                                   Let regulog As String = Regex.Match(attrs, "regulog=[^]]+").Value.Split("=").Last
                                                   Select New Regprecise.Tfbs_Info With {
                                                       .tfbsId = attrs.Split.First,
                                                       .siteId = siteId,
                                                       .Regulator = regulator,
                                                       .Regulog = regulog,
                                                       .Motif = FsaObject.SequenceData}).ToArray
            Return LQuery
        End Function
    End Class

    ''' <summary>
    ''' Bidirection best hit regulator
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BidrBhRegulator : Implements LANS.SystemsBiology.Assembly.ComponentModel.Collection.Generic.IAccessionIdEnumerable

        <Column("Matched_Id")> Public Property Matched As String Implements LANS.SystemsBiology.Assembly.ComponentModel.Collection.Generic.IAccessionIdEnumerable.UniqueId
        ''' <summary>
        ''' <see cref="BidrBhRegulator.Matched">本蛋白质实体对象</see>的蛋白质分子长度
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("length")> Public Property Length As String
        <Column("description")> Public Property description As String
        ''' <summary>
        ''' Regprecise_regulator
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("Regprecise_regulator")> Public Overridable Property Regulator As String
        <Column("Family")> Public Property Family As String
        <Column("COG")> Public Property Cog As String
        <Column("BiologiclaProcess")> Public Property BiologicalProcess As String
        <ArrayColumn("Effectors")> Public Property Effectors As String()
        <ArrayColumn("Regprecise_tfbs")> Public Property TfbsIds As String()
        <Column("Pfam-string")> Public Property Pfam As String

        Public Overrides Function ToString() As String
            Return Regulator
        End Function

        ''' <summary>
        ''' 通过最佳双向比对来匹配目标蛋白质集合中在Regprecise数据库中存在的记录
        ''' </summary>
        ''' <param name="RegpreciseRegulators">Regprecise调控因子序列数据库</param>
        ''' <param name="Query">所将要进行匹配的目标蛋白质集合</param>
        ''' <param name="LocalBLAST">本地BLAST服务</param>
        ''' <param name="QueryGrep">可选参数，目标蛋白质集合的ID号的解析脚本</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Match(RegpreciseRegulators As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile, RegpreciseTfbs As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile,
                                     Query As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaFile,
                                     LocalBLAST As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.InteropService,
                                     Optional QueryGrep As String = "", Optional WorkDir As String = "", Optional ExportAll As Boolean = True) As BidrBhRegulator()

            Dim siteInforList = (From item As LANS.SystemsBiology.Assembly.SequenceModel.FASTA.FastaObject
                                 In RegpreciseTfbs
                                 Let site = Regex.Match(item.Title, "gene=[^]]+").Value.Split("=").Last Select New KeyValuePair(Of String, String)(site, item.Attributes.First.Split.First)).ToArray
            Dim BesthitBLAST = New LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BidirBestHitBLAST(LocalBLAST, If(String.IsNullOrEmpty(WorkDir), My.Computer.FileSystem.SpecialDirectories.Temp, WorkDir))
            Dim bhArray = BesthitBLAST.Peformance(Query.SourceFile, RegpreciseRegulators.SourceFile,
                                                  LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.GrepScript.Compile(QueryGrep).MethodPointer,
                                                  LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.GrepScript.Compile("tokens ' ' 0;tokens | last").MethodPointer,
                                                  "1e-3", ExportAll:=ExportAll) _
                         .AsDataSource(Of LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BidirBestHitBLAST.BiDireBesthit)(False)
            Dim ExtractedTfbsInfo = (From item In RegpreciseRegulators
                                     Let tfbs = Function() As String()
                                                    Dim Tokens = Regex.Match(item.Attributes.Last, "tfbs=[^]]+").Value.Split("=").Last.Split(";")
                                                    Dim GetTfbs = (From site As KeyValuePair(Of String, String) In siteInforList Where Array.IndexOf(Tokens, site.Key) > -1 Select site.Value).ToArray
                                                    Return GetTfbs
                                                End Function
                                     Select New KeyValuePair(Of String, String())(item.Title.Split.First.Split("|").Last, tfbs())).ToArray
            Dim LQuery = (From item In bhArray
                          Where Not String.IsNullOrEmpty(item.HitName)
                          Select New BidrBhRegulator With {.Regulator = item.HitName, .Matched = item.QueryName,
                                                           .TfbsIds = (From tfbs
                                                                       In ExtractedTfbsInfo.AsParallel
                                                                       Where String.Equals(item.HitName, tfbs.Key)
                                                                       Select tfbs.Value).First}).ToArray
            Return LQuery
        End Function

        Public Shared Function Match(BidrBhRegulator As BidrBhRegulator(),
                                     myva_COG As NCBI.Extensions.LocalBLAST.Application.RpsBLAST.CogCategory.myvaCog(),
                                     GenomeRegulator As Regprecise.TranscriptionFactors, PfamStrings As NCBI.Extensions.LocalBLAST.Application.RpsBLAST.ProteinDomainArchitecture.PfamString()) As BidrBhRegulator()
            Dim RegpreciseRegulators = GenomeRegulator.Regulators
            Dim GetMyvaCog = Function(Id As String) As NCBI.Extensions.LocalBLAST.Application.RpsBLAST.CogCategory.myvaCog
                                 Dim LQuery = (From item In myva_COG.AsParallel Where String.Equals(item.QueryName, Id) Select item).ToArray
                                 If LQuery.IsNullOrEmpty Then
                                     Return Nothing
                                 Else
                                     Return LQuery.First
                                 End If
                             End Function
            Dim GetPfamString = Function(Id As String) As NCBI.Extensions.LocalBLAST.Application.RpsBLAST.ProteinDomainArchitecture.PfamString
                                    Dim LQuery = (From item In PfamStrings.AsParallel Where String.Equals(item.Protein, Id) Select item).ToArray
                                    If LQuery.IsNullOrEmpty Then
                                        Return Nothing
                                    Else
                                        Return LQuery.First
                                    End If
                                End Function
            Dim GetRegpreciseRegulator = Function(Id As String) As ExternalDBSource.Regprecise.BacteriaGenome.Regulon.Regulator
                                             Dim LQuery = (From item In RegpreciseRegulators Where String.Equals(Id, item.LocusTag.Key) Select item).ToArray
                                             If LQuery.IsNullOrEmpty Then
                                                 Return Nothing
                                             Else
                                                 Return LQuery.First
                                             End If
                                         End Function
            Dim ChunkBuffer As BidrBhRegulator() = New BidrBhRegulator(BidrBhRegulator.Count - 1) {}
            For i As Integer = 0 To ChunkBuffer.Count - 1
                Dim matchedItem = BidrBhRegulator(i)
                Dim Cog = GetMyvaCog(matchedItem.Matched)
                Dim Pfam = GetPfamString(matchedItem.Matched)
                Dim regprecise = GetRegpreciseRegulator(matchedItem.Regulator.Split(":").Last)

                If Cog IsNot Nothing Then
                    matchedItem.Cog = Cog.Cog_category
                End If

                If Pfam IsNot Nothing Then
                    matchedItem.description = Pfam.Description
                    matchedItem.Length = Pfam.Length
                    matchedItem.Pfam = Pfam.PfamString
                End If

                If regprecise IsNot Nothing Then
                    matchedItem.BiologicalProcess = regprecise.BiologicalProcess
                    matchedItem.Effectors = Split(regprecise.Effector, "; ")
                    matchedItem.Family = regprecise.Family
                End If

                ChunkBuffer(i) = matchedItem
            Next

            Return ChunkBuffer
        End Function

        ''' <summary>
        ''' 从最佳双向BLAST比对结果之中得到所需要的基础数据
        ''' </summary>
        ''' <param name="BLASTbh"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Convert(BLASTbh As LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application.BidirBestHitBLAST.BiDireBesthit()) As BidrBhRegulator()
            Dim LQuery = (From item In BLASTbh Select New BidrBhRegulator With {.Matched = item.QueryName, .Regulator = item.HitName, .Length = item.Length, .description = item.Description}).ToArray
            Return LQuery
        End Function

        Private Shared Function GetRegpreciseRegulator(Id As String, RegpreciseRegulators As Regprecise.BacteriaGenome.Regulon.Regulator()) As ExternalDBSource.Regprecise.BacteriaGenome.Regulon.Regulator
            Dim LQuery = (From item In RegpreciseRegulators Where String.Equals(Id, item.LocusTag.Key) Select item).ToArray
            If LQuery.IsNullOrEmpty Then
                Return Nothing
            Else
                Return LQuery.First
            End If
        End Function

        Public Shared Function Match(BidrBhRegulator As BidrBhRegulator(), GenomeRegulator As Regprecise.TranscriptionFactors) As BidrBhRegulator()
            Dim RegpreciseRegulators = GenomeRegulator.Regulators
            Dim LQuery = (From matchedItem In BidrBhRegulator.AsParallel Let regprecise = GetRegpreciseRegulator(matchedItem.Regulator.Split(":").Last, RegpreciseRegulators)
                          Let Apply = Function() As BidrBhRegulator
                                          If regprecise IsNot Nothing Then
                                              matchedItem.BiologicalProcess = regprecise.BiologicalProcess
                                              matchedItem.Effectors = Split(regprecise.Effector, "; ")
                                              matchedItem.Family = regprecise.Family
                                          End If
                                          Return matchedItem
                                      End Function Select Apply()).ToArray
            Return LQuery
        End Function
    End Class
End Namespace
