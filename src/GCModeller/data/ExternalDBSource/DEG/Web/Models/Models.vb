Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq

Namespace DEG.Web

    <XmlType("essentialGene")> Public Class EssentialGene

        <XmlAttribute> Public Property ID As String
        <XmlAttribute> Public Property Name As String

        Public Property FunctionDescrib As String
        Public Property Organism As String
        Public Property geneRef As String
        Public Property RefSeq As String
        Public Property UniProt As String
        Public Property COG As String
        Public Property GO As String()
        Public Property FuncClass As String
        Public Property Reference As String
        Public Property Condition As String
        Public Property Nt As String
        Public Property Aa As String

    End Class

    <XmlRoot("genome")> Public Class Genome : Inherits XmlDataModel
        Implements Enumeration(Of EssentialGene)

        <XmlAttribute> Public Property ID As String
        Public Property Organism As String
        <XmlAttribute> Public Property numOfDEG As Integer
        <XmlAttribute> Public Property Conditions As String
        Public Property Reference As String

        Public Property summary As Summary

        <XmlElement>
        Public Property EssentialGenes As EssentialGene()

        Public Iterator Function GenericEnumerator() As IEnumerator(Of EssentialGene) Implements Enumeration(Of EssentialGene).GenericEnumerator
            For Each gene As EssentialGene In EssentialGenes
                Yield gene
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of EssentialGene).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class

End Namespace