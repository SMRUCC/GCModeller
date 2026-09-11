// export R# package module type define for javascript/typescript language
//
//    imports "circos" from "visualkit";
//
// ref=visualkit.circosTool@visualkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace circos {
   /**
     * @param env default value Is ``null``.
   */
   function blastn_mapping(source: any, chrs: any, env?: object): object;
   /**
   */
   function default_identity_colors(default: string): object;
   /**
    * Creates the circos gene circle from the PTT database which is defined 
    *  in the ``*.ptt/*.rnt`` file, and you can download this directory from 
    *  the NCBI FTP website.
    * 
    * 
     * @param PTT -
     * @param COG -
     * @param defaultColor -
     * 
     * + default value Is ``'blue'``.
     * @param env 
     * + default value Is ``null``.
   */
   function genome_circle(PTT: object, COG: any, defaultColor?: string, env?: object): object;
   /**
     * @param depth default value Is ``10``.
     * @param default default value Is ``'Brown'``.
     * @param mapName default value Is ``'Jet'``.
   */
   function identity_colors(min: number, max: number, depth?: object, default?: string, mapName?: string): object;
   /**
     * @param steps default value Is ``2048``.
     * @param env default value Is ``null``.
   */
   function track_hits(source: any, karyotype: object, steps?: object, env?: object): object;
}
