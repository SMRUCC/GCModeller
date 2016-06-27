Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports System.Xml.Serialization

Namespace ComponentModel

    Public MustInherit Class PathwayBrief
        Implements IKeyValuePairObject(Of String, String)
        Implements sIdEnumerable

        <XmlAttribute>
        Public Overridable Property EntryId As String Implements sIdEnumerable.Identifier, IKeyValuePairObject(Of String, String).Identifier
        Public Property Description As String Implements IKeyValuePairObject(Of String, String).Value

        ''' <summary>
        ''' Gets the pathway related genes.
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function GetPathwayGenes() As String()

        ''' <summary>
        ''' 和具体的物种的编号无关的在KEGG数据库之中的参考对象的编号
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property BriteId As String
            Get
                Return EntryId
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}: {1}", EntryId, Description)
        End Function
    End Class
End Namespace