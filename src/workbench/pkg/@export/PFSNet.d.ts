// export R# package module type define for javascript/typescript language
//
// ref=phenotype_kit.PFSNetAnalysis

/**
 * 
*/
declare namespace PFSNet {
   module load {
      /**
      */
      function pathway_network(file:string): any;
   }
   module build {
      /**
        * @param env default value is ``null``.
      */
      function pathway_network(maps:object, reactions:any, env?:object): any;
   }
   module save {
      /**
        * @param env default value is ``null``.
      */
      function pathway_network(ggi:any, file:any, env?:object): any;
   }
   /**
     * @param b default value is ``0.5``.
     * @param t1 default value is ``0.95``.
     * @param t2 default value is ``0.85``.
     * @param n default value is ``1000``.
   */
   function pfsnet(expr1o:any, expr2o:any, ggi:object, b?:number, t1?:number, t2?:number, n?:object): any;
   module read {
      /**
        * @param format default value is ``null``.
        * @param env default value is ``null``.
      */
      function pfsnet_result(file:string, format?:object, env?:object): any;
   }
}
