// export R# package module type define for javascript/typescript language
//
//    imports "bioModels.stringdb.ppi" from "cytoscape";
//    imports "bioModels.stringdb.ppi" from "cytoscape_toolkit";
//
// ref=cytoscape_toolkit.stringdbPPI@cytoscape, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=cytoscape_toolkit.stringdbPPI@cytoscape_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * protein-protein interaction network model from string-db
 * 
*/
declare namespace bioModels.stringdb.ppi {
   module as {
      /**
       * export string-db interaction result set as graph
       * 
       * 
        * @param stringNetwork -
        * @param uniprot ``STRING -> uniprot``
        * @param coordinates -
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function network(stringNetwork: object, uniprot: any, coordinates?: object, env?: object): object;
   }
   module read {
      /**
      */
      function coordinates(string_network_coordinates: string): object;
      /**
       * parse the string-db table file
       * 
       * 
        * @param file the string db protein links data files, example like:
        *  
        *  1. 9606.protein.links.v11.5.txt
        *  2. 9606.protein.links.full.v11.5.txt
        *  3. 9606.protein.links.detailed.v11.5.txt
        * @param remove_taxonomyId 
        * + default value Is ``true``.
        * @param link_matrix 
        * + default value Is ``false``.
        * @param combine_score 
        * + default value Is ``-1``.
      */
      function string_db(file: string, remove_taxonomyId?: boolean, link_matrix?: boolean, combine_score?: number): object|object;
      /**
      */
      function string_interactions(string_interactions: string): object;
   }
}
