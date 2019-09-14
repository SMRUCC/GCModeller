#Region "Microsoft.VisualBasic::c5caf3009912accc816e2802c8a181e3, engine\IO\GCTabular\Compiler\KEGG.Compiler\Reactions.vb"

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

    '     Module Reactions
    ' 
    '         Function: __match, CompileCARMEN, CompileExpasy, CompileSmallMoleculeReactions, Convert
    '                   GenerateModel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.Archives.Xml.Nodes
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Interops
Imports SMRUCC.genomics.Model.SBML

Namespace KEGG.Compiler

    Public Module Reactions

        <Extension>
        Private Function __match(models As Dictionary(Of String, Reaction), item As CARMEN.Reaction, ReactionsDownloads As String) As Reaction
            If models.ContainsKey(item.rnId) Then
                Return models(item.rnId)
            Else
                Dim DownloadFile As String = String.Format("{0}/Downloads_CARMEN/{1}.xml", ReactionsDownloads, item.rnId)
                If FileIO.FileSystem.FileExists(DownloadFile) Then
                    Return Nothing  '由于CARMEN软件的数据库与KEGG数据库的版本不一致，故而会出现这个情况，对这种错误进行忽略
                End If

                Dim DownloadReactionModel = ReactionWebAPI.Download(item.rnId)
                Call DownloadReactionModel.GetXml.SaveTo(DownloadFile)
                Return DownloadReactionModel
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="KEGGReactions">整个KEGG Reaction的数据库，酶促反映对象会使用CARMEN软件进行筛选</param>
        ''' <param name="CARMEN_DIR"></param>
        ''' <param name="ModelLoader">包含有整个KEGG数据库之中的Compound</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CompileCARMEN(KEGGReactions As IEnumerable(Of Reaction),
                                      CARMEN_DIR As String,
                                      ModelLoader As FileStream.IO.XmlresxLoader,
                                      ReactionsDownloads As String,
                                      CompoundsDownloads As String,
                                      Logging As LogFile) As List(Of FileStream.MetabolismFlux)

            Dim Models = KEGGReactions.ToDictionary(Function(item As Reaction) item.ID)
            Dim CARMEN = SMRUCC.genomics.Interops.CARMEN.Merge(CARMEN_DIR, EnzymaticOnly:=True)
            Dim LQuery = (From item As CARMEN.Reaction In CARMEN.AsParallel
                          Let Model = Models.__match(item, ReactionsDownloads)
                          Where Not Model Is Nothing
                          Select New With {.Enzymes = item.lstGene, .Model = Model}).ToArray

            Dim DownloadMetabolites As List(Of FileStream.Metabolite) = New List(Of FileStream.Metabolite)
            Dim Metabolites = ModelLoader.MetabolitesModel.Values.ToDictionary(Function(Metabolite As FileStream.Metabolite) Metabolite.KEGGCompound) '将字典对象由UniqueID转换为KEGGCompound标识符
            Dim Reactions As New List(Of FileStream.MetabolismFlux)(LQuery.Select(Function(model) GenerateModel(model.Model, Metabolites, model.Enzymes, CompoundsDownloads, DownloadMetabolites, Logging))) ' 由于可能会涉及到List.Add操作（当需要下载缺失数据的时候）故而不能够进行并行化

            If Not DownloadMetabolites.IsNullOrEmpty Then
                Call ModelLoader.MetabolitesModel.AddRange(DownloadMetabolites)
            End If

            Return Reactions
        End Function

        Private Function Convert(DataModel As Level2.Elements.Reaction) As String
            Dim Equation = EquationBuilder.ToString(
                LeftSide:=(From item In DataModel.Reactants Select New KeyValuePair(Of Double, String)(item.stoichiometry, item.species)).ToArray,
                RightSide:=(From item In DataModel.Products Select New KeyValuePair(Of Double, String)(item.stoichiometry, item.species)).ToArray,
                Reversible:=DataModel.reversible)

            Return Equation
        End Function

        ''' <summary>
        ''' 在调用本方法之前，请确认已经将代谢物模型给编译好了
        ''' </summary>
        ''' <param name="ECTable"></param>
        ''' <param name="KEGGReactions"></param>
        ''' <param name="ModelLoader"></param>
        ''' <param name="MetaCyc"></param>
        ''' <param name="Sbml"></param>
        ''' <param name="CompoundsDownloads"></param>
        ''' <param name="ReactionDownloads"></param>
        ''' <param name="Logging"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CompileExpasy(ECTable As Generic.IEnumerable(Of SMRUCC.genomics.Assembly.Expasy.AnnotationsTool.T_EnzymeClass_BLAST_OUT),
                                      KEGGReactions As Generic.IEnumerable(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction),
                                      ModelLoader As FileStream.IO.XmlresxLoader,
                                      MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,
                                      Sbml As Level2.XmlFile,
                                      CompoundsDownloads As String,
                                      ReactionDownloads As String, Logging As LogFile) _
            As List(Of FileStream.MetabolismFlux)

            Dim Reactions As New Dictionary(Of String, KeyValuePair(Of List(Of String), SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction))
            Dim i As Integer
            Dim MetaCycEnzymaticsReactions = MetaCyc.GetReactions.getECDictionary
            Dim SbmlModels = Sbml.Model.listOfReactions.ToDictionary(Function(item As Level2.Elements.Reaction) item.id)

            For Each Enzyme In ECTable
                Dim LQuery = (From item In KEGGReactions.AsParallel Where EC_Mapping.IsECEquals(item.Enzyme, Enzyme.Class) Select item).ToArray

                If LQuery.IsNullOrEmpty Then
                    If Not MetaCycEnzymaticsReactions.ContainsKey(Enzyme.Class) Then
                        i += 1
                        Call Logging.WriteLine(String.Format("[{0}] ""{1}"" could not found any entry in the KEGG reaction database!", Enzyme.Class, Enzyme.ProteinId), "CompileExpasy()")
                        Continue For
                    End If

                    Dim ReactionModels = MetaCycEnzymaticsReactions(Enzyme.Class) '将ReactionModel转换为LQuery
                    LQuery = (From mRxn As Slots.Reaction In ReactionModels
                              Where SbmlModels.ContainsKey(mRxn.Identifier)
                              Select New Reaction With {
                                  .ID = mRxn.Identifier,
                                  .Definition = mRxn.CommonName,
                                  .CommonNames = If(mRxn.Names.IsNullOrEmpty, Nothing, mRxn.Names.ToArray),
                                  .Comments = mRxn.Comment,
                                  .Enzyme = {Enzyme.Class},
                                  .Equation = Convert(SbmlModels(mRxn.Identifier))
                                  }).ToArray
                End If

                For Each item In LQuery
                    If Not Reactions.ContainsKey(item.ID) Then
                        Call Reactions.Add(item.ID, New KeyValuePair(Of List(Of String), SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction)(New List(Of String) From {Enzyme.ProteinId}, item))
                    Else
                        Dim Target = Reactions(item.ID)
                        Call Target.Key.Add(Enzyme.ProteinId)
                    End If
                Next
            Next

            Call Logging.WriteLine(String.Format("{0}%({1}/{2}) are unable to assign reaction entry", 100 * i / ECTable.Count, i, ECTable.Count), "CompileExpasy()")

            Dim DownloadMetabolites As List(Of FileStream.Metabolite) = New List(Of FileStream.Metabolite)
            Dim Metabolites = ModelLoader.MetabolitesModel.Values.ToDictionary(Function(Metabolite As FileStream.Metabolite) As String
                                                                                   If Not String.IsNullOrEmpty(Metabolite.KEGGCompound) Then Return Metabolite.KEGGCompound
                                                                                   If Not String.IsNullOrEmpty(Metabolite.MetaCycId) Then Return Metabolite.MetaCycId
                                                                                   Return Metabolite.Identifier
                                                                               End Function)
            Dim ReactionDataModels = LinqAPI.MakeList(Of FileStream.MetabolismFlux) <=
 _
                From Model
                In Reactions.Values
                Let ObjectModel = GenerateModel(Model.Value, Metabolites, Model.Key, CompoundsDownloads, DownloadMetabolites, Logging)
                Where Not ObjectModel Is Nothing
                Select ObjectModel ' 由于可能会涉及到List.Add操作（当需要下载缺失数据的时候）故而不能够进行并行化

            Call Logging.WriteLine(String.Format("Assign {0} KEGG reactions", ReactionDataModels.Count), "CompileExpasy()")

            If Not DownloadMetabolites.IsNullOrEmpty Then Call ModelLoader.MetabolitesModel.AddRange(DownloadMetabolites)

            Call DownloadMetabolites.Clear()
            Call ReactionDataModels.AddRange(CompileSmallMoleculeReactions(MetaCyc, SbmlModels, Metabolites, CompoundsDownloads, DownloadMetabolites, Logging))

            If Not DownloadMetabolites.IsNullOrEmpty Then Call ModelLoader.MetabolitesModel.AddRange(DownloadMetabolites)

            Return ReactionDataModels
        End Function

        ''' <summary>
        ''' 编译所有不受任何酶分子催化的小分子化合物代谢反应过程
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <param name="Sbml"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CompileSmallMoleculeReactions(MetaCyc As SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.DatabaseLoadder,
                                                       Sbml As Dictionary(Of String, Level2.Elements.Reaction),
                                                       Metabolites As Dictionary(Of String, FileStream.Metabolite),
                                                       DownloadDir As String,
                                                       ByRef DownloadList As List(Of FileStream.Metabolite), Logging As LogFile) _
            As FileStream.MetabolismFlux()

            Dim TempList = DownloadList
            Dim LQuery = (From item In MetaCyc.GetReactions.AsParallel Where String.IsNullOrEmpty(item.ECNumber) AndAlso item.EnzymaticReaction.IsNullOrEmpty Select item).ToArray
            Dim SbmlModels = (From item In LQuery
                              Where Sbml.ContainsKey(item.Identifier)
                              Let SbmlModel = Sbml(item.Identifier)
                              Let KEGGDataModel = New SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Reaction With {
                                  .ID = item.Identifier,
                                  .Definition = item.CommonName,
                                  .CommonNames = If(item.Names.IsNullOrEmpty, Nothing, item.Names.ToArray),
                                  .Comments = item.Comment,
                                  .Equation = Convert(SbmlModel)}
                              Select GenerateModel(KEGGDataModel, Metabolites, Nothing, DownloadDir, TempList, Logging)).ToArray
            DownloadList = TempList

            Return (From item In SbmlModels Where Not item Is Nothing Select item).ToArray
        End Function

        Private Function GenerateModel(Model As Reaction,
                                       Metabolites As Dictionary(Of String, FileStream.Metabolite),
                                       Enzymes As IEnumerable(Of String),
                                       DownloadDir As String,
                                       ByRef DownloadList As List(Of FileStream.Metabolite), Logging As LogFile) As FileStream.MetabolismFlux
            Dim fluxModel As FileStream.MetabolismFlux = New FileStream.MetabolismFlux With {
                .Identifier = Model.ID,
                .CommonName = Model.Comments
            }
            Dim EquationModel = EquationBuilder.CreateObject(Model.Equation)
            Dim Substrates = EquationModel.GetMetabolites.ToArray
            Dim IsKEGGReaction As Boolean = True

            For i As Integer = 0 To Substrates.Length - 1
                Dim UniqueId As String = Substrates(i).ID

                UniqueId = Regex.Replace(UniqueId, "\(.+?\)", "") '由于这里的编号仅为KEGGCompound的编号，故而可以直接在这里使用替换操作
                UniqueId = UniqueId.Split.Last.ToUpper

                If Not Metabolites.ContainsKey(UniqueId) Then
Download:
                    If Regex.Match(UniqueId, "C\d{5}").Success Then
                        Dim DownloadCompoundModel = MetaboliteWebApi.DownloadCompound(UniqueId)

                        Call DownloadCompoundModel.GetXml.SaveTo(String.Format("{0}/Downloads/{1}.xml", DownloadDir, UniqueId))
                        Call DownloadList.Add(Compound.GenerateObject(DownloadCompoundModel))
                        Call Metabolites.Add(UniqueId, DownloadList.Last)
                    ElseIf Regex.Match(UniqueId, "G\d{5}").Success Then
                        Dim SavedCache As String = String.Format("{0}/Downloads/{1}.xml", DownloadDir, UniqueId)
                        Dim DownloadCompoundModel As SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Glycan

                        If FileIO.FileSystem.FileExists(SavedCache) Then
                            DownloadCompoundModel = SavedCache.LoadXml(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Glycan)()

                            If Not String.IsNullOrEmpty(DownloadCompoundModel.Entry) Then
                                Call DownloadList.Add(Compound.GenerateObject(DownloadCompoundModel.ToCompound))
                                Call Metabolites.Add(UniqueId, DownloadList.Last)

                                Continue For
                            End If
                        End If

                        DownloadCompoundModel = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Glycan.Download(UniqueId)

                        Call DownloadCompoundModel.GetXml.SaveTo(SavedCache)
                        Call DownloadList.Add(Compound.GenerateObject(DownloadCompoundModel.ToCompound))
                        Call Metabolites.Add(UniqueId, DownloadList.Last)
                    Else
                        Dim MCQuery = (From item In Metabolites.AsParallel
                                       Where String.Equals(UniqueId, item.Value.MetaCycId, StringComparison.OrdinalIgnoreCase)
                                       Select item).ToArray     '目标可能是MetaCyc数据库之中的代谢物，在这里按照MetaCyc编号来查找

                        Dim Compound = If(MCQuery.IsNullOrEmpty, Nothing, MCQuery.First.Value)

                        If Compound Is Nothing Then
                            ' 已经无法知晓代谢物的任何情况了，说明该反应过程可能是错误的也可能是通用底物的过程
                            ' 则只能在编译器日志之中记录下当前的编号，然后将当前的反应过程删除
                            Call Logging.WriteLine(String.Format("{0} is unable to recognized, reaction {1} [{2}] was deleted!", UniqueId, Model.ID, Model.Equation), "GenerateModel()")
                            Return Nothing
                        Else
                            IsKEGGReaction = False
                        End If

                        If Not String.IsNullOrEmpty(Compound.KEGGCompound) Then
                            If Metabolites.ContainsKey(Compound.KEGGCompound) Then
                                Substrates(i).ID = Metabolites(Compound.KEGGCompound).Identifier
                                Continue For
                            End If

                            UniqueId = Compound.KEGGCompound

                            Call Logging.WriteLine(String.Format("A kegg compound was extract from metacyc:  {0}, goto download", UniqueId), "GenerateModel")

                            GoTo Download
                        Else
                            Continue For    '不做任何改变
                        End If
                    End If

                    Substrates(i).ID = DownloadList.Last.Identifier
                Else
                    Substrates(i).ID = Metabolites(UniqueId).Identifier
                End If
            Next

            fluxModel.Equation = EquationBuilder.ToString(EquationModel)

            If fluxModel.Reversible Then
                fluxModel.UPPER_Bound = 10
                fluxModel.LOWER_Bound = -10
                fluxModel.p_Dynamics_K_1 = 1
                fluxModel.p_Dynamics_K_2 = 1
            Else
                fluxModel.UPPER_Bound = 10
                fluxModel.p_Dynamics_K_1 = 1
            End If

            fluxModel.Enzymes = If(Enzymes.Empty, Nothing, Enzymes.ToArray)
            fluxModel.KEGGReaction = If(IsKEGGReaction, Model.ID, "")
            fluxModel.EnzymeClass = Model.Enzyme.First

            Return fluxModel
        End Function
    End Module
End Namespace
