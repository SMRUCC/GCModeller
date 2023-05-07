declare namespace repository {
   /**
   */
   function enzyme_description(): any;
   module write {
      /**
        * @param env default value is ``null``.
      */
      function msgpack(data:any, file:any, env?:object): any;
   }
   module load {
      /**
        * @param rawList default value is ``false``.
        * @param ignoreGlycan default value is ``false``.
        * @param env default value is ``null``.
      */
      function compounds(repository:any, rawList?:boolean, ignoreGlycan?:boolean, env?:object): any;
      /**
        * @param raw default value is ``true``.
        * @param env default value is ``null``.
      */
      function reactions(repository:any, raw?:boolean, env?:object): any;
      /**
        * @param rawMaps default value is ``true``.
      */
      function maps(repository:any, rawMaps?:boolean): any;
      /**
        * @param referenceMap default value is ``true``.
        * @param env default value is ``null``.
      */
      function pathways(repository:string, referenceMap?:boolean, env?:object): any;
   }
   module reactions {
      /**
        * @param env default value is ``null``.
      */
      function table(repo:any, env?:object): any;
   }
   /**
     * @param env default value is ``null``.
   */
   function reactionsId(repo:any, env?:object): any;
   /**
     * @param env default value is ``null``.
   */
   function compoundsId(repo:any, env?:object): any;
   module reaction_class {
      /**
        * @param env default value is ``null``.
      */
      function repo(repo:any, env?:object): any;
      /**
        * @param env default value is ``null``.
      */
      function table(repo:any, env?:object): any;
   }
   module fetch {
      /**
        * @param resource default value is ``'http://www.kegg.jp/kegg/catalog/org_list.html'``.
        * @param type default value is ``null``.
      */
      function kegg_organism(resource?:string, type?:object): any;
   }
   module save {
      /**
        * @param env default value is ``null``.
      */
      function KEGG_pathway(pathway:any, file:string, env?:object): any;
      /**
        * @param env default value is ``null``.
      */
      function kegg_organism(organism:object, file:string, env?:object): any;
   }
   module read {
      /**
        * @param env default value is ``null``.
      */
      function KEGG_pathway(file:string, env?:object): any;
      /**
      */
      function kegg_organism(file:string): any;
   }
   /**
   */
   function compound(entry:string, name:string, formula:string, exactMass:number, reaction:string, enzyme:string, remarks:string, KCF:string, DBLinks:object, pathway:object, modules:object): any;
   /**
     * @param env default value is ``null``.
   */
   function pathway(id:string, name:string, description:string, modules:object, DBLinks:object, KO_pathway:string, references:object, compounds:object, drugs:object, genes:object, organism:object, disease:object, env?:object): any;
   /**
   */
   function reaction(id:string, name:string, definition:string, equation:string, comment:string, reaction_class:object, enzyme:string, pathways:object, modules:object, KO:object, links:object): any;
   /**
   */
   function shapeAreas(data:object): any;
   /**
   */
   function keggMap(id:string, name:string, description:string, img:string, url:string, area:object): any;
}
