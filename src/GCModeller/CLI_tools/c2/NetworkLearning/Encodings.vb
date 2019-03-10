#Region "Microsoft.VisualBasic::f0546509c7b6622e6077a47d653c59ee, CLI_tools\c2\NetworkLearning\Encodings.vb"

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

    ' Class Encodings
    ' 
    '     Function: Encoding, GetEncodingStandards, Ndih
    ' 
    '     Sub: (+2 Overloads) EncodingChipData
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 使用标准正态分布进行网络节点水平的编码工具
''' </summary>
''' <remarks></remarks>
Public Class Encodings

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="nd">概率的大小，即标准正态分布的计算结果</param>
    ''' <returns>返回反向计算所得到的标准正太分布函数的随机变量x</returns>
    ''' <remarks></remarks>
    Public Shared Function Ndih(nd As Double) As Double
        Return System.Math.Cosh(nd)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="RPKMS">基因芯片数据</param>
    ''' <param name="pList">概率分布</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetEncodingStandards(RPKMS As Double(), pList As Double()) As Double()
        Dim rpkmMax = RPKMS.Max, rpkmMin = RPKMS.Min
        Dim Length As Double = rpkmMax - rpkmMin
        Dim ONE = System.Math.Abs(Encodings.Ndih(nd:=1))
        Dim XQuery = (From d As Double In (From n In pList Select n Order By n Ascending).ToArray Select Encodings.Ndih(nd:=d) / ONE).ToArray
        Dim LQuery = (From x As Double In XQuery Select Length * x + rpkmMin).ToArray
        Return LQuery
    End Function

    Const CODES = "abcdefghijklmnopqrstuvwxyz"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="RPKMS"></param>
    ''' <param name="EncodingStandards">Generated from the <see cref="GetEncodingStandards">method</see>, 这个必须是按照升序排序的</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Encoding(RPKMS As Double(), EncodingStandards As Double()) As Char()
        Dim ChunkBuffer As Char() = New Char(RPKMS.Count - 1) {}
        For i As Integer = 0 To ChunkBuffer.Count - 1
            Dim ObjectValue As Double = RPKMS(i)

            For encodingId As Integer = 0 To EncodingStandards.Count - 1
                If ObjectValue < EncodingStandards(encodingId) Then
                    ChunkBuffer(i) = CODES(encodingId)
                    Exit For
                End If
            Next
        Next

        Return ChunkBuffer
    End Function

    Public Shared Sub EncodingChipData(ChipData As Microsoft.VisualBasic.ComponentModel.Collection.Generic.IKeyValuePairObject(Of String, Double())(), pList As Double(), ExportDir As String)
        Dim GetEncodingStandardsLQuery = (From item In ChipData Select New KeyValuePair(Of String, Double())(item.Identifier, Encodings.GetEncodingStandards(RPKMS:=item.Value, pList:=pList))).ToArray
        Dim EncodingsLQuery = (From i As Integer In ChipData.Sequence
                               Let est = GetEncodingStandardsLQuery(i)
                               Let rpkms = ChipData(i)
                               Select New KeyValuePair(Of String, Char())(est.Key, Encodings.Encoding(RPKMS:=rpkms.Value, EncodingStandards:=est.Value))).ToArray

        Dim EstCsv = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File From {New String() {"GeneId"}}
        Call EstCsv.First.AddRange((From p In pList Select CStr(p)).ToArray)
        Call EstCsv.AppendRange((From item In GetEncodingStandardsLQuery.AsParallel
                                 Let Generate = Function() As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
                                                    Dim Row As New List(Of String) From {item.Key}
                                                    Call Row.AddRange((From n In item.Value Select CStr(n)).ToArray)
                                                    Return Row.ToArray
                                                End Function Select Generate()).ToArray)
        Call EstCsv.Save(String.Format("{0}/ChipData_EncodingStandards.csv", ExportDir), False)

        Dim EncodingsCsv = New Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.File From {New String() {"GeneId"}}
        Call EncodingsCsv.First.AddRange((From p In pList Select CStr(p)).ToArray)
        Call EncodingsCsv.AppendRange((From item In EncodingsLQuery.AsParallel
                                       Let Generate = Function() As Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.RowObject
                                                          Dim Row As New List(Of String) From {item.Key}
                                                          Call Row.AddRange((From n In item.Value Select CStr(n)).ToArray)
                                                          Return Row.ToArray
                                                      End Function Select Generate()).ToArray)
        Call EncodingsCsv.Save(String.Format("{0}/ChipData_Encodings.csv", ExportDir), False)
    End Sub

    Public Shared Sub EncodingChipData(ChipData_int As Microsoft.VisualBasic.ComponentModel.Collection.Generic.IKeyValuePairObject(Of String, Integer())(), pList As Double(), ExportDir As String)
        Dim ChipData = (From item In ChipData_int Select New Microsoft.VisualBasic.ComponentModel.Collection.Generic.KeyValuePairObject(Of String, Double()) With {.Key = item.Identifier, .Value = (From n In item.Value Select CType(n, Double)).ToArray}).ToArray
        Call EncodingChipData(ChipData:=ChipData, pList:=pList, ExportDir:=ExportDir)
    End Sub
End Class
