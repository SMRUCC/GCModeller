#Region "Microsoft.VisualBasic::33448238955f7c045fb0bc433e91d334, CARMEN\Analysis\CARMEN.vb"

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

    ' Module CARMEN
    ' 
    '     Function: __getKEGGReactionId, LoadDocument, Merge, Replace
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.Level2.Elements

''' <summary>
''' Mapping between the kegg database and the target bacteria genome.
''' </summary>
<Package("Annotations.CarmenMapping",
                    Description:="Mapping between the kegg database and the target bacteria genome.",
                    Cites:="Schneider, J., et al. (2010). ""CARMEN - Comparative Analysis And in silico Reconstruction of organism-specific MEtabolic Networks."" Genet Mol Res 9(3): 1660-1672.
<p>New sequencing technologies provide ultra-fast access to novel microbial genome data. For their interpretation, an efficient bioinformatics pipeline that facilitates in silico reconstruction of metabolic networks is highly desirable. The software tool CARMEN performs in silico reconstruction of metabolic networks to interpret genome data in a functional context. CARMEN supports the visualization of automatically derived metabolic networks based on pathway information from the KEGG database or from user-defined SBML templates; this software also enables comparative genomics. The reconstructed networks are stored in standardized SBML format. We demonstrated the functionality of CARMEN with a major application example focusing on the reconstruction of glycolysis and related metabolic reactions of Xanthomonas campestris pv. campestris B100. The curation of such pathways facilitates enhanced visualization of experimental results, simulations and comparative genomics. A second application of this software was performed on a set of corynebacteria to compare and to visualize their carbohydrate metabolism. In conclusion, using CARMEN, we developed highly automated data analysis software that rapidly converts sequence data into new knowledge, replacing the time-consuming manual reconstruction of metabolic networks. This tool is particularly useful for obtaining an overview of newly sequenced genomes and their metabolic blueprints and for comparative genome analysis. The generated pathways provide automated access to modeling and simulation tools that are compliant with the SBML standard. A user-friendly web interface of CARMEN is available at http://carmen.cebitec.uni-bielefeld.de.",
                    Publisher:="xie.guigang@gmail.com")>
Public Module CARMEN

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="DIR"></param>
    ''' <param name="EnzymaticOnly">是否仅返回只具有酶分子的反应过程</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    <ExportAPI("Doc.Merge")>
    Public Function Merge(DIR As String, <MetaData.Parameter("Enzymatic.Only")> Optional EnzymaticOnly As Boolean = True) As Reaction()
        Dim lstFiles = (From sbml As String
                        In FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchAllSubDirectories, "*3-1.sbml").AsParallel
                        Select pwy = FileIO.FileSystem.GetFileInfo(sbml).Directory.Name,
                            model = LoadDocument(sbml)).ToArray
        Dim list As New List(Of Reaction)
        Dim setValue = New SetValue(Of Reaction)().GetSet(NameOf(Reaction.Pathway))

        For Each pwyModel In lstFiles
            Dim LQuery As Reaction() =
                LinqAPI.Exec(Of Reaction) <= From rn As Level3.Reaction
                                             In pwyModel.model.model.listOfReactions
                                             Select setValue(Reaction.CreateObject(rn), pwyModel.pwy)
            Call list.AddRange(LQuery)
        Next

        If EnzymaticOnly Then
            Dim LQuery = (From rxn As Reaction In list
                          Where Not rxn.lstGene.IsNullOrEmpty
                          Select rxn).ToArray
            Return LQuery
        Else
            Return list.ToArray
        End If
    End Function

    <ExportAPI("Read.Xml.Carmen")>
    Public Function LoadDocument(path As String) As Level3.XmlFile
        Dim File = Replace(path.LoadXml(Of Level3.XmlFile)())
        Return File
    End Function

    Private Function Replace(XmlFile As Level3.XmlFile) As Level3.XmlFile
        Dim ListOfSpecies = XmlFile.model.listOfSpecies

        For Each Reaction In XmlFile.model.listOfReactions
            Reaction.id = __getKEGGReactionId(Reaction)
            For Each Item In Reaction.listOfModifiers
                Item.species = ListOfSpecies.GetItem(Item.species).name
            Next
            For Each Item In Reaction.listOfProducts
                Item.species = ListOfSpecies.GetItem(Item.species).name
            Next
            For Each Item In Reaction.listOfReactants
                Item.species = ListOfSpecies.GetItem(Item.species).name
            Next
        Next
        For Each Item As Level3.Species In ListOfSpecies
            Item.id = Item.name
        Next

        Return XmlFile
    End Function

    ''' <summary>
    ''' 得到KEGG代谢途径的编号
    ''' </summary>
    ''' <param name="Reaction"></param>
    ''' <returns></returns>
    Private Function __getKEGGReactionId(Reaction As Level3.Reaction) As String
        Dim strData As String = Regex.Match(Reaction.Notes.Text, "REACTION_ID: rn:[0-9a-zA-Z]+", RegexOptions.Multiline).Value
        Return strData.Split(CChar(":")).Last
    End Function
End Module
