#Region "Microsoft.VisualBasic::5e9a64be6c68380fd3989bce58324c48, engine\IO\GCTabular\Mapping.vb"

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
    ' 
    '     Function: CreateEnzrxnGeneMap, EffectorMapping, (+3 Overloads) GetEffectors, GetSBMLQueryKeyword, InternalGetEffectors
    '               (+3 Overloads) IsEqually
    ' 
    '     Sub: (+2 Overloads) Dispose
    '     Class EnzymeGeneMap
    ' 
    '         Properties: CommonName, EnzymeRxn, GeneId
    ' 
    '         Function: ToString
    ' 
    '         Sub: CopyTo
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise

''' <summary>
''' MetaCyc数据库专用的CompoundMapping操作对象
''' </summary>
''' <remarks></remarks>
Public Class Mapping : Implements System.IDisposable

    Dim MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder
    Dim SBMLMetabolite As FileStream.Metabolite()

    Sub New(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, SBMLMetabolites As FileStream.Metabolite())
        Me.MetaCyc = MetaCyc
        Me.SBMLMetabolite = If(SBMLMetabolites Is Nothing, New FileStream.Metabolite() {}, SBMLMetabolites)
    End Sub

    Public Class EnzymeGeneMap
        <Column("Enzymatic-Reaction")> Public Property EnzymeRxn As String
        <CollectionAttribute("GeneId")> Public Property GeneId As String()
        <Column("Common-Name")> Public Property CommonName As String

        Public Overrides Function ToString() As String
            Return CommonName
        End Function

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
                                                                 Where it.Table = SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.proteins
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
    ''' 主要的算法思路就是将名称与MetaCyc Compound中的通用名称和同义名进行匹配
    ''' </summary>
    ''' <param name="Regprecise"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EffectorMapping(Regprecise As TranscriptionFactors) As List(Of MetaCyc.Schema.EffectorMap)
        Dim Effectors = GetEffectors(Regprecise)
        Dim Compounds = MetaCyc.GetCompounds

        For i As Integer = 0 To Effectors.Count - 1
            Dim Effector = Effectors(i)
            Dim LQuery '= (From cpd As Compounds In Compounds.AsParallel Where IsEqually(Effector, cpd) Select cpd).ToArray
            Dim CommonNames As New List(Of String)(Effector.EffectorAlias)

            If Not LQuery.IsNullOrEmpty Then '在MetaCyc数据库之中查询到了相对应的记录数据
                Dim Compound = LQuery.First

                'Effector.MetaCycId = Compound.Identifier.ToUpper
                'Call CommonNames.Add(Compound.CommonName)

                'If Not Compound.Synonyms.IsNullOrEmpty Then
                '    Call CommonNames.AddRange(Compound.Synonyms)
                'End If
            Else '没有在MetaCyc数据库之中查询到相对应的记录数据，则尝试在SBML代谢物列表中进行查询
                Dim SBMLQuery = (From Metabolite In Me.SBMLMetabolite.AsParallel Where IsEqually(Effector, Metabolite) Select Metabolite).ToArray

                If Not SBMLQuery.IsNullOrEmpty Then
                    Dim Metabolite = SBMLQuery.First

                    Effector.MetaCycId = Metabolite.Identifier.ToUpper
                    Call CommonNames.AddRange(Metabolite.CommonNames)
                End If
            End If

            Effector.EffectorAlias = CommonNames.ToArray
        Next

        Call Effectors.SaveTo(My.Computer.FileSystem.SpecialDirectories.Temp & "/____temp___EffectorMapping.csv", False)

        Return Effectors
    End Function

    '''' <summary>
    '''' 主要的算法思路就是将名称与MetaCyc Compound中的通用名称和同义名进行匹配
    '''' </summary>
    '''' <param name="Regprecise"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Shared Function EffectorMapping(Regprecise As IEnumerable(Of TranscriptRegulation), Mapping As IEnumerable(Of ICompoundObject)) As List(Of MetaCyc.Schema.EffectorMap)
    '    Dim Effectors = GetEffectors(Regprecise.ToArray)
    '    Effectors = InternalEffectorMapping(Effectors, Mapping)
    '    Call Effectors.SaveTo(My.Computer.FileSystem.SpecialDirectories.Temp & "/____temp___EffectorMapping.csv", False)

    '    Return Effectors
    'End Function

    'Private Shared Function InternalEffectorMapping(Effectors As List(Of MetaCyc.Schema.EffectorMap), Mapping As IEnumerable(Of ICompoundObject)) As List(Of MetaCyc.Schema.EffectorMap)
    '    For i As Integer = 0 To Effectors.Count - 1
    '        Dim Effector = Effectors(i)
    '        Dim LQuery = (From Compound In Mapping.AsParallel Where IsEqually(Effector, Compound) Select Compound).ToArray
    '        Dim CommonNames As New List(Of String)(Effector.EffectorAlias)

    '        If Not LQuery.IsNullOrEmpty Then '在MetaCyc数据库之中查询到了相对应的记录数据
    '            Dim Compound = LQuery.First

    '            Effector.MetaCycId = Compound.Key.ToUpper
    '            Call CommonNames.AddRange(Compound.CommonNames)
    '        End If

    '        Effector.EffectorAlias = CommonNames.ToArray
    '    Next

    '    Call Effectors.SaveTo(My.Computer.FileSystem.SpecialDirectories.Temp & "/____temp___EffectorMapping.csv", False)

    '    Return Effectors
    'End Function

    '''' <summary>
    '''' 主要的算法思路就是将名称与MetaCyc Compound中的通用名称和同义名进行匹配
    '''' </summary>
    '''' <param name="Regprecise"></param>
    '''' <returns></returns>
    '''' <remarks></remarks>
    'Public Shared Function EffectorMapping(Regprecise As IEnumerable(Of RegpreciseMPBBH), Mapping As IEnumerable(Of ICompoundObject)) As List(Of MetaCyc.Schema.EffectorMap)
    '    Dim Effectors = GetEffectors(bh:=Regprecise.ToArray)
    '    Effectors = InternalEffectorMapping(Effectors, Mapping)
    '    Call Effectors.SaveTo(My.Computer.FileSystem.SpecialDirectories.Temp & "/____temp___EffectorMapping.csv", False)

    '    Return Effectors
    'End Function

    'Private Shared Function IsEqually(Effector As MetaCyc.Schema.EffectorMap, Compound As ICompoundObject) As Boolean
    '    For i As Integer = 0 To Effector.EffectorAlias.Count - 1
    '        If IsEquals(Effector.EffectorAlias(i), Compound) Then
    '            Return True
    '        End If
    '    Next
    '    Return False
    'End Function

    'Private Shared Function IsEqually(Effector As MetaCyc.Schema.EffectorMap, Compound As MetaCyc.File.DataFiles.Slots.Compound) As Boolean
    '    For i As Integer = 0 To Effector.EffectorAlias.Count - 1
    '        If IsEqually(Effector.EffectorAlias(i), Compound) Then
    '            Return True
    '        End If
    '    Next
    '    Return False
    'End Function

    'Private Shared Function IsEquals(Effector As String, Compound As ICompoundObject) As Boolean
    '    If String.Equals(Effector.ToUpper, Compound.Key) Then
    '        Return True
    '    Else
    '        For Each strName As String In Compound.CommonNames
    '            If String.Equals(strName.ToLower, Effector) Then
    '                Return True
    '            End If
    '        Next
    '    End If

    '    Return False
    'End Function

    Private Shared Function IsEqually(Effector As String, Compound As MetaCyc.File.DataFiles.Slots.Compound) As Boolean
        If String.Equals(Effector, Compound.CommonName.ToLower) Then
            Return True
        ElseIf String.Equals(Effector, Compound.AbbrevName.ToLower) Then
            Return True
        ElseIf String.Equals(Effector.ToUpper, Compound.Identifier) Then
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

    Private Shared Function IsEqually(Effector As MetaCyc.Schema.EffectorMap, Metabolite As FileStream.Metabolite) As Boolean
        For Each AliasId As String In Effector.EffectorAlias
            Dim QueryId As String = GetSBMLQueryKeyword(AliasId)
            If IsEqually(QueryId, Metabolite) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Shared Function IsEqually(Effector As String, Metabolite As FileStream.Metabolite) As Boolean
        If String.Equals(Effector, Metabolite.Identifier, StringComparison.OrdinalIgnoreCase) Then
            Return True
        End If

        Return False
    End Function

    Private Shared Function GetSBMLQueryKeyword(Effector As String) As String
        Dim strData As String = Effector

        If Regex.Match(Effector, "[a-z0-9]+[-]trna", RegexOptions.IgnoreCase).Success Then
            strData = (Effector & "s").ToUpper
        End If

        Return strData
    End Function

    Public Shared Function GetEffectors(Regprecise As TranscriptRegulation()) As List(Of MetaCyc.Schema.EffectorMap)
        Dim EffectorIdList As List(Of String) = New List(Of String)
        For Each item In Regprecise
            If item.Effectors.IsNullOrEmpty Then
                Continue For
            End If
            Call EffectorIdList.AddRange(item.Effectors)
        Next

        Return InternalGetEffectors(EffectorIdList)
    End Function

    Public Shared Function GetEffectors(bh As IEnumerable(Of RegpreciseMPBBH)) As List(Of MetaCyc.Schema.EffectorMap)
        Dim EffectorIdList As List(Of String) = New List(Of String)
        For Each item In bh
            If item.Effectors.IsNullOrEmpty Then
                Continue For
            End If
            Call EffectorIdList.AddRange(item.Effectors)
        Next

        Return InternalGetEffectors(EffectorIdList)
    End Function

    Public Shared Function InternalGetEffectors(EffectorIdList As List(Of String)) As List(Of MetaCyc.Schema.EffectorMap)
        Dim TempChunk As String() = (From strId As String In EffectorIdList Where Not (String.IsNullOrEmpty(strId) OrElse String.Equals(strId, "-")) Select strId Distinct Order By strId Ascending).ToArray
        Call EffectorIdList.Clear()
        For Each strItem As String In TempChunk
            Call EffectorIdList.AddRange(Strings.Split(strItem, "; "))
        Next

        EffectorIdList = LinqAPI.MakeList(Of String) <= From strId As String
                                                        In EffectorIdList
                                                        Where Not (String.IsNullOrEmpty(strId) OrElse String.Equals(strId, "-"))
                                                        Select strId
                                                        Distinct
                                                        Order By strId Ascending

        Dim Effectors = LinqAPI.MakeList(Of MetaCyc.Schema.EffectorMap) <=
            From strId As String
            In EffectorIdList
            Select New MetaCyc.Schema.EffectorMap With {
                .Effector = strId
            }

        For Each Effector In Effectors
            Dim Tokens = Strings.Split(Effector.Effector, ", ")
            Dim AliasList As List(Of String) = New List(Of String)

            For Each Token As String In Tokens
                Dim strTemp As String = Regex.Match(Token, "^\([^)]+\)$", RegexOptions.Multiline).Value

                If String.IsNullOrEmpty(strTemp) Then
                    strTemp = Regex.Match(Token, "^.+\([^)]+\)$", RegexOptions.Multiline).Value

                    If String.IsNullOrEmpty(strTemp) Then
                        Call AliasList.Add(Token)
                    Else
                        Dim T1, T2 As String, p As Integer = InStr(strTemp, "(")
                        T2 = Mid(strTemp, p + 1)
                        T2 = Mid(T2, 1, Len(T2) - 1)
                        Call AliasList.Add(T2)

                        T1 = Mid(strTemp, 1, p - 1).Trim
                        If Not String.Equals(T1, "ion", StringComparison.OrdinalIgnoreCase) Then
                            Call AliasList.Add(T1)
                        End If
                    End If
                Else
                    strTemp = Mid(strTemp, 2)
                    strTemp = Mid(strTemp, 1, Len(strTemp) - 1)
                    Call AliasList.Add(strTemp)
                End If
            Next
            Effector.EffectorAlias = (From strId As String In AliasList
                                      Where Not (String.IsNullOrEmpty(strId) OrElse String.Equals(strId, "-"))
                                      Let strLine As String = strId.Trim
                                      Select strLine
                                      Distinct
                                      Order By strLine Ascending).ToArray
        Next

        Return Effectors
    End Function

    Public Shared Function GetEffectors(Regprecise As TranscriptionFactors) As List(Of MetaCyc.Schema.EffectorMap)
        Dim EffectorIdList = LinqAPI.MakeList(Of String) <=
            From item As Regprecise.BacteriaRegulome
            In Regprecise.genomes
            Select From regulator
                   In item.regulome.regulators
                   Where Not String.IsNullOrEmpty(regulator.effector)
                   Select regulator.effector.ToLower.Trim

        Dim TempChunk As String() = (From strId As String In EffectorIdList Where Not (String.IsNullOrEmpty(strId) OrElse String.Equals(strId, "-")) Select strId Distinct Order By strId Ascending).ToArray
        Call EffectorIdList.Clear()
        For Each strItem As String In TempChunk
            Call EffectorIdList.AddRange(Strings.Split(strItem, "; "))
        Next
        EffectorIdList = LinqAPI.MakeList(Of String) <= From strId As String
                                                        In EffectorIdList
                                                        Where Not (String.IsNullOrEmpty(strId) OrElse String.Equals(strId, "-"))
                                                        Select strId
                                                        Distinct
                                                        Order By strId Ascending

        Dim Effectors = LinqAPI.MakeList(Of MetaCyc.Schema.EffectorMap) <=
            From strId As String
            In EffectorIdList
            Select New MetaCyc.Schema.EffectorMap With {
                .Effector = strId
            }

        For Each Effector In Effectors
            Dim Tokens = Strings.Split(Effector.Effector, ", ")
            Dim AliasList As List(Of String) = New List(Of String)

            For Each Token As String In Tokens
                Dim strTemp As String = Regex.Match(Token, "^\([^)]+\)$", RegexOptions.Multiline).Value

                If String.IsNullOrEmpty(strTemp) Then
                    strTemp = Regex.Match(Token, "^.+\([^)]+\)$", RegexOptions.Multiline).Value

                    If String.IsNullOrEmpty(strTemp) Then
                        Call AliasList.Add(Token)
                    Else
                        Dim T1, T2 As String, p As Integer = InStr(strTemp, "(")
                        T2 = Mid(strTemp, p + 1)
                        T2 = Mid(T2, 1, Len(T2) - 1)
                        Call AliasList.Add(T2)

                        T1 = Mid(strTemp, 1, p - 1).Trim
                        If Not String.Equals(T1, "ion", StringComparison.OrdinalIgnoreCase) Then
                            Call AliasList.Add(T1)
                        End If
                    End If
                Else
                    strTemp = Mid(strTemp, 2)
                    strTemp = Mid(strTemp, 1, Len(strTemp) - 1)
                    Call AliasList.Add(strTemp)
                End If
            Next
            Effector.EffectorAlias = (From strId As String In AliasList
                                      Where Not (String.IsNullOrEmpty(strId) OrElse String.Equals(strId, "-"))
                                      Let strLine As String = strId.Trim
                                      Select strLine
                                      Distinct
                                      Order By strLine Ascending).ToArray
        Next

        Return Effectors
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 检测冗余的调用

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO:  释放托管状态(托管对象)。
            End If

            ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
            ' TODO:  将大型字段设置为 null。
        End If
        Me.disposedValue = True
    End Sub

    ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
    'Protected Overrides Sub Finalize()
    '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Visual Basic 添加此代码是为了正确实现可处置模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
