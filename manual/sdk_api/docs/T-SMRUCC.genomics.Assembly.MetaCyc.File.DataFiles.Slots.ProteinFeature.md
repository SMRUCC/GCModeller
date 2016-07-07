---
title: ProteinFeature
---

# ProteinFeature
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.html)_

This class describes sites of interest (such as binding sites, modification sites, cleavage
 sites) on a polypeptide. Instances of this class define a region of interest on a polypeptide,
 plus, optionally, one or more states associated with the region. Different subclasses are used 
 to specify single amino acid sites, linear regions, and regions involving noncontiguous 
 segments of an amino-acid chain. For example, an instance F of this class could define an amino 
 acid residue that can be phosphorylated, plus the fact that this residue can take on two 
 possible states: PHOSPHORYLATED and UNPHOSPHORYLATED.
 The feature instance itself does not describe the state of a particular protein. Instead, we 
 would represent the phosphorylated and unphosphorylated forms of a protein by creating two 
 instances of class Polypeptides. Both of those instances would link to the same feature F via 
 the FEATURES slot. However, in the two proteins, F would be annotated differently to indicate 
 the state of that feature. One protein would use an annotation label STATE with the value 
 PHOSPHORYLATED to denote that the residue is phosphorylated, while the other would use the same 
 annotation label STATE with the value UNPHOSPHORYLATED.




### Properties

#### AttachedGroup
For a binding feature, this slot lists the entity that binds to the protein feature — it can be
 either an instance of Chemicals or another Protein-Feature (e.g., in the case of crosslinks
 forming between two sites on the same or different polypeptide).
#### FeatureOf
This slot points to the polypeptide frames with which this feature is associated (there
 could be more than one such frame, if all are different forms of the same protein, e.g., a
 modified and an unmodified form).
#### LeftEndPosition
For a feature that consists of a contiguous linear stretch of amino acids, this slot encodes
 the residue number of the leftmost amino acid, with number 1 referring to the N-terminal
 amino acid.
#### PossibleFeatureStates
For a given feature class, this slot describes the possible states available to instances of
 the class. For example, a feature that represents a binding site can have either a bound or 
 unbound state. The list of possible states is stored at the class level as values for this slot. 
 
 A particular instance F of the class (a specific feature of a specific protein) can then be 
 labeled with this state information using the STATE annotation when F appears in the FEATURES 
 slot of the protein. For example, two forms of the same protein would link to the same feature 
 F, but one form P1 would have the feature annotated label STATE and value BOUND, whereas the 
 other form P2 would use the label STATE and value UNBOUND.
#### ResidueNumber
For a feature that consists of a single amino acid or some number of noncontiguous amino
 acids, this slot contains the numeric index or indices of the amino acid residue or residues
 that make up this site. Number 1 corresponds to the N-terminal amino acid.
#### RightEndPosition
For a feature that consists of a contiguous linear stretch of amino acids, this slot encodes
 the residue number of the rightmost amino acid, relative to the start of the protein.
