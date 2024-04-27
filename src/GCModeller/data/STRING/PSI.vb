#Region "Microsoft.VisualBasic::d87e4432ae52732064783db99514e816, G:/GCModeller/src/GCModeller/data/STRING//PSI.vb"

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

    '   Total Lines: 53
    '    Code Lines: 41
    ' Comment Lines: 3
    '   Blank Lines: 9
    '     File Size: 1.95 KB


    ' Module PSI
    ' 
    '     Function: __extractEdge, __extractNetwork, (+2 Overloads) ExtractNetwork
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Data.STRING.SimpleCsv
Imports SMRUCC.genomics.foundation.psidev.XML

''' <summary>
''' PSI network file extensions API.
''' </summary>
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
        Node.value = Interaction.ConfidenceList.First.value
        Node.FromNode = mif25.GetInteractor(Interaction.ParticipantList.First.InteractorRef).Xref.primaryRef.id
        Node.ToNode = mif25.GetInteractor(Interaction.ParticipantList.Last.InteractorRef).Xref.primaryRef.id
        Return Node
    End Function

    Public Function ExtractNetwork([Imports] As String) As SimpleCsv.PitrNode()
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
