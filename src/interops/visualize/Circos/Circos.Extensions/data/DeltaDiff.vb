#Region "Microsoft.VisualBasic::c5151455371a53828f7d25ffa594e1e9, visualize\Circos\Circos.Extensions\data\DeltaDiff.vb"

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

    '     Class DeltaDiff
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetEnumerator
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ListExtensions
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace Documents.Karyotype.NtProps

    Public Class DeltaDiff : Inherits data(Of ValueTrackData)

        Dim _Steps As Integer
        Dim dbufs As Double()

        Sub New(SequenceModel As IPolymerSequenceModel, SlideWindowSize As Integer, Steps As Integer)
            Me._Steps = Steps
            Dim NT = New NucleotideModels.NucleicAcid(SequenceModel)
            Dim SW = NT.ToArray.CreateSlideWindows(SlideWindowSize, Steps)
            Dim NT_Cache = New NucleicAcid(NT.ToArray)
            Dim ChunkBuffer = (From n In SW.AsParallel
                               Select n.Left,
                                   d = Sigma(NT_Cache, New NucleotideModels.NucleicAcid(n.Items))
                               Order By Left Ascending).ToArray

            Dim LastSegment = SW.Last.Items.AsList
            Dim TempChunk As List(Of NucleotideModels.DNA)
            Dim p As Long = SW.Last.Left
            Dim NT_Array = NT.ToArray
            Dim List = New List(Of Double)

            For i As Integer = 0 To LastSegment.Count - 1 Step Steps
                TempChunk = LastSegment.Skip(i).AsList
                Call TempChunk.AddRange(NT_Array.Take(i).ToArray)
                Call List.Add(Sigma(NT_Cache, New NucleotideModels.NucleicAcid(TempChunk.ToArray)))
            Next

            Dim MergeList = (From item In ChunkBuffer Select item.d).AsList
            Call MergeList.AddRange(List)
            Me.dbufs = MergeList.ToArray
        End Sub

        Public Overrides Iterator Function GetEnumerator() As IEnumerator(Of ValueTrackData)
            Dim p As Integer

            For i As Integer = 0 To dbufs.Length - 1
                Yield New ValueTrackData With {
                    .chr = "chr1",
                    .start = p,
                    .end = p + _Steps,
                    .value = dbufs(i)
                }
                p += _Steps
            Next
        End Function
    End Class
End Namespace
