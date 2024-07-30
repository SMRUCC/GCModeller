// export R# package module type define for javascript/typescript language
//
//    imports "mesh" from "kb";
//
// ref=kb.meshTools@kb, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The National Center For Biotechnology Information (NCBI) Medical Subject Headings (MeSH) 
 *  system Is a comprehensive controlled vocabulary used For the indexing And cataloging Of 
 *  scientific literature In the field Of biomedicine. Developed by the National Library Of 
 *  Medicine (NLM), MeSH serves As a thesaurus that provides a consistent way To organize And 
 *  retrieve information from the vast biomedical literature.
 *  
 *  Here 's an introduction to the NCBI MeSH system:
 *  
 *  1. **Purpose**: The primary purpose Of MeSH Is To enable the efficient retrieval Of information from
 *       the PubMed database And other NLM databases. It helps researchers, healthcare professionals, And 
 *       the general Public find relevant articles And resources.
 *  2. **Structure**: MeSH Is structured hierarchically, with a tree-Like organization. It consists of 
 *       Descriptors (main headings), Qualifiers (subheadings), And Entry Terms (synonyms Or related terms). 
 *       Descriptors are organized into 16 main categories, known as Trees.
 *  3. **Descriptors**: Descriptors are the main indexing terms used To describe the subject Of an 
 *       article. Each Descriptor Is assigned a unique MeSH ID And can have multiple Entry Terms associated
 *       With it.
 *  4. **Trees**: The 16 main Trees in MeSH are: 
 *      - Anatomy
 *      - Organisms
 *      - Diseases
 *      - Chemicals And Drugs
 *      - Analytical, Diagnostic And Therapeutic Techniques And Equipment
 *      - Psychiatry And Psychology
 *      - Phenomena And Processes
 *      - Disciplines And Occupations
 *      - Anthropology, Education, Sociology, And Social Phenomena
 *      - Technology, Industry, And Agriculture
 *      - Humanities
 *      - Information Science
 *      - Named Groups
 *      - Health Care
 *      - Publication Characteristics
 *      - Geographicals
 *  5. **Qualifiers**: These are used To describe the specific aspects Of a Descriptor. For example, "Drug 
 *       Therapy" might be a Qualifier For the Descriptor "Hypertension."
 *  6. **Entry Terms**: These are synonyms Or closely related terms To the Descriptors. They help ensure that
 *       users can find relevant information even If they use different terminology.
 *  7. **Updates**: MeSH Is updated annually To reflect the evolving nature Of biomedical research. New 
 *       Descriptors, Qualifiers, And Entry Terms are added, while outdated terms are removed Or revised.
 *  8. **Search And Navigation**: Users can search For MeSH terms directly In the MeSH Browser Or use them 
 *       To refine searches In PubMed. The hierarchical Structure Of MeSH allows users To navigate from broad 
 *       To more specific topics.
 *  9. **Integration with PubMed**: MeSH terms are used To index articles In PubMed. When users search For a 
 *       specific topic, they can use MeSH terms To ensure they retrieve the most relevant And comprehensive 
 *       results.
 * 
*/
declare namespace mesh {
   /**
    * build background model for enrichment based on ncbi mesh terms
    * 
    * 
     * @param tree -
     * @param category -
     * @param levels 
     * + default value Is ``1``.
   */
   function mesh_background(tree: object, category: object, levels?: object): object;
   /**
    * get mesh category values that assign to current mesh term
    * 
    * 
     * @param term -
   */
   function mesh_category(term: object): object;
   module read {
      /**
       * read the tree of mesh terms
       * 
       * 
        * @param file the file path to the ncbi mesh term file, usually be the ``data/mtrees2024.txt``.
        * @param as_tree -
        * 
        * + default value Is ``true``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function mesh_tree(file: any, as_tree?: boolean, env?: object): object;
      /**
      */
      function mesh_xml(file: string): object;
   }
}
