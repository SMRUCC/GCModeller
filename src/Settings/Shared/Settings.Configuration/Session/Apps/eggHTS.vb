Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: G:/GCModeller/GCModeller/bin/eggHTS.exe

Namespace GCModellerApps


    ''' <summary>
    '''eggHTS.CLI
    ''' </summary>
    '''
    Public Class eggHTS : Inherits InteropService


        Sub New(App$)
            MyBase._executableAssembly = App$
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function COG_profiling_plot(_in As String, Optional _size As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/COG.profiling.plot /in ""{_in}"" /size ""{_size}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Data_Add_Mappings(_in As String, _bbh As String, _ID_mappings As String, _uniprot As String, Optional _id As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/Data.Add.Mappings /in ""{_in}"" /bbh ""{_bbh}"" /ID.mappings ""{_ID_mappings}"" /uniprot ""{_uniprot}"" /id ""{_id}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Data_Add_ORF(_in As String, _uniprot As String, Optional _id As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/Data.Add.ORF /in ""{_in}"" /uniprot ""{_uniprot}"" /id ""{_id}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Data_Add_uniprotIDs(_in As String, _data As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Data.Add.uniprotIDs /in ""{_in}"" /data ""{_data}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Generates the heatmap plot input data. The default label profile is using for the iTraq result.
        ''' </summary>
        '''
        Public Function DEP_heatmap(_data As String, Optional _level As String = "", Optional _fc_tag As String = "", Optional _pvalue As String = "", Optional _out As String = "", Optional _non_dep_blank As Boolean = False) As Integer
            Dim CLI$ = $"/DEP.heatmap /data ""{_data}"" /level ""{_level}"" /fc.tag ""{_fc_tag}"" /pvalue ""{_pvalue}"" /out ""{_out}"" {If(_non_dep_blank, "/non_dep.blank", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Using for plots the FC histogram when the experiment have no biological replicates.
        ''' </summary>
        '''
        Public Function DEP_logFC_hist(_in As String, Optional _step As String = "", Optional _tag As String = "", Optional _legend_title As String = "", Optional _x_axis As String = "", Optional _color As String = "", Optional _size As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/DEP.logFC.hist /in ""{_in}"" /step ""{_step}"" /tag ""{_tag}"" /legend.title ""{_legend_title}"" /x.axis ""{_x_axis}"" /color ""{_color}"" /size ""{_size}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function DEP_logFC_Volcano(_in As String, Optional _size As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/DEP.logFC.Volcano /in ""{_in}"" /size ""{_size}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function DEP_uniprot_list(_DEP As String, _sample As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/DEP.uniprot.list /DEP ""{_DEP}"" /sample ""{_sample}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function DEP_uniprot_list2(_in As String, Optional _dep_flag As String = "", Optional _uniprot_flag As String = "", Optional _species As String = "", Optional _uniprot As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/DEP.uniprot.list2 /in ""{_in}"" /dep.flag ""{_dep_flag}"" /uniprot.flag ""{_uniprot_flag}"" /species ""{_species}"" /uniprot ""{_uniprot}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Generate the VennDiagram plot data and the venn plot tiff. The default parameter profile is using for the iTraq data.
        ''' </summary>
        '''
        Public Function DEP_venn(_data As String, Optional _level As String = "", Optional _fc_tag As String = "", Optional _title As String = "", Optional _pvalue As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/DEP.venn /data ""{_data}"" /level ""{_level}"" /fc.tag ""{_fc_tag}"" /title ""{_title}"" /pvalue ""{_pvalue}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function enricher_background(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/enricher.background /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function enrichment_go(_deg As String, _backgrounds As String, _t2g As String, Optional _go As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/enrichment.go /deg ""{_deg}"" /backgrounds ""{_backgrounds}"" /t2g ""{_t2g}"" /go ""{_go}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Filter the specific term result from the analysis output by using pattern keyword
        ''' </summary>
        '''
        Public Function Enrichment_Term_Filter(_in As String, _filter As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Enrichment.Term.Filter /in ""{_in}"" /filter ""{_filter}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Retrive KEGG/GO info for the genes in the enrichment result.
        ''' </summary>
        '''
        Public Function Enrichments_ORF_info(_in As String, _proteins As String, Optional _out As String = "", Optional _nocut As Boolean = False, Optional _orf As Boolean = False) As Integer
            Dim CLI$ = $"/Enrichments.ORF.info /in ""{_in}"" /proteins ""{_proteins}"" /out ""{_out}"" {If(_nocut, "/nocut", "")} {If(_orf, "/orf", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Fasta_IDlist(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Fasta.IDlist /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Visualize of the subcellular location result from the GO enrichment analysis.
        ''' </summary>
        '''
        Public Function GO_cellular_location_Plot(_in As String, Optional _go As String = "", Optional _colors As String = "Paired:c8", Optional _out As String = "", Optional _3d As Boolean = False) As Integer
            Dim CLI$ = $"/GO.cellular_location.Plot /in ""{_in}"" /go ""{_go}"" /colors ""{_colors}"" /out ""{_out}"" {If(_3d, "/3d", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Go_enrichment_plot(_in As String, Optional _r As String = "", Optional _displays As String = "", Optional _pvalue As String = "", Optional _size As String = "", Optional _tick As String = "", Optional _go As String = "", Optional _out As String = "", Optional _bubble As Boolean = False, Optional _corrected As Boolean = False, Optional _plantregmap As Boolean = False, Optional _label_right As Boolean = False, Optional _gray As Boolean = False) As Integer
            Dim CLI$ = $"/Go.enrichment.plot /in ""{_in}"" /r ""{_r}"" /displays ""{_displays}"" /pvalue ""{_pvalue}"" /size ""{_size}"" /tick ""{_tick}"" /go ""{_go}"" /out ""{_out}"" {If(_bubble, "/bubble", "")} {If(_corrected, "/corrected", "")} {If(_plantregmap, "/plantregmap", "")} {If(_label_right, "/label.right", "")} {If(_gray, "/gray", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Reverse the FC value from the source result.
        ''' </summary>
        '''
        Public Function iTraq_Reverse_FC(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/iTraq.Reverse.FC /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Show the KEGG pathway map image by using KOBAS KEGG pathway enrichment result.
        ''' </summary>
        '''
        Public Function KEGG_Enrichment_PathwayMap(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/KEGG.Enrichment.PathwayMap /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Bar plots of the KEGG enrichment analysis result.
        ''' </summary>
        '''
        Public Function KEGG_enrichment_plot(_in As String, Optional _pvalue As String = "", Optional _tick As String = "", Optional _size As String = "", Optional _out As String = "", Optional _gray As Boolean = False, Optional _label_right As Boolean = False) As Integer
            Dim CLI$ = $"/KEGG.enrichment.plot /in ""{_in}"" /pvalue ""{_pvalue}"" /tick ""{_tick}"" /size ""{_size}"" /out ""{_out}"" {If(_gray, "/gray", "")} {If(_label_right, "/label.right", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Display the barplot of the KEGG orthology match.
        ''' </summary>
        '''
        Public Function KO_Catalogs(_in As String, _ko As String, Optional _key As String = "", Optional _mapto As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/KO.Catalogs /in ""{_in}"" /ko ""{_ko}"" /key ""{_key}"" /mapto ""{_mapto}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function KOBAS_add_ORF(_in As String, _sample As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/KOBAS.add.ORF /in ""{_in}"" /sample ""{_sample}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function KOBAS_split(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/KOBAS.split /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Usually using for generates the heatmap plot matrix of the DEPs. This function call will generates two dataset, one is using for the heatmap plot and another is using for the venn diagram plot.
        ''' </summary>
        '''
        Public Function Merge_DEPs(_in As String, Optional _threshold As String = "", Optional _raw As String = "", Optional _out As String = "", Optional _log2 As Boolean = False) As Integer
            Dim CLI$ = $"/Merge.DEPs /in ""{_in}"" /threshold ""{_threshold}"" /raw ""{_raw}"" /out ""{_out}"" {If(_log2, "/log2", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Perseus_Stat(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Perseus.Stat /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Perseus_Table(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Perseus.Table /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Perseus_Table_annotations(_in As String, _uniprot As String, Optional _scientifcname As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/Perseus.Table.annotations /in ""{_in}"" /uniprot ""{_uniprot}"" /scientifcname ""{_scientifcname}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        ''''calc. pI'/'MW [kDa]' scatter plot of the protomics raw sample data.
        ''' </summary>
        '''
        Public Function plot_pimw(_in As String, Optional _field_pi As String = "", Optional _field_mw As String = "", Optional _legend_fontsize As String = "", Optional _legend_size As String = "", Optional _x_axis As String = "", Optional _y_axis As String = "", Optional _out As String = "", Optional _size As String = "", Optional _color As String = "", Optional _pt_size As String = "") As Integer
            Dim CLI$ = $"/plot.pimw /in ""{_in}"" /field.pi ""{_field_pi}"" /field.mw ""{_field_mw}"" /legend.fontsize ""{_legend_fontsize}"" /legend.size ""{_legend_size}"" /x.axis ""{_x_axis}"" /y.axis ""{_y_axis}"" /out ""{_out}"" /size ""{_size}"" /color ""{_color}"" /pt.size ""{_pt_size}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Total proteins functional annotation by using uniprot database.
        ''' </summary>
        '''
        Public Function protein_annotations(_uniprot As String, Optional _list As String = "", Optional _out As String = "", Optional _itraq As Boolean = False) As Integer
            Dim CLI$ = $"/protein.annotations /uniprot ""{_uniprot}"" /list ""{_list}"" /out ""{_out}"" {If(_itraq, "/itraq", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function protein_annotations_shotgun(_p1 As String, _p2 As String, _uniprot As String, Optional _out As String = "", Optional _remapping As Boolean = False) As Integer
            Dim CLI$ = $"/protein.annotations.shotgun /p1 ""{_p1}"" /p2 ""{_p2}"" /uniprot ""{_uniprot}"" /out ""{_out}"" {If(_remapping, "/remapping", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function protein_EXPORT(_in As String, Optional _sp As String = "", Optional _out As String = "", Optional _exclude As Boolean = False) As Integer
            Dim CLI$ = $"/protein.EXPORT /in ""{_in}"" /sp ""{_sp}"" /out ""{_out}"" {If(_exclude, "/exclude", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''ProteinGroups sample data go profiling plot from the uniprot annotation data.
        ''' </summary>
        '''
        Public Function proteins_Go_plot(_in As String, Optional _go As String = "", Optional _tick As String = "", Optional _top As String = "", Optional _size As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/proteins.Go.plot /in ""{_in}"" /go ""{_go}"" /tick ""{_tick}"" /top ""{_top}"" /size ""{_size}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function proteins_KEGG_plot(_in As String, Optional _size As String = "", Optional _tick As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/proteins.KEGG.plot /in ""{_in}"" /size ""{_size}"" /tick ""{_tick}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Sample_Species_Normalization(_bbh As String, _uniprot As String, _idMapping As String, _sample As String, Optional _description As String = "", Optional _id As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/Sample.Species.Normalization /bbh ""{_bbh}"" /uniprot ""{_uniprot}"" /idMapping ""{_idMapping}"" /sample ""{_sample}"" /description ""{_description}"" /id ""{_id}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Extracts the protein hits from the protomics sample data, and using this ID list for downlaods the uniprot annotation data.
        ''' </summary>
        '''
        Public Function Samples_IDlist(_in As String, Optional _pair As String = "", Optional _out As String = "", Optional _perseus As Boolean = False, Optional _shotgun As Boolean = False) As Integer
            Dim CLI$ = $"/Samples.IDlist /in ""{_in}"" /pair ""{_pair}"" /out ""{_out}"" {If(_perseus, "/perseus", "")} {If(_shotgun, "/shotgun", "")}"
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Shotgun_Data_Strip(_in As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Shotgun.Data.Strip /in ""{_in}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Species_Normalization(_bbh As String, _uniprot As String, _idMapping As String, _annotations As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Species.Normalization /bbh ""{_bbh}"" /uniprot ""{_uniprot}"" /idMapping ""{_idMapping}"" /annotations ""{_annotations}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Term2genes(_in As String, Optional _term As String = "", Optional _id As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/Term2genes /in ""{_in}"" /term ""{_term}"" /id ""{_id}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''Retrieve the uniprot annotation data by using ID mapping operations.
        ''' </summary>
        '''
        Public Function Uniprot_Mappings(_in As String, Optional _type As String = "", Optional _out As String = "") As Integer
            Dim CLI$ = $"/Uniprot.Mappings /in ""{_in}"" /type ""{_type}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function

        ''' <summary>
        '''
        ''' </summary>
        '''
        Public Function Venn_Functions(_venn As String, _anno As String, Optional _out As String = "") As Integer
            Dim CLI$ = $"/Venn.Functions /venn ""{_venn}"" /anno ""{_anno}"" /out ""{_out}"""
            Dim proc As IIORedirectAbstract = RunDotNetApp(CLI$)
            Return proc.Run()
        End Function
    End Class
End Namespace
