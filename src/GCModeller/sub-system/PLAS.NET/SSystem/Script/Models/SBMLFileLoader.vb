Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
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
        Private ReadOnly Property Produce As Dictionary(Of String, List(Of SbmlReaction))
        ''' <summary>
        ''' 消耗目标代谢物的所有生化反应
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property Consume As Dictionary(Of String, List(Of SbmlReaction))

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

        ''' <summary>
        ''' 获取生成目标代谢物的所有生化反应
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function GetProduce(id As String) As SbmlReaction()
            Return rawChannels(Produce, id)
        End Function

        ''' <summary>
        ''' 获取消耗目标代谢物的所有生化反应
        ''' </summary>
        ''' <param name="id"></param>
        ''' <returns></returns>
        Public Function GetConsume(id As String) As SbmlReaction()
            Return rawChannels(Consume, id)
        End Function

        Private Shared Function rawChannels(table As Dictionary(Of String, List(Of SbmlReaction)), id As String) As SbmlReaction()
            Dim list As List(Of SbmlReaction) = Nothing

            If String.IsNullOrEmpty(id) OrElse Not table.TryGetValue(id, list) Then
                Return New SbmlReaction() {}
            End If

            Return list.ToArray
        End Function
    End Class

    ''' <summary>
    ''' 自包含的 SBML 物种节点模型，同时兼容 <c>initialAmount</c> 与 <c>initialConcentration</c> 两种属性写法
    ''' </summary>
    <XmlType("species", Namespace:=SBMLFileLoader.Level3Namespace)>
    Public Class SbmlRawSpecies

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property compartment As String
        <XmlAttribute> Public Property initialAmount As Double
        <XmlAttribute> Public Property initialConcentration As Double
        <XmlAttribute> Public Property boundaryCondition As Boolean

        ''' <summary>
        ''' 取出当前物种节点的初始量：优先使用 initialAmount，其次使用 initialConcentration
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property InitialValue As Double
            Get
                If initialAmount > 0 Then
                    Return initialAmount
                Else
                    Return initialConcentration
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[{id}] {name} = {InitialValue}"
        End Function
    End Class

    ''' <summary>
    ''' 自包含的 SBML model 节点模型
    ''' </summary>
    <XmlType("model", Namespace:=SBMLFileLoader.Level3Namespace)>
    Public Class SbmlRawModel

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property name As String
        ''' <summary>
        ''' 列表外层元素为 listOfSpecies，其中的子元素为 species 节点
        ''' </summary>
        ''' <returns></returns>
        Public Property listOfSpecies As SbmlRawSpecies()
        ''' <summary>
        ''' 列表外层元素为 listOfReactions，其中的子元素为 reaction 节点
        ''' </summary>
        ''' <returns></returns>
        Public Property listOfReactions As L3.reactionList(Of L3.Reaction)

        Public Overrides Function ToString() As String
            Return If(String.IsNullOrEmpty(name), id, name)
        End Function
    End Class

    ''' <summary>
    ''' 自包含的 SBML 文档根节点模型
    ''' </summary>
    <XmlRoot("sbml", Namespace:=SBMLFileLoader.Level3Namespace)>
    Public Class SbmlRawDocument

        <XmlAttribute> Public Property level As Integer
        <XmlAttribute> Public Property version As Integer
        <XmlElement("model")> Public Property model As SbmlRawModel
    End Class

    ''' <summary>
    ''' The SBML document loader for both of the level2 and level3 sbml file.(支持 Level2 与 Level3 的SBML文档加载器)
    ''' </summary>
    ''' <remarks>
    ''' 由于 <c>SMRUCC.genomics.Model.SBML.Level2.XmlFile</c> 的类型体系在 XmlSerializer 之上无法被反射
    ''' （reaction.Id 与其基类成员存在自定义特性冲突），故这里统一使用 Level3 core 的命名空间来反序列化
    ''' 两种版本的文档：加载之前会先把文档之中的 SBML 命名空间统一规整化为 Level3 core 的命名空间。
    ''' </remarks>
    Public Module SBMLFileLoader

        ''' <summary>
        ''' SBML Level3 core 的命名空间
        ''' </summary>
        Public Const Level3Namespace As String = "http://www.sbml.org/sbml/level3/version1/core"

        ReadOnly numberRegex As New Regex("\d", RegexOptions.Compiled)
        ''' <summary>
        ''' 匹配形如 http://www.sbml.org/sbml/level2 或者 .../level2/version4 / .../level3/version1/core 的命名空间
        ''' （使用零宽后向断言确保不会误伤 fbc 这类扩展包的命名空间）
        ''' </summary>
        ReadOnly sbmlXmlnsRegex As New Regex("http://www\.sbml\.org/sbml/level\d+(/version\d+)?(/core)?(?=[""'\s/>])", RegexOptions.Compiled)
        ReadOnly sbmlRootRegex As New Regex("<sbml\b[^>]*>", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
        ReadOnly levelRegex As New Regex("level\s*=\s*""(?<lvl>\d+)""", RegexOptions.IgnoreCase Or RegexOptions.Compiled)

        ''' <summary>
        ''' Loading a sbml document file into the unified network model.
        ''' (将SBML文档加载为统一的网络视图模型)
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function Load(path As String) As SbmlNetwork
            Dim document As SbmlRawDocument = LoadDocument(path)
            Dim model As SbmlRawModel = document.model
            Dim network As New SbmlNetwork With {
                .Level = document.level,
                .Version = document.version,
                .ModelName = If(model Is Nothing, "", model.ToString)
            }

            If model Is Nothing Then
                Return network
            End If

            For Each sp As SbmlRawSpecies In model.listOfSpecies.SafeQuery
                Dim initial As Double = sp.InitialValue

                Call network.Species.Add(New SbmlSpecies With {
                    .Id = StripNumber(sp.id),
                    .Name = sp.name,
                    .Compartment = sp.compartment,
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

            Return network
        End Function

        ''' <summary>
        ''' 将SBML文档反序列化为自包含的文档对象模型
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function LoadDocument(path As String) As SbmlRawDocument
            Dim xml As String = File.ReadAllText(path, Encoding.UTF8)

            ' 统一规整化命名空间，这样 Level2 与 Level3 的文档都可以使用同一套类型进行反序列化
            xml = sbmlXmlnsRegex.Replace(xml, Level3Namespace)

            Using reader As New StringReader(xml)
                Dim serializer As New XmlSerializer(GetType(SbmlRawDocument))
                Dim document As SbmlRawDocument = DirectCast(serializer.Deserialize(reader), SbmlRawDocument)

                If document.level = 0 Then
                    document.level = DetectLevel(xml)
                End If

                Return document
            End Using
        End Function

        ''' <summary>
        ''' 探测目标SBML文档是否为 Level3 及以上版本的文档
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function IsLevel3(path As String) As Boolean
            Return DetectLevel(path) >= 3
        End Function

        ''' <summary>
        ''' 从SBML文档的根元素之上探测出文档的层级版本号
        ''' </summary>
        ''' <param name="document">可以是文件路径，也可以是XML文档的文本内容</param>
        ''' <returns></returns>
        Public Function DetectLevel(document As String) As Integer
            Dim head As String

            If document.Length > 8192 Then
                head = document.Substring(0, 8192)
            Else
                head = document
            End If

            Dim root As String = sbmlRootRegex.Match(head).Value

            If String.IsNullOrEmpty(root) Then
                Return 2
            End If

            Dim m As Match = levelRegex.Match(root)

            If m.Success Then
                Return CInt(Val(m.Groups("lvl").Value))
            End If
            If InStr(root, Level3Namespace) > 0 Then
                Return 3
            End If

            Return 2
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
    End Module
End Namespace
