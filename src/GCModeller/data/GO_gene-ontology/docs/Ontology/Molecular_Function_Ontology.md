# Molecular Function Ontology Guidelines

## The Essence of a Function Term
The functions of a gene product are the jobs that it does or the "abilities" that it has. These may include transporting things around, binding to things, holding things together and changing one thing into another. This is different from the biological processes the gene product is involved in, which involve more than one activity. One way to understand this is to consider the analogy of a company or organization. Individuals (gene products) have different abilities or tasks (functions) and they work together to achieve different goals (processes). It is easy to confuse a job title (gene product name) with a function; for example, 'secretarial activity' may seem like a valid function because you have a good conceptual idea of what a secretary does. However, in different companies, secretaries might do different things. One secretary might have the functions 'typing', 'answering phone' and 'making coffee', whilst another might have these functions and additionally 'photocopying'. In the Gene Ontology, a function should be unambiguous and it should mean the same thing no matter what species you are dealing with. If there's any conceptual ambiguity, or you think there could be ambiguity, check to make sure that you're actually talking about a function and not about a gene product.

## Standard Definitions
The following function terms have standard definitions:

+ **x binding**
  Interacting selectively and non-covalently with x, [brief description of x].
+ **[enzyme] activity**
  Catalysis of the reaction: [reaction catalyzed by enzyme].
+ **x receptor activity**
  Combining with x to initiate a change in cell activity.
+ **x transporter activity**
  Enables the directed movement of x into, out of or within a cell, or between cells.

### Standard Synonyms
Synonyms are added to aid searching. The following standard synonyms are used in the molecular function ontology.

+ **x receptor binding**
  x receptor ligand*
+ **x receptor binding**
  The gene name or gene class of any ligand that can bind the x receptor.*

*N.B. The synonym x receptor ligand describes something that acts as a ligand for the x receptor. Similarly, a gene name or gene class could be added if it binds to that receptor. These phrases are not exact synonyms but are useful search tools for biologists in a specific field. In given biological fields, gene names or classes may be used in the literature to refer to a concept that is analogous to a GO function.

## Parentage and Annotation
A gene product may have many different functions, but it would be wrong to create a function term that represents multiple functions. Gene product information should be captured at the annotation stage, by annotating the gene product to several function terms, rather than by hardwiring the information into the ontology by adding extra parents. If a term has parentage which isn't immediately obvious from the term name or the definition, and therefore requires you to have background knowledge, then it's probable that the function term has been mistaken for the gene product of the same name and gene product specific information has been incorporated by adding extra parents.

A good example is the term retinoic acid receptor activity, which was wrongly given the parents receptor activity and transcription regulator activity. The ontology structure looked like this:

+ molecular function
  + [i]receptor activity
  + [i]retinoic acid receptor activity
...

+ [i]transcription regulator activity
  + [i]retinoic acid receptor activity

The gene product retinoic acid receptor alpha could be annotated to retinoic acid receptor activity, and, by reasoning over the GO graph, it would also be annotated to the parent terms receptor activity and transcription regulator activity.

The definition (a standard GO receptor activity definition) is "Combining with retinoic acid to initiate a change in cell activity". With extra background reading, you might find out that retinoic acid receptors can function as transcriptional regulators, but there is nothing in the term name or definition to suggest any relationship with transcriptional regulators. If a relationship isn't obvious from the term name or definition, it's probably referring to a gene product property. Encoding gene product information in the function ontology like this has the added hazard of species specificity: in some organisms, a gene product may have different functions to those it has in another organism.

The correct way to deal with this situation is to remove the link in the ontology between retinoic acid receptor activity and transcription regulator activity and capture the information in the annotation instead.

+ molecular function
  + [i]receptor activity
    + [i]retinoic acid receptor activity
...
+ [i]transcription regulator activity

We would annotate retinoic acid receptor alpha to both retinoic acid receptor activity and transcription regulator activity.

## Granularity
GO functions describe interactions at the level of molecules, rather than atoms. Therefore a reaction would not be split into function terms describing each step of the reaction in atomic or subatomic terms (eg. electrons attracted to positive charge or formation of unstable intermediate); it would consider the starting state and the end state in terms of the molecules involved. As a consequence of this, separate function terms would be created to cover the various situations in which different reaction mechanisms provide the route between the same set of same reactants and products. In addition to this, GO functions should not cover reactions that always occur spontaneously and without the need for a gene product catalyst. Since there is no gene product involved in such a reaction, the term would never be used for annotation.

If you have a reaction where you can pick out molecular intermediates, you should consider whether to make multiple function terms. If the intermediates are known to be released and used in other biological processes, function terms should be added to represent the steps of the reaction, and a biological process term added to represent the sum of these functions. If the separate activities generating and acting on the molecular intermediates can be associated with different subunits of the enzyme, two or more functions should probably be made. If neither of these conditions holds, a single function term can be made.

