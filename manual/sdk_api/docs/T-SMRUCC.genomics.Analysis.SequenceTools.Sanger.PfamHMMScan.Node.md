---
title: Node
---

# Node
_namespace: [SMRUCC.genomics.Analysis.SequenceTools.Sanger.PfamHMMScan](N-SMRUCC.genomics.Analysis.SequenceTools.Sanger.PfamHMMScan.html)_

The remainder of the model has three lines per node, for M nodes (where M is the number of match
 states, As given by the LENG line). These three lines are (K Is the alphabet size In residues)




### Properties

#### Address
残基编号
#### Insert
[Insert emission line] 
 The K fields on this line are the insert emission scores, one per symbol, in alphabetic
 order.
#### Match
[Match emission line] 
 The first field is the node number (1 : : :M). The parser verifies this number as a
 consistency check(it expects the nodes to come in order). The next K numbers for
 match emissions, one per symbol, In alphabetic order.
 The next field Is the MAP annotation for this node. If MAP was yes in the header,
 then this Is an integer, representing the alignment column index for this match state
 (1..alen); otherwise, this field Is '-’.
 The next field Is the CONS consensus residue for this node. If CONS was yes in the
 header, then this Is a single character, representing the consensus residue annotation
 For this match state; otherwise, this field Is '-’.
 The next field Is the RF annotation for this node. If RF was yes in the header, then
 this Is a single character, representing the reference annotation for this match state;
 otherwise, this field Is '-’.
 The next field Is the MM mask value for this node. If MM was yes in the header, then
 this Is a single 'm’ character, indicating that the position was identified as a masked
 position during model construction; otherwise, this field Is '-’.
 The next field Is the CS annotation for this node. If CS was yes, then this Is a single
 character, representing the consensus structure at this match state; otherwise this
 field Is '-’.
#### StateTransitions
[State transition line]
 The seven fields on this line are the transitions for node k, in the order shown by the
 transition header line: Mk ! Mk+1; Ik;Dk+1; Ik ! Mk+1; Ik; Dk ! Mk+1;Dk+1.
 For transitions from the final node M, match state M + 1 Is interpreted as the END
 state E, and there Is no delete state M + 1; therefore the final Mk ! Dk+1 And
 Dk ! Dk+1 transitions are always * (zero probability), And the final Dk ! Mk+1
 transition Is always 0.0 (probability 1.0).
