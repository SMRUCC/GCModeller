// export R# package module type define for javascript/typescript language
//
// ref=cytoscape_toolkit.TRN

/**
 * Transcription Regulation Network Builder Tools
 * 
*/
declare namespace bioModels.TRN {
   module fpkm {
      /**
        * @param cutoff default value is ``0.65``.
      */
      function connections(fpkm:object, cutoff?:number): any;
   }
}
