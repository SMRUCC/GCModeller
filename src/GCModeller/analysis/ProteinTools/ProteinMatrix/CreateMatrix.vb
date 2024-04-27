#Region "Microsoft.VisualBasic::3f8310184b75ea47f918daa75e0d067e, G:/GCModeller/src/GCModeller/analysis/ProteinTools/ProteinMatrix//CreateMatrix.vb"

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

    '   Total Lines: 46
    '    Code Lines: 33
    ' Comment Lines: 4
    '   Blank Lines: 9
    '     File Size: 1.43 KB


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
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.Polypeptides

Public Class CreateMatrix

    Dim sgt As SequenceGraphTransform

    ''' <summary>
    ''' get the dimension size of the generated protein matrix via the function <see cref="ToMatrix(FastaSeq)"/>
    ''' </summary>
    ''' <returns>The CNN input size</returns>
    Public ReadOnly Property dimension As Size
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return New Size(sgt.alphabets.Length, sgt.alphabets.Length)
        End Get
    End Property

    Sub New()
        Dim allChars As String() = AminoAcidObjUtility _
            .AminoAcidLetters _
            .JoinIterates("-") _
            .Select(Function(c) c.ToString) _
            .ToArray

        sgt = New SequenceGraphTransform
        sgt.set_alphabets(allChars)
    End Sub

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
