#Region "Microsoft.VisualBasic::4b13f714e234e02386a8aa19fc0b8a07, CLI_tools\c2\Workflows\RegulationNetwork\PromoterParser.vb"

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

    ' Class PromoterParser
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: TryParse
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Public Class PromoterParser : Implements System.IDisposable

    Protected OperonPromoters As SortedDictionary(Of Integer, SortedDictionary(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)) =
        New SortedDictionary(Of Integer, SortedDictionary(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken))
    Protected DoorOperons As LANS.SystemsBiology.Assembly.Door.OperonView

    Sub New(DoorOperons As LANS.SystemsBiology.Assembly.Door.OperonView, GenomeSequence As String, Optional SegmentLengthArray As Integer() = Nothing)
        If SegmentLengthArray.IsNullOrEmpty Then
            SegmentLengthArray = New Integer() {150, 200, 250, 300}
        End If

        Dim SequenceData = New SortedDictionary(Of Integer, SortedDictionary(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken))
        Dim GenomeSequenceData As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader =
            New LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader(LANS.SystemsBiology.SequenceModel.FASTA.FastaToken.Load(GenomeSequence), False)

        For Each SegmentLength As Integer In SegmentLengthArray
            Call SequenceData.Add(SegmentLength, TryParse(DoorOperons, GenomeSequenceData, SegmentLength))
        Next

        Me.DoorOperons = DoorOperons
        Me.OperonPromoters = SequenceData
    End Sub

    Private Shared Function TryParse(DoorOperons As LANS.SystemsBiology.Assembly.Door.OperonView, GenomeSequence As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader, SegmentLength As Integer) _
        As SortedDictionary(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)

        Dim SequenceData As SortedDictionary(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken) =
            New SortedDictionary(Of String, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)

        For Each Operon In DoorOperons.Operons
            Dim FirstGene = Operon.FirstGene, Location = FirstGene.Location

            Dim PromoterFsa As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken =
                New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {
                    .Attributes = New String() {
                        String.Format("{0} [OperonPromoter={1}; {2}] [OperonGenes={3}]",
                                      Operon.Key,
                                      FirstGene.Synonym,
                                      FirstGene.Location.ToString,
                                      LANS.SystemsBiology.Assembly.Door.OperonView.GenerateLstIdString(Operon))
                    }
                }
            If Location.Strand = LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation.Strands.Forward Then
                PromoterFsa.SequenceData = GenomeSequence.TryParse(CInt(Location.Left), SegmentLength, False)
            Else
                PromoterFsa.SequenceData = GenomeSequence.ReadComplement(Location.Right, SegmentLength, True)
            End If

            Call SequenceData.Add(Operon.Key, PromoterFsa)
        Next

        Return SequenceData
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 检测冗余的调用

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO:  释放托管状态(托管对象)。
            End If

            ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
            ' TODO:  将大型字段设置为 null。
        End If
        Me.disposedValue = True
    End Sub

    ' TODO:  仅当上面的 Dispose(ByVal disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
    'Protected Overrides Sub Finalize()
    '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose(ByVal disposing As Boolean)中。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Visual Basic 添加此代码是为了正确实现可处置模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
