// export R# source type define for javascript/typescript language
//
// package_source=GCModeller

declare namespace GCModeller {
   module _ {
      /**
      */
      function cast_CLR_metpa(metpa: any): object;
      /**
      */
      function onLoad(): object;
      /**
        * @param entryName default value Is ``null``.
      */
      function readZipStream(zipfile: any, entryName?: any): object;
      /**
      */
      function safe_kegg_url_parser(url: any, compoundcolors: any, gene_highights: any): object;
      /**
        * @param data default value Is ``null``.
        * @param log2FC default value Is ``log2(FC)``.
        * @param id default value Is ``kegg``.
      */
      function url_encode(enrich: any, data?: any, log2FC?: any, id?: any): object;
      /**
        * @param data default value Is ``null``.
        * @param log2FC default value Is ``log2(FC)``.
        * @param id default value Is ``kegg``.
      */
      function url_encode_internal(enrich: any, data?: any, log2FC?: any, id?: any): object;
      /**
      */
      function write_dgr(dgr: any): object;
      /**
      */
      function write_dgrlist(dgrList: any): object;
      /**
      */
      function write_graph(graph: any): object;
      /**
      */
      function write_graphList(graphList: any): object;
      /**
      */
      function write_mset(mset: any): object;
      /**
      */
      function write_msetList(msetList: any): object;
      /**
      */
      function write_pathIds(pathIds: any): object;
      /**
      */
      function write_pathSmps(pathSmps: any): object;
      /**
      */
      function write_rbc(rbc: any): object;
      /**
      */
      function write_rbcList(rbcList: any): object;
   }
   /**
     * @param flag default value Is ``compounds``.
     * @param what default value Is ``kegg_compound``.
   */
   function __hds_compound_dir(kegg_db: any, dir: any, flag?: any, what?: any): object;
   /**
     * @param flag default value Is ``compounds``.
     * @param what default value Is ``kegg_compound``.
   */
   function __hds_compound_files(kegg_db: any, flag?: any, what?: any): object;
   /**
     * @param kegg_maps default value Is ``null``.
     * @param raw_maps default value Is ``false``.
   */
   function __load_kegg_map(kegg_maps?: any, raw_maps?: any): object;
   /**
   */
   function __parseIdvector(set: any): object;
   /**
   */
   function _unique_idset(id: any): object;
   /**
     * @param make.pathway_cluster default value Is ``true``.
   */
   function CompoundNetwork(compoundsId: any, make.pathway_cluster?: any): object;
   /**
     * @param ec_number default value Is ``*``.
   */
   function ecnumber_to_ko(ec_number?: any): object;
   eutils: any;
   /**
   */
   function extract_reactions(): object;
   /**
     * @param annotations default value Is ``["GENOME_GBFF", "GENOME_FASTA", "GENOME_GFF", "RNA_FASTA", "CDS_FASTA", "PROT_FASTA", "SEQUENCE_REPORT"]``.
   */
   function fetch_genbank(accession_id: any, annotations?: any): object;
   fetch_genbank_api: any;
   fetch_reference_genome: any;
   genbank_annotation_flags: any;
   /**
   */
   function hsa_pathways(): object;
   /**
     * @param up default value Is ``red``.
     * @param down default value Is ``blue``.
   */
   function kegg_colors(id: any, log2FC: any, up?: any, down?: any): object;
   /**
     * @param rawList default value Is ``false``.
     * @param reference_set default value Is ``true``.
   */
   function kegg_compounds(rawList?: any, reference_set?: any): object;
   /**
     * @param map_id default value Is ``KEGG``.
     * @param pathway_links default value Is ``pathway_links``.
     * @param outputdir default value Is ``./``.
     * @param min_objects default value Is ``0``.
     * @param kegg_maps default value Is ``null``.
   */
   function KEGG_MapRender(enrich: any, map_id?: any, pathway_links?: any, outputdir?: any, min_objects?: any, kegg_maps?: any): object;
   /**
     * @param rawMaps default value Is ``true``.
     * @param repo default value Is ``Call "system.file"("data/kegg/KEGG_maps.zip", "package" <- "GCModeller")``.
   */
   function kegg_maps(rawMaps?: any, repo?: any): object;
   /**
     * @param raw default value Is ``true``.
   */
   function kegg_reactions(raw?: any): object;
   /**
     * @param raw_maps default value Is ``true``.
   */
   function load_kegg_maps(raw_maps?: any): object;
   /**
     * @param compoundcolors default value Is ``red``.
     * @param gene_highights default value Is ``blue``.
     * @param outputdir default value Is ``./``.
     * @param min_objects default value Is ``0``.
   */
   function localRenderMap(KEGG_maps: any, pathwayList: any, compoundcolors?: any, gene_highights?: any, outputdir?: any, min_objects?: any): object;
   /**
     * @param taxonomy_name default value Is ``null``.
     * @param raw default value Is ``true``.
     * @param multiple_omics default value Is ``false``.
   */
   function metpa_background(pathways: any, taxonomy_name?: any, raw?: any, multiple_omics?: any): object;
   /**
     * @param log2FC default value Is ``log2(FC)``.
     * @param id default value Is ``kegg``.
   */
   function metpa_enrich(data: any, metpa: any, log2FC?: any, id?: any): object;
   /**
     * @param log2FC default value Is ``log2(FC)``.
     * @param id default value Is ``kegg``.
   */
   function metpa_enrich_dataframe(data: any, metpa: any, log2FC?: any, id?: any): object;
   /**
   */
   function metpa_enrich_ids(data: any, metpa: any): object;
   /**
     * @param download_dir default value Is ``./``.
     * @param overrides default value Is ``true``.
   */
   function ncbi_assembly_ftp(ref: any, download_dir?: any, overrides?: any): object;
   /**
   */
   function reference_genome(ncbi_taxid: any): object;
   /**
   */
   function split_omics_idset(IDs: any): object;
   taxonomy_query: any;
   /**
   */
   function taxonomy_search(name: any): object;
   /**
   */
   function unify_mapid(x: any): object;
   /**
     * @param outputdir default value Is ``./``.
     * @param id default value Is ``KEGG``.
     * @param compound default value Is ``compound``.
     * @param gene default value Is ``gene``.
     * @param protein default value Is ``protein``.
     * @param text.color default value Is ``white``.
     * @param kegg_maps default value Is ``null``.
   */
   function union_render(union_data: any, outputdir?: any, id?: any, compound?: any, gene?: any, protein?: any, text.color?: any, kegg_maps?: any): object;
   module url {
      /**
      */
      function search_taxonomy(name: any): object;
   }
}
