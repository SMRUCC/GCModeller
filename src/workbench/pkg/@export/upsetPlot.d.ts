// export R# package module type define for javascript/typescript language
//
// ref=visualkit.upsetPlot@visualkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace upsetPlot {
   module as {
      /**
        * @param desc default value Is ``true``.
        * @param intersectionCut default value Is ``0``.
      */
      function upsetData(vennSet: object, desc?: boolean, intersectionCut?: object): object;
   }
}
