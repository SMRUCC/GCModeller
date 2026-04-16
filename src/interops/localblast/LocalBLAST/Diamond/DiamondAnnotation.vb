#Region "Microsoft.VisualBasic::01ef3cd9c115326c367d0bbd7057b1a2, localblast\LocalBLAST\Diamond\DiamondAnnotation.vb"

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


    ' Code Statistics:

    '   Total Lines: 103
    '    Code Lines: 37 (35.92%)
    ' Comment Lines: 51 (49.51%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 15 (14.56%)
    '     File Size: 2.98 KB


    ' Class DiamondAnnotation
    ' 
    '     Properties: BitScore, EValue, GapOpen, Length, Mismatch
    '                 Pident, QEnd, QseqId, QStart, SEnd
    '                 SseqId, SStart
    ' 
    '     Function: GetSingleHit, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Map(Of String, String)
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

''' <summary>
''' 代表 DIAMOND BLASTP 结果文件 (.m8) 中的一行记录
''' </summary>
Public Class DiamondAnnotation : Implements IBlastHit, IMap, IQueryHits

    ''' <summary>
    ''' 1. 查询序列ID
    ''' </summary>
    ''' <returns></returns>
    Public Property QseqId As String Implements IBlastHit.queryName, IMap.Key

    ''' <summary>
    ''' 2. 目标序列ID
    ''' </summary>
    ''' <returns></returns>
    Public Property SseqId As String Implements IBlastHit.hitName, IBlastHit.description, IMap.Maps

    ''' <summary>
    ''' 3. 比对一致性百分比
    ''' </summary>
    ''' <returns></returns>
    Public Property Pident As Double Implements IQueryHits.identities

    ''' <summary>
    ''' 4. 比对长度
    ''' </summary>
    ''' <returns></returns>
    Public Property Length As Integer

    ''' <summary>
    ''' 5. 错配数
    ''' </summary>
    ''' <returns></returns>
    Public Property Mismatch As Integer

    ''' <summary>
    ''' 6. Gap 打开次数
    ''' </summary>
    ''' <returns></returns>
    Public Property GapOpen As Integer

    ''' <summary>
    ''' 7. 查询序列起始位置
    ''' </summary>
    ''' <returns></returns>
    Public Property QStart As Integer

    ''' <summary>
    ''' 8. 查询序列结束位置
    ''' </summary>
    ''' <returns></returns>
    Public Property QEnd As Integer

    ''' <summary>
    ''' 9. 目标序列起始位置
    ''' </summary>
    ''' <returns></returns>
    Public Property SStart As Integer

    ''' <summary>
    ''' 10. 目标序列结束位置
    ''' </summary>
    ''' <returns></returns>
    Public Property SEnd As Integer

    ''' <summary>
    ''' 11. E-value (期望值)
    ''' </summary>
    ''' <returns></returns>
    Public Property EValue As Double

    ''' <summary>
    ''' 12. Bit Score (比特得分)
    ''' </summary>
    ''' <returns></returns>
    Public Property BitScore As Double

    Public Overrides Function ToString() As String
        Return $"{QseqId} vs {SseqId} | Identity: {Pident}% | E-value: {EValue}"
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetSingleHit() As BestHit
        Return New BestHit With {
            .QueryName = QseqId,
            .HitName = SseqId,
            .identities = Pident,
            .length_hsp = Length,
            .evalue = EValue,
            .score = BitScore,
            .length_query = QEnd - QStart,
            .length_hit = SEnd - SStart,
            .hit_length = .length_hit,
            .query_length = .length_query,
            .positive = .identities
        }
    End Function
End Class

