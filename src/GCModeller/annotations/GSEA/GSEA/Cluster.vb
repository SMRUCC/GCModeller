Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

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
    <XmlElement>
    Public Property description As String

    ''' <summary>
    ''' 当前的这个聚类之中的基因列表
    ''' </summary>
    ''' <returns></returns>
    Public Property members As Synonym()

    Dim index As Index(Of String)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Intersect(list As IEnumerable(Of String)) As IEnumerable(Of String)
        If index Is Nothing Then
            index = members _
                .Select(Function(name) name.AsEnumerable) _
                .IteratesALL _
                .Distinct _
                .ToArray
        End If

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
    Public Property comments As String
    Public Property build As Date = Now

    <XmlElement>
    Public Property clusters As Cluster()

    Public Overrides Function ToString() As String
        Return name
    End Function
End Class