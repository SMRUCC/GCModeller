// export R# package module type define for javascript/typescript language
//
// ref=gseakit.profiles
// ref=kegg_kit.profiles

/**
 * enrichment term statics helper
 * 
 * 
*/
declare namespace profiles {
   /**
   */
   function kegg_category(background:object): any;
   module GO {
      module enrichment {
         /**
           * @param top default value is ``10``.
         */
         function profile(enrichments:object, goDb:object, top?:object): any;
      }
   }
   module KEGG {
      module enrichment {
         /**
           * @param top default value is ``10``.
           * @param env default value is ``null``.
         */
         function profile(enrichments:any, top?:object, env?:object): any;
      }
   }
   /**
     * @param top default value is ``10``.
   */
   function sort_profiles(profile:object, top?:object): any;
   /**
   */
   function cut_profiles(profile:object, valueCut:number): any;
   /**
     * @param top default value is ``30``.
     * @param env default value is ``null``.
   */
   function no_catagory_profile(enrichments:object, name:string, top?:object, env?:object): any;
   /**
     * @param level default value is ``1``.
     * @param env default value is ``null``.
   */
   function category_labels(category:object, idSet:string, level?:object, env?:object): any;
   module compounds {
      module pathway {
         /**
           * @param env default value is ``null``.
         */
         function index(pathways:any, env?:object): any;
         /**
         */
         function profiles(pathways:object, compounds:string): any;
      }
   }
   /**
     * @param env default value is ``null``.
   */
   function getProfileMapping(map:object, mapping:object, env?:object): any;
   module flux {
      module map {
         /**
           * @param env default value is ``null``.
         */
         function profiles(flux:any, maps:object, env?:object): any;
      }
   }
   module KO {
      module map {
         /**
           * @param env default value is ``null``.
         */
         function profiles(KO:any, env?:object): any;
      }
   }
   module kegg {
      /**
      */
      function category_profiles(profiles:object): any;
   }
   /**
   */
   function map_category(): any;
}
