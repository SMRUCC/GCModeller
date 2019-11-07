Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Terminal.ProgressBar
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Class ReactionClass : Inherits XmlDataModel

        <XmlAttribute>
        Public Property entryId As String
        Public Property definition As String
        Public Property reactantPairs As ReactionCompoundTransform()
        Public Property reactions As NamedValue()
        Public Property enzymes As NamedValue()
        Public Property pathways As NamedValue()
        Public Property orthology As NamedValue()

        Public Shared Iterator Function ScanRepository(repository As String) As IEnumerable(Of ReactionClass)
            Dim busy As New SwayBar
            Dim message$
            Dim [class] As ReactionClass
            Dim loaded As New Index(Of String)

            For Each xml As String In ls - l - r - "*.xml" <= repository
                [class] = xml.LoadXml(Of ReactionClass)
                message = [class].definition

                Call busy.Step(message)

                If Not [class].entryId Like loaded Then
                    loaded.Add([class].entryId)

                    ' return current file data
                    Yield [class]
                End If
            Next
        End Function
    End Class

    Public Class ReactionCompoundTransform

        <XmlAttribute> Public Property from As String
        <XmlAttribute> Public Property [to] As String

        Public Overrides Function ToString() As String
            Return $"{from}->{[to]}"
        End Function
    End Class
End Namespace