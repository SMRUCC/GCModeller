#Region "Microsoft.VisualBasic::a2b5891f5256724319afe1091d0a8bcd, ..\CLI_tools\c2\Workflows\RegulationNetwork\GenomeWildRandomParser.vb"

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

''' <summary>
''' 根据WGCNA权重数据在基因组中解析所有尽可能多的组合形式以用于调控位点的分析
''' </summary>
''' <remarks></remarks>
Public Class GenomeWildRandomParser : Inherits PromoterParser

    Sub New(DoorOperons As LANS.SystemsBiology.Assembly.Door.OperonView, GenomeSequence As String, Optional SegmentLengthArray As Integer() = Nothing)
        Call MyBase.New(DoorOperons, GenomeSequence, SegmentLengthArray)
    End Sub

    Public Sub TryParse(WGCNAWeights As c2.WGCNAWeight, WeightCutoff As Double, SplitUnits As Integer, ExportDir As String)
        For Each Operon In MyBase.DoorOperons.Operons
            Dim OperonIds As String() = GetOperonsCutoff(Operon, WGCNAWeights, Cutoffvalue:=WeightCutoff)
            Dim RndChunkBuffer = RandomSplit(OperonIds, 20, SplitUnits)

            Call Console.WriteLine("xcb_door_{0}", Operon.ToString)

            For i As Integer = 0 To RndChunkBuffer.Count - 1

                For Each SegmentLength In MyBase.OperonPromoters.Keys
                    Dim SavedFile As String = String.Format("{0}/{1}_bp/xcb_door_{2}_{3}.fsa", ExportDir, SegmentLength.ToString, Operon.Key, i + 1)
                    Dim PromoterSegments = MyBase.OperonPromoters(SegmentLength)
                    Dim FastaData As LANS.SystemsBiology.SequenceModel.FASTA.FastaFile = (From Id As String In RndChunkBuffer(i) Select PromoterSegments(Id)).ToArray
                    Call FastaData.Save(SavedFile)
                Next
            Next
        Next
    End Sub

    Private Shared Function RandomSplit(IdList As String(), SplitCounts As Integer, SplitUnits As Integer) As String()()
        If SplitUnits >= IdList.Count Then
            Return New String()() {IdList}
        End If

        Dim ArrayList As List(Of String()) = New List(Of String())

        For i As Integer = 1 To SplitCounts
            Dim ChunkBuffer As String() = New String(SplitUnits - 1) {}
            Dim TempChunk As List(Of String) = IdList.ToList

            For j As Integer = 0 To SplitUnits - 1
                Dim idx As Integer = RandomDouble() * (TempChunk.Count - 1)
                Call Randomize()
                ChunkBuffer(j) = TempChunk(idx)
                Call TempChunk.RemoveAt(idx)
            Next

            Call ArrayList.Add(ChunkBuffer)
        Next
        Return ArrayList.ToArray
    End Function

    ''' <summary>
    ''' 对Operon里面的基因进行遍历，查找出所有共表达权重大于阈值的基因，最后将基因转化为操纵子
    ''' </summary>
    ''' <param name="Operon"></param>
    ''' <param name="WGCNAWeight"></param>
    ''' <param name="Cutoffvalue"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetOperonsCutoff(Operon As LANS.SystemsBiology.Assembly.Door.Operon, WGCNAWeight As c2.WGCNAWeight, Cutoffvalue As Double) As String()
        Dim GeneIdList As List(Of String) = New List(Of String)
        '获取所有权重大于阈值的基因
        Dim FilteringWeights = (From Gene In Operon.Value.AsParallel
                                Let Weights = WGCNAWeight.Find(Gene.Synonym, Cutoffvalue)
                                Where Not Weights.IsNullOrEmpty
                                Let GetId = Function() As String()
                                                Dim IdList As String() = (From item In Weights Select item.GetOpposite(Gene.Synonym)).ToArray
                                                Return IdList
                                            End Function Select GetId()).ToArray
        For Each ChunkBuffer In FilteringWeights
            Call GeneIdList.AddRange(ChunkBuffer)
        Next

        'For Each Gene In Operon.Value
        '    Dim Weights = WGCNAWeight.Find(Gene.Synonym) '获取所有权重大于阈值的基因
        '    If Not Weights.IsNullOrEmpty Then
        '        Call GeneIdList.AddRange((From item In Weights Select item.GetOpposite(Gene.Synonym)).ToArray)
        '    End If
        'Next

        GeneIdList = GeneIdList.Distinct.ToList

        Dim LQuery = (From Id As String In GeneIdList Select MyBase.DoorOperons.Select(Id).Key Distinct).ToList  '将基因转化为操纵子的Id
        If Not LQuery.IndexOf(Operon.Key) > -1 Then Call LQuery.Add(Operon.Key)
        Return LQuery.ToArray
    End Function
End Class
