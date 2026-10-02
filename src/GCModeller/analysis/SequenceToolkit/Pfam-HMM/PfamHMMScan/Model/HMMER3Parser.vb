#Region "Microsoft.VisualBasic::af780beab6289e3706739e49387082fc, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\HMMER3Parser.vb"

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

    '   Total Lines: 248
    '    Code Lines: 153 (61.69%)
    ' Comment Lines: 71 (28.63%)
    '    - Xml Docs: 67.61%
    ' 
    '   Blank Lines: 24 (9.68%)
    '     File Size: 10.30 KB


    ' Class HMMER3Parser
    ' 
    '     Function: Parse, ParseContent, ParseLines, ParseStatsLine, ParseValue
    ' 
    '     Sub: ParseCompoInsertLine, ParseCompoLine, ParseCompoTransLine, ParseStateBlock
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' HMMER3模型解析器与蛋白质序列分类注释模块
' 
' 基于现有HMM算法框架，实现HMMER3格式模型文件的读取和蛋白质序列分类注释功能
' 
' Author: 基于用户现有HMM代码框架扩展
' Copyright (c) 2024 GPL3 Licensed
' ============================================================================

Imports System.IO
Imports System.Runtime.CompilerServices

''' <summary>
''' HMMER3模型文件解析器
''' 用于读取.hmm格式的HMMER3模型文件
''' </summary>
''' <remarks>
''' HMMER3文件格式说明：
''' - HMMER3/f: 文件格式标识
''' - NAME: 模型名称
''' - LENG: 模型长度（匹配状态数）
''' - ALPH: 字母表类型（amino表示蛋白质）
''' - HMM: 概率矩阵部分
'''   - COMPO行: 背景分布
'''   - 每个状态包含:
'''     - 匹配发射概率（20个氨基酸）
'''     - 插入发射概率（20个氨基酸）
'''     - 转移概率（7个：m->m, m->i, m->d, i->m, i->i, d->m, d->d）
''' </remarks>
Public Module HMMER3Parser

    ' 氨基酸字母表顺序（HMMER3标准顺序）
    Public ReadOnly AA_ALPHABET As String() = {
        "A", "C", "D", "E", "F", "G", "H", "I", "K", "L",
        "M", "N", "P", "Q", "R", "S", "T", "V", "W", "Y"
    }

    ''' <summary>
    ''' 解析HMMER3模型文件
    ''' </summary>
    ''' <param name="filePath">HMMER3模型文件路径</param>
    ''' <returns>解析后的ProfileHMM对象</returns>
    Public Function Parse(filePath As String) As ProfileHMM
        If Not File.Exists(filePath) Then
            Throw New FileNotFoundException($"HMMER3 model file not found: {filePath}")
        Else
            Return filePath.ReadAllLines.ParseLines
        End If
    End Function

    <Extension>
    Public Iterator Function LoadDatabase(s As Stream) As IEnumerable(Of ProfileHMM)
        For Each block As String() In s.IterateAllLines(tqdm_wrap:=True).Split("//")
            Yield block.ParseLines
        Next
    End Function

    ''' <summary>
    ''' 解析HMMER3模型文本内容
    ''' </summary>
    ''' <param name="content">HMMER3模型文本内容</param>
    ''' <returns>解析后的ProfileHMM对象</returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ParseContent(content As String) As ProfileHMM
        Return content.LineTokens.ParseLines()
    End Function

    ''' <summary>
    ''' 解析HMMER3模型行数据
    ''' </summary>
    ''' 
    <Extension>
    Private Function ParseLines(lines As String()) As ProfileHMM
        Return New MatrixReader().ParseModel(lines)
    End Function
End Module
