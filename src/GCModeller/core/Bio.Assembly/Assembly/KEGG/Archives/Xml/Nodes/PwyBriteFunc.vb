Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.KEGG.DBGET

Namespace Assembly.KEGG.Archives.Xml.Nodes

    <XmlType("KEGG.CellPhenotype_BritText", Namespace:="http://code.google.com/p/genome-in-code/kegg/bio_model/")>
    Public Class PwyBriteFunc

        ''' <summary>
        ''' 大分类
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property [Class] As String
        ''' <summary>
        ''' 小分类
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Category As String
        <XmlArray("KEGG.Pathways")> Public Property Pathways As bGetObject.Pathway()

        Public Overrides Function ToString() As String
            Return [Class]
        End Function
    End Class
End Namespace