#Region "Microsoft.VisualBasic::70b709b76a9f2023266f887f9875da3e, ..\interops\visualize\Circos\Circos\TrackDatas\Adapter\NtProps\GenomeGCContent.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports SMRUCC.genomics.NCBI.Extensions
Imports SMRUCC.genomics.NCBI.Extensions.NCBIBlastResult
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.ISequenceModel
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace TrackDatas.NtProps

    Public Class GenomeGCContent : Inherits data(Of NASegment_GC)

        Sub New(SourceFasta As FASTA.FastaToken, Optional SegmentLength As Integer = -1, Optional steps As Integer = 10)
            Call MyBase.New(
                __sourceGC(SourceFasta,
                         If(SegmentLength <= 0, 10, SegmentLength),
                         steps))
        End Sub

        Private Overloads Shared Function __sourceGC(nt As FASTA.FastaToken, segLen As Integer, steps As Integer) As NASegment_GC()
            Dim source As NASegment_GC() =
                GCProps.GetGCContentForGENOME(nt, winSize:=segLen, steps:=steps)
            Dim avg As Double = (From n In source Select n.value).Average

            For Each x As NASegment_GC In source
                x.value -= avg
            Next

            Return source
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
