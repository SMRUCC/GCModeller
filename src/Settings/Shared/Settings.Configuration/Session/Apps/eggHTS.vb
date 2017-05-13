Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/eggHTS.exe

Namespace GCModellerApps


    ''' <summary>
    ''' eggHTS.CLI
    ''' </summary>
    '''
    Public Class eggHTS : Inherits InteropService


        Sub New(App$)
            MyBase._executableAssembly = App$
        End Sub

        ''' <summary>
        ''' ```
        ''' /COG.profiling.plot /in &lt;myvacog.csv> [/size &lt;image_size, default=1800,1200> /out &lt;out.png>]
        ''' ```
        ''' Plots the COGs category statics profiling of the target genome from the COG annotation file.
        ''' </summary>
        '''
        Public Function COGCatalogProfilingPlot(_in As String, Optional _size As String = "1800,1200", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("COGCatalogProfilingPlot")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _size.StringEmpty Then
                Call CLI.Append("/size " & """" & _size & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Data.Add.Mappings /in &lt;data.csv> /bbh &lt;bbh.csv> /ID.mappings &lt;uniprot.ID.mappings.tsv> /uniprot &lt;uniprot.XML> [/ID &lt;fieldName> /out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function AddReMapping(_in As String, _bbh As String, _ID_mappings As String, _uniprot As String, Optional _id As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("AddReMapping")
            Call CLI.Append("/in " & """" & _in & """ ")
            Call CLI.Append("/bbh " & """" & _bbh & """ ")
            Call CLI.Append("/ID.mappings " & """" & _ID_mappings & """ ")
            Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
            If Not _id.StringEmpty Then
                Call CLI.Append("/id " & """" & _id & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Data.Add.ORF /in &lt;data.csv> /uniprot &lt;uniprot.XML> [/ID &lt;fieldName> /out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function DataAddORF(_in As String, _uniprot As String, Optional _id As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("DataAddORF")
            Call CLI.Append("/in " & """" & _in & """ ")
            Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
            If Not _id.StringEmpty Then
                Call CLI.Append("/id " & """" & _id & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Data.Add.uniprotIDs /in &lt;annotations.csv> /data &lt;data.csv> [/out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function DataAddUniprotIDs(_in As String, _data As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("DataAddUniprotIDs")
            Call CLI.Append("/in " & """" & _in & """ ")
            Call CLI.Append("/data " & """" & _data & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /DEP.heatmap /data &lt;Directory> [/non_DEP.blank /level 1.25 /FC.tag &lt;FC.avg> /pvalue &lt;p.value> /out &lt;out.csv>]
        ''' ```
        ''' Generates the heatmap plot input data. The default label profile is using for the iTraq result.
        ''' </summary>
        '''
        Public Function Heatmap(_data As String, Optional _level As String = "", Optional _fc_tag As String = "", Optional _pvalue As String = "", Optional _out As String = "", Optional _non_dep_blank As Boolean = False) As Integer
            Dim CLI As New StringBuilder("Heatmap")
            Call CLI.Append("/data " & """" & _data & """ ")
            If Not _level.StringEmpty Then
                Call CLI.Append("/level " & """" & _level & """ ")
            End If
            If Not _fc_tag.StringEmpty Then
                Call CLI.Append("/fc.tag " & """" & _fc_tag & """ ")
            End If
            If Not _pvalue.StringEmpty Then
                Call CLI.Append("/pvalue " & """" & _pvalue & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If
            If _non_dep_blank Then
                Call CLI.Append("/non_dep.blank ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /DEP.logFC.hist /in &lt;log2test.csv> [/step &lt;0.5> /tag &lt;logFC> /legend.title &lt;Frequency(logFC)> /x.axis "(min,max),tick=0.25" /color &lt;lightblue> /size &lt;1600,1200> /out &lt;out.png>]
        ''' ```
        ''' Using for plots the FC histogram when the experiment have no biological replicates.
        ''' </summary>
        '''
        Public Function logFCHistogram(_in As String, Optional _step As String = "", Optional _tag As String = "", Optional _legend_title As String = "", Optional _x_axis As String = "(min,max),tick=0.25", Optional _color As String = "", Optional _size As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("logFCHistogram")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _step.StringEmpty Then
                Call CLI.Append("/step " & """" & _step & """ ")
            End If
            If Not _tag.StringEmpty Then
                Call CLI.Append("/tag " & """" & _tag & """ ")
            End If
            If Not _legend_title.StringEmpty Then
                Call CLI.Append("/legend.title " & """" & _legend_title & """ ")
            End If
            If Not _x_axis.StringEmpty Then
                Call CLI.Append("/x.axis " & """" & _x_axis & """ ")
            End If
            If Not _color.StringEmpty Then
                Call CLI.Append("/color " & """" & _color & """ ")
            End If
            If Not _size.StringEmpty Then
                Call CLI.Append("/size " & """" & _size & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /DEP.logFC.Volcano /in &lt;DEP.qlfTable.csv> [/size &lt;1920,1440> /out &lt;plot.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function logFCVolcano(_in As String, Optional _size As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("logFCVolcano")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _size.StringEmpty Then
                Call CLI.Append("/size " & """" & _size & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /DEP.uniprot.list /DEP &lt;log2-test.DEP.csv> /sample &lt;sample.csv> [/out &lt;out.txt>]
        ''' ```
        ''' </summary>
        '''
        Public Function DEPUniprotIDlist(_DEP As String, _sample As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("DEPUniprotIDlist")
            Call CLI.Append("/DEP " & """" & _DEP & """ ")
            Call CLI.Append("/sample " & """" & _sample & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /DEP.uniprot.list2 /in &lt;log2.test.csv> [/DEP.Flag &lt;is.DEP?> /uniprot.Flag &lt;uniprot> /species &lt;scientifcName> /uniprot &lt;uniprotXML> /out &lt;out.txt>]
        ''' ```
        ''' </summary>
        '''
        Public Function DEPUniprotIDs2(_in As String, Optional _dep_flag As String = "", Optional _uniprot_flag As String = "", Optional _species As String = "", Optional _uniprot As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("DEPUniprotIDs2")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _dep_flag.StringEmpty Then
                Call CLI.Append("/dep.flag " & """" & _dep_flag & """ ")
            End If
            If Not _uniprot_flag.StringEmpty Then
                Call CLI.Append("/uniprot.flag " & """" & _uniprot_flag & """ ")
            End If
            If Not _species.StringEmpty Then
                Call CLI.Append("/species " & """" & _species & """ ")
            End If
            If Not _uniprot.StringEmpty Then
                Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /DEP.venn /data &lt;Directory> [/level &lt;1.25> /FC.tag &lt;FC.avg> /title &lt;VennDiagram title> /pvalue &lt;p.value> /out &lt;out.DIR>]
        ''' ```
        ''' Generate the VennDiagram plot data and the venn plot tiff. The default parameter profile is using for the iTraq data.
        ''' </summary>
        '''
        Public Function VennData(_data As String, Optional _level As String = "", Optional _fc_tag As String = "", Optional _title As String = "", Optional _pvalue As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("VennData")
            Call CLI.Append("/data " & """" & _data & """ ")
            If Not _level.StringEmpty Then
                Call CLI.Append("/level " & """" & _level & """ ")
            End If
            If Not _fc_tag.StringEmpty Then
                Call CLI.Append("/fc.tag " & """" & _fc_tag & """ ")
            End If
            If Not _title.StringEmpty Then
                Call CLI.Append("/title " & """" & _title & """ ")
            End If
            If Not _pvalue.StringEmpty Then
                Call CLI.Append("/pvalue " & """" & _pvalue & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /enricher.background /in &lt;genbank.gb> [/out &lt;universe.txt>]
        ''' ```
        ''' </summary>
        '''
        Public Function Backgrounds(_in As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("Backgrounds")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /enrichment.go /deg &lt;deg.list> /backgrounds &lt;genome_genes.list> /t2g &lt;term2gene.csv> [/go &lt;go_brief.csv> /out &lt;enricher.result.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function GoEnrichment(_deg As String, _backgrounds As String, _t2g As String, Optional _go As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("GoEnrichment")
            Call CLI.Append("/deg " & """" & _deg & """ ")
            Call CLI.Append("/backgrounds " & """" & _backgrounds & """ ")
            Call CLI.Append("/t2g " & """" & _t2g & """ ")
            If Not _go.StringEmpty Then
                Call CLI.Append("/go " & """" & _go & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


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
        Public Function EnrichmentTermFilter(_in As String, _filter As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("EnrichmentTermFilter")
            Call CLI.Append("/in " & """" & _in & """ ")
            Call CLI.Append("/filter " & """" & _filter & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


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
        Public Function RetriveEnrichmentGeneInfo(_in As String, _proteins As String, Optional _out As String = "", Optional _nocut As Boolean = False, Optional _orf As Boolean = False) As Integer
            Dim CLI As New StringBuilder("RetriveEnrichmentGeneInfo")
            Call CLI.Append("/in " & """" & _in & """ ")
            Call CLI.Append("/proteins " & """" & _proteins & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If
            If _nocut Then
                Call CLI.Append("/nocut ")
            End If
            If _orf Then
                Call CLI.Append("/orf ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Fasta.IDlist /in &lt;prot.fasta> [/out &lt;geneIDs.txt>]
        ''' ```
        ''' </summary>
        '''
        Public Function GetFastaIDlist(_in As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("GetFastaIDlist")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


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
        Public Function GO_cellularLocationPlot(_in As String, Optional _go As String = "", Optional _colors As String = "Paired:c8", Optional _out As String = "", Optional _3d As Boolean = False) As Integer
            Dim CLI As New StringBuilder("GO_cellularLocationPlot")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _go.StringEmpty Then
                Call CLI.Append("/go " & """" & _go & """ ")
            End If
            If Not _colors.StringEmpty Then
                Call CLI.Append("/colors " & """" & _colors & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If
            If _3d Then
                Call CLI.Append("/3d ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Go.enrichment.plot /in &lt;enrichmentTerm.csv> [/bubble /r "log(x,1.5)" /Corrected /displays 10 /PlantRegMap /label.right /gray /pvalue &lt;0.05> /size &lt;2000,1600> /tick 1 /go &lt;go.obo> /out &lt;out.png>]
        ''' ```
        ''' </summary>
        '''
        Public Function GO_enrichmentPlot(_in As String, Optional _r As String = "log(x,1.5)", Optional _displays As String = "", Optional _pvalue As String = "", Optional _size As String = "", Optional _tick As String = "", Optional _go As String = "", Optional _out As String = "", Optional _bubble As Boolean = False, Optional _corrected As Boolean = False, Optional _plantregmap As Boolean = False, Optional _label_right As Boolean = False, Optional _gray As Boolean = False) As Integer
            Dim CLI As New StringBuilder("GO_enrichmentPlot")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _r.StringEmpty Then
                Call CLI.Append("/r " & """" & _r & """ ")
            End If
            If Not _displays.StringEmpty Then
                Call CLI.Append("/displays " & """" & _displays & """ ")
            End If
            If Not _pvalue.StringEmpty Then
                Call CLI.Append("/pvalue " & """" & _pvalue & """ ")
            End If
            If Not _size.StringEmpty Then
                Call CLI.Append("/size " & """" & _size & """ ")
            End If
            If Not _tick.StringEmpty Then
                Call CLI.Append("/tick " & """" & _tick & """ ")
            End If
            If Not _go.StringEmpty Then
                Call CLI.Append("/go " & """" & _go & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If
            If _bubble Then
                Call CLI.Append("/bubble ")
            End If
            If _corrected Then
                Call CLI.Append("/corrected ")
            End If
            If _plantregmap Then
                Call CLI.Append("/plantregmap ")
            End If
            If _label_right Then
                Call CLI.Append("/label.right ")
            End If
            If _gray Then
                Call CLI.Append("/gray ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /iTraq.Reverse.FC /in &lt;data.csv> [/out &lt;Reverse.csv>]
        ''' ```
        ''' Reverse the FC value from the source result.
        ''' </summary>
        '''
        Public Function iTraqInvert(_in As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("iTraqInvert")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /KEGG.Enrichment.PathwayMap /in &lt;kobas.csv> [/out &lt;DIR>]
        ''' ```
        ''' Show the KEGG pathway map image by using KOBAS KEGG pathway enrichment result.
        ''' </summary>
        '''
        Public Function KEGGEnrichmentPathwayMap(_in As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("KEGGEnrichmentPathwayMap")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /KEGG.enrichment.plot /in &lt;enrichmentTerm.csv> [/gray /label.right /pvalue &lt;0.05> /tick 1 /size &lt;2000,1600> /out &lt;out.png>]
        ''' ```
        ''' Bar plots of the KEGG enrichment analysis result.
        ''' </summary>
        '''
        Public Function KEGG_enrichment(_in As String, Optional _pvalue As String = "", Optional _tick As String = "", Optional _size As String = "", Optional _out As String = "", Optional _gray As Boolean = False, Optional _label_right As Boolean = False) As Integer
            Dim CLI As New StringBuilder("KEGG_enrichment")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _pvalue.StringEmpty Then
                Call CLI.Append("/pvalue " & """" & _pvalue & """ ")
            End If
            If Not _tick.StringEmpty Then
                Call CLI.Append("/tick " & """" & _tick & """ ")
            End If
            If Not _size.StringEmpty Then
                Call CLI.Append("/size " & """" & _size & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If
            If _gray Then
                Call CLI.Append("/gray ")
            End If
            If _label_right Then
                Call CLI.Append("/label.right ")
            End If


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
        Public Function KOCatalogs(_in As String, _ko As String, Optional _key As String = "", Optional _mapto As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("KOCatalogs")
            Call CLI.Append("/in " & """" & _in & """ ")
            Call CLI.Append("/ko " & """" & _ko & """ ")
            If Not _key.StringEmpty Then
                Call CLI.Append("/key " & """" & _key & """ ")
            End If
            If Not _mapto.StringEmpty Then
                Call CLI.Append("/mapto " & """" & _mapto & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /KOBAS.add.ORF /in &lt;table.csv> /sample &lt;sample.csv> [/out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function KOBASaddORFsource(_in As String, _sample As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("KOBASaddORFsource")
            Call CLI.Append("/in " & """" & _in & """ ")
            Call CLI.Append("/sample " & """" & _sample & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /KOBAS.split /in &lt;kobas.out_run.txt> [/out &lt;DIR>]
        ''' ```
        ''' </summary>
        '''
        Public Function KOBASSplit(_in As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("KOBASSplit")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


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
        Public Function MergeDEPs(_in As String, Optional _threshold As String = "log(1.5,2)", Optional _raw As String = "", Optional _out As String = "", Optional _log2 As Boolean = False) As Integer
            Dim CLI As New StringBuilder("MergeDEPs")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _threshold.StringEmpty Then
                Call CLI.Append("/threshold " & """" & _threshold & """ ")
            End If
            If Not _raw.StringEmpty Then
                Call CLI.Append("/raw " & """" & _raw & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If
            If _log2 Then
                Call CLI.Append("/log2 ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Perseus.Stat /in &lt;proteinGroups.txt> [/out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function PerseusStatics(_in As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("PerseusStatics")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Perseus.Table /in &lt;proteinGroups.txt> [/out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function PerseusTable(_in As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("PerseusTable")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Perseus.Table.annotations /in &lt;proteinGroups.csv> /uniprot &lt;uniprot.XML> [/scientifcName &lt;""> /out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function PerseusTableAnnotations(_in As String, _uniprot As String, Optional _scientifcname As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("PerseusTableAnnotations")
            Call CLI.Append("/in " & """" & _in & """ ")
            Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
            If Not _scientifcname.StringEmpty Then
                Call CLI.Append("/scientifcname " & """" & _scientifcname & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /plot.pimw /in &lt;samples.csv> [/field.pi &lt;calc. pI> /field.mw &lt;MW [kDa]> /legend.fontsize &lt;20> /legend.size (100,30) /x.axis "(min,max),tick=2" /y.axis "(min,max),n=10" /out &lt;pimw.png> /size &lt;1600,1200> /color &lt;black> /pt.size &lt;8>]
        ''' ```
        ''' 'calc. pI'/'MW [kDa]' scatter plot of the protomics raw sample data.
        ''' </summary>
        '''
        Public Function pimwScatterPlot(_in As String, Optional _field_pi As String = "", Optional _field_mw As String = "", Optional _legend_fontsize As String = "", Optional _legend_size As String = "", Optional _x_axis As String = "(min,max),tick=2", Optional _y_axis As String = "(min,max),n=10", Optional _out As String = "", Optional _size As String = "", Optional _color As String = "", Optional _pt_size As String = "") As Integer
            Dim CLI As New StringBuilder("pimwScatterPlot")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _field_pi.StringEmpty Then
                Call CLI.Append("/field.pi " & """" & _field_pi & """ ")
            End If
            If Not _field_mw.StringEmpty Then
                Call CLI.Append("/field.mw " & """" & _field_mw & """ ")
            End If
            If Not _legend_fontsize.StringEmpty Then
                Call CLI.Append("/legend.fontsize " & """" & _legend_fontsize & """ ")
            End If
            If Not _legend_size.StringEmpty Then
                Call CLI.Append("/legend.size " & """" & _legend_size & """ ")
            End If
            If Not _x_axis.StringEmpty Then
                Call CLI.Append("/x.axis " & """" & _x_axis & """ ")
            End If
            If Not _y_axis.StringEmpty Then
                Call CLI.Append("/y.axis " & """" & _y_axis & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If
            If Not _size.StringEmpty Then
                Call CLI.Append("/size " & """" & _size & """ ")
            End If
            If Not _color.StringEmpty Then
                Call CLI.Append("/color " & """" & _color & """ ")
            End If
            If Not _pt_size.StringEmpty Then
                Call CLI.Append("/pt.size " & """" & _pt_size & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /protein.annotations /uniprot &lt;uniprot.XML> [/iTraq /list &lt;uniprot.id.list.txt> /out &lt;out.csv>]
        ''' ```
        ''' Total proteins functional annotation by using uniprot database.
        ''' </summary>
        '''
        Public Function SampleAnnotations(_uniprot As String, Optional _list As String = "", Optional _out As String = "", Optional _itraq As Boolean = False) As Integer
            Dim CLI As New StringBuilder("SampleAnnotations")
            Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
            If Not _list.StringEmpty Then
                Call CLI.Append("/list " & """" & _list & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If
            If _itraq Then
                Call CLI.Append("/itraq ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /protein.annotations.shotgun /p1 &lt;data.csv> /p2 &lt;data.csv> /uniprot &lt;data.DIR/*.xml,*.tab> [/remapping /out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function SampleAnnotations2(_p1 As String, _p2 As String, _uniprot As String, Optional _out As String = "", Optional _remapping As Boolean = False) As Integer
            Dim CLI As New StringBuilder("SampleAnnotations2")
            Call CLI.Append("/p1 " & """" & _p1 & """ ")
            Call CLI.Append("/p2 " & """" & _p2 & """ ")
            Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If
            If _remapping Then
                Call CLI.Append("/remapping ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /protein.EXPORT /in &lt;uniprot.xml> [/sp &lt;name> /exclude /out &lt;out.fasta>]
        ''' ```
        ''' </summary>
        '''
        Public Function proteinEXPORT(_in As String, Optional _sp As String = "", Optional _out As String = "", Optional _exclude As Boolean = False) As Integer
            Dim CLI As New StringBuilder("proteinEXPORT")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _sp.StringEmpty Then
                Call CLI.Append("/sp " & """" & _sp & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If
            If _exclude Then
                Call CLI.Append("/exclude ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /proteins.Go.plot /in &lt;proteins-uniprot-annotations.csv> [/GO &lt;go.obo> /tick 50 /top 20 /size &lt;2000,4000> /out &lt;out.DIR>]
        ''' ```
        ''' ProteinGroups sample data go profiling plot from the uniprot annotation data.
        ''' </summary>
        '''
        Public Function ProteinsGoPlot(_in As String, Optional _go As String = "", Optional _tick As String = "", Optional _top As String = "", Optional _size As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("ProteinsGoPlot")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _go.StringEmpty Then
                Call CLI.Append("/go " & """" & _go & """ ")
            End If
            If Not _tick.StringEmpty Then
                Call CLI.Append("/tick " & """" & _tick & """ ")
            End If
            If Not _top.StringEmpty Then
                Call CLI.Append("/top " & """" & _top & """ ")
            End If
            If Not _size.StringEmpty Then
                Call CLI.Append("/size " & """" & _size & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /proteins.KEGG.plot /in &lt;proteins-uniprot-annotations.csv> [/size &lt;2000,4000> /tick 20 /out &lt;out.DIR>]
        ''' ```
        ''' </summary>
        '''
        Public Function proteinsKEGGPlot(_in As String, Optional _size As String = "", Optional _tick As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("proteinsKEGGPlot")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _size.StringEmpty Then
                Call CLI.Append("/size " & """" & _size & """ ")
            End If
            If Not _tick.StringEmpty Then
                Call CLI.Append("/tick " & """" & _tick & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Sample.Species.Normalization /bbh &lt;bbh.csv> /uniprot &lt;uniprot.XML/DIR> /idMapping &lt;refSeq2uniprotKB_mappings.tsv> /sample &lt;sample.csv> [/Description &lt;Description.FileName> /ID &lt;columnName> /out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function NormalizeSpecies_samples(_bbh As String, _uniprot As String, _idMapping As String, _sample As String, Optional _description As String = "", Optional _id As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("NormalizeSpecies_samples")
            Call CLI.Append("/bbh " & """" & _bbh & """ ")
            Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
            Call CLI.Append("/idMapping " & """" & _idMapping & """ ")
            Call CLI.Append("/sample " & """" & _sample & """ ")
            If Not _description.StringEmpty Then
                Call CLI.Append("/description " & """" & _description & """ ")
            End If
            If Not _id.StringEmpty Then
                Call CLI.Append("/id " & """" & _id & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Samples.IDlist /in &lt;samples.csv> [/Perseus /shotgun /pair &lt;samples2.csv> /out &lt;out.list.txt>]
        ''' ```
        ''' Extracts the protein hits from the protomics sample data, and using this ID list for downlaods the uniprot annotation data.
        ''' </summary>
        '''
        Public Function GetIDlistFromSampleTable(_in As String, Optional _pair As String = "", Optional _out As String = "", Optional _perseus As Boolean = False, Optional _shotgun As Boolean = False) As Integer
            Dim CLI As New StringBuilder("GetIDlistFromSampleTable")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _pair.StringEmpty Then
                Call CLI.Append("/pair " & """" & _pair & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If
            If _perseus Then
                Call CLI.Append("/perseus ")
            End If
            If _shotgun Then
                Call CLI.Append("/shotgun ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Shotgun.Data.Strip /in &lt;data.csv> [/out &lt;output.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function StripShotgunData(_in As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("StripShotgunData")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Species.Normalization /bbh &lt;bbh.csv> /uniprot &lt;uniprot.XML> /idMapping &lt;refSeq2uniprotKB_mappings.tsv> /annotations &lt;annotations.csv> [/out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function NormalizeSpecies(_bbh As String, _uniprot As String, _idMapping As String, _annotations As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("NormalizeSpecies")
            Call CLI.Append("/bbh " & """" & _bbh & """ ")
            Call CLI.Append("/uniprot " & """" & _uniprot & """ ")
            Call CLI.Append("/idMapping " & """" & _idMapping & """ ")
            Call CLI.Append("/annotations " & """" & _annotations & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Term2genes /in &lt;uniprot.XML> [/term &lt;GO> /id &lt;ORF> /out &lt;out.tsv>]
        ''' ```
        ''' </summary>
        '''
        Public Function Term2Genes(_in As String, Optional _term As String = "", Optional _id As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("Term2Genes")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _term.StringEmpty Then
                Call CLI.Append("/term " & """" & _term & """ ")
            End If
            If Not _id.StringEmpty Then
                Call CLI.Append("/id " & """" & _id & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


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
        Public Function UniprotMappings(_in As String, Optional _type As String = "", Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("UniprotMappings")
            Call CLI.Append("/in " & """" & _in & """ ")
            If Not _type.StringEmpty Then
                Call CLI.Append("/type " & """" & _type & """ ")
            End If
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function

        ''' <summary>
        ''' ```
        ''' /Venn.Functions /venn &lt;venn.csv> /anno &lt;annotations.csv> [/out &lt;out.csv>]
        ''' ```
        ''' </summary>
        '''
        Public Function VennFunctions(_venn As String, _anno As String, Optional _out As String = "") As Integer
            Dim CLI As New StringBuilder("VennFunctions")
            Call CLI.Append("/venn " & """" & _venn & """ ")
            Call CLI.Append("/anno " & """" & _anno & """ ")
            If Not _out.StringEmpty Then
                Call CLI.Append("/out " & """" & _out & """ ")
            End If


            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI.ToString())
            Return proc.Run()
        End Function
    End Class
End Namespace
