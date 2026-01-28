// export R# package module type define for javascript/typescript language
//
//    imports "taxonomy_kit" from "metagenomics_kit";
//
// ref=metagenomics_kit.TaxonomyKit@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * toolkit for process ncbi taxonomy tree data
 * 
 * > The Taxonomy Database is a curated classification and nomenclature for all of the 
 * >  organisms in the public sequence databases. This currently represents about 10% 
 * >  of the described species of life on the planet.
*/
declare namespace taxonomy_kit {
   /**
    * Create a stream read to the ncbi accession id mapping to ncbi taxonomy id
    * 
    * 
     * @param map_files should be a set of file path to the ncbi accession map to taxid table file, example as: ``c('nucl_gb.accession2taxid', 'nucl_wgs.accession2taxid')``
   */
   function accession2Taxid(map_files: any): object;
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
   }
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
       * parse the taxonomy string in BIOM style
       * 
       * 
        * @param taxonomy a character vector of the taxonomy string in BIOM style
        * @param env -
        * 
        * + default value Is ``null``.
        * @return a vector of @``T:SMRUCC.genomics.Metagenomics.Taxonomy`` object.
      */
      function parse(taxonomy: any, env?: object): any;
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
       * >  
       * >  + https://www.biostars.org/p/13452/ 
       * >  + https://pythonhosted.org/ete2/tutorial/tutorial_ncbitaxonomy.html
       * 
        * @param repo a directory folder path which contains the NCBI taxonomy 
        *  tree data files: ``nodes.dmp`` and ``names.dmp``.
      */
      function taxonomy_tree(repo: string): object;
   }
   /**
    * cast the ncbi taxonomy tree model to taxonomy ranks data
    * 
    * 
     * @param ncbi_tree -
   */
   function ranks(ncbi_tree: object): object;
   module read {
      /**
       * Parse the result output from mothur command ``summary.tax``.
       * 
       * 
        * @param file -
      */
      function mothurTree(file: string): object;
   }
   module taxonomy {
      /**
       * 
       * 
        * @param tree the ncbi taxonomy tree model
        * @param range a collection of the ncbi taxonomy id or BIOM taxonomy string.
        * @param taxid a lambda function will be returns if this ncbi taxonomy id set is missing.
        * 
        * + default value Is ``null``.
      */
      function filter(tree: object, range: string, taxid?: object): object|object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function taxonomy_name(taxonomy: any, env?: object): any;
   /**
   */
   function taxonomy_range(tax: object, rank: object): object;
   /**
    * get all taxonomy tree nodes of the specific taxonomy ranks
    * 
    * 
     * @param tree -
     * @param rank -
   */
   function taxonomy_ranks(tree: object, rank: object): object;
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
}
