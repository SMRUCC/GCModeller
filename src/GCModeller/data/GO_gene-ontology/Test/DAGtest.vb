#Region "Microsoft.VisualBasic::e21861573956ff8335fcf35dc464d4b03, data\GO_gene-ontology\Test\DAGtest.vb"

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

    ' Module DAGtest
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Diagnostics
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Module DAGtest

    ''' <summary>
    ''' 用于测试的GO本体数据库文件
    ''' </summary>
    Const go_obo$ = "J:\go.obo"

    Sub Main()
        Dim watch As New Stopwatch
        Dim testTerm$ = "GO:0000007"

        Call watch.Start()
        Dim g As Graph = New Graph(go_obo)
        Call watch.Stop()

        Call Console.WriteLine($"Load GO ontology database: {go_obo}")
        Call Console.WriteLine($"    {g.Terms.Count} go terms, build in {watch.ElapsedMilliseconds} ms")
        Call Console.WriteLine($"    header: {If(g.header Is Nothing, "(nothing)", g.header.ToString)}")

        If g.Terms.Count = 0 Then
            Call Console.WriteLine("[ERROR] no go term was loaded!")
            Call Pause()
            Return
        End If

        Call testAncestors(g, testTerm)
        Call testDescendants(g, testTerm)
        Call testFamily(g, testTerm)
        Call testDefinition(g, testTerm)
        Call testLevelStat(g)

        Call GOEnrichmentTest.Run(g)

        Call Pause()
    End Sub

    Private Sub testAncestors(g As Graph, testTerm$)
        Call Console.WriteLine("")
        Call Console.WriteLine("---------------- ancestor closure test ----------------")

        Dim node As TermNode = g.GetTerm(testTerm)

        If node Is Nothing Then
            Call Console.WriteLine($"[SKIP] missing go term {testTerm}")
            Return
        End If

        Dim ancestors As String() = g.GetAncestors(testTerm)

        Call Console.WriteLine($"    {testTerm} ""{node.GO_term.name}"" [{node.namespace}]")
        Call Console.WriteLine($"    ancestors: {ancestors.Length}")

        ' 每一个GO词条的祖先链的最顶端都必须是三大namespace根节点之一
        Dim roots As String() = {"GO:0008150", "GO:0003674", "GO:0005575"}
        Dim hitRoot As String = ancestors _
            .Where(Function(id) Array.IndexOf(roots, id) > -1) _
            .FirstOrDefault

        Call assert(Not String.IsNullOrEmpty(hitRoot), $"{testTerm} should be linked to one of the GO root terms")

        Dim rootNode As TermNode = g.GetTerm(hitRoot)

        Call Console.WriteLine($"    top root : {hitRoot} ""{rootNode.GO_term.name}""")
        Call assert(rootNode.namespace = node.namespace, "the root namespace should be equals to the term namespace")

        ' 祖先集合之中不能够包含自身
        Call assert(Array.IndexOf(ancestors, testTerm) = -1, "ancestors should not contains the term itself")
        Call assert(g.GetAncestors(testTerm, includeSelf:=True).Length = ancestors.Length + 1, "includeSelf should add the term itself")

        ' 祖先集合必须与其子孙索引互为反向
        Dim descendants As TermNode() = g.GetDescendants(hitRoot)

        Call assert(descendants.Length > 0, "the root term should have descendants")
        Call assert(g.IsAncestorOf(testTerm, hitRoot), $"{hitRoot} should be the ancestor of {testTerm}")
        Call assert(Not g.IsAncestorOf(hitRoot, testTerm), "the reverse relationship should not be exists")
    End Sub

    Private Sub testDescendants(g As Graph, testTerm$)
        Call Console.WriteLine("")
        Call Console.WriteLine("---------------- descendant index test ----------------")

        Dim watch As New Stopwatch

        Call watch.Start()

        Dim watch2 = Stopwatch.StartNew
        Dim members As TermNode() = g.GetDescendants("GO:0003674")
        Dim elapsed = watch2.ElapsedMilliseconds

        Call Console.WriteLine($"    molecular_function has {members.Length} descendants, evaluate in {elapsed} ms")

        ' 随机抽样校验：子孙节点必须都能够反查到GO:0003674这个祖先
        Dim rand As New Random(1024)
        Dim sample As TermNode() = members.OrderBy(Function(t) rand.Next).Take(100).ToArray

        For Each node As TermNode In sample
            Call assert(g.IsAncestorOf(node.id, "GO:0003674"), $"{node.id} should be a descendant of molecular_function")
        Next

        ' GetClusterMembers 与 GetDescendants 必须给出一致的结果
        Dim clusterMembers As TermNode() = g.GetClusterMembers("GO:0003674").ToArray

        Call assert(clusterMembers.Length = members.Length, "GetClusterMembers should be equals to GetDescendants")
        Call assert(Array.IndexOf(clusterMembers.Select(Function(t) t.id).ToArray, testTerm) > -1,
                    $"{testTerm} should be a member of the molecular_function cluster")
    End Sub

    Private Sub testFamily(g As Graph, testTerm$)
        Call Console.WriteLine("")
        Call Console.WriteLine("---------------- family chain test ----------------")

        Dim chains = g.Family(testTerm) _
            .Select(Function(x) x.Strip) _
            .ToArray

        Call Console.WriteLine($"    {testTerm} has {chains.Length} inherits chains")

        Call assert(chains.Length > 0, "should have at least one inherits chain")

        For Each chain In chains
            ' 继承链的最顶端必须是三大namespace之一
            Call assert(Array.IndexOf(Graph.RootNames, chain.Namespace) > -1,
                        $"the chain top namespace should be one of the three GO roots, but got {chain.Namespace}")

            ' 继承链之中不应该出现重复的节点
            Call assert(chain.Route.Count = chain.Route.Distinct.Count, "the chain route should not contains any duplicated node")

            ' Level(1) 必须是根节点
            Dim top As Term = chain.Level(1)

            Call assert(Not top Is Nothing, "Level(1) should not be nothing")
            Call assert(top.name = chain.Namespace, "Level(1) should be the root term")

            Call Console.WriteLine($"    [{chain.Namespace}] {chain.Family.JoinBy(" -> ")}")
        Next

        ' Family(id, root) 向上查找到指定的根节点为止
        Dim root$ = g.GetTerm("GO:0003674").GO_term.name
        Dim subChains = g.Family(testTerm, root).ToArray

        Call Console.WriteLine($"    Family({testTerm}, {root}) => {subChains.Length} chains")

        Call assert(subChains.Length > 0, "should have at least one chain to the root")

        For Each chain In subChains
            ' 继承链的最后一个元素就是root的直接子节点
            Dim label$ = chain.Family(0)

            Call Console.WriteLine($"        category label: {label}")
            Call assert(Not String.IsNullOrEmpty(label), "the category label should not be empty")

            Dim parent As TermNode = g.GetTerm(chain.Route.Last.id)

            Call assert(Array.IndexOf(parent.GO_term.is_a.SafeQuery _
                .Select(Function(s) s.GetTagValue(" ! ", trim:=True).Name) _
                .ToArray, "GO:0003674") > -1,
                $"{parent.id} should be a direct child of the root")
        Next
    End Sub

    ''' <summary>
    ''' ``DAG.def``的构造函数在旧版本之中会错误的引用尚未赋值的成员字段
    ''' 从而抛出空引用异常，这里做一个回归测试
    ''' </summary>
    Private Sub testDefinition(g As Graph, testTerm$)
        Call Console.WriteLine("")
        Call Console.WriteLine("---------------- term definition parser test ----------------")

        Dim node As TermNode = g.GetTerm(testTerm)

        If node Is Nothing OrElse String.IsNullOrEmpty(node.GO_term.def) Then
            Call Console.WriteLine($"[SKIP] {testTerm} has no definition")
            Return
        End If

        Dim info As New def(node.GO_term.def)

        Call Console.WriteLine($"    {info}")

        Call assert(Not String.IsNullOrEmpty(info.def), "the definition text should not be empty")
        Call assert(Not info.def.Contains("["), "the evidence reference block should be trimmed")
    End Sub

    Private Sub testLevelStat(g As Graph)
        Call Console.WriteLine("")
        Call Console.WriteLine("---------------- go term level stat test ----------------")

        Dim stat As New Dictionary(Of String, NamedValue(Of Integer)()) From {
            {"biological_process", {
                New NamedValue(Of Integer)("GO:0009409", 1),
                New NamedValue(Of Integer)("GO:0009725", 1),
                New NamedValue(Of Integer)("GO:0033993", 1),
                New NamedValue(Of Integer)("GO:0097305", 1),
                New NamedValue(Of Integer)("GO:0009743", 1),
                New NamedValue(Of Integer)("GO:0014070", 1)
            }}
        }
        Dim level3 = stat.LevelGOTerms(3, g)

        Call Console.WriteLine($"    level3 has {level3.Count} namespaces")

        For Each ns In level3
            Call Console.WriteLine($"    [{ns.Key}] {ns.Value.Length} terms")

            For Each term As NamedValue(Of Integer) In ns.Value.Take(5)
                Call Console.WriteLine($"        {term.Name} = {term.Value}")
            Next
        Next

        Call assert(level3.Count > 0, "level go terms stat should not be empty")
    End Sub

    ''' <summary>
    ''' 简单的断言测试
    ''' </summary>
    ''' <param name="condition"></param>
    ''' <param name="message"></param>
    Friend Sub assert(condition As Boolean, message As String)
        If condition Then
            Call Console.WriteLine($"        [PASS] {message}")
        Else
            Call Console.WriteLine($"        [FAIL] {message}")
        End If
    End Sub
End Module
