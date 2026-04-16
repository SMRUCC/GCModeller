// export R# package module type define for javascript/typescript language
//
//    imports "rawXML" from "vcellkit";
//
// ref=vcellkit.RawXmlKit@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the virtual cell raw data
 * 
 * > the combination of the stream frame data in the rawpack file:
 * >  
 * >  mass profile:
 * >  
 * >  + transcriptome -> mass_profile
 * >  + proteome -> mass_profile
 * >  + metabolome -> mass_profile
 * >  
 * >  flux profile:
 * >  
 * >  + transcriptome -> activity
 * >  + proteome -> activity
 * >  + metabolome -> flux_size
*/
declare namespace rawXML {
   module entity {
      /**
       * 
       * 
        * @param raw -
        * @param stream module descripting of the stream content to read, should be a list of content type mapping:
        *  list element name could be: "transcriptome", "proteome", "metabolome"
        *  element content type could be: mass_profile, activity, flux_size
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function names(raw: object, stream: any, env?: object): string;
   }
   module frame {
      /**
       * [debug api]
       * 
       * 
        * @param raw -
      */
      function index(raw: object): object;
      /**
       * get a frame matrix for compares between different samples.
       * 
       * 
        * @param raw -
        * @param tick -
        * @param stream the frame stream reference to the matrix data, should be one of the module type:
        *  
        *  transcriptome/proteome/metabolome
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function matrix(raw: string, tick: object, stream: any, env?: object): any;
   }
   module open {
      /**
       * open the simulation data storage driver
       * 
       * 
        * @param file the file path to the storage data
        * @param mode the binary file open mode for the data storage driver, should be ``read``/``write``.
        * 
        * + default value Is ``'read'``.
        * @param args -
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function vcellPack(file: string, mode?: string, args?: any, env?: object): object|object;
      /**
       * open gcXML raw data file for read/write
       * 
       * 
        * @param mode 
        * + default value Is ``'read'``.
        * @param args -
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function vcellXml(file: string, mode?: string, args?: any, env?: object): object|object;
   }
   module time {
      /**
       * Get a sample matrix data in a timeline.
       * 
       * 
        * @param raw -
        * @param symbol_name prefer the symbol name for export matrix data?
        * 
        * + default value Is ``false``.
        * @param stream -
        * 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function frames(raw: any, symbol_name?: boolean, stream?: any, env?: object): object;
   }
}
