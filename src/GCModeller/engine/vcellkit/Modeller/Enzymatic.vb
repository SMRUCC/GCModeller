Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data
Imports sbXML = SMRUCC.genomics.Model.SBML.Level3.XmlFile(Of SMRUCC.genomics.Data.SABIORK.SBML.SBMLReaction)

''' <summary>
''' enzymatic reaction network modeller
''' </summary>
<Package("enzymatic")>
Module Enzymatic

    <ExportAPI("query_reaction")>
    Public Function QueryReaction(reaction As Rhea.Reaction, Optional cache As String = "./.cache/") As Object
        Dim list As New Dictionary(Of String, sbXML)

        For Each id As String In reaction.enzyme.SafeQuery
            list.Add(id, SABIORK.WebRequest.QueryByECNumber(id, cache))
        Next

        Return list
    End Function
End Module
