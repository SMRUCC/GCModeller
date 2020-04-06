Imports SMRUCC.genomics.Model.SBML.Level3
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

Module Module1

    Sub parseMathMLTest()
        Dim text As String = "D:\GCModeller\src\GCModeller\engine\Rscript\modelling\sabio-rk.sbml.xml".ReadAllText
        Dim formulas = MathMLParser.ParseMathML(text).ToArray

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
