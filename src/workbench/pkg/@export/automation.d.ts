// export R# package module type define for javascript/typescript language
//
// ref=cytoscape_toolkit.automation@cytoscape_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace automation {
   /**
     * @param version default value Is ``'v1'``.
     * @param port default value Is ``1234``.
     * @param host default value Is ``'localhost'``.
   */
   function finalize(version?:string, port?:object, host?:string): ;
   /**
     * @param algorithmName default value Is ``'force-directed'``.
     * @param version default value Is ``'v1'``.
     * @param port default value Is ``1234``.
     * @param host default value Is ``'localhost'``.
     * @param env default value Is ``null``.
   */
   function layout(networkId:any, algorithmName?:string, version?:string, port?:object, host?:string, env?:object): any;
   /**
    * GET list of layout algorithms
    * 
    * 
     * @param version 
     * + default value Is ``'v1'``.
     * @param port 
     * + default value Is ``1234``.
     * @param host 
     * + default value Is ``'localhost'``.
   */
   function layouts(version?:string, port?:object, host?:string): string;
   /**
     * @param version default value Is ``'v1'``.
     * @param port default value Is ``1234``.
     * @param host default value Is ``'localhost'``.
     * @param env default value Is ``null``.
   */
   function networkView(networkId:any, viewId:any, version?:string, port?:object, host?:string, env?:object): object;
   /**
     * @param collection default value Is ``null``.
     * @param title default value Is ``null``.
     * @param version default value Is ``'v1'``.
     * @param port default value Is ``1234``.
     * @param host default value Is ``'localhost'``.
     * @param env default value Is ``null``.
   */
   function put_network(network:any, collection?:string, title?:string, version?:string, port?:object, host?:string, env?:object): object;
   module session {
      /**
        * @param version default value Is ``'v1'``.
        * @param port default value Is ``1234``.
        * @param host default value Is ``'localhost'``.
      */
      function save(file:string, version?:string, port?:object, host?:string): any;
   }
   /**
     * @param version default value Is ``'v1'``.
     * @param port default value Is ``1234``.
     * @param host default value Is ``'localhost'``.
   */
   function view(version?:string, port?:object, host?:string): object;
}
