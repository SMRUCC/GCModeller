#Region "Microsoft.VisualBasic::ec7d04826673fe14e21311a1a931a899, data\GO_gene-ontology\GeneOntology\Ontologies.vb"

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

    ' Enum Ontologies
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel

''' <summary>
''' The Gene Ontology project provides controlled vocabularies of defined terms representing gene product properties. 
''' These cover three domains: 
''' 
''' + ``Cellular Component``, the parts of a cell or its extracellular environment; 
''' + ``Molecular Function``, the elemental activities of a gene product at the molecular level, such as binding or catalysis; 
''' + ``Biological Process``, operations or sets of molecular events with a defined beginning and end, pertinent to the 
'''   functioning of integrated living units: cells, tissues, organs, and organisms.
''' 
''' The GO ontology Is structured As a directed acyclic graph where Each term has defined relationships To one Or more other 
''' terms In the same domain, And sometimes To other domains. The GO vocabulary Is designed To be species-agnostic, And 
''' includes terms applicable To prokaryotes And eukaryotes, And Single And multicellular organisms.
''' 
''' In an example of GO annotation, the gene product ``cytochrome c`` can be described by the ``Molecular Function`` term 
''' ``oxidoreductase activity``, the ``Biological Process`` terms ``oxidative phosphorylation`` And ``induction of cell death``, 
''' And the ``Cellular Component`` terms ``mitochondrial matrix`` And ``mitochondrial inner membrane``.
''' </summary>
Public Enum Ontologies As Byte

    ''' <summary>
    ''' These terms describe a component of a cell that is part of a larger object, such as an anatomical structure 
    ''' (e.g. rough endoplasmic reticulum or nucleus) or a gene product group (e.g. ribosome, proteasome or a protein dimer).
    ''' </summary>
    <Description("cellular_component")> CellularComponent
    ''' <summary>
    ''' A biological process term describes a series of events accomplished by one or more organized assemblies of molecular functions. 
    ''' Examples of broad biological process terms are "cellular physiological process" or "signal transduction". Examples of more 
    ''' specific terms are "pyrimidine metabolic process" or "alpha-glucoside transport". The general rule to assist in distinguishing 
    ''' between a biological process and a molecular function is that a process must have more than one distinct steps.
    ''' A biological process Is Not equivalent To a pathway. At present, the GO does Not Try To represent the dynamics Or dependencies 
    ''' that would be required To fully describe a pathway.
    ''' </summary>
    <Description("biological_process")> BiologicalProcess
    ''' <summary>
    ''' Molecular function terms describes activities that occur at the molecular level, such as "catalytic activity" or "binding activity". 
    ''' GO molecular function terms represent activities rather than the entities (molecules or complexes) that perform the actions, 
    ''' and do not specify where, when, or in what context the action takes place. Molecular functions generally correspond to activities 
    ''' that can be performed by individual gene products, but some activities are performed by assembled complexes of gene products. 
    ''' Examples of broad functional terms are "catalytic activity" and "transporter activity"; examples of narrower functional terms are 
    ''' "adenylate cyclase activity" or "Toll receptor binding".
    ''' It Is easy To confuse a gene product name With its molecular Function; For that reason GO molecular functions are often appended 
    ''' With the word "activity".
    ''' </summary>
    <Description("molecular_function")> MolecularFunction
End Enum
