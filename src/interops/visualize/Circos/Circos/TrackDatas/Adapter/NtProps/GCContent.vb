#Region "Microsoft.VisualBasic::636020dbb0194da36f4c2c8b15085bc8, visualize\Circos\Circos\TrackDatas\Adapter\NtProps\GCContent.vb"

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

    '     Class GeneGCContent
    ' 
    '         Properties: SourceFasta
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

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

        Sub New(genome As IEnumerable(Of FastaSeq),
                karyotype As Karyotype.SkeletonInfo,
                winSize As Integer,
                steps As Integer,
                getValue As Func(Of NASegment_GC, Double))

            Dim list As New List(Of ValueTrackData)
            Dim chrs = karyotype.Karyotypes.ToDictionary(
                Function(x) x.chrLabel,
                Function(x) x.chrName)

            For Each nt As FastaSeq In genome
                Dim raw As Double() = GCProps.GetGCContentForGENOME(
                    nt,
                    winSize,
                    steps).Select(getValue).ToArray
                Dim chr As String = nt.Title.Split("."c).First

                chr = chrs(chr)
                list += GCSkew.trackValues(chr, GCSkew.means(raw), [steps])

                Call Console.Write(">")
            Next

            source = list
        End Sub
    End Class
End Namespace
