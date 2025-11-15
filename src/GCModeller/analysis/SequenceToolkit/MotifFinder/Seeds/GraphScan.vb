#Region "Microsoft.VisualBasic::b5ecdbff9ef151d1c2559c6b07d3f6be, analysis\SequenceToolkit\MotifFinder\Seeds\GraphScan.vb"

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

'   Total Lines: 39
'    Code Lines: 30 (76.92%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 9 (23.08%)
'     File Size: 1.59 KB


' Class GraphScan
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: GetSeeds, GetSequenceIndex
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class GraphScan : Inherits SeedScanner

    Public Sub New(param As PopulatorParameter, debug As Boolean)
        MyBase.New(param, debug)
    End Sub

    Private Function GetSequenceIndex(regions() As FastaSeq) As Dictionary(Of String, FastaSeq)
        Dim index As New Dictionary(Of String, FastaSeq)

        For Each seq As FastaSeq In regions
            index(seq.Title) = seq
        Next

        Return index
    End Function

    Public Overrides Iterator Function GetSeeds(regions() As FastaSeq) As IEnumerable(Of HSP)
        Dim source = GetSequenceIndex(regions)
        Dim tree = SeedCluster.BuildAVLTreeCluster(regions.Select(Function(f) New NamedValue(Of String)(f.Title, f.SequenceData)), cutoff:=0.7)
        Dim groups = tree.PopulateNodes.Select(Function(t, i) New NamedCollection(Of String)((i + 1).ToString, t.Members)).ToArray
        Dim full As New FullScan(param, debug)

        Call param.logText($"get total {groups.Length} sequence groups!")

        For Each seqs As NamedCollection(Of String) In groups
            Call param.logText($"> processing sequence group with {seqs.Length} sequence.")

            If seqs.Length >= 2 Then
                For Each seed As HSP In full.GetSeeds(seqs.Select(Function(i) source(i)).ToArray)
                    Yield seed
                Next
            End If
        Next
    End Function
End Class
