// export R# package module type define for javascript/typescript language
//
// ref=phenotype_kit.PFSNetAnalysis@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace PFSNet {
   module build {
      /**
        * @param env default value Is ``null``.
      */
      function pathway_network(maps:object, reactions:any, env?:object): object;
   }
   module load {
      /**
      */
      function pathway_network(file:string): object;
   }
   /**
    * Finding consistent disease subnetworks using PFSNet
    * 
    * 
     * @param expr1o -
     * @param expr2o -
     * @param ggi -
     * @param b -
     * 
     * + default value Is ``0.5``.
     * @param t1 -
     * 
     * + default value Is ``0.95``.
     * @param t2 -
     * 
     * + default value Is ``0.85``.
     * @param n -
     * 
     * + default value Is ``1000``.
   */
   function pfsnet(expr1o:any, expr2o:any, ggi:object, b?:number, t1?:number, t2?:number, n?:object): object;
   module read {
      /**
       * 
       * 
        * @param file -
        * @param format xml/json
        * 
        * + default value Is ``null``.
        * @param env 
        * + default value Is ``null``.
      */
      function pfsnet_result(file:string, format?:object, env?:object): object;
   }
   module save {
      /**
        * @param env default value Is ``null``.
      */
      function pathway_network(ggi:any, file:any, env?:object): boolean;
   }
}
