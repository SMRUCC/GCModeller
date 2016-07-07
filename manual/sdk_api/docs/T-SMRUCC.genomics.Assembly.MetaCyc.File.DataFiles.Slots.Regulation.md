---
title: Regulation
---

# Regulation
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.html)_

This class describes most forms of protein, RNA or activity regulation. Regulation can
 be either by a direct influence on the protein’s activity (e.g. allosteric inhibition of an
 enzyme) or by influencing the quantity of active protein available (e.g. by inducing or
 blocking its transcription or translation). The one form of regulation that is not covered
 by this class is when the quantity of a protein is regulated as a result of chemical
 or binding reactions that either produce or consume the active form of a protein – these
 are represented as Reactions instead. There can be some ambiguity as to what should be
 represented as a reaction and what should be represented as a regulation event. In general,
 an event that can be represented as a reaction should be when a) there is sufficiently
 detailed information known to model it as a reaction, b) both reactants and products exist
 as stable, independent entities, and c) our schema supports referring to both reactant
 and product of the reaction independently and there is some justification for wanting
 to go down to that level of detail. For example, a transcription factor bound to a small
 molecule will generally have a different activity than the unbound transcription factor.
 This could be represented either as the reaction TF + x -¿ TF-x or as a regulation event in
 which x activates or inhibits the activity of TF. However, because both TF and TF-x are
 stable molecules which can potentially regulate different transcription units (not all will,
 but some do), or TF could bind another small molecule y and regulate yet another set of
 transcription units, we prefer to model this kind of interaction as a reaction when the data
 is available. On the other hand, an enzyme binding to some inhibitor could also be represented
 as a reaction, but since there is rarely any reason to refer to the enzyme-inhibitor
 complex outside of the context of the reaction the enzyme catalyzes, we choose instead
 to model these events as regulation events in which the inhibitor regulates the activity of
 the enzyme.
 Instances of this class represent a one-to-one mapping between regulator and regulatedentity
 (i.e. an entity may regulate many processes, or a process may be regulated by many
 entities, but each one requires its own instance of Regulation to represent it)




### Properties

#### AssociatedBindingSite
This slot is applicable to regulation of transcription or translation in which an entity 
 (protein, small-molecule or RNA) binds to DNA or the mRNA transcript. Its values are 
 instances of either DNA-Binding-Sites or mRNA-Binding-Sites, depending on the type of
 regulation.
#### IsMetabolismRegulation
当前的调控类型对象是否为对酶促反应的调控类型对象
#### Ki
This slot is used for instances of regulation of enzyme activity. Ki is the dissociation
 constant for the binding of an inhibitor to an enzyme or an enzyme-substrate complex.
 When the inhibitor is competitive, Ki is the dissociation constant for the binding of an
 inhibitor to the enzyme, and is often written as Kic. When the inhibitor is uncompetitive,
 Ki is the dissociation constant for the binding of an inhibitor to the enzyme-substrate
 complex, and is often written as Kiu or Ki. The units for Ki are mole.
#### Mechanism
This slot optionally contains a keyword which describes the mechanism of the regulation.
 Appropriate possible values will vary depending on the particular subclass of regulation.
 Some subclasses will not use this slot at all.
#### Mode
This slot specifies whether the regulator activates or inhibits the regulated-entity. Possible
 values are:
 “+” — The regulator activates or increases quantity or activity of the regulated-entity
 (an exception is transcription attenuation, in which even though the regulated-entity is a
 terminator object, “+” means activation of transcription of the downstream genes rather
 than of the terminator).
 “-” — The regulator inhibits or decreases quantity or activity of the regulated-entity (with
 the same caveat about transcription attenuation as above)..
#### RegulatedEntity
This slot links the regulation frame to the object that is being regulated. In the case of
 enzyme modulation, this object will be an Enzymatic-Reaction frame. In the case of transcription
 initiation regulation, it will be a Promoter frame. In the case of transcription
 attenuation, it will be a Terminator frame. In other cases, it could be a gene or a protein
 frame. The regulated entity will link back to the regulation frame using the inverse of this
 slot, Regulated-By
 (本属性连接本类型的对象至被其所调控的目标对象。当调控对象为酶活力调控的时候，将会链接至酶促反应对象，
 当调控类型为转录起始调控的时候，目标对象将会是启动子对象，当为转录终止调控的时候，目标对象将会是一个
 终止子对象，对于其他情况而言，所调控的目标对象可能为一个基因或者蛋白质。对于被调控的目标对象而言，都
 会有一个Regulated-By属性连接回本对象之上)
#### Regulator
This slot links the regulation frame to the object that is doing the regulating, typically
 a protein, RNA or small molecule. The regulator frame will link back to the regulation
 frame using the inverse of this slot, Regulates.
