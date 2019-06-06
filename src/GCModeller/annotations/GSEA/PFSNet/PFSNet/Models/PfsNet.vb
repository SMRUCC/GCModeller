Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace DataStructure

    ''' <summary>
    ''' The gene node in the PfsNET evaluated sub network.(PfsNET所计算出来的子网络结果之中的一个基因节点)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PFSNetGraphNode

        ''' <summary>
        ''' The gene name.(基因名称)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Name As String
        ''' <summary>
        ''' Fuzzy weight of this gene node in current sub network.(这个基因节点在当前的这个子网络之中的模糊权重)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property weight As Double
        ''' <summary>
        ''' Fuzzy weight2 of this gene node in the current sub network.(这个基因节点在当前的这个子网络之中的模糊权重2) 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property weight2 As Double

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

    ''' <summary>
    ''' A metabolism pathway network or its calculated sub network.(一个代谢途径或者子网络，或者说是所属出的计算结果之中的一个子网络对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class PFSNetGraph

        Dim __nodes As New Dictionary(Of String, PFSNetGraphNode)

        ''' <summary>
        ''' The nodes in the PfsNET sub network.(网络之中的基因节点)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Nodes As PFSNetGraphNode()
            Get
                Return __nodes.Values.ToArray
            End Get
            Set(value As PFSNetGraphNode())
                __nodes = value.ToDictionary(Function(n) n.Name)
            End Set
        End Property

        ''' <summary>
        ''' Gene to gene interaction, ggi.(基因与基因之间的连接，即ggi，基因对基因的互作)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Edges As GraphEdge()
        <XmlAttribute> Public Property Id As String
        <XmlAttribute> Public Property statistics As Double
        <XmlAttribute> Public Property pvalue As Double
        <XmlAttribute> Public Property masked As Boolean

        ''' <summary>
        ''' The gene counts in the current calculated PfsNET sub network.
        ''' (当前的这个PfsNET子网络之中所计算出来的基因节点的数目)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Length As Integer
            Get
                Return Nodes.DataCounts
            End Get
        End Property

        ''' <summary>
        ''' Gets a specific gene node from its name property.(通过基因名来获取本网路对象之中的一个基因节点，当该节点不存在的时候会返回空值)
        ''' </summary>
        ''' <param name="name"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public ReadOnly Property Node(Name As String) As PFSNetGraphNode
            Get
                If __nodes.ContainsKey(Name) Then
                    Return __nodes(Name)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            If Edges.IsNullOrEmpty Then
                Return String.Format("{0} ({1})", Id, String.Join("; ", (From Node In Me.Nodes Select Node.Name).ToArray))
            End If
            Return String.Format("{0}: {1}", Id, String.Join("; ", (From ed In Edges Select String.Format("{0} <-> {1}", ed.g1, ed.g2))))
        End Function
    End Class

    ''' <summary>
    ''' Gene to Gene Interaction.(基因与基因之间的互作关系)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GraphEdge

        <XmlAttribute> Public Property PathwayID As String
        <XmlAttribute("GeneObject1")> Public Property g1 As String
        <XmlAttribute("GeneObject2")> Public Property g2 As String

        Public ReadOnly Property SelfLoop As Boolean
            Get
                Return String.Equals(g1, g2)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Join(vbTab, PathwayID, g1, g2)
        End Function

        Public Shared Function LoadData(path As String) As GraphEdge()
            Dim LQuery As GraphEdge() =
                LinqAPI.Exec(Of GraphEdge) <= From line As String
                                              In IO.File.ReadAllLines(path)
                                              Let tokens As String() = Strings.Split(line, vbTab)
                                              Select New GraphEdge With {
                                                  .PathwayID = tokens(0),
                                                  .g1 = tokens(1),
                                                  .g2 = tokens(2)
                                              }
            Return LQuery
        End Function
    End Class
End Namespace