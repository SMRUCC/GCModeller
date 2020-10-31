Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\eggHTS.exe

' 
'  // 
'  // SMRUCC genomics GCModeller Programs Profiles Manager
'  // 
'  // VERSION:   3.3277.7609.23259
'  // ASSEMBLY:  Settings, Version=3.3277.7609.23259, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright (c) SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
'  // BUILT:     10/31/2020 12:55:18 PM
'  // 
' 
' 
'  Custom KO classification set can be download from: http://www.kegg.jp/kegg-bin/get_htext?ko00001.keg.
'  You can replace the %s mark using kegg organism code in url example as: http://www.kegg.jp/kegg-bin/download_htext?htext=%s00001&format=htext&filedir=
'  for download the custom KO classification set.
' 
' 
' SYNOPSIS
' Settings command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /COG.profiling.plot:                    Plots the COGs category statics profiling of the target genome
'                                          from the COG annotation file.
'  /DEPs.takes.values:                     
'  /DEPs.union:                            
'  /Exocarta.Hits:                         
'  /Fasta.IDlist:                          
'  /iBAQ.Cloud:                            Cloud plot of the iBAQ DEPs result.
'  /KEGG.enrichment.profile:               
'  /KO.Catalogs:                           Display the barplot of the KEGG orthology match.
'  /KOBAS.Sim.Heatmap:                     
'  /KOBAS.similarity:                      
'  /KOBAS.Term.Kmeans:                     
'  /Network.PCC:                           
'  /paired.sample.designer:                
'  /Perseus.MajorityProteinIDs:            Export the uniprot ID list from ``Majority Protein IDs``
'                                          row and generates a text file for batch search of the uniprot
'                                          database.
'  /Perseus.Table.annotations:             
'  /pfamstring.enrichment:                 
'  /protein.annotations.shotgun:           
'  /UniProt.ID.Maps:                       
'  /UniRef.map.organism:                   
' 
' 
' API list that with functional grouping
' 
' 1. 0. Samples CLI tools
' 
' 
'    /Data.Add.Mappings:                     
'    /Data.Add.ORF:                          
'    /Data.Add.uniprotIDs:                   
'    /Perseus.Stat:                          
'    /Perseus.Table:                         
'    /plot.pimw:                             'calc. pI' - 'MW [kDa]' scatter plot of the protomics raw
'                                            sample data.
'    /Sample.Species.Normalization:          
'    /Shotgun.Data.Strip:                    
' 
' 
' 2. 0. Samples expression analysis
' 
' 
'    /FoldChange.Matrix.Invert:              Reverse the FoldChange value from the source result matrix.
'    /proteinGroups.venn:                    
'    /Relative.amount:                       Statistics of the relative expression value of the total
'                                            proteins.
' 
' 
' 3. 1. uniprot annotation CLI tools
' 
' 
'    /blastX.fill.ORF:                       
'    /ID.Replace.bbh:                        LabelFree result helper: replace the source ID to a unify
'                                            organism protein ID by using ``bbh`` method.
'                                            This tools required the protein in ``datatset.csv``
'                                            associated with the alignment result in ``bbh.csv`` by using
'                                            the ``query_name`` property.
'    /KEGG.Color.Pathway:                    
'    /protein.annotations:                   Total proteins functional annotation by using uniprot database.
'    /proteins.Go.plot:                      ProteinGroups sample data go profiling plot from the uniprot
'                                            annotation data.
'    /proteins.KEGG.plot:                    KEGG function catalog profiling plot of the TP sample.
'    /Samples.IDlist:                        Extracts the protein hits from the protomics sample data,
'                                            and using this ID list for downlaods the uniprot annotation
'                                            data.
'    /Species.Normalization:                 
'    /Uniprot.Mappings:                      Retrieve the uniprot annotation data by using ID mapping
'                                            operations.
'    /UniRef.UniprotKB:                      
'    /update.uniprot.mapped:                 
' 
' 
' 4. 2. DEP analysis CLI tools
' 
' 
'    /DEP.logFC.hist:                        Using for plots the FC histogram when the experiment have
'                                            no biological replicates.
'    /DEP.logFC.Volcano:                     Volcano plot of the DEPs' analysis result.
'    /DEP.uniprot.list:                      
'    /DEP.uniprot.list2:                     
'    /DEP.venn:                              Generate the VennDiagram plot data and the venn plot tiff.
'                                            The default parameter profile is using for the iTraq data.
'    /DEPs.heatmap:                          Generates the heatmap plot input data. The default label
'                                            profile is using for the iTraq result.
'    /DEPs.stat:                             https://github.com/xieguigang/GCModeller.cli2R/blob/master/GCModeller.cli2R/R/log2FC_t-test.R
'    /edgeR.Designer:                        Generates the edgeR inputs table
'    /Merge.DEPs:                            Usually using for generates the heatmap plot matrix of the
'                                            DEPs. This function call will generates two dataset, one
'                                            is using for the heatmap plot and another is using for the
'                                            venn diagram plot.
'    /T.test.Designer.iTraq:                 Generates the iTraq data t.test DEP method inputs table
'    /T.test.Designer.LFQ:                   Generates the LFQ data t.test DEP method inputs table
' 
' 
' 5. 3. Enrichment analysis tools
' 
' 
'    /Converts:                              Converts the GCModeller enrichment analysis output as the
'                                            KOBAS enrichment analysis result output table.
'    /Enrichment.Term.Filter:                Filter the specific term result from the analysis output
'                                            by using pattern keyword
'    /Enrichments.ORF.info:                  Retrive KEGG/GO info for the genes in the enrichment result.
'    /GO.cellular_location.Plot:             Visualize of the subcellular location result from the GO
'                                            enrichment analysis.
'    /Go.enrichment.plot:                    Go enrichment plot base on the KOBAS enrichment analysis
'                                            result.
'    /KEGG.Enrichment.PathwayMap:            Show the KEGG pathway map image by using KOBAS KEGG pathway
'                                            enrichment result.
'    /KEGG.Enrichment.PathwayMap.Render:     KEGG pathway map enrichment analysis visual rendering locally.
'                                            This function required a local kegg pathway repository.
'    /KEGG.enrichment.plot:                  Bar plots of the KEGG enrichment analysis result.
'    /KOBAS.add.ORF:                         
'    /KOBAS.split:                           Split the KOBAS run output result text file as seperated
'                                            csv file.
' 
' 
' 6. 3. Enrichment analysis tools: clusterProfiler
' 
' 
'    /enricher.background:                   Create enrichment analysis background based on the uniprot
'                                            xml database.
'    /Term2genes:                            
' 
' 
' 7. 3. Enrichment analysis tools: DAVID
' 
' 
'    /DAVID.Split:                           
'    /GO.enrichment.DAVID:                   
'    /KEGG.enrichment.DAVID:                 
'    /KEGG.enrichment.DAVID.pathwaymap:      
' 
' 
' 8. 4. Network enrichment visualize tools
' 
' 
'    /func.rich.string:                      DEPs' functional enrichment network based on string-db exports,
'                                            and color by KEGG pathway.
'    /Gene.list.from.KOBAS:                  Using this command for generates the gene id list input for
'                                            the STRING-db search.
'    /richfun.KOBAS:                         
' 
' 
' 9. Data visualization tool
' 
' 
'    /DEP.heatmap.scatter.3D:                Visualize the DEPs' kmeans cluster result by using 3D scatter
'                                            plot.
'    /DEP.kmeans.scatter2D:                  
'    /matrix.clustering:                     
' 
' 
' 10. iTraq data analysis tool
' 
' 
'     /iTraq.Bridge.Matrix:                   
'     /iTraq.matrix.split:                    Split the raw matrix into different compare group based on
'                                             the experimental designer information.
'     /iTraq.RSD-P.Density:                   Visualize data QC analysis result.
'     /iTraq.Symbol.Replacement:              * Using this CLI tool for processing the tag header of iTraq
'                                             result at first.
'     /iTraq.t.test:                          Implements the screening for different expression proteins
'                                             by using log2FC threshold and t.test pvalue threshold.
' 
' 
' 11. Label Free data analysis tools
' 
' 
'     /labelFree.matrix:                      
'     /labelFree.matrix.split:                
'     /labelFree.t.test:                      
'     /Matrix.Normalization:                  
'     /names:                                 
' 
' 
' 12. Repository data tools
' 
' 
'     /Imports.Go.obo.mysql:                  Dumping GO obo database as mysql database files.
'     /Imports.Uniprot.Xml:                   Dumping the UniprotKB XML database as mysql database file.
' 
' 
' 13. UniProt tools
' 
' 
'     /Retrieve.ID.mapping:                   Convert the protein id from other database to UniProtKB.
'     /UniProt.IDs:                           
'     /Uniprot.organism_id:                   Get uniprot_id to Organism-specific databases id map table.
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Settings ??<commandName>" for getting more details command help.
'    2. Using command "Settings /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Settings /i" for enter interactive console mode.

Namespace GCModellerApps


