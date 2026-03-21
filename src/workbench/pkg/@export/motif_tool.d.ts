// export R# package module type define for javascript/typescript language
//
//    imports "motif_tool" from "TRNtoolkit";
//
// ref=TRNtoolkit.MotifsTool@TRNtoolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace motif_tool {
   /**
    * read meme motif text file
    * 
    * 
     * @param file file path to the meme motif text file(*.meme)
   */
   function read_meme(file: string): object;
}
