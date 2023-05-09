// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.Compiler@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The GCModeller virtual cell model creator
 * 
*/
declare namespace compiler {
   /**
    * create kegg repository arguments
    * 
    * 
     * @param compounds A directory path which the folder contains the compounds data models.
     * @param maps A directory path which the folder contains the kegg reference map data.
     * @param reactions -
     * @param glycan2Cpd -
   */
   function kegg(compounds: string, maps: string, reactions: string, glycan2Cpd: object): object;
   module geneKO {
      /**
       * create a list that map gene id to KO id.
       * 
       * 
        * @param data any kind of dataframe dataset.
        * @param KOcol the column name for get KO term id
        * 
        * + default value Is ``'KO'``.
        * @param geneIDcol the column name for get gene id
        * 
        * + default value Is ``'ID'``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function maps(data: any, KOcol?: string, geneIDcol?: string, env?: object): object;
   }
   module assembling {
      /**
       * create genome framework for the virtual cell model
       * 
       * 
        * @param replicons -
        * @param geneKO -
        * @param lociAsLocus_tag -
        * 
        * + default value Is ``false``.
      */
      function genome(replicons: object, geneKO: object, lociAsLocus_tag?: boolean): object;
      /**
       * add metabolism network information for the virtual cell model
       * 
       * 
        * @param cell -
        * @param geneKO -
        * @param repo -
      */
      function metabolic(cell: object, geneKO: object, repo: object): object;
      /**
       * add transcription regulation network information for the virtual cell model
       * 
       * 
        * @param model -
        * @param regulations -
      */
      function TRN(model: object, regulations: object): object;
   }
   module vcell {
      /**
       * Save the virtual cell data model in XML file format.
       * 
       * 
        * @param model -
        * @param genomes -
        * @param KEGG -
        * @param regulations -
        * @param lociAsLocus_tag -
        * 
        * + default value Is ``false``.
        * @param logfile 
        * + default value Is ``null``.
      */
      function markup(model: object, genomes: object, KEGG: object, regulations: object, lociAsLocus_tag?: boolean, logfile?: string): object;
   }
   module compile {
      /**
        * @param logfile default value Is ``'./gcc.log'``.
      */
      function biocyc(biocyc: object, genomes: object, logfile?: string): object;
   }
}
