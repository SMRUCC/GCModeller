﻿// export R# source type define for javascript/typescript language
//
// package_source=GCModeller

declare namespace GCModeller {
   module _ {
      /**
      */
      function onLoad(): object;
      /**
        * @param data default value Is ``NULL``.
        * @param log2FC default value Is ``"log2(FC)"``.
        * @param id default value Is ``"kegg"``.
      */
      function url_encode(enrich:any, data:any, log2FC:string, id:string): object;
      /**
        * @param entryName default value Is ``NULL``.
      */
      function readZipStream(zipfile:any, entryName:any): object;
      /**
      */
      function cast_CLR_metpa(metpa:any): object;
      /**
      */
      function write_dgr(dgr:any): object;
      /**
      */
      function write_graph(graph:any): object;
      /**
      */
      function write_graphList(graphList:any): object;
      /**
      */
      function write_dgrlist(dgrList:any): object;
      /**
      */
      function write_mset(mset:any): object;
      /**
      */
      function write_msetList(msetList:any): object;
      /**
      */
      function write_pathIds(pathIds:any): object;
      /**
      */
      function write_rbc(rbc:any): object;
      /**
      */
      function write_rbcList(rbcList:any): object;
      /**
      */
      function write_pathSmps(pathSmps:any): object;
   }
   /**
     * @param map_id default value Is ``"KEGG"``.
     * @param pathway_links default value Is ``"pathway_links"``.
     * @param outputdir default value Is ``"./"``.
     * @param kegg_maps default value Is ``NULL``.
   */
   function KEGG_MapRender(enrich:any, map_id:string, pathway_links:string, outputdir:string, kegg_maps:any): object;
   /**
     * @param compoundcolors default value Is ``"red"``.
     * @param gene_highights default value Is ``"blue"``.
     * @param outputdir default value Is ``"./"``.
   */
   function localRenderMap(KEGG_maps:any, pathwayList:any, compoundcolors:string, gene_highights:string, outputdir:string): object;
   /**
   */
   function _unique_idset(id:any): object;
   /**
     * @param log2FC default value Is ``"log2(FC)"``.
     * @param id default value Is ``"kegg"``.
   */
   function metpa_enrich(data:any, metpa:any, log2FC:string, id:string): object;
   /**
   */
   function __parseIdvector(set:any): object;
   /**
     * @param up default value Is ``"red"``.
     * @param down default value Is ``"blue"``.
   */
   function kegg_colors(id:any, log2FC:any, up:string, down:string): object;
   /**
     * @param make.pathway_cluster default value Is ``True``.
   */
   function CompoundNetwork(compoundsId:any, make.pathway_cluster:boolean): object;
   /**
     * @param rawList default value Is ``False``.
   */
   function kegg_compounds(rawList:boolean): object;
   /**
     * @param rawMaps default value Is ``True``.
     * @param repo default value Is ``Call "system.file"("data/kegg/KEGG_maps.zip", "package" <- "GCModeller")``.
   */
   function kegg_maps(rawMaps:boolean, repo:any): object;
   /**
     * @param raw default value Is ``True``.
   */
   function kegg_reactions(raw:boolean): object;
   /**
     * @param taxonomy_name default value Is ``NULL``.
     * @param raw default value Is ``True``.
   */
   function metpa_background(pathways:any, taxonomy_name:any, raw:boolean): object;
   /**
   */
   function hsa_pathways(): object;
}