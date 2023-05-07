declare namespace rawXML {
   module open {
      /**
        * @param mode default value is ``'read'``.
        * @param args default value is ``null``.
        * @param env default value is ``null``.
      */
      function vcellXml(file:string, mode?:string, args?:any, env?:object): any;
   }
   module frame {
      /**
      */
      function index(raw:object): any;
      /**
        * @param env default value is ``null``.
      */
      function matrix(raw:string, tick:object, stream:any, env?:object): any;
   }
   module entity {
      /**
        * @param env default value is ``null``.
      */
      function names(raw:object, stream:any, env?:object): any;
   }
   module time {
      /**
        * @param env default value is ``null``.
      */
      function frames(raw:object, stream:any, env?:object): any;
   }
}
