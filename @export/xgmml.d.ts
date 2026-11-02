// export R# package module type define for javascript/typescript language
//
//    imports "xgmml" from "cytoscape";
//
// ref=cytoscape_toolkit.xgmmlToolkit@cytoscape, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace xgmml {
   module read {
      /**
      */
      function xgmml(file: string): object;
   }
   /**
   */
   function set_images(model: object, dir: string, attr: string): object;
   module write {
      /**
      */
      function xgmml(model: object, file: string): boolean;
   }
   module xgmml {
      /**
        * @param size default value Is ``'10(A0)'``.
        * @param convexHull default value Is ``null``.
        * @param edgeBends default value Is ``false``.
        * @param altStyle default value Is ``false``.
        * @param rewriteGroupCategoryColors default value Is ``'TSF'``.
        * @param enzymeColorSchema default value Is ``'Set1'``.
        * @param compoundColorSchema default value Is ``'Clusters'``.
        * @param reactionKOMapping default value Is ``null``.
        * @param renderStyle default value Is ``false``.
        * @param nodes default value Is ``null``.
        * @param compoundNames default value Is ``null``.
        * @param attrs default value Is ``["label","class"]``.
        * @param env default value Is ``null``.
      */
      function render(model: any, size?: string, convexHull?: string, edgeBends?: boolean, altStyle?: boolean, rewriteGroupCategoryColors?: string, enzymeColorSchema?: string, compoundColorSchema?: string, reactionKOMapping?: object, renderStyle?: boolean, nodes?: object, compoundNames?: object, attrs?: any, env?: object): object;
   }
}
