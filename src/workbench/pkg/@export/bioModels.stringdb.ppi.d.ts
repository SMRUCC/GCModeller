declare namespace bioModels.stringdb.ppi {
   module as {
      /**
        * @param coordinates default value is ``null``.
        * @param env default value is ``null``.
      */
      function network(stringNetwork:object, uniprot:any, coordinates?:object, env?:object): any;
   }
   module read {
      /**
      */
      function string_interactions(string_interactions:string): any;
      /**
      */
      function coordinates(string_network_coordinates:string): any;
      /**
        * @param remove_taxonomyId default value is ``true``.
        * @param link_matrix default value is ``false``.
        * @param combine_score default value is ``-1``.
      */
      function string_db(file:string, remove_taxonomyId?:boolean, link_matrix?:boolean, combine_score?:number): any;
   }
}
