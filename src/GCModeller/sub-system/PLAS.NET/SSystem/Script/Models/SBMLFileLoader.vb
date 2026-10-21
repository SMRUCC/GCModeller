Imports System.IO
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq
Imports L2 = SMRUCC.genomics.Model.SBML.Level2
Imports L3 = SMRUCC.genomics.Model.SBML.Level3

Namespace Script

    ''' <summary>
    ''' SBML 文档之中的物种节点的统一视图
    ''' </summary>
    Public Class SbmlSpecies

        ''' <summary>
        ''' 唯一的标识符，其中的连接符已经被规整化过了
        ''' </summary>
        ''' <returns></returns>
        Public Property Id As String
        ''' <summary>
        ''' 代谢物的名称
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String
        ''' <summary>
        ''' 所在的细胞结构的编号
        ''' </summary>
        ''' <returns></returns>
        Public Property Compartment As String
        ''' <summary>
        ''' 初始量，缺失的时候为零
        ''' </summary>
        ''' <returns></returns>
        Public Property InitialAmount As Double
        ''' <summary>
        ''' 原始的SBML文档之中是否显式的给出了初始量属性
        ''' </summary>
        ''' <returns></returns>
        Public Property HasInitialAmount As Boolean

        Public Overrides Function ToString() As String
            Return $"[{Id}] {Name} ({Compartment}) = {InitialAmount}"
        End Function
    End Class

    ''' <summary>
    ''' SBML 文档之中的生化反应节点的统一视图
    ''' </summary>
    Public Class SbmlReaction

        ''' <summary>
        ''' 反应的唯一标识符
        ''' </summary>
        ''' <returns></returns>
        Public Property Id As String
        ''' <summary>
        ''' 反应物的代谢物编号列表
        ''' </summary>
        ''' <returns></returns>
        Public Property Reactants As String()
        ''' <summary>
        ''' 生成物的代谢物编号列表
        ''' </summary>
        ''' <returns></returns>
        Public Property Products As String()

        Public Overrides Function ToString() As String
            Return $"[{Id}] {String.Join(" + ", Reactants)} => {String.Join(" + ", Products)}"
        End Function
    End Class

    ''' <summary>
    ''' SBML Level2 与 Level3 文档的统一视图，屏蔽掉两个版本之间的类型差异
    ''' </summary>
    Public Class SbmlNetwork

        ''' <summary>
        ''' 模型的名称
        ''' </summary>
        ''' <returns></returns>
        Public Property ModelName As String
        ''' <summary>
        ''' SBML 文档的层级版本号
        ''' </summary>
        ''' <returns></returns>
        Public Property Level As Integer
        ''' <summary>
        ''' SBML 文档的版本号
        ''' </summary>
        ''' <returns></returns>
        Public Property Version As Integer
        ''' <summary>
        ''' 当前的reaction网络之中的所有的代谢物的列表
        ''' </summary>
        ''' <returns></returns>
        Public Property Species As New List(Of SbmlSpecies)
        ''' <summary>
        ''' 当前的reaction网络之中的所有的生化反应的列表
        ''' </summary>
        ''' <returns></returns>
        Public Property Reactions As New List(Of SbmlReaction)

        Public Overrides Function ToString() As String
            Return $"{ModelName}: {Species.Count} species x {Reactions.Count} reactions (SBML Level{Level}V{Version})"
        End Function
    End Class

    ''' <summary>
    ''' 物种到生化反应之间的反向索引：用于替代原先对全reaction列表的 O(n*m) 线性扫描
    ''' </summary>
    Public Class SbmlReactionIndex

        ''' <summary>
        ''' 生成目标代谢物的所有生化反应
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Produce As Dictionary(Of String, List(Of SbmlReaction))
        ''' <summary>
        ''' 消耗目标代谢物的所有生化反应
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Consume As Dictionary(Of String, List(Of SbmlReaction))

        Sub New(network As SbmlNetwork)
            Produce = New Dictionary(Of String, List(Of SbmlReaction))
            Consume = New Dictionary(Of String, List(Of SbmlReaction))

            For Each rxn As SbmlReaction In network.Reactions
                For Each id As String In rxn.Products.SafeQuery
                    Call push(Produce, id, rxn)
                Next
                For Each id As String In rxn.Reactants.SafeQuery
                    Call push(Consume, id, rxn)
                Next
            Next
        End Sub

        Private Shared Sub push(table As Dictionary(Of String, List(Of SbmlReaction)), id As String, rxn As SbmlReaction)
            Dim list As List(Of SbmlReaction) = Nothing

            If String.IsNullOrEmpty(id) Then
                Return
            End If
            If Not table.TryGetValue(id, list) Then
                list = New List(Of SbmlReaction)
                Call table.Add(id, list)
            End If

            Call list.Add(rxn)
        End Sub

        ''' <summary>
        ''' 目标代谢物是否存在有关联的生化反应？
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function HasReaction(id As String) As Boolean
            Return Produce.ContainsKey(id) OrElse Consume.ContainsKey(id)
        End Function
    End Class

    ''' <summary>
    ''' The SBML document loader for both of the level2 and level3 sbml file.(支持 Level2 与 Level3 的SBML文档加载器)
    ''' </summary>
    Public Module SBMLFileLoader

        ''' <summary>
        ''' SBML Level3 core 的命名空间
        ''' </summary>
        Public Const Level3Namespace As String = "http://www.sbml.org/sbml/level3/version1/core"

        ReadOnly numberRegex As New Regex("\d", RegexOptions.Compiled)
        ReadOnly sbmlRootRegex As New Regex("<sbml\b[^>]*>", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
        ReadOnly levelRegex As New Regex("level\s*=\s*""(?<lvl>\d+)""", RegexOptions.IgnoreCase Or RegexOptions.Compiled)

        ''' <summary>
        ''' Loading a sbml document file into the unified network model.
        ''' (将SBML文档加载为统一的网络视图模型)
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function Load(path As String) As SbmlNetwork
            If IsLevel3(path) Then
                Return loadLevel3(path)
            Else
                Return loadLevel2(path)
            End If
        End Function

        ''' <summary>
        ''' 探测目标SBML文档是否为 Level3 及以上版本的文档
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function IsLevel3(path As String) As Boolean
            Using reader As New StreamReader(path)
                Dim buffer As Char() = New Char(8191) {}
                Dim count As Integer = reader.Read(buffer, 0, buffer.Length)

                If count <= 0 Then
                    Return False
                End If

                Dim head As String = New String(buffer, 0, count)
                Dim root As String = sbmlRootRegex.Match(head).Value

                If String.IsNullOrEmpty(root) Then
                    Return False
                End If
                If InStr(root, Level3Namespace) > 0 Then
                    Return True
                End If

                Dim m As Match = levelRegex.Match(root)

                If m.Success Then
                    Return Val(m.Groups("lvl").Value) >= 3
                End If

                Return False
            End Using
        End Function

        ''' <summary>
        ''' 需要在这里将``-``连接符替换为下划线``_``不然在解析数学表达式的时候会被当作为减号
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function StripNumber(id As String) As String
            If String.IsNullOrEmpty(id) Then
                Return id
            End If

            Dim n As String = numberRegex.Match(id).Value

            If Not String.IsNullOrEmpty(n) AndAlso InStr(id, n) = 1 Then
                id = "N_" & id
            End If

            Return id.Replace("-", "_").Replace("+", "_")
        End Function

        Private Function loadLevel2(path As String) As SbmlNetwork
            Dim document As L2.XmlFile = L2.XmlFile.Load(path)
            Dim model As L2.Elements.Model = document.Model
            Dim network As New SbmlNetwork With {
                .Level = document.level,
                .Version = document.version,
                .ModelName = If(model Is Nothing, "", model.name)
            }

            If Not model Is Nothing Then
                For Each sp As L2.Elements.Specie In model.listOfSpecies.SafeQuery
                    Call network.Species.Add(New SbmlSpecies With {
                        .Id = StripNumber(sp.ID),
                        .Name = sp.name,
                        .Compartment = sp.compartmentId,
                        .InitialAmount = sp.InitialAmount,
                        .HasInitialAmount = sp.InitialAmount > 0
                    })
                Next

                For Each rxn As L2.Elements.Reaction In model.listOfReactions.SafeQuery
                    Call network.Reactions.Add(New SbmlReaction With {
                        .Id = rxn.id,
                        .Reactants = rxn.Reactants _
                            .SafeQuery _
                            .Select(Function(m) StripNumber(m.species)) _
                            .ToArray,
                        .Products = rxn.Products _
                            .SafeQuery _
                            .Select(Function(m) StripNumber(m.species)) _
                            .ToArray
                    })
                Next
            End If

            Return network
        End Function

        Private Function loadLevel3(path As String) As SbmlNetwork
            Dim document As L3.XmlFile(Of L3.Reaction) = L3.XmlFile(Of L3.Reaction).LoadDocument(path)
            Dim model As L3.Model(Of L3.Reaction) = document.model
            Dim network As New SbmlNetwork With {
                .Level = document.level,
                .Version = document.version,
                .ModelName = If(model Is Nothing, "", model.name)
            }

            If Not model Is Nothing Then
                For Each sp As L3.species In model.listOfSpecies.SafeQuery
                    Dim initial As Double = sp.initialConcentration

                    Call network.Species.Add(New SbmlSpecies With {
                        .Id = StripNumber(sp.id),
                        .Name = sp.name,
                        .Compartment = sp.compartmentId,
                        .InitialAmount = initial,
                        .HasInitialAmount = initial > 0
                    })
                Next

                For Each rxn As L3.Reaction In model.listOfReactions.SafeQuery
                    Call network.Reactions.Add(New SbmlReaction With {
                        .Id = rxn.id,
                        .Reactants = rxn.listOfReactants _
                            .SafeQuery _
                            .Select(Function(m) StripNumber(m.species)) _
                            .ToArray,
                        .Products = rxn.listOfProducts _
                            .SafeQuery _
                            .Select(Function(m) StripNumber(m.species)) _
                            .ToArray
                    })
                Next
            End If

            Return network
        End Function
    End Module
End Namespace
