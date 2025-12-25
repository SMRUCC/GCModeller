#Region "Microsoft.VisualBasic::e4e38aabd98e19e760767e65622e4480, R#\phenotype_kit\WGCNA.vb"

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


' Code Statistics:

'   Total Lines: 122
'    Code Lines: 52 (42.62%)
' Comment Lines: 63 (51.64%)
'    - Xml Docs: 82.54%
' 
'   Blank Lines: 7 (5.74%)
'     File Size: 7.23 KB


' Module WGCNA
' 
'     Function: applyModuleColors, readModules, readWeightMatrix, runAnalysis
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.HTS.WGCNA
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.Network
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports any = Microsoft.VisualBasic.Scripting
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

''' <summary>
''' WGCNA, which stands for Weighted Gene Co-expression Network Analysis, is a systems biology method used to describe the 
''' correlation patterns among genes across different samples. It is particularly useful for identifying modules of 
''' co-expressed genes, which can then be correlated with external sample traits such as clinical features or environmental 
''' conditions. Here's a brief overview of how WGCNA works and its applications:
''' 
''' ### Key Concepts:
''' 
''' 1. **Co-expression Networks**: Genes that are co-expressed across different conditions or samples are likely to be 
'''    functionally related. WGCNA constructs a network where nodes represent genes, and edges represent the pairwise 
'''    correlations between genes.
''' 2. **Weighted Networks**: Traditional correlation-based networks use Pearson or Spearman correlations, which are 
'''    unweighted. WGCNA uses a weighted approach, often employing the soft thresholding of the correlation matrix to 
'''    transform it into a weighted adjacency matrix. This weighting helps to amplify strong correlations and diminish 
'''    weak ones, making the network more robust to noise.
''' 3. **Modules**: Groups of highly correlated genes are identified as modules. These modules are clusters of genes 
'''    that have similar expression profiles across the samples and are often enriched for specific biological functions 
'''    or pathways.
''' 4. **Topological Overlap Matrix (TOM)**: WGCNA uses the TOM to measure the network connectivity of genes, which 
'''    considers not only direct connections but also shared neighbors. This helps in identifying modules more accurately.
''' 5. **Eigengenes**: Each module can be represented by an eigengene, which is the first principal component of the gene
'''    expression profiles within the module. The eigengene serves as a representative of the module's expression pattern.
'''    
''' ### Steps in WGCNA:
''' 
''' 1. **Data Preprocessing**: This includes filtering out low-quality genes, normalizing expression data, and handling missing values.
''' 2. **Network Construction**: Calculate the pairwise correlation matrix and apply soft thresholding to create a weighted adjacency matrix.
''' 3. **Module Detection**: Use hierarchical clustering or other clustering methods on the TOM to identify modules of co-expressed genes.
''' 4. **Module Eigengenes**: Compute the eigengene for each module to represent its expression pattern.
''' 5. **Relating Modules to External Traits**: Correlate module eigengenes with external sample traits to identify which modules are associated with specific conditions or phenotypes.
''' 6. **Functional Enrichment Analysis**: Perform gene ontology (GO) or pathway enrichment analysis on the genes within each 
'''    module to infer their biological functions.
'''    
''' ### Applications:
''' 
''' - **Disease Biomarker Discovery**: Identifying gene modules associated with disease states can lead to the discovery of novel biomarkers.
''' - **Understanding Disease Mechanisms**: By analyzing the functions of co-expressed gene modules, researchers can gain insights into the molecular mechanisms underlying diseases.
''' - **Drug Target Identification**: Modules that are significantly altered in disease states may contain potential drug targets.
''' - **Comparative Analysis**: WGCNA can be used to compare gene expression patterns across different species, tissues, or conditions.
''' 
''' ### Tools and Software:
''' 
''' WGCNA is implemented in R, and there is a comprehensive package available for users to perform the analysis. The package provides 
''' functions for all steps of the analysis, from data preprocessing to module detection and trait correlation.
''' 
''' ### Limitations:
''' 
''' - **Sample Size**: WGCNA requires a sufficient number of samples to reliably detect co-expression patterns.
''' - **Interpretation**: While WGCNA can identify co-expressed modules, interpreting their biological significance often requires additional functional validation.
''' - **Computational Intensity**: The analysis can be computationally intensive, especially with large datasets.
''' 
''' WGCNA is a powerful tool for exploring gene co-expression patterns and has been widely used in genomics research to uncover 
''' the underlying biology of complex traits and diseases.
''' </summary>
<Package("WGCNA")>
Module WGCNA

    ''' <summary>
    ''' load TOM module network nodes
    ''' </summary>
    ''' <param name="file">
    ''' the TOM network nodes text file, should be a tsv file of the cytoscape network export result
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("read.modules")>
    Public Function readModules(<RRawVectorArgument> file As Object, Optional prefix$ = Nothing, Optional result_modules As Boolean = False) As Object
        Dim all = CLRVector.asCharacter(file) _
            .Select(Function(path)
                        Return WGCNAModules.LoadModules(path)
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(g) g.nodeName) _
            .Select(Function(g) g.First) _
            .ToArray
        Dim result As list = list.empty

        If result_modules Then
            For Each gene As CExprMods In all
                Dim key_ref As String = If(prefix Is Nothing, gene.nodeName, prefix & gene.nodeName)

                Call result.add(key_ref, New ClusterModuleResult With {
                    .color = gene.nodesPresent,
                    .gene_id = key_ref,
                    .[module] = 0
                })
            Next
        Else
            For Each gene As CExprMods In all
                Call result.add(If(prefix Is Nothing, gene.nodeName, prefix & gene.nodeName), gene.nodesPresent)
            Next
        End If

        Return result
    End Function

    <ExportAPI("read_clusters")>
    Public Function readClusterModuleResult(file As String, Optional prefix$ = Nothing) As ClusterModuleResult()
        Return ClusterModuleResult.LoadTable(tsv:=file, prefix).ToArray
    End Function

    ''' <summary>
    ''' read the TOM correlation network matrix file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="threshold"></param>
    ''' <param name="prefix">
    ''' a prefix to the fromNode and toNode id
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' imports a network edge table file that export from WGCNA TOM module, with data headers: 
    ''' <br /><br />
    ''' fromNode<br />
    ''' toNode<br />
    ''' weight<br />
    ''' direction<br />
    ''' fromAltName<br />
    ''' toAltName<br />
    ''' </remarks>
    <ExportAPI("read.weight_matrix")>
    <RApiReturn(GetType(WGCNAWeight), GetType(DataMatrix))>
    Public Function readWeightMatrix(<RRawVectorArgument> file As Object,
                                     Optional threshold As Double = 0,
                                     Optional prefix$ = Nothing,
                                     Optional as_matrix As Boolean = False) As Object

        Dim wgcna As WGCNAWeight = FastImports(CLRVector.asCharacter(file), threshold:=threshold, prefix:=prefix)

        If as_matrix Then
            Return wgcna.AsDataMatrix
        Else
            Return wgcna
        End If
    End Function

    ''' <summary>
    ''' load network graph from the WGCNA exportNetworkToCytoscape function exports
    ''' </summary>
    ''' <param name="edges"></param>
    ''' <param name="nodes"></param>
    ''' <param name="threshold"></param>
    ''' <param name="prefix$"></param>
    ''' <returns></returns>
    ''' 
    <ExportAPI("load_TOM_graph")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function LoadTOMModuleGraph_call(edges As String, nodes As String, Optional threshold As Double = 0, Optional prefix$ = Nothing) As NetworkGraph
        Return LoadTOMModuleGraph(edges, nodes, threshold, prefix)
    End Function

    ''' <summary>
    ''' export a dataframe of the node information with connectivity value 
    ''' </summary>
    ''' <param name="x"></param>
    ''' <returns></returns>
    <ExportAPI("connectivity")>
    Public Function connectivitySummary(x As NetworkGraph) As Object
        Dim nodes As Node() = x.vertex.ToArray
        Dim df As New dataframe With {
            .rownames = nodes.Select(Function(v) v.label).ToArray,
            .columns = New Dictionary(Of String, Array)
        }

        Call df.add("connectivity", From v As Node In nodes Select v.directedVertex.connectivity)
        Call df.add("degree", From v As Node In nodes Select v.directedVertex.inDegree + v.directedVertex.outDegree)
        Call df.add("module", From v As Node In nodes Select v!module)

        Return df
    End Function

    <ExportAPI("applyModuleColors")>
    Public Function applyModuleColors(g As NetworkGraph, modules As list) As Object
        For Each geneId As String In modules.getNames
            If Not g.GetElementByID(geneId) Is Nothing Then
                g.GetElementByID(geneId).data.color = any.ToString(modules(geneId)).GetBrush
            End If
        Next

        Return g
    End Function

    ''' <summary>
    ''' Create correlation network based on WGCNA method
    ''' </summary>
    ''' <param name="x">
    ''' should be an expression matrix object of gene features in rows and sample id in columns
    ''' </param>
    ''' <param name="adjacency"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("cor_network")>
    <RApiReturn(GetType(Result))>
    Public Function runAnalysis(x As Matrix,
                                Optional adjacency As Double = 0.6,
                                Optional pca_layout As Boolean = True,
                                Optional env As Environment = Nothing) As Object

        Return Analysis.Run(x, adjacency, pca_layout)
    End Function
End Module
