// export R# package module type define for javascript/typescript language
//
// ref=cytoscape_toolkit.models@cytoscape_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * api for create network graph model for cytoscape
 * 
*/
declare namespace models {
   /**
   */
   function sif(source: any, interaction: any, target: any): object;
   /**
     * @param env default value Is ``null``.
   */
   function cyjs(network: any, env?: object): object;
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
   module open {
      /**
       * open a new cytoscape session file reader
       * 
       * 
        * @param cys -
      */
      function cys(cys: string): object;
   }
   module get {
      /**
      */
      function sessionInfo(cys: object): object;
      /**
        * @param collection default value Is ``null``.
        * @param name default value Is ``null``.
      */
      function network_graph(cys: object, collection?: string, name?: string): object;
   }
   module list {
      /**
      */
      function networks(cys: object): object;
   }
}
