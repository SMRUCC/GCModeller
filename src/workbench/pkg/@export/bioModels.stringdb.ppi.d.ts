// export R# package module type define for javascript/typescript language
//
//    imports "bioModels.stringdb.ppi" from "cytoscape";
//
// ref=cytoscape_toolkit.stringdbPPI@cytoscape, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace bioModels.stringdb.ppi {
   module as {
      /**
        * @param coordinates default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function network(stringNetwork: object, uniprot: any, coordinates?: object, env?: object): object;
   }
   module read {
      /**
      */
      function coordinates(string_network_coordinates: string): object;
      /**
        * @param remove_taxonomyId default value Is ``true``.
        * @param link_matrix default value Is ``false``.
        * @param combine_score default value Is ``-1``.
      */
      function string_db(file: string, remove_taxonomyId?: boolean, link_matrix?: boolean, combine_score?: number): object|object;
      /**
      */
      function string_interactions(string_interactions: string): object;
   }
}
