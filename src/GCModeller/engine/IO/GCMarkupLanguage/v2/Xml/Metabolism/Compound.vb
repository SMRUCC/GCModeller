Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace v2

    <XmlType("compound", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Compound : Implements INamedValue

        <XmlAttribute>
        Public Property ID As String Implements IKeyedEntity(Of String).Key

        ''' <summary>
        ''' the kegg reference id of current metabolite
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute>
        Public Property kegg_id As String()

        <XmlText> Public Property name As String

        ''' <summary>
        ''' the initialize mass content of current compound.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property mass0 As Double = 1000

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