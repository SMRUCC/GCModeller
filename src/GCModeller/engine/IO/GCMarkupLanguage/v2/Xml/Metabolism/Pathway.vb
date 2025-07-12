Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace v2

    Public Class FunctionalCategory

        <XmlAttribute>
        Public Property category As String
        <XmlElement("pathway")>
        Public Property pathways As Pathway()

        Public Overrides Function ToString() As String
            Return category
        End Function

    End Class

    <XmlType("pathway", [Namespace]:=VirtualCell.GCMarkupLanguage)>
    Public Class Pathway : Implements INamedValue

        <XmlAttribute> Public Property ID As String Implements IKeyedEntity(Of String).Key
        <XmlAttribute> Public Property name As String

        ''' <summary>
        ''' 属性的值含义如下：
        ''' 
        ''' + <see cref="[Property].name"/>: protein_id
        ''' + <see cref="[Property].value"/>: KO number
        ''' + <see cref="[Property].Comment"/>: gene locus_tag
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("enzyme")>
        Public Property enzymes As [Property]()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"[{ID}] {name} with {enzymes.Length} enzymes"
        End Function

    End Class

End Namespace