#Region "Microsoft.VisualBasic::6eef6f7e98134164e3d9962f66148350, CARMEN\PerlInvoke.vb"

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

    ' Module PerlInvoke
    ' 
    '     Properties: KGML
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: KGMLReconstruct
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' The software CARMEN was developed to support functional and comparative genome analysis. 
''' CARMEN provides the visualization of automatically obtained metabolic networks based on 
''' KEGG database information and stores the generated data in standardized SBML format. 
''' SBML is an open source XML-based format that facilitates the description of models and 
''' their exchange between various simulation and analysis tools (Hucka, 2003).
''' </summary>
<Package("CARMEN",
                  Description:="The software CARMEN was developed to support functional and comparative genome analysis. CARMEN provides the visualization of automatically obtained metabolic networks based on KEGG database information and stores the generated data in standardized SBML format. SBML is an open source XML-based format that facilitates the description of models and their exchange between various simulation and analysis tools (Hucka, 2003). 
                  <br />The reconstruction procedures of CARMEN allow users to quickly gain insights into metabolic networks of published or unpublished genome sequences and offer a user-friendly way to visualize and adapt automatically generated metabolic networks. The combination of biochemical reactions and genome annotation data facilitates the rapid detection of metabolic features. Furthermore, the generated pathways provide automated access to modeling and simulation tools that are compliant to the SBML standard. 
                  <br /><pre>CARMEN provides two main applications:
The generation of KGML-based models based on KEGG database information
The generation of SBML(template)-based models for comparative genomics</pre>",
                  Cites:="Schneider, J., et al. (2010). ""CARMEN - Comparative Analysis And in silico Reconstruction of organism-specific MEtabolic Networks."" Genet Mol Res 9(3): 1660-1672.<br />
New sequencing technologies provide ultra-fast access to novel microbial genome data. For their interpretation, an efficient bioinformatics pipeline that facilitates 
                  in silico reconstruction of metabolic networks is highly desirable. The software tool CARMEN performs in silico reconstruction of metabolic networks to interpret genome data 
                  in a functional context. CARMEN supports the visualization of automatically derived metabolic networks based on pathway information from the KEGG database or from user-defined SBML templates; 
                  this software also enables comparative genomics. The reconstructed networks are stored in standardized SBML format. We demonstrated the functionality of CARMEN with a major 
                  application example focusing on the reconstruction of glycolysis and related metabolic reactions of Xanthomonas campestris pv. campestris B100. The curation of such pathways 
                  facilitates enhanced visualization of experimental results, simulations and comparative genomics. A second application of this software was performed on a set of corynebacteria 
                  to compare and to visualize their carbohydrate metabolism. In conclusion, using CARMEN, we developed highly automated data analysis software that rapidly converts sequence data into new knowledge, 
                  replacing the time-consuming manual reconstruction of metabolic networks. This tool is particularly useful for obtaining an overview of newly sequenced genomes and their metabolic blueprints 
                  and for comparative genome analysis. The generated pathways provide automated access to modeling and simulation tools that are compliant with the SBML standard. 
                  A user-friendly web interface of CARMEN is available at http://carmen.cebitec.uni-bielefeld.de.

",
                  Publisher:="agoesman@CeBiTec.Uni-Bielefeld.DE",
                  Url:="http://carmen.cebitec.uni-bielefeld.de",
                  Category:=APICategories.ResearchTools)>
<Cite(Title:="CARMEN - Comparative Analysis And in silico Reconstruction of organism-specific MEtabolic Networks.",
      PubMed:=20799163,
      Abstract:="New sequencing technologies provide ultra-fast access to novel microbial genome data. For their interpretation, an efficient bioinformatics pipeline that facilitates in silico reconstruction of metabolic networks is highly desirable. 
The software tool CARMEN performs in silico reconstruction of metabolic networks to interpret genome data in a functional context. 
CARMEN supports the visualization of automatically derived metabolic networks based on pathway information from the KEGG database or from user-defined SBML templates; this software also enables comparative genomics. 
The reconstructed networks are stored in standardized SBML format. We demonstrated the functionality of CARMEN with a major application example focusing on the reconstruction of glycolysis and related metabolic reactions of Xanthomonas campestris pv. campestris B100. 
The curation of such pathways facilitates enhanced visualization of experimental results, simulations and comparative genomics. A second application of this software was performed on a set of corynebacteria to compare and to visualize their carbohydrate metabolism. 
<p><p>In conclusion, using CARMEN, we developed highly automated data analysis software that rapidly converts sequence data into new knowledge, replacing the time-consuming manual reconstruction of metabolic networks. 
This tool is particularly useful for obtaining an overview of newly sequenced genomes and their metabolic blueprints and for comparative genome analysis. 
The generated pathways provide automated access to modeling and simulation tools that are compliant with the SBML standard. A user-friendly web interface of CARMEN is available at http://carmen.cebitec.uni-bielefeld.de.",
      Pages:="1660-72",
      Keywords:="Animals
Computational Biology/*methods
Corynebacterium/genetics
Models, Theoretical
Systems Biology/*methods
Xanthomonas/genetics",
      Journal:="Genetics and molecular research : GMR",
      Issue:="3",
      DOI:="10.4238/vol9-3gmr901",
      ISSN:="1676-5680 (Electronic)
1676-5680 (Linking)",
      AuthorAddress:="Computational Genomics Group, Institute for Bioinformatics, Center for Biotechnology, Bielefeld University, Germany.",
      Authors:="Schneider, J.
Vorholter, F. J.
Trost, E.
Blom, J.
Musa, Y. R.
Neuweger, H.
Niehaus, K.
Schatschneider, S.
Tauch, A.
Goesmann, A.",
      URL:="http://carmen.cebitec.uni-bielefeld.de", Volume:=9, Year:=2010)>
Public Module PerlInvoke

    ''' <summary>
    ''' KGML_reconstruction.pl
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property KGML As String

    Sub New()
        Call Settings.Session.Initialize()

        Dim Perl As String = Settings.DataCache & "/CARMEN/KGML/"

        KGML = Perl & "/KGML_reconstruction.pl"

        Call My.Resources.CarmenConfig.FlushStream(Perl & "/CarmenConfig.pm")
        Call My.Resources.GetNCBIData.FlushStream(Perl & "/GetNCBIData.pm")
        Call My.Resources.GetSpeciesData.FlushStream(Perl & "/GetSpeciesData.pm")
        Call My.Resources.GetXMLData.FlushStream(Perl & "/GetXMLData.pm")
        Call My.Resources.KEGG_Information.FlushStream(Perl & "/KEGG_Information.pm")
        Call My.Resources.KGML_reconstruction.SaveTo(Perl & "/KGML_reconstruction.pl")
        Call My.Resources.LICENSE.FlushStream(Perl & "/LICENSE")
        Call My.Resources.metabolite_abbreviations.SaveTo(Perl & "/metabolite_abbreviations.txt")
        Call My.Resources.ReactionData.FlushStream(Perl & "/ReactionData.pm")
        Call My.Resources.shortcuts.SaveTo(Perl & "/shortcuts.txt")
        Call My.Resources.WriteSBML2_1_CellDesigner.FlushStream(Perl & "/WriteSBML2_1_CellDesigner.pm")
        Call My.Resources.WriteSBML2_4.FlushStream(Perl & "/WriteSBML2_4.pm")
        Call My.Resources.WriteSBML2_4_CellDesigner.FlushStream(Perl & "/WriteSBML2_4_CellDesigner.pm")
        Call My.Resources.WriteSBML3_1.FlushStream(Perl & "/WriteSBML3_1.pm")
    End Sub

    ''' <summary>
    ''' Perl-Programm to reconstruct metabolic pathways based on KGML files of the KEGG database.
    ''' </summary>
    ''' <param name="g">-g      Name of the Genbank file</param>
    ''' <param name="l">-l      File containing tab-separated EC number list</param>
    ''' <param name="o">-o      Name of the SBML-output-file</param>
    ''' <param name="n">-n      Number of the columns of the sbml file</param>
    ''' <param name="k">-k      Id's of the kegg-maps, for example 00010</param>
    ''' <param name="m">-m      include maplinks, add T for true (default = F)</param>
    ''' <param name="c">-c      Cofactor integration, add T for true (default = F)</param>
    ''' <param name="e">-e      EC number joining, add T for true (default = F)</param>
    ''' <param name="f">-f      output format, 1=SBML 2.1 (for CellDesigner), 2=SBML2.4; 3=SBML 2.4 (for CellDesigner); 4=SBML 3.1; A=all (default = 3)</param>
    ''' <returns></returns>
    <ExportAPI("Reconstruct.KGML")>
    Public Function KGMLReconstruct(<Parameter("gb", "Name of the Genbank file")> g As String,
                                    <Parameter("lst.EC", "File containing tab-separated EC number list")> l As String(),
                                    <Parameter("out.sbml", "Name of the SBML-output-file")> o As String,
                                    <Parameter("Num.Cols", "Number of the columns of the sbml file")> n As Integer,
                                    <Parameter("KEGG.Maps", "Id's of the kegg-maps, for example 00010")> k As String,
                                    <Parameter("MapLink.Include?", "include maplinks, add T for true (default = F)")> Optional m As Boolean = False,
                                    <Parameter("Cofactor.Include?", "Cofactor integration, add T for true (default = F)")> Optional c As Boolean = False,
                                    <Parameter("EC.Joining?", "EC number joining, add T for true (default = F)")> Optional e As Boolean = False,
                                    <Parameter("Format", "output format, 1=SBML 2.1 (for CellDesigner), 2=SBML2.4; 3=SBML 2.4 (for CellDesigner); 4=SBML 3.1; A=all (default = 3)")>
                                    Optional f As Integer = 3) As Boolean

    End Function
End Module
