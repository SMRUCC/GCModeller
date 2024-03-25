// export R# package module type define for javascript/typescript language
//
//    imports "mesh" from "kb";
//
// ref=kb.meshTools@kb, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace mesh {
   /**
   */
   function mesh_category(term: object): object;
   module read {
      /**
        * @param as_tree default value Is ``true``.
        * @param env default value Is ``null``.
      */
      function mesh_tree(file: any, as_tree?: boolean, env?: object): object;
      /**
      */
      function mesh_xml(file: string): object;
   }
}
