// export R# package module type define for javascript/typescript language
//
//    imports "models" from "cytoscape";
//    imports "models" from "cytoscape_toolkit";
//
// ref=cytoscape_toolkit.models@cytoscape, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// ref=cytoscape_toolkit.models@cytoscape_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * api for create network graph model for cytoscape
 * 
*/
declare namespace models {
   module as {
      /**
       * convert the cytoscape cyjs/xgmml file to network graph model.
       * 
       * 
        * @param model -
        * @param propertyNames -
        * 
        * + default value Is ``["label","class","group.category","group.category.color"]``.
        * @param env 
        * + default value Is ``null``.
      */
      function graph(model: any, propertyNames?: any, env?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function cyjs(network: any, env?: object): object;
   module get {
      /**
        * @param collection default value Is ``null``.
        * @param name default value Is ``null``.
      */
      function network_graph(cys: object, collection?: string, name?: string): object;
      /**
       * get session information about current session file
       * 
       * > get session information from @``T:SMRUCC.genomics.Visualize.Cytoscape.Session.cyTables`` xml file
       * 
        * @param cys -
      */
      function sessionInfo(cys: object): object;
   }
   module list {
      /**
       * list of the network id inside current cytoscape session file
       * 
       * 
        * @param cys -
      */
      function networks(cys: object): object;
   }
   module open {
      /**
       * open a new cytoscape session file reader
       * 
       * 
        * @param cys -
      */
      function cys(cys: string): object;
   }
   /**
    * create sif network
    * 
    * 
     * @param source a character vector of the source id
     * @param interaction a character vector of the iteraction type labels
     * @param target a character vector of the target id
     * @return a simple network graph which consist with a set of the simple iteraction links.
   */
   function sif(source: any, interaction: any, target: any): object;
}
