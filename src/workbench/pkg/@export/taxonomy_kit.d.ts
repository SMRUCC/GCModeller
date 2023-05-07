declare namespace taxonomy_kit {
   module biom {
      /**
        * @param trim_genusName default value is ``false``.
        * @param env default value is ``null``.
      */
      function string(taxonomy:any, trim_genusName?:boolean, env?:object): any;
   }
   module biom_string {
      /**
        * @param env default value is ``null``.
      */
      function parse(taxonomy:any, env?:object): any;
   }
   /**
     * @param env default value is ``null``.
   */
   function unique_taxonomy(taxonomy:any, env?:object): any;
   module Ncbi {
      /**
      */
      function taxonomy_tree(repo:string): any;
   }
   module taxonomy {
      /**
        * @param taxid default value is ``null``.
      */
      function filter(tree:object, range:string, taxid?:object): any;
   }
   /**
     * @param fullName default value is ``false``.
   */
   function lineage(tree:object, tax:string, fullName?:boolean): any;
   module as {
      module taxonomy {
         /**
         */
         function tree(taxonomy:object): any;
      }
      /**
      */
      function OTU_table(tree:object): any;
      /**
        * @param id default value is ``'OTU_num'``.
        * @param taxonomy default value is ``'taxonomy'``.
        * @param env default value is ``null``.
      */
      function OTUtable(table:object, id?:string, taxonomy?:string, env?:object): any;
   }
   /**
   */
   function consensus(tree:object, rank:object): any;
   module read {
      /**
      */
      function mothurTree(file:string): any;
      /**
        * @param sumDuplicated default value is ``true``.
      */
      function OTUtable(file:string, sumDuplicated?:boolean): any;
   }
   /**
   */
   function taxonomy_range(tax:object, rank:object): any;
}
