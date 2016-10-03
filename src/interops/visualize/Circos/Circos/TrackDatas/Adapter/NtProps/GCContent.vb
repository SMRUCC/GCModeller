#Region "Microsoft.VisualBasic::8d9abadc2af6627e400f222270e9e0d0, ..\interops\visualize\Circos\Circos\TrackDatas\Adapter\NtProps\GCContent.vb"

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

Imports System.Text
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.ISequenceModel
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Linq

Namespace TrackDatas.NtProps

    ''' <summary>
    ''' 每一个基因的GC%的表述
    ''' </summary>
    Public Class GeneGCContent : Inherits data(Of ValueTrackData)

        Public ReadOnly Property SourceFasta As FastaFile

        Sub New(Source As FastaFile)
            Call MyBase.New(
                GCProps.GetGCContentForGenes(Source) _
                       .Select(
                       Function(x) DirectCast(x, ValueTrackData)))
            SourceFasta = Source
        End Sub

        Sub New(genome As IEnumerable(Of FastaToken),
                karyotype As Karyotype.SkeletonInfo,
                winSize As Integer,
                steps As Integer,
                getValue As Func(Of NASegment_GC, Double))

            Dim list As New List(Of ValueTrackData)
            Dim chrs = karyotype.Karyotypes.ToDictionary(
                Function(x) x.chrLabel,
                Function(x) x.chrName)

            For Each nt As FastaToken In genome
                Dim raw As Double() = GCProps.GetGCContentForGENOME(
                    nt,
                    winSize,
                    steps).ToArray(getValue)
                Dim chr As String = nt.Title.Split("."c).First

                chr = chrs(chr)
                list += GCSkew.__sourceGC(chr, GCSkew.__avgData(raw), [steps])

                Call Console.Write(">")
            Next

            __source = list
        End Sub
    End Class
End Namespace
