#Region "Microsoft.VisualBasic::9bdc0af1a3ca80550c147508081feb61, GCModeller\core\Bio.Assembly\SequenceModel\FASTA\HeaderFormats.vb"

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

    '   Total Lines: 57
    '    Code Lines: 34
    ' Comment Lines: 14
    '   Blank Lines: 9
    '     File Size: 1.84 KB


    '     Module HeaderFormats
    ' 
    '         Function: GetUniProtAccession, HasVersionNumber, TrimAccessionVersion, TryGetUniProtAccession
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Assembly.Uniprot
Imports r = System.Text.RegularExpressions.Regex

Namespace SequenceModel.FASTA

    ''' <summary>
    ''' Fasta序列在不同的数据库之中的标题的格式的帮助函数模块
    ''' </summary>
    Public Module HeaderFormats

        ''' <summary>
        ''' 在这里移除序列编号之中的版本号
        ''' </summary>
        ''' <param name="accession">``XXXXX.1``</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TrimAccessionVersion(accession As String) As String
            Return accession.Split("."c)(Scan0)
        End Function

        Public Function HasVersionNumber(accession As String) As Boolean
            Dim ends As Match = r.Match(accession, "\.\d+$", RegexICMul)

            If ends.Success Then
                Return True
            Else
                Return False
            End If
        End Function

#Region "UniProt"

        ''' <summary>
        ''' 格式参见<see cref="UniprotFasta"/>
        ''' </summary>
        ''' <param name="title"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetUniProtAccession(title As String) As String
            Return title.Split("|"c).ElementAtOrDefault(1)
        End Function

        Public Function TryGetUniProtAccession(title As String, ByRef accession As String) As Boolean
            If Not title.StringEmpty AndAlso title.IndexOf("|"c) > -1 Then
                accession = title.Split("|"c)(1)
                Return True
            Else
                Return False
            End If
        End Function
#End Region

    End Module
End Namespace