In the reaction catalyzed by magnesium-protoporphyrin IX monomethyl ester (oxidative) cyclase, the intermediates are not released and the three separate catalytic activities are not associated with different subunits of the enzyme, so a single function term is appropriate.

Enzyme Commission entry for magnesium-protoporphyrin IX monomethyl ester (oxidative) cyclase, EC 1.14.13.81:

1. magnesium-protoporphyrin IX 13-monomethyl ester + NADPH + H+ + O2 = 13(1)-hydroxy-magnesium-protoporphyrin IX 13-monomethyl ester + NADP+ + H2O
2. 13(1)-hydroxy-magnesium-protoporphyrin IX 13-monomethyl ester + NADPH + H+ + O2 = 13(1)-oxo-magnesium-protoporphyrin IX 13-monomethyl ester + NADP+ + 2 H2O
3. 13(1)-oxo-magnesium-protoporphyrin IX 13-monomethyl ester + NADPH + H+ + O2 = divinylprotochlorophyllide + NADP+ + 2 H2O

GO molecular function term:
**magnesium-protoporphyrin IX monomethyl ester (oxidative) cyclase activity**
Catalysis of the reaction: magnesium-protoporphyrin IX 13-monomethyl ester + 3 NADPH + 3 H+ + 3 O2 = divinylprotochlorophyllide + 3 NADP+ + 5 H2O.
Conversely, the reactions of the glycine cleavage system , a multienzyme complex involved in the catabolism of glycine, should be represented by separate function terms.
The overall reaction of the complex is glycine + tetrahydrofolate + NAD = NH3 + 5,10-methylene-THF + CO2 + NADH

but this can be split into steps, which, by the criteria above, warrant individual function terms.

GO molecular function terms:

**glycine dehydrogenase (decarboxylating) activity**
Catalysis of the reaction: glycine + H-protein-lipoyllysine = H-protein-S-aminomethyldihydrolipoyllysine + CO2.
**aminomethyltransferase activity**
Catalysis of the reaction: protein-S-aminomethyldihydrolipoyllysine + tetrahydrofolate = protein-dihydrolipoyllysine + 5,10-methylenetetrahydrofolate + NH3.

## Classifying Enzymatic Reactions
The function ontology has terms representing many of the enzymes in the Enzyme Commission (EC) database, and it uses the EC classification system to group and classify them. To stay in line with EC, proposed enzyme function terms should be checked in the EC database to ensure that the EC recommended name is used. Not all enzyme entries in the EC database are converted directly to single entries in the function ontology, because some enzymes carry out multiple functions.
If an EC number is given, the term can be added under the EC parent term. For example, thiamin-triphosphatase activity, EC:3.6.1.28, should be added under the parent EC:3.6.1.-, hydrolase activity, acting on acid anhydrides, in phosphorus-containing anhydrides ; GO:0016818. A check of the proposed sibling terms should reveal similar reactions and EC numbers in the same range (EC:3.6.1.x in this case).

If an EC entry cannot be found for the enzyme, it may be worth checking some other databases for it. BRENDA contains the same enzymes as the EC database, but with a greater number of alternative names; MetaCyc University of Minnesota Biocatalysis/Biodegradation Database (UM-BBD) both contain reactions not covered by EC which they have given a partial EC number (eg. EC:2.1.-.-). Both these databases can be used in the general dbxrefs; examples would be MetaCyc:GLYOXIII-RXN and UM-BBD_enzymeID:e0225 respectively.

### Bi-directional Reactions
For bi-directional reactions, we will create a single term that describes both directions of the reaction unless there is reason to believe that there is a biological justification to separate the two directions of the reaction into separate terms.
A single term covering both directions of a reaction

**NADP phosphatase activity**
Catalysis of the reaction: H2O + NADP = NAD + phosphate.
Two separate terms covering the opposing directions of a single reaction
**sodium-transporting ATP synthase activity, rotational mechanism ; GO:0046932**
Catalysis of the reaction: ADP + phosphate + Na+(out) = ATP + H2O + Na+(in), by a rotational mechanism.
**sodium-transporting ATPase activity, rotational mechanism ; GO:0046962**
Catalysis of the reaction: ATP + H2O + Na+(in) = ADP + phosphate + Na+(out), by a rotational mechanism.

## Valid Function Terms
These are some guidelines for deciding whether a term is a valid molecular function or not.
For a function term that considers binding you must know the molecule that is being bound. For example you wouldn't say 'vesicle binding'; instead you would find out which protein in the vesicle membrane was being bound and use that in the term name.

The function must be a single reaction step. Anything that requires multiple steps is a process.

Functions are not restricted to the activities of single gene products; multi-gene product complexes can also have functions.

Do not confuse the following:

+ Two things that happen at the same time or that are done by the same molecule.
+ Two things that are dependent on each other and cannot occur independently.

