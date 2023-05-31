// export R# package module type define for javascript/typescript language
//
//    imports "stringWebApi" from "cytoscape_toolkit";
//
// ref=cytoscape_toolkit.stringWebApi@cytoscape_toolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * STRING has an application programming interface (API) which enables you to get the data without using the 
 *  graphical user interface of the web page. The API is convenient if you need to programmatically access some 
 *  information but still do not want to download the entire dataset. There are several scenarios when it is 
 *  practical to use it. For example, you might need to access some interaction from your own scripts or want to 
 *  incorporate some information in STRING to a web page.
 * 
 *  We currently provide an implementation using HTTP, where the database information is accessed by HTTP requests. 
 *  Due to implementation and licensing reasons, The API provide methods to query individual items only, similar to 
 *  the web site. If you need access to bulk data, you can download the entire dataset by signing the academic 
 *  license agreement.
 *  
 *  http://[database]/[access]/[format]/[request]?[parameter]=[value]
 * 
*/
declare namespace stringWebApi {
   module downloads {
      module gene_ids {
         /**
          * 
          * > http://string-db.org/api/psi-mi/interactions?identifier=XC_1184
          * 
           * @param idList -
           * @param EXPORT 
           * + default value Is ``'./'``.
           * @return 返回成功的个数
         */
         function interaction(idList: object, EXPORT?: string): object;
      }
      module genomics {
         /**
          * 
          * > http://string-db.org/api/psi-mi/interactions?identifier=XC_1184
          * 
           * @param EXPORT 
           * + default value Is ``'./'``.
           * @return 返回成功的个数
         */
         function interactions(PTT: object, EXPORT?: string): object;
      }
   }
}
