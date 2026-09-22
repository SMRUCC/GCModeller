// export R# package module type define for javascript/typescript language
//
//    imports "gast" from "metagenomics_kit";
//
// ref=metagenomics_kit.gastTools@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace gast {
   module OTU {
      /**
        * @param min_pct default value Is ``0.97``.
        * @param gast_consensus default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function taxonomy(blastn: any, OTUs: object, taxonomy: object, min_pct?: number, gast_consensus?: boolean, env?: object): object;
   }
   module parse {
      /**
      */
      function greengenes_tax(tax: string): object;
      /**
        * @param removes_lt default value Is ``0.0001``.
      */
      function mothur_OTUs(OTU_rep_fasta: string, removes_lt?: number): object;
   }
}
