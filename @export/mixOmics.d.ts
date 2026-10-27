// export R# package module type define for javascript/typescript language
//
//    imports "mixOmics" from "phenotype_kit";
//
// ref=phenotype_kit.mixOmics@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * multiple omics analysis tool
 * 
*/
declare namespace mixOmics {
   /**
    * CC lasso analysis
    * 
    * 
     * @param x -
     * @param y -
     * @param lam_min_ratio -
     * 
     * + default value Is ``0.001``.
     * @param nfold -
     * 
     * + default value Is ``5``.
     * @param n_bootstraps -
     * 
     * + default value Is ``500``.
     * @param strict -
     * 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function cclasso(x: object, y: object, lam_min_ratio?: number, nfold?: object, n_bootstraps?: object, strict?: boolean, env?: object): object;
   /**
     * @param pval_cutoff default value Is ``0.05``.
     * @param cor_cutoff default value Is ``0.6``.
   */
   function connections(cor: any, pval_cutoff?: number, cor_cutoff?: number): object|object;
   /**
   */
   function map_force(x: object, y: object, maps: object): object;
   /**
    * Spearman + MIC
    * 
    * 
     * @param x -
     * @param y -
   */
   function mine(x: object, y: object): object;
   /**
     * @param freqCut default value Is ``19``.
     * @param uniqueCut default value Is ``10``.
   */
   function nearZeroVar(expr_mat: object, freqCut?: number, uniqueCut?: number): string;
   module omics {
      /**
        * @param xlab default value Is ``'X'``.
        * @param ylab default value Is ``'Y'``.
        * @param size default value Is ``'3000,3000'``.
        * @param padding default value Is ``'padding: 200px 250px 200px 100px;'``.
        * @param ptSize default value Is ``10``.
        * @param env default value Is ``null``.
      */
      function 2D_scatter(x: any, y: any, xlab?: string, ylab?: string, size?: any, padding?: any, ptSize?: number, env?: object): any;
   }
   /**
    * 
    * 
     * @param x -
     * @param name -
     * @param dir 方向约定：`+1` 表示数值升高代表通路活性升高（mRNA / protein），
     *  `-1` 表示数值升高代表通路活性下降（例如甲基化 beta 值），
     *  打分完成后会对该层的 ES 取负以统一到"活性方向"。
     * 
     * + default value Is ``1``.
     * @param verbose -
     * 
     * + default value Is ``false``.
   */
   function omics_layer(x: object, name: string, dir?: object, verbose?: boolean): object;
   /**
    * SparCC correlation analysis
    * 
    * 
     * @param x -
     * @param y -
     * @param strict -
     * 
     * + default value Is ``true``.
   */
   function sparcc(x: object, y: object, strict?: boolean): object;
}
