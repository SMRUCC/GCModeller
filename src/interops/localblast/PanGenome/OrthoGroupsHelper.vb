Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Public Module OrthoGroupsHelper

    ''' <summary>
    ''' 将基因组vs参考库的BHR直系同源结果放入到基因簇构建模块之中
    ''' </summary>
    ''' <param name="uf"></param>
    ''' <param name="bhr">query为待分析的基因组内的基因ID，hit为参考库中的基因ID</param>
    ''' <returns></returns>
    Public Function SetReferenceBHR(uf As UnionFind, bhr As BiDirectionalBesthit()) As UnionFind
        For Each hit As BiDirectionalBesthit In bhr
            Call uf.AddElement(hit.QueryName)
            Call uf.AddElement(hit.HitName)
            Call uf.Union(hit.HitName, hit.QueryName)
        Next

        Return uf
    End Function

End Module
