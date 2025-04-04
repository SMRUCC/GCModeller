﻿// export R# package module type define for javascript/typescript language
//
//    imports "profiles" from "gseakit";
//    imports "profiles" from "kegg_kit";
//
// ref=gseakit.profiles@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=kegg_kit.profiles@kegg_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * enrichment term statics helper
 * 
 * 
*/
declare namespace profiles {
   /**
    * get category labels for a given id set
    * 
    * 
     * @param category -
     * @param idSet a character vector of the target id set
     * @param level level1 or level2
     * 
     * + default value Is ``1``.
     * @param env 
     * + default value Is ``null``.
   */
   function category_labels(category: object, idSet: string, level?: object, env?: object): any;
   module compounds {
      module pathway {
         /**
           * @param env default value Is ``null``.
         */
         function index(pathways: any, env?: object): object;
         /**
          * Do statistics of the KEGG pathway profiles based on the given kegg id
          * 
          * 
           * @param pathways The pathway compound reference index data
           * @param compounds The kegg compound id
         */
         function profiles(pathways: object, compounds: string): object;
      }
   }
   /**
   */
   function cut_profiles(profile: object, valueCut: number): object;
   /**
    * 
    * 
     * @param enrichments -
     * @param obo -
     * @param root the root term id
     * @param top -
     * 
     * + default value Is ``10``.
     * @param pvalue_cut -
     * 
     * + default value Is ``1``.
     * @param sort -
     * 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function dag_enrichment_profile(enrichments: any, obo: object, root: string, top?: object, pvalue_cut?: number, sort?: boolean, env?: object): any;
   module flux {
      module map {
         /**
           * @param env default value Is ``null``.
         */
         function profiles(flux: any, maps: object, env?: object): object;
      }
   }
   /**
     * @param env default value Is ``null``.
   */
   function getProfileMapping(map: object, mapping: object, env?: object): any;
   module GO {
      module enrichment {
         /**
          * Create catalog profiles data for GO enrichment result its data visualization.
          * 
          * 
           * @param enrichments the kobas @``T:SMRUCC.genomics.Analysis.Microarray.KOBAS.EnrichmentTerm`` or gcmodeller @``T:SMRUCC.genomics.Analysis.HTS.GSEA.EnrichmentResult``.
           * @param goDb -
           * @param top display the top n enriched GO terms.
           * 
           * + default value Is ``10``.
           * @param pvalue_cut 
           * + default value Is ``1``.
           * @param sort sort of the namespace
           * 
           * + default value Is ``true``.
           * @param env 
           * + default value Is ``null``.
         */
         function profile(enrichments: any, goDb: object, top?: object, pvalue_cut?: number, sort?: boolean, env?: object): object;
      }
   }
   module kegg {
      /**
       * create kegg catalog profiles data table
       * 
       * 
        * @param profiles -
      */
      function category_profiles(profiles: object): object;
   }
   module KEGG {
      module enrichment {
         /**
          * A method for cast the kegg enrichment result to the 
          *  category profiles for run data visualization
          * 
          * 
           * @param enrichments -
           * @param top -
           * 
           * + default value Is ``10``.
           * @param env -
           * 
           * + default value Is ``null``.
         */
         function profile(enrichments: any, top?: object, env?: object): object;
      }
   }
   /**
    * create kegg category class model from a gsea background model
    * 
    * 
     * @param background -
   */
   function kegg_category(background: object): object;
   module KO {
      module map {
         /**
          * create KEGG map prfiles via a given KO id list.
          * 
          * 
           * @param KO a character vector of KO id list.
           * @param env -
           * 
           * + default value Is ``null``.
         */
         function profiles(KO: any, env?: object): object;
      }
   }
   /**
   */
   function map_category(): object;
   /**
     * @param top default value Is ``30``.
     * @param env default value Is ``null``.
   */
   function no_catagory_profile(enrichments: object, name: string, top?: object, env?: object): object;
   /**
     * @param top default value Is ``10``.
   */
   function sort_profiles(profile: object, top?: object): object;
}
