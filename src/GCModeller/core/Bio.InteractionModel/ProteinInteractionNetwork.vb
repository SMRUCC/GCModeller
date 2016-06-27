Imports System.Text
Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly
Imports LANS.SystemsBiology.ProteinModel
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic

Public Class ProteinInteractionNetwork

    Public Const INTERACTION_SIF_ITEM As String = "{0}	interaction	{1}"

    Public Class Interaction : Implements IKeyValuePairObject(Of String, String())

        <XmlAttribute> Public Property Protein As String Implements IKeyValuePairObject(Of String, String()).Identifier
        Public Property InteractionWith As String() Implements IKeyValuePairObject(Of String, String()).Value

        Public Overrides Function ToString() As String
            Return Protein
        End Function
    End Class

    Public Shared Sub ExportNetwork(Network As Interaction(), SavedFile As String)
        Dim InteractionList As List(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))
        For Each Node In Network
            For Each Edge In Node.InteractionWith
                Call InteractionList.Add(New KeyValuePair(Of String, String)(Node.Protein, Edge)) '
            Next
        Next

        Dim _InteractionList2 As List(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))
        For i As Integer = 0 To InteractionList.Count - 1
            Dim item = InteractionList(i)
            Call _InteractionList2.Add(item)
            Call InteractionList.RemoveAll(Function(itemObj As KeyValuePair(Of String, String)) Equals(itemObj, item))
            If i > InteractionList.Count Then
                Exit For
            End If
        Next

        Dim sBuilder As StringBuilder = New StringBuilder(4096)
        For Each item In _InteractionList2
            Call sBuilder.AppendLine(String.Format(INTERACTION_SIF_ITEM, item.Key, item.Value))
        Next

        Call sBuilder.ToString.SaveTo(SavedFile, Encoding:=System.Text.Encoding.ASCII)
    End Sub

    Private Overloads Shared Function Equals(obj1 As KeyValuePair(Of String, String), obj2 As KeyValuePair(Of String, String)) As Boolean
        If String.Equals(obj1.Key, obj2.Key) OrElse String.Equals(obj1.Key, obj2.Value) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Shared Function BuildInteraction(Proteins As Protein(), DOMINE As DOMINE.Database) As Interaction()
        Dim InteractionList As List(Of Interaction) = New List(Of Interaction)

        For Each Protein In Proteins
            Dim InteractionDomains = Protein.InteractionWith(DOMINE)
            Dim InteractionProteins As List(Of String) = New List(Of String)

            For Each DomainId In InteractionDomains
                Dim LQuery = From Pro As Protein
                             In Proteins
                             Where Pro.ContainsDomain(DomainId)
                             Select Pro.Identifier '
                Call InteractionProteins.AddRange(LQuery.ToArray)
            Next

            Call InteractionList.Add(New Interaction With {.Protein = Protein.Identifier, .InteractionWith = InteractionProteins.ToArray})
        Next

        Return InteractionList.ToArray
    End Function
End Class
