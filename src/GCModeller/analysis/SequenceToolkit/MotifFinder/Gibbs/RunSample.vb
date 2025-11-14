Imports Microsoft.VisualBasic.Language

Friend Class RunSample

    Friend ReadOnly sampler As GibbsSampler
    Friend ReadOnly sequences As String()
    Friend ReadOnly maxInformationContent As Value(Of Double) = Double.NegativeInfinity
    Friend ReadOnly predictedMotifs As New List(Of String)
    Friend ReadOnly predictedSites As New List(Of Integer)

    Sub New(gibbs As GibbsSampler)
        sampler = gibbs
        sequences = gibbs.m_sequences.Select(Function(fa) fa.SequenceData).ToArray
    End Sub

    Public Sub RunOne(maxIterations As Integer)
        SyncLock maxInformationContent
            If CDbl(maxInformationContent) / sampler.m_motifLength = 2.0 Then
                Return
            End If
        End SyncLock

        Dim sites As IList(Of Integer) = sampler.gibbsSample(maxIterations, New List(Of String)(sequences))
        Dim motifs As IList(Of String) = sampler.getMotifStrings(sequences, sites)
        Dim informationContent = sampler.informationContent(motifs)
        Dim newMax As Boolean

        SyncLock maxInformationContent
            newMax = informationContent >= CDbl(maxInformationContent)
        End SyncLock

        If newMax Then
            Dim s As String = sites.Select(Function(k) k.ToString).JoinBy(" ")

            SyncLock maxInformationContent
                maxInformationContent.Value = informationContent
            End SyncLock
            SyncLock predictedSites
                predictedSites.Clear()
                predictedSites.AddRange(sites)
            End SyncLock
            SyncLock predictedMotifs
                predictedMotifs.Clear()
                predictedMotifs.AddRange(motifs)
            End SyncLock

            Call VBDebugger.EchoLine(informationContent.ToString() & " :: " & s)
        End If
    End Sub
End Class
