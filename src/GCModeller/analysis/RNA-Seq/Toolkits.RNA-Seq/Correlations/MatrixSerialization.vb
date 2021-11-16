#Region "Microsoft.VisualBasic::e7a49f8f2d3c0474a22f26d51bf05611, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\MatrixSerialization.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::ae922db5f35fb8c1434df78c40756b59, analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\MatrixSerialization.vb"

'' Author:
'' 
''       asuka (amethyst.asuka@gcmodeller.org)
''       xie (genetics@smrucc.org)
''       xieguigang (xie.guigang@live.com)
'' 
'' Copyright (c) 2018 GPL3 Licensed
'' 
'' 
'' GNU GENERAL PUBLIC LICENSE (GPL3)
'' 
'' 
'' This program is free software: you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation, either version 3 of the License, or
'' (at your option) any later version.
'' 
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
'' 
'' You should have received a copy of the GNU General Public License
'' along with this program. If not, see <http://www.gnu.org/licenses/>.



'' /********************************************************************************/

'' Summaries:

'' Module MatrixSerialization
'' 
''     Function: CreateObject, Load, SaveBin, Serialize
'' 
'' /********************************************************************************/

'#End Region

'Imports System.Text
'Imports Microsoft.VisualBasic
'Imports Microsoft.VisualBasic.CommandLine.Reflection
'Imports Microsoft.VisualBasic.Language
'Imports Microsoft.VisualBasic.Linq.Extensions
'Imports Microsoft.VisualBasic.Net.Protocols
'Imports Microsoft.VisualBasic.Serialization.RawStream
'Imports Microsoft.VisualBasic.Scripting.MetaData
'Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT
'Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein

'''' <summary>
'''' 对PCC矩阵进行快速的二进制序列化
'''' </summary>
'''' <remarks>由于是一个二维的矩阵，坐标之间有着一一对应的顺序关系，所以这里不可以使用并行化拓展</remarks>
'<Package("PCC.Matrix.Serialization",
'                  Description:="Tools for fast IO of the gene expression pcc matrix.",
'                  Category:=APICategories.UtilityTools,
'                  Publisher:="xie.guigang@gcmodeller.org")>
'Public Module MatrixSerialization

'    ''' <summary>
'    ''' &lt;TOTAL_BYTES> + &lt;STRING_LENGTH>
'    ''' </summary>
'    ''' <param name="sample"></param>
'    ''' <returns></returns>
'    ''' 
'    <ExportAPI("Sample.Serialize")>
'    Public Function Serialize(sample As ExprSamples) As Byte()
'        Dim locusId As Byte() = Encoding.ASCII.GetBytes(sample.locusId)
'        Dim array As New ArrayRow With {
'            .data = sample.data
'        }
'        Dim arrayBuffer As Byte() = array.Serialize

'        ' totalSize + tag_length + tag_string + array
'        Dim chunkBuffer As Byte() = New Byte(INT64 + INT32 + locusId.Length + arrayBuffer.Length) {}
'        Dim TotalSize As Long = locusId.Length + arrayBuffer.Length
'        Dim header As Byte() = BitConverter.GetBytes(TotalSize)
'        Dim idLength As Byte() = BitConverter.GetBytes(locusId.Length)
'        Dim p As i32 = Scan0

'        Call System.Array.ConstrainedCopy(header, Scan0, chunkBuffer, p + INT64, INT64)
'        Call System.Array.ConstrainedCopy(idLength, Scan0, chunkBuffer, p + INT32, INT32)
'        Call System.Array.ConstrainedCopy(locusId, Scan0, chunkBuffer, p + locusId.Length, locusId.Length)
'        Call System.Array.ConstrainedCopy(arrayBuffer, Scan0, chunkBuffer, p, arrayBuffer.Length)

'        Return chunkBuffer
'    End Function

'    <ExportAPI("Save.bin")>
'    Public Function SaveBin(MAT As PccMatrix, SaveTo As String) As Boolean
'        Dim LQuery As Byte() = MAT.Select(Function(sample) MatrixSerialization.Serialize(sample)).ToVector
'        Return LQuery.FlushStream(SaveTo)
'    End Function

'    ''' <summary>
'    ''' 加载二进制数据库
'    ''' </summary>
'    ''' <param name="from"></param>
'    ''' <returns></returns>
'    <ExportAPI("Matrix.Load.bin")>
'    Public Function Load(from As String) As PccMatrix
'        Dim byts As Byte() = IO.File.ReadAllBytes(from)
'        Dim samples As New List(Of ExprSamples)
'        Dim p As Long = Scan0
'        Dim header As Byte() = New Byte(INT64 - 1) {}
'        Dim headOffset As Integer = INT64 + INT32

'        ' 偏移量是数据块的总长度+INT64的长度
'        Do While p < byts.Length - 1
'            Call Array.ConstrainedCopy(byts, p, header, Scan0, INT64)

'            Dim totalSize As Long = BitConverter.ToInt64(header, Scan0) + headOffset
'            Dim chunkBuffer As Byte() = New Byte(totalSize - 1) {}

'            Call Array.ConstrainedCopy(byts, p, chunkBuffer, Scan0, chunkBuffer.Length)
'            Call samples.Add(MatrixSerialization.CreateObject(chunkBuffer))

'            p += (totalSize + 1)
'        Loop

'        Return New PccMatrix(samples)
'    End Function

'    Private Function CreateObject(byts As Byte()) As ExprSamples
'        Dim locusLen As Byte() = New Byte(INT32 - 1) {}
'        Call Array.ConstrainedCopy(byts, INT64, locusLen, Scan0, INT32)
'        Dim len As Integer = BitConverter.ToInt32(locusLen, Scan0)
'        Dim chunkBuffer As Byte() = New Byte(len - 1) {}
'        Call Array.ConstrainedCopy(byts, INT64 + INT32, chunkBuffer, Scan0, len)
'        Dim ID As String = System.Text.Encoding.ASCII.GetString(chunkBuffer)
'        locusLen = New Byte(INT64 - 1) {}
'        Call Array.ConstrainedCopy(byts, Scan0, locusLen, Scan0, INT64)
'        Dim arrayLength As Integer = BitConverter.ToInt64(locusLen, Scan0) - len
'        chunkBuffer = New Byte(arrayLength - 1) {}
'        Call Array.ConstrainedCopy(byts, INT64 + INT32 + len, chunkBuffer, Scan0, arrayLength)
'        Dim samples As New Streams.Array.Double(chunkBuffer)
'        Dim sampleValue As New ExprSamples With {
'            .locusId = ID,
'            .Values = samples.Values
'        }
'        Return sampleValue
'    End Function
'End Module
