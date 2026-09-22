// export R# package module type define for javascript/typescript language
//
//    imports "proteinKit" from "seqtoolkit";
//
// ref=seqtoolkit.proteinKit@seqtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace proteinKit {
   /**
     * @param env default value Is ``null``.
   */
   function analysis_domains(blastp: any, env?: object): object;
   /**
     * @param polyaa default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function chou_fasman(prot: any, polyaa?: boolean, env?: object): string|object;
   /**
     * @param kmer default value Is ``3``.
     * @param env default value Is ``null``.
   */
   function enzyme_builder(enzymes: any, kmer?: object, env?: object): object;
   /**
     * @param radius default value Is ``3``.
     * @param len default value Is ``4096``.
   */
   function kmer_fingerprint(graph: object, radius?: object, len?: object): any;
   /**
     * @param k default value Is ``3``.
     * @param env default value Is ``null``.
   */
   function kmer_graph(prot: any, k?: object, env?: object): object;
   /**
     * @param key default value Is ``null``.
     * @param number default value Is ``-1``.
   */
   function ligands(pdb: object, key?: string, number?: object): object;
   /**
     * @param safe default value Is ``false``.
     * @param verbose default value Is ``false``.
   */
   function parse_pdb(pdb_txt: string, safe?: boolean, verbose?: boolean): object;
   /**
     * @param as_vector default value Is ``false``.
   */
   function pdb_centroid(pdb: object, as_vector?: boolean): object|number;
   /**
   */
   function pdb_models(pdb: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function predict_sequence(model: object, ec_number: any, env?: object): object;
   module read {
      /**
        * @param safe default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function pdb(file: any, safe?: boolean, env?: object): object;
   }
}
