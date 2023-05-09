// export R# package module type define for javascript/typescript language
//
// ref=metagenomics_kit.TaxonomyKit@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * toolkit for process ncbi taxonomy tree data
 * 
*/
declare namespace taxonomy_kit {
   module biom {
      /**
       * cast taxonomy object to biom style taxonomy string
       * 
       * 
        * @param taxonomy -
        * @param trim_genusName removes the genus name from the species name?
        * 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function string(taxonomy: any, trim_genusName?: boolean, env?: object): string;
   }
   module biom_string {
      /**
        * @param env default value Is ``null``.
      */
      function parse(taxonomy: any, env?: object): any;
   }
   /**
    * make taxonomy object unique
    * 
    * 
     * @param taxonomy -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function unique_taxonomy(taxonomy: any, env?: object): any;
   module Ncbi {
      /**
       * load ncbi taxonomy tree model from the given data files
       * 
       * > Builds the following dictionnary from NCBI taxonomy ``nodes.dmp`` and ``names.dmp``
       * >  files 
       * >  
       * >  ```json 
       * >  { Taxid namedtuple('Node', ['name', 'rank', 'parent', 'children']
       * >      } 
       * >  ``` 
       * >  + https://www.biostars.org/p/13452/ 
       * >  + https://pythonhosted.org/ete2/tutorial/tutorial_ncbitaxonomy.html
       * 
        * @param repo a directory folder path which contains the NCBI taxonomy 
        *  tree data files: ``nodes.dmp`` and ``names.dmp``.
      */
      function taxonomy_tree(repo: string): object;
   }
   module taxonomy {
      /**
        * @param taxid default value Is ``null``.
      */
      function filter(tree: object, range: string, taxid?: object): object|object;
   }
   /**
    * get taxonomy lineage model from the ncbi taxonomy tree by given taxonomy id
    * 
    * 
     * @param tree the ncbi taxonomy tree model
     * @param tax the ncbi taxonomy id or taxonomy string in BIOM style.
     * @param fullName -
     * 
     * + default value Is ``false``.
   */
   function lineage(tree: object, tax: string, fullName?: boolean): object;
   module as {
      module taxonomy {
         /**
          * build taxonomy tree based on a given collection of taxonomy object.
          * 
          * 
           * @param taxonomy -
         */
         function tree(taxonomy: object): object;
      }
      /**
       * convert the mothur rank tree as the OTU table
       * 
       * 
        * @param tree -
      */
      function OTU_table(tree: object): object;
      /**
       * convert the mothur rank tree as the OTU table
       * 
       * 
        * @param id 
        * + default value Is ``'OTU_num'``.
        * @param taxonomy 
        * + default value Is ``'taxonomy'``.
        * @param env 
        * + default value Is ``null``.
      */
      function OTUtable(table: object, id?: string, taxonomy?: string, env?: object): object;
   }
   /**
   */
   function consensus(tree: object, rank: object): object;
   module read {
      /**
       * Parse the result output from mothur command ``summary.tax``.
       * 
       * 
        * @param file -
      */
      function mothurTree(file: string): object;
      /**
       * read 16s OTU table
       * 
       * 
        * @param file -
        * @param sumDuplicated -
        * 
        * + default value Is ``true``.
      */
      function OTUtable(file: string, sumDuplicated?: boolean): object;
   }
   /**
   */
   function taxonomy_range(tax: object, rank: object): object;
}
