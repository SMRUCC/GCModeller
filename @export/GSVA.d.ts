// export R# package module type define for javascript/typescript language
//
//    imports "GSVA" from "gseakit";
//
// ref=gseakit.GSVA@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace GSVA {
   /**
   */
   function diff(gsva: object, compares: object): object;
   /**
     * @param name_suffix default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function gsva(expr: any, geneSet: any, name_suffix?: boolean, env?: object): object;
   /**
     * @param pathId default value Is ``'pathNames'``.
     * @param t default value Is ``'t'``.
     * @param pvalue default value Is ``'pvalue'``.
   */
   function matrix_to_diff(diff: object, pathId?: string, t?: string, pvalue?: string): object;
}
