// export R# package module type define for javascript/typescript language
//
//    imports "rawXML" from "vcellkit";
//
// ref=vcellkit.RawXmlKit@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace rawXML {
   module entity {
      /**
        * @param env default value Is ``null``.
      */
      function names(raw: object, stream: any, env?: object): string;
   }
   module frame {
      /**
      */
      function index(raw: object): object;
      /**
        * @param env default value Is ``null``.
      */
      function matrix(raw: string, tick: object, stream: any, env?: object): any;
   }
   module open {
      /**
        * @param mode default value Is ``'read'``.
        * @param args default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function vcellPack(file: string, mode?: string, args?: any, env?: object): object|object;
      /**
        * @param mode default value Is ``'read'``.
        * @param args default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function vcellXml(file: string, mode?: string, args?: any, env?: object): object|object;
   }
   module time {
      /**
        * @param symbol_name default value Is ``false``.
        * @param stream default value Is ``null``.
        * @param env default value Is ``null``.
      */
      function frames(raw: any, symbol_name?: boolean, stream?: any, env?: object): object;
   }
}
