#Region "Microsoft.VisualBasic::5fb90232cf3898a3d1846e5eabbc7d83, GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Translation\NtHelper.vb"

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
    '    Code Lines: 32
    ' Comment Lines: 12
    '   Blank Lines: 13
    '     File Size: 1.87 KB


    '     Module NtHelper
    ' 
    '         Function: DoCheckNtDirection
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace SequenceModel.NucleotideModels.Translation

    Module NtHelper

        ''' <summary>
        ''' Check nt sequence direction by start and stop codon
        ''' </summary>
        ''' <param name="sequence"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function DoCheckNtDirection(translTable As TranslTable, sequence As String, ByRef operations As String()) As String
            Dim first As String = Mid(sequence, 1, 3)
            Dim last As String = Mid(sequence, Len(sequence) - 3)

            If sequence.Length Mod 3 <> 0 Then
                ' is not an ORF sequence?

            End If

            If translTable.IsInitCoden(first) Then
                ' 正常的序列
                Return sequence
            End If

            Dim lastAsInit As String = last.Reverse.CharString

            If translTable.IsInitCoden(lastAsInit) Then
                ' 方向可能颠倒了
                operations = {"reverse"}
                Return New String(sequence.Reverse.ToArray)
            End If

            first = NucleicAcid.Complement(first)

            If translTable.IsInitCoden(first) Then
                ' 互补的序列
                operations = {"complement"}
                Return NucleicAcid.Complement(sequence)
            End If

            lastAsInit = NucleicAcid.Complement(lastAsInit)

            If translTable.IsInitCoden(lastAsInit) Then
                ' 方向可能颠倒了
                operations = {"reverse", "complement"}
                Return NucleicAcid.Complement(sequence).Reverse.CharString
            End If

            ' 实在判断不出来了，只能够硬着头皮翻译下去了 
            operations = {"invalid"}
            Return sequence
        End Function
    End Module
End Namespace
