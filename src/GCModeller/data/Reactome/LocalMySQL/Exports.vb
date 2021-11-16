#Region "Microsoft.VisualBasic::1ff83da89c6d481961821ba50603ded6, data\Reactome\LocalMySQL\Exports.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::83a7953cf8e92739ab08609f3e56dad0, data\Reactome\LocalMySQL\Exports.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    '     Class Exports
'    ' 
'    '         Properties: Ping
'    ' 
'    '         Constructor: (+1 Overloads) Sub New
'    '         Function: ExportReactionInformation
'    ' 
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports Oracle.LinuxCompatibility.MySQL
'Imports Oracle.LinuxCompatibility.MySQL.Reflection
'Imports Oracle.LinuxCompatibility.MySQL.Uri

'Namespace LocalMySQL.ExportServices

'    ''' <summary>
'    ''' Reactome Data Model
'    ''' 
'    ''' 1. Introduction
'    ''' 
'    ''' Life on the c ellular level is a network of molecular interactions. Molecules are synthesized and degraded, 
'    ''' undergo a bewildering array of temporary and permanent modifications, are transported from one location to 
'    ''' another, and form complexes with other molecules. Reactome represents all of this complexity as reactions in 
'    ''' which input physical entities are converted to output entities. These reactions can occur spontaneously or be 
'    ''' facilitated by physical entities acting as catalysts, and their progress can be modulated by regulatory effects of 
'    ''' other physical entities. Reactions are linked together by shared physical entities: a product from one reaction 
'    ''' may be a substrate in another reaction and may catalyze yet a thir d. It is often convenient, if sometimes 
'    ''' arbitrary, to group such sets of interlinked reactions into pathways.
'    ''' The functions of macromolecular entities such as proteins are often determined not only by their primary 
'    ''' sequenc es, but by chemical modific ations they have undergone. In Reactome, unmodified and modif ied forms 
'    ''' of a protein are distinct physical entities and the modification proc ess is treated as an ex plicit reaction. A 
'    ''' macromolecule’s function may depend on whether the molecule is free or complexed with specif ic other 
'    ''' molecules. Reactome treats complexes as physical entities distinct from their components, and the 
'    ''' multimerization events that build up complexes are modeled explicitly as reactions.
'    ''' Cellular compartments play a key role in biological proc esses. The segregation of molecules into different 
'    ''' compartments often regulates the reactions in which those entities can participate, or can be responsible for 
'    ''' driving a reaction forward. In Reactome, a molecule in one compartment is distinct from that molecule in 
'    ''' another compartment. Thus, extracellular and cytosolic glucose are different Reactome entities and, e.g., the 
'    ''' movement of glucose across the plasma membrane is a reaction that converts the extracellular glucose entity 
'    ''' into the cytosolic one. 
'    ''' Many biochemical entities and proc esses appear redundant: there are two or more chemically distinct entities 
'    ''' that can act more or less interchangeably. It is often useful to treat functionally equivalent protein isoforms, 
'    ''' splice variants, and paralogues as a single entity, implying that any individual entity from the given set could 
'    ''' fulfill the same role in a given situation. The Reactome data model allows this type of generalization, but does 
'    ''' so explicitly in a way that allows us to trac e specific functions back to the individual molecules covered by the 
'    ''' generalization.
'    ''' The goal of the Reactome knowledgebase is to represent human biological processes, but many of these 
'    ''' processes have not been directly studied in humans. Rather, a human event has been inferred from 
'    ''' experiments on material from a model organism. In such cases, the model organism reaction is annotated in 
'    ''' Reactome, the inferred human reaction is annotated as a separate event, and the inferential link between the 
'    ''' two reactions is explicitly noted.
'    ''' Reactome uses a frame- based knowledge representation. The data model consists of classes (frames) that 
'    ''' describe the different concepts (e.g., reaction, simple entity). Knowledge is captured as instanc es of these 
'    ''' classes (e.g., “glucose transport across the plasma membrane”, “cytosolic ATP”). Classes have attributes 
'    ''' (slots) which hold properties of the instances (e.g., the identities of the molecules that participate as inputs and 
'    ''' outputs in a reaction).
'    ''' 
'    ''' 2. Key data classes
'    ''' 
'    ''' 2.1 PhysicalEntity
'    ''' PhysicalEntities include individual molecules, multi-molecular complex es, and sets of molecules or complexes 
'    ''' grouped together on the basis of shared characteristics. Molecules are further c lassif ied as genome encoded 
'    ''' (DNA, RNA, and proteins) or not (all others). Attributes of a PhysicalEntity instanc e capture the chemical 
'    ''' structure of an entity, including any covalent modifications in the case of a macromolecule, and its subc ellular 
'    ''' localization. 
'    ''' PhysicalEntity instanc es that represent, e.g., the same chemical in different compartments, or different 
'    ''' post-translationally modified forms of a single protein, share numerous invariant f eatures such as names, 
'    ''' molecular structure and links to external databases like UniProt or ChEBI. To enable storage of this shared 
'    ''' information in a single place, and to create an explic it link among all the variant forms of what c an also be seen 
'    ''' as a single chemical entity, Reactome creates instances of the separate Refere nceEntity class. 
'    ''' A ReferenceEntity instance captures the invariant features of a molecule. A PhysicalEntity instance is then the 
'    ''' combination of a Referenc eEntity attribute (e.g., Glycogen phosphorylase UniProt:P06737) and attributes 
'    ''' giving specific conditional information (e.g., localization to the cytosol and phosphorylation on serine residue 
'    ''' 14).
'    ''' The PhysicalEntity class has subclasses to distinguish between different kinds of entity and to ensure data 
'    ''' integrity while enabling different handling rules for different categories:
'    ''' EntityWithAccessionedSequence - proteins and nucleic acids with known sequences.
'    ''' GenomeEncodedEntity - a species-specif ic protein or nucleic acid whose sequence is unknown, such as an 
'    ''' enzyme that has been characterized functionally but not yet purified and sequenced, e.g. cytosolic triokinase
'    ''' SimpleEntity - other fully characterized molecules, e.g. nucleoplasmic ATP or cytosolic glutathione
'    ''' Complex - a complex of two or more PhysicalEntities, e.g. Trimerization of the FASL:FAS receptor complex
'    ''' EntitySet - a set of PhysicalEntities (molecules or c omplexes) which function interchangeably in a given
'    ''' situation, e.g. Notch 3 heterodimer binds with a Notch ligand in the extracellular spac e. This notation allows
'    ''' collective properties of multiple individual entities to be described explicitly.
'    ''' 
'    ''' 2.2. CatalystActivity
'    ''' PhysicalEntities are paired with molecular functions taken from the Gene Ontology molecular function 
'    ''' controlled vocabulary to describe instanc es of biological cataly sis. An optional ActiveUnit attribute indicates 
'    ''' the specif ic domain of a protein or subunit of a c omplex that mediates the catalysis. If a PhysicalEntity has 
'    ''' multiple catalytic activities, a separate CatalystActivity is created for each. This strategy allo ws the association 
'    ''' of specific activities with specific variant forms of a protein or complex, and also enables easy retrieval of all 
'    ''' activities of a protein, or all proteins capable of mediating a specific molecular function.
'    ''' 
'    ''' 2.3. Event
'    ''' Events – the conversion of input entities to output entities in one or more steps – are the building blocks used 
'    ''' in Reactome to represent all biological processes. Two subclasses of Event are 
'    ''' recognized, ReactionlikeEventand Pathway. A ReactionlikeEvent is an event that converts inputs into outputs . 
'    ''' A Pathway is any grouping of related Events. An event may be a member of more than one Pathway.
'    ''' The ReactionlikeEvent class is further divided 
'    ''' into Reaction, BlackBoxEvent, Polymerisation andDepolymerisation. The Reaction class holds bona fide 
'    ''' reactions with balanced inputs and outputs. The BlackBoxEvent class is used for ‘unbalanced’ reactions like 
'    ''' protein synthesis or degradation, as well as ‘shortcut’ reactions for more complex proc esses that essentially 
'    ''' convert inputs into outputs, e.g. the series of cyclical reactions involved in fatty ac id biosynthesis. The 
'    ''' De-/Polymerisation classes can hold reactions that describe the mechanics of a de -/polymerisation reaction, 
'    ''' which is inherent ly ‘unbalanced’ due to the nature of a Polymer (that remains the ‘same’ entity even 
'    ''' after adding or subtracting a unit).
'    ''' 
'    ''' Full specification of the Reactome data model
'    ''' A full specification of all Reactome classes, slots and a listing of all instances of eac h class is accessible from 
'    ''' theSchema page on the top menu bar. There is also a Data model glossary on the Reactome wiki page, 
'    ''' giving more details on the usage of the various classes and slots.
'    ''' </summary>
'    Public Class Exports

'        Dim MySQL As MySqli

'        Sub New(DbServer As String,
'                UserName As String,
'                Password As String,
'                Optional DbName As String = "reactome",
'                Optional Port As Integer = 3306)

'            MySQL = New ConnectionUri With {
'                .Database = DbName,
'                .Port = Port,
'                .Password = Password,
'                .IPAddress = DbServer,
'                .User = UserName
'            }
'        End Sub

'        Public ReadOnly Property Ping As Double
'            Get
'                Return MySQL.Ping
'            End Get
'        End Property

'        Public Function ExportReactionInformation() As ObjectModels.Reaction()
'            Dim ReactionEntries As List(Of Tables.gk_current.reaction) =
'                CType(MySQL.UriMySQL, DataTable(Of Tables.gk_current.reaction)).Query("SELECT * FROM gk_current.reaction;")
'            Throw New NotImplementedException
'        End Function
'    End Class
'End Namespace
