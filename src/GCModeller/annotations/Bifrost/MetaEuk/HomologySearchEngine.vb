#Region "Microsoft.VisualBasic::8ae26edaefd49a16c663e80a8db3d969, annotations\Bifrost\MetaEuk\HomologySearchEngine.vb"

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

    '   Total Lines: 90
    '    Code Lines: 62 (68.89%)
    ' Comment Lines: 7 (7.78%)
    '    - Xml Docs: 57.14%
    ' 
    '   Blank Lines: 21 (23.33%)
    '     File Size: 2.89 KB


    ' Class HomologySearchEngine
    ' 
    '     Properties: nsize
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: GetResult, SearchAll
    ' 
    '     Sub: Search, Solve
    ' 
    ' /********************************************************************************/

#End Region


' ========================================================================
' MODULE 5: HOMOLOGY SEARCH ENGINE
' ========================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports Microsoft.VisualBasic.Parallel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class HomologySearchEngine : Inherits VectorTask

    ReadOnly references As FastaSeq()
    ReadOnly hits As New List(Of HomologyHit)
    ReadOnly config As MetaEukConfig

    Dim frag As CandidateFragment

    Public ReadOnly Property nsize As Integer
        Get
            Return hits.Count
        End Get
    End Property

    Public Sub New(references As IReadOnlyCollection(Of FastaSeq), config As MetaEukConfig)
        MyBase.New(references.Count)
        Me.references = references.ToArray()
        Me.config = config
    End Sub

    Shared Sub New()
        VectorTask.n_threads = 12
    End Sub

    Public Sub Search(frag As CandidateFragment)
        Me.frag = frag
        Call Run()
    End Sub

    Public Function GetResult() As IEnumerable(Of HomologyHit)
        Return hits
    End Function

    Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
        Dim hits As New List(Of HomologyHit)
        Dim gapOpen As Integer = 11
        Dim gapExtend As Integer = 1

        For i As Integer = start To ends
            Dim refSeq As FastaSeq = references(i)
            Dim hit = SmithWatermanAligner.Align(
                  frag.Peptide, refSeq.SequenceData, gapOpen, gapExtend, config)

            If hit IsNot Nothing Then
                hit.Fragment = frag
                hit.TargetID = refSeq.locus_tag
                hits.Add(hit)
            End If
        Next

        If hits.Any Then
            SyncLock Me.hits
                Call Me.hits.AddRange(hits)
            End SyncLock
        End If
    End Sub

    ''' <summary>
    ''' Search all candidate fragments against reference protein database.
    ''' Returns list of significant homology hits.
    ''' </summary>
    Public Shared Function SearchAll(
        fragments As List(Of CandidateFragment),
        references As List(Of FastaSeq),
        config As MetaEukConfig) As List(Of HomologyHit)

        Dim align As New HomologySearchEngine(references, config)

        Console.WriteLine($"[INFO] Searching {fragments.Count} fragments against {references.Count} reference proteins...")

        For Each frag As CandidateFragment In TqdmWrapper.Wrap(fragments)
            Call align.Search(frag)
        Next

        Console.WriteLine($"[INFO] Found {align.nsize} significant homology hits")

        Return align.GetResult
    End Function

End Class

