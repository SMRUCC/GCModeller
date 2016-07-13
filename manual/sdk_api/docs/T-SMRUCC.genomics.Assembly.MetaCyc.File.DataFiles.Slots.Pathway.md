---
title: Pathway
---

# Pathway
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.html)_






### Properties

#### AssumeUniqueEnzymes
By default it is assumed that all enzymes that can catalyze a reaction will do so in each
 pathway in which the reaction occurs. That default assumption is encoded by the default
 value of FALSE for this slot; when you want to assume that only one enzyme exists in the
 DB to catalyze every reaction in this pathway, this slot should be given the value TRUE.
 This slot can be used for consistency-checking purposes, that is, in a pathway for which
 this slot is TRUE, there should not be any reactions that are catalyzed by more than one
 reaction.
#### ClassInstanceLinks
Each value of this slot is a reaction in the pathway. Two annotations (in addition to the
 usual possibilities) are available on this slot: REACTANT-INSTANCES and PRODUCTINSTANCES,
 whose values are compounds. If one of the reactants of the slot-value
 reaction is a class C and the REACTANT-INSTANCES are instances of C, then the instances
 are drawn as part of the pathway, with identity links to the class. The PRODUCTINSTANCES
 are treated similarly.
#### DisableDisplay
When the value is true, this slot disables display of the pathway drawing for a pathway.
#### EnzymesNotUsed
Proteins or protein-RNA complexes listed in this slot are those which would otherwise
 have been inferred to take part in the pathway or reaction, but which in reality do not.
 The protein may catalyze a reaction of the pathway in other circumstances, but not as
 part of the pathway (e.g. it may be not be in the same cellular compartment as the other
 components of the pathway, or it may not be expressed in situations when the pathway
 is active.).
#### EnzymeUse
By default it is assumed that all enzymes that can catalyze a reaction will do so in each
 pathway in which the reaction occurs. This slot is used in the case that this assumption
 does not hold, that is, if a reaction is catalyzed in a particular pathway by only a subset
 (or none) of the possible enzymes that are known to catalyze that reaction. Therefore,
 this slot can be used only when the value of the assume-unique-enzymes slot is FALSE
 (because multiple enzymes catalyze some step in the pathway).
 The form of a value for the slot is (reaction-ID enzymatic-reaction-ID-1... enzymaticreaction-
 ID-n). That is, each value specifies a reaction, and specifies the one or more
 enzymatic reactions that catalyze that reaction in this pathway. If no enzymatic reactions
 are specified, then none of the enzymes that are known to catalyze the reaction do so in
 this pathway.
 For example, under aerobic conditions the oxidation of succinate to fumarate is catalyzed
 by succinate dehydrogenase in the forward direction, and, under anaerobic conditions,
 by fumarate reductase in the reverse direction. The TCA cycle is active only in aerobic
 conditions, so only succinate dehydrogenase is used in this pathway. This fact would be
 recorded as follows:
 enzyme-use: (succ-fum-oxred-rxn succinate-oxn-enzrxn)
#### HypotheticalReactions
A list of reactions in this pathway that are considered hypothetical, probably because
 presence of the enzyme has not been demonstrated.
#### LayoutAdvice
Each value of this slot is a dotted pair of the form (advice-keyword . advice, and represents
 some piece of advice to the automatic pathway layout code. Currently supported
 advice keywords are
 1. :CYCLE-TOP-CPD: The advice is a compound key. In pathways containing a cycle,
 the cycle will be rotated so that the specified compound is positioned at twelve
 o’clock.
 2. :REVERSIBLE-RXNS: The advice is a list of reactions that should be drawn as reversible,
 even when the pathway is being drawn to show pathway flow (rather than
 true reversibility).
 3. :CASCADE-RXN-ORDERING: The advice is a list of reactions that form a partial order
 for reactions in a cascade pathway (i.e., the 2-component signaling pathways).
#### PathwayInteractions
This slot holds a comment that describes interactions between this pathway and other
 biochemical pathways, such as those pathways that supply an important precursor.
#### PathwayLinks
This slot indicates linkages among pathways in pathway drawings. Each value of this slot
 is a list of the form (cpd other-pwy*). The Navigator draws an arrow from the specified
 compound pointing to the names of the specified pathways, to note that the compound
 is also a substrate in those other pathways. If no other pathways are specified, then links
 are drawn to and from all other pathways that the compound is in (i.e., if the compound
 is produced by the current pathway, then links are drawn to all other pathways that consume
 it, and vice versa).
#### PolymerizationLinks
This slot controls drawing of polymerization relationships within a pathway. Each
 value of this slot is of the form (cpd-class product-rxn reactant-rxn). When both reactions
 are non-nil, an identity link is created between the polymer compound class cpd-
 class, a product of product-rxn, and the same compound class as a reactant of reactantrxn.
 The PRODUCT-NAME-SLOT and REACTANT-NAME-SLOT annotations specify
 which slot should be used to derive the compound label in product-rxn and reactant-rxn
 above, respectively, if one or both are omitted, COMMON-NAME is assumed. Either
 reaction above may be nil; in this case, no identity link is created. This form is used solely
 in conjunction with one of the name-slot annotations to specify a name-slot other than
 COMMON-NAME for a polymer compound class in a reaction of the pathway.
#### Predecessors
This slot describes the linked reactions that compose the current pathway. Since pathways
 have a variety of topologies — from linear to circular to tree structured — pathways
 cannot be represented as simple sequences of reactions. A pathway is a list of reaction/
 predecessor pairs.
#### Primaries
When drawing a pathway, the Navigator software usually computes automatically which
 compounds are primaries (mains) and which compounds are secondaries (sides). Occasionally,
 the heuristics used are not sufficient to make the correct distinction, in which
 case you can specify primary compounds explicitly. This slot can contain the list of primary
 reactants, primary products, or both for a particular reaction in the pathway. Each
 value for this slot is of the form (reaction-ID (primary-reactant-ID-1 ... primary-reactant-
 ID-n) (primary-product-ID-1 ... primary-product-ID-n)), where an empty list in either
 the reactant or product position means that that information is not supplied and should
 be computed. An empty list in the product position can also be omitted completely.
 For example, in the purine synthesis pathway, we want to specify that the primary product
 for the final reaction in the pathway should be AMP and not fumarate. The primary
 reactants are still computed. The corresponding slot value would be
 primaries: (ampsyn-rxn () (amp))
#### ReactionList
This slot lists all reactions in the current pathway, in no particular order.
#### Species
This slot is used only in pathway frames in the MetaCyc DB, in which case the slot identifies
 the one or more species in which this pathway is known to occur experimentally.
#### SubPathways
This slot is the inverse of the Super-Pathways slot. It lists all the direct subpathways of a
 pathway.
#### SuperPathways
This slot lists direct super-pathways of a pathway.
