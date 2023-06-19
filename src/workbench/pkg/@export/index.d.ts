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
        * @param data default value Is ``null``.
        * @param log2FC default value Is ``log2(FC)``.
        * @param id default value Is ``kegg``.
      */
      function url_encode(enrich: any, data?: any, log2FC?: any, id?: any): object;
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
   */
   function __parseIdvector(set: any): object;
   /**
   */
   function _unique_idset(id: any): object;
   /**
     * @param make.pathway_cluster default value Is ``true``.
   */
   function CompoundNetwork(compoundsId: any, make.pathway_cluster?: any): object;
   eutils: any;
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
   */
   function kegg_compounds(rawList?: any): object;
   /**
     * @param map_id default value Is ``KEGG``.
     * @param pathway_links default value Is ``pathway_links``.
     * @param outputdir default value Is ``./``.
     * @param kegg_maps default value Is ``null``.
   */
   function KEGG_MapRender(enrich: any, map_id?: any, pathway_links?: any, outputdir?: any, kegg_maps?: any): object;
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
     * @param compoundcolors default value Is ``red``.
     * @param gene_highights default value Is ``blue``.
     * @param outputdir default value Is ``./``.
   */
   function localRenderMap(KEGG_maps: any, pathwayList: any, compoundcolors?: any, gene_highights?: any, outputdir?: any): object;
   /**
     * @param taxonomy_name default value Is ``null``.
     * @param raw default value Is ``true``.
   */
   function metpa_background(pathways: any, taxonomy_name?: any, raw?: any): object;
   /**
     * @param log2FC default value Is ``log2(FC)``.
     * @param id default value Is ``kegg``.
   */
   function metpa_enrich(data: any, metpa: any, log2FC?: any, id?: any): object;
   /**
   */
   function reference_genome(ncbi_taxid: any): object;
   taxonomy_query: any;
   /**
   */
   function taxonomy_search(name: any): object;
   module url {
      /**
      */
      function search_taxonomy(name: any): object;
   }
}
