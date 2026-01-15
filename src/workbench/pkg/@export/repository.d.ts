// export R# package module type define for javascript/typescript language
//
//    imports "repository" from "kegg_kit";
//
// ref=kegg_kit.repository@kegg_kit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The kegg data model repository tool
 * 
*/
declare namespace repository {
   /**
    * construct a new kegg compound data model
    * 
    * 
     * @param entry the kegg compound id
     * @param name the compound common names
     * @param formula the chemical formula of the kegg compound
     * @param exactMass the evaluated exact mass value based on the formula string
     * @param reaction A set of the related kegg reaction data about current metabolite
     * 
     * + default value Is ``null``.
     * @param enzyme A set of the related enzyme that associated with the reaction list
     * 
     * + default value Is ``null``.
     * @param remarks comment text about this metabolite
     * 
     * + default value Is ``null``.
     * @param KCF the molecular strucutre text
     * 
     * + default value Is ``null``.
     * @param DBLinks A dataframe object that contains the external reference link to 
     *  other database of current metabolite object, data fields: "db", "id", "link" should 
     *  be exists in this dataframe object.
     * 
     * + default value Is ``null``.
     * @param pathway -
     * 
     * + default value Is ``null``.
     * @param modules -
     * 
     * + default value Is ``null``.
   */
   function compound(entry: string, name: string, formula: string, exactMass: number, reaction?: string, enzyme?: string, remarks?: string, KCF?: string, DBLinks?: object, pathway?: object, modules?: object): object;
   /**
    * get a vector of kegg compound id from the kegg reaction_class/pathway maps data repository
    * 
    * 
     * @param repo -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function compoundsId(repo: any, env?: object): any;
   /**
    * Get the class description category data for EC number
    * 
    * 
   */
   function enzyme_description(): any;
   module fetch {
      /**
       * Fetch the kegg organism table data from a given resource
       * 
       * 
        * @param resource the kegg organism data resource, by default is the kegg online page data.
        * 
        * + default value Is ``'http://www.kegg.jp/kegg/catalog/org_list.html'``.
        * @param type 0. all
        *  1. prokaryote
        *  2. eukaryotes
        * 
        * + default value Is ``null``.
      */
      function kegg_organism(resource?: string, type?: object): object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function index(repo: any, env?: object): any;
   /**
   */
   function keggMap(id: string, name: string, description: string, img: string, url: string, area: object): object;
   module load {
      /**
       * load repository of kegg @``T:SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound``.
       * 
       * 
        * @param repository -
        * @param rawList 
        * + default value Is ``false``.
        * @param ignoreGlycan 
        * + default value Is ``false``.
        * @param env 
        * + default value Is ``null``.
      */
      function compounds(repository: any, rawList?: boolean, ignoreGlycan?: boolean, env?: object): object|object;
      /**
       * load list of kegg reference @``T:SMRUCC.genomics.Assembly.KEGG.WebServices.XML.Map``.
       * 
       * 
        * @param repository repository data source could be:
        *  
        *  1. a directory of repository data for kegg reference @``T:SMRUCC.genomics.Assembly.KEGG.WebServices.XML.Map``.
        *  2. a file stream for the map vector in messagepack or HDS pack format
        * @param rawMaps 
        * + default value Is ``true``.
        * @return a kegg reference map object vector, which can be indexed 
        *  via @``P:SMRUCC.genomics.ComponentModel.Annotation.PathwayBrief.EntryId``.
        *  
        *  the data type will be dertermined by the **`rawMaps`** parameter:
        *  
        *  1. for raw maps TRUE: a vector of the @``T:SMRUCC.genomics.Assembly.KEGG.WebServices.XML.Map`` object will be returns
        *  2. for raw maps FALSE: a wrapper of the kegg map collection 
        *     @``T:SMRUCC.genomics.Assembly.KEGG.WebServices.MapRepository`` will be generates from this function
      */
      function maps(repository: any, rawMaps?: boolean): object|object;
      /**
       * load kegg pathway maps from a given repository data directory.
       * 
       * 
        * @param repository -
        * @param referenceMap 
        * + default value Is ``true``.
        * @param env 
        * + default value Is ``null``.
      */
      function pathways(repository: string, referenceMap?: boolean, env?: object): object|object;
      /**
       * ### load kegg reaction data repository
       *  
       *  load reaction data from the given kegg reaction data 
       *  repository.
       * 
       * 
        * @param repository Could be a data pack file or a directory
        *  path that contains multiple reaction model data files.
        * @param raw this function will just returns a vector of the kegg reaction data if
        *  this parameter value is set to TRUE.
        * 
        * + default value Is ``true``.
        * @param env 
        * + default value Is ``null``.
      */
      function reactions(repository: any, raw?: boolean, env?: object): object|object;
   }
   /**
     * @param env default value Is ``null``.
   */
   function pathway(id: string, name: string, description: string, modules: object, DBLinks: object, KO_pathway: string, references: object, compounds: object, drugs: object, genes: object, organism: object, disease: object, env?: object): object;
   /**
   */
   function reaction(id: string, name: string, definition: string, equation: string, comment: string, reaction_class: object, enzyme: string, pathways: object, modules: object, KO: object, links: object): object;
   module reaction_class {
      /**
       * load stream of the reaction_class data model from kegg data repository.
       * 
       * 
        * @param repo -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function repo(repo: any, env?: object): object;
      /**
       * load reaction class data from a repository data source.
       * 
       * 
        * @param repo -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function table(repo: any, env?: object): object;
   }
   module reactions {
      /**
       * load kegg reaction table from a given repository model or resource file on your filesystem.
       * 
       * 
        * @param repo -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function table(repo: any, env?: object): any;
   }
   /**
     * @param env default value Is ``null``.
   */
   function reactionsId(repo: any, env?: object): any;
   module read {
      /**
      */
      function kegg_organism(file: string): object;
      /**
       * read the kegg pathway annotation result data.
       * 
       * 
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function KEGG_pathway(file: string, env?: object): object;
      /**
      */
      function kgml(file: string): object;
   }
   module save {
      /**
       * save the kegg organism data as data table file.
       * 
       * 
        * @param organism -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function kegg_organism(organism: object, file: string, env?: object): boolean;
      /**
       * save the kegg pathway annotation result data.
       * 
       * 
        * @param pathway -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function KEGG_pathway(pathway: any, file: string, env?: object): any;
   }
   /**
    * create shape area data for kegg graph rendering
    * 
    * 
     * @param data -
   */
   function shapeAreas(data: object): object;
   module write {
      /**
       * a generic method for write kegg data stream as messagepack
       * 
       * 
        * @param data -
        * @param file -
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function msgpack(data: any, file: any, env?: object): boolean;
   }
}
