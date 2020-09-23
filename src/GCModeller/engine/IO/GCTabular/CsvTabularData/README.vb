#Region "Microsoft.VisualBasic::ad3a7cfe2e650c4d4f75b6c1a6071480, engine\IO\GCTabular\CsvTabularData\README.vb"

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

    '     Module README
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace FileStream

    Module README
        Public ReadOnly README As Xml.Linq.XElement =
            <README>---==== !README ====---

If you are going to use the GCModeller, please cite our article:
G.Gang Xie, etc. A Comprehensive study of the genome wild regulation profile for bacteria Xanthomonas campestris pv. campestris str. 8004. NAR. 2014

If you have any question about the model data, please contact the program author from this E-Mail address:
Mr Xie's Gmail: xie.guigang@gmail.com

All of the model data is mainly consist of two parts of data file: 
* A XML file for define the cell system and its model data entry; 
* Some Csv Excel file that stores in sevral data directory define the kinetic model which was required of the whole cell simulation. 


* Directory descriptions:
1. The annotiations data for the model "{0}" was saved in data directory "./Annotations/"
2. The data model for the bacterial species "{1}" was saved in data directory "./DataModels/", all of the compiled mathematics kinetics model was saved in the csv format file in this directory is required for the running in the gchost program.
   PLEASE NOTICE: all of the column, which its name contains text like Handle or Hwnd, you should not modify its value in these csv files, as these column value is indicate the relationship between each object and construct the correct cell system network structure, the changed of these value will modified the network structure and lead to a unexpected wrong experiment data.
   There are some special file you should manual edit after the model compilation:
       a. Transcripts.Csv: All of the RNA object was defined in this file, the tRNA object defined here you should maunal edit its uniqueId property value for construct a correctly structure as it is differently decided how to define the tRNA in the program
       b. DispositionFlux.csv: You should manual modified the enzymes value in this file, thoese enzyme is the proteins that catalyst the degradation of the polypeptide or RNA molecule.
       c. You should maunal define the cultivating mediums, the data structure of the cultivating medium shows below:

        Column/Fields         Description
     +--------------------+--------------------------------------------------------------------------------------------------------------------------------------------------------+
        UniqueId	          The unique-id property value of target metabolite that exists in the cultivating mediums, its value should exists in the cell system metabolite models file.
        Handle	              Just leave this value empty if you have no idea how to modify it.
        UptakeRate	          How fast the cell can uptake this metabolite from the cultivating mediums
        MetabolitesHandle     Just leave this value empty if you have no idea how to modify it.

       d. Constraint_Metabolites.csv: the metabolite objects defined in this file is required for the transcription event and translation event, if the model source is comes from the BioCyc/MetaCyc database, then you have no need to edit this file again, but if you are using another database, you should manual edit this file for correct the substrate constraints in the cell protein expression events. The unique id value should be the amino acid tRNA and nucleotide, and these substrate is required for the synthesis the polypeptide and nucleotide acid sequence in the expression events. 
   Note: because for each time the compiler finish the job, when it save the model data, the exists file will be overwrite, so you should make a backup for those four data file I've mentioned above before you start the model compelling job.

