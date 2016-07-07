---
title: Enzrxn
---

# Enzrxn
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.html)_






### Properties

#### Enzyme
This slot lists the enzyme whose activity is described in this frame. More specifically, 
 the value of this slot is the key of a frame from the class [Protein-Complexes] or 
 [Polypeptides].
#### Km
The Michaelis constant (KM) of an enzyme is equal to the substrate concentration at which
 the rate of the reaction is at half of its maximum value. The Michaelis constant is an apparent
 dissociation constant of the enzyme-substrate complex, and thereby is an indicator
 of the affinity of an enzyme to a given substrate. Values of this slot are two-element lists
 of the form (cpd-frame Km) where cpd-frame is the frame id for a substrate of the reaction
 referred to by this enzymatic-reaction frame and Km is the Michaelis constant, a floating
 point number.
#### PhysiologicallyRelevant
PHYSIOLOGICALLY-RELEVANT?
#### Reaction
The value of this slot is the key of a frame from the Reactions class -- the second half 
 of the enzyme/reaction pair that the current frame describes. In fact, this slot can have 
 multiple values, which encode the multiple reactions that one catalytic site of an enzyme 
 catalyzes.
#### ReactionDirection
This slot specifies the directionality of a reaction. This slot is used in slightly different 
 ways in class Reactions and Enzymatic-Reactions. In class Enzymatic-Reactions, the slot 
 specifies information about the direction of the reaction associated with the enzymatic-reaction, 
 by the associated enzyme. That is, the directionality information refers only to the case in 
 which the reaction is catalyzed by that enzyme, and may be influenced by the regulation of 
 that enzyme.
 The slot is particularly important to fill for reactions that are not part of a pathway, because 
 for such reactions, the direction cannot be determined automatically, whereas for reactions 
 within a pathway, the direction can be inferred from the pathway context. This slot aids the user 
 and software in inferring the direction in which the reaction typically occurs in physiological 
 settings, relative to the direction in which the reaction is stored in the database.
#### RegulatedBy
The values of this slot are members of the [Regulation] class, describing activator or 
 inhibitor compounds for this enzymatic reaction.
#### RequiredProteinComplex
Some enzymes catalyze only a particular reaction when they are components of a larger 
 protein complex. For such an enzyme, this slot identifies the particular protein complex 
 of which the enzyme must be a component.
