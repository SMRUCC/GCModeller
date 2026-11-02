// export R# package module type define for javascript/typescript language
//
//    imports "cytoscape.kegg" from "cytoscape";
//
// ref=cytoscape_toolkit.kegg@cytoscape, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace cytoscape.kegg {
   module compounds {
      /**
        * @param enzymes default value Is ``null``.
        * @param filterByEnzymes default value Is ``false``.
        * @param extended default value Is ``false``.
        * @param strictReactionNetwork default value Is ``false``.
        * @param enzymeBridged default value Is ``true``.
        * @param random_layout default value Is ``true``.
      */
      function network(reactions: object, compounds: string, enzymes?: object, filterByEnzymes?: boolean, extended?: boolean, strictReactionNetwork?: boolean, enzymeBridged?: boolean, random_layout?: boolean): object;
   }
   /**
     * @param top3 default value Is ``true``.
     * @param excludesGlobalAndOverviewMaps default value Is ``true``.
   */
   function pathway_class(graph: object, maps: object, top3?: boolean, excludesGlobalAndOverviewMaps?: boolean): object;
}
