#Region "Microsoft.VisualBasic::b8ae97faedbae8d2c8de63945397aa76, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\MetabolismBuilder.vb"

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


    ' Code Statistics:

    '   Total Lines: 175
    '    Code Lines: 114
    ' Comment Lines: 37
    '   Blank Lines: 24
    '     File Size: 10.44 KB


    '     Class MetabolismBuilder
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetReactionHandles, InsertMetabolites, InsertReaction, Invoke, IsChemicalReaction
    '                   TrimMetabolites
    ' 
    '         Sub: BuildPathwayMap, Link
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports Microsoft.VisualBasic.Extensions
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem

Namespace Builder

    Public Class MetabolismBuilder : Inherits IBuilder

        Sub New(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel)
            MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Call TrimMetabolites(MetaCyc, Model)
            Call Link(MetaCyc.GetReactions, MetaCyc.GetEnzrxns, Model)
            Call BuildPathwayMap(MetaCyc, Model)

            Return MyBase.Model
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 在Model.PreLoad方法之中，程序已经将SBML模型文件之中所列举的代谢物都加载进入模型之中了，
        ''' 在本过程中则是将MetaCyc数据库之中的RNA，蛋白质单体，蛋白质复合物对象都加载进入模型之中
        ''' </remarks>
        Private Shared Function TrimMetabolites(MetaCyc As DatabaseLoadder, Model As BacterialModel) As Boolean
            Dim Query1 = From e As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Compound In MetaCyc.GetCompounds.AsParallel Where Model.Metabolism.Metabolites.IndexOf(e.Identifier) = -1 Select e '
            Dim Query2 = From e As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Protein In MetaCyc.GetProteins.AsParallel Where Model.Metabolism.Metabolites.IndexOf(e.Identifier) = -1 Select e '
            Dim Query3 = From e As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.ProtLigandCplxe In MetaCyc.GetProtLigandCplx.AsParallel Where Model.Metabolism.Metabolites.IndexOf(e.Identifier) = -1 Select e '

            Call Model.Metabolism.Metabolites.AddRange(Query1.ToArray.AsMetabolites)
            Call Model.Metabolism.Metabolites.AddRange(Query2.ToArray.AsMetabolites)
            Call Model.Metabolism.Metabolites.AddRange(Query3.ToArray.AsMetabolites)

            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="RxnCollections">reactions.dat</param>
        ''' <param name="EnzRxn">enzrxns.dat</param>
        ''' <remarks></remarks>
        Private Shared Sub Link(RxnCollections As MetaCyc.File.DataFiles.Reactions, EnzRxn As MetaCyc.File.DataFiles.Enzrxns, Model As BacterialModel)
            Dim RemovedIndex As List(Of Long) = New List(Of Long)

            For i As Integer = 0 To RxnCollections.NumOfTokens - 1
                Dim Rxn = RxnCollections(i)
                Dim Handle = Model.Metabolism.MetabolismNetwork.IndexOf(Rxn.Identifier)

                If IsChemicalReaction(Rxn) Then
                    If Handle > -1 Then 'SBML模型中存在本反应对象
                        Model.Metabolism.MetabolismNetwork(Handle).BaseType = Rxn
                    Else                       'FBA模型中不存在本反应对象，则进行添加
                        If InsertReaction(Rxn, Model) > -1 Then
                            Printf("Insert reaction into the compiled model: %s", Rxn.Identifier)
                        End If
                    End If
                Else
                    If Handle > -1 Then  '存在本反应过程，则进行移除
                        Call RemovedIndex.Add(Handle)
                    End If
                End If
            Next

            For Each Index In RemovedIndex '执行移除操作
                '          Call Model.Metabolism.MetabolismNetwork.RemoveAll(Function(rxn As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction) RemovedIndex.IndexOf(rxn.Handle) > -1)
            Next

            For i As Integer = 0 To Model.Metabolism.MetabolismNetwork.Count - 1
                Dim Rxn = Model.Metabolism.MetabolismNetwork(i)
                Dim EnzRxns As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Enzrxn() = EnzRxn.Takes(Rxn.BaseType.EnzymaticReaction)
                Rxn.Enzymes = (From e In EnzRxns Select e.Enzyme).ToArray
                '  Rxn.EnzymaticRxn = EnzRxns

                If EnzRxns.Count > 0 Then
                    Rxn.Name = EnzRxns.First.CommonName
                Else
                    Rxn.Name = Rxn.Identifier
                End If
            Next
        End Sub

        ''' <summary>
        ''' 目标对象不是蛋白质反应，不是转运反应，不是Protein-Ligand-Binding-Reactions，不是Protein-Modification-Reactions则进行添加
        ''' </summary>
        ''' <param name="rxn"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function IsChemicalReaction(rxn As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Reaction) As Boolean
            Return (rxn.Types.IndexOf("Small-Molecule-Reactions") > -1 AndAlso rxn.Types.IndexOf("Chemical-Reactions") > -1)
        End Function

        ''' <summary>
        ''' 将MetaCyc数据库中相对应的反应对象添加进入模型之中并返回模型的代谢组网络在执行添加操作之后的节点数目
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared Function InsertReaction(Rxn As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Reaction, Model As BacterialModel) As Integer
            Dim Reaction As SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction = Rxn

            If Reaction.Reactants.IsNullOrEmpty OrElse Reaction.Products.IsNullOrEmpty Then
                printf("Broken reaction: %s", Reaction.Identifier)
                Return -1
            End If
            Call Model.Metabolism.MetabolismNetwork.Add(Reaction)

            Return InsertMetabolites(Reaction, Model)
        End Function

        Private Shared Function InsertMetabolites(Reaction As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction, Model As BacterialModel) As Integer
            Dim AddAction As System.Func(Of String, Integer) = Function(Id As String) As Integer
                                                                   If Model.Metabolism.Metabolites.IndexOf(UniqueId:=Id) = -1 Then
                                                                       Model.Metabolism.Metabolites.Add(Id.AsMetabolite)
                                                                       printf("Adding new metabolite in to the compiled model: %s", Id)
                                                                   End If
                                                                   Return 1
                                                               End Function
            Dim LQuery = From Id As String In Reaction.BaseType.Substrates Select AddAction(Id) '
            Return LQuery.ToArray.Count
        End Function

        ''' <summary>
        ''' 依照MetaCyc数据库中的代谢途径对象的定义将相对应的反应对象加入到相对应的代谢途径对象中
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared Sub BuildPathwayMap(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder, Model As BacterialModel)
            Call printf("Start to build up the metaboloism network mapping...")

            Dim LQuery = From e As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Pathway
                         In MetaCyc.GetPathways.AsParallel
                         Let Pwy = GCML_Documents.XmlElements.Metabolism.Pathway.CastTo(e)
                         Select Pwy Order By Pwy.Identifier Ascending  '
            Model.Metabolism.Pathways = LQuery.ToArray
            For i As Integer = 0 To Model.Metabolism.Pathways.Count - 1
                '        Model.Metabolism.Pathways(i).MetabolismNetwork = (From int In GetReactionHandles(Model.Metabolism.Pathways(i), MetaCyc, Model) Select int Distinct Order By int Ascending).ToArray   '递归的添加所有的反应对象
            Next
            Call printf("function()::END_OF_BUILD_PATHWAY_MAP()")
        End Sub

        ''' <summary>
        ''' 递归的获取某一个代谢途径对象中的所有的生化反应对象的句柄值
        ''' </summary>
        ''' <param name="Pathway"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetReactionHandles(Pathway As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Pathway,
                                                   MetaCyc As MetaCyc.File.FileSystem.DatabaseLoadder,
                                                   Model As BacterialModel) As Integer()
            Dim HandleList As New List(Of Integer)
            Dim Reactions = MetaCyc.GetReactions

            For Each Id As String In Pathway.BaseType.ReactionList
                Dim Handle = Model.Metabolism.MetabolismNetwork.IndexOf(Id)
                If Handle = -1 Then   '目标反应对象不存在于模型之中，则从MetaCyc数据库之中添加相应的反应对象
                    Dim PwyHandle As Integer = MetacycObjectFindingMethods.IndexOf(Of Slots.Pathway, GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Pathway)(Model.Metabolism.Pathways, Id)
                    If PwyHandle > -1 Then '当前所指向的对象为一个子代谢途径对象，则对其进行递归操作：找出其中所有的反应列表，将其链接至本反应途径对象之上
                        HandleList.AddRange(GetReactionHandles(Model.Metabolism.Pathways(PwyHandle), MetaCyc, Model))  '递归操作
                        Continue For   '已经将所有的反应都加入到当前的途径对象中，移至下一个反应对象
                    Else
                        Handle = InsertReaction(Reactions.Item(Id), Model)  '更新了一个新的对象句柄值
                        If Handle = -1 Then Continue For
                    End If
                End If

                Call HandleList.Add(Handle)
            Next
            Return HandleList.ToArray
        End Function
    End Class
End Namespace
