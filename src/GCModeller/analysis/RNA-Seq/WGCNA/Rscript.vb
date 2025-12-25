#Region "Microsoft.VisualBasic::99b703267b590bc82becd991c877a004, analysis\RNA-Seq\WGCNA\Rscript.vb"

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

'   Total Lines: 99
'    Code Lines: 93 (93.94%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 6 (6.06%)
'     File Size: 8.89 KB


' Module Rscript
' 
'     Function: FastImports
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework.IO.CSVFile
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.Network

<Package("WGCNA", Cites:="Langfelder, P. and S. Horvath (2008). ""WGCNA: an R package For weighted correlation network analysis."" BMC Bioinformatics 9: 559.
<p>BACKGROUND: Correlation networks are increasingly being used in bioinformatics applications. For example, weighted gene co-expression network analysis is a systems biology method for describing the correlation patterns among genes across microarray samples. Weighted correlation network analysis (WGCNA) can be used for finding clusters (modules) of highly correlated genes, for summarizing such clusters using the module eigengene or an intramodular hub gene, for relating modules to one another and to external sample traits (using eigengene network methodology), and for calculating module membership measures. Correlation networks facilitate network based gene screening methods that can be used to identify candidate biomarkers or therapeutic targets. These methods have been successfully applied in various biological contexts, e.g. cancer, mouse genetics, yeast genetics, and analysis of brain imaging data. While parts of the correlation network methodology have been described in separate publications, there is a need to provide a user-friendly, comprehensive, and consistent software implementation and an accompanying tutorial. RESULTS: The WGCNA R software package is a comprehensive collection of R functions for performing various aspects of weighted correlation network analysis. The package includes functions for network construction, module detection, gene selection, calculations of topological properties, data simulation, visualization, and interfacing with external software. Along with the R package we also present R software tutorials. While the methods development was motivated by gene expression data, the underlying data mining approach can be applied to a variety of different settings. CONCLUSION: The WGCNA package provides R functions for weighted correlation network analysis, e.g. co-expression network analysis of gene expression data. The R package along with its source code and additional material are freely available at http://www.genetics.ucla.edu/labs/horvath/CoexpressionNetwork/Rpackages/WGCNA.

", Url:="http://www.genetics.ucla.edu/labs/horvath/CoexpressionNetwork/Rpackages/WGCNA",
                Category:=APICategories.ResearchTools,
                Publisher:="Peter Langfelder - Peter.Langfelder@gmail.com; 
<br />Steve Horvath* - shorvath@mednet.ucla.edu")>
<Cite(Title:="WGCNA: an R package for weighted correlation network analysis",
      URL:="http://www.genetics.ucla.edu/labs/horvath/CoexpressionNetwork/Rpackages/WGCNA",
      Pages:="559",
      Year:=2008,
      Volume:=9,
      ISSN:="1471-2105 (Electronic)
1471-2105 (Linking)",
      Journal:="BMC bioinformatics",
      Issue:="",
      AuthorAddress:="Department of Human Genetics and Department of Biostatistics, University of California, Los Angeles, CA 90095, USA. Peter.Langfelder@gmail.com",
      Abstract:="BACKGROUND: Correlation networks are increasingly being used in bioinformatics applications. For example, weighted gene co-expression network analysis is a systems biology method for describing the correlation patterns among genes across microarray samples. 
Weighted correlation network analysis (WGCNA) can be used for finding clusters (modules) of highly correlated genes, for summarizing such clusters using the module eigengene or an intramodular hub gene, for relating modules to one another and to external sample traits (using eigengene network methodology), and for calculating module membership measures. 
Correlation networks facilitate network based gene screening methods that can be used to identify candidate biomarkers or therapeutic targets. 
These methods have been successfully applied in various biological contexts, e.g. cancer, mouse genetics, yeast genetics, and analysis of brain imaging data. While parts of the correlation network methodology have been described in separate publications, there is a need to provide a user-friendly, comprehensive, and consistent software implementation and an accompanying tutorial. 
     
<p><p>RESULTS: The WGCNA R software package is a comprehensive collection of R functions for performing various aspects of weighted correlation network analysis. 
The package includes functions for network construction, module detection, gene selection, calculations of topological properties, data simulation, visualization, and interfacing with external software. 
Along with the R package we also present R software tutorials. While the methods development was motivated by gene expression data, the underlying data mining approach can be applied to a variety of different settings. 
<p><p>CONCLUSION: The WGCNA package provides R functions for weighted correlation network analysis, e.g. co-expression network analysis of gene expression data. 
The R package along with its source code and additional material are freely available at <p>http://www.genetics.ucla.edu/labs/horvath/CoexpressionNetwork/Rpackages/WGCNA.",
      Authors:="Langfelder, P.
Horvath, S.",
      DOI:="10.1186/1471-2105-9-559",
      Keywords:="Algorithms
Animals
Computational Biology/*methods
Computer Graphics
*Computing Methodologies
Databases, Genetic
Gene Expression Profiling/methods
Humans
Mice
Oligonucleotide Array Sequence Analysis/*methods
Pattern Recognition, Automated
Programming Languages
*Software
Systems Biology",
      PubMed:=19114008)>
<Cite(Title:="Weighted correlation network analysis (WGCNA) applied to the tomato fruit metabolome",
      Pages:="e26683",
      Notes:="",
      Issue:="10",
      ISSN:="1932-6203 (Electronic)
1932-6203 (Linking)",
      DOI:="10.1371/journal.pone.0026683",
      Authors:="DiLeo, M. V.
Strahan, G. D.
den Bakker, M.
Hoekenga, O. A.",
      AuthorAddress:="Boyce Thompson Institute for Plant Research, Ithaca, New York, United States of America.",
      Abstract:="BACKGROUND: Advances in ""omics"" technologies have revolutionized the collection of biological data. 
A matching revolution in our understanding of biological systems, however, will only be realized when similar advances are made in informatic analysis of the resulting ""big data."" 
Here, we compare the capabilities of three conventional and novel statistical approaches to summarize and decipher the tomato metabolome. 
<p><p>METHODOLOGY: Principal component analysis (PCA), batch learning self-organizing maps (BL-SOM) and weighted gene co-expression network analysis (WGCNA) were applied to a multivariate NMR dataset collected from developmentally staged tomato fruits belonging to several genotypes. 
While PCA and BL-SOM are appropriate and commonly used methods, WGCNA holds several advantages in the analysis of highly multivariate, complex data. 
<p><p>CONCLUSIONS: PCA separated the two major genetic backgrounds (AC and NC), but provided little further information. 
Both BL-SOM and WGCNA clustered metabolites by expression, but WGCNA additionally defined ""modules"" of co-expressed metabolites explicitly and provided additional network statistics that described the systems properties of the tomato metabolic network. 
Our first application of WGCNA to tomato metabolomics data identified three major modules of metabolites that were associated with ripening-related traits and genetic background.",
      Journal:="PloS one",
      Keywords:="Lycopersicon esculentum/*metabolism
*Metabolome
Nuclear Magnetic Resonance, Biomolecular
Principal Component Analysis",
      URL:="",
      Year:=2011, Volume:=6, PubMed:=22039529)>
Public Module Rscript

    ''' <summary>
    ''' load wgcna weight matrix
    ''' </summary>
    ''' <param name="files">weight edge data in different modules result folder</param>
    ''' <param name="threshold"></param>
    ''' <param name="prefix"></param>
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
    Public Function FastImports(files As IEnumerable(Of String),
                                Optional threshold As Double = 0,
                                Optional prefix$ = Nothing) As WGCNAWeight

        Dim fileSet As String() = files.ToArray
        Dim pullAll As IEnumerable(Of Weight) = fileSet _
            .Select(Function(path)
                        Return LoadTOMWeights(path, threshold, prefix)
                    End Function) _
            .IteratesALL
        Dim cor As WGCNAWeight = WGCNAWeight.CreateMatrix(pullAll)

        Return cor
    End Function

    ''' <summary>
    ''' load wgcna weight matrix
    ''' </summary>
    ''' <param name="path"></param>
    ''' <param name="threshold"></param>
    ''' <param name="prefix"></param>
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
    Public Function FastImports(path As String, Optional threshold As Double = 0, Optional prefix$ = Nothing) As WGCNAWeight
        Return WGCNAWeight.CreateMatrix(dataSet:=LoadTOMWeights(path, threshold, prefix))
    End Function

    <Extension>
    Public Function AsDataMatrix(wgcna As WGCNAWeight) As DataMatrix
        Dim names As String() = wgcna.geneSet.ToArray
        Dim w As Double()() = RectangularArray.Matrix(Of Double)(names.Length, names.Length)

        For i As Integer = 0 To names.Length - 1
            Dim u As String = names(i)
            Dim wij As Double() = w(i)

            For j As Integer = 0 To names.Length - 1
                wij(j) = wgcna(u, names(j))
            Next
        Next

        Return New DataMatrix(names, w)
    End Function

    Private Function LoadTOMWeights(path As String, threshold As Double, prefix$) As IEnumerable(Of Weight)
        Dim headers As Index(Of String) = Tokenizer.CharsParser(path.ReadFirstLine, delimiter:=ASCII.TAB).Indexing
        Dim fromNode As Integer = headers(NameOf(fromNode))
        Dim toNode As Integer = headers(NameOf(toNode))
        Dim weight As Integer = headers(NameOf(weight))
        Dim direction As Integer = headers(NameOf(direction))
        Dim fromAltName As Integer = headers(NameOf(fromAltName))
        Dim toAltName As Integer = headers(NameOf(toAltName))
        Dim weights As IEnumerable(Of Weight) =
            From line As String
            In path.IterateAllLines(tqdm_wrap:=True).Skip(1).AsParallel
            Let data As String() = Tokenizer.CharsParser(line, ASCII.TAB).ToArray
            Let w As Double = Double.Parse(data(weight))
            Where w > threshold
            Select New Weight With {
                .weight = w,
                .direction = If(direction >= 0, data(direction), Nothing),
                .fromAltName = If(fromAltName >= 0, data(fromAltName), Nothing),
                .fromNode = If(prefix Is Nothing, data(fromNode), prefix & data(fromNode)),
                .toAltName = If(toAltName >= 0, data(toAltName), Nothing),
                .toNode = If(prefix Is Nothing, data(toNode), prefix & data(toNode))
            }

        Return weights
    End Function

    ''' <summary>
    ''' load network graph from the WGCNA exportNetworkToCytoscape function exports
    ''' </summary>
    ''' <param name="edges"></param>
    ''' <param name="nodes"></param>
    ''' <param name="threshold"></param>
    ''' <param name="prefix"></param>
    ''' <returns></returns>
    Public Function LoadTOMModuleGraph(edges As String, nodes As String, Optional threshold As Double = 0, Optional prefix$ = Nothing) As NetworkGraph
        Return LoadTOMModuleGraph(LoadTOMWeights(edges, threshold, prefix), WGCNAModules.LoadModules(nodes))
    End Function

    <Extension>
    Public Function LoadTOMModuleGraph(edges As IEnumerable(Of Weight), nodes As IEnumerable(Of CExprMods)) As NetworkGraph
        Dim g As New NetworkGraph

        For Each node As CExprMods In nodes
            Call g.CreateNode(node.nodeName, New NodeData With {
                .label = node.nodeName,
                .origID = node.nodeName,
                .Properties = New Dictionary(Of String, String) From {
                    {"module", node.nodesPresent}
                }
            })
        Next

        For Each edge As Weight In edges
            Call g.CreateEdge(
                g.GetElementByID(edge.fromNode),
                g.GetElementByID(edge.toNode),
                weight:=edge.weight,
                data:=New EdgeData With {
                    .label = $"{edge.fromNode}--{edge.toNode}",
                    .Properties = New Dictionary(Of String, String) From {
                        {"fromAltName", edge.fromAltName},
                        {"toAltName", edge.toAltName},
                        {"direction", edge.direction}
                    }
                })
        Next

        Return g
    End Function
End Module
