// export R# package module type define for javascript/typescript language
//
//    imports "property_edit" from "vcellkit";
//
// ref=vcellkit.EditorAPI@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace property_edit {
   /**
   */
   function add_author(model: object, value: string): object;
   /**
   */
   function add_email(model: object, value: string): object;
   module Publication {
      /**
      */
      function Add(model: object, value: string): object;
   }
   module Reversion {
      /**
      */
      function Plus(model: object): object;
   }
   module Url {
      /**
      */
      function Add(model: object, value: string): object;
   }
   module write {
      /**
      */
      function comment(model: object, value: string): object;
      /**
      */
      function name(model: object, value: string): object;
      /**
      */
      function species(model: object, value: string): object;
      /**
      */
      function title(model: object, value: string): object;
   }
   module Write {
      /**
      */
      function DBLinks(model: object, value: object): object;
   }
}
