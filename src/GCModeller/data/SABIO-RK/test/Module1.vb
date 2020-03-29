Imports SMRUCC.genomics.Model.SBML.Level3
Imports SMRUCC.genomics.Data.SABIORK.SBML

Module Module1

    Sub Main()
        Dim sbml = XmlFile(Of SBMLReaction).LoadDocument("E:\GCModeller\src\GCModeller\engine\Rscript\modelling\sabio-rk.sbml.xml")

        Pause()
    End Sub

End Module
