// export R# package module type define for javascript/typescript language
//
//    imports "microbiome" from "metagenomics_kit";
//
// ref=metagenomics_kit.microbiomeKit@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace microbiome {
   module build {
      /**
        * @param env default value Is ``null``.
      */
      function PICRUSt_db(ggtax: object, copyNumbers_16s: object, ko_13_5_precalculated: object, save: object, env?: object): boolean;
   }
   /**
     * @param rank default value Is ``null``.
     * @param ranges default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function compound_origins(annotations: object, tree: object, rank?: object, ranges?: string, env?: object): object;
   module compounds {
      module origin {
         /**
         */
         function profile(taxonomy: object, organism: string): object;
      }
   }
   module diff {
      /**
        * @param rank default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function entropy(v1: object, v2: object, rank?: object, env?: object): number;
   }
   /**
   */
   function make_vfdb_model(file: string): object;
   module parse {
      /**
        * @param env default value Is ``null``.
      */
      function otu_taxonomy(file: any, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function predict_metagenomes(PICRUSt: object, table: any, env?: object): object;
   /**
   */
   function read_PICRUSt(file: object): object;
   module taxonomy {
      /**
        * @param as_matrix default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function rank_table(otus: any, as_matrix?: boolean, env?: object): object|object;
   }
}
