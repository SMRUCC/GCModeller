// export R# package module type define for javascript/typescript language
//
//    imports "background" from "gseakit";
//
// ref=gseakit.GSEABackground@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * tools for handling GSEA background model.
 * 
*/
declare namespace background {
   module append {
      /**
       * Append id terms to a given gsea background
       * 
       * 
        * @param background the GSEA background model object, this model will be modified in place.
        * @param term_name the term name that will be written into the ``name`` field of the :class:``BackgroundGene.term_id``.
        * @param terms a named list of the term id vector, the name of each slot is the gene id and the
        *  value is the term id vector of that gene.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return the given background model with the new term id information appended.
      */
      function id_terms(background: object, term_name: string, terms: object, env?: object): any;
   }
   module as {
      /**
       * cast the cluster data as the enrichment background
       * 
       * 
        * @param clusters a data cluster or a collection of kegg pathway model, value could be a collection of [Cluster](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Cluster) object, parsed [Gmt](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Formats.Gmt), parsed [Gmx](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Formats.Gmx) file data.
        * @param background_size default value -1 or zero means auto evaluated
        * 
        * + default value Is ``-1``.
        * @param name the background model name
        * 
        * + default value Is ``'n/a'``.
        * @param tax_id ncbi taxonomy id of the target organism
        * 
        * + default value Is ``'n/a'``.
        * @param desc the model description
        * 
        * + default value Is ``'n/a'``.
        * @param omics Create a enrichment background model for run multiple omics data analysis?
        *  this parameter is only works for the kegg pathway model where you are 
        *  speicifc via the **clusters** parameter.
        * 
        * + default value Is ``null``.
        * @param filter_compoundId do compound id filtering when target model is **omics**?
        *  (all of the KEGG drug id and KEGG glycan id will be removed from the cluster model)
        * 
        * + default value Is ``true``.
        * @param kegg_code the kegg organism code when the given **clusters** collection is
        *  a collection of the pathway object.
        * 
        * + default value Is ``null``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model that is created from the given cluster data.
      */
      function background(clusters: any, background_size?: object, name?: string, tax_id?: string, desc?: string, omics?: object, filter_compoundId?: boolean, kegg_code?: string, env?: object): object;
      /**
       * Extract the gene set list from the background model
       * 
       * > the result list could be used for save as json file for 
       * >  parsed in R by ``jsonlite::fromJSON`` function, and used
       * >  for the gsva analysis.
       * 
        * @param background the GSEA background model object.
        * @param export_alias exports the gene alias ids instead of the gene accession ids? Default ``FALSE``.
        * 
        * + default value Is ``false``.
        * @return a tuple list object that contains the gene set information,
        *  data result in format like:
        *  
        * ```r
        *  list(
        *      "cluster id 1" = c("gene id", "gene id", ...),
        *      "cluster id 2" = c("gene id", "gene id", ...),
        *      ...
        *  )
        *  ```
      */
      function geneSet(background: object, export_alias?: boolean): object;
   }
   module background {
      /**
       * do id mapping of the members in the background cluster
       * 
       * 
        * @param background the GSEA background model object.
        * @param mapping do id translation via this id source list
        * @param subset only the cluster which has the member gene id exists in this
        *  collection then the cluster will be keeps from the result
        *  background if this parameter is not null or empty.
        * 
        * + default value Is ``null``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a new [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model with the translated member gene ids.
      */
      function id_mapping(background: object, mapping: object, subset?: string, env?: object): object;
   }
   /**
    * summary of the background model as dataframe
    * 
    * 
     * @param background the GSEA background model object.
     * @param gene_names writes the gene names in the ``factors`` column when ``TRUE`` (default), otherwise
     *  writes the gene accession ids.
     * 
     * + default value Is ``true``.
     * @return a ``dataframe`` of the cluster information, the row names are the cluster ids and
     *  the columns are ``class``, ``category``, ``name``, ``description``,
     *  ``cluster_size`` and ``factors`` (the member gene ids joined by ``;``).
   */
   function background_summary(background: object, gene_names?: boolean): object;
   /**
    * Remove the clusters from the given background model by the cluster id.
    * 
    * 
     * @param background the GSEA background model object.
     * @param id_removes a character vector of the cluster ids that will be removed from the background model.
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return a new [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model that does not contains the removed clusters,
     *  the ``size`` of the result model is re-evaluated from the remaining clusters.
   */
   function cluster_filter(background: object, id_removes: any, env?: object): any;
   /**
    * Get the cluster name of each gene id from the given background model.
    * 
    * > This function returns only the first matched cluster name when a gene is annotated
    * >  in multiple clusters, use ``geneSet.intersects`` with ``get_clusterID=TRUE`` for
    * >  getting all of the cluster ids of the gene.
    * 
     * @param background the GSEA background model object.
     * @param idset a character vector of the gene ids, the first cluster that contains the target
     *  gene will be used for query the cluster name.
     * @return a character vector of the cluster name of each input gene id, a ``NULL`` value will be
     *  returned for the gene id that is not annotated in any cluster.
   */
   function cluster_names(background: object, idset: any): string;
   /**
    * get all of the cluster id set from the given background model object
    * 
    * 
     * @param background the GSEA background model object.
     * @return A character vector of the cluster id(or pathway id) that defined inside the given background model
   */
   function clusterIDs(background: object): string;
   /**
    * Get the cluster information data table of a specific cluster.
    * 
    * 
     * @param background the GSEA background model object.
     * @param clusterId the cluster id (or the pathway id) that will be query from the background model.
     * @return a ``dataframe`` of the cluster member genes, the row names are the gene ids and
     *  the columns are ``clusterId``, ``clusterName``, ``name``, ``locus_tag``,
     *  ``description`` and ``terms``. An empty ``dataframe`` will be returned when the
     *  cluster id is not found.
   */
   function clusterInfo(background: object, clusterId: string): object;
   /**
    * get kegg compound class brite background model
    * 
    * > this function generates a background model from the internal
    * >  kegg resource database, the generated background model which 
    * >  could be used for the compound class category annotation.
    * 
     * @return a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model of the KEGG compound class (brite) categories.
   */
   function compoundBrite(): object;
   /**
    * make gsea background dynamic cut
    * 
    * 
     * @param background the GSEA background model object.
     * @param annotated a character vector of the annotated gene ids, only the cluster members that are
     *  annotated in this gene id set will be kept.
     * @return a new [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model that only contains the annotated gene ids;
     *  returns ``Nothing`` when the given gene id set is empty.
   */
   function cut_background(background: object, annotated: any): object;
   module dag {
      /**
       * create gsea background from a given obo ontology file data.
       * 
       * 
        * @param dag the GO obo database object that is used for creating the background model.
        * @param flat Flat the ontology tree into cluster via the ``is_a`` relationship?
        *  
        *  default false, required of the ``enrichment.go`` function for run enrichment analysis
        *  value true, will flat the ontology tree into cluster, then the enrichment analysis could be
        *  applied via the ``enrichment`` function.
        * 
        * + default value Is ``false``.
        * @param verbose_progress prints the progress message? Default ``TRUE``.
        * 
        * + default value Is ``true``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model of the GO terms.
      */
      function background(dag: object, flat?: boolean, verbose_progress?: boolean, env?: object): object;
   }
   /**
    * cast the cluster data as the enrichment background
    * 
    * 
     * @param geneSet a collection of the gene feature clusters, usualy be a tuple list in format of:
     *  cluster id as the list name and the corresponding tuple list value should be
     *  the gene id character vector.
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model that is created from the given list;
     *  returns ``Nothing`` when the given list is empty.
   */
   function fromList(geneSet: object, env?: object): object;
   module geneSet {
      /**
       * Make the gene set annotation via a given GSEA background model.
       * 
       * 
        * @param background the GSEA background model object.
        * @param geneSet the gene id set that will be annotated, could be a character vector of the gene ids
        *  or a ``dataframe`` object whose row names are the gene ids. When a ``dataframe`` is
        *  given the annotation columns will be appended to it directly.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a ``dataframe`` with the gene id as the row names and the columns of the cluster
        *  information (``cluster``, ``clusterName``, ``category``, ``class`` and the
        *  annotation terms) of each gene.
      */
      function annotations(background: object, geneSet: any, env?: object): any;
      /**
       * Remove the clusters from the given background model by the cluster id.
       * 
       * 
        * @param background the GSEA background model object.
        * @param geneSet 
        * + default value Is ``null``.
        * @param min_size 
        * + default value Is ``3``.
        * @param max_intersects 
        * + default value Is ``500``.
        * @param remove_clusters 
        * + default value Is ``null``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a new [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model that does not contains the removed clusters,
        *  the ``size`` of the result model is re-evaluated from the remaining clusters.
      */
      function filter(background: object, geneSet?: any, min_size?: object, max_intersects?: object, remove_clusters?: any, env?: object): object;
      /**
       * get an intersection id list between the background
       *  model and the given gene id list.
       * 
       * 
        * @param cluster A gene cluster model or gsea background model object.
        * @param geneSet a character vector of the gene ids for the intersection query.
        * @param isLocusTag matches the gene id with the locus tag instead of the accession id? Default ``FALSE``.
        * 
        * + default value Is ``false``.
        * @param get_clusterID this function will returns a set of mapping cluster id if this 
        *  parameter value is set to value TRUE, otherwise this function
        *  returns a set of intersected geneID list by default.
        *  
        *  this parameter only works when the cluster object is a 
        *  gsea background model object.
        * 
        * + default value Is ``false``.
        * @param term_map returns a ``dataframe`` of the gene id and the annotation term of each gene instead of
        *  the intersected id set when this parameter is set to ``TRUE``. Default ``FALSE``.
        * 
        * + default value Is ``false``.
        * @param id_map keeps the original gene id in the ``term`` column of the result dataframe when the gene
        *  has no annotation term? Default ``FALSE`` means the empty value will be used.
        * 
        * + default value Is ``false``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a character vector of the intersected gene id set or the cluster id set based on the option of parameter **get_clusterID**.
      */
      function intersects(cluster: any, geneSet: string, isLocusTag?: boolean, get_clusterID?: boolean, term_map?: boolean, id_map?: boolean, env?: object): string;
   }
   /**
    * Create a cluster for gsea background
    * 
    * > the input dataframe could be a set of database xrefs, example as:
    * >  
    * >  |xref|name|alias|KEGG|uniprot|
    * >  |----|----|-----|----|-------|
    * >  |    |    |     |    |       |
    * 
     * @param x id, name data fields should be exists in current dataframe object, 
     *  other data fields will be used as the gene member terms
     * @param clusterId id of the cluster
     * @param clusterName display name of the cluster model
     * @param desc the description of the cluster model
     * 
     * + default value Is ``'n/a'``.
     * @param id the field column name for get gene members id
     * 
     * + default value Is ``'xref'``.
     * @param name the field column name for get gene members name
     * 
     * + default value Is ``'name'``.
     * @return a [Cluster](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Cluster) object that is created from the given dataframe.
   */
   function gsea_cluster(x: object, clusterId: string, clusterName: string, desc?: string, id?: string, name?: string): object;
   module KO {
      /**
       * create kegg background model
       * 
       * 
        * @param genes a set of molecules with the kegg orthology/compound id mapping
        * @param maps the kegg maps model
        * @param size the background gene size, default -1 or zero means auto calculation.
        * 
        * + default value Is ``-1``.
        * @param genomeName the genome name that tagged with this background model.
        * 
        * + default value Is ``'unknown'``.
        * @param id_map an optional id mapping data for the gene id, the ko id and the gene name, it could be
        *  a ``list`` object or a ``dataframe`` object. Default ``NULL`` means no extra id mapping.
        * 
        * + default value Is ``null``.
        * @param multiple_omics creates the background model for the multiple omics data analysis? Default ``FALSE``.
        * 
        * + default value Is ``false``.
        * @param term_db the database label of the term that will be written into the result model. Default ``unknown``.
        * 
        * + default value Is ``'unknown'``.
        * @param instance_map the compound instance map that is used for the metabolism enrichment analysis. Default ``NULL``.
        * 
        * + default value Is ``null``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model that is created from the given KO id mapping.
      */
      function background(genes: any, maps: any, size?: object, genomeName?: string, id_map?: any, multiple_omics?: boolean, term_db?: string, instance_map?: object, env?: object): object;
      /**
       * Convert the KO annotation of the background model into a node table.
       * 
       * 
        * @param background the GSEA background model object.
        * @return an array of the [EntityObject](cref:T:Microsoft.VisualBasic.Data.Framework.IO.EntityObject) objects, the ``ID`` of each object is the
        *  gene accession id and the ``KO`` property is the KO term of that gene.
      */
      function table(background: object): object;
   }
   /**
    * gene/protein KO id background
    * 
    * 
     * @return A reference background of the kegg pathway by parse the internal resource file.
   */
   function KO_reference(): object;
   /**
    * Create a GSEA background model from the KEGG pathway graph query result of the
    *  enrichment analysis result terms.
    * 
    * 
     * @param enrich the enrichment analysis result set, the term id of each result object will be used
     *  as the pathway id for query the pathway graph from the **graphQuery**.
     * @param graphQuery the KEGG pathway graph query object.
     * @return a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model that is created from the pathway graph data.
   */
   function meta_background(enrich: object, graphQuery: object): object;
   module metabolism {
      /**
       * create kegg maps background for the metabolism data analysis
       * 
       * 
        * @param kegg Should be a collection of the kegg map object
        * @param filter a character vector of the map id that will be kept in the background model,
        *  default ``NULL`` means all of the given maps will be used.
        * 
        * + default value Is ``null``.
        * @param env the R# runtime environment.
        * 
        * + default value Is ``null``.
        * @return a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model that is created from the given KEGG maps.
      */
      function background(kegg: any, filter?: string, env?: object): object;
   }
   /**
    * Create the gsea background model for metabolism analysis
    * 
    * 
     * @param kegg the kegg [Pathway](cref:T:SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway) model collection of current organism or 
     *  the KEGG [Map](cref:T:SMRUCC.genomics.Assembly.KEGG.WebServices.XML.Map) data collection.
     *  andalso could be a tuple list of the idset.
     * @param reactions A collection of the reference [ReactionTable](cref:T:SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork.ReactionTable) model 
     *  data for build the metabolism network
     * @param org_name the KEGG organism code (or the organism name) of the target model, default ``NULL``.
     * 
     * + default value Is ``null``.
     * @param is_ko_ref treats the given KEGG pathway model as the KO reference model? Default ``FALSE`` means
     *  the pathway model of a specific organism.
     * 
     * + default value Is ``false``.
     * @param multipleOmics creates the background model for the multiple omics data analysis? Default ``FALSE``.
     * 
     * + default value Is ``false``.
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return a [metpa](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.KnowledgeBase.Metabolism.Metpa.metpa) background model for the metabolism enrichment analysis.
   */
   function metpa(kegg: any, reactions: any, org_name?: string, is_ko_ref?: boolean, multipleOmics?: boolean, env?: object): object;
   /**
    * get all of the molecule id set from the given background model object
    * 
    * 
     * @param background the GSEA background model object.
     * @return A character vector of the gene id that defined inside the given background model
   */
   function moleculeIDs(background: object): string;
   module read {
      /**
       * Load GSEA background model from a xml file.
       * 
       * 
        * @param file the xml file path of the GSEA background model.
        * @return a [Background](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Background) model object.
      */
      function background(file: string): object;
   }
   /**
    * Read the gene set matrix (``gmt``) file.
    * 
    * > Each line of the ``gmt`` file is a gene set in the format of
    * >  ``set_id \t description \t gene1 \t gene2 ...``.
    * 
     * @param file the ``gmt`` file path.
     * @return the parsed [Gmt](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Formats.Gmt) gene set matrix object.
   */
   function read_gmt(file: string): object;
   /**
    * Read the gene set matrix (``gmx``) file.
    * 
    * > The ``gmx`` format is the transposed version of the ``gmt`` format: the first
    * >  column of each row is the gene id and the rest columns are the member gene set ids.
    * 
     * @param file the ``gmx`` file path.
     * @return the parsed [Gmx](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.Formats.Gmx) gene set matrix object.
   */
   function read_gmx(file: string): object;
   module write {
      /**
       * Save GSEA background model as xml file
       * 
       * 
        * @param background the GSEA background model object that will be saved.
        * @param file the output xml file path.
        * @return this function returns ``TRUE`` when the file is saved successfully.
      */
      function background(background: object, file: string): boolean;
   }
}
