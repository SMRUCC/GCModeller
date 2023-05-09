// export R# package module type define for javascript/typescript language
//
// ref=vcellkit.Enzymatic@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * enzymatic reaction network modeller
 * 
*/
declare namespace enzymatic {
   /**
     * @param env default value Is ``null``.
   */
   function imports_rhea(rhea:any, repo:string, env?:object): any;
   module open {
      /**
      */
      function rhea(repo:string): any;
   }
   /**
     * @param cache default value Is ``'./.cache/'``.
   */
   function query_reaction(reaction:object, cache?:any): any;
   module read {
      /**
      */
      function rhea(file:string): any;
   }
}
