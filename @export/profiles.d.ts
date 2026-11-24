// export R# package module type define for javascript/typescript language
//
//    imports "profiles" from "gseakit";
//    imports "profiles" from "kegg_kit";
//
// ref=gseakit.profiles@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=kegg_kit.profiles@kegg_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * enrichment term statics helper
 * 
 * annotation profiles tools
 * 
*/
declare namespace profiles {
   /**
    * create gsea background based on the reference kegg map data
    * 
    * 
     * @param kegg a collection of the reference kegg maps
     * @param ko a id mapping from kegg ko to gene id, [ko => gene_id]
     * @param multiple_omics the compound id will be keeps if multiple omics flag is TRUE.
     * 
     * + default value Is ``false``.
     * @param tcode the kegg organism code
     * 
     * + default value Is ``null``.
     * @param map_set 
     * + default value Is ``null``.
     * @param env 
     * + default value Is ``null``.
   */
   function assemble_background(kegg: object, ko: object, multiple_omics?: boolean, tcode?: string, map_set?: any, env?: object): object;
   /**
    * Get the category labels for a given id set.
    * 
    * 
     * @param category the KEGG category class model that is created by the ``kegg_category`` function.
     * @param idSet a character vector of the target id set
     * @param level the category level that will be returns: ``1`` (default) for the level 1 class, ``2`` for the level 2 category.
     * 
     * + default value Is ``1``.
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return a character vector of the category label of each input id, a ``NULL`` value will be
     *  returned for the id that is not found in the category model.
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
    * Cut the catalog profiles by the profile value (usually the ``-log10(p)`` score).
    * 
    * 
     * @param profile the catalog profiles object, for example the result of the ``KEGG.enrichment.profile`` function.
     * @param valueCut the minimum profile value that a term should have, the terms with a smaller value
     *  will be removed from the catalog profiles.
     * @return a new [CatalogProfiles](cref:T:SMRUCC.genomics.ComponentModel.Annotation.CatalogProfiles) object that only contains the terms with a value greater than or equals to the cutoff.
   */
   function cut_profiles(profile: object, valueCut: number): object;
   /**
    * Create the catalog profiles data of the GO enrichment result by the GO DAG term
    *  hierarchy, the terms are grouped by the child node of the given root term.
    * 
    * 
     * @param enrichments the kobas [EnrichmentTerm](cref:T:SMRUCC.genomics.Analysis.Microarray.KOBAS.EnrichmentTerm) or gcmodeller [EnrichmentResult](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.EnrichmentResult).
     * @param obo the GO obo database that is used for creating the DAG graph.
     * @param root the root term id
     * @param top display the top n enriched GO terms of each catalog.
     * 
     * + default value Is ``10``.
     * @param pvalue_cut the p-value cutoff of the enrichment terms. Default ``1`` means no filtering.
     * 
     * + default value Is ``1``.
     * @param sort sorts the terms of each catalog by the ``-log10(p)`` score? Default ``TRUE``.
     * 
     * + default value Is ``true``.
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return a [CatalogProfiles](cref:T:SMRUCC.genomics.ComponentModel.Annotation.CatalogProfiles) object, the name of each catalog is the child term of
     *  the given **root** and the value is the enriched GO terms of that branch.
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
           * @param enrichments the kobas [EnrichmentTerm](cref:T:SMRUCC.genomics.Analysis.Microarray.KOBAS.EnrichmentTerm) or gcmodeller [EnrichmentResult](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.EnrichmentResult).
           * @param goDb the GO obo database that is used for query the namespace of the GO terms.
           * @param top display the top n enriched GO terms.
           * 
           * + default value Is ``10``.
           * @param pvalue_cut the p-value cutoff of the enrichment terms. Default ``1`` means no filtering.
           * 
           * + default value Is ``1``.
           * @param sort sort of the namespace
           * 
           * + default value Is ``true``.
           * @param env the R# runtime environment.
           * 
           * + default value Is ``null``.
           * @return a [CatalogProfiles](cref:T:SMRUCC.genomics.ComponentModel.Annotation.CatalogProfiles) object, the name of each catalog is the GO namespace
           *  and the value is the top enriched GO terms of that namespace (the profile value is
           *  the ``-log10(p)`` score of the term).
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
           * @param enrichments the kobas [EnrichmentTerm](cref:T:SMRUCC.genomics.Analysis.Microarray.KOBAS.EnrichmentTerm) or gcmodeller [EnrichmentResult](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.EnrichmentResult).
           * @param top display the top n enriched pathway terms of each KEGG category.
           * 
           * + default value Is ``10``.
           * @param env the R# runtime environment.
           * 
           * + default value Is ``null``.
           * @return a [CatalogProfiles](cref:T:SMRUCC.genomics.ComponentModel.Annotation.CatalogProfiles) object, the name of each catalog is the KEGG category
           *  and the value is the top enriched pathway terms of that category.
         */
         function profile(enrichments: any, top?: object, env?: object): object;
      }
   }
   /**
    * Create the KEGG category class model from a GSEA background model.
    * 
    * 
     * @param background the GSEA background model that contains the KEGG class and category annotation.
     * @return a [ClassProfiles](cref:T:SMRUCC.genomics.ComponentModel.Annotation.ClassProfiles) object which contains the relationship between the KEGG
     *  class (or category) and the pathway ids.
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
    * Create a catalog profiles object that does not contains the category information,
    *  that means all of the enrichment terms are placed into a single catalog.
    * 
    * 
     * @param enrichments the enrichment analysis result set, both of the [EnrichmentTerm](cref:T:SMRUCC.genomics.Analysis.Microarray.KOBAS.EnrichmentTerm) and the
     *  [EnrichmentResult](cref:T:SMRUCC.genomics.Analysis.HTS.GSEA.EnrichmentResult) array are accepted.
     * @param name the catalog name that will be used for the result object.
     * @param top the number of the most significant terms that will be kept. Default ``30``.
     * 
     * + default value Is ``30``.
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return a [CatalogProfiles](cref:T:SMRUCC.genomics.ComponentModel.Annotation.CatalogProfiles) object that contains only one catalog which is named by
     *  the **name** parameter, or an error message when the input data type is
     *  not supported.
   */
   function no_catagory_profile(enrichments: object, name: string, top?: object, env?: object): object;
   /**
    * Keep only the top n terms of each catalog of the given catalog profiles.
    * 
    * 
     * @param profile the catalog profiles object, for example the result of the ``KEGG.enrichment.profile`` function.
     * @param top the number of the terms that will be kept in each catalog. Default ``10``.
     * 
     * + default value Is ``10``.
     * @return a new [CatalogProfiles](cref:T:SMRUCC.genomics.ComponentModel.Annotation.CatalogProfiles) object that only contains the top n terms of each catalog.
   */
   function sort_profiles(profile: object, top?: object): object;
}
