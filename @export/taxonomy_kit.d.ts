// export R# package module type define for javascript/typescript language
//
//    imports "taxonomy_kit" from "metagenomics_kit";
//
// ref=metagenomics_kit.TaxonomyKit@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace taxonomy_kit {
   /**
   */
   function accession2Taxid(map_files: any): object;
   module as {
      module taxonomy {
         /**
         */
         function tree(taxonomy: object): object;
      }
   }
   module biom {
      /**
        * @param trim_genusName default value Is ``false``.
        * @param env default value Is ``null``.
      */
      function string(taxonomy: any, trim_genusName?: boolean, env?: object): string;
   }
   module biom_string {
      /**
        * @param env default value Is ``null``.
      */
      function parse(taxonomy: any, env?: object): object;
   }
   /**
   */
   function consensus(tree: object, rank: object): object;
   /**
     * @param min_supports default value Is ``0.5``.
     * @param as_list default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function LCA(tree: object, ncbi_taxid: any, min_supports?: number, as_list?: boolean, env?: object): object;
   /**
     * @param fullName default value Is ``false``.
   */
   function lineage(tree: object, tax: string, fullName?: boolean): object;
   module Ncbi {
      /**
      */
      function taxonomy_tree(repo: string): object;
   }
   /**
   */
   function ranks(ncbi_tree: object): object;
   module read {
      /**
      */
      function mothurTree(file: string): object;
   }
   module taxonomy {
      /**
        * @param taxid default value Is ``null``.
      */
      function filter(tree: object, range: string, taxid?: object): object|object;
   }
   /**
     * @param rank default value Is ``null``.
     * @param missing default value Is ``'Unknown'``.
     * @param env default value Is ``null``.
   */
   function taxonomy_name(taxonomy: any, rank?: object, missing?: string, env?: object): any;
   /**
   */
   function taxonomy_range(tax: object, rank: object): object;
   /**
   */
   function taxonomy_ranks(tree: object, rank: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function unique_taxonomy(taxonomy: any, env?: object): any;
}
