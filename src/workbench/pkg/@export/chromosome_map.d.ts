// export R# package module type define for javascript/typescript language
//
//    imports "chromosome_map" from "visualkit";
//
// ref=visualkit.chromosome_map@visualkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * chromosome map visualize for bacterial genome
 * 
*/
declare namespace chromosome_map {
   /**
    * load configuration file or create default configuration.
    * 
    * 
     * @param conf -
     * 
     * + default value Is ``null``.
   */
   function config(conf?: string): object;
   /**
    * 
    * 
     * @param genome -
     * @param config -
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function draw(genome: any, config?: any, env?: object): any;
}
