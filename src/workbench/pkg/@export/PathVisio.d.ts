// export R# package module type define for javascript/typescript language
//
// ref=cytoscape_toolkit.PathVisio

/**
*/
declare namespace PathVisio {
   module read {
      /**
        * @param env default value is ``null``.
      */
      function gpml(file:string, env?:object): any;
   }
   module nodes {
      /**
      */
      function table(pathway:object): any;
   }
   module as {
      /**
      */
      function graph(pathway:object): any;
   }
}
