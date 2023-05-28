// export R# package module type define for javascript/typescript language
//
//    imports "models" from "cytoscape_toolkit"
//
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
      */
      function sessionInfo(cys: object): object;
   }
   module list {
      /**
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
   */
   function sif(source: any, interaction: any, target: any): object;
}
