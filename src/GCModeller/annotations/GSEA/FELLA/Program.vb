' ============================================================================
' FELLA Algorithm - VB.NET Implementation
' Program.vb - Main entry point with demo analysis
' 
' Demonstrates the complete FELLA workflow:
' 1. Build KEGG graph (demo dataset)
' 2. Precompute database matrices
' 3. Define input compounds
' 4. Run enrichment analysis (all three methods)
' 5. Export and display results
' ============================================================================

Module Program

    Sub Main(args As String())
        Console.WriteLine(New String("="c, 70))
        Console.WriteLine("FELLA - Network-based Metabolomics Enrichment Analysis")
        Console.WriteLine("VB.NET Implementation")
        Console.WriteLine(New String("="c, 70))
        Console.WriteLine()

        ' ====================================================================
        ' Step 1: Build KEGG graph
        ' ====================================================================
        Console.WriteLine("Step 1: Building KEGG graph (demo dataset)...")
        Dim graph = FELLA.Core.BuildGraph.BuildDemoGraph()
        Console.WriteLine(graph.GetStatistics())
        Console.WriteLine()

        ' ====================================================================
        ' Step 2: Build FELLA database (precompute matrices)
        ' ====================================================================
        Console.WriteLine("Step 2: Building FELLA database (precomputing matrices)...")
        Dim sw = System.Diagnostics.Stopwatch.StartNew()

        Dim data As New FELLA.Core.FellaData()
        data.Graph = graph
        data.DampingFactor = 0.85
        data.Gamma = 1.0

        ' Set background compounds
        Dim compounds = graph.GetNodesByType(FELLA.Core.KeggNodeType.Compound)
        data.BackgroundCompounds = New HashSet(Of String)(compounds.Select(Function(c) c.Id))

        ' Build all precomputed matrices
        data.BuildAll(nInput:=5,
                       buildHypergeom:=True,
                       buildDiffusion:=True,
                       buildPagerank:=True)

        sw.Stop()
        Console.WriteLine($"  Database built in {sw.ElapsedMilliseconds} ms")
        Console.WriteLine($"  Hypergeometric matrix: {If(data.HypergeomMatrix IsNot Nothing, "OK", "N/A")}")
        Console.WriteLine($"  Diffusion kernel: {If(data.DiffusionKernel IsNot Nothing, "OK", "N/A")}")
        Console.WriteLine($"  PageRank matrix: {If(data.PagerankMatrix IsNot Nothing, "OK", "N/A")}")
        Console.WriteLine()

        ' ====================================================================
        ' Step 3: Define input compounds (simulated differential metabolites)
        ' ====================================================================
        Console.WriteLine("Step 3: Defining input compounds...")
        Dim inputCompounds As New List(Of String) From {
            "C00031",  ' D-Glucose
            "C00002",  ' ATP
            "C00092",  ' D-Glucose 6-phosphate
            "C00085",  ' D-Fructose 6-phosphate
            "C00149"   ' Malate
        }
        Console.WriteLine($"  Input compounds: {String.Join(", ", inputCompounds)}")
        Console.WriteLine()

        ' ====================================================================
        ' Step 4: Run enrichment analysis
        ' ====================================================================
        Console.WriteLine("Step 4: Running enrichment analysis...")
        Console.WriteLine()

        ' Run all three methods with normality approximation
        Dim user = FELLA.Core.Enrich.Run(
            inputCompounds,
            data,
            methods:=FELLA.Core.Enrich.ListMethods(),
            approx:=FELLA.Core.ApproximationMethod.Normality
        )

        ' ====================================================================
        ' Step 5: Display results
        ' ====================================================================
        Console.WriteLine()
        Console.WriteLine(New String("="c, 70))
        Console.WriteLine("RESULTS")
        Console.WriteLine(New String("="c, 70))

        ' Hypergeometric results
        Console.WriteLine()
        Console.WriteLine(FELLA.Core.ExportResults.GenerateDetailedTable(
            user, FELLA.Core.EnrichmentMethod.Hypergeometric, threshold:=0.1))

        ' Diffusion results
        Console.WriteLine()
        Console.WriteLine(FELLA.Core.ExportResults.GenerateDetailedTable(
            user, FELLA.Core.EnrichmentMethod.Diffusion, threshold:=0.1))

        ' PageRank results
        Console.WriteLine()
        Console.WriteLine(FELLA.Core.ExportResults.GenerateDetailedTable(
            user, FELLA.Core.EnrichmentMethod.PageRank, threshold:=0.1))

        ' ====================================================================
        ' Step 6: Export results to CSV
        ' ====================================================================
        Console.WriteLine()
        Console.WriteLine("Step 6: Exporting results to CSV files...")

        Dim outputDir = System.IO.Directory.GetCurrentDirectory()

        Try
            FELLA.Core.ExportResults.ExportToCsv(
                System.IO.Path.Combine(outputDir, "fella_hypergeom.csv"),
                user, FELLA.Core.EnrichmentMethod.Hypergeometric, threshold:=0.1)

            FELLA.Core.ExportResults.ExportToCsv(
                System.IO.Path.Combine(outputDir, "fella_diffusion.csv"),
                user, FELLA.Core.EnrichmentMethod.Diffusion, threshold:=0.1)

            FELLA.Core.ExportResults.ExportToCsv(
                System.IO.Path.Combine(outputDir, "fella_pagerank.csv"),
                user, FELLA.Core.EnrichmentMethod.PageRank, threshold:=0.1)

            Console.WriteLine("  CSV files exported successfully.")
        Catch ex As Exception
            Console.WriteLine($"  Warning: Could not export CSV files: {ex.Message}")
        End Try

        ' ====================================================================
        ' Step 7: Extract and report sub-network
        ' ====================================================================
        Console.WriteLine()
        Console.WriteLine("Step 7: Extracting sub-network for diffusion method...")
        Dim subGraphNodes = FELLA.Core.ExportResults.GenerateResultsGraph(
            user, data, FELLA.Core.EnrichmentMethod.Diffusion, threshold:=0.1)

        Console.WriteLine($"  Sub-network contains {subGraphNodes.Count} nodes:")
        For Each idx In subGraphNodes.OrderBy(Function(i) i)
            Dim node = graph.GetNode(idx)
            Console.WriteLine($"    [{idx}] {node.Id} - {node.Name} ({node.NodeType})")
        Next

        ' ====================================================================
        ' Summary report
        ' ====================================================================
        Console.WriteLine()
        Console.WriteLine(FELLA.Core.ExportResults.GenerateSummaryReport(user, data, threshold:=0.1))

        ' ====================================================================
        ' Demonstrate Monte Carlo simulation approach
        ' ====================================================================
        Console.WriteLine()
        Console.WriteLine(New String("="c, 70))
        Console.WriteLine("BONUS: Running Diffusion with Monte Carlo Simulation (n=1000)...")
        Console.WriteLine(New String("="c, 70))

        Dim userSim = FELLA.Core.Enrich.Run(
            inputCompounds,
            data,
            methods:=New List(Of FELLA.Core.EnrichmentMethod) From {FELLA.Core.EnrichmentMethod.Diffusion},
            approx:=FELLA.Core.ApproximationMethod.Simulation,
            nSim:=1000
        )

        Console.WriteLine()
        Console.WriteLine(FELLA.Core.ExportResults.GenerateDetailedTable(
            userSim, FELLA.Core.EnrichmentMethod.Diffusion, threshold:=0.1))

        Console.WriteLine()
        Console.WriteLine("Analysis complete. Press any key to exit.")
        Console.ReadKey()
    End Sub

End Module
