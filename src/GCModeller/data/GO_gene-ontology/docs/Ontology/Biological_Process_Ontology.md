# Biological Process Ontology Guidelines

A biological process is a recognized series of events or molecular functions. A process is a collection of molecular events with a defined beginning and end. Mutant phenotypes often reflect disruptions in biological processes.

## General Considerations

#### Beginning and end
Every process should have a discrete beginning and end, and these should be clearly stated in the process term definition.

#### Collections of processes
The biological process ontology includes terms that represent collections of processes as well as terms that represent a specific, entire process. Generally, the former will have mainly is_a children, and the latter will have part_of children that represent subprocesses. Also see "is_a or part_of" below.

#### is_a or part_of
To determine whether a process term should be an is a or part of child of its parent, ask: is an instance of the child process an instance of the entire parent process? That is, does the whole process, from start to finish, take place? If yes, the child is is a; but if the process is only a portion of the parent process, the child is part of.

#### Miscellaneous Standard Defs

+ **membrane fusion**
  The joining of the lipid bilayer membrane around X to the lipid bilayer membrane around Y.
+ **cellular component organization**
  A process that results in the assembly, arrangement of constituent parts, or disassembly of a cellular component.
+ **cellular component biogenesis**
  A process that results in the biosynthesis of constituent macromolecules, assembly, and arrangement of constituent parts of a cellular component.
+ **macromolecular complex assembly**
  The aggregation, arrangement and bonding together of a set of components to form a complex.
+ **xxx distribution**
  Any process that establishes the spatial arrangement of xxx.