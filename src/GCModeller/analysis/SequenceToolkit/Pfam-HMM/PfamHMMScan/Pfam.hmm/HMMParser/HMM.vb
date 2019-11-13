#Region "Microsoft.VisualBasic::3e62cbd077c62253ec352de6a30e6357, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\Pfam.hmm\HMMParser\HMM.vb"

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

    ' Class HMM
    ' 
    '     Properties: COMPO, nodes, Residues
    ' 
    '     Function: ToString
    ' 
    ' Structure Node
    ' 
    '     Properties: Address, Insert, Match, StateTransitions
    ' 
    '     Function: ToString
    ' 
    '     Sub: Assign
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' 
''' </summary>
Public Class HMM

    Public Property COMPO As Node
    Public Property nodes As Node()

    Public Shared ReadOnly Property Residues As IReadOnlyCollection(Of String) =
        New String() {"A", "C", "D", "E", "F", "G", "H", "I", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "V", "W", "Y"}

    Public Overrides Function ToString() As String
        Return nodes.GetJson
    End Function
End Class

''' <summary>
''' The remainder of the model has three lines per node, for M nodes (where M is the number of match
''' states, As given by the LENG line). These three lines are (K Is the alphabet size In residues)
''' </summary>
Public Structure Node : Implements IAddressOf

    ''' <summary>
    ''' [Match emission line] 
    ''' The first field is the node number (1 : : :M). The parser verifies this number as a
    ''' consistency check(it expects the nodes to come in order). The next K numbers for
    ''' match emissions, one per symbol, In alphabetic order.
    ''' The next field Is the MAP annotation for this node. If MAP was yes in the header,
    ''' then this Is an integer, representing the alignment column index for this match state
    ''' (1..alen); otherwise, this field Is '-’.
    ''' The next field Is the CONS consensus residue for this node. If CONS was yes in the
    ''' header, then this Is a single character, representing the consensus residue annotation
    ''' For this match state; otherwise, this field Is '-’.
    ''' The next field Is the RF annotation for this node. If RF was yes in the header, then
    ''' this Is a single character, representing the reference annotation for this match state;
    ''' otherwise, this field Is '-’.
    ''' The next field Is the MM mask value for this node. If MM was yes in the header, then
    ''' this Is a single 'm’ character, indicating that the position was identified as a masked
    ''' position during model construction; otherwise, this field Is '-’.
    ''' The next field Is the CS annotation for this node. If CS was yes, then this Is a single
    ''' character, representing the consensus structure at this match state; otherwise this
    ''' field Is '-’.
    ''' </summary>
    ''' <returns></returns>
    Public Property Match As Double()
    ''' <summary>
    ''' [Insert emission line] 
    ''' The K fields on this line are the insert emission scores, one per symbol, in alphabetic
    ''' order.
    ''' </summary>
    ''' <returns></returns>
    Public Property Insert As Double()
    ''' <summary>
    ''' [State transition line]
    ''' The seven fields on this line are the transitions for node k, in the order shown by the
    ''' transition header line: Mk ! Mk+1; Ik;Dk+1; Ik ! Mk+1; Ik; Dk ! Mk+1;Dk+1.
    ''' For transitions from the final node M, match state M + 1 Is interpreted as the END
    ''' state E, and there Is no delete state M + 1; therefore the final Mk ! Dk+1 And
    ''' Dk ! Dk+1 transitions are always * (zero probability), And the final Dk ! Mk+1
    ''' transition Is always 0.0 (probability 1.0).
    ''' </summary>
    ''' <returns></returns>
    Public Property StateTransitions As Double()
    ''' <summary>
    ''' 残基编号
    ''' </summary>
    ''' <returns></returns>
    Public Property Address As Integer Implements IAddressOf.Address

    Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
        Me.Address = address
    End Sub

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
   
End Structure
