' ============================================================================
' FELLA Algorithm - VB.NET Implementation
' BuildGraph.vb - Build KEGG graph from data sources
' 
' Provides functions to:
' - Build a KEGG graph from structured data (tab-delimited files)
' - Build a demo graph for testing
' - Parse KEGG relationship data
' 
' In the R package, this uses KEGGREST API to fetch data from KEGG.
' Here we provide file-based loading and a built-in demo dataset.
' ============================================================================

Namespace Core

    ''' <summary>
    ''' Build KEGG graph from data sources.
    ''' 
    ''' In the R FELLA package, buildGraphFromKEGGREST() uses the KEGG REST API
    ''' to download pathway, module, enzyme, reaction, and compound data.
    ''' This VB.NET implementation provides:
    ''' 1. File-based loading from tab-delimited text files
    ''' 2. Programmatic graph construction
    ''' 3. A built-in demo dataset for testing
    ''' </summary>
    Public Class BuildGraph

        ''' <summary>
        ''' Build a KEGG graph from tab-delimited data files.
        ''' 
        ''' Expected file format (tab-delimited, one entry per line):
        ''' 
        ''' nodes.txt:
        '''   KEGG_ID    NAME    TYPE
        '''   C00002     ATP     compound
        '''   R00100     reaction1   reaction
        '''   ...
        ''' 
        ''' edges.txt:
        '''   SOURCE_ID  TARGET_ID  EDGE_TYPE
        '''   C00002     R00100     compound-reaction
        '''   ...
        ''' </summary>
        Public Shared Function BuildFromFiles(nodesFilePath As String,
                                               edgesFilePath As String) As KeggGraph
            Dim graph As New KeggGraph()

            ' Read nodes
            For Each line In System.IO.File.ReadLines(nodesFilePath)
                If String.IsNullOrWhiteSpace(line) Then Continue For
                If line.StartsWith("#") Then Continue For ' Comment lines

                Dim parts = line.Split(vbTab)
                If parts.Length < 3 Then Continue For

                Dim keggId = parts(0).Trim()
                Dim name = parts(1).Trim()
                Dim typeStr = parts(2).Trim().ToLower()

                Dim nodeType As KeggNodeType
                Select Case typeStr
                    Case "pathway" : nodeType = KeggNodeType.Pathway
                    Case "module" : nodeType = KeggNodeType.Module
                    Case "enzyme" : nodeType = KeggNodeType.Enzyme
                    Case "reaction" : nodeType = KeggNodeType.Reaction
                    Case "compound" : nodeType = KeggNodeType.Compound
                    Case Else : Continue For ' Skip unknown types
                End Select

                graph.AddNode(keggId, name, nodeType)
            Next

            ' Read edges
            For Each line In System.IO.File.ReadLines(edgesFilePath)
                If String.IsNullOrWhiteSpace(line) Then Continue For
                If line.StartsWith("#") Then Continue For

                Dim parts = line.Split(vbTab)
                If parts.Length < 3 Then Continue For

                Dim sourceId = parts(0).Trim()
                Dim targetId = parts(1).Trim()
                Dim edgeType = parts(2).Trim()

                If graph.ContainsId(sourceId) AndAlso graph.ContainsId(targetId) Then
                    graph.AddEdge(sourceId, targetId, edgeType)
                End If
            Next

            Return graph
        End Function

        ''' <summary>
        ''' Build a demo KEGG graph for testing purposes.
        ''' This creates a small but realistic multi-layer graph that mimics
        ''' the structure of a real KEGG metabolic network.
        ''' 
        ''' The demo graph contains:
        ''' - 3 pathways (Glycolysis, TCA Cycle, Purine Metabolism)
        ''' - 2 modules
        ''' - 5 enzymes
        ''' - 6 reactions
        ''' - 10 compounds
        ''' </summary>
        Public Shared Function BuildDemoGraph() As KeggGraph
            Dim graph As New KeggGraph()

            ' === Pathways ===
            graph.AddNode("hsa00010", "Glycolysis / Gluconeogenesis", KeggNodeType.Pathway)
            graph.AddNode("hsa00020", "Citrate cycle (TCA cycle)", KeggNodeType.Pathway)
            graph.AddNode("hsa00230", "Purine metabolism", KeggNodeType.Pathway)

            ' === Modules ===
            graph.AddNode("M00001", "Glycolysis (Embden-Meyerhof pathway)", KeggNodeType.Module)
            graph.AddNode("M00009", "Citrate cycle (TCA cycle, Krebs cycle)", KeggNodeType.Module)

            ' === Enzymes ===
            graph.AddNode("2.7.1.1", "Hexokinase (HK)", KeggNodeType.Enzyme)
            graph.AddNode("5.3.1.9", "Glucose-6-phosphate isomerase (GPI)", KeggNodeType.Enzyme)
            graph.AddNode("4.1.2.13", "Fructose-bisphosphate aldolase (ALDO)", KeggNodeType.Enzyme)
            graph.AddNode("1.1.1.37", "Malate dehydrogenase (MDH)", KeggNodeType.Enzyme)
            graph.AddNode("2.7.4.6", "Nucleoside-diphosphate kinase (NDK)", KeggNodeType.Enzyme)

            ' === Reactions ===
            graph.AddNode("R01786", "Glucose + ATP => G6P + ADP", KeggNodeType.Reaction)
            graph.AddNode("R02740", "G6P <=> F6P", KeggNodeType.Reaction)
            graph.AddNode("R01070", "F1,6BP <=> DHAP + G3P", KeggNodeType.Reaction)
            graph.AddNode("R00342", "Malate + NAD+ <=> Oxaloacetate + NADH + H+", KeggNodeType.Reaction)
            graph.AddNode("R00127", "ATP + AMP <=> 2 ADP", KeggNodeType.Reaction)
            graph.AddNode("R01126", "GTP + ADP <=> GDP + ATP", KeggNodeType.Reaction)

            ' === Compounds ===
            graph.AddNode("C00031", "D-Glucose", KeggNodeType.Compound)
            graph.AddNode("C00002", "ATP", KeggNodeType.Compound)
            graph.AddNode("C00092", "D-Glucose 6-phosphate", KeggNodeType.Compound)
            graph.AddNode("C00085", "D-Fructose 6-phosphate", KeggNodeType.Compound)
            graph.AddNode("C00354", "D-Fructose 1,6-bisphosphate", KeggNodeType.Compound)
            graph.AddNode("C00111", "Glycerone phosphate (DHAP)", KeggNodeType.Compound)
            graph.AddNode("C00118", "D-Glyceraldehyde 3-phosphate", KeggNodeType.Compound)
            graph.AddNode("C00149", "Malate", KeggNodeType.Compound)
            graph.AddNode("C00036", "Oxaloacetate", KeggNodeType.Compound)
            graph.AddNode("C00008", "ADP", KeggNodeType.Compound)

            ' === Edges: Pathway -> Module ===
            graph.AddEdge("hsa00010", "M00001", "pathway-module")
            graph.AddEdge("hsa00020", "M00009", "pathway-module")

            ' === Edges: Module -> Enzyme ===
            graph.AddEdge("M00001", "2.7.1.1", "module-enzyme")
            graph.AddEdge("M00001", "5.3.1.9", "module-enzyme")
            graph.AddEdge("M00001", "4.1.2.13", "module-enzyme")
            graph.AddEdge("M00009", "1.1.1.37", "module-enzyme")
            graph.AddEdge("hsa00230", "2.7.4.6", "pathway-enzyme")

            ' === Edges: Enzyme -> Reaction ===
            graph.AddEdge("2.7.1.1", "R01786", "enzyme-reaction")
            graph.AddEdge("5.3.1.9", "R02740", "enzyme-reaction")
            graph.AddEdge("4.1.2.13", "R01070", "enzyme-reaction")
            graph.AddEdge("1.1.1.37", "R00342", "enzyme-reaction")
            graph.AddEdge("2.7.4.6", "R01126", "enzyme-reaction")

            ' === Edges: Reaction -> Compound ===
            graph.AddEdge("R01786", "C00031", "reaction-compound")
            graph.AddEdge("R01786", "C00002", "reaction-compound")
            graph.AddEdge("R01786", "C00092", "reaction-compound")
            graph.AddEdge("R01786", "C00008", "reaction-compound")
            graph.AddEdge("R02740", "C00092", "reaction-compound")
            graph.AddEdge("R02740", "C00085", "reaction-compound")
            graph.AddEdge("R01070", "C00354", "reaction-compound")
            graph.AddEdge("R01070", "C00111", "reaction-compound")
            graph.AddEdge("R01070", "C00118", "reaction-compound")
            graph.AddEdge("R00342", "C00149", "reaction-compound")
            graph.AddEdge("R00342", "C00036", "reaction-compound")
            graph.AddEdge("R01126", "C00002", "reaction-compound")
            graph.AddEdge("R01126", "C00008", "reaction-compound")

            ' === Additional cross-pathway edges ===
            ' Purine metabolism connects to ATP/ADP
            graph.AddEdge("R00127", "C00002", "reaction-compound")
            graph.AddEdge("R00127", "C00008", "reaction-compound")
            graph.AddEdge("hsa00230", "R00127", "pathway-reaction")

            Return graph
        End Function

        ''' <summary>
        ''' Build a FELLA data object from a KEGG graph.
        ''' This precomputes all matrices needed for enrichment analysis.
        ''' </summary>
        Public Shared Function BuildDataFromGraph(graph As KeggGraph,
                                                    Optional dampingFactor As Double = 0.85,
                                                    Optional gamma As Double = 1.0,
                                                    Optional nInput As Integer = 5,
                                                    Optional buildHypergeom As Boolean = True,
                                                    Optional buildDiffusion As Boolean = True,
                                                    Optional buildPagerank As Boolean = True) As FellaData
            Dim data As New FellaData()

            data.Graph = graph
            data.DampingFactor = dampingFactor
            data.Gamma = gamma

            ' Set background compounds
            Dim compounds = graph.GetNodesByType(KeggNodeType.Compound)
            data.BackgroundCompounds = New HashSet(Of String)(compounds.Select(Function(c) c.Id))

            ' Build precomputed matrices
            data.BuildAll(nInput, buildHypergeom, buildDiffusion, buildPagerank)

            Return data
        End Function

    End Class

End Namespace
