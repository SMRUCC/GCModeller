#Region "Microsoft.VisualBasic::399031b850fdd7336fc651d638883ab8, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\WGCNA\API.vb"

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

'     Module API
' 
'         Function: CallInvoke, CreateObject, GetValue
' 
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports RDotNET.Extensions.VisualBasic
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.Network
Imports SMRUCC.genomics.Analysis.RNA_Seq.WGCNA

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

    <ExportAPI("WGCNA.GetValue")>
    Public Function GetValue([operator] As WGCNAWeight, id1 As String, id2 As String) As Double
        Return [operator].Find(id1, id2).Weight
    End Function

    <ExportAPI("Read.WGCNA.Weights")>
    Public Function CreateObject(Path As String) As WGCNAWeight
        If String.IsNullOrEmpty(Path) Then
            Return New WGCNAWeight With {
                .PairItems = New Weight() {}
            }
        End If
        Return New WGCNAWeight With {
            .PairItems = Path.AsDataSource(Of Weight)(" ", False)
        }
    End Function

    <ExportAPI("WGCNA.Fast.Imports")>
    Public Function FastImports(Path As String) As WGCNAWeight
        If Not Path.FileExists Then
            Call VBDebugger.Warning($"{Path.ToFileURL} is not exists on the file system!")
            Return New WGCNAWeight
        End If

        Dim Lines As String() = IO.File.ReadAllLines(Path)
        Dim Tokens = Lines.Skip(1).Select(Function(line) Strings.Split(line, vbTab)).ToArray
        Dim weights As Weight() =
            Tokens.Select(
                Function(line) New Weight With {
                    .FromNode = line(Scan0),
                    .ToNode = line(1),
                    .Weight = Val(line(2))}).ToArray
        Return New WGCNAWeight With {.PairItems = weights}
    End Function

    Const DEFAULT_COLORS As String = "yellow|blue|grey|pink|red|black|turquoise|midnightblue|brown|magenta|purple|cyan|greenyellow|green|tan|salmon"

    ''' <summary>
    ''' Applying the WGCNA analysis on your transcriptome data.
    ''' </summary>
    ''' <param name="dataExpr">
    ''' The text encoding of this document should be ASCII, or the data reading in the R will be failed!
    ''' (转录组数据的csv文件的位置，请注意！，数据文件都必须是ASCII编码的)
    ''' </param>
    ''' <param name="GeneIdLabel">使用这个参数来修改Id映射</param>
    ''' <returns>
    ''' 函数返回的是最终的WGCNA导出到Cytoscape的网络模型文件的文件路径，假若脚本执行失败，则返回空字符串
    ''' </returns>
    ''' 
    <ExportAPI("WGCNA.Analysis")>
    Public Function CallInvoke(<Parameter("dataExpr.csv",
                                          "The csv document file path for the WGCNA data source. Which the first column in the document should be the genes locus_tag and the first row is the experiment title list.")>
                               dataExpr As String,
                               <Parameter("annotations.csv",
                                          "The csv document file path for the gene annotation data source, this file should contains at least two columns which one of the column should named Id for the genes' locus_tag and named gene_symbol for the gene name.")>
                               annotations As String,
                               <Parameter("DIR.Export",
                                          "Export the saved data and image plots to this directory, default location is current directory.")>
                               Optional outDIR As String = "./",
                               Optional GeneIdLabel As String = "GeneId",
                               <Parameter("list.mod", "Module was represents in colors, using | as seperator.")>
                               Optional modules As String = DEFAULT_COLORS) As String

        Dim WGCNA As StringBuilder = New StringBuilder(My.Resources.WGCNA)
        outDIR = outDIR.GetDirectoryFullPath
        Call WGCNA.Replace("[dataExpr]", dataExpr.GetFullPath)
        Call WGCNA.Replace("[WORK]", outDIR)
        Call WGCNA.Replace("[GeneId_LABEL]", GeneIdLabel)
        Call WGCNA.Replace("[TOMsave]", BaseName(dataExpr) & ".TOMsave")
        Call WGCNA.Replace("[Annotations.csv]", annotations.GetFullPath)

        Dim mods As String() = modules.ToLower.Trim.Split("|"c).Select(Function(sCl) $"""{sCl}""")
        Call WGCNA.Replace("[list.MODs]", String.Join(", ", mods))
        Call WGCNA.SaveTo($"{outDIR}/{BaseName(dataExpr)}.WGCNACallInvoke.R", System.Text.Encoding.ASCII)

        Call dataExpr.TransEncoding(Encodings.ASCII)
        Call annotations.TransEncoding(Encodings.ASCII)

#If DEBUG Then
            Call My.Computer.FileSystem.CurrentDirectory.__DEBUG_ECHO
#End If
        Dim STD As String() = R.WriteLine(WGCNA.ToString)
        Dim Cytoscape As String = outDIR & "/CytoscapeEdges.txt"

        Call STD.SaveTo(outDIR & "/WGCNA.STD.txt")

        If Not Cytoscape.FileExists Then
            Return ""
        Else
            Return Cytoscape
        End If
    End Function
End Module
