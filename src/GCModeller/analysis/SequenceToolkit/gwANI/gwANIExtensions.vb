#Region "Microsoft.VisualBasic::9c1e3a5c7e9b1d3f5a7c9e1b3d5f7a9c, analysis\SequenceToolkit\gwANI\GwaniExtensions.vb"

' Author:
'
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
'
' Copyright (c) 2018 GPL3 Licensed
'
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

Option Strict On

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports SMRUCC.genomics.SequenceModel.FASTA


    ''' <summary>
    ''' The gwANI facade functions.
    ''' </summary>
    <HideModuleName>
    Public Module GwaniExtensions

        ''' <summary>
        ''' Calculates the genome wide average nucleotide identity of every sample
        ''' against all of the other samples.
        ''' </summary>
        ''' <param name="msa">
        ''' The input must be a multi-FASTA alignment, i.e. all of the sequences share
        ''' the same length.
        ''' </param>
        ''' <param name="options"></param>
        ''' <returns>
        ''' One <see cref="DataSet"/> per sample, in the input order. The cell below the
        ''' diagonal is -1 and the diagonal is 100, exactly like the output of the
        ''' original panito tool.
        ''' </returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CalculateGwani(msa As FastaFile, Optional options As GwaniOptions = Nothing) As DataSet()
            Return GwaniCalculator.Evaluate(msa, options)
        End Function

    End Module


#End Region
