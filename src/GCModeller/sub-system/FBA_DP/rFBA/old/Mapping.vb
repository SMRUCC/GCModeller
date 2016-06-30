Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic

Public Class Mapping
    Dim MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder

    Sub New(MetaCyc As LANS.SystemsBiology.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder)
        Me.MetaCyc = MetaCyc
    End Sub

    Public Class EnzymeGeneMap
        <Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection.Column("Enzymatic-Reaction")> Public Property EnzymeRxn As String
        <Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection.CollectionAttribute("GeneId")> Public Property GeneId As String()
        <Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection.Column("Common-Name")> Public Property CommonName As String

        Protected Friend Sub CopyTo(Target As EnzymeGeneMap)
            Target.GeneId = GeneId
            Target.CommonName = CommonName
        End Sub
    End Class

    Public Function CreateEnzrxnGeneMap() As EnzymeGeneMap()
        Dim Proteins = MetaCyc.GetProteins, Genes = MetaCyc.GetGenes, Reactions = MetaCyc.GetReactions
        Dim EnzymaticReactions = MetaCyc.GetEnzrxns
        Dim proteinSelector = New LANS.SystemsBiology.Assembly.MetaCyc.Schema.ProteinQuery(MetaCyc)
        Dim LQuery = (From Reaction In Reactions
                      Let rxnId As String = Reaction.Identifier
                      Let EnzymeList = (From enzrxn In EnzymaticReactions.AsParallel Where enzrxn.Reaction.IndexOf(rxnId) > -1 Select enzrxn.Enzyme).ToArray
                      Where Not EnzymeList.IsNullOrEmpty
                      Let _createObject = Function() As EnzymeGeneMap
                                              Dim Map As EnzymeGeneMap = New EnzymeGeneMap With {.EnzymeRxn = rxnId, .CommonName = Reaction.CommonName}
                                              Dim GeneIdList As List(Of String) = New List(Of String)

                                              For Each enzyme In EnzymeList
                                                  Dim Components = proteinSelector.GetAllComponentList(ProteinId:=enzyme)
                                                  Dim IdQuery = (From it As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Object
                                                                 In Components.AsParallel
                                                                 Where it.Table = Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.proteins
                                                                 Let protein As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein = DirectCast(it, LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein)
                                                                 Let GetGeneId = Function() As String
                                                                                     If protein.Components.IsNullOrEmpty Then
                                                                                         Return protein.Gene
                                                                                     Else
                                                                                         Return ""
                                                                                     End If
                                                                                 End Function Select GetGeneId()).ToArray
                                                  Dim GeneList = Genes.Takes(IdQuery)
                                                  If Not GeneList.IsNullOrEmpty Then
                                                      Call GeneIdList.AddRange((From Gene In GeneList Select Gene.Accession1).ToArray)
                                                  End If
                                              Next

                                              Map.GeneId = (From id As String In GeneIdList Select id Distinct Order By id Ascending).ToArray

                                              Return Map
                                          End Function Select _createObject()).ToArray
        Return LQuery
    End Function

    ''' <summary>
    ''' Regprecise Effector与MetaCyc Compounds Mapping
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EffectorMap
        <Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection.Column("regprecise-effector")> Public Property Effector As String
        Public Property MetaCycId As String
        <Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection.Column("Common-Name")> Public Property CommonName As String
        Public Property Synonym As String
    End Class

    Public Function EffectorMapping(Regprecise As LANS.SystemsBiology.DatabaseServices.Regprecise.TranscriptionFactors) As EffectorMap()
        Dim Effectors = GetEffectors(Regprecise)
        Dim MapDataChunk As EffectorMap() = New EffectorMap(Effectors.Count - 1) {}
        Dim Compounds = MetaCyc.GetCompounds

        For i As Integer = 0 To Effectors.Count - 1
            Dim Effector As String = Effectors(i)
            Dim LQuery = (From Compound In Compounds.AsParallel Where IsEqually(Effector, Compound) Select Compound).ToArray
            Dim Map As EffectorMap = New EffectorMap With {.Effector = Effector}

            If Not LQuery.IsNullOrEmpty Then
                Dim Compound = LQuery.First

                Map.MetaCycId = Compound.Identifier
                Map.CommonName = Compound.CommonName

                If Compound.Synonyms.IsNullOrEmpty Then
                    Map.Synonym = ""
                ElseIf Compound.Synonyms.Count = 1 Then
                    Map.Synonym = Compound.Synonyms.First
                Else
                    Dim sBuilder As StringBuilder = New StringBuilder(1024)

                    For Each strData As String In Compound.Synonyms
                        Call sBuilder.AppendFormat("[{0}]; ", strData)
                    Next

                    Map.Synonym = sBuilder.ToString
                End If
            End If

            MapDataChunk(i) = Map
        Next

        Return MapDataChunk
    End Function

    Private Shared Function IsEqually(Effector As String, Compound As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Compound) As Boolean
        If String.Equals(Effector, Compound.CommonName.ToLower) Then
            Return True
        ElseIf String.Equals(Effector, Compound.AbbrevName.ToLower) Then
            Return True
        Else
            For Each strName As String In Compound.Synonyms
                If String.Equals(strName.ToLower, Effector) Then
                    Return True
                End If
            Next
        End If

        Return False
    End Function

    Private Shared Function GetEffectors(Regprecise As LANS.SystemsBiology.DatabaseServices.Regprecise.TranscriptionFactors) As String()
        Dim EffectorQuery = (From item In Regprecise.BacteriaGenomes Select (From regulator In item.Regulons.Regulators Select regulator.Effector.ToLower).ToArray).ToArray
        Dim Effectors As List(Of String) = New List(Of String)

        For Each List In EffectorQuery
            Call Effectors.AddRange(List)
        Next
        Dim NewList As List(Of String) = New List(Of String)
        For Each Effector In Effectors
            Dim Tokens = Strings.Split(Effector, "; ")
            For Each Token As String In Tokens
                Dim strTemp As String = Regex.Match(Token, "ion, \([^)]+\)").Value

                If String.IsNullOrEmpty(strTemp) Then
                    Call NewList.Add(Token)
                Else
                    Dim T = Strings.Split(Token, ", ")
                    Call NewList.Add(T(0))

                    T(1) = Mid(T(1), 2)
                    T(1) = Mid(T(1), 1, Len(T(1)) - 1)
                    Call NewList.Add(T(1))
                End If
            Next
        Next
        Effectors = (From strData As String In NewList Where Not (String.IsNullOrEmpty(strData) OrElse String.Equals(strData, "-")) Let strLine As String = strData.Trim Select strLine Distinct Order By strLine Ascending).ToList

        Return Effectors.ToArray
    End Function
End Class
