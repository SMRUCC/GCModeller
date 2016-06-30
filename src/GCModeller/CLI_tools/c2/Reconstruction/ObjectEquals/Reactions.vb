Imports c2.Reconstruction.Operation

Namespace Reconstruction.ObjectEquals

    Public Class Reactions : Inherits c2.Reconstruction.ObjectEquals.EqualsOperation

        Dim SchemaCache As c2.Reconstruction.ObjectEquals.EqualsOperation.ObjectSchema(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Reaction)
        Dim rctCompounds As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Compounds

        Sub New(Session As OperationSession)
            Call MyBase.New(Session)
            SchemaCache = New EqualsOperation.ObjectSchema(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Reaction)(FieldList:=New String() {"LEFT", "RIGHT"})
            rctCompounds = Reconstructed.GetCompounds
        End Sub

        Public Overrides Function Initialize() As Integer
            Dim EqualsList As List(Of KeyValuePair(Of String, String())) = New List(Of KeyValuePair(Of String, String()))
            Dim rctReactions = MyBase.Session.ReconstructedMetaCyc.GetReactions

            For Each Reaction In MyBase.Subject.GetReactions
                If AllCompoundsExists(Reaction, rctCompounds) Then
                    Dim LQuery = From rxn As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Reaction
                                 In rctReactions.Select(Reaction, SchemaCache.ItemProperties, SchemaCache.FieldAttributes)
                                 Select rxn.Identifier '
                    Dim rctReactionList As String() = LQuery.ToArray
                    If Not rctReactionList.IsNullOrEmpty Then
                        Call EqualsList.Add(New KeyValuePair(Of String, String())(Reaction.Identifier, rctReactionList))
                    End If
                End If
            Next

            MyBase.EqualsList = EqualsList.ToArray
            Return MyBase.EqualsList.Count
        End Function

        ''' <summary>
        ''' 查看目标Reaction是否已经存在于Reconstructed数据库之中
        ''' </summary>
        ''' <param name="sbjReaction">Subject Reaction的UniqueId</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Exists(sbjReaction As String) As Boolean
            Dim LQuery = From r In MyBase.EqualsList Where String.Equals(sbjReaction, r.Key) Select 1 '

            If LQuery.ToArray.Count > 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Reaction">Subject数据库之中的一个Reaction对象</param>
        ''' <param name="Table">Reconstructed数据库中的Compounds表</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' </remarks>
        Friend Shared Function AllCompoundsExists(Reaction As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Reaction, Table As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Compounds) As Boolean
            For Each Reactant As String In Reaction.Substrates
                If Table.IndexOf(Reactant) = -1 Then
                    Return False '只要任何一个底物对象没有寻找到，则不能进行重建
                End If
            Next

            Return True
        End Function
    End Class
End Namespace