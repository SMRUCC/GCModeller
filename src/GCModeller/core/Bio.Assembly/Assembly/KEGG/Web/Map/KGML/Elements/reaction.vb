Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace Assembly.KEGG.WebServices.KGML

    ''' <summary>
    ''' &lt;reaction>元素表示通路中的一种生化反应，用于连接代谢物节点，形成代谢网络中的化学转化关系。在代谢通路中，&lt;reaction>元素主要涉及底物（substrates）
    ''' 和产物（products）两类代谢物节点。它描述了一个化学反应如何将一组底物转化为另一组产物，以及该反应是否可逆等信息。&lt;reaction>元素在KGML中通常用于
    ''' 表示化学网络（chemical network），即以代谢物为节点、反应为边的关系网络。
    ''' </summary>
    Public Class reaction : Inherits link

        ''' <summary>
        ''' 该反应的唯一标识符，在通路范围内唯一。通常是一个整数，用于在KGML中引用该反应。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property id As String
        ''' <summary>
        ''' 该反应的名称，通常以KEGG REACTION数据库的ID表示，例如name="rn:R00710"。
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property name As String

        ''' <summary>
        ''' 一个或多个&lt;substrate>子元素，表示该反应的底物。每个&lt;substrate>通过id属性引用一个代谢物节点的ID（即对应&lt;entry>的id），并通过name属性提供该代谢物的KEGG ID。
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("substrate")>
        Public Property substrates As compound()
        ''' <summary>
        ''' 一个或多个&lt;product>子元素，表示该反应的产物。每个&lt;product>同样通过id引用代谢物节点ID，并通过name提供代谢物KEGG ID。
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("product")>
        Public Property products As compound()

        Public Function GetModel() As Equation
            Return New Equation With {
                .id = id,
                .reversible = True,
                .Reactants = substrates.Select(Function(c) New CompoundSpecieReference(1, c.name)).ToArray,
                .products = products.Select(Function(c) New CompoundSpecieReference(1, c.name)).ToArray
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return GetModel.ToString
        End Function
    End Class

End Namespace