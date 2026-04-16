// export R# package module type define for javascript/typescript language
//
//    imports "report.utils" from "kegg_kit";
//
// ref=kegg_kit.report@kegg_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the kegg pathway map report helper tool
 * 
*/
declare namespace report.utils {
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
      function highlights(map: object, highlights: any, text_color?: string, env?: object): any;
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
      function reportHtml(map: object, highlights: any, text_color?: string, env?: object): any;
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
      function url(mapId: string, highlights: any, env?: object): string;
   }
   /**
    * load a blank kegg pathway map template object from a given file object.
    * 
    * 
     * @param file a given file object, it can be a file path or a file input stream.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function loadMap(file: any, env?: object): object;
   module map {
      /**
       * check object id that intersect with the current given map object.
       * 
       * 
        * @param map a given kegg pathway map object model
        * @param list an object id list
      */
      function intersects(map: object, list: string): string;
      /**
      */
      function local_render(maps: object): object;
   }
   /**
   */
   function node_images(dir: string): object;
   module nodes {
      /**
      */
      function colorAs(nodes: string, color: string): object;
   }
   module parse {
      /**
       * Parse the kegg pathway node highlight information
       * 
       * 
        * @param x should be a character string that contains the pathway nodes id and 
        *  optional highligh color for make the kegg pathway map rendering.
        *  
        *  value should be in formats of:
        *  
        *      "K00001:blue;K00002:red;C00001:green"
        *  
        *  where the first part is the kegg id, the second part is the highlight color.
        *  
        *  or just a list of the kegg id without highlight color, such as:
        *  
        *      "K00001;K00002;C00001"
        *  
        *  The default color is "red" if the highlight color is not specified.
        * @param default the default color for the hightlights
        * 
        * + default value Is ``'red'``.
        * @return a tuple list that contains the highlight information, such as:
        *  
        *  ```r
        *  list(K00001 = "blue", K00002 = "red", C00001 = "green");
        *  ```
      */
      function highlight_tuples(x: string, default?: string): any;
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
   function parseKeggUrl(url: string, compound?: string, gene?: string, reaction?: string): object;
   /**
     * @param size default value Is ``[3000,3000]``.
     * @param env default value Is ``null``.
   */
   function render_kgml(kgml: object, images: object, size?: any, env?: object): object;
}
