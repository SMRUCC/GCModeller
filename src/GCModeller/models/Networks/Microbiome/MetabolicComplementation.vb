Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Model.Network.KEGG

''' <summary>
''' 微生物组营养互补网络，在这个模块之中节点为微生物，网络的边为互补或者竞争的营养物
''' </summary>
Public Module MetabolicComplementation

    ''' <summary>
    ''' 通过细菌的基因组内的KO编号列表查询出相对应的代谢反应过程模型，然后将这些代谢反应过程通过代谢物交点组装出代谢网络
    ''' </summary>
    ''' <param name="KO$">某一个细菌物种的基因组内的KO编号列表可以批量的从Uniprot数据库获取得到</param>
    ''' <param name="reactions">KEGG数据库之中的参考代谢反应列表</param>
    ''' <returns></returns>
    <Extension> Public Function BuildInternalNetwork(KO$(), reactions As IEnumerable(Of ReactionTable)) As NetworkGraph

    End Function

    <Extension>
    Public Function BuildInternalNetwork(genome As IEnumerable(Of entry), reactions As IEnumerable(Of ReactionTable)) As NetworkGraph
        Return genome _
            .Where(Function(protein) protein.Xrefs.ContainsKey("KO")) _
            .Select(Function(protein) protein.Xrefs("KO")) _
            .IteratesALL _
            .Select(Function(xref) xref.id) _
            .Distinct _
            .ToArray _
            .BuildInternalNetwork(reactions)
    End Function
End Module
