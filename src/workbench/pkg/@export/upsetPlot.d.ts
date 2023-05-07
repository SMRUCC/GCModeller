// export R# package module type define for javascript/typescript language
//
// ref=visualkit.upsetPlot

/**
 * UpSet plot provides an efficient way to 
 *  visualize intersections of multiple sets 
 *  compared to the traditional approaches, 
 *  i.e. the Venn Diagram.
 * 
*/
declare namespace upsetPlot {
   module as {
      /**
        * @param desc default value is ``true``.
        * @param intersectionCut default value is ``0``.
      */
      function upsetData(vennSet:object, desc?:boolean, intersectionCut?:object): any;
   }
}
