#Region "Microsoft.VisualBasic::659b68c6cd5f7018a373a37c2510bd1d, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\Pfam.hmm\HMMStruct.vb"

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

    ' Class HMMStruct
    ' 
    '     Properties: Alphabet, AlphaLength, BeginX, DeleteX, FlankingInsertX
    '                 InsertEmission, InsertX, LoopX, MatchEmission, MatchX
    '                 ModelDescription, ModelLength, Name, NullEmission, NullX
    '                 PfamAccessionNumber
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

''' <summary>
''' Structure containing information for an HMM profile retrieved from the PFAM database.
''' </summary>
''' <remarks>
''' http://cn.mathworks.com/help/bioinfo/ref/gethmmprof.html
''' </remarks>
Public Class HMMStruct

    ''' <summary>
    ''' The protein family name (unique identifier) Of the HMM profile record In the PFAM database.
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
    ''' <summary>
    ''' The protein family accession number Of the HMM profile record In the PFAM database.
    ''' </summary>
    ''' <returns></returns>
    Public Property PfamAccessionNumber As String
    ''' <summary>
    ''' Description Of the HMM profile.
    ''' </summary>
    ''' <returns></returns>
    Public Property ModelDescription As String
    ''' <summary>
    ''' The length Of the profile (number Of MATCH states).
    ''' </summary>
    ''' <returns></returns>
    Public Property ModelLength As Integer
    ''' <summary>
    ''' The alphabet used In the model, 'AA' or 'NT'. Note:  AlphaLength Is 20 for 'AA' and 4 for 'NT'.
    ''' </summary>
    ''' <returns></returns>
    Public Property Alphabet As String

    Public ReadOnly Property AlphaLength As Integer
        Get
            If Alphabet = "AA" Then
                Return 20
            Else
                Return 4
            End If
        End Get
    End Property

    ''' <summary>
    ''' Symbol emission probabilities In the MATCH states.
    ''' The format Is a matrix Of size <see cref="ModelLength"/>-by-<see cref="AlphaLength"/>, where Each row corresponds To the emission distribution For a specific MATCH state.
    ''' </summary>
    ''' <returns></returns>
    Public Property MatchEmission As Double()()
    ''' <summary>
    ''' Symbol emission probabilities In the INSERT state.
    ''' The format Is a matrix Of size ModelLength-by-AlphaLength, where Each row corresponds To the emission distribution For a specific INSERT state.
    ''' </summary>
    ''' <returns></returns>
    Public Property InsertEmission As Double()()
    ''' <summary>
    ''' Symbol emission probabilities In the MATCH And INSERT states For the NULL model.
    ''' The format Is a 1-by-AlphaLength row vector.
    ''' Note: NULL probabilities are also known As the background probabilities.
    ''' </summary>
    ''' <returns></returns>
    Public Property NullEmission As Double()
    ''' <summary>
    ''' BEGIN state transition probabilities.
    ''' Format Is a 1-by-(ModelLength + 1) row vector
    ''' [B->D1 B->M1 B->M2 B->M3 .... B->Mend]
    ''' </summary>
    ''' <returns></returns>
    Public Property BeginX As String
    ''' <summary>
    ''' MATCH state transition probabilities.
    ''' Format Is a 4-by-(ModelLength - 1) matrix
    ''' [ M1->M2 M2->M3 ... M[end-1]->Mend;
    '''   M1->I1 M2->I2 ... M[end-1]->I[end-1];
    '''   M1->D2 M2->D3 ... M[end-1]->Dend;
    '''   M1->E  M2->E  ... M[end-1]->E  ]
    ''' </summary>
    ''' <returns></returns>
    Public Property MatchX As String
    ''' <summary>
    ''' INSERT state transition probabilities.
    ''' Format Is a 2-by-(ModelLength - 1) matrix
    ''' [ I1->M2 I2->M3 ... I[end-1]->Mend;
    '''   I1->I1 I2->I2 ... I[end-1]->I[end-1] ]
    ''' </summary>
    ''' <returns></returns>
    Public Property InsertX As String
    ''' <summary>
    ''' DELETE state transition probabilities.
    ''' Format Is a 2-by-(ModelLength - 1) matrix
    ''' [ D1->M2 D2->M3 ... D[end-1]->Mend ;
    '''   D1->D2 D2->D3 ... D[end-1]->Dend ]
    ''' </summary>
    ''' <returns></returns>
    Public Property DeleteX As Double()()

#Region "No idea"

    ''' <summary>
    ''' Flanking insert states (N And C) used For LOCAL profile alignment.
    ''' Format Is a 2-by-2 matrix
    ''' [N->B  C->T ;
    '''  N->N  C->C]
    ''' </summary>
    ''' <returns></returns>
    Public Property FlankingInsertX As String
    ''' <summary>
    ''' Loop states transition probabilities used for multiple hits alignment.
    ''' Format Is a 2-by-2 matrix
    ''' [E->C  J->B ;
    '''  E->J  J->J]
    ''' </summary>
    ''' <returns></returns>
    Public Property LoopX As String
    ''' <summary>
    ''' Null transition probabilities used To provide scores With log-odds values also For state transitions.
    ''' Format Is a 2-by-1 column vector
    ''' [G->F ; G->G]
    ''' </summary>
    ''' <returns></returns>
    Public Property NullX As String
#End Region

    Sub New(data As HMMParser)
        Me.Alphabet = If(data.ALPH = "amino", "AA", "NT")
        Me.ModelDescription = data.DESC
        Me.ModelLength = data.LENG
        Me.Name = data.NAME
        Me.PfamAccessionNumber = data.ACC
        Me.MatchEmission = data.HMM.nodes.Select(Function(x) x.Match).ToArray
        Me.InsertEmission = data.HMM.nodes.Select(Function(x) x.Insert).ToArray
        Me.NullEmission = data.HMM.COMPO.Match

        ' 最后一行数据之中：
        ' These seven numbers are:
        ' B->M1, B->I0, B->D1; I0->M1, I0->I0;

        ' The seven fields on this line are the transitions for node k, 
        ' in the order shown by the transition header line: 
        ' Mk->Mk+1; Ik; Dk+1; Ik->Mk+1; Ik; Dk->Mk+1; Dk+1.
        ' 0         1   2     3         4   5         6

        Me.DeleteX = {
            data.HMM _
            .nodes _
            .Select(Function(x) x.StateTransitions(5)) _
            .ToArray
        }
    End Sub

    Sub New()
    End Sub
End Class
