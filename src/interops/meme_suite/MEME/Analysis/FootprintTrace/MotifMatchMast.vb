#Region "Microsoft.VisualBasic::7b4f28062e5db4375e47332eb5894edb, meme_suite\MEME\Analysis\FootprintTrace\MotifMatchMast.vb"

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

    '     Module MotifMatchMast
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) __fill, __getPath, __parallelBatch, __toSites, (+4 Overloads) AssignContext
    '                   BatchCompile, BatchCompileDirectly, BuildVirtualFootprints, CompileDirectly, CompileSingle
    '                   MotifMatchCompile, PreCompile
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME.Text
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.XmlOutput.MAST
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace Analysis.FootprintTraceAPI

    ''' <summary>
    ''' 通过MAST来得到MEME中的Motif的结果
    ''' </summary>
    ''' 
    <Package("MAST.MotifMatch")>
    Public Module MotifMatchMast

        Sub New()
            Call Settings.Session.Initialize()
        End Sub

        <ExportAPI("Assign.Context")>
        Public Function AssignContext(footprints As FootprintTrace, PTT As PTT) As FootprintTrace
            Dim RegPrecise As Regulations = GCModeller.FileSystem.GetRegulations.LoadXml(Of Regulations)
            Return footprints.AssignContext(PTT, RegPrecise)
        End Function

        <ExportAPI("Assign.Context"), Extension>
        Public Function AssignContext(footprints As FootprintTrace, PTT As PTT, RegPrecise As Regulations) As FootprintTrace
            If Not footprints.Footprints.IsNullOrEmpty Then
                For Each x In footprints.Footprints
                    Call x.AssignContext(PTT, RegPrecise)
                Next
            End If

            Return footprints
        End Function

        <ExportAPI("Assign.Context"), Extension>
        Public Function AssignContext(ByRef footprints As MatchResult, PTT As PTT, RegPrecise As Regulations) As MatchResult
            If Not footprints.Matches.IsNullOrEmpty Then
                For Each x In footprints.Matches
                    Call x.AssignContext(PTT, RegPrecise)
                Next
            End If

            Return footprints
        End Function

        <ExportAPI("Assign.Context"), Extension>
        Public Function AssignContext(ByRef footprints As MotifHits, PTT As PTT, RegPrecise As Regulations) As MotifHits
            For Each site As LDM.Site In footprints.MEME
                Dim g = PTT.GeneObject(site.Name)
                Dim loci = g.Located(site, site.Size)
                site.Start = loci.Left
                site.Right = loci.Right
            Next

            For Each site In footprints.MAST
                Dim regs = RegPrecise.GetRegulators(site.RegPrecise)
                site.Regulators = regs
            Next

            Return footprints
        End Function

        ''' <summary>
        ''' 预编译资源数据
        ''' </summary>
        ''' <param name="MEME_OUT"></param>
        ''' <param name="MAST_OUT"></param>
        ''' <param name="direct">假若这个参数为真，则不是家族比对，则每一个文件夹都直接是比对的结果，加入为False，则说明是家族的比对，则每一个文件夹下面是家族的比对结果文件夹</param>
        ''' <returns>取决于参数<paramref name="direct"/>: True -> MAST_DIR/mast.xml; False -> MAST/DIR</returns>
        <ExportAPI("Res.PreCompile")>
        Public Function PreCompile(MEME_OUT As String, MAST_OUT As String, direct As Boolean) As Dictionary(Of String, String)
            Call "Start to pre-compile meme_out resource...".__DEBUG_ECHO

            Dim memeText As IEnumerable(Of String) =
                FileIO.FileSystem.GetFiles(MEME_OUT, FileIO.SearchOption.SearchTopLevelOnly, "*.txt")

            Call "files enumeration job done!, start to compile hash table...".__DEBUG_ECHO

            Dim LQuery = (From s As String In memeText.AsParallel
                          Let mast As String = __getPath(s, MAST_OUT)
                          Where Not String.IsNullOrEmpty(mast)
                          Select s,
                              mast).ToDictionary(Function(x) x.s,
                                                 Function(x) If(direct, x.mast & "/mast.xml", x.mast))

            Call $"dataset(size:={LQuery.Count}), build done!".__DEBUG_ECHO

            If LQuery.Count = 0 Then
                Call VBDebugger.Warning("Resource compile failured! Probably you should using the '/Copys' CLI command before you are compile this motif information....")
            End If

            Return LQuery
        End Function

        Private Function __getPath(memeText As String, MAST_OUT As String) As String
            Dim base As String = memeText.BaseName
            Dim DIR As String = MAST_OUT & "/" & base
            If DIR.DirectoryExists Then
                Return DIR
            Else
                DIR = MAST_OUT & "/" & FileIO.FileSystem.GetFileInfo(memeText).Name
                If DIR.DirectoryExists Then
                    Return DIR
                Else
                    Return Nothing
                End If
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MEME">path of meme.txt</param>
        ''' <param name="MAST">path of mast.xml</param>
        ''' <returns></returns>
        Public Function CompileDirectly(MEME As String, MAST As String) As MotifSiteHit()
            Dim motifs As LDM.Motif() = MEME_TEXT.SafelyLoad(MEME)
            If motifs.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim MASTXml As MAST = MAST.LoadXml(Of MAST)(ThrowEx:=False)
            If MASTXml Is Nothing Then
                Dim ex As New Exception("MAST is null!!")
                ex = New Exception(New KeyValuePair(MEME, MAST).GetJson, ex)
                Call App.LogException(ex)
                Call ex.PrintException
                Return Nothing
            End If

            Dim sites As MotifSiteHit() = MASTXml.MotifMatchCompile(trace:=MEME.BaseName)
            Dim memeHash As Dictionary(Of String, LDM.Motif) = motifs.ToDictionary(Function(x) x.Id)

            For Each site As MotifSiteHit In sites
                Dim id As String = site.Trace.Split("."c).Last
                If Not memeHash.ContainsKey(id) Then
                    Call VBDebugger.Warning("???? NOT FOUND:  " & site.Trace)
                Else
                    site.source = memeHash(id).Sites.Select(Function(x) x.Name)
                End If
            Next

            Return sites
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sites">从Csv文档之中得到的位点信息，Xml文档由于缺少位点信息所以无法进行调控关系的构建</param>
        ''' <param name="bbh"></param>
        ''' <param name="hitHash">是否使用hit作为键名</param>
        ''' <returns></returns>
        <Extension>
        Public Function BuildVirtualFootprints(sites As IEnumerable(Of MotifSiteHit),
                                               bbh As IEnumerable(Of BBHIndex),
                                               hitHash As Boolean,
                                               PTT As PTT,
                                               DOOR As DOOR,
                                               MEME_DIR As String) As PredictedRegulationFootprint()

            Dim TFhash As Dictionary(Of String, String())
            If hitHash Then  ' 
                TFhash = (From x As BBHIndex
                          In bbh
                          Select x
                          Group x By x.HitName Into Group) _
                               .ToDictionary(Function(x) x.HitName,
                                             Function(x) x.Group.Select(Function(o) o.QueryName).Distinct.ToArray)
            Else
                TFhash = (From x As BBHIndex
                          In bbh
                          Select x
                          Group x By x.QueryName Into Group) _
                               .ToDictionary(Function(x) x.QueryName,
                                             Function(x) x.Group.Select(Function(o) o.HitName).Distinct.ToArray)
            End If

            Dim result As New List(Of PredictedRegulationFootprint)
            Dim motifs As Dictionary(Of AnnotationModel) = AnnotationModel.LoadMEMEOUT(MEME_DIR, True, True)

            For Each site As MotifSiteHit In sites
                Dim TF As String = site.Regulators.DefaultFirst   ' Csv 文档之中的Tf只有一个记录
                Dim regulators As String()
                If String.IsNullOrEmpty(TF) OrElse Not TFhash.ContainsKey(TF) Then
                    regulators = {}
                Else
                    regulators = TFhash(TF)
                End If

                If Not motifs.ContainsKey(site.Trace) Then
                    Dim msg As String = site.Trace & " is not exists in keys:  " & motifs.Keys.ToArray.GetJson
                    Call VBDebugger.WriteLine(msg, ConsoleColor.Cyan)
                    Throw New KeyNotFoundException(msg)
                End If

                Dim MEMEsite As AnnotationModel = motifs(site.Trace)

                Dim footprint As New PredictedRegulationFootprint With {
                    .MotifFamily = site.Family,
                    .MotifId = site.Trace,
                    .MotifTrace = site.Trace,
                    .RegulatorTrace = TF
                }

                result += footprint.__fill(site, regulators, MEMEsite, DOOR, PTT)
            Next

            Return result.ToArray
        End Function

        <Extension>
        Private Iterator Function __fill(footprint As PredictedRegulationFootprint,
                                         site As MotifSiteHit,
                                         TFs As String(),
                                         anno As AnnotationModel,
                                         DOOR As DOOR,
                                         PTT As PTT) As IEnumerable(Of PredictedRegulationFootprint)

            For Each locus_tag As String In site.source
                Dim ORF_COPY As PredictedRegulationFootprint = footprint.Copy
                Dim gene = PTT(locus_tag)
                Dim motifSite As MotifScans.Site = anno(locus_tag)
                Dim loci As NucleotideLocation = motifSite.GetLoci(PTT)

                ORF_COPY.ORF = locus_tag
                ORF_COPY.Sequence = motifSite.Site
                ORF_COPY.ORFDirection = gene.Strand
                ORF_COPY.Signature = anno.Expression
                ORF_COPY.Starts = loci.Left
                ORF_COPY.Ends = loci.Right
                ORF_COPY.Strand = loci.Strand.GetBriefCode
                ORF_COPY.Distance = motifSite.GetDist(gene.Location.Strand)

                Dim dg = DOOR.GetGene(locus_tag)
                ORF_COPY.DoorId = dg.OperonID
                If String.Equals(DOOR.DOOROperonView.GetOperon(dg.OperonID).InitialX.Synonym, locus_tag, StringComparison.OrdinalIgnoreCase) Then
                    ORF_COPY.InitX = "1"c
                Else
                    ORF_COPY.InitX = "0"c
                End If

                If TFs.IsNullOrEmpty Then
                    Yield ORF_COPY
                Else
                    For Each TF As String In TFs
                        Dim anoCopy As PredictedRegulationFootprint = ORF_COPY.Copy
                        anoCopy.Regulator = TF
                        Yield anoCopy
                    Next
                End If
            Next
        End Function

        <ExportAPI("Compile.Batch")>
        Public Function BatchCompileDirectly(res As Dictionary(Of String, String)) As MotifSiteHit()
            Dim LQuery = (From x In res Select CompileDirectly(x.Key, x.Value)).IteratesALL
            Dim RegPrecise As TranscriptionFactors =
                GCModeller.FileSystem.RegPrecise.RegPreciseRegulations.LoadXml(Of TranscriptionFactors)
            LQuery = (From site As MotifSiteHit In LQuery Select RegPrecise.__fill(site)).IteratesALL.ToArray
            Return DirectCast(LQuery, MotifSiteHit())
        End Function

        <Extension>
        Private Iterator Function __fill(RegPrecise As TranscriptionFactors, site As MotifSiteHit) As IEnumerable(Of MotifSiteHit)
            Dim regulators As Regulator() = RegPrecise.GetRegulators(site.RegPrecise)

            If regulators.IsNullOrEmpty Then
                Call VBDebugger.Warning($"Unable retrieve motif site information for " & site.RegPrecise)
                site.Family = "*"
                Yield site
            Else
                For Each TF As Regulator In regulators
                    Dim copy As MotifSiteHit = site.Copy
                    copy.Regulators = {TF.LocusId}
                    copy.Family = TF.family

                    Yield copy
                Next
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MEME">meme.txt</param>
        ''' <param name="MAST_OUT">当前的这个meme.txt对Motif数据库的fasta序列的比对输出结果的文件夹</param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Compile.Single")>
        Public Function CompileSingle(MEME As String, MAST_OUT As String) As MatchResult
            Dim DIRs = FileIO.FileSystem.GetDirectories(MAST_OUT, FileIO.SearchOption.SearchTopLevelOnly)  ' 获取某个模块的顶层的文件夹列表，下面是Motif家族的文件夹列表
            Dim source As String = BaseName(MEME)
            Dim masts As MotifSiteHit() = (From DIR As String In DIRs
                                           Let mast As MAST = (DIR & "/mast.xml").LoadXml(Of MAST)(ThrowEx:=False)
                                           Where Not mast Is Nothing
                                           Select mast.MotifMatchCompile(trace:=source)).ToVector
            Dim MEMEDoc = MEME_TEXT.SafelyLoad(MEME)
            If masts.IsNullOrEmpty Then
                Return New MatchResult With {
                    .MEME = source
                }
            End If
            Dim mastHash = (From x As MotifSiteHit In masts
                            Select x
                            Group x By x.Trace.Split("."c).Last Into Group) _
                                 .ToDictionary(Function(x) x.Last,
                                               Function(x) x.Group.ToArray)
            Dim result As List(Of MotifHits) = New List(Of MotifHits)

            For Each motif As LDM.Motif In MEMEDoc
                If Not mastHash.ContainsKey(motif.Id) Then
                    Call Console.WriteLine("[MAST_NOT_FOUND] " & motif.ToString & vbCrLf)
                    Continue For  ' Motif不能匹配到数据库之中，则跳过这个Motif 
                End If

                Dim sites As MotifSiteHit() = mastHash(motif.Id)
                Dim hits As New MotifHits With {
                    .Evalue = motif.Evalue,
                    .MAST = sites,
                    .MEME = motif.Sites,
                    .Trace = source & "::" & motif.Id
                }

                Call result.Add(hits)
            Next

            Return New MatchResult With {
                .Matches = result.ToArray,
                .MEME = source
            }
        End Function

        <ExportAPI("Compile.MotifMatch")>
        <Extension> Public Function MotifMatchCompile(MAST As MAST, trace As String) As MotifSiteHit()
            If MAST.Sequences Is Nothing OrElse
                MAST.Sequences.SequenceList.IsNullOrEmpty Then
                Return New MotifSiteHit() {}
            End If

            Dim list As New List(Of MotifSiteHit)
            Dim Family As String =
                BaseName(MAST.Sequences.Databases.First.name)

            For Each x As SequenceDescript In MAST.Sequences.SequenceList
                If x.Segments.IsNullOrEmpty Then
                    Continue For
                End If

                Dim sites As MotifSiteHit() = x.Segments.Select(AddressOf __toSites).ToVector
                Dim RegPrecise As String = x.name.Split("|"c).First

                For Each site As MotifSiteHit In sites
                    site.Family = Family
                    site.Trace = trace & "." & site.Trace
                    site.RegPrecise = RegPrecise
                Next

                Call list.AddRange(sites)
            Next

            Return list.ToArray
        End Function

        Private Function __toSites(segment As Segment) As MotifSiteHit()
            Dim sites = segment.Hits.Select(
                Function(x) New MotifSiteHit With {
                    .Pvalue = x.pvalue,
                    .gStart = segment.start + x.pos,
                    .Trace = x.motif.Replace("motif_", ""),
                    .SequenceData = segment.SequenceData
            })
            Return sites
        End Function

        <ExportAPI("Compile.Batch")>
        Public Function BatchCompile(res As Dictionary(Of String, String)) As FootprintTrace
            If res.Count < 45 Then
                Return New FootprintTrace With {
                    .Footprints = res.__parallelBatch
                }
            Else  ' 数据太多了，为了提高效率，必须要分区进行处理
                Dim blocks = res.Split(30)
                Dim list As New List(Of MatchResult)
                Dim dl As EventProc = blocks.LinqProc

                Call $"There are {blocks.Length} data buffer objects...".__DEBUG_ECHO

                For Each block In blocks
                    list += block.__parallelBatch
                    Call dl.Tick()
                Next

                Return New FootprintTrace With {
                    .Footprints = list.ToArray
                }
            End If
        End Function

        <Extension>
        Private Function __parallelBatch(res As IEnumerable(Of KeyValuePair(Of String, String))) As MatchResult()
            Dim LQuery = (From x As KeyValuePair(Of String, String)
                          In res.AsParallel  ' 请注意，在这里已经使用了并行化了，则下面的栈之中都不再推荐使用并行化
                          Select CompileSingle(x.Key, x.Value)).ToArray
            Return LQuery
        End Function
    End Module
End Namespace
