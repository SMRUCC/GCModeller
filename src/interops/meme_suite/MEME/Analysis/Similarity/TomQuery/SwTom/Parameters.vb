#Region "Microsoft.VisualBasic::6f28cab2dc8aadb9eb0120ac382bd728, meme_suite\MEME\Analysis\Similarity\TomQuery\SwTom\Parameters.vb"

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

    '     Class Parameters
    ' 
    '         Properties: AllowComplement, BitsLevel, LevCost, Method, MinW
    '                     Parallel, SWOffset, SWThreshold, TOMThreshold
    ' 
    '         Function: SiteScanProfile, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace Analysis.Similarity.TOMQuery

    ''' <summary>
    ''' Motif相似度比较的参数输入的集合
    ''' </summary>
    Public Class Parameters

        ''' <summary>
        ''' m字符的结果的位点的数量或者SW-TOM里面的高分区的最短的片段长度
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property MinW As Integer = 4
        ''' <summary>
        ''' 筛选高分区的时候使用
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Column("SW.Threshold")>
        <XmlAttribute> Public Property SWThreshold As Double = 0.75
        <XmlAttribute> Public Property Method As String = "pcc"
        <Column("TOM.Threshold")>
        <XmlAttribute> Public Property TOMThreshold As Double = 0.75
        <Column("Bits.Level")>
        <XmlAttribute> Public Property BitsLevel As Double = 1.5
        <XmlAttribute> Public Property Parallel As Boolean
        ''' <summary>
        ''' 创建Smith-waterman高分区的时候，计算出相似度之后向得分转换过程之中的偏移值
        ''' </summary>
        ''' <returns></returns>
        <Column("SW.Offsets")>
        <XmlAttribute> Public Property SWOffset As Double = 0.6
        <XmlAttribute> <Column("Complement.Allowed?")> Public Property AllowComplement As Boolean = False
        <XmlAttribute> <Column("Lev.Cost")> Public Property LevCost As Double = 0.7

        Public Overrides Function ToString() As String
            Return Me.GetXml
        End Function

        ''' <summary>
        ''' 由于序列的残疾出现频率是1，所以阈值过高会很容易筛掉可能的序列位点
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function SiteScanProfile() As Parameters
            Return New Parameters With {
                .SWThreshold = 0.3,
                .TOMThreshold = 0.3,
                .BitsLevel = 3,
                .Parallel = True,
                .AllowComplement = False
            }
        End Function
    End Class
End Namespace
