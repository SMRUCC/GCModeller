// export R# package module type define for javascript/typescript language
//
//    imports "metabolic_network" from "annotationKit";
//
// ref=annotationKit.metabolicNetwork@annotationKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace metabolic_network {
   /**
   */
   function find_pathway(router: object, target: string): object;
   /**
     * @param env default value Is ``null``.
   */
   function router(compounds: any, reactions: any, env?: object): object;
}