For example, the proposed term actin binding with sliding actually includes the two functions binding and motor activity, so it is not appropriate as a function term. However, calcium-transporting ATPase activity represents two activities that are dependent on each other and cannot occur independently; thus calcium-transporting ATPase activity is appropriate as a GO molecular function.
Following on from this, it is also important not to confuse the case of two interdependent activities with the superficially similar situation where a process and an activity are dependent on each other. For example, cell adhesion receptor activity would not be a function ontology term since it describes the activity of receptor activity coupled to the process of cell adhesion.

It helps to consider the term name. Is it immediately obvious what's going on or does it sound like a gene product with 'activity' stuck on the end? For example with transporter activity, you know immediately what kind of function this is describing; whereas with actin activity it not really clear. It should be obvious what a function is without in-depth biological knowledge of a certain area.

### Avoid Cellular Component Information
Cellular structures are not functions. Many cellular component references have been made obsolete in the function ontology. For example, a mitochondrial primase needs only be primase activity because annotators can assign location to gene products by annotating with appropriate terms from the cellular component ontology. By contrast, there are many cases where component terms are appropriate in the process ontology. For example, Golgi organization and biogenesis is different from lysosome organization and biogenesis, so the anatomical qualifiers 'Golgi' and 'lysosome' are necessary.

### Avoid Gene Products
Gene products in themselves are not nodes of the function ontology, although doing something with or to a specific gene product can be one. For example, being hedgehog or a hedgehog receptor are not functions, but hedgehog receptor binding and hedgehog binding are functions. Most GO molecular function terms include the word 'activity' to help differentiate them from the physical gene product. When defining molecular function terms, be careful not to describe them as gene products. For example, the molecular function term kinase activity is defined as 'Catalysis of the transfer of a phosphate group, usually from ATP, to a substrate molecule', not 'an enzyme that catalyzes the transfer of a phosphate group, usually from ATP, to a substrate molecule'.

### Function Terms for Subunits
Regulatory and catalytic subunits of kinases, heterotrimeric G proteins, etc., are represented in the function ontology by a regulator activity term, under the enzyme regulator activity node, and an enzyme activity term, under the catalytic activity node.
Note that GO no longer uses the part of relationship between the enzyme regulator term and the catalytic activity term. A full discussion of this topic can be found under 'Annotation Issues' in the minutes from the September 2003 Bar Harbor GO meeting.

Please see the GO annotation guide for advice on how to annotate subunits of a complex.

### Avoid Binding Relationships
See the binding working group pages on the GO wiki for more on binding and the current thinking on how to annotate binding.
Catalytic activities should not be related in the ontology to binding terms; for example, ATPase activity should not have a relationship to ATP binding hard-coded in the ontology. Binding terms should only be used in cases where a stable binding interaction occurs. There are several reasons for this.

Firstly, transporter, catalysis and binding activities are all in the function ontology, which is used to describe elemental single step activities that occur at the macromolecular level. That means that if we were to further subdivide these functions - for example, splitting the catalysis of a reaction into steps such as "substrate binding", "formation of unstable intermediate" or "attraction of electrons to positive charge" - we would be saying that a reaction was actually a series of functions - i.e. a process. Additionally, we would be going beyond the scope of the molecular function ontology as we would be dealing with events on a molecular or atomic level.

Another reason is the sheer practicality of sorting through the 4000+ catalytic reactions we have in GO and deciding which of the substrates and products should be given 'binding' terms. Should we say that only substrates are bound by an enzyme? How about reversible reactions or cases where the reaction mechanism is unknown?

Finally, the GO binding terms are supposed to represent stable binding interactions, as opposed to the transient binding that occurs prior to catalysis. Hence there should not be a connection between stable binding and catalysis.

## Function Grouping Terms
Terms in the function ontology should be grouped on the basis of functional similarity, rather than being involved in the same process. For example, the grouping term monosaccharide transporter activity might have children such as glucose transporter activity and ribose transporter activity, and is a valid function term in its own right. However, the term defense/immunity protein activity, used to group terms such as antigen binding, blood coagulation factor activity and Fc receptor activity, is not a valid function as it represents a protein involved in the defense or immune response (process) of an organism. If a grouping term is not a function itself, or it contains disparate children with no functional similarity, it should be made obsolete.

## Appending Terms with 'Activity'
GO molecular function terms are all appended (with the exception of the root 'molecular function', and all binding terms) with the word 'activity'. This is because GO molecular functions are what philosophers would call 'occurrents', meaning events, processes or activities, rather than 'continuants' which are entities e.g. organisms, cells, or chromosomes. The word activity helps distinguish between the protein and the activity of that protein, for example, nuclease and nuclease activity.
In fact, a molecular 'function' is distinct from a molecular 'activity'. A function is the potential to perform an activity, whereas an activity is the realization, the occurrence of that function; so in fact, 'molecular function' might more properly be renamed 'molecular activity'. However, for reasons of consistency and stability, the string 'molecular function' endures.