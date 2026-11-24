// export R# package module type define for javascript/typescript language
//
//    imports "network" from "kegg_kit";
//
// ref=kegg_kit.network@kegg_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace network {
   /**
    * assign kegg class to the graph nodes
    * 
    * 
     * @param g a network graph data model, with nodes label id must 
     *  be the kegg pathway id and compounds id or kegg KO id
   */
   function assignKeggClass(g: object): object;
   /**
    * create metabolism graph from a given set of compounds
    * 
    * 
     * @param compoundsId -
     * @param graph -
     * @param compounds -
     * 
     * + default value Is ``null``.
     * @param enzymeBridged -
     * 
     * + default value Is ``true``.
   */
   function fromCompounds(compoundsId: string, graph: object, compounds?: object, enzymeBridged?: boolean): object;
   /**
   */
   function fromKGML(kgml: object): object;
   /**
     * @param cor default value Is ``null``.
     * @param modules default value Is ``null``.
     * @param names default value Is ``null``.
     * @param leave_blankName default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function gsva_network(gsva: any, diff_exprs: any, model: object, cor?: object, modules?: any, names?: object, leave_blankName?: boolean, env?: object): any;
}
