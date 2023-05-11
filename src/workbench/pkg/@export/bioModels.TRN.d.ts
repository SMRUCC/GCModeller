// export R# package module type define for javascript/typescript language
//
// ref=cytoscape_toolkit.TRN@cytoscape_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Transcription Regulation Network Builder Tools
 * 
*/
declare namespace bioModels.TRN {
   module fpkm {
      /**
        * @param cutoff default value Is ``0.65``.
      */
      function connections(fpkm: object, cutoff?: number): object;
   }
}
