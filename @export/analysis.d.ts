// export R# package module type define for javascript/typescript language
//
//    imports "analysis" from "vcellkit";
//
// ref=vcellkit.Analysis@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace analysis {
   module compound {
      /**
      */
      function names(metabolites: object, names: object): object;
   }
   module union {
      /**
        * @param setName default value Is ``'mass\metabolome.json'``.
        * @param trim default value Is ``true``.
      */
      function matrix(result: string, setName?: string, trim?: boolean): object;
   }
   module vcell {
      /**
        * @param env default value Is ``null``.
      */
      function graph(vcell: any, env?: object): object;
   }
}
