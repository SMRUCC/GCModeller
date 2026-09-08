Imports System.Diagnostics
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.RetroPath
Imports SMRUCC.genomics.Analysis.RetroPath.Chem
Imports SMRUCC.genomics.Analysis.RetroPath.Model
Imports SMRUCC.genomics.Analysis.RetroPath.Search
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc

''' <summary>
''' 把 BioCyc（MetaCyc/EcoCyc）PGDB 装配成 RetroPath 的逆向合成通路搜索器。
'''
''' 构造期一次性完成：
'''   1) 化合物结构索引（SMILES 净化 + 元素白名单 + 解析）；
'''   2) 反应实例 → 广义反应规则（MCS 原子映射 + 反应中心提取 + SMARTS 泛化）；
'''   3) 底盘内源代谢物汇集合。
''' 之后每次 <see cref="FindPathway"/> 只是对已装配的 Netwalk 发起一次搜索。
''' </summary>
Public Class BioCycAdapter

    ReadOnly netwalk As Netwalk
    ReadOnly opts As SearchOptions
    ReadOnly w As ScoreWeights
    ReadOnly ruleList As New List(Of Rule)()
    ReadOnly structures As New Dictionary(Of String, CompoundStructure)()
    ''' <summary>汇条目：(分子指纹, compound id, 净化后的 SMILES)</summary>
    ReadOnly sinkEntries As New List(Of (key As String, id As String, smiles As String))()

    ''' <summary>底物/产物在反应中的出现次数（用于判定枢纽代谢物）</summary>
    ReadOnly degrees As New Dictionary(Of String, Integer)()

    ''' <summary>current sink mode</summary>
    Public ReadOnly Property SinkMode As SinkModes
    ''' <summary>规则挖掘过程中被跳过的条目及原因计数</summary>
    Public ReadOnly Property Skipped As New Dictionary(Of String, Integer)()
    ''' <summary>逐条反应的跳过原因（reaction uniqueId → 原因），用于排查某条反应为何未进规则库</summary>
    Public ReadOnly Property RuleTrace As New Dictionary(Of String, String)()
    ''' <summary>装配统计信息（化合物总数 / 可用结构数 / 规则数 / 汇大小 / 耗时）</summary>
    Public ReadOnly Property Stats As New Dictionary(Of String, String)()

    ''' <summary>挖掘得到的广义反应规则集</summary>
    Public ReadOnly Property Rules As List(Of Rule)
        Get
            Return ruleList
        End Get
    End Property

    ''' <summary>compound frame id → 结构（净化后的 SMILES + 分子图）</summary>
    Public ReadOnly Property Compounds As Dictionary(Of String, CompoundStructure)
        Get
            Return structures
        End Get
    End Property

    ''' <summary>
    ''' 从 BioCyc 工作目录装配搜索器。
    ''' </summary>
    ''' <param name="biocyc">BioCyc 数据库工作区（如 Workspace.Open("F:\ecoli\29.0")）</param>
    ''' <param name="opts">束搜索参数（默认 beam / width 50 / depth 6）</param>
    ''' <param name="w">路径评分权重（默认 0.4 热力学 / 0.3 酶可得性 / 0.3 长度）</param>
    ''' <param name="sinkMode">汇集合模式：Core = 核心中心代谢子集；All = 全库</param>
    ''' <param name="coreDegree">Core 模式下判定枢纽代谢物的最少反应出现次数</param>
    ''' <param name="maxMoleculeAtoms">参与反应的分子重原子数上限（超出则跳过该反应，控耗时）</param>
    ''' <param name="maxPatternAtoms">模式原子数上限（超出则规则过特异，跳过）</param>
    ''' <param name="mcsNodeBudget">MCS 原子映射的回溯节点预算</param>
    ''' <param name="includeBuiltinRules">是否叠加 RetroPath 内置的 9 条广义规则</param>
    ''' <param name="verbose">是否向 stderr 输出装配诊断</param>
    Sub New(biocyc As Workspace,
            Optional opts As SearchOptions = Nothing,
            Optional w As ScoreWeights = Nothing,
            Optional sinkMode As SinkModes = SinkModes.Core,
            Optional coreDegree As Integer = 4,
            Optional maxMoleculeAtoms As Integer = 80,
            Optional maxPatternAtoms As Integer = 32,
            Optional mcsNodeBudget As Integer = 60000,
            Optional maxUnmappedAtoms As Integer = 3,
            Optional shellRadius As Integer = 2,
            Optional includeBuiltinRules As Boolean = False,
            Optional verbose As Boolean = True,
            Optional keepRuleTrace As Boolean = False)

        Dim sw As Stopwatch = Stopwatch.StartNew()

        Dim compounds As compounds() = biocyc.compounds.AsEnumerable.ToArray
        Dim reactions As reactions() = biocyc.reactions.AsEnumerable.ToArray

        Me.opts = If(opts, New SearchOptions)
        Me.w = If(w, New ScoreWeights)
        Me.SinkMode = sinkMode

        ' ---------- 1) 化合物结构索引 ----------
        Dim noSmiles As Integer = 0
        Dim badSmiles As Integer = 0
        Dim rejectReasons As New Dictionary(Of String, Integer)()
        Dim rejectSamples As New Dictionary(Of String, String)()

        For Each cpd As compounds In compounds
            If cpd Is Nothing OrElse String.IsNullOrEmpty(cpd.uniqueId) Then Continue For
            If String.IsNullOrEmpty(cpd.SMILES) Then
                noSmiles += 1
                Continue For
            End If

            Dim smiles As String = Nothing
            Dim mol As Molecule = Nothing
            Dim why As String = Nothing

            If Not BioCycSmiles.TryParse(cpd.SMILES, smiles, mol, maxMoleculeAtoms, why) Then
                badSmiles += 1
                Dim n As Integer = 0
                rejectReasons.TryGetValue(why, n)
                rejectReasons(why) = n + 1
                If Not rejectSamples.ContainsKey(why) Then
                    rejectSamples(why) = cpd.uniqueId & " = " & cpd.SMILES
                End If
                Continue For
            End If

            structures(cpd.uniqueId) = New CompoundStructure With {
                .Id = cpd.uniqueId,
                .Smiles = smiles,
                .Mol = mol
            }
        Next

        ' ---------- 2) 反应参与度（判定枢纽代谢物） ----------
        For Each rxn As reactions In reactions
            If rxn Is Nothing Then Continue For
            CountSide(rxn.left, degrees)
            CountSide(rxn.right, degrees)
        Next

        ' ---------- 3) 反应实例 → 广义反应规则 ----------
        ruleList.AddRange(BioCycRuleMiner.Mine(
            reactionList:=reactions _
                .Where(Function(r) r IsNot Nothing AndAlso Not String.IsNullOrEmpty(r.uniqueId)) _
                .OrderBy(Function(r) r.uniqueId, StringComparer.Ordinal),
            structures:=structures,
            maxMoleculeAtoms:=maxMoleculeAtoms,
            maxPatternAtoms:=maxPatternAtoms,
            mcsNodeBudget:=mcsNodeBudget,
            maxUnmappedAtoms:=maxUnmappedAtoms,
            shellRadius:=shellRadius,
            includeBuiltin:=includeBuiltinRules,
            skipped:=Skipped,
            trace:=If(keepRuleTrace, RuleTrace, Nothing)))

        ' ---------- 4) 底盘汇集合 ----------
        Dim sinkList As List(Of (String, smiles As String)) =
            BioCycSink.Build(structures, degrees, sinkMode, coreDegree)

        For Each s In sinkList
            Dim st As CompoundStructure = structures(s.Item1)
            sinkEntries.Add((st.Mol.MolKey(), s.Item1, s.smiles))
        Next

        Dim sink As New List(Of (String, smiles As String))()
        For Each e In sinkEntries
            sink.Add((e.id, e.smiles))
        Next

        sw.Stop()
        netwalk = New Netwalk(ruleList, sink, Me.opts, Me.w)

        ' ---------- 5) 诊断 ----------
        Stats("compounds") = compounds.Length
        Stats("structures") = structures.Count
        Stats("no_smiles") = noSmiles
        Stats("bad_smiles") = badSmiles
        Stats("reactions") = reactions.Length
        Stats("rules") = ruleList.Count
        Stats("sink") = sink.Count
        Stats("sink_mode") = sinkMode.ToString()
        Stats("elapsed_ms") = CLng(sw.Elapsed.TotalMilliseconds)

        If verbose Then
            Console.Error.WriteLine($"[BioCycAdapter] 化合物 {compounds.Length}（可用结构 {structures.Count}，" &
                                    $"无 SMILES {noSmiles}，不可用 {badSmiles}）")
            If rejectReasons.Count > 0 Then
                For Each kv In rejectReasons.OrderByDescending(Function(x) x.Value).Take(8)
                    Dim sample As String = Nothing
                    rejectSamples.TryGetValue(kv.Key, sample)
                    If sample Is Nothing Then sample = ""
                    If sample.Length > 110 Then sample = sample.Substring(0, 110) & "..."
                    Console.Error.WriteLine($"[BioCycAdapter]   跳过 {kv.Key} × {kv.Value}  例：{sample}")
                Next
            End If
            Console.Error.WriteLine($"[BioCycAdapter] 反应 {reactions.Length} → 广义规则 {rules.Count}（模式原子上限 {maxPatternAtoms}）")
            If Skipped.Count > 0 Then
                Dim reasons = Skipped.OrderByDescending(Function(kv) kv.Value).
                    Select(Function(kv) $"{kv.Key}={kv.Value}")
                Console.Error.WriteLine($"[BioCycAdapter] 跳过：{String.Join(", ", reasons)}")
            End If
            Console.Error.WriteLine($"[BioCycAdapter] 汇集合 = {sink.Count}（模式 {sinkMode}，coreDegree≥{coreDegree}），装配耗时 {sw.Elapsed.TotalMilliseconds:F0}ms")
        End If
    End Sub

    Private Shared Sub CountSide(side As CompoundSpecieReference(),
                                 deg As Dictionary(Of String, Integer))
        If side Is Nothing Then Return

        For Each spec As CompoundSpecieReference In side
            If spec Is Nothing OrElse String.IsNullOrEmpty(spec.ID) Then Continue For

            Dim id As String = spec.ID.Split(","c)(0).Trim()
            Dim n As Integer = 0

            deg.TryGetValue(id, n)
            deg(id) = n + 1
        Next
    End Sub

    ''' <summary>按 compound frame id 取结构（无结构时返回 Nothing）</summary>
    Public Function GetCompound(id As String) As CompoundStructure
        Dim st As CompoundStructure = Nothing
        structures.TryGetValue(id, st)
        Return st
    End Function

    ''' <summary>
    ''' 以给定 SMILES 为目标做逆向合成通路搜索。
    ''' 查询时会把目标自身从汇集合中剔除：BeamSearch 一旦判定"目标已属于汇"就直接返回
    ''' 0 条路径，而"目标在底盘中已存在"对通路设计没有意义——我们想知道的是它怎么被合成出来。
    ''' </summary>
    Public Function FindPathway(targetSmiles As String) As PathReport
        Dim walker As Netwalk = netwalk
        Dim key As String = Nothing

        Try
            key = SmilesIO.Parse(targetSmiles).MolKey()
        Catch ex As Exception
            key = Nothing
        End Try

        If key IsNot Nothing Then
            Dim sink As New List(Of (String, smiles As String))()
            For Each e In sinkEntries
                If e.key <> key Then sink.Add((e.id, e.smiles))
            Next
            walker = New Netwalk(ruleList, sink, opts, w)
        End If

        Return walker.Search(targetSmiles)
    End Function

    ''' <summary>以 BioCyc 化合物 id 为目标做搜索（如 "ENTEROBACTIN"）</summary>
    Public Function FindPathwayById(compoundId As String) As PathReport
        Dim st As CompoundStructure = GetCompound(compoundId)

        If st Is Nothing Then
            Throw New ArgumentException($"化合物 {compoundId} 在当前 BioCyc 库中没有可用结构（缺失 SMILES 或含不支持的元素）")
        End If

        Return FindPathway(st.Smiles)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Search(targetSmiles As String) As PathReport
        Return FindPathway(targetSmiles)
    End Function

End Class
