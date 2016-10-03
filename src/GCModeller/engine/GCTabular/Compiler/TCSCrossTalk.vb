#Region "Microsoft.VisualBasic::d27aa4ae249142611efdf704bcf17096, ..\GCModeller\engine\GCTabular\Compiler\TCSCrossTalk.vb"

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

'Module TCSCrossTalk

'    ''' <summary>
'    ''' 扫描每一个Operon，对于同一个Operon之中的TCS对都取出来，他们是同源的TCS之间的互作权重为1，对于不同的Operon之间的TCS，则根据计算结果来取值
'    ''' </summary>
'    ''' <param name="Door"></param>
'    ''' <param name="CrossTalkProfile"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Function Assemble(Door As SMRUCC.genomics.Assembly.Door.Door,
'                             MisT2 As SMRUCC.genomics.Assembly.MiST2.WebRequestHandler.SignalTransductionProfile,
'                             CrossTalkProfile As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File) As Microsoft.VisualBasic.Datavisualization.Network.FileStream.NetworkNode()

'        Dim CrossTalks As List(Of Microsoft.VisualBasic.Datavisualization.Network.FileStream.NetworkNode) =
'            New List(Of Microsoft.VisualBasic.Datavisualization.Network.FileStream.NetworkNode)

'        For Each Operon In Door.DoorOperonView  '组合同源的TCS
'            Dim Hks = (From Gene In Operon Where MisT2.IsHisK(Gene.Value.Synonym) Select Gene.Value.Synonym).ToArray
'            Dim RRs = (From Gene In Operon Where MisT2.IsRR(Gene.Value.Synonym) Select Gene.Value.Synonym).ToArray

'            If Hks.IsNullOrEmpty OrElse RRs.IsNullOrEmpty Then
'                Continue For
'            End If

'            For Each Hk As String In Hks
'                Call CrossTalks.AddRange((From RR As String
'                                          In RRs
'                                          Select New Microsoft.VisualBasic.Datavisualization.Network.FileStream.NetworkNode With {
'                                              .Confidence = 1,
'                                              .FromNode = Hk,
'                                              .ToNode = RR}).ToArray)
'            Next
'        Next

'        Dim Reader As SMRUCC.genomics.Toolkits.RNASeq.ChipData = SMRUCC.genomics.Toolkits.RNASeq.ChipData.Load(CrossTalkProfile)
'        For Each RR As String In Reader.ExperimentIdlist
'            Call Reader.SetColumnAuto(RR)

'            Dim LQuery = (From Hk As String
'                          In Reader.GeneIdlist
'                          Let c = Reader.GetValue(Hk)
'                          Where c <> 0
'                          Select New Microsoft.VisualBasic.Datavisualization.Network.FileStream.NetworkNode With {.FromNode = Hk, .ToNode = RR, .Confidence = c}).ToArray
'            Call CrossTalks.AddRange(LQuery)
'        Next

'        Return CrossTalks.ToArray
'    End Function
'End Module
