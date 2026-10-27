// export R# package module type define for javascript/typescript language
//
//    imports "kegg" from "gseakit";
//
// ref=gseakit.KEGG@gseakit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * the kegg background helper
 * 
*/
declare namespace kegg {
   /**
    * Get the kegg compound set from a kegg pathway map collection.
    * 
    * 
     * @param maps a collection of the KEGG pathway [Map](cref:T:SMRUCC.genomics.Assembly.KEGG.WebServices.XML.Map) object.
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return a ``list`` object, the name of each slot is the KEGG map id and the value is a named
     *  list of the compound set (the name is the compound id and the value is the compound name)
     *  of that map.
   */
   function compound_set(maps: any, env?: object): any;
   /**
    * Append the KEGG BRITE category annotation to a given dataframe by the KEGG id column.
    * 
    * > Only the first KEGG id will be used when the value of the ``kegg_id`` column contains
    * >  multiple ids that are separated by ``;``, ``/`` or other blank characters.
    * 
     * @param kegg the KEGG BRITE background model that is used for query the category information of
     *  the KEGG id, for example the model that is created by ``KO.background``.
     * @param anno the target dataframe that contains the KEGG id column, the annotation columns will be
     *  appended to this dataframe directly.
     * @param kegg_id the column name of the KEGG id in the **anno** dataframe. Default ``kegg_id``.
     * 
     * + default value Is ``'kegg_id'``.
     * @param class_field the column name of the output KEGG class information. Default ``kegg_class``.
     * 
     * + default value Is ``'kegg_class'``.
     * @param category_field the column name of the output KEGG category information. Default ``kegg_category``.
     * 
     * + default value Is ``'kegg_category'``.
     * @param env the R# runtime environment.
     * 
     * + default value Is ``null``.
     * @return the given dataframe with two new columns (``kegg_class`` and ``kegg_category``) appended,
     *  or an error message when the required KEGG id column is missing.
   */
   function kegg_category_annotation(kegg: object, anno: object, kegg_id?: string, class_field?: string, category_field?: string, env?: object): any;
}
