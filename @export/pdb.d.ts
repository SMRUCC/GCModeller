// export R# package module type define for javascript/typescript language
//
//    imports "pdb" from "molecular_kit";
//
// ref=molecular_kit.pdbTools@molecular_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace pdb {
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
   module read {
      /**
        * @param safe default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function pdb(file: any, safe?: boolean, env?: object): object;
   }
}
