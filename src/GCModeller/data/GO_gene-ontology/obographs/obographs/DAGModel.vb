Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Module DAGModel

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="go"></param>
    ''' <param name="terms">
    ''' A go term <see cref="Term.id"/> collection.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateGraph(go As GO_OBO, terms As IEnumerable(Of String)) As NetworkGraph

    End Function
End Module