''' <summary>
''' Custom KO classification set can be download from: http://www.kegg.jp/kegg-bin/get_htext?ko00001.keg.
''' You can replace the %s mark using kegg organism code in url example as: http://www.kegg.jp/kegg-bin/download_htext?htext=%s00001&amp;format=htext&amp;filedir= for download the custom KO classification set.
''' </summary>
'''
Public Class eggHTS : Inherits InteropService

    Public Const App$ = "eggHTS.exe"

    Sub New(App$)
        MyBase._executableAssembly = App$
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As eggHTS
          Return New eggHTS(App:=directory & "/" & eggHTS.App)
     End Function

''' <summary>
''' ```bash
''' /blastX.fill.ORF /in &lt;annotations.csv&gt; /blastx &lt;blastx.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function BlastXFillORF([in] As String, blastx As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/blastX.fill.ORF")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/blastx " & """" & blastx & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /COG.profiling.plot /in &lt;myvacog.csv&gt; [/size &lt;image_size, default=1800,1200&gt; /out &lt;out.png&gt;]
''' ```
''' Plots the COGs category statics profiling of the target genome from the COG annotation file.
''' </summary>
'''
''' <param name="[in]"> The COG annotation result.
''' </param>
Public Function COGCatalogProfilingPlot([in] As String, Optional size As String = "1800,1200", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/COG.profiling.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Converts /in &lt;GSEA.terms.csv&gt; [/DAVID2KOBAS /out &lt;result.terms.csv&gt;]
''' ```
''' Converts the GCModeller enrichment analysis output as the KOBAS enrichment analysis result output table.
''' </summary>
'''
''' <param name="[in]"> The GCModeller enrichment analysis output table.
''' </param>
Public Function Converts([in] As String, Optional out As String = "", Optional david2kobas As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Converts")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If david2kobas Then
        Call CLI.Append("/david2kobas ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Data.Add.Mappings /in &lt;data.csv&gt; /bbh &lt;bbh.csv&gt; /ID.mappings &lt;uniprot.ID.mappings.tsv&gt; /uniprot &lt;uniprot.XML&gt; [/ID &lt;fieldName&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function AddReMapping([in] As String, 
                                bbh As String, 
                                ID_mappings As String, 
                                uniprot As String, 
                                Optional id As String = "", 
                                Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Data.Add.Mappings")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/ID.mappings " & """" & ID_mappings & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    If Not id.StringEmpty Then
            Call CLI.Append("/id " & """" & id & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Data.Add.ORF /in &lt;data.csv&gt; /uniprot &lt;uniprot.XML&gt; [/ID &lt;fieldName&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function DataAddORF([in] As String, uniprot As String, Optional id As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Data.Add.ORF")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    If Not id.StringEmpty Then
            Call CLI.Append("/id " & """" & id & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Data.Add.uniprotIDs /in &lt;annotations.csv&gt; /data &lt;data.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function DataAddUniprotIDs([in] As String, data As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Data.Add.uniprotIDs")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/data " & """" & data & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DAVID.Split /in &lt;DAVID.txt&gt; [/out &lt;out.DIR, default=./&gt;]
''' ```
''' </summary>
'''

Public Function SplitDAVID([in] As String, Optional out As String = "./") As Integer
    Dim CLI As New StringBuilder("/DAVID.Split")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DEP.heatmap.scatter.3D /in &lt;kmeans.csv&gt; /sampleInfo &lt;sampleInfo.csv&gt; [/display.labels &lt;default=-1&gt; /cluster.prefix &lt;default=&quot;cluster: #&quot;&gt; /size &lt;default=1600,1400&gt; /schema &lt;default=clusters&gt; /view.angle &lt;default=30,60,-56.25&gt; /view.distance &lt;default=2500&gt; /arrow.factor &lt;default=1,2&gt; /cluster.title &lt;names.csv&gt; /out &lt;out.png&gt;]
''' ```
''' Visualize the DEPs&apos; kmeans cluster result by using 3D scatter plot.
''' </summary>
'''
''' <param name="[in]"> The kmeans cluster result from ``/DEP.heatmap`` command.
''' </param>
''' <param name="sampleInfo"> Sample info fot grouping the matrix column data and generates the 3d plot ``&lt;x,y,z&gt;`` coordinations.
''' </param>
''' <param name="cluster_prefix"> The term prefix of the kmeans cluster name when display on the legend title.
''' </param>
''' <param name="size"> The output 3D scatter plot image size.
''' </param>
''' <param name="view_angle"> The view angle of the 3D scatter plot objects, in 3D direction of ``&lt;X&gt;,&lt;Y&gt;,&lt;Z&gt;``
''' </param>
''' <param name="view_distance"> The view distance from the 3D camera screen to the 3D objects.
''' </param>
''' <param name="out"> The file path of the output plot image.
''' </param>
''' <param name="display_labels"> If this parameter is positive and then all of the value greater than this quantile threshold its labels will be display on the plot.
''' </param>
Public Function DEPHeatmapScatter3D([in] As String, 
                                       sampleInfo As String, 
                                       Optional display_labels As String = "-1", 
                                       Optional cluster_prefix As String = "cluster: #", 
                                       Optional size As String = "1600,1400", 
                                       Optional schema As String = "clusters", 
                                       Optional view_angle As String = "30,60,-56.25", 
                                       Optional view_distance As String = "2500", 
                                       Optional arrow_factor As String = "1,2", 
                                       Optional cluster_title As String = "", 
                                       Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/DEP.heatmap.scatter.3D")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/sampleInfo " & """" & sampleInfo & """ ")
    If Not display_labels.StringEmpty Then
            Call CLI.Append("/display.labels " & """" & display_labels & """ ")
    End If
    If Not cluster_prefix.StringEmpty Then
            Call CLI.Append("/cluster.prefix " & """" & cluster_prefix & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not schema.StringEmpty Then
            Call CLI.Append("/schema " & """" & schema & """ ")
    End If
    If Not view_angle.StringEmpty Then
            Call CLI.Append("/view.angle " & """" & view_angle & """ ")
    End If
    If Not view_distance.StringEmpty Then
            Call CLI.Append("/view.distance " & """" & view_distance & """ ")
    End If
    If Not arrow_factor.StringEmpty Then
            Call CLI.Append("/arrow.factor " & """" & arrow_factor & """ ")
    End If
    If Not cluster_title.StringEmpty Then
            Call CLI.Append("/cluster.title " & """" & cluster_title & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DEP.kmeans.scatter2D /in &lt;kmeans.csv&gt; /sampleInfo &lt;sampleInfo.csv&gt; [/t.log &lt;default=-1&gt; /cluster.prefix &lt;default=&quot;cluster: #&quot;&gt; /size &lt;2500,2200&gt; /pt.size &lt;radius pixels, default=15&gt; /schema &lt;default=clusters&gt; /out &lt;out.png&gt;]
''' ```
''' </summary>
'''
''' <param name="sampleinfo"> This file describ how to assign the axis data. The ``sample_group`` in this file defines the X or Y axis label, 
'''                             and the corresponding ``sample_name`` data is the data for plot on the X or Y axis.
''' </param>
Public Function DEPKmeansScatter2D([in] As String, 
                                      sampleInfo As String, 
                                      Optional t_log As String = "-1", 
                                      Optional cluster_prefix As String = "cluster: #", 
                                      Optional size As String = "", 
                                      Optional pt_size As String = "15", 
                                      Optional schema As String = "clusters", 
                                      Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/DEP.kmeans.scatter2D")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/sampleInfo " & """" & sampleInfo & """ ")
    If Not t_log.StringEmpty Then
            Call CLI.Append("/t.log " & """" & t_log & """ ")
    End If
    If Not cluster_prefix.StringEmpty Then
            Call CLI.Append("/cluster.prefix " & """" & cluster_prefix & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not pt_size.StringEmpty Then
            Call CLI.Append("/pt.size " & """" & pt_size & """ ")
    End If
    If Not schema.StringEmpty Then
            Call CLI.Append("/schema " & """" & schema & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DEP.logFC.hist /in &lt;log2test.csv&gt; [/step &lt;0.25&gt; /type &lt;default=log2fc&gt; /legend.title &lt;Frequency(log2FC)&gt; /x.axis &quot;(min,max),tick=0.25&quot; /color &lt;lightblue&gt; /size &lt;1400,900&gt; /out &lt;out.png&gt;]
''' ```
''' Using for plots the FC histogram when the experiment have no biological replicates.
''' </summary>
'''
''' <param name="type"> Which field in the input dataframe should be using as the data source for the histogram plot? Default field(column) name is &quot;log2FC&quot;.
''' </param>
''' <param name="[step]"> The steps for generates the histogram test data.
''' </param>
Public Function logFCHistogram([in] As String, 
                                  Optional [step] As String = "", 
                                  Optional type As String = "log2fc", 
                                  Optional legend_title As String = "", 
                                  Optional x_axis As String = "(min,max),tick=0.25", 
                                  Optional color As String = "", 
                                  Optional size As String = "", 
                                  Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/DEP.logFC.hist")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not [step].StringEmpty Then
            Call CLI.Append("/step " & """" & [step] & """ ")
    End If
    If Not type.StringEmpty Then
            Call CLI.Append("/type " & """" & type & """ ")
    End If
    If Not legend_title.StringEmpty Then
            Call CLI.Append("/legend.title " & """" & legend_title & """ ")
    End If
    If Not x_axis.StringEmpty Then
            Call CLI.Append("/x.axis " & """" & x_axis & """ ")
    End If
    If Not color.StringEmpty Then
            Call CLI.Append("/color " & """" & color & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DEP.logFC.Volcano /in &lt;DEP-log2FC.t.test-table.csv&gt; [/title &lt;title&gt; /p.value &lt;default=0.05&gt; /level &lt;default=1.5&gt; /colors &lt;up=red;down=green;other=black&gt; /label.p &lt;default=-1&gt; /size &lt;1400,1400&gt; /display.count /out &lt;plot.csv&gt;]
''' ```
''' Volcano plot of the DEPs&apos; analysis result.
''' </summary>
'''
''' <param name="size"> The canvas size of the output image.
''' </param>
''' <param name="[in]"> The input DEPs t.test result, should contains at least 3 columns which are names: ``ID``, ``log2FC`` and ``p.value``
''' </param>
''' <param name="colors"> The color profile for the DEPs and proteins that no-changes, value string in format like: key=value, and seperated by ``;`` symbol.
''' </param>
''' <param name="title"> The plot main title.
''' </param>
''' <param name="p_value"> The p.value cutoff threshold, default is 0.05.
''' </param>
''' <param name="level"> The log2FC value cutoff threshold, default is ``log2(1.5)``.
''' </param>
''' <param name="display_count"> Display the protein counts in the legend label? by default is not.
''' </param>
''' <param name="label_p"> Display the DEP protein name on the plot? by default -1 means not display. using this parameter for set the P value cutoff of the DEP for display labels.
''' </param>
Public Function logFCVolcano([in] As String, 
                                Optional title As String = "", 
                                Optional p_value As String = "0.05", 
                                Optional level As String = "1.5", 
                                Optional colors As String = "", 
                                Optional label_p As String = "-1", 
                                Optional size As String = "", 
                                Optional out As String = "", 
                                Optional display_count As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/DEP.logFC.Volcano")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not title.StringEmpty Then
            Call CLI.Append("/title " & """" & title & """ ")
    End If
    If Not p_value.StringEmpty Then
            Call CLI.Append("/p.value " & """" & p_value & """ ")
    End If
    If Not level.StringEmpty Then
            Call CLI.Append("/level " & """" & level & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not label_p.StringEmpty Then
            Call CLI.Append("/label.p " & """" & label_p & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If display_count Then
        Call CLI.Append("/display.count ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DEP.uniprot.list /DEP &lt;log2-test.DEP.csv&gt; /sample &lt;sample.csv&gt; [/out &lt;out.txt&gt;]
''' ```
''' </summary>
'''

Public Function DEPUniprotIDlist(DEP As String, sample As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/DEP.uniprot.list")
    Call CLI.Append(" ")
    Call CLI.Append("/DEP " & """" & DEP & """ ")
    Call CLI.Append("/sample " & """" & sample & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DEP.uniprot.list2 /in &lt;log2.test.csv&gt; [/DEP.Flag &lt;is.DEP?&gt; /uniprot.Flag &lt;uniprot&gt; /species &lt;scientifcName&gt; /uniprot &lt;uniprotXML&gt; /out &lt;out.txt&gt;]
''' ```
''' </summary>
'''

Public Function DEPUniprotIDs2([in] As String, 
                                  Optional dep_flag As String = "", 
                                  Optional uniprot_flag As String = "", 
                                  Optional species As String = "", 
                                  Optional uniprot As String = "", 
                                  Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/DEP.uniprot.list2")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not dep_flag.StringEmpty Then
            Call CLI.Append("/dep.flag " & """" & dep_flag & """ ")
    End If
    If Not uniprot_flag.StringEmpty Then
            Call CLI.Append("/uniprot.flag " & """" & uniprot_flag & """ ")
    End If
    If Not species.StringEmpty Then
            Call CLI.Append("/species " & """" & species & """ ")
    End If
    If Not uniprot.StringEmpty Then
            Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DEP.venn /data &lt;Directory&gt; [/title &lt;VennDiagram title&gt; /out &lt;out.DIR&gt;]
''' ```
''' Generate the VennDiagram plot data and the venn plot tiff. The default parameter profile is using for the iTraq data.
''' </summary>
'''
''' <param name="data"> A directory path which it contains the DEPs matrix csv files from the sample groups&apos;s analysis result.
''' </param>
''' <param name="out"> A directory path which it will contains the venn data result, includes venn matrix, venn plot tiff image, etc.
''' </param>
''' <param name="title"> The main title of the venn plot.
''' </param>
Public Function VennData(data As String, Optional title As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/DEP.venn")
    Call CLI.Append(" ")
    Call CLI.Append("/data " & """" & data & """ ")
    If Not title.StringEmpty Then
            Call CLI.Append("/title " & """" & title & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DEPs.heatmap /data &lt;Directory/csv_file&gt; [/labelFree /schema &lt;color_schema, default=RdYlGn:c11&gt; /no-clrev /KO.class /annotation &lt;annotation.csv&gt; /row.labels.geneName /hide.labels /is.matrix /cluster.n &lt;default=6&gt; /sampleInfo &lt;sampleinfo.csv&gt; /non_DEP.blank /title &quot;Heatmap of DEPs log2FC&quot; /t.log2 /tick &lt;-1&gt; /size &lt;size, default=2000,3000&gt; /legend.size &lt;size, default=600,100&gt; /out &lt;out.DIR&gt;]
''' ```
''' Generates the heatmap plot input data. The default label profile is using for the iTraq result.
''' </summary>
'''
''' <param name="non_DEP_blank"> If this parameter present, then all of the non-DEP that bring by the DEP set union, will strip as blank on its foldchange value, and set to 1 at finally. Default is reserve this non-DEP foldchange value.
''' </param>
''' <param name="KO_class"> If this argument was set, then the KO class information for uniprotID will be draw on the output heatmap.
''' </param>
''' <param name="sampleInfo"> Describ the experimental group information
''' </param>
''' <param name="data"> This file path parameter can be both a directory which contains a set of DEPs result or a single csv file path.
''' </param>
''' <param name="hide_labels"> Hide the row labels?
''' </param>
''' <param name="cluster_n"> Expects the kmeans cluster result number, default is output 6 kmeans clusters.
''' </param>
''' <param name="schema"> The color patterns of the heatmap visualize, by default is using ``ColorBrewer`` colors.
''' </param>
''' <param name="out"> A directory path where will save the output heatmap plot image and the kmeans cluster details info.
''' </param>
''' <param name="title"> The main title of this chart plot.
''' </param>
''' <param name="t_log2"> If this parameter is presented, then it will means apply the log2 transform on the matrix cell value before the heatmap plot.
''' </param>
''' <param name="tick"> The ticks value of the color legend, by default value -1 means generates ticks automatically.
''' </param>
''' <param name="no_clrev"> Do not reverse the color sequence.
''' </param>
''' <param name="size"> The canvas size.
''' </param>
''' <param name="is_matrix"> The input data is a data matrix, can be using for heatmap drawing directly.
''' </param>
''' <param name="row_labels_geneName"> This option will use the ``geneName``(from the annotation data) as the row display label instead of using uniprotID or geneID. This option required of the ``/annotation`` presented.
''' </param>
''' <param name="annotation"> The protein annotation data that extract from the uniprot database. Some advanced heatmap plot feature required of this annotation data presented.
''' </param>
Public Function DEPs_heatmapKmeans(data As String, 
                                      Optional schema As String = "RdYlGn:c11", 
                                      Optional annotation As String = "", 
                                      Optional cluster_n As String = "6", 
                                      Optional sampleinfo As String = "", 
                                      Optional title As String = "Heatmap of DEPs log2FC", 
                                      Optional tick As String = "", 
                                      Optional size As String = "2000,3000", 
                                      Optional legend_size As String = "600,100", 
                                      Optional out As String = "", 
                                      Optional labelfree As Boolean = False, 
                                      Optional no_clrev As Boolean = False, 
                                      Optional ko_class As Boolean = False, 
                                      Optional row_labels_genename As Boolean = False, 
                                      Optional hide_labels As Boolean = False, 
                                      Optional is_matrix As Boolean = False, 
                                      Optional non_dep_blank As Boolean = False, 
                                      Optional t_log2 As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/DEPs.heatmap")
    Call CLI.Append(" ")
    Call CLI.Append("/data " & """" & data & """ ")
    If Not schema.StringEmpty Then
            Call CLI.Append("/schema " & """" & schema & """ ")
    End If
    If Not annotation.StringEmpty Then
            Call CLI.Append("/annotation " & """" & annotation & """ ")
    End If
    If Not cluster_n.StringEmpty Then
            Call CLI.Append("/cluster.n " & """" & cluster_n & """ ")
    End If
    If Not sampleinfo.StringEmpty Then
            Call CLI.Append("/sampleinfo " & """" & sampleinfo & """ ")
    End If
    If Not title.StringEmpty Then
            Call CLI.Append("/title " & """" & title & """ ")
    End If
    If Not tick.StringEmpty Then
            Call CLI.Append("/tick " & """" & tick & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not legend_size.StringEmpty Then
            Call CLI.Append("/legend.size " & """" & legend_size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If labelfree Then
        Call CLI.Append("/labelfree ")
    End If
    If no_clrev Then
        Call CLI.Append("/no-clrev ")
    End If
    If ko_class Then
        Call CLI.Append("/ko.class ")
    End If
    If row_labels_genename Then
        Call CLI.Append("/row.labels.genename ")
    End If
    If hide_labels Then
        Call CLI.Append("/hide.labels ")
    End If
    If is_matrix Then
        Call CLI.Append("/is.matrix ")
    End If
    If non_dep_blank Then
        Call CLI.Append("/non_dep.blank ")
    End If
    If t_log2 Then
        Call CLI.Append("/t.log2 ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DEPs.stat /in &lt;log2.test.csv&gt; [/log2FC &lt;default=log2FC&gt; /out &lt;out.stat.csv&gt;]
''' ```
''' https://github.com/xieguigang/GCModeller.cli2R/blob/master/GCModeller.cli2R/R/log2FC_t-test.R
''' </summary>
'''
''' <param name="log2FC"> The field name that stores the log2FC value of the average FoldChange
''' </param>
''' <param name="[in]"> The DEPs&apos; t.test result in csv file format.
''' </param>
''' <param name="out"> The stat count output file path.
''' </param>
Public Function DEPStatics([in] As String, Optional log2fc As String = "log2FC", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/DEPs.stat")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not log2fc.StringEmpty Then
            Call CLI.Append("/log2fc " & """" & log2fc & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DEPs.takes.values /in &lt;DEPs.csv&gt; [/boolean.tag &lt;default=is.DEP&gt; /by.FC &lt;tag=value, default=logFC=log2(1.5)&gt; /by.p.value &lt;tag=value, p.value=0.05&gt; /data &lt;data.csv&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function TakeDEPsValues([in] As String, 
                                  Optional boolean_tag As String = "is.DEP", 
                                  Optional by_fc As String = "logFC=log2(1.5)", 
                                  Optional by_p_value As String = "", 
                                  Optional data As String = "", 
                                  Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/DEPs.takes.values")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not boolean_tag.StringEmpty Then
            Call CLI.Append("/boolean.tag " & """" & boolean_tag & """ ")
    End If
    If Not by_fc.StringEmpty Then
            Call CLI.Append("/by.fc " & """" & by_fc & """ ")
    End If
    If Not by_p_value.StringEmpty Then
            Call CLI.Append("/by.p.value " & """" & by_p_value & """ ")
    End If
    If Not data.StringEmpty Then
            Call CLI.Append("/data " & """" & data & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /DEPs.union /in &lt;csv.DIR&gt; [/FC &lt;default=logFC&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function DEPsUnion([in] As String, Optional fc As String = "logFC", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/DEPs.union")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not fc.StringEmpty Then
            Call CLI.Append("/fc " & """" & fc & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /edgeR.Designer /in &lt;proteinGroups.csv&gt; /designer &lt;designer.csv&gt; [/label &lt;default is empty&gt; /deli &lt;default=-&gt; /out &lt;out.DIR&gt;]
''' ```
''' Generates the edgeR inputs table
''' </summary>
'''

Public Function edgeRDesigner([in] As String, 
                                 designer As String, 
                                 Optional label As String = "", 
                                 Optional deli As String = "-", 
                                 Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/edgeR.Designer")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/designer " & """" & designer & """ ")
    If Not label.StringEmpty Then
            Call CLI.Append("/label " & """" & label & """ ")
    End If
    If Not deli.StringEmpty Then
            Call CLI.Append("/deli " & """" & deli & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /enricher.background /in &lt;uniprot.XML&gt; [/mapping &lt;maps.tsv&gt; /out &lt;term2gene.txt.DIR&gt;]
''' ```
''' Create enrichment analysis background based on the uniprot xml database.
''' </summary>
'''
''' <param name="mapping"> The id mapping file, each row in format like ``id&lt;TAB&gt;uniprotID``
''' </param>
''' <param name="[in]"> The uniprotKB XML database which can be download from http://uniprot.org
''' </param>
Public Function Backgrounds([in] As String, Optional mapping As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/enricher.background")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not mapping.StringEmpty Then
            Call CLI.Append("/mapping " & """" & mapping & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Enrichment.Term.Filter /in &lt;enrichment.csv&gt; /filter &lt;key-string&gt; [/out &lt;out.csv&gt;]
''' ```
''' Filter the specific term result from the analysis output by using pattern keyword
''' </summary>
'''

Public Function EnrichmentTermFilter([in] As String, filter As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Enrichment.Term.Filter")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/filter " & """" & filter & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Enrichments.ORF.info /in &lt;enrichment.csv&gt; /proteins &lt;uniprot-genome.XML&gt; [/nocut /ORF /out &lt;out.csv&gt;]
''' ```
''' Retrive KEGG/GO info for the genes in the enrichment result.
''' </summary>
'''
''' <param name="ORF"> If this argument presented, then the program will using the ORF value in ``uniprot.xml`` as the record identifier, 
'''               default is using uniprotID in the accessions fields of the uniprot.XML records.
''' </param>
''' <param name="nocut"> Default is using pvalue &lt; 0.05 as term cutoff, if this argument presented, then will no pavlue cutoff for the terms input.
''' </param>
''' <param name="[in]"> KOBAS analysis result output.
''' </param>
Public Function RetriveEnrichmentGeneInfo([in] As String, 
                                             proteins As String, 
                                             Optional out As String = "", 
                                             Optional nocut As Boolean = False, 
                                             Optional orf As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Enrichments.ORF.info")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/proteins " & """" & proteins & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If nocut Then
        Call CLI.Append("/nocut ")
    End If
    If orf Then
        Call CLI.Append("/orf ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Exocarta.Hits /in &lt;list.txt&gt; /annotation &lt;annotations.csv&gt; /exocarta &lt;Exocarta.tsv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function ExocartaHits([in] As String, annotation As String, exocarta As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Exocarta.Hits")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/annotation " & """" & annotation & """ ")
    Call CLI.Append("/exocarta " & """" & exocarta & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Fasta.IDlist /in &lt;prot.fasta&gt; [/out &lt;geneIDs.txt&gt;]
''' ```
''' </summary>
'''

Public Function GetFastaIDlist([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Fasta.IDlist")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /FoldChange.Matrix.Invert /in &lt;data.csv&gt; [/log2FC /out &lt;invert.csv&gt;]
''' ```
''' Reverse the FoldChange value from the source result matrix.
''' </summary>
'''
''' <param name="log2FC"> This boolean flag indicated that the fold change value is log2FC, which required of power 2 and then invert by divided by 1.
''' </param>
''' <param name="out"> This function will output a FoldChange matrix.
''' </param>
Public Function iTraqInvert([in] As String, Optional out As String = "", Optional log2fc As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/FoldChange.Matrix.Invert")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If log2fc Then
        Call CLI.Append("/log2fc ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /func.rich.string /in &lt;string_interactions.tsv&gt; /uniprot &lt;uniprot.XML&gt; /DEP &lt;dep.t.test.csv&gt; [/map &lt;map.tsv&gt; /r.range &lt;default=12,30&gt; /size &lt;default=1920,1080&gt; /log2FC &lt;default=log2FC&gt; /layout &lt;string_network_coordinates.txt&gt; /out &lt;out.network.DIR&gt;]
''' ```
''' DEPs&apos; functional enrichment network based on string-db exports, and color by KEGG pathway.
''' </summary>
'''
''' <param name="map"> A tsv file that using for map the user custom gene ID as the uniprotKB ID, in format like: ``UserID&lt;TAB&gt;UniprotID``
''' </param>
''' <param name="DEP"> The DEPs t.test output result csv file.
''' </param>
''' <param name="r_range"> The network node size radius range, input string in format like: ``min,max``
''' </param>
''' <param name="log2FC"> The csv field name for read the DEPs fold change value, default is ``log2FC`` as the field name.
''' </param>
Public Function FunctionalNetworkEnrichment([in] As String, 
                                               uniprot As String, 
                                               DEP As String, 
                                               Optional map As String = "", 
                                               Optional r_range As String = "12,30", 
                                               Optional size As String = "1920,1080", 
                                               Optional log2fc As String = "log2FC", 
                                               Optional layout As String = "", 
                                               Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/func.rich.string")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    Call CLI.Append("/DEP " & """" & DEP & """ ")
    If Not map.StringEmpty Then
            Call CLI.Append("/map " & """" & map & """ ")
    End If
    If Not r_range.StringEmpty Then
            Call CLI.Append("/r.range " & """" & r_range & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not log2fc.StringEmpty Then
            Call CLI.Append("/log2fc " & """" & log2fc & """ ")
    End If
    If Not layout.StringEmpty Then
            Call CLI.Append("/layout " & """" & layout & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Gene.list.from.KOBAS /in &lt;KOBAS.csv&gt; [/p.value &lt;default=1&gt; /out &lt;out.txt&gt;]
''' ```
''' Using this command for generates the gene id list input for the STRING-db search.
''' </summary>
'''
''' <param name="p_value"> Using for enrichment term result filters, default is p.value less than or equals to 1, means no cutoff.
''' </param>
Public Function GeneIDListFromKOBASResult([in] As String, Optional p_value As String = "1", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Gene.list.from.KOBAS")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not p_value.StringEmpty Then
            Call CLI.Append("/p.value " & """" & p_value & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /GO.cellular_location.Plot /in &lt;KOBAS.GO.csv&gt; [/GO &lt;go.obo&gt; /3D /colors &lt;schemaName, default=Paired:c8&gt; /out &lt;out.png&gt;]
''' ```
''' Visualize of the subcellular location result from the GO enrichment analysis.
''' </summary>
'''
''' <param name="_3D"> 3D style pie chart for the plot?
''' </param>
''' <param name="colors"> Color schema name, default using color brewer color schema.
''' </param>
Public Function GO_cellularLocationPlot([in] As String, 
                                           Optional go As String = "", 
                                           Optional colors As String = "Paired:c8", 
                                           Optional out As String = "", 
                                           Optional _3d As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/GO.cellular_location.Plot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not go.StringEmpty Then
            Call CLI.Append("/go " & """" & go & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If _3d Then
        Call CLI.Append("/3d ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /GO.enrichment.DAVID /in &lt;DAVID.csv&gt; [/tsv /go &lt;go.obo&gt; /colors &lt;default=Set1:c6&gt; /size &lt;default=1200,1000&gt; /tick 1 /p.value &lt;0.05&gt; /out &lt;out.png&gt;]
''' ```
''' </summary>
'''
''' <param name="colors"> Change the default color profiles of the categories plots. Value can be a color profile name term or color name list that join by delimiter comma symbol:
'''               
'''               + &lt;profile name term&gt;: Set1:c6 
'''               Full list of the profile names: https://github.com/xieguigang/sciBASIC/blob/master/gr/Colors/colorbrewer/colorbrewer.json
'''               + &lt;color name list&gt;: black,green,blue 
'''               Full list of the color names: https://github.com/xieguigang/sciBASIC/blob/master/etc/VB.NET_Colors.html
''' </param>
Public Function DAVID_GOplot([in] As String, 
                                Optional go As String = "", 
                                Optional colors As String = "Set1:c6", 
                                Optional size As String = "1200,1000", 
                                Optional tick As String = "", 
                                Optional p_value As String = "", 
                                Optional out As String = "", 
                                Optional tsv As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/GO.enrichment.DAVID")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not go.StringEmpty Then
            Call CLI.Append("/go " & """" & go & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not tick.StringEmpty Then
            Call CLI.Append("/tick " & """" & tick & """ ")
    End If
    If Not p_value.StringEmpty Then
            Call CLI.Append("/p.value " & """" & p_value & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If tsv Then
        Call CLI.Append("/tsv ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Go.enrichment.plot /in &lt;enrichmentTerm.csv&gt; [/bubble /r &quot;log(x,1.5)&quot; /Corrected /displays &lt;default=10&gt; /PlantRegMap /label.right /label.color.disable /label.maxlen &lt;char_count, default=64&gt; /colors &lt;default=Set1:c6&gt; /gray /pvalue &lt;0.05&gt; /size &lt;2000,1600&gt; /tick 1 /go &lt;go.obo&gt; /out &lt;out.png&gt;]
''' ```
''' Go enrichment plot base on the KOBAS enrichment analysis result.
''' </summary>
'''
''' <param name="[in]"> The KOBAS enrichment analysis output csv file.
''' </param>
''' <param name="out"> The file path of the output plot image. If the graphics driver is using svg engine, then this result can be output to the standard output if this parameter is not presented in the CLI input.
''' </param>
''' <param name="r"> The bubble radius expression, when this enrichment plot is in ``/bubble`` mode.
''' </param>
''' <param name="label_right"> Align the label to right if this argument presented.
''' </param>
''' <param name="Corrected"> Using the corrected p.value instead of using the p.value as the term filter for this enrichment plot.
''' </param>
''' <param name="pvalue"> The p.value threshold for choose the terms that will be plot on the image, default is plot all terms that their enrichment p.value is smaller than 0.05.
''' </param>
''' <param name="size"> The output image size in pixel.
''' </param>
''' <param name="tick"> The axis ticking interval value, using **-1** for generated this value automatically, or any other positive numeric value will setup this interval value manually.
''' </param>
''' <param name="GO"> The GO database for category the enrichment term result into their corrisponding Go namespace. If this argument value is not presented in the CLI input, then program will using the GO database file from the GCModeller repository data system.
''' </param>
''' <param name="bubble"> Visuallize the GO enrichment analysis result using bubble plot, not the bar plot.
''' </param>
''' <param name="displays"> If the ``/bubble`` argument is not presented, then this will means the top number of the enriched term will plot on the barplot, else it is the term label display number in the bubble plot mode. 
'''               Set this argument value to -1 for display all terms.
''' </param>
''' <param name="gray"> Set the color of all of the labels, bars, class labels on this chart plot output to color gray? If this presented, then color schema will not working. Otherwise if this parameter argument is not presented in the CLI input, then the labels and bars will render color based on their corresponding GO namespace.
''' </param>
''' <param name="colors"> Change the default color profiles of the categories plots. Value can be a color profile name term or color name list that join by delimiter comma symbol:
'''               
'''               + &lt;profile name term&gt;: Set1:c6 
'''               Full list of the profile names: https://github.com/xieguigang/sciBASIC/blob/master/gr/Colors/colorbrewer/colorbrewer.json
'''               + &lt;color name list&gt;: black,green,blue 
'''               Full list of the color names: https://github.com/xieguigang/sciBASIC/blob/master/etc/VB.NET_Colors.html
''' </param>
Public Function GO_enrichmentPlot([in] As String, 
                                     Optional r As String = "log(x,1.5)", 
                                     Optional displays As String = "10", 
                                     Optional label_maxlen As String = "64", 
                                     Optional colors As String = "Set1:c6", 
                                     Optional pvalue As String = "", 
                                     Optional size As String = "", 
                                     Optional tick As String = "", 
                                     Optional go As String = "", 
                                     Optional out As String = "", 
                                     Optional bubble As Boolean = False, 
                                     Optional corrected As Boolean = False, 
                                     Optional plantregmap As Boolean = False, 
                                     Optional label_right As Boolean = False, 
                                     Optional label_color_disable As Boolean = False, 
                                     Optional gray As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Go.enrichment.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not r.StringEmpty Then
            Call CLI.Append("/r " & """" & r & """ ")
    End If
    If Not displays.StringEmpty Then
            Call CLI.Append("/displays " & """" & displays & """ ")
    End If
    If Not label_maxlen.StringEmpty Then
            Call CLI.Append("/label.maxlen " & """" & label_maxlen & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not pvalue.StringEmpty Then
            Call CLI.Append("/pvalue " & """" & pvalue & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not tick.StringEmpty Then
            Call CLI.Append("/tick " & """" & tick & """ ")
    End If
    If Not go.StringEmpty Then
            Call CLI.Append("/go " & """" & go & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If bubble Then
        Call CLI.Append("/bubble ")
    End If
    If corrected Then
        Call CLI.Append("/corrected ")
    End If
    If plantregmap Then
        Call CLI.Append("/plantregmap ")
    End If
    If label_right Then
        Call CLI.Append("/label.right ")
    End If
    If label_color_disable Then
        Call CLI.Append("/label.color.disable ")
    End If
    If gray Then
        Call CLI.Append("/gray ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /iBAQ.Cloud /in &lt;expression.csv&gt; /annotations &lt;annotations.csv&gt; /DEPs &lt;DEPs.csv&gt; /tag &lt;expression&gt; [/out &lt;out.png&gt;]
''' ```
''' Cloud plot of the iBAQ DEPs result.
''' </summary>
'''
''' <param name="tag"> The field name in the ``/in`` matrix that using as the expression value.
''' </param>
Public Function DEPsCloudPlot([in] As String, 
                                 annotations As String, 
                                 DEPs As String, 
                                 tag As String, 
                                 Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/iBAQ.Cloud")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/annotations " & """" & annotations & """ ")
    Call CLI.Append("/DEPs " & """" & DEPs & """ ")
    Call CLI.Append("/tag " & """" & tag & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /ID.Replace.bbh /in &lt;dataset.csv&gt; /bbh &lt;bbh/sbh.csv&gt; [/description &lt;fieldName, default=Description&gt; /out &lt;ID.replaced.csv&gt;]
''' ```
''' LabelFree result helper: replace the source ID to a unify organism protein ID by using ``bbh`` method.
''' This tools required the protein in ``datatset.csv`` associated with the alignment result in ``bbh.csv`` by using the ``query_name`` property.
''' </summary>
'''

Public Function BBHReplace([in] As String, bbh As String, Optional description As String = "Description", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/ID.Replace.bbh")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    If Not description.StringEmpty Then
            Call CLI.Append("/description " & """" & description & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Imports.Go.obo.mysql /in &lt;go.obo&gt; [/out &lt;out.sql&gt;]
''' ```
''' Dumping GO obo database as mysql database files.
''' </summary>
'''
''' <param name="[in]"> The Go obo database file.
''' </param>
''' <param name="out"> The output file path of the generated sql database file. If this argument is not presented in the CLI inputs, then all of the generated content will be output to the console.
''' </param>
Public Function DumpGOAsMySQL([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Imports.Go.obo.mysql")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Imports.Uniprot.Xml /in &lt;uniprot.xml&gt; [/out &lt;out.sql&gt;]
''' ```
''' Dumping the UniprotKB XML database as mysql database file.
''' </summary>
'''
''' <param name="[in]"> The uniprotKB XML database file.
''' </param>
''' <param name="out"> The output file path of the generated sql database file. If this argument is not presented in the CLI inputs, then all of the generated content will be output to the console.
''' </param>
Public Function DumpUniprot([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Imports.Uniprot.Xml")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /iTraq.Bridge.Matrix /A &lt;A_iTraq.csv&gt; /B &lt;B_iTraq.csv&gt; /C &lt;bridge_symbol&gt; [/symbols.A &lt;symbols.csv&gt; /symbols.B &lt;symbols.csv&gt; /out &lt;matrix.csv&gt;]
''' ```
''' </summary>
'''

Public Function iTraqBridge(A As String, 
                               B As String, 
                               C As String, 
                               Optional symbols_a As String = "", 
                               Optional symbols_b As String = "", 
                               Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/iTraq.Bridge.Matrix")
    Call CLI.Append(" ")
    Call CLI.Append("/A " & """" & A & """ ")
    Call CLI.Append("/B " & """" & B & """ ")
    Call CLI.Append("/C " & """" & C & """ ")
    If Not symbols_a.StringEmpty Then
            Call CLI.Append("/symbols.a " & """" & symbols_a & """ ")
    End If
    If Not symbols_b.StringEmpty Then
            Call CLI.Append("/symbols.b " & """" & symbols_b & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /iTraq.matrix.split /in &lt;matrix.csv&gt; /sampleInfo &lt;sampleInfo.csv&gt; /designer &lt;analysis.design.csv&gt; [/allowed.swap /out &lt;out.Dir&gt;]
''' ```
''' Split the raw matrix into different compare group based on the experimental designer information.
''' </summary>
'''
''' <param name="sampleInfo">
''' </param>
''' <param name="designer"> The analysis designer in csv file format for the DEPs calculation, should contains at least two column: ``&lt;Controls&gt;,&lt;Experimental&gt;``. 
'''               The analysis design: ``controls vs experimental`` means formula ``experimental/controls`` in the FoldChange calculation.
''' </param>
Public Function iTraqAnalysisMatrixSplit([in] As String, 
                                            sampleInfo As String, 
                                            designer As String, 
                                            Optional out As String = "", 
                                            Optional allowed_swap As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/iTraq.matrix.split")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/sampleInfo " & """" & sampleInfo & """ ")
    Call CLI.Append("/designer " & """" & designer & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If allowed_swap Then
        Call CLI.Append("/allowed.swap ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /iTraq.RSD-P.Density /in &lt;matrix.csv&gt; [/out &lt;out.png&gt;]
''' ```
''' Visualize data QC analysis result.
''' </summary>
'''
''' <param name="[in]"> A data matrix which is comes from the ``/iTraq.matrix.split`` command.
''' </param>
''' <param name="out"> The file path of the plot result image.
''' </param>
Public Function iTraqRSDPvalueDensityPlot([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/iTraq.RSD-P.Density")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /iTraq.Symbol.Replacement /in &lt;iTraq.data.csv/xlsx&gt; /symbols &lt;symbols.csv/xlsx&gt; [/sheet.name &lt;Sheet1&gt; /symbolSheet &lt;sheetName&gt; /out &lt;out.DIR&gt;]
''' ```
''' * Using this CLI tool for processing the tag header of iTraq result at first.
''' </summary>
'''
''' <param name="[in]"> 
''' </param>
''' <param name="symbols"> Using for replace the mass spectrum expeirment symbol to the user experiment tag.
''' </param>
Public Function iTraqSignReplacement([in] As String, 
                                        symbols As String, 
                                        Optional sheet_name As String = "", 
                                        Optional symbolsheet As String = "", 
                                        Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/iTraq.Symbol.Replacement")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/symbols " & """" & symbols & """ ")
    If Not sheet_name.StringEmpty Then
            Call CLI.Append("/sheet.name " & """" & sheet_name & """ ")
    End If
    If Not symbolsheet.StringEmpty Then
            Call CLI.Append("/symbolsheet " & """" & symbolsheet & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /iTraq.t.test /in &lt;matrix.csv&gt; [/level &lt;default=1.5&gt; /p.value &lt;default=0.05&gt; /FDR &lt;default=0.05&gt; /skip.significant.test /pairInfo &lt;sampleTuple.csv&gt; /out &lt;out.csv&gt;]
''' ```
''' Implements the screening for different expression proteins by using log2FC threshold and t.test pvalue threshold.
''' </summary>
'''
''' <param name="FDR"> do FDR adjust on the p.value result? If this argument value is set to 1, means no adjustment.
''' </param>
''' <param name="skip_significant_test"> If this option is presented in the CLI input, then the significant test from the p.value and FDR will be disabled.
''' </param>
Public Function iTraqTtest([in] As String, 
                              Optional level As String = "1.5", 
                              Optional p_value As String = "0.05", 
                              Optional fdr As String = "0.05", 
                              Optional pairinfo As String = "", 
                              Optional out As String = "", 
                              Optional skip_significant_test As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/iTraq.t.test")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not level.StringEmpty Then
            Call CLI.Append("/level " & """" & level & """ ")
    End If
    If Not p_value.StringEmpty Then
            Call CLI.Append("/p.value " & """" & p_value & """ ")
    End If
    If Not fdr.StringEmpty Then
            Call CLI.Append("/fdr " & """" & fdr & """ ")
    End If
    If Not pairinfo.StringEmpty Then
            Call CLI.Append("/pairinfo " & """" & pairinfo & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If skip_significant_test Then
        Call CLI.Append("/skip.significant.test ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KEGG.Color.Pathway /in &lt;protein.annotations.csv&gt; /ref &lt;KEGG.ref.pathwayMap.directory repository&gt; [/out &lt;out.directory&gt;]
''' ```
''' </summary>
'''
''' <param name="ref"> 
''' </param>
Public Function ColorKEGGPathwayMap([in] As String, ref As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KEGG.Color.Pathway")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/ref " & """" & ref & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KEGG.enrichment.DAVID /in &lt;david.csv&gt; [/tsv /custom &lt;ko00001.keg&gt; /colors &lt;default=Set1:c6&gt; /size &lt;default=1200,1000&gt; /p.value &lt;default=0.05&gt; /tick 1 /out &lt;out.png&gt;]
''' ```
''' </summary>
'''
''' <param name="colors"> Change the default color profiles of the categories plots. Value can be a color profile name term or color name list that join by delimiter comma symbol:
'''               
'''               + &lt;profile name term&gt;: Set1:c6 
'''               Full list of the profile names: https://github.com/xieguigang/sciBASIC/blob/master/gr/Colors/colorbrewer/colorbrewer.json
'''               + &lt;color name list&gt;: black,green,blue 
'''               Full list of the color names: https://github.com/xieguigang/sciBASIC/blob/master/etc/VB.NET_Colors.html
''' </param>
Public Function DAVID_KEGGplot([in] As String, 
                                  Optional custom As String = "", 
                                  Optional colors As String = "Set1:c6", 
                                  Optional size As String = "1200,1000", 
                                  Optional p_value As String = "0.05", 
                                  Optional tick As String = "", 
                                  Optional out As String = "", 
                                  Optional tsv As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/KEGG.enrichment.DAVID")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not custom.StringEmpty Then
            Call CLI.Append("/custom " & """" & custom & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not p_value.StringEmpty Then
            Call CLI.Append("/p.value " & """" & p_value & """ ")
    End If
    If Not tick.StringEmpty Then
            Call CLI.Append("/tick " & """" & tick & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If tsv Then
        Call CLI.Append("/tsv ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KEGG.enrichment.DAVID.pathwaymap /in &lt;david.csv&gt; /uniprot &lt;uniprot.XML&gt; [/tsv /DEPs &lt;deps.csv&gt; /colors &lt;default=red,blue,green&gt; /tag &lt;default=log2FC&gt; /pvalue &lt;default=0.05&gt; /out &lt;out.DIR&gt;]
''' ```
''' </summary>
'''

Public Function DAVID_KEGGPathwayMap([in] As String, 
                                        uniprot As String, 
                                        Optional deps As String = "", 
                                        Optional colors As String = "red,blue,green", 
                                        Optional tag As String = "log2FC", 
                                        Optional pvalue As String = "0.05", 
                                        Optional out As String = "", 
                                        Optional tsv As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/KEGG.enrichment.DAVID.pathwaymap")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    If Not deps.StringEmpty Then
            Call CLI.Append("/deps " & """" & deps & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not tag.StringEmpty Then
            Call CLI.Append("/tag " & """" & tag & """ ")
    End If
    If Not pvalue.StringEmpty Then
            Call CLI.Append("/pvalue " & """" & pvalue & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If tsv Then
        Call CLI.Append("/tsv ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KEGG.Enrichment.PathwayMap /in &lt;kobas.csv&gt; [/DEPs &lt;deps.csv&gt; /colors &lt;default=red,blue,green&gt; /map &lt;id2uniprotID.txt&gt; /uniprot &lt;uniprot.XML&gt; /pvalue &lt;default=0.05&gt; /out &lt;DIR&gt;]
''' ```
''' Show the KEGG pathway map image by using KOBAS KEGG pathway enrichment result.
''' </summary>
'''
''' <param name="colors"> A string vector that setups the DEPs&apos; color profiles, if the argument ``/DEPs`` is presented. value format is ``up,down,present``
''' </param>
''' <param name="DEPs"> Using for rendering color of the KEGG pathway map. The ``/colors`` argument only works when this argument is presented.
''' </param>
''' <param name="map"> Maps user custom ID to uniprot ID. A tsv file with format: ``&lt;customID&gt;&lt;TAB&gt;&lt;uniprotID&gt;``
''' </param>
Public Function KEGGEnrichmentPathwayMap([in] As String, 
                                            Optional deps As String = "", 
                                            Optional colors As String = "red,blue,green", 
                                            Optional map As String = "", 
                                            Optional uniprot As String = "", 
                                            Optional pvalue As String = "0.05", 
                                            Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KEGG.Enrichment.PathwayMap")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not deps.StringEmpty Then
            Call CLI.Append("/deps " & """" & deps & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not map.StringEmpty Then
            Call CLI.Append("/map " & """" & map & """ ")
    End If
    If Not uniprot.StringEmpty Then
            Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    End If
    If Not pvalue.StringEmpty Then
            Call CLI.Append("/pvalue " & """" & pvalue & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KEGG.Enrichment.PathwayMap.Render /in &lt;enrichment.csv&gt; [/repo &lt;maps.directory&gt; /DEPs &lt;deps.csv&gt; /colors &lt;default=red,blue,green&gt; /map &lt;id2uniprotID.txt&gt; /uniprot &lt;uniprot.XML&gt; /pvalue &lt;default=0.05&gt; /out &lt;DIR&gt;]
''' ```
''' KEGG pathway map enrichment analysis visual rendering locally. This function required a local kegg pathway repository.
''' </summary>
'''
''' <param name="repo"> If this argument is omitted, then the default kegg pathway map repository will be used. But the default kegg pathway map repository only works for the KO numbers.
''' </param>
Public Function KEGGEnrichmentPathwayMapLocal([in] As String, 
                                                 Optional repo As String = "", 
                                                 Optional deps As String = "", 
                                                 Optional colors As String = "red,blue,green", 
                                                 Optional map As String = "", 
                                                 Optional uniprot As String = "", 
                                                 Optional pvalue As String = "0.05", 
                                                 Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KEGG.Enrichment.PathwayMap.Render")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not repo.StringEmpty Then
            Call CLI.Append("/repo " & """" & repo & """ ")
    End If
    If Not deps.StringEmpty Then
            Call CLI.Append("/deps " & """" & deps & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not map.StringEmpty Then
            Call CLI.Append("/map " & """" & map & """ ")
    End If
    If Not uniprot.StringEmpty Then
            Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    End If
    If Not pvalue.StringEmpty Then
            Call CLI.Append("/pvalue " & """" & pvalue & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KEGG.enrichment.plot /in &lt;enrichmentTerm.csv&gt; [/gray /colors &lt;default=Set1:c6&gt; /top &lt;default=13&gt; /label.right /pvalue &lt;0.05&gt; /tick 1 /size &lt;2000,1600&gt; /out &lt;out.png&gt;]
''' ```
''' Bar plots of the KEGG enrichment analysis result.
''' </summary>
'''
''' <param name="colors"> Change the default color profiles of the categories plots. Value can be a color profile name term or color name list that join by delimiter comma symbol:
'''               
'''               + &lt;profile name term&gt;: Set1:c6 
'''               Full list of the profile names: https://github.com/xieguigang/sciBASIC/blob/master/gr/Colors/colorbrewer/colorbrewer.json
'''               + &lt;color name list&gt;: black,green,blue 
'''               Full list of the color names: https://github.com/xieguigang/sciBASIC/blob/master/etc/VB.NET_Colors.html,
'''               + &lt;scale by value&gt;: scale(color_set_name)
'''               This will create color profiles based on the result value dataset.
''' </param>
Public Function KEGG_enrichment([in] As String, 
                                   Optional colors As String = "Set1:c6", 
                                   Optional top As String = "13", 
                                   Optional pvalue As String = "", 
                                   Optional tick As String = "", 
                                   Optional size As String = "", 
                                   Optional out As String = "", 
                                   Optional gray As Boolean = False, 
                                   Optional label_right As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/KEGG.enrichment.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not top.StringEmpty Then
            Call CLI.Append("/top " & """" & top & """ ")
    End If
    If Not pvalue.StringEmpty Then
            Call CLI.Append("/pvalue " & """" & pvalue & """ ")
    End If
    If Not tick.StringEmpty Then
            Call CLI.Append("/tick " & """" & tick & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If gray Then
        Call CLI.Append("/gray ")
    End If
    If label_right Then
        Call CLI.Append("/label.right ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KEGG.enrichment.profile /in &lt;enrichment.csv&gt; [/out &lt;profile.csv&gt;]
''' ```
''' </summary>
'''

Public Function KEGGPathwayEnrichmentProfile([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KEGG.enrichment.profile")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KO.Catalogs /in &lt;blast.mapping.csv&gt; /ko &lt;ko_genes.csv&gt; [/key &lt;Query_id&gt; /mapTo &lt;Subject_id&gt; /out &lt;outDIR&gt;]
''' ```
''' Display the barplot of the KEGG orthology match.
''' </summary>
'''

Public Function KOCatalogs([in] As String, 
                              ko As String, 
                              Optional key As String = "", 
                              Optional mapto As String = "", 
                              Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KO.Catalogs")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/ko " & """" & ko & """ ")
    If Not key.StringEmpty Then
            Call CLI.Append("/key " & """" & key & """ ")
    End If
    If Not mapto.StringEmpty Then
            Call CLI.Append("/mapto " & """" & mapto & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KOBAS.add.ORF /in &lt;table.csv&gt; /sample &lt;sample.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''
''' <param name="[in]"> The KOBAS enrichment result.
''' </param>
''' <param name="sample"> The uniprotID -&gt; ORF annotation data. this table file should have a field named &quot;ORF&quot;.
''' </param>
Public Function KOBASaddORFsource([in] As String, sample As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KOBAS.add.ORF")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/sample " & """" & sample & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KOBAS.Sim.Heatmap /in &lt;sim.csv&gt; [/size &lt;1024,800&gt; /colors &lt;RdYlBu:8&gt; /out &lt;out.png&gt;]
''' ```
''' </summary>
'''

Public Function SimHeatmap([in] As String, Optional size As String = "", Optional colors As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KOBAS.Sim.Heatmap")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KOBAS.Similarity /group1 &lt;DIR&gt; /group2 &lt;DIR&gt; [/fileName &lt;default=output_run-Gene Ontology.csv&gt; /out &lt;out.DIR&gt;]
''' ```
''' </summary>
'''

Public Function KOBASSimilarity(group1 As String, group2 As String, Optional filename As String = "output_run-Gene Ontology.csv", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KOBAS.Similarity")
    Call CLI.Append(" ")
    Call CLI.Append("/group1 " & """" & group1 & """ ")
    Call CLI.Append("/group2 " & """" & group2 & """ ")
    If Not filename.StringEmpty Then
            Call CLI.Append("/filename " & """" & filename & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KOBAS.split /in &lt;kobas.out_run.txt&gt; [/out &lt;DIR&gt;]
''' ```
''' Split the KOBAS run output result text file as seperated csv file.
''' </summary>
'''

Public Function KOBASSplit([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KOBAS.split")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /KOBAS.Term.Kmeans /in &lt;dir.input&gt; [/n &lt;default=3&gt; /out &lt;out.clusters.csv&gt;]
''' ```
''' </summary>
'''

Public Function KOBASKMeans([in] As String, Optional n As String = "3", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/KOBAS.Term.Kmeans")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not n.StringEmpty Then
            Call CLI.Append("/n " & """" & n & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /labelFree.matrix /in &lt;*.csv/*.xlsx&gt; [/sheet &lt;default=proteinGroups&gt; /intensity /uniprot &lt;uniprot.Xml&gt; /organism &lt;scientificName&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function LabelFreeMatrix([in] As String, 
                                   Optional sheet As String = "proteinGroups", 
                                   Optional uniprot As String = "", 
                                   Optional organism As String = "", 
                                   Optional out As String = "", 
                                   Optional intensity As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/labelFree.matrix")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not sheet.StringEmpty Then
            Call CLI.Append("/sheet " & """" & sheet & """ ")
    End If
    If Not uniprot.StringEmpty Then
            Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    End If
    If Not organism.StringEmpty Then
            Call CLI.Append("/organism " & """" & organism & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If intensity Then
        Call CLI.Append("/intensity ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /labelFree.matrix.split /in &lt;matrix.csv&gt; /sampleInfo &lt;sampleInfo.csv&gt; /designer &lt;analysis_designer.csv&gt; [/out &lt;directory&gt;]
''' ```
''' </summary>
'''

Public Function LabelFreeMatrixSplit([in] As String, sampleInfo As String, designer As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/labelFree.matrix.split")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/sampleInfo " & """" & sampleInfo & """ ")
    Call CLI.Append("/designer " & """" & designer & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /labelFree.t.test /in &lt;matrix.csv&gt; /sampleInfo &lt;sampleInfo.csv&gt; /control &lt;groupName&gt; /treatment &lt;groupName&gt; [/significant &lt;t.test/AB, default=t.test&gt; /level &lt;default=1.5&gt; /p.value &lt;default=0.05&gt; /FDR &lt;default=0.05&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function labelFreeTtest([in] As String, 
                                  sampleInfo As String, 
                                  control As String, 
                                  treatment As String, 
                                  Optional significant As String = "t.test", 
                                  Optional level As String = "1.5", 
                                  Optional p_value As String = "0.05", 
                                  Optional fdr As String = "0.05", 
                                  Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/labelFree.t.test")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/sampleInfo " & """" & sampleInfo & """ ")
    Call CLI.Append("/control " & """" & control & """ ")
    Call CLI.Append("/treatment " & """" & treatment & """ ")
    If Not significant.StringEmpty Then
            Call CLI.Append("/significant " & """" & significant & """ ")
    End If
    If Not level.StringEmpty Then
            Call CLI.Append("/level " & """" & level & """ ")
    End If
    If Not p_value.StringEmpty Then
            Call CLI.Append("/p.value " & """" & p_value & """ ")
    End If
    If Not fdr.StringEmpty Then
            Call CLI.Append("/fdr " & """" & fdr & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /matrix.clustering /in &lt;matrix.csv&gt; [/cluster.n &lt;default:=10&gt; /out &lt;EntityClusterModel.csv&gt;]
''' ```
''' </summary>
'''

Public Function MatrixClustering([in] As String, Optional cluster_n As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/matrix.clustering")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not cluster_n.StringEmpty Then
            Call CLI.Append("/cluster.n " & """" & cluster_n & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Matrix.Normalization /in &lt;matrix.csv&gt; /infer &lt;min/avg, default=min&gt; [/out &lt;normalized.csv&gt;]
''' ```
''' </summary>
'''

Public Function MatrixNormalize([in] As String, infer As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Matrix.Normalization")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/infer " & """" & infer & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Merge.DEPs /in &lt;*.csv,DIR&gt; [/log2 /threshold &quot;log(1.5,2)&quot; /raw &lt;sample.csv&gt; /out &lt;out.csv&gt;]
''' ```
''' Usually using for generates the heatmap plot matrix of the DEPs. This function call will generates two dataset, one is using for the heatmap plot and another is using for the venn diagram plot.
''' </summary>
'''

Public Function MergeDEPs([in] As String, 
                             Optional threshold As String = "log(1.5,2)", 
                             Optional raw As String = "", 
                             Optional out As String = "", 
                             Optional log2 As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Merge.DEPs")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not threshold.StringEmpty Then
            Call CLI.Append("/threshold " & """" & threshold & """ ")
    End If
    If Not raw.StringEmpty Then
            Call CLI.Append("/raw " & """" & raw & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If log2 Then
        Call CLI.Append("/log2 ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /names /in &lt;matrix.csv&gt; /sampleInfo &lt;sampleInfo.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function MatrixColRenames([in] As String, sampleInfo As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/names")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/sampleInfo " & """" & sampleInfo & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Network.PCC /in &lt;matrix.csv&gt; [/cut &lt;default=0.45&gt; /out &lt;out.DIR&gt;]
''' ```
''' </summary>
'''

Public Function PccNetwork([in] As String, Optional cut As String = "0.45", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Network.PCC")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not cut.StringEmpty Then
            Call CLI.Append("/cut " & """" & cut & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /paired.sample.designer /sampleinfo &lt;sampleInfo.csv&gt; /designer &lt;analysisDesigner.csv&gt; /tuple &lt;sampleTuple.csv&gt; [/out &lt;designer.out.csv.Directory&gt;]
''' ```
''' </summary>
'''

Public Function PairedSampleDesigner(sampleinfo As String, designer As String, tuple As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/paired.sample.designer")
    Call CLI.Append(" ")
    Call CLI.Append("/sampleinfo " & """" & sampleinfo & """ ")
    Call CLI.Append("/designer " & """" & designer & """ ")
    Call CLI.Append("/tuple " & """" & tuple & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Perseus.MajorityProteinIDs /in &lt;table.csv&gt; [/out &lt;out.txt&gt;]
''' ```
''' Export the uniprot ID list from ``Majority Protein IDs`` row and generates a text file for batch search of the uniprot database.
''' </summary>
'''

Public Function MajorityProteinIDs([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Perseus.MajorityProteinIDs")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Perseus.Stat /in &lt;proteinGroups.txt&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function PerseusStatics([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Perseus.Stat")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Perseus.Table /in &lt;proteinGroups.txt&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function PerseusTable([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Perseus.Table")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Perseus.Table.annotations /in &lt;proteinGroups.csv&gt; /uniprot &lt;uniprot.XML&gt; [/scientifcName &lt;&quot;&quot;&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function PerseusTableAnnotations([in] As String, uniprot As String, Optional scientifcname As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Perseus.Table.annotations")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    If Not scientifcname.StringEmpty Then
            Call CLI.Append("/scientifcname " & """" & scientifcname & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /pfamstring.enrichment /in &lt;EntityClusterModel.csv&gt; /pfamstring &lt;pfamstring.csv&gt; [/out &lt;out.directory&gt;]
''' ```
''' </summary>
'''

Public Function PfamStringEnrichment([in] As String, pfamstring As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/pfamstring.enrichment")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/pfamstring " & """" & pfamstring & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /plot.pimw /in &lt;samples.csv&gt; [/field.pi &lt;default=&quot;calc. pI&quot;&gt; /field.mw &lt;default=&quot;MW [kDa]&quot;&gt; /legend.fontsize &lt;20&gt; /legend.size (100,30) /quantile.removes &lt;default=1&gt; /out &lt;pimw.png&gt; /size &lt;1600,1200&gt; /color &lt;black&gt; /ticks.Y &lt;-1&gt; /pt.size &lt;8&gt;]
''' ```
''' &apos;calc. pI&apos; - &apos;MW [kDa]&apos; scatter plot of the protomics raw sample data.
''' </summary>
'''
''' <param name="field_pi"> The field name that records the pI value of the protein samples, default is using iTraq result out format: ``calc. pI``
''' </param>
''' <param name="field_mw"> The field name that records the MW value of the protein samples, default is using iTraq result out format: ``MW [kDa]``
''' </param>
''' <param name="pt_size"> The radius size of the point in this scatter plot.
''' </param>
''' <param name="size"> The plot image its canvas size of this scatter plot.
''' </param>
''' <param name="quantile_removes"> All of the Y sample value greater than this quantile value will be removed. By default is quantile 100%, means no cuts.
''' </param>
Public Function pimwScatterPlot([in] As String, 
                                   Optional field_pi As String = "calc. pI", 
                                   Optional field_mw As String = "MW [kDa]", 
                                   Optional legend_fontsize As String = "", 
                                   Optional legend_size As String = "", 
                                   Optional quantile_removes As String = "1", 
                                   Optional out As String = "", 
                                   Optional size As String = "", 
                                   Optional color As String = "", 
                                   Optional ticks_y As String = "", 
                                   Optional pt_size As String = "") As Integer
    Dim CLI As New StringBuilder("/plot.pimw")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not field_pi.StringEmpty Then
            Call CLI.Append("/field.pi " & """" & field_pi & """ ")
    End If
    If Not field_mw.StringEmpty Then
            Call CLI.Append("/field.mw " & """" & field_mw & """ ")
    End If
    If Not legend_fontsize.StringEmpty Then
            Call CLI.Append("/legend.fontsize " & """" & legend_fontsize & """ ")
    End If
    If Not legend_size.StringEmpty Then
            Call CLI.Append("/legend.size " & """" & legend_size & """ ")
    End If
    If Not quantile_removes.StringEmpty Then
            Call CLI.Append("/quantile.removes " & """" & quantile_removes & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not color.StringEmpty Then
            Call CLI.Append("/color " & """" & color & """ ")
    End If
    If Not ticks_y.StringEmpty Then
            Call CLI.Append("/ticks.y " & """" & ticks_y & """ ")
    End If
    If Not pt_size.StringEmpty Then
            Call CLI.Append("/pt.size " & """" & pt_size & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /protein.annotations /uniprot &lt;uniprot.XML&gt; [/accession.ID /iTraq /list &lt;uniprot.id.list.txt/rawtable.csv/Xlsx&gt; /mapping &lt;mappings.tab/tsv&gt; /out &lt;out.csv&gt;]
''' ```
''' Total proteins functional annotation by using uniprot database.
''' </summary>
'''
''' <param name="list"> Using for the iTraq method result.
''' </param>
''' <param name="iTraq"> * Using for the iTraq method result. If this option was enabled, then the protein ID in the output table using be using the value from the uniprot ID field.
''' </param>
''' <param name="mapping"> The id mapping table, only works when the argument ``/list`` is presented.
''' </param>
''' <param name="uniprot"> The Uniprot protein database in XML file format.
''' </param>
''' <param name="accession_ID"> Using the uniprot protein ID from the ``/uniprot`` input as the generated dataset&apos;s ID value, instead of using the numeric sequence as the ID value.
''' </param>
''' <param name="out"> The file path for output protein annotation table where to save.
''' </param>
Public Function SampleAnnotations(uniprot As String, 
                                     Optional list As String = "", 
                                     Optional mapping As String = "", 
                                     Optional out As String = "", 
                                     Optional accession_id As Boolean = False, 
                                     Optional itraq As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/protein.annotations")
    Call CLI.Append(" ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    If Not list.StringEmpty Then
            Call CLI.Append("/list " & """" & list & """ ")
    End If
    If Not mapping.StringEmpty Then
            Call CLI.Append("/mapping " & """" & mapping & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If accession_id Then
        Call CLI.Append("/accession.id ")
    End If
    If itraq Then
        Call CLI.Append("/itraq ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /protein.annotations.shotgun /p1 &lt;data.csv&gt; /p2 &lt;data.csv&gt; /uniprot &lt;data.DIR/*.xml,*.tab&gt; [/remapping /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function SampleAnnotations2(p1 As String, 
                                      p2 As String, 
                                      uniprot As String, 
                                      Optional out As String = "", 
                                      Optional remapping As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/protein.annotations.shotgun")
    Call CLI.Append(" ")
    Call CLI.Append("/p1 " & """" & p1 & """ ")
    Call CLI.Append("/p2 " & """" & p2 & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If remapping Then
        Call CLI.Append("/remapping ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /proteinGroups.venn /in &lt;proteinGroups.csv&gt; /designer &lt;designer.csv&gt; [/label &lt;tag label&gt; /deli &lt;delimiter, default=_&gt; /out &lt;out.venn.DIR&gt;]
''' ```
''' </summary>
'''

Public Function proteinGroupsVenn([in] As String, 
                                     designer As String, 
                                     Optional label As String = "", 
                                     Optional deli As String = "_", 
                                     Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/proteinGroups.venn")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/designer " & """" & designer & """ ")
    If Not label.StringEmpty Then
            Call CLI.Append("/label " & """" & label & """ ")
    End If
    If Not deli.StringEmpty Then
            Call CLI.Append("/deli " & """" & deli & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /proteins.Go.plot /in &lt;proteins-uniprot-annotations.csv&gt; [/GO &lt;go.obo&gt; /label.right /colors &lt;default=Set1:c6&gt; /tick &lt;default=-1&gt; /level &lt;default=2&gt; /selects Q3 /size &lt;2000,2200&gt; /out &lt;out.DIR&gt;]
''' ```
''' ProteinGroups sample data go profiling plot from the uniprot annotation data.
''' </summary>
'''
''' <param name="GO"> The go database file path, if this argument is present in the CLI, then will using the GO.obo database file from GCModeller repository.
''' </param>
''' <param name="level"> The GO annotation level from the DAG, default is level 2. Argument value -1 means no level.
''' </param>
''' <param name="label_right"> Plot GO term their label will be alignment on right. default is alignment left if this aegument is not present.
''' </param>
''' <param name="[in]"> Uniprot XML database export result from ``/protein.annotations`` command.
''' </param>
''' <param name="tick"> The Axis ticking interval, if this argument is not present in the CLI, then program will create this interval value automatically.
''' </param>
''' <param name="size"> The size of the output plot image.
''' </param>
''' <param name="selects"> The quantity selector for the bar plot content, by default is using quartile Q3 value, which means the term should have at least greater than Q3 quantitle then it will be draw on the bar plot.
''' </param>
''' <param name="out"> A directory path which will created for save the output result. The output result from this command contains a bar plot png image and a csv file for view the Go terms distribution in the sample uniprot annotation data.
''' </param>
''' <param name="colors"> Change the default color profiles of the categories plots. Value can be a color profile name term or color name list that join by delimiter comma symbol:
'''               
'''               + &lt;profile name term&gt;: Set1:c6 
'''               Full list of the profile names: https://github.com/xieguigang/sciBASIC/blob/master/gr/Colors/colorbrewer/colorbrewer.json
'''               + &lt;color name list&gt;: black,green,blue 
'''               Full list of the color names: https://github.com/xieguigang/sciBASIC/blob/master/etc/VB.NET_Colors.html
''' </param>
Public Function ProteinsGoPlot([in] As String, 
                                  Optional go As String = "", 
                                  Optional colors As String = "Set1:c6", 
                                  Optional tick As String = "-1", 
                                  Optional level As String = "2", 
                                  Optional selects As String = "", 
                                  Optional size As String = "", 
                                  Optional out As String = "", 
                                  Optional label_right As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/proteins.Go.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not go.StringEmpty Then
            Call CLI.Append("/go " & """" & go & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not tick.StringEmpty Then
            Call CLI.Append("/tick " & """" & tick & """ ")
    End If
    If Not level.StringEmpty Then
            Call CLI.Append("/level " & """" & level & """ ")
    End If
    If Not selects.StringEmpty Then
            Call CLI.Append("/selects " & """" & selects & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If label_right Then
        Call CLI.Append("/label.right ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /proteins.KEGG.plot /in &lt;proteins-uniprot-annotations.csv&gt; [/field &lt;default=KO&gt; /not.human /geneId.field &lt;default=nothing&gt; /label.right /colors &lt;default=Set1:c6&gt; /custom &lt;sp00001.keg&gt; /size &lt;2200,2000&gt; /tick 20 /out &lt;out.DIR&gt;]
''' ```
''' KEGG function catalog profiling plot of the TP sample.
''' </summary>
'''
''' <param name="custom"> Custom KO classification set can be download from: http://www.kegg.jp/kegg-bin/get_htext?ko00001.keg. 
'''               You can replace the %s mark using kegg organism code in url example as: http://www.kegg.jp/kegg-bin/download_htext?htext=%s00001&amp;format=htext&amp;filedir= for download the custom KO classification set.
''' </param>
''' <param name="label_right"> Align the label from right.
''' </param>
''' <param name="size"> The canvas size value.
''' </param>
''' <param name="[in]"> Total protein annotation from UniProtKB database. Which is generated from the command ``/protein.annotations``.
''' </param>
''' <param name="colors"> Change the default color profiles of the categories plots. Value can be a color profile name term or color name list that join by delimiter comma symbol:
'''               
'''               + &lt;profile name term&gt;: Set1:c6 
'''               Full list of the profile names: https://github.com/xieguigang/sciBASIC/blob/master/gr/Colors/colorbrewer/colorbrewer.json
'''               + &lt;color name list&gt;: black,green,blue 
'''               Full list of the color names: https://github.com/xieguigang/sciBASIC/blob/master/etc/VB.NET_Colors.html
''' </param>
Public Function proteinsKEGGPlot([in] As String, 
                                    Optional field As String = "KO", 
                                    Optional geneid_field As String = "nothing", 
                                    Optional colors As String = "Set1:c6", 
                                    Optional custom As String = "", 
                                    Optional size As String = "", 
                                    Optional tick As String = "", 
                                    Optional out As String = "", 
                                    Optional not_human As Boolean = False, 
                                    Optional label_right As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/proteins.KEGG.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not field.StringEmpty Then
            Call CLI.Append("/field " & """" & field & """ ")
    End If
    If Not geneid_field.StringEmpty Then
            Call CLI.Append("/geneid.field " & """" & geneid_field & """ ")
    End If
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
    End If
    If Not custom.StringEmpty Then
            Call CLI.Append("/custom " & """" & custom & """ ")
    End If
    If Not size.StringEmpty Then
            Call CLI.Append("/size " & """" & size & """ ")
    End If
    If Not tick.StringEmpty Then
            Call CLI.Append("/tick " & """" & tick & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If not_human Then
        Call CLI.Append("/not.human ")
    End If
    If label_right Then
        Call CLI.Append("/label.right ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Relative.amount /in &lt;proteinGroups.csv&gt; /designer &lt;designer.csv&gt; [/uniprot &lt;annotations.csv&gt; /label &lt;tag label&gt; /deli &lt;delimiter, default=_&gt; /out &lt;out.csv&gt;]
''' ```
''' Statistics of the relative expression value of the total proteins.
''' </summary>
'''

Public Function RelativeAmount([in] As String, 
                                  designer As String, 
                                  Optional uniprot As String = "", 
                                  Optional label As String = "", 
                                  Optional deli As String = "_", 
                                  Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Relative.amount")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/designer " & """" & designer & """ ")
    If Not uniprot.StringEmpty Then
            Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    End If
    If Not label.StringEmpty Then
            Call CLI.Append("/label " & """" & label & """ ")
    End If
    If Not deli.StringEmpty Then
            Call CLI.Append("/deli " & """" & deli & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Retrieve.ID.mapping /list &lt;geneID.list&gt; /uniprot &lt;uniprot/uniparc.Xml&gt; [/out &lt;map.list.csv&gt;]
''' ```
''' Convert the protein id from other database to UniProtKB.
''' </summary>
'''

Public Function RetrieveIDmapping(list As String, uniprot As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Retrieve.ID.mapping")
    Call CLI.Append(" ")
    Call CLI.Append("/list " & """" & list & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /richfun.KOBAS /in &lt;string_interactions.tsv&gt; /uniprot &lt;uniprot.XML&gt; /DEP &lt;dep.t.test.csv&gt; /KOBAS &lt;enrichment.csv&gt; [/r.range &lt;default=5,20&gt; /fold &lt;1.5&gt; /iTraq /logFC &lt;logFC&gt; /layout &lt;string_network_coordinates.txt&gt; /out &lt;out.network.DIR&gt;]
''' ```
''' </summary>
'''
''' <param name="KOBAS"> The pvalue result in the enrichment term, will be using as the node radius size.
''' </param>
Public Function KOBASNetwork([in] As String, 
                                uniprot As String, 
                                DEP As String, 
                                KOBAS As String, 
                                Optional r_range As String = "5,20", 
                                Optional fold As String = "", 
                                Optional logfc As String = "", 
                                Optional layout As String = "", 
                                Optional out As String = "", 
                                Optional itraq As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/richfun.KOBAS")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    Call CLI.Append("/DEP " & """" & DEP & """ ")
    Call CLI.Append("/KOBAS " & """" & KOBAS & """ ")
    If Not r_range.StringEmpty Then
            Call CLI.Append("/r.range " & """" & r_range & """ ")
    End If
    If Not fold.StringEmpty Then
            Call CLI.Append("/fold " & """" & fold & """ ")
    End If
    If Not logfc.StringEmpty Then
            Call CLI.Append("/logfc " & """" & logfc & """ ")
    End If
    If Not layout.StringEmpty Then
            Call CLI.Append("/layout " & """" & layout & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If itraq Then
        Call CLI.Append("/itraq ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Sample.Species.Normalization /bbh &lt;bbh.csv&gt; /uniprot &lt;uniprot.XML/DIR&gt; /idMapping &lt;refSeq2uniprotKB_mappings.tsv&gt; /sample &lt;sample.csv&gt; [/Description &lt;Description.FileName&gt; /ID &lt;columnName&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''
''' <param name="bbh"> The queryName should be the entry accession ID in the uniprot and the subject name is the refSeq proteinID in the NCBI database.
''' </param>
Public Function NormalizeSpecies_samples(bbh As String, 
                                            uniprot As String, 
                                            idMapping As String, 
                                            sample As String, 
                                            Optional description As String = "", 
                                            Optional id As String = "", 
                                            Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Sample.Species.Normalization")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    Call CLI.Append("/idMapping " & """" & idMapping & """ ")
    Call CLI.Append("/sample " & """" & sample & """ ")
    If Not description.StringEmpty Then
            Call CLI.Append("/description " & """" & description & """ ")
    End If
    If Not id.StringEmpty Then
            Call CLI.Append("/id " & """" & id & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Samples.IDlist /in &lt;samples.csv&gt; [/uniprot /out &lt;out.list.txt&gt;]
''' ```
''' Extracts the protein hits from the protomics sample data, and using this ID list for downlaods the uniprot annotation data.
''' </summary>
'''

Public Function GetIDlistFromSampleTable([in] As String, Optional out As String = "", Optional uniprot As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Samples.IDlist")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If uniprot Then
        Call CLI.Append("/uniprot ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Shotgun.Data.Strip /in &lt;data.csv&gt; [/out &lt;output.csv&gt;]
''' ```
''' </summary>
'''

Public Function StripShotgunData([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Shotgun.Data.Strip")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Species.Normalization /bbh &lt;bbh.csv&gt; /uniprot &lt;uniprot.XML&gt; /idMapping &lt;refSeq2uniprotKB_mappings.tsv&gt; /annotations &lt;annotations.csv&gt; [/out &lt;out.csv&gt;]
''' ```
''' </summary>
'''
''' <param name="bbh"> The queryName should be the entry accession ID in the uniprot and the subject name is the refSeq proteinID in the NCBI database.
''' </param>
Public Function NormalizeSpecies(bbh As String, 
                                    uniprot As String, 
                                    idMapping As String, 
                                    annotations As String, 
                                    Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Species.Normalization")
    Call CLI.Append(" ")
    Call CLI.Append("/bbh " & """" & bbh & """ ")
    Call CLI.Append("/uniprot " & """" & uniprot & """ ")
    Call CLI.Append("/idMapping " & """" & idMapping & """ ")
    Call CLI.Append("/annotations " & """" & annotations & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /T.test.Designer.iTraq /in &lt;proteinGroups.csv&gt; /designer &lt;designer.csv&gt; [/label &lt;default is empty&gt; /deli &lt;default=-&gt; /out &lt;out.DIR&gt;]
''' ```
''' Generates the iTraq data t.test DEP method inputs table
''' </summary>
'''

Public Function TtestDesigner([in] As String, 
                                 designer As String, 
                                 Optional label As String = "", 
                                 Optional deli As String = "-", 
                                 Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/T.test.Designer.iTraq")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/designer " & """" & designer & """ ")
    If Not label.StringEmpty Then
            Call CLI.Append("/label " & """" & label & """ ")
    End If
    If Not deli.StringEmpty Then
            Call CLI.Append("/deli " & """" & deli & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /T.test.Designer.LFQ /in &lt;proteinGroups.csv&gt; /designer &lt;designer.csv&gt; [/label &lt;default is empty&gt; /deli &lt;default=-&gt; /out &lt;out.DIR&gt;]
''' ```
''' Generates the LFQ data t.test DEP method inputs table
''' </summary>
'''

Public Function TtestDesignerLFQ([in] As String, 
                                    designer As String, 
                                    Optional label As String = "", 
                                    Optional deli As String = "-", 
                                    Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/T.test.Designer.LFQ")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/designer " & """" & designer & """ ")
    If Not label.StringEmpty Then
            Call CLI.Append("/label " & """" & label & """ ")
    End If
    If Not deli.StringEmpty Then
            Call CLI.Append("/deli " & """" & deli & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Term2genes /in &lt;uniprot.XML&gt; [/term &lt;GO&gt; /id &lt;ORF&gt; /out &lt;out.tsv&gt;]
''' ```
''' </summary>
'''

Public Function Term2Genes([in] As String, Optional term As String = "", Optional id As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Term2genes")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not term.StringEmpty Then
            Call CLI.Append("/term " & """" & term & """ ")
    End If
    If Not id.StringEmpty Then
            Call CLI.Append("/id " & """" & id & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /UniProt.ID.Maps /in &lt;uniprot.Xml&gt; /dbName &lt;xref_dbname&gt; [/out &lt;maps.list&gt;]
''' ```
''' </summary>
'''

Public Function UniProtIDMaps([in] As String, dbName As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/UniProt.ID.Maps")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/dbName " & """" & dbName & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /UniProt.IDs /in &lt;list.csv/txt&gt; [/out &lt;list.txt&gt;]
''' ```
''' </summary>
'''

Public Function UniProtIDList([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/UniProt.IDs")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Uniprot.Mappings /in &lt;id.list&gt; [/type &lt;P_REFSEQ_AC&gt; /out &lt;out.DIR&gt;]
''' ```
''' Retrieve the uniprot annotation data by using ID mapping operations.
''' </summary>
'''

Public Function UniprotMappings([in] As String, Optional type As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Uniprot.Mappings")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not type.StringEmpty Then
            Call CLI.Append("/type " & """" & type & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /Uniprot.organism_id /in &lt;uniprot_data.Xml&gt; /dbName &lt;name&gt; [/out &lt;out.csv&gt;]
''' ```
''' Get uniprot_id to Organism-specific databases id map table.
''' </summary>
'''

Public Function OrganismSpecificDatabases([in] As String, dbName As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Uniprot.organism_id")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/dbName " & """" & dbName & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /UniRef.map.organism /in &lt;uniref.xml&gt; [/org &lt;organism_name&gt; /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''
''' <param name="[in]"> The uniRef XML cluster database its file path.
''' </param>
''' <param name="org"> The organism scientific name. If this argument is presented in the CLI input, then this program will output the top organism in this input data.
''' </param>
Public Function UniRefMap2Organism([in] As String, Optional org As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/UniRef.map.organism")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not org.StringEmpty Then
            Call CLI.Append("/org " & """" & org & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /UniRef.UniprotKB /in &lt;uniref.xml&gt; [/out &lt;maps.csv&gt;]
''' ```
''' </summary>
'''
''' <param name="[in]"> The uniRef XML cluster database its file path.
''' </param>
Public Function UniRef2UniprotKB([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/UniRef.UniprotKB")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```bash
''' /update.uniprot.mapped /in &lt;table.csv&gt; /mapping &lt;mapping.tsv/tab&gt; [/source /out &lt;out.csv&gt;]
''' ```
''' </summary>
'''

Public Function Update2UniprotMappedID([in] As String, mapping As String, Optional out As String = "", Optional source As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/update.uniprot.mapped")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    Call CLI.Append("/mapping " & """" & mapping & """ ")
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
    If source Then
        Call CLI.Append("/source ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function
End Class
End Namespace
