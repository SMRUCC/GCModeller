// export R# package module type define for javascript/typescript language
//
//    imports "models" from "cytoscape";
//
// ref=cytoscape_toolkit.models@cytoscape, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace models {
   module as {
      /**
        * @param propertyNames default value Is ``["label","class","group.category","group.category.color"]``.
        * @param env default value Is ``null``.
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
      */
      function cys(cys: string): object;
   }
   /**
   */
   function sif(source: any, interaction: any, target: any): object;
}
