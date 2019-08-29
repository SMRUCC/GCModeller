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
'  // VERSION:   1.0.0.0
'  // COPYRIGHT: Copyright © SMRUCC genomics. 2014
'  // GUID:      a554d5f5-a2aa-46d6-8bbb-f7df46dbbe27
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
'    /enrichment.go:                         
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
'     /iTraq.RSD-P.Density:                   
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

     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As eggHTS
          Return New eggHTS(App:=directory & "/" & eggHTS.App)
     End Function

''' <summary>
''' ```
''' /blastX.fill.ORF /in &lt;annotations.csv> /blastx &lt;blastx.csv> [/out &lt;out.csv>]
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
''' ```
''' /COG.profiling.plot /in &lt;myvacog.csv> [/size &lt;image_size, default=1800,1200> /out &lt;out.png>]
''' ```
''' Plots the COGs category statics profiling of the target genome from the COG annotation file.
''' </summary>
'''
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
''' ```
''' /Converts /in &lt;GSEA.terms.csv> [/out &lt;result.terms.csv>]
''' ```
''' Converts the GCModeller enrichment analysis output as the KOBAS enrichment analysis result output table.
''' </summary>
'''
Public Function Converts([in] As String, Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/Converts")
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
''' ```
''' /Data.Add.Mappings /in &lt;data.csv> /bbh &lt;bbh.csv> /ID.mappings &lt;uniprot.ID.mappings.tsv> /uniprot &lt;uniprot.XML> [/ID &lt;fieldName> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function AddReMapping([in] As String, bbh As String, ID_mappings As String, uniprot As String, Optional id As String = "", Optional out As String = "") As Integer
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
''' ```
''' /Data.Add.ORF /in &lt;data.csv> /uniprot &lt;uniprot.XML> [/ID &lt;fieldName> /out &lt;out.csv>]
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
''' ```
''' /Data.Add.uniprotIDs /in &lt;annotations.csv> /data &lt;data.csv> [/out &lt;out.csv>]
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
''' ```
''' /DAVID.Split /in &lt;DAVID.txt> [/out &lt;out.DIR, default=./>]
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
''' ```
''' /DEP.heatmap.scatter.3D /in &lt;kmeans.csv> /sampleInfo &lt;sampleInfo.csv> [/display.labels &lt;default=-1> /cluster.prefix &lt;default="cluster: #"> /size &lt;default=1600,1400> /schema &lt;default=clusters> /view.angle &lt;default=30,60,-56.25> /view.distance &lt;default=2500> /arrow.factor &lt;default=1,2> /cluster.title &lt;names.csv> /out &lt;out.png>]
''' ```
''' Visualize the DEPs' kmeans cluster result by using 3D scatter plot.
''' </summary>
'''
Public Function DEPHeatmapScatter3D([in] As String, sampleInfo As String, Optional display_labels As String = "-1", Optional cluster_prefix As String = "cluster: #", Optional size As String = "1600,1400", Optional schema As String = "clusters", Optional view_angle As String = "30,60,-56.25", Optional view_distance As String = "2500", Optional arrow_factor As String = "1,2", Optional cluster_title As String = "", Optional out As String = "") As Integer
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
''' ```
''' /DEP.kmeans.scatter2D /in &lt;kmeans.csv> /sampleInfo &lt;sampleInfo.csv> [/t.log &lt;default=-1> /cluster.prefix &lt;default="cluster: #"> /size &lt;2500,2200> /pt.size &lt;radius pixels, default=15> /schema &lt;default=clusters> /out &lt;out.png>]
''' ```
''' </summary>
'''
Public Function DEPKmeansScatter2D([in] As String, sampleInfo As String, Optional t_log As String = "-1", Optional cluster_prefix As String = "cluster: #", Optional size As String = "", Optional pt_size As String = "15", Optional schema As String = "clusters", Optional out As String = "") As Integer
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
''' ```
''' /DEP.logFC.hist /in &lt;log2test.csv> [/step &lt;0.25> /type &lt;default=log2fc> /legend.title &lt;Frequency(log2FC)> /x.axis "(min,max),tick=0.25" /color &lt;lightblue> /size &lt;1400,900> /out &lt;out.png>]
''' ```
''' Using for plots the FC histogram when the experiment have no biological replicates.
''' </summary>
'''
Public Function logFCHistogram([in] As String, Optional [step] As String = "", Optional type As String = "log2fc", Optional legend_title As String = "", Optional x_axis As String = "(min,max),tick=0.25", Optional color As String = "", Optional size As String = "", Optional out As String = "") As Integer
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
''' ```
''' /DEP.logFC.Volcano /in &lt;DEP-log2FC.t.test-table.csv> [/title &lt;title> /p.value &lt;default=0.05> /level &lt;default=1.5> /colors &lt;up=red;down=green;other=black> /label.p &lt;default=-1> /size &lt;1400,1400> /display.count /out &lt;plot.csv>]
''' ```
''' Volcano plot of the DEPs' analysis result.
''' </summary>
'''
Public Function logFCVolcano([in] As String, Optional title As String = "", Optional p_value As String = "0.05", Optional level As String = "1.5", Optional colors As String = "", Optional label_p As String = "-1", Optional size As String = "", Optional out As String = "", Optional display_count As Boolean = False) As Integer
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
''' ```
''' /DEP.uniprot.list /DEP &lt;log2-test.DEP.csv> /sample &lt;sample.csv> [/out &lt;out.txt>]
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
''' ```
''' /DEP.uniprot.list2 /in &lt;log2.test.csv> [/DEP.Flag &lt;is.DEP?> /uniprot.Flag &lt;uniprot> /species &lt;scientifcName> /uniprot &lt;uniprotXML> /out &lt;out.txt>]
''' ```
''' </summary>
'''
Public Function DEPUniprotIDs2([in] As String, Optional dep_flag As String = "", Optional uniprot_flag As String = "", Optional species As String = "", Optional uniprot As String = "", Optional out As String = "") As Integer
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
''' ```
''' /DEP.venn /data &lt;Directory> [/title &lt;VennDiagram title> /out &lt;out.DIR>]
''' ```
''' Generate the VennDiagram plot data and the venn plot tiff. The default parameter profile is using for the iTraq data.
''' </summary>
'''
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
''' ```
''' /DEPs.heatmap /data &lt;Directory/csv_file> [/labelFree /schema &lt;color_schema, default=RdYlGn:c11> /no-clrev /KO.class /annotation &lt;annotation.csv> /row.labels.geneName /hide.labels /is.matrix /cluster.n &lt;default=6> /sampleInfo &lt;sampleinfo.csv> /non_DEP.blank /title "Heatmap of DEPs log2FC" /t.log2 /tick &lt;-1> /size &lt;size, default=2000,3000> /legend.size &lt;size, default=600,100> /out &lt;out.DIR>]
''' ```
''' Generates the heatmap plot input data. The default label profile is using for the iTraq result.
''' </summary>
'''
Public Function DEPs_heatmapKmeans(data As String, Optional schema As String = "RdYlGn:c11", Optional annotation As String = "", Optional cluster_n As String = "6", Optional sampleinfo As String = "", Optional title As String = "Heatmap of DEPs log2FC", Optional tick As String = "", Optional size As String = "2000,3000", Optional legend_size As String = "600,100", Optional out As String = "", Optional labelfree As Boolean = False, Optional no_clrev As Boolean = False, Optional ko_class As Boolean = False, Optional row_labels_genename As Boolean = False, Optional hide_labels As Boolean = False, Optional is_matrix As Boolean = False, Optional non_dep_blank As Boolean = False, Optional t_log2 As Boolean = False) As Integer
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
''' ```
''' /DEPs.stat /in &lt;log2.test.csv> [/log2FC &lt;default=log2FC> /out &lt;out.stat.csv>]
''' ```
''' https://github.com/xieguigang/GCModeller.cli2R/blob/master/GCModeller.cli2R/R/log2FC_t-test.R
''' </summary>
'''
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
''' ```
''' /DEPs.takes.values /in &lt;DEPs.csv> [/boolean.tag &lt;default=is.DEP> /by.FC &lt;tag=value, default=logFC=log2(1.5)> /by.p.value &lt;tag=value, p.value=0.05> /data &lt;data.csv> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function TakeDEPsValues([in] As String, Optional boolean_tag As String = "is.DEP", Optional by_fc As String = "logFC=log2(1.5)", Optional by_p_value As String = "", Optional data As String = "", Optional out As String = "") As Integer
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
''' ```
''' /DEPs.union /in &lt;csv.DIR> [/FC &lt;default=logFC> /out &lt;out.csv>]
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
''' ```
''' /edgeR.Designer /in &lt;proteinGroups.csv> /designer &lt;designer.csv> [/label &lt;default is empty> /deli &lt;default=-> /out &lt;out.DIR>]
''' ```
''' Generates the edgeR inputs table
''' </summary>
'''
Public Function edgeRDesigner([in] As String, designer As String, Optional label As String = "", Optional deli As String = "-", Optional out As String = "") As Integer
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
''' ```
''' /enricher.background /in &lt;uniprot.XML> [/mapping &lt;maps.tsv> /out &lt;term2gene.txt.DIR>]
''' ```
''' Create enrichment analysis background based on the uniprot xml database.
''' </summary>
'''
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
''' ```
''' /enrichment.go /deg &lt;deg.list> /backgrounds &lt;genome_genes.list> /t2g &lt;term2gene.csv> [/go &lt;go_brief.csv> /out &lt;enricher.result.csv>]
''' ```
''' </summary>
'''
Public Function GoEnrichment(deg As String, backgrounds As String, t2g As String, Optional go As String = "", Optional out As String = "") As Integer
    Dim CLI As New StringBuilder("/enrichment.go")
    Call CLI.Append(" ")
    Call CLI.Append("/deg " & """" & deg & """ ")
    Call CLI.Append("/backgrounds " & """" & backgrounds & """ ")
    Call CLI.Append("/t2g " & """" & t2g & """ ")
    If Not go.StringEmpty Then
            Call CLI.Append("/go " & """" & go & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /Enrichment.Term.Filter /in &lt;enrichment.csv> /filter &lt;key-string> [/out &lt;out.csv>]
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
''' ```
''' /Enrichments.ORF.info /in &lt;enrichment.csv> /proteins &lt;uniprot-genome.XML> [/nocut /ORF /out &lt;out.csv>]
''' ```
''' Retrive KEGG/GO info for the genes in the enrichment result.
''' </summary>
'''
Public Function RetriveEnrichmentGeneInfo([in] As String, proteins As String, Optional out As String = "", Optional nocut As Boolean = False, Optional orf As Boolean = False) As Integer
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
''' ```
''' /Exocarta.Hits /in &lt;list.txt> /annotation &lt;annotations.csv> /exocarta &lt;Exocarta.tsv> [/out &lt;out.csv>]
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
''' ```
''' /Fasta.IDlist /in &lt;prot.fasta> [/out &lt;geneIDs.txt>]
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
''' ```
''' /FoldChange.Matrix.Invert /in &lt;data.csv> [/log2FC /out &lt;invert.csv>]
''' ```
''' Reverse the FoldChange value from the source result matrix.
''' </summary>
'''
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
''' ```
''' /func.rich.string /in &lt;string_interactions.tsv> /uniprot &lt;uniprot.XML> /DEP &lt;dep.t.test.csv> [/map &lt;map.tsv> /r.range &lt;default=12,30> /log2FC &lt;default=log2FC> /layout &lt;string_network_coordinates.txt> /out &lt;out.network.DIR>]
''' ```
''' DEPs' functional enrichment network based on string-db exports, and color by KEGG pathway.
''' </summary>
'''
Public Function FunctionalNetworkEnrichment([in] As String, uniprot As String, DEP As String, Optional map As String = "", Optional r_range As String = "12,30", Optional log2fc As String = "log2FC", Optional layout As String = "", Optional out As String = "") As Integer
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
''' ```
''' /Gene.list.from.KOBAS /in &lt;KOBAS.csv> [/p.value &lt;default=1> /out &lt;out.txt>]
''' ```
''' Using this command for generates the gene id list input for the STRING-db search.
''' </summary>
'''
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
''' ```
''' /GO.cellular_location.Plot /in &lt;KOBAS.GO.csv> [/GO &lt;go.obo> /3D /colors &lt;schemaName, default=Paired:c8> /out &lt;out.png>]
''' ```
''' Visualize of the subcellular location result from the GO enrichment analysis.
''' </summary>
'''
Public Function GO_cellularLocationPlot([in] As String, Optional go As String = "", Optional colors As String = "Paired:c8", Optional out As String = "", Optional _3d As Boolean = False) As Integer
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
''' ```
''' /GO.enrichment.DAVID /in &lt;DAVID.csv> [/tsv /go &lt;go.obo> /colors &lt;default=Set1:c6> /size &lt;default=1200,1000> /tick 1 /p.value &lt;0.05> /out &lt;out.png>]
''' ```
''' </summary>
'''
Public Function DAVID_GOplot([in] As String, Optional go As String = "", Optional colors As String = "Set1:c6", Optional size As String = "1200,1000", Optional tick As String = "", Optional p_value As String = "", Optional out As String = "", Optional tsv As Boolean = False) As Integer
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
''' ```
''' /Go.enrichment.plot /in &lt;enrichmentTerm.csv> [/bubble /r "log(x,1.5)" /Corrected /displays &lt;default=10> /PlantRegMap /label.right /colors &lt;default=Set1:c6> /gray /pvalue &lt;0.05> /size &lt;2000,1600> /tick 1 /go &lt;go.obo> /out &lt;out.png>]
''' ```
''' Go enrichment plot base on the KOBAS enrichment analysis result.
''' </summary>
'''
Public Function GO_enrichmentPlot([in] As String, Optional r As String = "log(x,1.5)", Optional displays As String = "10", Optional colors As String = "Set1:c6", Optional pvalue As String = "", Optional size As String = "", Optional tick As String = "", Optional go As String = "", Optional out As String = "", Optional bubble As Boolean = False, Optional corrected As Boolean = False, Optional plantregmap As Boolean = False, Optional label_right As Boolean = False, Optional gray As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/Go.enrichment.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not r.StringEmpty Then
            Call CLI.Append("/r " & """" & r & """ ")
    End If
    If Not displays.StringEmpty Then
            Call CLI.Append("/displays " & """" & displays & """ ")
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
    If gray Then
        Call CLI.Append("/gray ")
    End If
     Call CLI.Append("/@set --internal_pipeline=TRUE ")


    Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
    Return proc.Run()
End Function

''' <summary>
''' ```
''' /iBAQ.Cloud /in &lt;expression.csv> /annotations &lt;annotations.csv> /DEPs &lt;DEPs.csv> /tag &lt;expression> [/out &lt;out.png>]
''' ```
''' Cloud plot of the iBAQ DEPs result.
''' </summary>
'''
Public Function DEPsCloudPlot([in] As String, annotations As String, DEPs As String, tag As String, Optional out As String = "") As Integer
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
''' ```
''' /ID.Replace.bbh /in &lt;dataset.csv> /bbh &lt;bbh/sbh.csv> [/description &lt;fieldName, default=Description> /out &lt;ID.replaced.csv>]
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
''' ```
''' /Imports.Go.obo.mysql /in &lt;go.obo> [/out &lt;out.sql>]
''' ```
''' Dumping GO obo database as mysql database files.
''' </summary>
'''
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
''' ```
''' /Imports.Uniprot.Xml /in &lt;uniprot.xml> [/out &lt;out.sql>]
''' ```
''' Dumping the UniprotKB XML database as mysql database file.
''' </summary>
'''
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
''' ```
''' /iTraq.Bridge.Matrix /A &lt;A_iTraq.csv> /B &lt;B_iTraq.csv> /C &lt;bridge_symbol> [/symbols.A &lt;symbols.csv> /symbols.B &lt;symbols.csv> /out &lt;matrix.csv>]
''' ```
''' </summary>
'''
Public Function iTraqBridge(A As String, B As String, C As String, Optional symbols_a As String = "", Optional symbols_b As String = "", Optional out As String = "") As Integer
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
''' ```
''' /iTraq.matrix.split /in &lt;matrix.csv> /sampleInfo &lt;sampleInfo.csv> /designer &lt;analysis.design.csv> [/allowed.swap /out &lt;out.Dir>]
''' ```
''' Split the raw matrix into different compare group based on the experimental designer information.
''' </summary>
'''
Public Function iTraqAnalysisMatrixSplit([in] As String, sampleInfo As String, designer As String, Optional out As String = "", Optional allowed_swap As Boolean = False) As Integer
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
''' ```
''' /iTraq.RSD-P.Density /in &lt;matrix.csv> [/out &lt;out.png>]
''' ```
''' </summary>
'''
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
''' ```
''' /iTraq.Symbol.Replacement /in &lt;iTraq.data.csv/xlsx> /symbols &lt;symbols.csv/xlsx> [/sheet.name &lt;Sheet1> /symbolSheet &lt;sheetName> /out &lt;out.DIR>]
''' ```
''' * Using this CLI tool for processing the tag header of iTraq result at first.
''' </summary>
'''
Public Function iTraqSignReplacement([in] As String, symbols As String, Optional sheet_name As String = "", Optional symbolsheet As String = "", Optional out As String = "") As Integer
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
''' ```
''' /iTraq.t.test /in &lt;matrix.csv> [/level &lt;default=1.5> /p.value &lt;default=0.05> /FDR &lt;default=0.05> /skip.significant.test /pairInfo &lt;sampleTuple.csv> /out &lt;out.csv>]
''' ```
''' Implements the screening for different expression proteins by using log2FC threshold and t.test pvalue threshold.
''' </summary>
'''
Public Function iTraqTtest([in] As String, Optional level As String = "1.5", Optional p_value As String = "0.05", Optional fdr As String = "0.05", Optional pairinfo As String = "", Optional out As String = "", Optional skip_significant_test As Boolean = False) As Integer
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
''' ```
''' /KEGG.Color.Pathway /in &lt;protein.annotations.csv> /ref &lt;KEGG.ref.pathwayMap.directory repository> [/out &lt;out.directory>]
''' ```
''' </summary>
'''
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
''' ```
''' /KEGG.enrichment.DAVID /in &lt;david.csv> [/tsv /custom &lt;ko00001.keg> /colors &lt;default=Set1:c6> /size &lt;default=1200,1000> /p.value &lt;default=0.05> /tick 1 /out &lt;out.png>]
''' ```
''' </summary>
'''
Public Function DAVID_KEGGplot([in] As String, Optional custom As String = "", Optional colors As String = "Set1:c6", Optional size As String = "1200,1000", Optional p_value As String = "0.05", Optional tick As String = "", Optional out As String = "", Optional tsv As Boolean = False) As Integer
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
''' ```
''' /KEGG.enrichment.DAVID.pathwaymap /in &lt;david.csv> /uniprot &lt;uniprot.XML> [/tsv /DEPs &lt;deps.csv> /colors &lt;default=red,blue,green> /tag &lt;default=log2FC> /pvalue &lt;default=0.05> /out &lt;out.DIR>]
''' ```
''' </summary>
'''
Public Function DAVID_KEGGPathwayMap([in] As String, uniprot As String, Optional deps As String = "", Optional colors As String = "red,blue,green", Optional tag As String = "log2FC", Optional pvalue As String = "0.05", Optional out As String = "", Optional tsv As Boolean = False) As Integer
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
''' ```
''' /KEGG.Enrichment.PathwayMap /in &lt;kobas.csv> [/DEPs &lt;deps.csv> /colors &lt;default=red,blue,green> /map &lt;id2uniprotID.txt> /uniprot &lt;uniprot.XML> /pvalue &lt;default=0.05> /out &lt;DIR>]
''' ```
''' Show the KEGG pathway map image by using KOBAS KEGG pathway enrichment result.
''' </summary>
'''
Public Function KEGGEnrichmentPathwayMap([in] As String, Optional deps As String = "", Optional colors As String = "red,blue,green", Optional map As String = "", Optional uniprot As String = "", Optional pvalue As String = "0.05", Optional out As String = "") As Integer
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
''' ```
''' /KEGG.Enrichment.PathwayMap.Render /in &lt;enrichment.csv> [/repo &lt;maps.directory> /DEPs &lt;deps.csv> /colors &lt;default=red,blue,green> /map &lt;id2uniprotID.txt> /uniprot &lt;uniprot.XML> /pvalue &lt;default=0.05> /out &lt;DIR>]
''' ```
''' KEGG pathway map enrichment analysis visual rendering locally. This function required a local kegg pathway repository.
''' </summary>
'''
Public Function KEGGEnrichmentPathwayMapLocal([in] As String, Optional repo As String = "", Optional deps As String = "", Optional colors As String = "red,blue,green", Optional map As String = "", Optional uniprot As String = "", Optional pvalue As String = "0.05", Optional out As String = "") As Integer
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
''' ```
''' /KEGG.enrichment.plot /in &lt;enrichmentTerm.csv> [/gray /colors &lt;default=Set1:c6> /label.right /pvalue &lt;0.05> /tick 1 /size &lt;2000,1600> /out &lt;out.png>]
''' ```
''' Bar plots of the KEGG enrichment analysis result.
''' </summary>
'''
Public Function KEGG_enrichment([in] As String, Optional colors As String = "Set1:c6", Optional pvalue As String = "", Optional tick As String = "", Optional size As String = "", Optional out As String = "", Optional gray As Boolean = False, Optional label_right As Boolean = False) As Integer
    Dim CLI As New StringBuilder("/KEGG.enrichment.plot")
    Call CLI.Append(" ")
    Call CLI.Append("/in " & """" & [in] & """ ")
    If Not colors.StringEmpty Then
            Call CLI.Append("/colors " & """" & colors & """ ")
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
''' ```
''' /KO.Catalogs /in &lt;blast.mapping.csv> /ko &lt;ko_genes.csv> [/key &lt;Query_id> /mapTo &lt;Subject_id> /out &lt;outDIR>]
''' ```
''' Display the barplot of the KEGG orthology match.
''' </summary>
'''
Public Function KOCatalogs([in] As String, ko As String, Optional key As String = "", Optional mapto As String = "", Optional out As String = "") As Integer
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
''' ```
''' /KOBAS.add.ORF /in &lt;table.csv> /sample &lt;sample.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
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
''' ```
''' /KOBAS.Sim.Heatmap /in &lt;sim.csv> [/size &lt;1024,800> /colors &lt;RdYlBu:8> /out &lt;out.png>]
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
''' ```
''' /KOBAS.Similarity /group1 &lt;DIR> /group2 &lt;DIR> [/fileName &lt;default=output_run-Gene Ontology.csv> /out &lt;out.DIR>]
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
''' ```
''' /KOBAS.split /in &lt;kobas.out_run.txt> [/out &lt;DIR>]
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
''' ```
''' /KOBAS.Term.Kmeans /in &lt;dir.input> [/n &lt;default=3> /out &lt;out.clusters.csv>]
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
''' ```
''' /labelFree.matrix /in &lt;*.csv/*.xlsx> [/sheet &lt;default=proteinGroups> /intensity /uniprot &lt;uniprot.Xml> /organism &lt;scientificName> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function LabelFreeMatrix([in] As String, Optional sheet As String = "proteinGroups", Optional uniprot As String = "", Optional organism As String = "", Optional out As String = "", Optional intensity As Boolean = False) As Integer
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
''' ```
''' /labelFree.matrix.split /in &lt;matrix.csv> /sampleInfo &lt;sampleInfo.csv> /designer &lt;analysis_designer.csv> [/out &lt;directory>]
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
''' ```
''' /labelFree.t.test /in &lt;matrix.csv> /sampleInfo &lt;sampleInfo.csv> /control &lt;groupName> /treatment &lt;groupName> [/significant &lt;t.test/AB, default=t.test> /level &lt;default=1.5> /p.value &lt;default=0.05> /FDR &lt;default=0.05> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function labelFreeTtest([in] As String, sampleInfo As String, control As String, treatment As String, Optional significant As String = "t.test", Optional level As String = "1.5", Optional p_value As String = "0.05", Optional fdr As String = "0.05", Optional out As String = "") As Integer
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
''' ```
''' /matrix.clustering /in &lt;matrix.csv> [/cluster.n &lt;default:=10> /out &lt;EntityClusterModel.csv>]
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
''' ```
''' /Matrix.Normalization /in &lt;matrix.csv> /infer &lt;min/avg, default=min> [/out &lt;normalized.csv>]
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
''' ```
''' /Merge.DEPs /in &lt;*.csv,DIR> [/log2 /threshold "log(1.5,2)" /raw &lt;sample.csv> /out &lt;out.csv>]
''' ```
''' Usually using for generates the heatmap plot matrix of the DEPs. This function call will generates two dataset, one is using for the heatmap plot and another is using for the venn diagram plot.
''' </summary>
'''
Public Function MergeDEPs([in] As String, Optional threshold As String = "log(1.5,2)", Optional raw As String = "", Optional out As String = "", Optional log2 As Boolean = False) As Integer
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
''' ```
''' /names /in &lt;matrix.csv> /sampleInfo &lt;sampleInfo.csv> [/out &lt;out.csv>]
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
''' ```
''' /Network.PCC /in &lt;matrix.csv> [/cut &lt;default=0.45> /out &lt;out.DIR>]
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
''' ```
''' /paired.sample.designer /sampleinfo &lt;sampleInfo.csv> /designer &lt;analysisDesigner.csv> /tuple &lt;sampleTuple.csv> [/out &lt;designer.out.csv.Directory>]
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
''' ```
''' /Perseus.MajorityProteinIDs /in &lt;table.csv> [/out &lt;out.txt>]
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
''' ```
''' /Perseus.Stat /in &lt;proteinGroups.txt> [/out &lt;out.csv>]
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
''' ```
''' /Perseus.Table /in &lt;proteinGroups.txt> [/out &lt;out.csv>]
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
''' ```
''' /Perseus.Table.annotations /in &lt;proteinGroups.csv> /uniprot &lt;uniprot.XML> [/scientifcName &lt;""> /out &lt;out.csv>]
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
''' ```
''' /pfamstring.enrichment /in &lt;EntityClusterModel.csv> /pfamstring &lt;pfamstring.csv> [/out &lt;out.directory>]
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
''' ```
''' /plot.pimw /in &lt;samples.csv> [/field.pi &lt;default="calc. pI"> /field.mw &lt;default="MW [kDa]"> /legend.fontsize &lt;20> /legend.size (100,30) /quantile.removes &lt;default=1> /out &lt;pimw.png> /size &lt;1600,1200> /color &lt;black> /ticks.Y &lt;-1> /pt.size &lt;8>]
''' ```
''' 'calc. pI' - 'MW [kDa]' scatter plot of the protomics raw sample data.
''' </summary>
'''
Public Function pimwScatterPlot([in] As String, Optional field_pi As String = "calc. pI", Optional field_mw As String = "MW [kDa]", Optional legend_fontsize As String = "", Optional legend_size As String = "", Optional quantile_removes As String = "1", Optional out As String = "", Optional size As String = "", Optional color As String = "", Optional ticks_y As String = "", Optional pt_size As String = "") As Integer
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
''' ```
''' /protein.annotations /uniprot &lt;uniprot.XML> [/accession.ID /iTraq /list &lt;uniprot.id.list.txt/rawtable.csv/Xlsx> /mapping &lt;mappings.tab/tsv> /out &lt;out.csv>]
''' ```
''' Total proteins functional annotation by using uniprot database.
''' </summary>
'''
Public Function SampleAnnotations(uniprot As String, Optional list As String = "", Optional mapping As String = "", Optional out As String = "", Optional accession_id As Boolean = False, Optional itraq As Boolean = False) As Integer
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
''' ```
''' /protein.annotations.shotgun /p1 &lt;data.csv> /p2 &lt;data.csv> /uniprot &lt;data.DIR/*.xml,*.tab> [/remapping /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function SampleAnnotations2(p1 As String, p2 As String, uniprot As String, Optional out As String = "", Optional remapping As Boolean = False) As Integer
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
''' ```
''' /proteinGroups.venn /in &lt;proteinGroups.csv> /designer &lt;designer.csv> [/label &lt;tag label> /deli &lt;delimiter, default=_> /out &lt;out.venn.DIR>]
''' ```
''' </summary>
'''
Public Function proteinGroupsVenn([in] As String, designer As String, Optional label As String = "", Optional deli As String = "_", Optional out As String = "") As Integer
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
''' ```
''' /proteins.Go.plot /in &lt;proteins-uniprot-annotations.csv> [/GO &lt;go.obo> /label.right /colors &lt;default=Set1:c6> /tick &lt;default=-1> /level &lt;default=2> /selects Q3 /size &lt;2000,2200> /out &lt;out.DIR>]
''' ```
''' ProteinGroups sample data go profiling plot from the uniprot annotation data.
''' </summary>
'''
Public Function ProteinsGoPlot([in] As String, Optional go As String = "", Optional colors As String = "Set1:c6", Optional tick As String = "-1", Optional level As String = "2", Optional selects As String = "", Optional size As String = "", Optional out As String = "", Optional label_right As Boolean = False) As Integer
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
''' ```
''' /proteins.KEGG.plot /in &lt;proteins-uniprot-annotations.csv> [/field &lt;default=KO> /not.human /geneId.field &lt;default=nothing> /label.right /colors &lt;default=Set1:c6> /custom &lt;sp00001.keg> /size &lt;2200,2000> /tick 20 /out &lt;out.DIR>]
''' ```
''' KEGG function catalog profiling plot of the TP sample.
''' </summary>
'''
Public Function proteinsKEGGPlot([in] As String, Optional field As String = "KO", Optional geneid_field As String = "nothing", Optional colors As String = "Set1:c6", Optional custom As String = "", Optional size As String = "", Optional tick As String = "", Optional out As String = "", Optional not_human As Boolean = False, Optional label_right As Boolean = False) As Integer
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
''' ```
''' /Relative.amount /in &lt;proteinGroups.csv> /designer &lt;designer.csv> [/uniprot &lt;annotations.csv> /label &lt;tag label> /deli &lt;delimiter, default=_> /out &lt;out.csv>]
''' ```
''' Statistics of the relative expression value of the total proteins.
''' </summary>
'''
Public Function RelativeAmount([in] As String, designer As String, Optional uniprot As String = "", Optional label As String = "", Optional deli As String = "_", Optional out As String = "") As Integer
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
''' ```
''' /Retrieve.ID.mapping /list &lt;geneID.list> /uniprot &lt;uniprot/uniparc.Xml> [/out &lt;map.list.csv>]
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
''' ```
''' /richfun.KOBAS /in &lt;string_interactions.tsv> /uniprot &lt;uniprot.XML> /DEP &lt;dep.t.test.csv> /KOBAS &lt;enrichment.csv> [/r.range &lt;default=5,20> /fold &lt;1.5> /iTraq /logFC &lt;logFC> /layout &lt;string_network_coordinates.txt> /out &lt;out.network.DIR>]
''' ```
''' </summary>
'''
Public Function KOBASNetwork([in] As String, uniprot As String, DEP As String, KOBAS As String, Optional r_range As String = "5,20", Optional fold As String = "", Optional logfc As String = "", Optional layout As String = "", Optional out As String = "", Optional itraq As Boolean = False) As Integer
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
''' ```
''' /Sample.Species.Normalization /bbh &lt;bbh.csv> /uniprot &lt;uniprot.XML/DIR> /idMapping &lt;refSeq2uniprotKB_mappings.tsv> /sample &lt;sample.csv> [/Description &lt;Description.FileName> /ID &lt;columnName> /out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function NormalizeSpecies_samples(bbh As String, uniprot As String, idMapping As String, sample As String, Optional description As String = "", Optional id As String = "", Optional out As String = "") As Integer
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
''' ```
''' /Samples.IDlist /in &lt;samples.csv> [/uniprot /out &lt;out.list.txt>]
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
''' ```
''' /Shotgun.Data.Strip /in &lt;data.csv> [/out &lt;output.csv>]
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
''' ```
''' /Species.Normalization /bbh &lt;bbh.csv> /uniprot &lt;uniprot.XML> /idMapping &lt;refSeq2uniprotKB_mappings.tsv> /annotations &lt;annotations.csv> [/out &lt;out.csv>]
''' ```
''' </summary>
'''
Public Function NormalizeSpecies(bbh As String, uniprot As String, idMapping As String, annotations As String, Optional out As String = "") As Integer
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
''' ```
''' /T.test.Designer.iTraq /in &lt;proteinGroups.csv> /designer &lt;designer.csv> [/label &lt;default is empty> /deli &lt;default=-> /out &lt;out.DIR>]
''' ```
''' Generates the iTraq data t.test DEP method inputs table
''' </summary>
'''
Public Function TtestDesigner([in] As String, designer As String, Optional label As String = "", Optional deli As String = "-", Optional out As String = "") As Integer
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
''' ```
''' /T.test.Designer.LFQ /in &lt;proteinGroups.csv> /designer &lt;designer.csv> [/label &lt;default is empty> /deli &lt;default=-> /out &lt;out.DIR>]
''' ```
''' Generates the LFQ data t.test DEP method inputs table
''' </summary>
'''
Public Function TtestDesignerLFQ([in] As String, designer As String, Optional label As String = "", Optional deli As String = "-", Optional out As String = "") As Integer
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
''' ```
''' /Term2genes /in &lt;uniprot.XML> [/term &lt;GO> /id &lt;ORF> /out &lt;out.tsv>]
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
''' ```
''' /UniProt.ID.Maps /in &lt;uniprot.Xml> /dbName &lt;xref_dbname> [/out &lt;maps.list>]
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
''' ```
''' /UniProt.IDs /in &lt;list.csv/txt> [/out &lt;list.txt>]
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
''' ```
''' /Uniprot.Mappings /in &lt;id.list> [/type &lt;P_REFSEQ_AC> /out &lt;out.DIR>]
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
''' ```
''' /Uniprot.organism_id /in &lt;uniprot_data.Xml> /dbName &lt;name> [/out &lt;out.csv>]
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
''' ```
''' /UniRef.map.organism /in &lt;uniref.xml> [/org &lt;organism_name> /out &lt;out.csv>]
''' ```
''' </summary>
'''
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
''' ```
''' /UniRef.UniprotKB /in &lt;uniref.xml> [/out &lt;maps.csv>]
''' ```
''' </summary>
'''
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
''' ```
''' /update.uniprot.mapped /in &lt;table.csv> /mapping &lt;mapping.tsv/tab> [/source /out &lt;out.csv>]
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
