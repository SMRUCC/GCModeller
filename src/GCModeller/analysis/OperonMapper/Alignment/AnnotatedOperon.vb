#Region "Microsoft.VisualBasic::482d094138b3a5aaee0a2b746665ab76, analysis\OperonMapper\Alignment\AnnotatedOperon.vb"

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

    '   Total Lines: 51
    '    Code Lines: 22 (43.14%)
    ' Comment Lines: 24 (47.06%)
    '    - Xml Docs: 95.83%
    ' 
    '   Blank Lines: 5 (9.80%)
    '     File Size: 1.53 KB


    ' Enum OperonType
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' Class AnnotatedOperon
    ' 
    '     Properties: Genes, InsertedGeneIds, KnownGeneIds, left, MissingGeneIds
    '                 name, OperonID, right, Scores, strand
    '                 Type
    ' 
    '     Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' Operon的类型枚举
''' </summary>
Public Enum OperonType
    Conserved  ' 保守的
    Insertion  ' 插入突变
    Deletion   ' 缺失突变
End Enum

' 用于表示最终注释结果的Operon结构
Public Class AnnotatedOperon
    Public Property OperonID As String
    Public Property name As String
    Public Property Type As OperonType
    ''' <summary>
    ''' 组成此Operon的基因组上的基因
    ''' </summary>
    ''' <returns></returns>
    Public Property Genes As String()
    ''' <summary>
    ''' 每个基因对当前OperonID的投票总得分
    ''' </summary>
    ''' <returns></returns>
    Public Property Scores As Double()
    ''' <summary>
    ''' 参考Operon中应有的基因ID
    ''' </summary>
    ''' <returns></returns>
    Public Property KnownGeneIds As String()
    ''' <summary>
    ''' 插入的新基因ID
    ''' </summary>
    ''' <returns></returns>
    Public Property InsertedGeneIds As String()
    ''' <summary>
    ''' 缺失的基因ID
    ''' </summary>
    ''' <returns></returns>
    Public Property MissingGeneIds As String()

    Public Property strand As String
    Public Property left As Integer
    Public Property right As Integer

    Public Overrides Function ToString() As String
        Return $"{Type.Description} #{OperonID} at {strand}:{left}-{right} with {Genes.TryCount} gene members {Genes.GetJson}"
    End Function
End Class


