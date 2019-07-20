Imports System.Runtime.CompilerServices
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
