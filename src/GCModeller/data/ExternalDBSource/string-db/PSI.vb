Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Data.StringDB.SimpleCsv
Imports SMRUCC.genomics.foundation.psidev.XML

Namespace StringDB

    Public Module PSI

        <Extension>
        Public Function ExtractNetwork(mif25 As EntrySet) As PitrNode()
            Dim LQuery As PitrNode() = LinqAPI.Exec(Of PitrNode) <=
                From entry As Entry
                In mif25.Entries
                Select entry.__extractNetwork(mif25)

            Return LQuery
        End Function

        <Extension>
        Private Function __extractNetwork(entry As Entry, mif25 As EntrySet) As SimpleCsv.PitrNode()
            Dim LQuery As PitrNode() = LinqAPI.Exec(Of PitrNode) <=
                From interacts As Interaction
                In entry.InteractionList
                Select __extractEdge(mif25, interacts)

            Return LQuery
        End Function

        <Extension>
        Private Function __extractEdge(mif25 As EntrySet, Interaction As Interaction) As PitrNode
            Dim Node As New PitrNode
            Node.Confidence = Interaction.ConfidenceList.First.value
            Node.FromNode = mif25.GetInteractor(Interaction.ParticipantList.First.InteractorRef).Xref.primaryRef.id
            Node.ToNode = mif25.GetInteractor(Interaction.ParticipantList.Last.InteractorRef).Xref.primaryRef.id
            Return Node
        End Function

        Public Function ExtractNetwork([Imports] As String) As StringDB.SimpleCsv.PitrNode()
            Dim files As IEnumerable(Of String) = ls - l - r - wildcards("*.xml") <= [Imports]
            Dim net As PitrNode() = LinqAPI.Exec(Of PitrNode) <=
                From file As String
                In files.AsParallel
                Let mif25 As EntrySet = file.LoadXml(Of EntrySet)()
                Select edges = mif25.ExtractNetwork

            net = net.TrimNull

            Return net
        End Function
    End Module
End Namespace