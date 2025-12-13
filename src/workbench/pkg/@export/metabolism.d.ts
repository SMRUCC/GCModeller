// export R# package module type define for javascript/typescript language
//
//    imports "metabolism" from "kegg_kit";
//
// ref=kegg_kit.metabolism@kegg_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The kegg metabolism model toolkit
 * 
*/
declare namespace metabolism {
   module filter {
      /**
       * Removes invalid kegg compound id
       * 
       * 
        * @param identified -
      */
      function invalid_keggIds(identified: string): string;
   }
   /**
    * do kegg pathway reconstruction by given protein annotation data
    * 
    * 
     * @param reference the kegg reference maps
     * @param reactions a list of the kegg reaction data models
     * @param annotations the @``T:SMRUCC.genomics.Annotation.Ptf.ProteinAnnotation`` data stream with kegg ontology('ko' attribute) id.
     * @param min_cov coverage cutoff of the ratio of annotation protein hit against the all proteins on the pathway map
     * 
     * + default value Is ``0.3``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return A set of the kegg pathway object that contains with the KEGG id mapping(protein id mapping and assigned compound id list)
   */
   function kegg_reconstruction(reference: any, reactions: any, annotations: any, min_cov?: number, env?: object): object;
   module load {
      module reaction {
         /**
         */
         function cacheIndex(file: string): object;
      }
   }
   /**
    * pick the reaction list from the kegg reaction
    *  network repository by KO id terms
    * 
    * 
     * @param reactions -
     * @param terms the KO id terms
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function pickNetwork(reactions: object, terms: any, env?: object): any;
   module related {
      /**
       * Get compounds kegg id which is related to the given KO id list
       * 
       * 
        * @param enzymes KO id list
        * @param reactions -
      */
      function compounds(enzymes: string, reactions: object): string;
   }
}
