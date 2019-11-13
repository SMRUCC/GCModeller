#Region "Microsoft.VisualBasic::f2248b017588c10bb89a2ce1fb6eec54, engine\IO\GCTabular\CsvTabularData\Storage\NodeDocument.vb"

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

    '     Module NodeDocument
    ' 
    '         Properties: DataAnnotationComment, EnvironmentnComments, PhenotypeDataComment, ProgrammingDataComment, SignalTransductionNetwork
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace FileStream.XmlFormat

    Module NodeDocument

        Public ReadOnly Property DataAnnotationComment As String
            Get
                Return _
<Comment.DataAnnotations>
GCModeller virtual cell bacterial annotation data, this chunk of the data is not required for the simulation. 
This chunk of the data mainly contains of the data required for well learn of a kind of bacteria species, or 
any other data information required for the manual data-error correction of the virtual cell model.

The data stores in this section contains the data such as genome functional annotation, genome sequence, 
protein sequence, reaction information, etc.

</Comment.DataAnnotations>
            End Get
        End Property

        Public ReadOnly Property ProgrammingDataComment As String
            Get
                Return _
<Comment.Bacteria_Genome_Programming_Information>
This section of the data chunk is required for the virtual cell model genome wide dynamics regulation property.
For the briefly, the programming information of a bacteria genome is the generated phenotype raising from the 
emergency of the complex genome wide regulation network interactions. To build up such a complex next, the motif 
information, bacteria regulator information and signal transduction network information is required.

How to generate such bacteria genome programming information data? This list may be help when you are using the 
GCModeller to create a virtual cell model of a bacteria:

1. genome wide regulation data
the regulation model in the GCModeller modelling engine kernel is based on a DFL network, and it is a possibility 
based event, form the DFL network model, the gene is transcript from a independent transcription unit object. 
you can generate such data from the door operon predicted data. and the DFL regulation model assume that, to 
regulates these transcription unit, serval motif site which are distributed on the transcript unit is required. 
briefly concludes this model is that, one transcript unit express one or more CDS product and threr one or more 
motif site on that transcript unit, each motif site was regulated by one or more regulators. you can generate 
these genome wide regulation data from the MEME module, and this data in the MEME module is called footprint data. 

2. signal transduction network
The main component of a bacteria species' signal transduction network is the TCS cross talk network, this is 
required for the formation of the component to response to the environment changes in the GCModeller simulation 
experiment. you can obtain the bacteria signal transduction network annotation data from the mist2 database. but 
the mist2 database is not enough, the data in the mist2 database just the component annotation data, without any 
interaction data. to obtains these cross-talk data is not easily from the laboratory experiments, but is easily 
predicted from the exists experiment data using the Bayesian network simulation. you can get these data from the
protein interaction module in this "genome-in-code" virtual cell modelling package. 

3. enzyme mapping data for generate cell phenotype
As some regulator is the one component type regulator, and that regulator is required some specific metabolite as 
the signal sensor, and these metabolite component is called effector to a TF regulator. some of the effector is comes
from the inner cellular metabolism pathway and other is comes from the environment which is call the secondary messager.
no matter what the effector is, all of these metabolite to effect on the gene expression regulations is required of 
the metabolism pathway network:
inner cellular metabolite required of the enzyme to catalyzed the reaction to synthesis and the external secondary 
messager required the trans-membrane transportation pathway to get inside the cell compartment. to formation a 
correctly regulation network, required you highly accurately annotation the enzyme class of each gene product. 
so that this part of the data is important to support of the genome programming events.   

</Comment.Bacteria_Genome_Programming_Information>
            End Get
        End Property

        Public ReadOnly Property SignalTransductionNetwork As String
            Get
                Return _
<Comment.SignalTransductionNetwork>
As we've mention above about the genome programming data in the GCModeller virtual cell. the signal transduction network is 
required for the virtual cell object response to experiment environmental stimuli. The signal transduction network is mainly 
consist of the chemotaxis system and two component system. the one component system is mainly response to the cellular 
environment, so that the ocs is not define in this section, but defines in the genome programming data section in the footprint
regulation model data.
in the GCModeller virtual cell, when you have modify the experiment environment variable, some special stimulating compound 
quantity changes, so that the signal will be sensed by the chemotaxis system, and then the signal will be transfer to the two 
component system, at last the tcs_rr component will effect on the gene expression event. then the cell object will changes it 
expression pattern in real time, the cell phenotype will changed through the pathway flux value changes. which means the 
virtual cell responded to the environment stimulation. 
</Comment.SignalTransductionNetwork>
            End Get
        End Property

        Public ReadOnly Property PhenotypeDataComment As String
            Get
                Return _
<Comment.Cell_Phenotype>
The correctly genome programming generated the cell phenotype, and you can view the phenotype as the result of 
the simple node interactions in the complexity gene expression regulation network. so, how to defined a cell phenotype,
a simple but not so correctly anwser is the KEGG pathway category information on the KEGG database. the cell phenotype 
in the GCModeller can be describe as the flux value changes in the pathway object. and based on the KEGG database 
descriptions, on kind of cell phenotype is arising from changes of the sevral functional related pathway. so that I think
the cell phenotype can be describe by the KEGG pathway category data

this part of the data is mainly consist of the KEGG pathway data, kegg reaction data and kegg compounds data. due to 
the reason of the GCModeller is using the kegg pathway category data to investigates the cell phenotype changes after 
the gene mutation. but if have another kind of the pathway category inforation, you can replace these kegg data using 
the customized data that you haved.

to investigates the cell phenotype changes, a software called pfsnet is doing its job on this kind of calculation, you 
can analysis the cell phenotype changes data after the virtual cell simulation using the Toolkits.RNASeq module. 
</Comment.Cell_Phenotype>
            End Get
        End Property

        Public ReadOnly Property EnvironmentnComments As String
            Get
                Return _
<Comment.Environment>
The environment model defines the GCModeller virtual cell experinment environment. due to the reason of bacteria may adapted 
to the environment, so that you can change the paramater of the cultivating mediums to compares the expression diferrennce 
of the virtual cell simulation.
</Comment.Environment>
            End Get
        End Property
    End Module
End Namespace
