// export R# package module type define for javascript/typescript language
//
// ref=cytoscape_toolkit.automation

/**
 * 
*/
declare namespace automation {
   /**
     * @param version default value is ``'v1'``.
     * @param port default value is ``1234``.
     * @param host default value is ``'localhost'``.
   */
   function layouts(version?:string, port?:object, host?:string): any;
   /**
     * @param collection default value is ``null``.
     * @param title default value is ``null``.
     * @param version default value is ``'v1'``.
     * @param port default value is ``1234``.
     * @param host default value is ``'localhost'``.
     * @param env default value is ``null``.
   */
   function put_network(network:any, collection?:string, title?:string, version?:string, port?:object, host?:string, env?:object): any;
   /**
     * @param algorithmName default value is ``'force-directed'``.
     * @param version default value is ``'v1'``.
     * @param port default value is ``1234``.
     * @param host default value is ``'localhost'``.
     * @param env default value is ``null``.
   */
   function layout(networkId:any, algorithmName?:string, version?:string, port?:object, host?:string, env?:object): any;
   module session {
      /**
        * @param version default value is ``'v1'``.
        * @param port default value is ``1234``.
        * @param host default value is ``'localhost'``.
      */
      function save(file:string, version?:string, port?:object, host?:string): any;
   }
   /**
     * @param version default value is ``'v1'``.
     * @param port default value is ``1234``.
     * @param host default value is ``'localhost'``.
   */
   function view(version?:string, port?:object, host?:string): any;
   /**
     * @param version default value is ``'v1'``.
     * @param port default value is ``1234``.
     * @param host default value is ``'localhost'``.
     * @param env default value is ``null``.
   */
   function networkView(networkId:any, viewId:any, version?:string, port?:object, host?:string, env?:object): any;
   /**
     * @param version default value is ``'v1'``.
     * @param port default value is ``1234``.
     * @param host default value is ``'localhost'``.
   */
   function finalize(version?:string, port?:object, host?:string): any;
}
