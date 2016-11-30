#Region "Microsoft.VisualBasic::7eb5ea7895661c03ecb4f7276b7e89c3, ..\interops\visualize\Circos\Circos\TrackDatas\Adapter\NtProps\GenomeGCContent.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
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

Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace TrackDatas.NtProps

    ''' <summary>
    ''' 整个基因组范围内的GC%变化的情况
    ''' </summary>
    Public Class GenomeGCContent : Inherits data(Of ValueTrackData)

        Sub New(nt As FastaToken, Optional SegmentLength As Integer = -1, Optional steps As Integer = 10, Optional avg As Boolean = True)
            Call MyBase.New(
                __sourceGC(nt, If(SegmentLength <= 0, 10, SegmentLength), steps, avg))
        End Sub

        Sub New(data As IEnumerable(Of NASegment_GC))
            MyBase.New(data)
        End Sub

        Private Overloads Shared Function __sourceGC(nt As FastaToken, segLen As Integer, steps As Integer, usingAvg As Boolean) As NASegment_GC()
            Dim source As NASegment_GC() = GCProps.GetGCContentForGENOME(
                nt,
                winSize:=segLen,
                steps:=steps)
            Dim avg As Double = source.Average(Function(n) n.value)

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
