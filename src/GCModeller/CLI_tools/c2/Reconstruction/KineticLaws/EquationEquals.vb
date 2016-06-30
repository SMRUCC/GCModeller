Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Public Class EquationEquals : Inherits LANS.SystemsBiology.Assembly.MetaCyc.Schema.EquationEquals

    Public Class EquationEqualsMapping
        Public Property KEGG_Id As String
        Public Property KEGG_Equation As String
        Public Property Metacyc_Id As String
        Public Property Metacyc_Equation As String
        Public Property CommonNames As String()
        Public Property Comments As String
        Public Property Types As String()
    End Class

    Dim _MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder

    Sub New(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,
            KEGGCompounds As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Compound())
        Call MyBase.New(MetaCyc.GetCompounds, KEGGCompounds)
        _MetaCyc = MetaCyc
    End Sub

    Public Function ApplyAnalysis(KEGGReactions As LANS.SystemsBiology.Assembly.KEGG.DBGET.bGetObject.Reaction()) As EquationEqualsMapping()
        Dim LQuery = (From Reaction In LANS.SystemsBiology.Assembly.MetaCyc.Schema.Metabolism.Reaction.DirectCast(_MetaCyc.GetReactions).AsParallel
                      Let EqualsPair = (From item In KEGGReactions Where MyBase.Equals(item.Equation, Reaction.Equation, False) Select item).ToArray
                      Let Generation = Function() As EquationEqualsMapping
                                           Dim EquationEqualsMapping = New EquationEqualsMapping With {.Metacyc_Id = Reaction.Identifier, .Metacyc_Equation = Reaction.Equation, .Types = Reaction.Types.ToArray}
                                           If Not EqualsPair.IsNullOrEmpty Then
                                               Dim EqualsKEGGReaction = EqualsPair.First
                                               EquationEqualsMapping.KEGG_Id = EqualsKEGGReaction.Entry
                                               EquationEqualsMapping.KEGG_Equation = EqualsKEGGReaction.Equation
                                               EquationEqualsMapping.CommonNames = EqualsKEGGReaction.CommonNames
                                               EquationEqualsMapping.Comments = EqualsKEGGReaction.Comments
                                           End If
                                           Return EquationEqualsMapping
                                       End Function Select Generation()).ToArray
        Return LQuery
    End Function
End Class
