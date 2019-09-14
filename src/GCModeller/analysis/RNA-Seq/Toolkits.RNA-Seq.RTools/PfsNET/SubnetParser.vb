#Region "Microsoft.VisualBasic::a7e0410a2832fbb49b7fc76be73cb242, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\PfsNET\SubnetParser.vb"

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

    '     Module SubnetParser
    ' 
    '         Function: __netParser, GenerateDefaultStruct, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace PfsNET

    Public Module SubnetParser

        ''' <summary>
        ''' 这个函数仅适用于一次R计算结果输出的结果文本的解析操作
        ''' </summary>
        ''' <param name="TextLines">对于一次PFSNet的计算结果输出而言，其仅有两个元素，分别表示两种表型的数据</param>
        ''' <returns>分别返回表型1和表型2的计算结果数据</returns>
        ''' <remarks></remarks>
        Public Function TryParse(TextLines As String()) As KeyValuePair(Of PfsNET(), PfsNET())
            Dim p1 As String = TextLines(0)
            Dim p2 As String = TextLines(1)
            Dim value = New KeyValuePair(Of PfsNET(), PfsNET())(__netParser(p1, "1"), __netParser(p2, "2"))
            Return value
        End Function

        Private Function __netParser(strLine As String, ClassId As Integer) As PfsNET()
            Dim setValue = New SetValue(Of PfsNET)().GetSet(NameOf(PfsNET.Class))
            Dim LQuery As PfsNET() =
                LinqAPI.Exec(Of PfsNET) <= From m As Match
                                           In Regex.Matches(strLine.Replace(" ", ""), PfsNET.LIST_ITEM, RegexOptions.IgnoreCase).AsParallel
                                           Let m_value As String = m.Value
                                           Where Not String.IsNullOrEmpty(m_value)
                                           Let result = PfsNET.TryParse(m_value)
                                           Select setValue(result, ClassId)
            Return LQuery
        End Function

        Public Function GenerateDefaultStruct(data As KeyValuePair(Of PfsNET(), PfsNET()), dataTag As String) As PFSNetResultOut
            Dim Result As New PFSNetResultOut With {
                .tag = dataTag
            }
            Result.Phenotype1 = (From x As PfsNET In data.Key Select x.ToPFSNet).ToArray
            Result.Phenotype2 = (From x As PfsNET In data.Value Select x.ToPFSNet).ToArray
            Return Result
        End Function
    End Module
End Namespace
