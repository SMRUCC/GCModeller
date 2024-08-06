﻿#Region "Microsoft.VisualBasic::eccff19573e008c32f36873c4d36d361, data\SABIO-RK\test\Module1.vb"

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

    '   Total Lines: 39
    '    Code Lines: 31 (79.49%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (20.51%)
    '     File Size: 1.56 KB


    ' Module Module1
    ' 
    '     Sub: Main, parseMathMLTest, xmlReadertest
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Model.SBML.Level3
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports Microsoft.VisualBasic.MIME.application.rdf_xml
Imports SMRUCC.genomics.Data.Rhea

Module Module1

    Sub parseMathMLTest()
        Dim text As String = "D:\GCModeller\src\GCModeller\engine\Rscript\modelling\sabio-rk.sbml.xml".ReadAllText
        Dim formulas = MathMLParser.ParseMathML(text).ToArray
        Dim doc As SbmlDocument = SbmlDocument.LoadDocument("D:\GCModeller\src\GCModeller\engine\Rscript\modelling\sabio-rk.sbml.xml")
        Dim index As New SBMLInternalIndexer(doc)

        Pause()
    End Sub

    Sub Main()
        Call load_rheaDataabse()
        Call rhea_rdf_test()
        Call parseMathMLTest()
    End Sub

    Sub xmlReadertest()
        Dim sbml = XmlFile(Of SBMLReaction).LoadDocument("E:\GCModeller\src\GCModeller\engine\Rscript\modelling\sabio-rk.sbml.xml")
        Dim newML As New XmlFile(Of SBMLReaction) With {
            .model = New Model(Of SBMLReaction) With {
            .listOfReactions = {New SBMLReaction With {
            .kineticLaw = New kineticLaw With {
            .annotation = New kineticLawAnnotation With {
                .sabiork = New sabiorkAnnotation With {.kineticLawID = 5},
                .RDF = New AnnotationInfo With {.description = {New SbmlAnnotationData With {
                .about = "12344",
                .isDescribedBy = {New [is] With {.Bag = New MIME.application.rdf_xml.Array With {.list = {New li With {.resource = "abccc"}}}}}}}
        }
        }}}}}}

        Call newML.GetXml.SaveTo("Z:\11111.XML")

        Pause()
    End Sub

    Sub rhea_rdf_test()
        Dim test As New RheaDescription With {.subClassOf = New Resource With {.resource = "aaaaaa"}, .type = New RDFType With {.resource = "xxxxx"}, .about = "test"}
        Dim doc As New RheaRDF() With {.description = {
            test
        }}

        Call doc.GetXml.SaveTo("Z:/dddddd.xml")
        Call Pause()
    End Sub

    Sub load_rheaDataabse()
        Dim doc As RheaRDF = RheaRDF.Load("J:\ossfs\rhea.rdf")
        Dim reactions = doc.GetReactions.ToArray

        Pause()
    End Sub
End Module
