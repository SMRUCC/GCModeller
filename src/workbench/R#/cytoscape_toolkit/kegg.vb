Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork

''' <summary>
''' The KEGG metabolism pathway network data R# scripting plugin for cytoscape software
''' </summary>
<Package("cytoscape.kegg", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
Module kegg

    ''' <summary>
    ''' Create kegg metabolism network based on the given metabolite compound data.
    ''' </summary>
    ''' <param name="reactions">The kegg ``br08201`` reaction database.</param>
    ''' <param name="compounds">Kegg compound id list</param>
    ''' <returns></returns>
    <ExportAPI("compounds.network")>
    Public Function compoundNetwork(reactions As ReactionTable(), compounds$(), Optional enzymes As Dictionary(Of String, String()) = Nothing) As NetworkGraph
        Return compounds _
            .Select(Function(cpd)
                        Return New NamedValue(Of String)(cpd, cpd)
                    End Function) _
            .DoCall(Function(list)
                        Return reactions.BuildModel(
                            compounds:=list,
                            enzymeInfo:=enzymes
                        )
                    End Function)
    End Function
End Module
