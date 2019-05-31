Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

''' <summary>
''' 主要是KEGG代谢途径，也可以是其他的具有生物学意义的聚类结果
''' </summary>
Public Class Cluster : Implements INamedValue

    ''' <summary>
    ''' 代谢途径的编号或者其他的标识符
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property ID As String Implements IKeyedEntity(Of String).Key
    Public Property names As String
    <XmlText>
    Public Property description As String

    ''' <summary>
    ''' 当前的这个聚类之中的基因列表
    ''' </summary>
    ''' <returns></returns>
    Public Property members As String()
        Get
            Return index.Objects
        End Get
        Set(value As String())
            index = value
        End Set
    End Property

    Dim index As Index(Of String)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Intersect(list As IEnumerable(Of String)) As IEnumerable(Of String)
        Return index.Intersect(collection:=list)
    End Function

    Public Overrides Function ToString() As String
        Return ID
    End Function
End Class

''' <summary>
''' 假设基因组是有许多个功能聚类的集合构成的
''' </summary>
<XmlRoot("background", [Namespace]:="http://gcmodeller.org/GSEA/background.xml")>
Public Class Background : Inherits XmlDataModel
    Implements INamedValue

    Public Property name As String Implements IKeyedEntity(Of String).Key
    Public Property clusters As Cluster()

    Public Overrides Function ToString() As String
        Return name
    End Function
End Class