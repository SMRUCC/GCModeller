// export R# package module type define for javascript/typescript language
//
// ref=kegg_kit.report@kegg_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the kegg pathway map report helper tool
 * 
*/
declare namespace report.utils {
   /**
    * load a blank kegg pathway map template object from a given file object.
    * 
    * 
     * @param file a given file object, it can be a file path or a file input stream.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function loadMap(file:any, env?:object): object;
   module map {
      /**
      */
      function local_render(maps:object): object;
      /**
       * check object id that intersect with the current given map object.
       * 
       * 
        * @param map a given kegg pathway map object model
        * @param list an object id list
      */
      function intersects(map:object, list:string): string;
   }
   module nodes {
      /**
      */
      function colorAs(nodes:string, color:string): object;
   }
   module keggMap {
      /**
       * generate the kegg pathway map highlight image render result
       * 
       * 
        * @param map the blank template of the kegg map
        * @param highlights a list of object with color highlights, or url
        * @param text_color 
        * + default value Is ``'white'``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function highlights(map:object, highlights:any, text_color?:string, env?:object): any;
      /**
       * generate the kegg pathway map highlight html report
       * 
       * 
        * @param map the blank template of the kegg map
        * @param text_color 
        * + default value Is ``'white'``.
        * @param env 
        * + default value Is ``null``.
      */
      function reportHtml(map:object, highlights:any, text_color?:string, env?:object): any;
      /**
       * generate the url that used for view the highlight result on kegg website.
       * 
       * 
        * @param mapId the id of the map object.
        * @param highlights a list of object with color highlight specifics
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function url(mapId:string, highlights:any, env?:object): string;
   }
   /**
    * 
    * 
     * @param url -
     * @param compound the default color string for kegg compounds
     * 
     * + default value Is ``'blue'``.
     * @param gene the default color string for KO/gene
     * 
     * + default value Is ``'red'``.
     * @param reaction the default color string for kegg reactions
     * 
     * + default value Is ``'green'``.
   */
   function parseKeggUrl(url:string, compound?:string, gene?:string, reaction?:string): object;
}
