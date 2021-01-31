#Region "Microsoft.VisualBasic::0dccf512a7127c2d8bd3878e7c656e1c, CLI_tools\RegPrecise\CLI\OperonBuilder.vb"

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
    '     Function: __getStrand, __operon, MergeDOOR, OperonBuilder, RegulonBatchBuilder
    ' 
    '     Sub: __scanOperon
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Parallel
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports gene = SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief

Partial Module CLI

    <ExportAPI("/DOOR.Merge",
               Usage:="/DOOR.Merge /in <operon.csv> /DOOR <genome.opr> [/out <out.opr>]")>
    Public Function MergeDOOR(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim opr As String = args - "/DOOR"
        Dim out As String = ("/out" <= args) ^ $"{[in].TrimSuffix}-{opr.BaseName}.opr"
        Dim operons As List(Of RegPreciseOperon) = LinqAPI.MakeList(Of RegPreciseOperon) <=
            From x As RegPreciseOperon
            In [in].LoadCsv(Of RegPreciseOperon)
            Select x
            Order By x.Operon.Length Descending

        Dim DOOR As DOOR = DOOR_API.Load(opr)

        ' 求交集
        Dim its As New List(Of RegPreciseOperon())

        Do While True
            Dim temp As New List(Of RegPreciseOperon)
            Dim first = operons.First

            For Each x As RegPreciseOperon In operons
                If Not first.Operon.Intersect(x.Operon).ToArray.IsNullOrEmpty Then
                    temp += x
                End If
            Next

            For Each x In temp
                operons.Remove(x)
            Next

            its += temp.ToArray

            If operons.Count = 0 Then
                Exit Do
            Else
                Call Console.Write(".")
            End If
        Loop

        Dim ooo = (From x As SeqValue(Of RegPreciseOperon())
                   In its.SeqIterator
                   Select x.i,
                       x.value.Select(Function(xx) xx.Operon).IteratesALL.Distinct.ToArray,
                       TF = x.value.Select(Function(xx) xx.Regulators).IteratesALL.Distinct.ToArray).ToArray

        Call ooo.SaveTo(out.TrimSuffix & ".Csv")

        Return 0
    End Function

    <ExportAPI("/Build.Regulons.Batch",
               Usage:="/Build.Regulons.Batch /bbh <bbh.DIR> /PTT <PTT.DIR> /tf-bbh <tf-bbh.DIR> /regprecise <regprecise.Xml> [/num_threads <-1> /hits_hash /out <outDIR>]")>
    Public Function RegulonBatchBuilder(args As CommandLine) As Integer
        Dim bbhDIR As String = args("/bbh")
        Dim PTT_DIR As String = args("/ptt")
        Dim tfDIR As String = args("/tf-bbh")
        Dim regprecise As String = args("/regprecise")
        Dim hits_hash As String = If(args.GetBoolean("/hits_hash"), "/tfHit_hash", "")
        Dim out As String = args.GetValue("/out", bbhDIR & "-RegPrecise.regulons/")
        Dim n As Integer = args.GetValue("/num_threads", -1)

        If n < 0 Then
            n = LQuerySchedule.CPU_NUMBER / 2
        ElseIf n = 0 Then
            n = 1
        End If

        Dim api As String = GetType(CLI).API(NameOf(OperonBuilder))
        Dim bbh As IEnumerable(Of String) = ls - l - r - wildcards("*.csv") <= bbhDIR
        Dim PTT As IEnumerable(Of String) = ls - l - r - wildcards("*.PTT") <= PTT_DIR
        Dim TFs As IEnumerable(Of String) = ls - l - r - wildcards("*.Csv") <= tfDIR
        Dim pairs = Path.Extensions.Pairs(New NamedValue(Of String())(NameOf(PTT), PTT.ToArray),
                                      New NamedValue(Of String())(NameOf(bbh), bbh.ToArray),
                                      New NamedValue(Of String())(NameOf(TFs), TFs.ToArray))
        Dim task As Func(Of Dictionary(Of String, String), String) =
            Function(source) _
                $"{api} /bbh {source(NameOf(bbh)).CLIPath} /PTT {source(NameOf(PTT)).CLIPath} /TF-bbh {source(NameOf(TFs)).CLIPath} /out {(out & source(NameOf(PTT)).BaseName & ".regulons.csv").CLIPath} /regprecise {regprecise.CLIPath}"
        Dim CLI As String() = pairs.Select(task).ToArray

        Return BatchTasks.SelfFolks(CLI, n)
    End Function

    ''' <summary>
    ''' 得到的只能是一个受共同调控因子调控的基因簇，可能和操纵子不太一样，所以还是使用DOOR的结果
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Build.Operons", Info:="If the /regprecise parameter is not presented, then you should install the regprecise in the GCModeller database repostiory first.",
               Usage:="/Build.Operons /bbh <bbh.csv> /PTT <genome.PTT> /TF-bbh <bbh.csv> [/tfHit_hash /out <out.csv> /regprecise <regprecise.Xml>]")>
    <ArgumentAttribute("/bbh", True,
                   Description:="The bbh result between the annotated genome And RegPrecise database. 
                   This result was used for generates the operons, and query should be the genes in 
                   the RegPrecise database and the hits is the genes in your annotated genome.")>
    Public Function OperonBuilder(args As CommandLine) As Integer
        Dim bbh As String = args - "/bbh"
        Dim PTT As PTT = TabularFormat.PTT.Load(args - "/PTT")
        Dim out As String = ("/out" <= args) ^ (bbh.TrimSuffix & "-" & (args - "/PTT").BaseName & ".Operons.Csv")
        Dim reg As String = ("/regprecise" <= args) ^ (GCModeller.FileSystem.RegPrecise.RegPreciseRegulations)
        Dim tfBBH As String = args - "/TF-bbh"
        Dim RegPrecise As TranscriptionFactors = reg.LoadXml(Of TranscriptionFactors)
        Dim plus As New DefaultHashHandle(Of gene)(From x As gene
                                                   In PTT.GetStrandGene(Strands.Forward)
                                                   Select x
                                                   Order By x.Location.Normalization.left Ascending)
        Dim minus As New DefaultHashHandle(Of gene)(From x As gene
                                                    In PTT.GetStrandGene(Strands.Reverse)
                                                    Select x
                                                    Order By x.Location.Normalization.right Descending)
        Dim hitsHash = (From x As BBHIndex
                        In bbh.LoadCsv(Of BBHIndex)
                        Where x.isMatched
                        Select x
                        Group x By x.QueryName.Split(":"c).Last Into Group) _
                             .ToDictionary(Function(x) x.Last,
                                           Function(x)
                                               Return x.Group.Select(Function(xx) xx.HitName)
                                           End Function)
        Dim tfHash As Dictionary(Of String, String()) =
            BBHIndex.BuildHitsTable(tfBBH.LoadCsv(Of BBHIndex), args.GetBoolean("/tfHit_hash"))
        Dim result As New List(Of RegPreciseOperon)

        For Each genome As BacteriaRegulome In RegPrecise.genomes
            For Each regulon As Regulator In genome.regulome.regulators
                Dim TF As String() = If(tfHash.ContainsKey(regulon.LocusId), tfHash(regulon.LocusId), Nothing)
                Dim isRNA As Boolean = regulon.type = Types.RNA

                For Each opr In regulon.operons
                    Dim oHits As New Dictionary(Of String, String())   ' {RegPrecise -> bbh}

                    For Each m As RegulatedGene In opr.members
                        If Not hitsHash.ContainsKey(m.locusId) Then
                            If Not oHits.ContainsKey(m.locusId) Then
                                oHits.Add(m.locusId, {})
                            End If
                        Else
                            oHits.Add(m.locusId, hitsHash(m.locusId))
                        End If
                    Next

                    Dim source = regulon.__operon(oHits, TF, plus, minus).SafeQuery

                    If isRNA Then
                        Dim setValue = New SetValue(Of RegPreciseOperon) <= NameOf(RegPreciseOperon.Regulators)

                        result += From x As RegPreciseOperon
                                  In source
                                  Where Not x.Operon.IsNullOrEmpty
                                  Select setValue(x, {regulon.family})
                    Else
                        Dim setValue = New SetValue(Of RegPreciseOperon) <= NameOf(RegPreciseOperon.Operon)

                        result += From x As RegPreciseOperon
                                  In source
                                  Where Not x.Operon.IsNullOrEmpty
                                  Select setValue(x, x.Operon.Distinct.ToArray)
                    End If
                Next

                Call Console.Write("-")
            Next

            Call genome.genome.GetJson.__DEBUG_ECHO
        Next

        Return result > out
    End Function

    <Extension>
    Private Function __getStrand(id As String,
                                 plus As DefaultHashHandle(Of gene),
                                 minus As DefaultHashHandle(Of gene)) As Strands
        If plus.HasElement(id) Then
            Return Strands.Forward
        ElseIf minus.HasElement(id) Then
            Return Strands.Reverse
        Else
            Return Strands.Unknown
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="regulon"></param>
    ''' <param name="members">{RegPrecise -> bbh}</param>
    ''' <param name="TF"></param>
    ''' <param name="plus"></param>
    ''' <param name="minus"></param>
    ''' <returns></returns>
    <Extension>
    Private Function __operon(regulon As Regulator,
                              members As Dictionary(Of String, String()),
                              TF As String(),
                              plus As DefaultHashHandle(Of gene),
                              minus As DefaultHashHandle(Of gene)) As RegPreciseOperon()

        If members.Count = 0 Then
            Return Nothing
        End If

        If members.Count = 1 Then
            Return members.Values.First.Select(
                Function(sid) New RegPreciseOperon(regulon,
                                                   TF,
                                                   {sid},
                                                   sid.__getStrand(plus, minus),
                                                   {members.First.Key}))
        End If

        Dim hits As String() = members.Keys.ToArray
        Dim locus As New List(Of String)(members.Values.IteratesALL.Distinct)
        Dim forwards = LinqAPI.MakeList(Of LinkNode(Of IHashValue(Of gene))) <=
            From x As String
            In locus
            Where plus.HasElement(x)
            Select gg = plus.Current(x)
            Order By gg.node.obj.Location.left Ascending

        Dim reversed = LinqAPI.MakeList(Of LinkNode(Of IHashValue(Of gene))) <=
            From x As String
            In locus
            Where minus.HasElement(x)
            Select gg = minus.Current(x)
            Order By gg.node.obj.Location.right Descending

        Dim n As Integer = (From x
                            In members
                            Where Not x.Value.IsNullOrEmpty
                            Select x).Count ' 原始的操纵子的基因的数量

        Dim result As New List(Of RegPreciseOperon)

        Call regulon.__scanOperon(forwards, n, "+", locus, result, TF, hits)
        Call regulon.__scanOperon(reversed, n, "-", locus, result, TF, hits)

        Return result.ToArray
    End Function

    ''' <summary>
    ''' 由于反向链上面的基因是反向排序的，在基因组上面的扫描构建的过程已经变得和正向链的基因一样了
    ''' </summary>
    ''' <param name="source">基因组上下文</param>
    ''' <param name="n">这个保守的操纵子之中的结构基因的数量</param>
    ''' <param name="strand">链的方向</param>
    ''' <param name="locus">bbh数据库比对结果之中的得到的在目标基因组之中的匹配记录</param>
    ''' <param name="result">所构建的操纵子列表</param>
    ''' 
    <Extension>
    Private Sub __scanOperon(regulon As Regulator,
                             source As List(Of LinkNode(Of IHashValue(Of gene))),
                             n As Integer,
                             strand As String,
                             locus As List(Of String),
                             ByRef result As List(Of RegPreciseOperon),
                             TF As String(),
                             hits As String())

        Dim g As LinkNode(Of IHashValue(Of gene))
        Dim gl As New List(Of String)

        Do While source.Count > 0
            gl.Clear()
            g = source.First

            Do While True
                Dim rm = (From x In source Where String.Equals(x.node.ID, g.node.ID) Select x).FirstOrDefault
                If Not rm Is Nothing Then '  由于Next元素是新构建出来的，指针的位置不对，所以不能够直接使用remove方法移除
                    Call source.Remove(rm)
                End If
                gl += g.node.ID
                g = g.Next
                If g.node Is Nothing Then  ' 到头了，已经没有任何元素了
                    Exit Do
                End If
                If -1 = locus.IndexOf(g.node.ID) Then  ' 在列表里面不存在，则可能是中间的某一个元素由于进化较远bbh没有比对上，这里是一个缺口，
                    g = g.Next                                 ' 但是后面可能还存在基因的， 试着比较一下下一个基因是否存在
                    If g Is Nothing OrElse g.node Is Nothing Then
                        Exit Do
                    End If
                    If -1 = locus.IndexOf(g.node.ID) Then
                        Exit Do
                    Else
                        gl += g.Previous.node.ID
                    End If
                End If
            Loop

            If gl.Count > 1 OrElse gl.Count >= n Then
                result += New RegPreciseOperon(regulon, TF, gl.ToArray, strand, hits)
            End If
        Loop
    End Sub
End Module
