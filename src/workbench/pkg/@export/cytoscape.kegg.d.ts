declare namespace cytoscape.kegg {
   module compounds {
      /**
        * @param enzymes default value is ``null``.
        * @param filterByEnzymes default value is ``false``.
        * @param extended default value is ``false``.
        * @param strictReactionNetwork default value is ``false``.
        * @param enzymeBridged default value is ``true``.
        * @param random_layout default value is ``true``.
      */
      function network(reactions:object, compounds:string, enzymes?:object, filterByEnzymes?:boolean, extended?:boolean, strictReactionNetwork?:boolean, enzymeBridged?:boolean, random_layout?:boolean): any;
   }
   /**
     * @param top3 default value is ``true``.
     * @param excludesGlobalAndOverviewMaps default value is ``true``.
   */
   function pathway_class(graph:object, maps:object, top3?:boolean, excludesGlobalAndOverviewMaps?:boolean): any;
}
