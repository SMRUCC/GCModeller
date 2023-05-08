// export R# package module type define for javascript/typescript language
//
// ref=cytoscape_toolkit.PathVisio@cytoscape_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace PathVisio {
   module read {
      /**
        * @param env default value Is ``null``.
      */
      function gpml(file:string, env?:object): object;
   }
   module nodes {
      /**
      */
      function table(pathway:object): object;
   }
   module as {
      /**
      */
      function graph(pathway:object): object;
   }
}
