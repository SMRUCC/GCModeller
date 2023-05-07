declare namespace models {
   /**
   */
   function sif(source:any, interaction:any, target:any): any;
   /**
     * @param env default value is ``null``.
   */
   function cyjs(network:any, env?:object): any;
   module as {
      /**
        * @param propertyNames default value is ``["label","class","group.category","group.category.color"]``.
        * @param env default value is ``null``.
      */
      function graph(model:any, propertyNames?:any, env?:object): any;
   }
   module open {
      /**
      */
      function cys(cys:string): any;
   }
   module get {
      /**
      */
      function sessionInfo(cys:object): any;
      /**
        * @param collection default value is ``null``.
        * @param name default value is ``null``.
      */
      function network_graph(cys:object, collection?:string, name?:string): any;
   }
   module list {
      /**
      */
      function networks(cys:object): any;
   }
}
