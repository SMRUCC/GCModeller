Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

''' <summary>
''' The KEGG metabolism pathway network data R# scripting plugin for cytoscape software
''' </summary>
<Package("cytoscape.kegg", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module kegg

    <ExportAPI("compounds.network")>
    Public Function compoundNetwork(reactions As ReactionTable(), compounds$()) As NetworkGraph
        Return reactions.BuildModel(compounds.Select(Function(cpd) New NamedValue(Of String)(cpd, cpd)))
    End Function
End Module
