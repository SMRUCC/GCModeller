#Region "Microsoft.VisualBasic::ed75753720ee589643700717af2f44bb, analysis\Motifs\MotifGraph\MotifTree\MotifComparison.vb"

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

    '   Total Lines: 44
    '    Code Lines: 35 (79.55%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (20.45%)
    '     File Size: 1.57 KB


    ' Class MotifComparison
    ' 
    '     Properties: motifIDs
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetObject, GetSimilarity, SmithWatermanAlignment
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns

Public Class MotifComparison : Inherits ComparisonProvider

    ReadOnly motifs As New Dictionary(Of String, Probability)

    Public ReadOnly Property motifIDs As IEnumerable(Of String)
        Get
            Return motifs.Keys
        End Get
    End Property

    Public Sub New(motifs As IEnumerable(Of Probability), equals As Double, gt As Double)
        MyBase.New(equals, gt)

        For Each motif As Probability In motifs.SafeQuery
            Call Me.motifs.Add(motif.name, motif)
        Next
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetSimilarity(x As String, y As String) As Double
        Return SmithWatermanAlignment(motifs(x), motifs(y))
    End Function

    Private Function SmithWatermanAlignment(pwm1 As Probability, pwm2 As Probability) As Double
        Dim top As Match = SmithWaterman.MakeAlignment(pwm1.region, pwm2.region, top:=1, norm:=True).FirstOrDefault

        If top Is Nothing Then
            Return 0
        Else
            Return top.score
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetObject(id As String) As Object
        Return motifs.TryGetValue(id)
    End Function
End Class