3. Here is the most important part of the study in the in silicon virtual cell simulation experiment on the GCModeller platform is that you can carryout the experiment that the regular molecular biology can not carryout.
 
   The cell system experiment data was saved in the directory "./Experiments/", and each time you compile a cell model using this compiler, a default empty experiment file was saved in path "./Experiments/ExperimentTemplate.csv". From define the data record in this file, you can easily to create some experiment about the testing the signal transduction pathway or cell system robust of a specific mutation, you can try something more from the experiment definition.
   The experiments and the specific gene mutation operation in this simulation program it's the most amazing thing I have created, as the regular molecular experiment will takes days or Weeks to achive the experiment result or some molecular experiment is hardly to carryout, but in this in silicon experiment just takes few minutes calculation and easily designed the experiment to observers how the cell behavior change after the environment change or the changes in the gene expression when pathogenic bacteria interacting with its host.
   The experiments you have done here in the in silicon experiment is a pretty good pre-experiment test for your regular molecular biology experiment!
   The experiment file data structure was show below:

       Column/Fields      Description
    +-------------------+--------------------------------------------------------------------------------------------------------------------------------------------------------+
       Metabolite_Id	  The experiment target uniqueId property. Please notice that all of the experiment was defined on the change of some specific metabolite concentration which means all of the experiment is carry out in the cell system through the action of change the metabolite concentration.
       Trigger   	      Two types of the experiment trigger will be created from the value of this field: Periodically Experiment Trigger and Conditional Experiment Trigger.

                              1. Periodically Experiment Trigger: The experiment start time, this value should be a positive value and small than the total simulation time.
                              2. Conditional Experiment Trigger: This type of the experiment trigger is the most intelligent trigger and it is scriptable. The one-line script text in this field is enough to enable you to create a experiment that the regular molecular biology could not carryout!
                                 There are sevral key object in the currently version of GCModeller engine kernel that you must use in the trigger condition script text, you can review theirs avaliable public property in the developer SDK documents of the GCModeller:

                                     a. kernel(EngineSystem.Engine.Modeller): this keyword point to the entired GCModeller engine kernel object in the runtime, all of the public property in the kernel you can access from the script text using this keyword. And this makes the enough prevailage for you to control the whole system during its whole run time.
                                     b. transcription(EngineSystem.ObjectModels.System.ExpressionRegulationNetwork): this keyword indicate to the transcriptom system in the target simulated cell system in the runtime, from this keyword you can access the property of the transcription event flux distribution of each operon, the mrna concentration level of each gene, the degradation rate of each mrna in the real time.
                                     c. metabolisim(EngineSystem.ObjectModels.System.MetabolismCompartment): this keyword indicate to the metabolism system in the target simulated cell system in the runtime, from this keyword you can access the property of the metabolite concentration and the metabolism reaction flux value for Rachel metabolism reaction.
                                     d. cell(EngineSystem.ObjectModels.System.CellSystem): this keyword indicate to the target simulated cell system itself, you can access the life event activity ratio of the cell system.

                          Trigger creation script line syntax:

                              1. Periodically Experiment Trigger: This type of the trigger creation just simple: you just leave a integer number in the cell
                              2. Conditional Experiment Trigger: This type of the trigger creation is a little bit of complex: each trigger creation script should contains in a TEST() function call, and then a boolean expression should use for create the trigger

                                 Here is the create example:

                                     TEST( metabolism.WATER >= 100 and transcription.XC_2252 > 50 )

                                 The example indicate that the created conditiional experiment trigger object Will be trigged in the cell system status of a specific metabolite which name is WATER its concentration greater than or equals to 100 mmol/mml and the mRNA concentration of gene XC_2252 greater than 50 copy. 
                               
                                 All of the avaliable logical operator include: and, or, not;
                                 All of the avaliable quantity relationship operator include: &gt;, &gt;=, =, &lt;&gt;, &lt;, &lt;=;

       Interval	          This value descript the periodic behavior of the experiment, it should be a positive value
       TICKS	          This value descript the periodic behavior of the experiment, if its value equals 1 that it means the experiment no periodic behavior, and a number value large than 1 will specific that this experiment has a total number of TICKS period and with a specific period Interval.   
       DisturbingType	  This is a enumeration type value to descript the system disturbing type: 

                               * 0 - Increase, The specific metabolite its concentration or quantity will increate a Delta value for each time the experiment event happening. 
                               * 1 - Decrease, The specific metabolite its concentration or quantity will decrease a Delta value for each time the experiment event happening.
                               * 2 - ChangeTo, The specific metabolite its concentration or quantity will be changed to a specific value that setted in the definition for each time the experiment event happening.  

                          NOTE: any other value will be treated as 2(ChangeTo)         
       value              The experiment disturbing value of each time the experiment happening, this value is associated with the column DisturbingType

All of the model data was saved in the file format of Excel csv tabular data, and you can easily edit these model data from Microsoft Excel, WPS spreadsheets or OpenOffice program. and please be caution that when you are finish the data edit and wanna save the edited model data, you should saved the data in the file format of *.CSV in the filesave dialog as the Excel save the csv data as text file as default.

Have fun! 






            </README>
    End Module
End Namespace
