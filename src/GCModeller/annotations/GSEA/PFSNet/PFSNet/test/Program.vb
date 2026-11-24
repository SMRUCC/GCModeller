Imports System.Globalization
Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.PFSNet
Imports SMRUCC.genomics.Analysis.PFSNet.DataStructure
Imports GraphData = SMRUCC.genomics.Analysis.PFSNet.R.Graph.Data
Imports RBase = SMRUCC.genomics.Analysis.PFSNet.R.Base

Module Program

    Private failures As Integer

    Public Function Main(args As String()) As Integer
        Console.OutputEncoding = Encoding.UTF8

        Try
            Dim permutations As Integer = GetIntegerOption(args, "--n", 100)
            Dim seed As Integer = GetIntegerOption(args, "--seed", 20261026)
            Dim dataDirectory As String = GetStringOption(args, "--data")

            If String.IsNullOrWhiteSpace(dataDirectory) Then
                dataDirectory = FindDataDirectory()
            End If

            Dim pathwaysFile As String = Path.Combine(dataDirectory, "pathways.txt")
            Dim controlFile As String = Path.Combine(dataDirectory, "sample_control.txt")
            Dim testFile As String = Path.Combine(dataDirectory, "sample_test.txt")
            Dim outputFile As String = GetStringOption(args, "--out")

            If String.IsNullOrWhiteSpace(outputFile) Then
                outputFile = Path.Combine(AppContext.BaseDirectory, "pfsnet_demo.csv")
            End If

            RunDeterministicChecks(pathwaysFile)

            Dim control = Matrix.LoadData(controlFile)
            Dim test = Matrix.LoadData(testFile)
            Dim pathways As GraphEdge() = GraphEdge.LoadData(pathwaysFile)
            Dim controlGenes As New HashSet(Of String)(control.expression.Select(Function(row) row.geneID), StringComparer.Ordinal)
            Dim testGenes As New HashSet(Of String)(test.expression.Select(Function(row) row.geneID), StringComparer.Ordinal)

            controlGenes.IntersectWith(testGenes)

            AssertTrue(control.sampleID.Length = 12, "control data contains 12 samples")
            AssertTrue(test.sampleID.Length = 42, "test data contains 42 samples")
            AssertTrue(controlGenes.Count > 0, "the two matrices share genes")
            AssertTrue(pathways.Length > 0, "pathway table contains edges")
            AssertTrue(pathways.All(Function(edge) Not edge.pathwayID.Equals("pathway", StringComparison.OrdinalIgnoreCase)), "pathway header is not loaded as an edge")

            Console.WriteLine()
            Console.WriteLine("Running PFSNet demo with {0} permutations and seed {1}...", permutations, seed)

            Dim result As PFSNetResultOut = PFSNetAlgorithm.pfsnet(
                controlFile,
                testFile,
                pathwaysFile,
                b:=0.5,
                t1:=0.8,
                t2:=0.7,
                n:=permutations,
                seed:=seed)

            ValidateResults(result)
            WriteResults(outputFile, result)

            Console.WriteLine()
            Console.WriteLine("control_samples={0}", control.sampleID.Length)
            Console.WriteLine("test_samples={0}", test.sampleID.Length)
            Console.WriteLine("common_genes={0}", controlGenes.Count)
            Console.WriteLine("pathways={0}", pathways.Select(Function(edge) edge.pathwayID).Distinct(StringComparer.Ordinal).Count())
            Console.WriteLine("input_edges={0}", pathways.Length)
            Console.WriteLine("phenotype1_significant={0}", result.phenotype1.Length)
            Console.WriteLine("phenotype2_significant={0}", result.phenotype2.Length)
            Console.WriteLine("result_csv={0}", Path.GetFullPath(outputFile))
            Console.WriteLine("assertions_failed={0}", failures)

            Return If(failures = 0, 0, 1)
        Catch ex As Exception
            Console.Error.WriteLine(ex.ToString())
            Return 2
        End Try
    End Function

    Private Sub RunDeterministicChecks(pathwaysFile As String)
        Console.WriteLine("Running deterministic PFSNet checks...")

        Dim paired As Double = PFSNetStatistics.PairedTTest({2.0, 4.0, 6.0}, {1.0, 2.0, 3.0})
        AssertNear(paired, 3.4641016151377544, 0.000000001, "paired t statistic matches the R reference")

        Dim expression As DataFrameRow() = {
            Row("g1", 0, 4),
            Row("g2", 0, 3),
            Row("g3", 1, 2),
            Row("g4", 2, 1)
        }
        Dim weights As DataFrameRow() = computew1Function.computew1(expression, 0.75, 0.25)
        Dim reordered As DataFrameRow() = computew1Function.computew1({expression(3), expression(1), expression(0), expression(2)}, 0.75, 0.25)
        Dim weightLookup = weights.ToDictionary(Function(row) row.geneID, StringComparer.Ordinal)
        Dim reorderedLookup = reordered.ToDictionary(Function(row) row.geneID, StringComparer.Ordinal)

        AssertNear(weightLookup("g1").experiments(0), weightLookup("g2").experiments(0), 0, "tied expression values receive equal fuzzy weights")
        AssertTrue(weights.SelectMany(Function(row) row.experiments).Where(Function(value) Double.IsFinite(value)).All(Function(value) value >= 0 AndAlso value <= 1), "fuzzy weights remain in [0,1]")

        For Each gene As String In weightLookup.Keys
            For sampleIndex As Integer = 0 To weightLookup(gene).samples - 1
                AssertNear(weightLookup(gene).experiments(sampleIndex), reorderedLookup(gene).experiments(sampleIndex), 0, "gene row order does not change fuzzy weights")
            Next
        Next

        Dim duplicateEdges As GraphEdge() = {
            Edge("p", "a", "b"),
            Edge("p", "b", "a"),
            Edge("p", "a", "a"),
            Edge("p", "b", "c")
        }
        Dim simple As PFSNetGraph = GraphData.simplify(GraphData.Frame(duplicateEdges, directed:=False))

        AssertTrue(simple.edges.Length = 2, "graph simplify removes loops and undirected duplicate edges")
        AssertTrue(simple.edges.All(Function(edge) Not edge.isSelfLoop), "simplified graph contains no self loops")

        Dim chain As GraphEdge() = {
            Edge("mean-test", "n1", "n2"),
            Edge("mean-test", "n2", "n3"),
            Edge("mean-test", "n3", "n4"),
            Edge("mean-test", "n4", "n5")
        }
        Dim firstWeights As DataFrameRow() = {
            Row("n1", 0.2, 0.4), Row("n2", 0.1, 0.3), Row("n3", 0.5, 0.7),
            Row("n4", 0.6, 0.8), Row("n5", 0.0, 1.0)
        }
        Dim secondWeights As DataFrameRow() = {
            Row("n1", 0.6, 0.8), Row("n2", 0.2, 0.4), Row("n3", 0.3, 0.5),
            Row("n4", 0.4, 0.6), Row("n5", 0.1, 0.9)
        }
        Dim components As PFSNetGraph() = masked_ggi.ccs(chain, firstWeights, secondWeights).ToArray

        AssertTrue(components.Length = 1, "five-node connected component is retained")
        AssertTrue(components(0).nodes.Length = 5, "component threshold counts distinct vertices")
        AssertTrue(components(0).edges.Length = 4, "component decomposition does not duplicate edges")
        AssertNear(components(0).Node("n1").weight, 0.3, 0.000000000001, "node weight is the first-group sample mean")
        AssertNear(components(0).Node("n1").weight2, 0.7, 0.000000000001, "node weight2 is the second-group sample mean")

        Dim rng As New Random(12345)
        Dim sawLastSample As Boolean
        Dim validGroupSizes As Boolean = True
        Dim validIndices As Boolean = True

        For iteration As Integer = 1 To 100
            Dim selected As Integer() = RBase.Sample(54, 12, rng)
            validGroupSizes = validGroupSizes AndAlso selected.Length = 12 AndAlso selected.Distinct.Count = 12
            validIndices = validIndices AndAlso selected.All(Function(index) index >= 0 AndAlso index < 54)
            sawLastSample = sawLastSample OrElse selected.Contains(53)
        Next

        AssertTrue(validGroupSizes, "permutation groups have 12 unique selected samples")
        AssertTrue(validIndices, "permutation indices cover only the full 0..53 range")
        AssertTrue(sawLastSample, "the final sample can participate in the permuted first group")

        Dim loadedEdges As GraphEdge() = GraphEdge.LoadData(pathwaysFile)
        AssertTrue(loadedEdges.All(Function(edge) edge.g1.Length > 0 AndAlso edge.g2.Length > 0), "pathway parser ignores malformed empty endpoints")
    End Sub

    Private Sub ValidateResults(result As PFSNetResultOut)
        AssertTrue(result IsNot Nothing, "PFSNet returns a result")
        AssertTrue(Not String.IsNullOrWhiteSpace(result.class1) AndAlso Not String.IsNullOrWhiteSpace(result.class2), "result phenotype labels are populated")
        AssertTrue(result.phenotype1 IsNot Nothing AndAlso result.phenotype2 IsNot Nothing, "result phenotype arrays are populated")

        Dim signatures As New HashSet(Of String)(StringComparer.Ordinal)

        ValidateGraphs(result.class1, result.phenotype1, signatures)
        ValidateGraphs(result.class2, result.phenotype2, signatures)
    End Sub

    Private Sub ValidateGraphs(phenotype As String, graphs As PFSNetGraph(), signatures As HashSet(Of String))
        For Each graph As PFSNetGraph In graphs
            Dim genes As String() = graph.nodes.Select(Function(node) node.name).OrderBy(Function(gene) gene, StringComparer.Ordinal).ToArray
            Dim members As New HashSet(Of String)(genes, StringComparer.Ordinal)
            Dim signature As String = phenotype & vbNullChar & graph.Id & vbNullChar & String.Join(";", genes)

            AssertTrue(graph.nodes.Length >= 5, "reported components contain at least five genes")
            AssertTrue(Double.IsFinite(graph.statistics), "reported t statistics are finite")
            AssertTrue(Double.IsFinite(graph.pvalue) AndAlso graph.pvalue >= 0 AndAlso graph.pvalue <= 1, "reported p-values are finite probabilities")
            AssertTrue(graph.nodes.All(Function(node) Double.IsFinite(node.weight) AndAlso node.weight >= 0 AndAlso node.weight <= 1 AndAlso Double.IsFinite(node.weight2) AndAlso node.weight2 >= 0 AndAlso node.weight2 <= 1), "reported node weights remain in [0,1]")
            AssertTrue(graph.edges.All(Function(edge) members.Contains(edge.g1) AndAlso members.Contains(edge.g2)), "all edge endpoints belong to the component")
            AssertTrue(signatures.Add(signature), "reported component signatures are unique")
        Next
    End Sub

    Private Sub WriteResults(outputFile As String, result As PFSNetResultOut)
        Dim rows As SubNetworkTableRow() = result.AsTabular()
        Dim savedPath As String = result.SaveTabular(outputFile)

        AssertTrue(rows.Length = result.phenotype1.Length + result.phenotype2.Length, "one tabular row is exported per significant subnetwork")
        AssertTrue(rows.All(Function(row) row.phenotype = "A" OrElse row.phenotype = "B"), "exported phenotype column uses the legacy A/B labels")
        AssertTrue(rows.All(Function(row) row.nodes >= 5), "exported rows keep the five-gene component threshold")
        AssertTrue(rows.All(Function(row) row.genes.Split(New String() {"; "}, StringSplitOptions.None).Length = row.nodes), "exported gene list matches the exported node count")

        Dim written As String() = File.ReadAllLines(savedPath, Encoding.UTF8)

        AssertTrue(written(0) = "subnetwork,phenotype,statistics,pvalue,nodes,edges,weight1,weight2,genes", "exported csv uses the legacy PFSNet header")
        AssertTrue(written.Length = rows.Length + 1, "exported csv contains the header and one line per row")
        AssertTrue(written.Skip(1).All(Function(line) line.Split(","c).Length >= 9), "exported csv rows contain at least the nine leading columns")
    End Sub

    Private Function Row(gene As String, ParamArray values As Double()) As DataFrameRow
        Return New DataFrameRow With {
            .geneID = gene,
            .experiments = values
        }
    End Function

    Private Function Edge(pathway As String, gene1 As String, gene2 As String) As GraphEdge
        Return New GraphEdge With {
            .pathwayID = pathway,
            .g1 = gene1,
            .g2 = gene2
        }
    End Function

    Private Sub AssertNear(actual As Double, expected As Double, tolerance As Double, message As String)
        AssertTrue(Double.IsFinite(actual) AndAlso System.Math.Abs(actual - expected) <= tolerance, message & $" (expected={expected:R}, actual={actual:R})")
    End Sub

    Private Sub AssertTrue(condition As Boolean, message As String)
        If condition Then
            Console.WriteLine("[PASS] " & message)
        Else
            failures += 1
            Console.Error.WriteLine("[FAIL] " & message)
        End If
    End Sub

    Private Function GetIntegerOption(args As String(), name As String, defaultValue As Integer) As Integer
        Dim text As String = GetStringOption(args, name)
        Dim value As Integer

        If String.IsNullOrWhiteSpace(text) Then
            Return defaultValue
        ElseIf Not Integer.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, value) OrElse value < 1 Then
            Throw New ArgumentException($"{name} must be a positive integer.")
        Else
            Return value
        End If
    End Function

    Private Function GetStringOption(args As String(), name As String) As String
        For i As Integer = 0 To args.Length - 2
            If args(i).Equals(name, StringComparison.OrdinalIgnoreCase) Then
                Return args(i + 1)
            End If
        Next

        Return Nothing
    End Function

    Private Function FindDataDirectory() As String
        Dim starts As String() = {Environment.CurrentDirectory, AppContext.BaseDirectory}

        For Each start As String In starts
            Dim current As New DirectoryInfo(Path.GetFullPath(start))

            While current IsNot Nothing
                Dim candidate As String = Path.Combine(current.FullName, "data")

                If File.Exists(Path.Combine(candidate, "pathways.txt")) AndAlso
                   File.Exists(Path.Combine(candidate, "sample_control.txt")) AndAlso
                   File.Exists(Path.Combine(candidate, "sample_test.txt")) Then
                    Return candidate
                End If

                current = current.Parent
            End While
        Next

        Throw New DirectoryNotFoundException("Unable to locate the PFSNet data directory. Pass it with --data <path>.")
    End Function
End Module
