declare namespace report.utils {
   /**
     * @param env default value is ``null``.
   */
   function loadMap(file:any, env?:object): any;
   module map {
      /**
      */
      function local_render(maps:object): any;
      /**
      */
      function intersects(map:object, list:string): any;
   }
   module nodes {
      /**
      */
      function colorAs(nodes:string, color:string): any;
   }
   module keggMap {
      /**
        * @param text_color default value is ``'white'``.
        * @param env default value is ``null``.
      */
      function highlights(map:object, highlights:any, text_color?:string, env?:object): any;
      /**
        * @param text_color default value is ``'white'``.
        * @param env default value is ``null``.
      */
      function reportHtml(map:object, highlights:any, text_color?:string, env?:object): any;
      /**
        * @param env default value is ``null``.
      */
      function url(mapId:string, highlights:any, env?:object): any;
   }
   /**
     * @param compound default value is ``'blue'``.
     * @param gene default value is ``'red'``.
     * @param reaction default value is ``'green'``.
   */
   function parseKeggUrl(url:string, compound?:string, gene?:string, reaction?:string): any;
}
