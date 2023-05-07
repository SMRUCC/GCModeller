// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.Enzymatic

/**
 * enzymatic reaction network modeller
 * 
*/
declare namespace enzymatic {
   /**
     * @param cache default value is ``'./.cache/'``.
   */
   function query_reaction(reaction:object, cache?:any): any;
   module read {
      /**
      */
      function rhea(file:string): any;
   }
   module open {
      /**
      */
      function rhea(repo:string): any;
   }
   /**
     * @param env default value is ``null``.
   */
   function imports_rhea(rhea:any, repo:string, env?:object): any;
}
