﻿#Region "Microsoft.VisualBasic::b8dfa51a477b579198436d1e01d37511, analysis\SequenceToolkit\SmithWaterman\SmithWaterman.vb"

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

    ' Class SmithWaterman
    ' 
    '     Constructor: (+2 Overloads) Sub New
    '     Function: Align, GetOutput
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Linq
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Language.Default
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' Smith-Waterman local alignment algorithm.
'''
''' Design Note: this class implements AminoAcids interface: a simple fix customized to amino acids, since that is all we deal with in this class
''' Supporting both DNA and Aminoacids, will require a more general design.
''' </summary>
Public Class SmithWaterman : Inherits GSW(Of Char)

    ''' <summary>
    ''' 蛋白比对的矩阵
    ''' </summary>
    Shared ReadOnly blosum62 As DefaultValue(Of Blosum) = Blosum.FromInnerBlosum62

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="subject"></param>
    ''' <param name="blosum">
    ''' If the matrix parameter is null, then the default build in blosum62 matrix will be used.
    ''' </param>
    Sub New(query$, subject$, Optional blosum As Blosum = Nothing)
        Call MyBase.New(query.ToArray, subject.ToArray, AddressOf (blosum Or blosum62).GetDistance, Function(x) x)
    End Sub

    Sub New(query As ISequenceModel, subject As ISequenceModel, Optional blosum As Blosum = Nothing)
        Call MyBase.New(query.SequenceData.ToArray, subject.SequenceData.ToArray, AddressOf (blosum Or blosum62).GetDistance, Function(x) x)
    End Sub

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="cutoff">0%-100%</param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetOutput(cutoff As Double, minW As Integer) As Output
        Return Output.CreateObject(Me, Function(x) x, cutoff, minW)
    End Function

    ''' <summary>
    ''' Default using ``Blosum62`` matrix.
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="subject"></param>
    ''' <param name="blosum"></param>
    ''' <returns></returns>
    Public Shared Function Align(query As FastaSeq, subject As FastaSeq, Optional blosum As Blosum = Nothing) As SmithWaterman
        Dim sw As New SmithWaterman(query.SequenceData, subject.SequenceData, blosum)
        Return sw
    End Function
End Class
