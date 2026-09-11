// export R# package module type define for javascript/typescript language
//
//    imports "circos" from "visualkit";
//
// ref=visualkit.circosTool@visualkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace circos {
   /**
   */
   function default_identity_colors(default: string): object;
   /**
     * @param depth default value Is ``10``.
     * @param default default value Is ``'Brown'``.
     * @param mapName default value Is ``'Jet'``.
   */
   function identity_colors(min: number, max: number, depth?: object, default?: string, mapName?: string): object;
}
