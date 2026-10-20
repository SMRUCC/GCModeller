#Region "Microsoft.VisualBasic::4004e3a471c16fe64104c522732b6b15, analysis\SequenceToolkit\gwANI\gwANIExtensions.vb"

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

'   Total Lines: 45
'    Code Lines: 26 (57.78%)
' Comment Lines: 14 (31.11%)
'    - Xml Docs: 92.86%
' 
'   Blank Lines: 5 (11.11%)
'     File Size: 1.66 KB


' Module gwANIExtensions
' 
'     Function: calculate_and_output_gwani, fast_calculate_gwani
' 
'     Sub: Evaluate
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports SMRUCC.genomics.SequenceModel.FASTA

<HideModuleName>
Public Module gwANIExtensions

    ''' <summary>
    ''' 执行入口点
    ''' </summary>
    ''' <param name="multipleSeq">
    ''' 必须是经过多序列对齐之后的序列集合
    ''' </param>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function calculate_and_output_gwani(ByRef multipleSeq As FastaFile) As DataSet()
        Return New gwANI().__calculate_and_output_gwani(multipleSeq).ToArray
    End Function

    ''' <summary>
    ''' 执行入口点
    ''' </summary>
    ''' <param name="multipleSeq">
    ''' 必须是经过多序列对齐之后的序列集合
    ''' </param>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function fast_calculate_gwani(ByRef multipleSeq As FastaFile) As DataSet()
        Return New gwANI().__fast_calculate_gwani(multipleSeq).ToArray
    End Function
End Module
