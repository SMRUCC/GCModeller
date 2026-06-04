// export R# package module type define for javascript/typescript language
//
//    imports "bnlearn" from "phenotype_kit";
//
// ref=phenotype_kit.bnlearn@phenotype_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace bnlearn {
   /**
     * @param priorNet default value Is ``null``.
     * @param max_itrs default value Is ``500``.
     * @param env default value Is ``null``.
   */
   function bnlearn(exprData: object, priorNet?: any, max_itrs?: object, env?: object): object;
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
   */
   function overexpress(bnlearn: object, geneNames: any): object;
   /**
   */
   function prior_network(TF: any, target_gene: any, regulation_type: any, confidence: any, evidence: any): object;
}
