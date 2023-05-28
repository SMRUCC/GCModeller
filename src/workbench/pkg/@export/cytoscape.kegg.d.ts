// export R# package module type define for javascript/typescript language
//
//    imports "cytoscape.kegg" from "cytoscape_toolkit"
//
// ref=cytoscape_toolkit.kegg@cytoscape_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The KEGG metabolism pathway network data R# scripting plugin for cytoscape software
 * 
*/
declare namespace cytoscape.kegg {
   module compounds {
      /**
       * Create kegg metabolism network based on the given metabolite compound data.
       * 
       * 
        * @param reactions The kegg ``br08201`` reaction database.
        * @param compounds Kegg compound id list
        * @param enzymes 
        * + default value Is ``null``.
        * @param filterByEnzymes 
        * + default value Is ``false``.
        * @param extended 
        * + default value Is ``false``.
        * @param strictReactionNetwork 
        * + default value Is ``false``.
        * @param enzymeBridged 
        * + default value Is ``true``.
        * @param random_layout 
        * + default value Is ``true``.
      */
      function network(reactions: object, compounds: string, enzymes?: object, filterByEnzymes?: boolean, extended?: boolean, strictReactionNetwork?: boolean, enzymeBridged?: boolean, random_layout?: boolean): object;
   }
   /**
    * assign pathway map id to the nodes in the given network graph
    * 
    * 
     * @param graph the node vertex in this network graph object its label value 
     *  could be one of: glycan, compound, kegg ortholog or reaction id
     * @param maps -
     * @param top3 -
     * 
     * + default value Is ``true``.
     * @param excludesGlobalAndOverviewMaps 
     * + default value Is ``true``.
   */
   function pathway_class(graph: object, maps: object, top3?: boolean, excludesGlobalAndOverviewMaps?: boolean): object;
}
