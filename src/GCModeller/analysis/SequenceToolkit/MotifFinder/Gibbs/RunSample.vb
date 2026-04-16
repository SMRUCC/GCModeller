#Region "Microsoft.VisualBasic::e4298780ea1a792d5c4a320bca7e2ef8, analysis\SequenceToolkit\MotifFinder\Gibbs\RunSample.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 50
    '    Code Lines: 41 (82.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (18.00%)
    '     File Size: 1.85 KB


    ' Class RunSample
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: RunOne
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language

Friend Class RunSample

    Friend ReadOnly sampler As GibbsSampler
    Friend ReadOnly sequences As String()
    Friend ReadOnly maxInformationContent As Value(Of Double) = Double.NegativeInfinity
    Friend ReadOnly predictedMotifs As New List(Of String)
    Friend ReadOnly predictedSites As New List(Of Integer)

    Sub New(gibbs As GibbsSampler)
        sampler = gibbs
        sequences = gibbs.Sequences.ToArray
    End Sub

    Public Sub RunOne(maxIterations As Integer)
        SyncLock maxInformationContent
            If CDbl(maxInformationContent) / sampler.m_motifLength = 2.0 Then
                Return
            End If
        End SyncLock

        Dim sites As List(Of Integer) = sampler.gibbsSample(maxIterations, New List(Of String)(sequences))
        Dim motifs As System.Collections.Generic.List(Of String) = sampler.getMotifStrings(sequences, sites)
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

