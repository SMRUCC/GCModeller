declare namespace ptf {
   module list {
      /**
      */
      function xrefs(ptf:object): any;
   }
   module summary {
      /**
      */
      function xrefs(ptf:object, xrefs:string): any;
   }
   /**
   */
   function load_xref(ptf:object, database:string): any;
   module read {
      /**
      */
      function ptf(file:string): any;
   }
   /**
     * @param id default value is ``null``.
   */
   function ID_mapping(proteins:object, from:string, mapTo:string, id?:string): any;
   /**
   */
   function loadBackgroundModel(ptf:object, database:string): any;
   module cache {
      /**
        * @param db_xref default value is ``["Bgee","KEGG","KO","GO","Pfam","RefSeq","EC","InterPro","BioCyc","eggNOG","keyword"]``.
        * @param cacheTaxonomy default value is ``false``.
        * @param hds_stream default value is ``false``.
        * @param env default value is ``null``.
      */
      function ptf(uniprot:any, file:any, db_xref?:any, cacheTaxonomy?:boolean, hds_stream?:boolean, env?:object): any;
   }
}
