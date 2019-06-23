Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2

''' <summary>
''' Create virtual cell xml file model from KEGG pathway data
''' </summary>
Public Module PathwayCompiler

    <Extension>
    Public Function CompileOrganism(genome As Dictionary(Of String, GBFF.File), keggModel As OrganismModel) As VirtualCell

    End Function
End Module
