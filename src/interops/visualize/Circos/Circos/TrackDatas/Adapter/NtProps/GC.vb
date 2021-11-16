#Region "Microsoft.VisualBasic::3f8532fc17f6682f9f40edaac1cb582e, visualize\Circos\Circos\TrackDatas\Adapter\NtProps\GC.vb"

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

    '     Module GCProps
    ' 
    '         Function: GetGCContentForGenes, GetGCContentForGENOME
    ' 
    '     Class GeneObjectGC
    ' 
    '         Properties: Title
    ' 
    '     Class NASegment_GC
    ' 
    '         Properties: AT, GC_AT, length, value
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcidStaticsProperty

Namespace TrackDatas.NtProps

    ''' <summary>
    ''' GC% calculation tools for drawing the circos elements.
    ''' </summary>
    ''' 
    <Package("Circos.Nt.Attributes")>
    Public Module GCProps

        ''' <summary>
        ''' Calculate GC% for each gene its fasta sequence in the <paramref name="FASTA"/>.
        ''' </summary>
        ''' <param name="FASTA"></param>
        ''' <returns></returns>
        <ExportAPI("Get.Genes.GC")>
        <Extension>
        Public Function GetGCContentForGenes(FASTA As IEnumerable(Of FastaSeq)) As GeneObjectGC()
            Dim LQuery As GeneObjectGC() =
                LinqAPI.Exec(Of GeneObjectGC) <= From fa As FastaSeq
                                                 In FASTA
                                                 Let gc As Double = GCContent(fa.SequenceData.ToUpper)
                                                 Let at As Double = 1 - gc
                                                 Select New GeneObjectGC With {
                                                     .Title = fa.Headers.First,
                                                     .value = gc,
                                                     .AT = at,
                                                     .GC_AT = (gc / at)
                                                 }
            Return LQuery
        End Function

        <ExportAPI("Get.Genome.GC")>
        Public Function GetGCContentForGENOME(FASTA As FastaSeq, winSize As Integer, steps As Integer) As NASegment_GC()
            Dim NT As DNA() = NucleicAcid.CreateObject(FASTA.SequenceData).ToArray
            Dim slideWins = NT.CreateSlideWindows(winSize, offset:=steps)
            Dim LQuery As List(Of NASegment_GC) = LinqAPI.MakeList(Of NASegment_GC) <=
 _
                From seg As SlideWindow(Of DNA)
                In slideWins
                Let gc As Double = seg.GC_Content
                Let at As Double = 1 - gc
                Select New NASegment_GC With {
                    .start = seg.Left,
                    .end = seg.Right,
                    .length = seg.Length,
                    .value = gc,
                    .AT = at,
                    .GC_AT = (gc / at)
                }

            Dim LastSegment As New List(Of DNA)(slideWins.Last.Items)
            Dim tmp As List(Of DNA)
            Dim p As Integer = LQuery.Last.start

            For i As Integer = 0 To LastSegment.Count - 1 Step steps
                tmp = New List(Of DNA)(LastSegment.Skip(i))
                tmp += NT.Take(i)
                LQuery += New NASegment_GC With {
                            .start = p + i,
                            .length = winSize,
                            .end = p + i + winSize,
                            .value = tmp.GC_Content
                }
            Next

            Return LQuery.ToArray
        End Function
    End Module

    Public Class GeneObjectGC : Inherits NASegment_GC
        Implements INamedValue

        Public Property Title As String Implements INamedValue.Key
    End Class

    ''' <summary>
    ''' <see cref="ValueTrackData.value"/> is GC% value.
    ''' </summary>
    Public Class NASegment_GC : Inherits ValueTrackData

        ''' <summary>
        ''' GC%
        ''' </summary>
        ''' <returns></returns>
        <Column("GC%")> Public Overrides Property value As Double
        ''' <summary>
        ''' AT%
        ''' </summary>
        ''' <returns></returns>
        <Column("AT%")> Public Property AT As Double

        ''' <summary>
        ''' GC/AT ratio
        ''' </summary>
        ''' <returns></returns>
        <Column("GC/AT")> Public Property GC_AT As Double
        ''' <summary>
        ''' The size of this fragment
        ''' </summary>
        ''' <returns></returns>
        Public Property length As Integer
    End Class
End Namespace
