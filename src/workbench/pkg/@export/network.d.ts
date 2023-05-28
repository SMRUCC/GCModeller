// export R# package module type define for javascript/typescript language
//
//    imports "network" from "kegg_kit"
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
     * @param compounds default value Is ``null``.
     * @param enzymeBridged default value Is ``true``.
   */
   function fromCompounds(compoundsId: string, graph: object, compounds?: object, enzymeBridged?: boolean): object;
}
