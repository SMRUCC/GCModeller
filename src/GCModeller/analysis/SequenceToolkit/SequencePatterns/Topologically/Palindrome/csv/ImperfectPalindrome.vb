#Region "Microsoft.VisualBasic::1a48aa76a8617d52ba3e32e74da48dd6, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Palindrome\csv\ImperfectPalindrome.vb"

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

    '   Total Lines: 72
    '    Code Lines: 26
    ' Comment Lines: 39
    '   Blank Lines: 7
    '     File Size: 2.35 KB


    '     Class ImperfectPalindrome
    ' 
    '         Properties: Data, Distance, Evolr, Left, Matches
    '                     MaxMatch, Palindrome, Paloci, Score, SequenceData
    '                     Site
    ' 
    '         Function: __getMappingLoci, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.SequenceModel

Namespace Topologically

    ''' <summary>
    ''' 只有一部分的序列是匹配上的回文序列
    ''' </summary>
    Public Class ImperfectPalindrome : Inherits NucleotideModels.Contig
        Implements ILoci
        Implements IPolymerSequenceModel

        ''' <summary>
        ''' 种子生成的序列
        ''' </summary>
        ''' <returns></returns>
        Public Property Site As String
        ''' <summary>
        ''' 种子序列在基因组上面的位置
        ''' </summary>
        ''' <returns></returns>
        Public Property Left As Integer Implements ILoci.Left
        ''' <summary>
        ''' 回文片段的位点位置
        ''' </summary>
        ''' <returns></returns>
        Public Property Paloci As Integer
        ''' <summary>
        ''' 回文片段的序列
        ''' </summary>
        ''' <returns></returns>
        Public Property Palindrome As String
        ''' <summary>
        ''' 片段相似度距离大小
        ''' </summary>
        ''' <returns></returns>
        Public Property Distance As Double
        ''' <summary>
        ''' 相似度高低
        ''' </summary>
        ''' <returns></returns>
        Public Property Score As Double
        ''' <summary>
        ''' 匹配的碱基
        ''' </summary>
        ''' <returns></returns>
        Public Property Matches As String
        ''' <summary>
        ''' 演化的路径
        ''' </summary>
        ''' <returns></returns>
        Public Property Evolr As String
        Public Property MaxMatch As Integer

        Public Property Data As Dictionary(Of String, String)

        ''' <summary>
        ''' sequence data for loci value <see cref="ImperfectPalindrome.MappingLocation(Boolean)"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData

        Public Overrides Function ToString() As String
            Return $"{Site} <==> {Palindrome}, {Matches}"
        End Function

        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(Left, Paloci)
        End Function
    End Class
End Namespace
