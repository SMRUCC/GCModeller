// export R# package module type define for javascript/typescript language
//
//    imports "property_edit" from "vcellkit";
//
// ref=vcellkit.EditorAPI@vcellkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Edit the model metadata
 * 
*/
declare namespace property_edit {
   /**
    * add author into model
    * 
    * 
     * @param model -
     * @param value -
   */
   function add_author(model: object, value: string): object;
   /**
    * add e-mail information about the author inside model
    * 
    * 
     * @param model -
     * @param value -
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
       * write comment text into the model
       * 
       * 
        * @param model -
        * @param value -
      */
      function comment(model: object, value: string): object;
      /**
       * set model name
       * 
       * 
        * @param model -
        * @param value -
      */
      function name(model: object, value: string): object;
      /**
       * write organism species information into the model
       * 
       * 
        * @param model -
        * @param value -
      */
      function species(model: object, value: string): object;
      /**
       * write the data title into the model
       * 
       * 
        * @param model -
        * @param value -
      */
      function title(model: object, value: string): object;
   }
   module Write {
      /**
      */
      function DBLinks(model: object, value: object): object;
   }
}
