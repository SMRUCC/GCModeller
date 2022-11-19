#Region "Microsoft.VisualBasic::e61cc4d25402a35e0cf037c71f0f64b0, GCModeller\engine\Dynamics\Core\Kinetics\ModularRateLaws.vb"

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


    ' Code Statistics:

    '   Total Lines: 81
    '    Code Lines: 61
    ' Comment Lines: 12
    '   Blank Lines: 8
    '     File Size: 6.08 KB


    '     Module ModularRateLaws
    ' 
    '         Function: FluxRate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Kinetics

    <Cite(Abstract:="MOTIVATION: Standard rate laws are a key requisite for systematically turning metabolic networks into kinetic models. They should provide simple, general and biochemically plausible formula for reaction velocities and reaction elasticities. At the same time, they need to respect thermodynamic relations between the kinetic constants and the metabolic fluxes and concentrations. 

<p>RESULTS: We present a family of reversible rate laws for reactions with arbitrary stoichiometries and various types of regulation, including mass-action, Michaelis-Menten and uni-uni reversible Hill kinetics as special cases. 
With a thermodynamically safe parameterization of these rate laws, parameter sets obtained by model fitting, sampling or optimization are guaranteed to lead to consistent chemical equilibrium states. 
A reformulation using saturation values yields simple formulae for rates and elasticities, which can be easily adjusted to the given stationary flux distributions. 

<p>Furthermore, this formulation highlights the role of chemical potential differences as thermodynamic driving forces. We compare the modular rate laws to the thermodynamic-kinetic modelling formalism and discuss a simplified rate law in which the reaction rate directly depends on the reaction affinity. 
For automatic handling of modular rate laws, we propose a standard syntax and semantic annotations for the Systems Biology Markup Language. 
<p>
<p>AVAILABILITY: An online tool for inserting the rate laws into SBML models is freely available at www.semanticsbml.org. 
<p>
<p>SUPPLEMENTARY INFORMATION: Supplementary data are available at Bioinformatics online.",
      AuthorAddress:="Institut fur Biologie, Theoretische Biophysik, Humboldt-Universitat zu Berlin, Berlin, Germany. wolfram.liebermeister@biologie.hu-berlin.de",
      Authors:="Liebermeister, W.
Uhlendorf, J.
Klipp, E.",
      DOI:="10.1093/bioinformatics/btq141",
      ISSN:="1367-4811 (Electronic)
1367-4803 (Linking)",
      Issue:="12",
      Journal:="Bioinformatics",
      Keywords:="Algorithms
Enzymes/*metabolism
Kinetics
*Metabolic Networks and Pathways
Systems Biology/*methods
*Thermodynamics",
      Notes:="Liebermeister, Wolfram
Uhlendorf, Jannis
Klipp, Edda
eng
Research Support, Non-U.S. Gov't
England
Oxford, England
2010/04/14 06:00
Bioinformatics. 2010 Jun 15;26(12):1528-34. doi: 10.1093/bioinformatics/btq141. Epub 2010 Apr 12.",
      Pages:="1528-34",
      PubMed:=20385728,
      StartPage:=0,
      Title:="Modular rate laws for enzymatic reactions: thermodynamics, elasticities and implementation",
      URL:="http://www.ncbi.nlm.nih.gov/pubmed/20385728",
      Volume:=26,
      Year:=2010)>
    <Package("ModularRateLaws",
                  Cites:="Liebermeister, W., et al. (2010). ""Modular rate laws for enzymatic reactions: thermodynamics, elasticities And implementation."" Bioinformatics 26(12): 1528-1534.
	MOTIVATION: Standard rate laws are a key requisite for systematically turning metabolic networks into kinetic models. They should provide simple, general and biochemically plausible formulae for reaction velocities and reaction elasticities. At the same time, they need to respect thermodynamic relations between the kinetic constants and the metabolic fluxes and concentrations. RESULTS: We present a family of reversible rate laws for reactions with arbitrary stoichiometries and various types of regulation, including mass-action, Michaelis-Menten and uni-uni reversible Hill kinetics as special cases. With a thermodynamically safe parameterization of these rate laws, parameter sets obtained by model fitting, sampling or optimization are guaranteed to lead to consistent chemical equilibrium states. A reformulation using saturation values yields simple formulae for rates and elasticities, which can be easily adjusted to the given stationary flux distributions. Furthermore, this formulation highlights the role of chemical potential differences as thermodynamic driving forces. We compare the modular rate laws to the thermodynamic-kinetic modelling formalism and discuss a simplified rate law in which the reaction rate directly depends on the reaction affinity. For automatic handling of modular rate laws, we propose a standard syntax and semantic annotations for the Systems Biology Markup Language. AVAILABILITY: An online tool for inserting the rate laws into SBML models is freely available at www.semanticsbml.org. SUPPLEMENTARY INFORMATION: Supplementary data are available at Bioinformatics online.",
                  Publisher:="Wolfram Liebermeister∗, Jannis Uhlendorf and Edda Klipp", Url:="http://www.semanticsbml.org")>
    Public Module ModularRateLaws

        ''' <summary>
        ''' Modular rate laws
        ''' share the form Equation (1). The terms f [Equation (12)], T [Equation (10)],
        ''' D [Equation 11] and Dreg [Equation (13)] depend on reaction stoichiometry,
        ''' rate law, allosteric regulation and on the preferred model parameterization.
        ''' </summary>
        ''' <param name="u">Enzyme level</param>
        ''' <param name="f">Complete or partial regulation</param>
        ''' <param name="T">Stoichiometry 3 parameterizations</param>
        ''' <param name="D">5 rate laws</param>
        ''' <param name="Dreg">Specific regulation</param>
        ''' <returns></returns>
        <ExportAPI("Flux.Rate")>
        Public Function FluxRate(<Parameter("u", "")> u As KineticsFactor,
                                 <Parameter("f", "")> f As KineticsFactor,
                                 <Parameter("T", "")> T As KineticsFactor,
                                 <Parameter("D", "")> D As KineticsFactor,
                                 <Parameter("D.reg", "")> Dreg As KineticsFactor) As <FunctionReturns("Reaction rate")> Double
            Return u * f * (T / (D + Dreg))
        End Function

#Region "D:  5 rate laws"

#End Region

    End Module
End Namespace
