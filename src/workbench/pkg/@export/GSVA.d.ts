declare namespace GSVA {
   /**
     * @param env default value is ``null``.
   */
   function gsva(expr:any, geneSet:any, env?:object): any;
   /**
   */
   function diff(gsva:object, compares:object): any;
   /**
     * @param pathId default value is ``'pathNames'``.
     * @param t default value is ``'t'``.
     * @param pvalue default value is ``'pvalue'``.
   */
   function matrix_to_diff(diff:object, pathId?:string, t?:string, pvalue?:string): any;
}
