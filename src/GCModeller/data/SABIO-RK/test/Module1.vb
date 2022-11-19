#Region "Microsoft.VisualBasic::d05ea722538cbc251b45315b6e0128da, GCModeller\data\SABIO-RK\test\Module1.vb"

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
    '    Code Lines: 31
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 1.52 KB


    ' Module Module1
    ' 
    '     Sub: Main, parseMathMLTest, xmlReadertest
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Model.SBML.Level3
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

Module Module1

    Sub parseMathMLTest()
        Dim text As String = "D:\GCModeller\src\GCModeller\engine\Rscript\modelling\sabio-rk.sbml.xml".ReadAllText
        Dim formulas = MathMLParser.ParseMathML(text).ToArray
        Dim doc As SbmlDocument = SbmlDocument.LoadDocument("D:\GCModeller\src\GCModeller\engine\Rscript\modelling\sabio-rk.sbml.xml")
        Dim index As New SBMLInternalIndexer(doc)

        Pause()
    End Sub

    Sub Main()
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
                .RDF = New AnnotationInfo With {.description = New SbmlAnnotationData With {
                .about = "12344",
                .isDescribedBy = {New [is] With {.Bag = New MIME.application.rdf_xml.Array With {.list = {New li With {.resource = "abccc"}}}}}}}
        }
        }}}}}

        Call newML.GetXml.SaveTo("X:\11111.XML")

        Pause()
    End Sub

End Module
