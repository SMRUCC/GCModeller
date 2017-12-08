Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FindPath
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data

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
    <Extension> Public Function BuildInternalNetwork(KO$(), reactions As ReactionRepository) As NetworkGraph
        Dim graph As New NetworkGraph
        Dim getNodes =
            Function(compounds As IEnumerable(Of CompoundSpecieReference))
                Dim nodes As New List(Of Node)

                For Each compound As CompoundSpecieReference In compounds
                    With graph.GetNode(compound.ID)
                        If .IsNothing Then
                            nodes += graph.CreateNode(compound.ID)
                        Else
                            nodes += .ref
                        End If
                    End With
                Next

                Return nodes
            End Function

        ' 对当前的这个细菌的基因组内的代谢网络进行装配
        ' 使用已经注释出来的KO编号列表，从参考反应模型库之中查询出相应的模型数据
        For Each link As Reaction In reactions.GetByKOMatch(KO)
            Dim flux = link.ReactionModel

            ' 节点为kegg compounds
            ' 链接的边为kegg reaction
            Dim reactants = getNodes(compounds:=flux.Reactants)
            Dim products = getNodes(compounds:=flux.Products)

            ' 对代谢物之间创建代谢反应连接边
            For Each r In reactants
                For Each p In products
                    With graph.GetEdges(r, p)
                        If .IsNullOrEmpty Then
                            Call graph.CreateEdge(r, p)
                        End If
                    End With
                Next
            Next
        Next

        Return graph
    End Function

    <Extension>
    Public Function BuildInternalNetwork(genome As IEnumerable(Of entry), reactions As ReactionRepository) As NetworkGraph
        Return genome _
            .Where(Function(protein) protein.Xrefs.ContainsKey("KO")) _
            .Select(Function(protein) protein.Xrefs("KO")) _
            .IteratesALL _
            .Select(Function(xref) xref.id) _
            .Distinct _
            .ToArray _
            .BuildInternalNetwork(reactions)
    End Function

    ''' <summary>
    ''' 构建出所给定的微生物组的代谢物互补与营养竞争网络
    ''' </summary>
    ''' <param name="metagenome">从Uniprot上批量下载的基因组蛋白注释数据</param>
    ''' <param name="reactions">KEGG参考代谢反应模型库</param>
    ''' <returns></returns>
    Public Function BuildMicrobiomeMetabolicNetwork(metagenome As IEnumerable(Of NamedValue(Of IEnumerable(Of entry))), reactions As ReactionRepository) As NetworkGraph
        Dim graph As New NetworkGraph

        ' 在微生物组的营养互补竞争网络之中
        ' 节点为微生物的基因组编号
        ' 链接的边为营养物关系
        For Each genome As NamedValue(Of IEnumerable(Of entry)) In metagenome
            Dim metabolicNetwork As NetworkGraph = genome _
                .Value _
                .BuildInternalNetwork(reactions)

            ' 添加该基因组的节点
            Dim node = graph.CreateNode(genome.Name)
            Dim endPoints = metabolicNetwork.EndPoints

            ' 将该微生物的代谢网络端点写入缓存之中
            Dim cache As New Dictionary(Of String, String())

            With cache
                ' 当前的这个基因组所必须的营养物，无法进行自身的合成
                !Essential_nutrients = endPoints _
                    .input _
                    .Select(Function(n) n.Data.label) _
                    .ToArray
                ' 当前的这个基因组所能够合成的次生代谢物网络终点
                !Secondary_metabolite = endPoints _
                    .output _
                    .Select(Function(n) n.Data.label) _
                    .ToArray
            End With

            With node.Data
                !MetabolicEndPoints = cache.GetJson
            End With
        Next

        ' 在构建完了所有的基因组的代谢网络的输入和输出端点之后
        ' 开始装配营养互补和竞争网络
        For Each genome As Node In graph.nodes
            Dim A = genome.Data _
                !MetabolicEndPoints _
                .LoadObject(Of Dictionary(Of String, String()))

            For Each member As Node In graph.nodes.Where(Function(n)
                                                             ' 忽略掉自身对自身的边连接，无意义
                                                             Return Not n Is genome
                                                         End Function)
                Dim B = member.Data _
                    !MetabolicEndPoints _
                    .LoadObject(Of Dictionary(Of String, String()))

                ' 通过查看A和B的输入输出端点是否有重合来了解二者是否存在营养互补的关系
                ' A input vs B output
                Dim Ainput = A!Essential_nutrients
                Dim Boutput = B!Secondary_metabolite

                With Ainput.Intersect(Boutput).ToArray
                    If Not .IsNullOrEmpty Then
                        ' 输入与输出有重叠部分，则可能存在营养互补
                        Dim complementary = graph.CreateEdge(member, genome)
                        complementary.Data!compounds = .GetJson
                        complementary.Directed = True
                        complementary.Weight = .Length
                        complementary.Data.label = $"{member.Label} => {genome.Label}"
                    End If
                End With

                ' A output vs B input
                Dim Binput = B!Essential_nutrients
                Dim Aoutput = A!Secondary_metabolite

                With Binput.Intersect(Aoutput).ToArray
                    If Not .IsNullOrEmpty Then
                        ' 输入与输出有重叠部分，则可能存在营养互补
                        Dim complementary = graph.CreateEdge(genome, member)
                        complementary.Data!compounds = .GetJson
                        complementary.Directed = True
                        complementary.Weight = .Length
                        complementary.Data.label = $"{genome.Label} => {member.Label}"
                    End If
                End With

                ' 通过查看A和B的输入输入端点是否有重合来了解二者是否存在营养竞争的关系
                ' A input vs B input
                With Ainput.Intersect(Binput).ToArray
                    If Not .IsNullOrEmpty Then
                        ' 两个基因组的代谢网络输入端点存在重叠的部分，则可能存在营养竞争关系
                        Dim competition = graph.CreateEdge(genome, member)
                        competition.Data!compounds = .GetJson
                        competition.Directed = False
                        competition.Weight = .Length
                        competition.Data.label = $"{genome.Label} vs {member.Label}"
                    End If
                End With
            Next
        Next

        Return graph
    End Function
End Module
