declare namespace xgmml {
   module read {
      /**
      */
      function xgmml(file:string): any;
   }
   module xgmml {
      /**
        * @param size default value is ``'10(A0)'``.
        * @param convexHull default value is ``null``.
        * @param edgeBends default value is ``false``.
        * @param altStyle default value is ``false``.
        * @param rewriteGroupCategoryColors default value is ``'TSF'``.
        * @param enzymeColorSchema default value is ``'Set1:c8'``.
        * @param compoundColorSchema default value is ``'Clusters'``.
        * @param reactionKOMapping default value is ``null``.
        * @param compoundNames default value is ``null``.
      */
      function render(model:any, size?:string, convexHull?:string, edgeBends?:boolean, altStyle?:boolean, rewriteGroupCategoryColors?:string, enzymeColorSchema?:string, compoundColorSchema?:string, reactionKOMapping?:object, compoundNames?:object): any;
   }
}
