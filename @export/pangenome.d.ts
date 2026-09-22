// export R# package module type define for javascript/typescript language
//
//    imports "pangenome" from "comparative_toolkit";
//
// ref=comparative_toolkit.pangenome@comparative_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * pan-genome analysis toolkit
 * 
*/
declare namespace pangenome {
   /**
    * run pan-genome analysis
    * 
    * 
     * @param pangenome context data for run pan-genome analysis
     * @param orthologSet gene ortholog data table, usually be the bi-direction best hit of the blast result
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function analysis(pangenome: object, orthologSet: object, env?: object): object;
   /**
    * Load the pangenome analysis context
    * 
    * 
     * @param genomes should be a collection of the genome GFF3 feature tables
     * @param soft_core_threshold threshold value for identify the gene as soft core, thres value 1 means core genes
     * 
     * + default value Is ``0.95``.
     * @param uniqueByAcc 
     * + default value Is ``false``.
     * @param genome_size 
     * + default value Is ``null``.
     * @param sv_cluster_count 
     * + default value Is ``4``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function build_context(genomes: any, soft_core_threshold?: number, uniqueByAcc?: boolean, genome_size?: any, sv_cluster_count?: object, env?: object): object;
   /**
    * export the gene family distribution percent matrix
    *  (rows are genomes plus one average row, columns are the four family categories)
    * 
    * 
     * @param result -
     * @param by the statistics caliber: ``gene`` for the gene(copy number) based percent (default),
     *  or ``family`` for the family count based percent.
     * 
     * + default value Is ``'gene'``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function category_percent_matrix(result: object, by?: string, env?: object): any;
   /**
   */
   function curve_data(result: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function family_groups(cdhit: any, env?: object): any;
   /**
   */
   function genetic_distance(result: object): any;
   /**
     * @param sep default value Is ``'.'``.
     * @param env default value Is ``null``.
   */
   function multiple_genome_alignment(aligns: any, sep?: string, env?: object): any;
   /**
   */
   function pav_matrix(result: object): any;
   /**
     * @param index default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function pav_table(result: object, index?: object, env?: object): object;
   /**
    * generates the html report text for the pan-genome analysis result
    * 
    * 
     * @param result -
   */
   function report_html(result: object): string;
   /**
    * 
    * 
     * @param result -
     * @return a tuple list that contains three elements:
     *  
     *  1. ``stats``: [GenomeStatRow](cref:T:SMRUCC.genomics.Analysis.PanGenome.ReportJSON.GenomeStatRow)
     *  2. ``pca``: [PCAScatterDataset](cref:T:SMRUCC.genomics.Analysis.PanGenome.ReportJSON.PCAScatterDataset)
     *  3. ``entropy``: [GenomeEntropyDataset](cref:T:SMRUCC.genomics.Analysis.PanGenome.ReportJSON.GenomeEntropyDataset)
   */
   function scatter_set(result: object): object;
   /**
    * set orthology group for make gene family
    * 
    * 
     * @param x gene ortholog annotation result set
     * @param uf -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function set_ortho_group(x: any, uf: object, env?: object): any;
   /**
    * set species name to the genome gff3 context model
    * 
    * 
     * @param genome -
     * @param source_name -
   */
   function source_id(genome: object, source_name: string): object;
   /**
    * export the SV CopyNumber matrix of the structural variations
    *  (rows are gene families which have at least one SV event, columns are genomes)
    * 
    * 
     * @param result -
   */
   function sv_copy_number_matrix(result: object): any;
   /**
    * get the SV structural variation information entropy scatter data and the kmeans clustering result
    * 
    * 
     * @param result -
     * @param cluster_count the k value of the kmeans clustering, default is 4
     * 
     * + default value Is ``4``.
   */
   function sv_entropy(result: object, cluster_count?: object): object;
   /**
    * export the SV Median matrix of the structural variations
    *  (rows are gene families which have at least one SV event, columns are genomes)
    * 
    * 
     * @param result -
   */
   function sv_median_matrix(result: object): any;
   /**
    * export structure variant result table
    * 
    * 
     * @param result -
     * @param index -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function sv_table(result: object, index?: object, env?: object): object;
}
