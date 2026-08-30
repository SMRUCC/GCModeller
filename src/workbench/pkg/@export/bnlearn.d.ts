// export R# package module type define for javascript/typescript language
//
//    imports "bnlearn" from "biosystem";
//
// ref=biosystem.bnlearn@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace bnlearn {
   module as {
      /**
        * @param env default value Is ``null``.
      */
      function prior_net(priorNet: any, env?: object): object;
   }
   /**
     * @param priorNet default value Is ``null``.
     * @param max_itrs default value Is ``500``.
     * @param strict default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function bnlearn(exprData: object, priorNet?: any, max_itrs?: object, strict?: object, env?: object): object;
   /**
   */
   function knockdown(bnlearn: object, geneNames: any): object;
   /**
   */
   function knockouts(bnlearn: object, geneNames: any): object;
   /**
     * @param pathway_info default value Is ``null``.
     * @param top_n default value Is ``50``.
     * @param env default value Is ``null``.
   */
   function make_exports(results: any, dir: string, pathway_info?: object, top_n?: object, env?: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function overexpress(bnlearn: object, geneNames: any, env?: object): object;
   /**
   */
   function prior_network(TF: any, target_gene: any, regulation_type: any, confidence: any, evidence: any): object;
   /**
    * save bnlearn model
    * 
    * 
     * @param bnlearn -
     * @param dir -
   */
   function save_model(bnlearn: object, dir: string): any;
}
