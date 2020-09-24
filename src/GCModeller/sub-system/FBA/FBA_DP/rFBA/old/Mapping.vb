#Region "Microsoft.VisualBasic::444ed859f98ca3b7445c545eb8375f20, sub-system\FBA\FBA_DP\rFBA\old\Mapping.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class Mapping
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateEnzrxnGeneMap, EffectorMapping, GetEffectors, IsEqually
    '     Class EnzymeGeneMap
    ' 
    '         Properties: CommonName, EnzymeRxn, GeneId
    ' 
    '         Sub: CopyTo
    ' 
    '     Class EffectorMap
    ' 
    '         Properties: CommonName, Effector, MetaCycId, Synonym
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.Data

Public Class Mapping
    Dim MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder

    Sub New(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder)
        Me.MetaCyc = MetaCyc
    End Sub

    Public Class EnzymeGeneMap
        <Column("Enzymatic-Reaction")> Public Property EnzymeRxn As String
        <CollectionAttribute("GeneId")> Public Property GeneId As String()
        <Column("Common-Name")> Public Property CommonName As String

        Protected Friend Sub CopyTo(Target As EnzymeGeneMap)
            Target.GeneId = GeneId
            Target.CommonName = CommonName
        End Sub
    End Class

    Public Function CreateEnzrxnGeneMap() As EnzymeGeneMap()
        Dim Proteins = MetaCyc.GetProteins, Genes = MetaCyc.GetGenes, Reactions = MetaCyc.GetReactions
        Dim EnzymaticReactions = MetaCyc.GetEnzrxns
        Dim proteinSelector = New SMRUCC.genomics.Assembly.MetaCyc.Schema.ProteinQuery(MetaCyc)
        Dim LQuery = (From Reaction In Reactions
                      Let rxnId As String = Reaction.Identifier
                      Let EnzymeList = (From enzrxn In EnzymaticReactions.AsParallel Where enzrxn.Reaction.IndexOf(rxnId) > -1 Select enzrxn.Enzyme).ToArray
                      Where Not EnzymeList.IsNullOrEmpty
                      Let _createObject = Function() As EnzymeGeneMap
                                              Dim Map As EnzymeGeneMap = New EnzymeGeneMap With {.EnzymeRxn = rxnId, .CommonName = Reaction.CommonName}
                                              Dim GeneIdList As List(Of String) = New List(Of String)

                                              For Each enzyme In EnzymeList
                                                  Dim Components = proteinSelector.GetAllComponentList(ProteinId:=enzyme)
                                                  Dim IdQuery = (From it As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object
                                                                 In Components.AsParallel
                                                                 Where it.Table = Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.proteins
                                                                 Let protein As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Protein = DirectCast(it, SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Protein)
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
        <Column("regprecise-effector")> Public Property Effector As String
        Public Property MetaCycId As String
        <Column("Common-Name")> Public Property CommonName As String
        Public Property Synonym As String
    End Class

    Public Function EffectorMapping(Regprecise As Regprecise.TranscriptionFactors) As EffectorMap()
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

    Private Shared Function IsEqually(Effector As String, Compound As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Compound) As Boolean
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

    Private Shared Function GetEffectors(Regprecise As Regprecise.TranscriptionFactors) As String()
        Dim EffectorQuery = (From item In Regprecise.genomes Select (From regulator In item.regulome.regulators Select regulator.effector.ToLower).ToArray).ToArray
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
        Effectors = (From strData As String In NewList Where Not (String.IsNullOrEmpty(strData) OrElse String.Equals(strData, "-")) Let strLine As String = strData.Trim Select strLine Distinct Order By strLine Ascending).AsList

        Return Effectors.ToArray
    End Function
End Class
