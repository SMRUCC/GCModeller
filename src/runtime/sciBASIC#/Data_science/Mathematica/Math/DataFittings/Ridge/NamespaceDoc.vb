Namespace Ridge

    ''' <summary>
    ''' Ridge (L2 regularized) regression.
    ''' </summary>
    ''' <remarks>
    ''' <para>两个入口，分别覆盖两类问题：</para>
    ''' <list type="bullet">
    ''' <item><description>
    ''' <see cref="RidgeFitting"/>：<b>单响应</b>岭回归，带对数间隔的 λ 网格与 GCV 选参，
    ''' 面向"给一条曲线选平滑强度"的场景；
    ''' </description></item>
    ''' <item><description>
    ''' <see cref="MultiResponseRidge"/>：<b>多响应</b>闭式岭回归（<c>(XᵀX+λI)B = XᵀY</c>），
    ''' 支持逐组行掩码与逐基因批量求解，面向"同一套特征同时解释很多组响应"的场景
    ''' （例如单细胞扰动响应里逐基因的残差校正）。
    ''' </description></item>
    ''' </list>
    ''' </remarks>
    Module NamespaceDoc
    End Module
End Namespace
