// export R# package module type define for javascript/typescript language
//
//    imports "bioModels.TRN" from "cytoscape";
//
// ref=cytoscape_toolkit.TRN@cytoscape, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace bioModels.TRN {
   module fpkm {
      /**
        * @param cutoff default value Is ``0.65``.
      */
      function connections(fpkm: object, cutoff?: number): object;
   }
}
