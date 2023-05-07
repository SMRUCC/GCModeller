// export R# package module type define for javascript/typescript language
//
// ref=cytoscape_toolkit.stringWebApi

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
           * @param EXPORT default value is ``'./'``.
         */
         function interaction(idList:object, EXPORT?:string): any;
      }
      module genomics {
         /**
           * @param EXPORT default value is ``'./'``.
         */
         function interactions(PTT:object, EXPORT?:string): any;
      }
   }
}
