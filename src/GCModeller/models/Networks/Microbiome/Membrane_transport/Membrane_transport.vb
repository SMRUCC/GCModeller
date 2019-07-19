Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data

Public Class Enzyme

    Public Property KO As String
    Public Property EC As ECNumber
    Public Property name As String

    Sub New(KO$, geneName$, EC$)
        Me.KO = KO
        Me.name = geneName
        Me.EC = ECNumber.ValueParser(EC)
    End Sub

    ''' <summary>
    ''' Select enzymes
    ''' </summary>
    ''' <param name="repo"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Selects(repo As ReactionRepository) As IReadOnlyDictionary(Of String, Reaction)
        Return repo _
            .GetWhere(Function(r)
                          Return r.Enzyme _
                              .Any(Function(id)
                                       Return Me.EC.Contains(id) OrElse ECNumber.ValueParser(id).Contains(EC)
                                   End Function)
                      End Function)
    End Function
End Class

Public Module Membrane_transport

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="metagenome"></param>
    ''' <param name="repo"></param>
    ''' <param name="enzymes">
    ''' ``{KO => enzyme}``
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildTransferNetwork(metagenome As IEnumerable(Of TaxonomyRef), repo As ReactionRepository, enzymes As Dictionary(Of String, Enzyme())) As NetworkGraph
        Dim g As New NetworkGraph

        ' 遍历所有的基因组
        For Each genome As TaxonomyRef In metagenome
            ' 得到相交的跨膜转运蛋白
            Dim transporters = enzymes.Takes(genome.KOTerms).IteratesALL.ToArray

            For Each enzyme As Enzyme In transporters
                Dim reactions = enzyme.Selects(repo).Values.ToArray


            Next
        Next

        Return g
    End Function
End Module
