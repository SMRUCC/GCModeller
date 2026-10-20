// export R# package module type define for javascript/typescript language
//
//    imports "linear" from "TRNtoolkit";
//
// ref=TRNtoolkit.Linear@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace linear {
   /**
   */
   function coef(grn: object): object;
   /**
    * 
    * 
     * @param x -
     * @param top -
     * 
     * + default value Is ``1000``.
     * @param candidates -
     * 
     * + default value Is ``1000``.
     * @param maxreg -
     * 
     * + default value Is ``10``.
     * @param l1_ratio -
     * 
     * + default value Is ``0.7``.
     * @param alphas -
     * 
     * + default value Is ``40``.
     * @param mutual -
     * 
     * + default value Is ``false``.
   */
   function elastic_net(x: object, top?: object, candidates?: object, maxreg?: object, l1_ratio?: number, alphas?: number, mutual?: boolean): object|object;
}
