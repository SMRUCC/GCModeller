#Region "Microsoft.VisualBasic::48f9f8ba7b73a1f12fd802e2dee98e16, CLI_tools\c2\Workflows\RegulationNetwork\RandomParser.vb"

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

    ' Class RandomParser
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: Generate
    ' 
    ' /********************************************************************************/

#End Region

Public Class RandomParser

    ''' <summary>
    ''' {OperonId, {SegmentLength, OperonPromoterRegion}}
    ''' </summary>
    ''' <remarks></remarks>
    Dim OperonPromoterRegions As List(Of KeyValuePair(Of String, KeyValuePair(Of Integer, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)())) =
        New List(Of KeyValuePair(Of String, KeyValuePair(Of Integer, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)()))

    Dim SegmentLengthArray As Integer()

    Sub New(Door As LANS.SystemsBiology.Assembly.Door.Door, MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, SegmentLengthArray As Integer())
        Dim Genome As LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader = New LANS.SystemsBiology.SequenceModel.NucleotideModels.SegmentReader(MetaCyc.Database.FASTAFiles.Origin, False)

        For Each Operon In Door.DoorOperonView.Operons
            Dim FirstGene = Operon.FirstGene
            Dim Location = FirstGene.Location

            Dim LQuery = (From SegmentLength As Integer In SegmentLengthArray
                          Let Parse = Function() As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
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
                                              PromoterFsa.SequenceData = Genome.TryParse(Location.Left, SegmentLength, directionDownstream:=False)
                                          Else
                                              PromoterFsa.SequenceData = Genome.ReadComplement(Location.Right, SegmentLength, True)
                                          End If
                                          Return PromoterFsa
                                      End Function Select New KeyValuePair(Of Integer, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)(SegmentLength, Parse())).ToArray
            Call OperonPromoterRegions.Add(New KeyValuePair(Of String, KeyValuePair(Of Integer, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)())(Operon.Key, LQuery))
        Next
        Me.SegmentLengthArray = SegmentLengthArray
    End Sub

    ''' <summary>
    ''' 生成随机的配对，并导出到目标文件夹
    ''' </summary>
    ''' <param name="ExportedDir"></param>
    ''' <param name="ObjectCounts">总共所需要生成的对象数目</param>
    ''' <param name="Units">每一个组合中的对象的数目</param>
    ''' <remarks></remarks>
    Public Sub Generate(ExportedDir As String, ObjectCounts As Long, Units As Integer)
        For Each Length As Integer In Me.SegmentLengthArray
            Dim SegmentArray = (From item In Me.OperonPromoterRegions
                                Let [Select] = Function() As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
                                                   Dim LQuery = (From itemObject As KeyValuePair(Of Integer, LANS.SystemsBiology.SequenceModel.FASTA.FastaToken)
                                                                 In item.Value
                                                                 Where itemObject.Key = Length
                                                                 Select itemObject).First
                                                   Return LQuery.Value
                                               End Function Select [Select]())
            For i As Integer = 0 To ObjectCounts
                Dim ChunkBuffer As List(Of LANS.SystemsBiology.SequenceModel.FASTA.FastaToken) = SegmentArray.ToList
                Dim NewFile As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile =
                    New LANS.SystemsBiology.SequenceModel.FASTA.FastaFile
                Dim path As String = String.Format("{0}/{1}/rnd_{2}.fsa", ExportedDir, Length, i)

                For idx = 0 To Units
                    Dim handle As Integer = RandomDouble() * (ChunkBuffer.Count - 1)
                    Call Randomize()

                    Call NewFile.Add(ChunkBuffer(handle))
                    Call ChunkBuffer.RemoveAt(handle)
                Next

                Call NewFile.Save(path)
            Next
        Next
    End Sub
End Class
