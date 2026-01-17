Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace v2

    <XmlType("compound", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Compound : Implements INamedValue

        <XmlAttribute>
        Public Property ID As String Implements IKeyedEntity(Of String).Key

        ''' <summary>
        ''' the reference id of current metabolite
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property referenceIds As String()

        <XmlText> Public Property name As String

        <XmlAttribute>
        Public Property formula As String

        ''' <summary>
        ''' the cross reference id of this compound, used for search on the workbench ui
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property db_xrefs As String()

        Sub New()
        End Sub

        Sub New(id As String, Optional name As String = Nothing)
            Me.ID = id
            Me.name = If(name, id)
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{ID}] {name}"
        End Function

    End Class
End Namespace