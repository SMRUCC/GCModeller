#Region "Microsoft.VisualBasic::ac693d168d77eb10494f504cff586195, CLI_tools\RegPrecise\CLI\CORN.vb"

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

' Module CLI
' 
'     Function: (+2 Overloads) __CORNsiteThread, __logs, CORN, CORNBatch, CORNSingleThread
'               MergeCORN, Supports
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Text
Imports Parallel.ThreadTask
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Data.Regprecise

Partial Module CLI

    ''' <summary>
    ''' Cluster of co-regulated orthologous operons.(假若服务器的内存和性能足够强大，可以直接使用这个函数进行比较，这个函数可能会准确性比较好些)
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("/CORN",
               Info:="Join two vertices by edge if the correspondent operons: 
               i) are orthologous; 
               ii) have cantiodate transcription factor binding sites. 
               Collect all linked components. Two operons from two different genomes are called orthologous if they share at least one orthologous gene.",
               Usage:="/CORN /in <regulons.DIR> /motif-sites <motiflogs.csv.DIR> /sites <motiflogs.csv> /ref <regulons.Csv> [/out <out.csv>]")>
    Public Function CORN(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim sites As String = args("/sites")
        Dim out As String = args.GetValue("/out", sites.TrimSuffix & ".CORN.Csv")
        Dim motifSites As IEnumerable(Of MotifLog) = sites.LoadCsv(Of MotifLog)
        Dim ref As String = args("/ref")
        Dim refName As String = ref.BaseName
        Dim regulons As IEnumerable(Of String) = From path As String
                                                 In ls - l - r - wildcards("*.Csv") <= [in]
                                                 Where Not String.Equals(refName, path.BaseName)
                                                 Select path

        ' {genome, {regulog, regulons()}}
        Dim operons As Dictionary(Of String, Dictionary(Of String, RegPreciseOperon())) =
            (From data As NamedValue(Of RegPreciseOperon())
             In BatchQueue.ReadQueue(Of RegPreciseOperon)(regulons)
             Let name As String = data.Name
             Let buf As RegPreciseOperon() = data.Value
             Let datahash As Dictionary(Of String, RegPreciseOperon()) = (From x As RegPreciseOperon
                                                                          In buf
                                                                          Select x
                                                                          Group x By x.source Into Group) _
                                                                               .ToDictionary(Function(x) x.source,
                                                                                             Function(x) x.Group.ToArray)
             Select name,
                 datahash).ToDictionary(Function(x) x.name,
                                        Function(x) x.datahash)

        Dim refHash As Dictionary(Of String, RegPreciseOperon()) = (From operon As RegPreciseOperon
                                                                    In ref.LoadCsv(Of RegPreciseOperon)
                                                                    Select operon
                                                                    Group operon By operon.source Into Group) _
                                                                         .ToDictionary(Function(x) x.source,
                                                                                       Function(x) x.Group.ToArray)

        refName = sites.BaseName

        Dim masts As IEnumerable(Of String) = From path As String
                                              In ls - l - r - wildcards("*.csv") <= args("/motif-sites")
                                              Where Not String.Equals(refName, path.BaseName)
                                              Select path

        ' {fileName, {regulog, {ORF, sites}}}
        Dim mastLogs As Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, MotifLog()))) =
            (From file As NamedValue(Of MotifLog())
             In BatchQueue.ReadQueue(Of MotifLog)(masts)
             Let name As String = file.Name
             Let data As IEnumerable(Of MotifLog) = file.Value
             Let datahash As Dictionary(Of String, Dictionary(Of String, MotifLog())) =
                 (From x As MotifLog
                  In data
                  Select x
                  Group x By x.Regulog Into Group) _
                       .ToDictionary(Function(x) x.Regulog,
                                     Function(x) (From site As MotifLog
                                                  In x.Group
                                                  Select site
                                                  Group site By site.ID Into Group) _
                                                       .ToDictionary(Function(g) g.ID,
                                                                     Function(g) g.Group.ToArray))
             Select name,
                 datahash) _
                .ToDictionary(Function(x) x.name,
                              Function(x) x.datahash)

        Dim matched As MotifLog() =
            LinqAPI.Exec(Of MotifLog) <= From site As MotifLog
                                         In motifSites.AsParallel
                                         Let testSite As MotifLog =
                                             site.__CORNsiteThread(refHash, mastLogs, operons)
                                         Where Not testSite Is Nothing
                                         Select testSite
        Return matched.SaveTo(out).CLICode
    End Function

    <ExportAPI("/CORN.Batch",
               Usage:="/CORN.Batch /sites <motiflogs.gff.sites.Csv.DIR> /regulons <regprecise.regulons.csv.DIR> [/name <name> /out <outDIR> /num_threads <-1> /null-regprecise]")>
    <ArgumentAttribute("/name", True, AcceptTypes:={GetType(String)},
                   Description:="")>
    <ArgumentAttribute("/sites", False, AcceptTypes:={GetType(MotifLog)})>
    <ArgumentAttribute("/regulons", False, AcceptTypes:={GetType(RegPreciseOperon)})>
    Public Function CORNBatch(args As CommandLine) As Integer
        Dim sitesDIR As String = args("/sites")
        Dim regulonsDIR As String = args("/regulons")
        Dim outDIR As String = args.GetValue("/out", sitesDIR.TrimDIR & "-" & regulonsDIR.BaseName & ".CORN/")
        Dim name As String = args("/name")
        Dim n As Integer =
            LQuerySchedule.AutoConfig(args.GetValue("/num_threads", -1))

        Dim sites As Dictionary(Of String, String) =
            (ls - l - r - wildcards("*.csv") <= sitesDIR).ToDictionary(AddressOf BaseName)
        Dim regulons As Dictionary(Of String, String) =
            (ls - l - r - wildcards("*.csv") <= regulonsDIR).ToDictionary(AddressOf BaseName)

        Dim build As Func(Of String, String()) =
            Function(fileBaseName) As String()
                Dim self As String = sites(fileBaseName)
                Dim selfRegulon As String = regulons(fileBaseName)
                Dim list As New List(Of String)

                For Each genome In sites
                    If String.Equals(genome.Key, fileBaseName) Then
                        Continue For
                    End If

                    Dim regulon As String = regulons(genome.Key)
                    Dim sitesHits As String = genome.Value
                    Dim out As String = (outDIR & $"/{fileBaseName}/{fileBaseName}-{sitesHits.BaseName}.csv").CLIPath

                    list +=
                    $"{GetType(CLI).API(NameOf(CORNSingleThread))} /hit {regulon.CLIPath} /hit-sites {sitesHits.CLIPath} /sites {self.CLIPath} /ref {selfRegulon.CLIPath} /out {out}"
                Next

                Return list
            End Function

        Dim CLI As String()

        If String.IsNullOrEmpty(name) Then
            CLI = (ls - l - r - wildcards("*.csv") <= sitesDIR) _
                .Select(AddressOf BaseName) _
                .Select(build) _
                .ToVector
        Else
            CLI = build(name)
        End If

        If args.GetBoolean("/null-regprecise") Then
            For i As Integer = 0 To CLI.Length - 1
                CLI(i) = CLI(i) & " /null-regprecise"
            Next
        End If

        Return BatchTasks.SelfFolks(CLI, n)
    End Function

    ''' <summary>
    ''' 单条基因组上下文模型的比对线程
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/CORN.thread",
               Usage:="/CORN.thread /hit <regulons.Csv> /hit-sites <motiflogs.csv> /sites <query.motiflogs.csv> /ref <query.regulons.Csv> [/null-regprecise /out <out.csv>]")>
    <ArgumentAttribute("/null-regprecise",
                   True,
                   AcceptTypes:={GetType(Boolean)},
                   Description:="Does the motif log data have the RegPrecise database value? If this parameter is presented that which it means the site data have no RegPrecise data.")>
    <ArgumentAttribute("/hit", False, AcceptTypes:={GetType(RegPreciseOperon)})>
    <ArgumentAttribute("/hit-sites", False, AcceptTypes:={GetType(MotifLog)})>
    <ArgumentAttribute("/sites", False, AcceptTypes:={GetType(MotifLog)})>
    <ArgumentAttribute("/ref", False, AcceptTypes:={GetType(RegPreciseOperon)})>
    <ArgumentAttribute("/out", True, AcceptTypes:={GetType(MotifLog)})>
    Public Function CORNSingleThread(args As CommandLine) As Integer
        Dim hit As String = args("/hit")
        Dim sites As String = args("/sites")
        Dim hitSites As String = args("/hit-sites")
        Dim out As String = args.GetValue("/out", sites.TrimSuffix & $"-{hitSites.BaseName}.CORN.Csv")
        Dim motifSites As IEnumerable(Of MotifLog) = sites.LoadCsv(Of MotifLog)
        Dim ref As String = args("/ref")
        Dim nul As Boolean = args.GetBoolean("/null-regprecise")

        ' {regulog, regulons()}
        Dim operons As Dictionary(Of String, RegPreciseOperon()) =
            (From x As RegPreciseOperon
             In hit.LoadCsv(Of RegPreciseOperon).AsParallel
             Select x
             Where Not x Is Nothing AndAlso
                 Not String.IsNullOrEmpty(x.source)
             Group x By x.source Into Group) _
                 .ToDictionary(Function(x) x.source,
                           Function(x) x.Group.ToArray)
        Dim refHash As Dictionary(Of String, RegPreciseOperon()) =
            (From operon As RegPreciseOperon
             In ref.LoadCsv(Of RegPreciseOperon)
             Select operon
             Where Not operon Is Nothing AndAlso
                 Not String.IsNullOrEmpty(operon.source)
             Group operon By operon.source Into Group) _
                  .ToDictionary(Function(x) x.source,
                                Function(x) x.Group.ToArray)
        ' {regulog, {ORF, sites}}
        Dim mastLogs As Dictionary(Of String, Dictionary(Of String, MotifLog())) =
            (From x As MotifLog
             In hitSites.LoadCsv(Of MotifLog)
             Select x
             Group x By x.Regulog Into Group) _
                       .ToDictionary(Function(x) x.Regulog,
                                     Function(x) (From site As MotifLog
                                                  In x.Group
                                                  Select site
                                                  Group site By site.ID Into Group) _
                                                       .ToDictionary(Function(g) g.ID,
                                                                     Function(g) g.Group.ToArray))
        Dim matched As MotifLog() =
            LinqAPI.Exec(Of MotifLog) <= From site As MotifLog
                                         In motifSites' .AsParallel
                                         Let testSite As MotifLog =
                                             site.__CORNsiteThread(refHash, mastLogs, operons)
                                         Where Not testSite Is Nothing
                                         Select testSite
        Return matched.SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 成立，会返回位点，不成立，则会返回空值
    ''' </summary>
    ''' <param name="site"></param>
    ''' <returns></returns>
    ''' <see cref="MotifLog.Regulog"/>
    <Extension>
    Private Function __CORNsiteThread(site As MotifLog,
                                      refHash As Dictionary(Of String, RegPreciseOperon()),
                                      mastLogs As Dictionary(Of String, Dictionary(Of String, MotifLog())),
                                      operons As Dictionary(Of String, RegPreciseOperon())) As MotifLog
        Dim ORF As String = site.ID
        Dim uid As String = site.Regulog

        If Not refHash.ContainsKey(uid) Then
            Return Nothing
        End If

        Dim candidates As RegPreciseOperon() = refHash(uid)

        For Each operon As RegPreciseOperon In From x As RegPreciseOperon
                                               In candidates
                                               Where Array.IndexOf(x.Operon, ORF) > -1
                                               Select x

            ' 只要能够在候选的操纵子之中找到至少两个有这个位点的基因组，则这个位点就很有可能存在
            Dim bbh As String = operon.bbhUID

            If Not mastLogs.ContainsKey(uid) Then  ' 这个基因组之中没有这个operon
                Continue For
            End If
            If Not operons.ContainsKey(uid) Then
                Continue For
            End If

            Dim ORFs As Dictionary(Of String, MotifLog()) = mastLogs(uid) ' 这个基因组之中具有这个调控位点的候选基因列表
            Dim hits As RegPreciseOperon() = operons(uid) ' operon列表
            Dim LQuery As IEnumerable(Of RegPreciseOperon) =
                LinqAPI.Exec(Of RegPreciseOperon) <= From gene As String
                                                     In ORFs.Keys
                                                     Select From r As RegPreciseOperon
                                                            In hits
                                                            Where Array.IndexOf(r.Operon, gene) > -1
                                                            Select r ' 得到包含有这个基因的所有的操纵子列表
            For Each candRef As RegPreciseOperon In LQuery
                If String.Equals(bbh, candRef.bbhUID, StringComparison.OrdinalIgnoreCase) Then
                    ' 已经满足条件了，将这个候选位点加入到结果列表之中
                    Call Console.Write(".")
                    Return site
                End If
            Next
        Next

        Return Nothing
    End Function

    ''' <summary>
    ''' 成立，会返回位点，不成立，则会返回空值
    ''' </summary>
    ''' <param name="site"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Private Function __CORNsiteThread(site As MotifLog,
                                      refHash As Dictionary(Of String, RegPreciseOperon()),
                                      mastLogs As Dictionary(Of String, Dictionary(Of String, Dictionary(Of String, MotifLog()))),
                                      operons As Dictionary(Of String, Dictionary(Of String, RegPreciseOperon()))) As MotifLog
        Dim ORF As String = site.ID
        Dim uid As String = site.Regulog

        If Not refHash.ContainsKey(uid) Then
            Return Nothing
        End If

        Dim candidates As RegPreciseOperon() = refHash(uid)

        For Each operon As RegPreciseOperon In (From x In candidates Where Array.IndexOf(x.Operon, ORF) > -1 Select x)
            ' 只要能够在候选的操纵子之中找到至少两个有这个位点的基因组，则这个位点就很有可能存在
            Dim n As Integer = 2
            Dim bbh As String = operon.bbhUID

            For Each genome In mastLogs
                If Not genome.Value.ContainsKey(uid) Then  ' 这个基因组之中没有这个operon
                    Continue For
                End If
                If Not operons(genome.Key).ContainsKey(uid) Then
                    Continue For
                End If

                Dim ORFs As Dictionary(Of String, MotifLog()) = genome.Value(uid) ' 这个基因组之中具有这个调控位点的候选基因列表
                Dim hits As RegPreciseOperon() = operons(genome.Key)(uid) ' operon列表
                Dim LQuery As IEnumerable(Of RegPreciseOperon) = (From gene As String
                                                                  In ORFs.Keys
                                                                  Select From r As RegPreciseOperon
                                                                         In hits
                                                                         Where Array.IndexOf(r.Operon, gene) > -1
                                                                         Select r).IteratesALL   ' 得到包含有这个基因的所有的操纵子列表

                For Each candRef As RegPreciseOperon In LQuery
                    If String.Equals(bbh, candRef.bbhUID, StringComparison.OrdinalIgnoreCase) Then
                        n -= 1
                        Exit For
                    End If
                Next

                If n = 0 Then
                    ' 已经满足条件了，将这个候选位点加入到结果列表之中
                    Call Console.Write(".")
                    Return site
                End If
            Next

            If n = 0 Then
                Return site
            End If
        Next

        Return Nothing
    End Function

    <ExportAPI("/Merge.CORN",
               Usage:="/Merge.CORN /in <inDIR> [/out <outDIR>]")>
    Public Function MergeCORN(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim outDIR As String = args.GetValue("/out", inDIR.TrimDIR & ".CORNs/")
        Dim DIRs As IEnumerable(Of String) = ls - l - lsDIR <= inDIR

        For Each DIR As String In DIRs
            Dim files As IEnumerable(Of String) = ls - l - r - wildcards("*.csv") <= DIR
            Dim list As New List(Of MotifLog)
            Dim out As String = outDIR & "/" & DIR.BaseName & ".Csv"

            For Each file As String In files
                list += file.LoadCsv(Of MotifLog).ToArray
            Next

            Dim Groups = From x As MotifLog
                         In list
                         Let uid As String = $"{x.Regulog}-{x.Start}.{x.Ends}.{x.Strand}"
                         Select x,
                             uid
                         Group By uid Into Group
            Dim supports = (From x In Groups
                            Let data = x.Group.ToArray
                            Let o As MotifLog = data(Scan0).x
                            Select support = data.Length,
                                o.ATGDist,
                                o.BiologicalProcess,
                                o.Complement,
                                o.Ends,
                                o.Family,
                                o.ID,
                                o.InPromoterRegion,
                                o.Location,
                                o.Regulog,
                                o.SequenceData,
                                o.Start,
                                o.Strand,
                                o.tag,
                                o.Taxonomy).ToArray

            Call supports.SaveTo(out.TrimSuffix & ".supports.Csv")
            Call list.SaveTo(out)
        Next

        Return 0
    End Function

    ''' <summary>
    ''' 假设文件名都是基因组的名称
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/heap.Supports",
               Usage:="/heap.supports /in <inDIR> [/out <out.Csv> /T /l]")>
    Public Function Supports(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR.TrimDIR & ".heapmap.csv")
        Dim files = From path As String
                    In ls - l - r - wildcards("*.csv") <= inDIR
                    Select path.BaseName,
                        df = path.LoadCsv(Of MotifLog)
        Dim [long] As Boolean = args.GetBoolean("/l")
        Dim data = (From x In files
                    Select From log As MotifLog
                           In x.df
                           Select x.BaseName,
                               __logs = log.__logs([long]),
                               support = log.tags("support")).IteratesALL
        Dim GroupLogs = (From x In data
                         Select x
                         Group x By x.__logs Into Group).ToArray
        Dim outDf As EntityObject() =
            LinqAPI.Exec(Of EntityObject) <= From log
                                             In GroupLogs
                                             Let id As String = log.__logs
                                             Let df As Dictionary(Of String, String) =
                                                 (From x
                                                  In log.Group  ' 原来的数据是使用位置来进行support数的计算的，在这里只是查看regulog和物种的关系，所以这里不需要位置了，故而有些regulog是重复的，将他们相加在一起就好了
                                                  Select x
                                                  Group x By x.BaseName Into Group).ToDictionary(
                                                    Function(x) x.BaseName,
                                                    Function(x) x.Group.Sum(Function(o) Val(o.support)).ToString)
                                             Select New EntityObject With {
                                                 .ID = id,
                                                 .Properties = df
                                             }
        If args.GetBoolean("/T") Then
            Call outDf.SaveTo(out, metaBlank:="0")

            Dim df As IO.File = IO.File.Load(out)
            df = df.Transpose
            Return df.Save(out, Encodings.ASCII)
        Else
            Return outDf.SaveTo(out, metaBlank:="0").CLICode
        End If
    End Function

    <Extension>
    Private Function __logs(x As MotifLog, [long] As Boolean) As String
        If [long] Then
            Return $"[{x.BiologicalProcess}]{x.Regulog}"
        Else
            Return x.Regulog
        End If
    End Function
End Module
