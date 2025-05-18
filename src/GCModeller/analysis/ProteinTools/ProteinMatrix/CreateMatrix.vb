#Region "Microsoft.VisualBasic::152ebaab3670da526bb7ac818ac7125e, analysis\ProteinTools\ProteinMatrix\CreateMatrix.vb"

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

'   Total Lines: 49
'    Code Lines: 33 (67.35%)
' Comment Lines: 7 (14.29%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 9 (18.37%)
'     File Size: 1.50 KB


' Class CreateMatrix
' 
'     Properties: dimension
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: ToMatrix, ToVector
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
    ''' get the dimension size of the generated biological sequence matrix via the function <see cref="ToMatrix(FastaSeq)"/>
    ''' </summary>
    ''' <returns>The CNN input size</returns>
    Public ReadOnly Property dimension As Size
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return New Size(sgt.alphabets.Length, sgt.alphabets.Length)
        End Get
    End Property

    Sub New(Optional mol As SeqTypes = SeqTypes.Protein)
        Dim allChars As String() = GetChars(mol)

        sgt = New SequenceGraphTransform
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

    Public Function ToMatrix(prot As FastaSeq) As Double()()
        Dim v = sgt.fit(prot.SequenceData)
        Dim m = sgt.TranslateMatrix(v)

        Return m
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ToVector(prot As FastaSeq) As Double()
        Return sgt.fitVector(prot.SequenceData)
    End Function

End Class
