#Region "Microsoft.VisualBasic::c6ae7395b22e627000bb4c9490622e04, analysis\ProteinTools\ProteinMatrix\CreateMatrix.vb"

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

    '   Total Lines: 69
    '    Code Lines: 46 (66.67%)
    ' Comment Lines: 13 (18.84%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (14.49%)
    '     File Size: 2.21 KB


    ' Class CreateMatrix
    ' 
    '     Properties: dimension
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: GetChars, ToMatrix, ToVector
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Polypeptides

''' <summary>
''' Create sgt embedding matrix
''' </summary>
Public Class CreateMatrix

    ReadOnly sgt As SequenceGraphTransform

    ''' <summary>
    ''' get the dimension size of the generated biological sequence 
    ''' matrix via the function <see cref="ToMatrix(FastaSeq)"/>
    ''' </summary>
    ''' <returns>The CNN input size</returns>
    Public ReadOnly Property dimension As Size
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return New Size(sgt.alphabets.Length, sgt.alphabets.Length)
        End Get
    End Property

    Sub New(Optional mol As SeqTypes = SeqTypes.Protein,
            Optional kappa As Double = 1,
            Optional lengthsensitive As Boolean = False)

        Dim allChars As String() = GetChars(mol)

        sgt = New SequenceGraphTransform(kappa:=kappa, lengthsensitive:=lengthsensitive)
        sgt.set_alphabets(allChars)
    End Sub

    Private Shared Function GetChars(mol As SeqTypes) As String()
        Select Case mol
            Case SeqTypes.Protein
                Return AminoAcidObjUtility _
                    .AminoAcidLetters _
                    .JoinIterates("-") _
                    .AsCharacter _
                    .ToArray
            Case Else
                Return mol.GetVector _
                    .JoinIterates({"-"c, "N"c}) _
                    .AsCharacter _
                    .ToArray
        End Select
    End Function

    Public Function ToMatrix(seq As FastaSeq) As Double()()
        Dim v = sgt.fit(seq.SequenceData)
        Dim m = sgt.TranslateMatrix(v)

        Return m
    End Function

    ''' <summary>
    ''' make the given fasta sequence embedding as vector
    ''' </summary>
    ''' <param name="seq"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ToVector(seq As FastaSeq) As Double()
        Return sgt.fitVector(seq.SequenceData)
    End Function

End Class
