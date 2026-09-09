Imports System.Diagnostics
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.GO
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.DAG

''' <summary>
''' 基于GO DAG图的富集分析的端到端测试
''' </summary>
Module GOEnrichmentTest

    Sub Run(dag As Graph)
        Call Console.WriteLine("")
        Call Console.WriteLine("================ GO DAG enrichment test ================")

        If dag.Terms.Count = 0 Then
            Call Console.WriteLine("[SKIP] the go ontology database is empty")
            Return
        End If

        ' 取出DAG图之中深度最深的一个词条作为本次测试的"信号"词条
        Dim signalTerm As String = deepestTerm(dag)
        Dim parents As String() = dag.GetAncestors(signalTerm)

        Call Console.WriteLine($"    signal go term: {signalTerm} ""{dag.GetTerm(signalTerm).GO_term.name}"" [{dag.GetTerm(signalTerm).namespace}]")
        Call Console.WriteLine($"    signal term has {parents.Length} ancestors")

        Call DAGtest.assert(parents.Length > 0, "the signal term should have at least one ancestor")

        Dim genomeSize% = 2000
        Dim signalSize% = 120
        Dim inputSignal% = 100
        Dim rand As New Random(2048)
        Dim allTerms As String() = dag.Terms _
            .Select(Function(t) t.id) _
            .ToArray
        Dim map As New Dictionary(Of String, String())

        For i As Integer = 0 To genomeSize - 1
            Dim geneID$ = $"GENE_{i.ToString("D5")}"
            Dim terms As New List(Of String)

            If i < signalSize Then
                terms.Add(signalTerm)
            End If

            For n As Integer = 1 To rand.Next(1, 6)
                terms.Add(allTerms(rand.Next(allTerms.Length)))
            Next

            map(geneID) = terms.Distinct.ToArray
        Next

        Dim annotations As GOAnnotation = GOAnnotation.FromDictionary(map)

        Call Console.WriteLine($"    simulate {annotations.universeSize} genes with GO annotations")

        ' 1. 校验祖先传播
        Call testPropagation(dag, annotations, signalTerm)

        ' 2. 构建背景模型
        Dim watch As Stopwatch = Stopwatch.StartNew
        Dim background As Background = annotations.CreateGOBackground(dag)

        Call watch.Stop()

        Call Console.WriteLine("")
        Call Console.WriteLine($"    build background: {background.clusters.Length} go terms as clusters, in {watch.ElapsedMilliseconds} ms")
        Call Console.WriteLine($"    background universe size: {background.size}")

        Call DAGtest.assert(background.size = genomeSize, "the universe size should be equals to the annotation gene size")

        Dim signalCluster As Cluster = background(signalTerm)

        Call DAGtest.assert(Not signalCluster Is Nothing, "the signal go term should be exists in the background")
        Call DAGtest.assert(signalCluster.members.Length >= signalSize,
                            $"the signal cluster should contains all of the {signalSize} signal genes, but got {signalCluster.members.Length}")

        ' 3. 富集计算
        Dim geneSet As New List(Of String)

        For i As Integer = 0 To inputSignal - 1
            geneSet.Add($"GENE_{i.ToString("D5")}")
        Next
        For i As Integer = 0 To 99
            geneSet.Add($"GENE_{(genomeSize - 1 - i).ToString("D5")}")
        Next

        watch = Stopwatch.StartNew

        Dim result As EnrichmentResult() = background.EnrichGO(geneSet, cutSize:=3)

        Call watch.Stop()

        Call Console.WriteLine("")
        Call Console.WriteLine($"    enrichment of {geneSet.Count} genes in {watch.ElapsedMilliseconds} ms, {result.Length} terms hits")

        Call testResult(dag, result, signalTerm, inputSignal)

        ' 4. 分本体富集
        Call testOntology(dag, background, geneSet)

        ' 5. 结果导出
        Dim saveAs$ = IO.Path.Combine(IO.Path.GetTempPath(), "go_dag_enrichment_test.csv")

        Call result.SaveTable(saveAs)
        Call Console.WriteLine("")
        Call Console.WriteLine($"    result table saved to: {saveAs}")

        Call DAGtest.assert(New IO.FileInfo(saveAs).Length > 0, "the result table should not be empty")
    End Sub

    Private Function deepestTerm(dag As Graph) As String
        Dim table As Dictionary(Of String, String()) = dag.GetAncestorTable()
        Dim maxLen As Integer = -1
        Dim term As String = Nothing

        For Each node As KeyValuePair(Of String, String()) In table
            If node.Value.Length > maxLen Then
                maxLen = node.Value.Length
                term = node.Key
            End If
        Next

        Return term
    End Function

    ''' <summary>
    ''' 校验一个只被标注到了深层词条上面的基因，是否能够正确的被传播到
    ''' 其所有的祖先词条之中
    ''' </summary>
    Private Sub testPropagation(dag As DAG.Graph, annotations As GOAnnotation, signalTerm$)
        Call Console.WriteLine("")
        Call Console.WriteLine("---------------- annotation propagation test ----------------")

        Dim geneID$ = "GENE_00000"
        Dim expanded As String() = annotations.Expand(dag)(geneID)
        Dim expected As String() = dag.GetAncestors(signalTerm, includeSelf:=True)

        Call Console.WriteLine($"    {geneID} was expanded from {annotations.GetTerms(geneID).Length} terms to {expanded.Length} terms")

        Dim missing As String() = expected _
            .Where(Function(id) Array.IndexOf(expanded, id) = -1) _
            .ToArray

        Call DAGtest.assert(missing.Length = 0,
                            $"all of the {expected.Length} ancestors of the signal term should be in the expanded gene terms")

        If missing.Length > 0 Then
            Call Console.WriteLine($"        missing: {missing.Take(5).JoinBy(", ")}")
        End If

        Dim termGenes As Dictionary(Of String, List(Of String)) = annotations.TermGenes(dag)
        Dim parent As String = dag.GetAncestors(signalTerm).FirstOrDefault

        Call DAGtest.assert(termGenes.ContainsKey(parent) AndAlso termGenes(parent).Contains(geneID),
                            $"{geneID} should be a member of its ancestor term {parent}")
        Call DAGtest.assert(termGenes(signalTerm).Contains(geneID),
                            $"{geneID} should be a member of the signal term itself")
    End Sub

    Private Sub testResult(dag As Graph, result As EnrichmentResult(), signalTerm$, inputSignal%)
        Call Console.WriteLine("")
        Call Console.WriteLine("---------------- enrichment result test ----------------")

        Dim signal As EnrichmentResult = result _
            .Where(Function(t) t.term = signalTerm) _
            .FirstOrDefault

        Call DAGtest.assert(Not signal Is Nothing, "the signal go term should be enriched")

        If Not signal Is Nothing Then
            Call Console.WriteLine($"    signal term: {signal.term} ""{signal.name}"" [{signal.category}]")
            Call Console.WriteLine($"        term size = {signal.cluster}, enriched = {signal.enriched}")
            Call Console.WriteLine($"        pvalue = {signal.pvalue.ToString("G4")}, FDR = {signal.FDR.ToString("G4")}, score = {signal.score.ToString("G4")}")

            Call DAGtest.assert(signal.enriched = inputSignal,
                                $"the enriched gene count should be equals to {inputSignal}, but got {signal.enriched}")
            Call DAGtest.assert(signal.pvalue < 0.05, "the signal term should be significantly enriched")
        End If

        ' p值与FDR的取值范围校验
        Dim badPvalue = result.Where(Function(t) t.pvalue < 0 OrElse t.pvalue > 1).ToArray
        Dim badFDR = result.Where(Function(t) t.FDR < 0 OrElse t.FDR > 1).ToArray

        Call DAGtest.assert(badPvalue.Length = 0, "all of the pvalue should be in range [0, 1]")
        Call DAGtest.assert(badFDR.Length = 0, "all of the FDR should be in range [0, 1]")

        ' 结果必须是按照p值从小到大排序的，并且FDR单调不减
        Dim sorted As Boolean = True
        Dim monotonic As Boolean = True

        For i As Integer = 1 To result.Length - 1
            If result(i).pvalue < result(i - 1).pvalue Then
                sorted = False
            End If
            If result(i).FDR < result(i - 1).FDR Then
                monotonic = False
            End If
        Next

        Call DAGtest.assert(sorted, "the result should be sorted by the pvalue ascending")
        Call DAGtest.assert(monotonic, "the FDR value should be monotonic non-decreasing")

        Call Console.WriteLine("")
        Call Console.WriteLine("    top 15 enriched go terms:")

        For Each term As EnrichmentResult In result.Take(15)
            Call Console.WriteLine($"        {term.term} ""{term.name}"" [{term.category}] " &
                                   $"{term.enriched}/{term.cluster} p={term.pvalue.ToString("G3")} fdr={term.FDR.ToString("G3")}")
        Next

        ' 分组
        Dim groups = result.SplitByOntology()

        Call Console.WriteLine("")
        Call Console.WriteLine($"    split into {groups.Count} ontologies")

        For Each group In groups
            Call Console.WriteLine($"        [{group.Key}] {group.Value.Length} terms")
        Next
    End Sub

    Private Sub testOntology(dag As Graph, background As Background, geneSet As IEnumerable(Of String))
        Call Console.WriteLine("")
        Call Console.WriteLine("---------------- ontology separated enrichment test ----------------")

        For Each ontology As Ontologies In New Ontologies() {
            Ontologies.BiologicalProcess,
            Ontologies.CellularComponent,
            Ontologies.MolecularFunction
        }
            Dim ns$ = GOEnrichment.NamespaceOf(ontology)
            Dim result As EnrichmentResult() = background.EnrichGO(geneSet, ontology:=ontology, cutSize:=3)
            Dim bad = result _
                .Where(Function(t) Not ns.Equals(t.category, StringComparison.OrdinalIgnoreCase)) _
                .ToArray

            Call Console.WriteLine($"    [{ns}] {result.Length} terms")
            Call DAGtest.assert(bad.Length = 0, $"all of the result of {ns} should be in the {ns} namespace")
        Next
    End Sub
End Module
