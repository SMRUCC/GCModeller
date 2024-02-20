﻿// export R# package module type define for javascript/typescript language
//
//    imports "microbiome" from "metagenomics_kit";
//
// ref=metagenomics_kit.microbiomeKit@metagenomics_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * tools for metagenomics and microbiome
 * 
*/
declare namespace microbiome {
   module compounds {
      module origin {
         /**
         */
         function profile(taxonomy: object, organism: string): object;
      }
   }
   module diff {
      /**
       * evaluate the similarity of two taxonomy data vector
       * 
       * > compares on a specific @``T:SMRUCC.genomics.Metagenomics.TaxonomyRanks``
       * 
        * @param v1 the names of the list should be the BIOM taxonomy string, 
        *  content value of the list is the relative abundance data.
        * @param v2 the names of the list should be the BIOM taxonomy string, 
        *  content value of the list is the relative abundance data.
        * @param rank 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function entropy(v1: object, v2: object, rank?: object, env?: object): number;
   }
   module parse {
      /**
       * parse the otu taxonomy data file
       * 
       * 
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function otu_taxonomy(file: any, env?: object): object;
   }
   /**
    * creates the final metagenome functional predictions. It 
    *  multiplies each normalized OTU abundance by each predicted 
    *  functional trait abundance to produce a table of functions 
    *  (rows) by samples (columns).
    * 
    * 
     * @param table should be a merged OTU dataframe object, that should be in format like:
     *  
     *  1. the colnames should be the sample name, and the column field value is the relative abundance value of each otu in each sample
     *  2. the rows in this dataframe should be the otu expression value across samples
     *  
     *  the GCModeller internal @``T:SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix`` is also avaiable 
     *  for this parameter.
     * @param env 
     * + default value Is ``null``.
   */
   function predict_metagenomes(PICRUSt: object, table: any, env?: object): object;
   module read {
      /**
      */
      function PICRUSt_matrix(file: object): object;
   }
   module save {
      /**
       * 
       * > write the data matrix via @``T:SMRUCC.genomics.Analysis.Metagenome.MetaFunction.PICRUSt.MetaBinaryWriter``
       * 
        * @param ggtax -
        * @param ko_13_5_precalculated -
        * @param save -
      */
      function PICRUSt_matrix(ggtax: object, ko_13_5_precalculated: object, save: object): boolean;
   }
}
