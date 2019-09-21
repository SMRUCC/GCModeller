#Region "Microsoft.VisualBasic::fc8e4cb6c5fc344b0d769fa5e2880316, engine\IO\GCMarkupLanguage\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: [Select], AsIntegerArray, AsMetabolite, (+3 Overloads) AsMetabolites, (+3 Overloads) AsStringArray
    '               Generate, (+3 Overloads) IndexOf, IsRegulator, (+2 Overloads) Replace, Replace2
    '               Takes
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Public Module Extensions

    Public Const Scan0 = 0

    ' ''' <summary>
    ' ''' 使用一个指定的酶促反应的标号来查询一个生化反应对象的句柄值
    ' ''' </summary>
    ' ''' <param name="List"></param>
    ' ''' <param name="UniqueId"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    '<Extension> Public Function FindEnzymaticRxn(List As List(Of Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction), UniqueId As String) As Integer
    '    For i As Integer = 0 To List.Count - 1
    '        Dim Rxn = List(i)
    '        If Rxn.EnzymaticRxn.IsNullOrEmpty Then
    '            Continue For
    '        End If
    '        For Idx As Integer = 0 To Rxn.EnzymaticRxn.Count - 1
    '            If String.Equals(UniqueId, Rxn.EnzymaticRxn(Idx).UniqueId) Then
    '                Return i
    '            End If
    '        Next
    '    Next
    '    Return -1
    'End Function

    <Extension> Public Function AsMetabolites(Collection As IEnumerable(Of Level2.Elements.Specie)) As List(Of Metabolism.Metabolite)
        Dim Query = From e In Collection.AsParallel Select Metabolism.Metabolite.CastTo(e) '
        Return Query.AsList
    End Function

    ''' <summary>
    ''' 将MetaCyc数据库之中的实体对象转换为FBA模型之中的代谢底物的定义
    ''' </summary>
    ''' <param name="Collection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function AsMetabolites(Collection As IEnumerable(Of Slots.Object)) As Metabolism.Metabolite()
        Dim Query = From [Object] In Collection.AsParallel
                    Let M = New Metabolism.Metabolite With {
                        .Identifier = [Object].Identifier,
                        .CommonName = [Object].CommonName,
                        .BoundaryCondition = False,
                        .Compartment = 2,
                        .InitialAmount = 1000
                    }
                    Select M Order By M.Identifier '

        Return Query.ToArray
    End Function

    ''' <summary>
    ''' 使用基因标号从基因列表之中提取出一个基因对象的集合
    ''' </summary>
    ''' <param name="Genes"></param>
    ''' <param name="IdCollection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function Takes(Genes As GeneObject(), IdCollection As List(Of String)) As GeneObject()
        Dim GeneList As List(Of GeneObject) = New List(Of GeneObject)

        For i As Integer = 0 To IdCollection.Count - 1
            Dim Id As String = IdCollection(i)
            GeneList.Add((From Gene In Genes.AsParallel Where String.Equals(Gene.Identifier, Id) Select Gene).First)
        Next
        Return GeneList.ToArray
    End Function

    <Extension> Public Function IndexOf(List As List(Of Compartment), Id As String) As Integer
        For i As Integer = 0 To List.Count - 1
            If String.Equals(List(i).id, Id) Then
                Return i
            End If
        Next
        Return -1
    End Function

    ''' <summary>
    ''' Query target enzyme object using its unique id property from a enzyme list.(依照对象的UniqueId属性在酶催化剂分子列表之中查找出目标对象)
    ''' </summary>
    ''' <param name="List"></param>
    ''' <param name="UniqueId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function [Select](List As List(Of Slots.Protein.IEnzyme), UniqueId As String) As Slots.Protein.IEnzyme
        Dim LQuery = From Enz In List.AsParallel Where String.Equals(Enz.UniqueId, UniqueId) Select Enz '
        Return LQuery.FirstOrDefault
    End Function

    ''' <summary>
    ''' 将Unique列表转换为代谢物的引用类型列表
    ''' </summary>
    ''' <param name="List"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function AsMetabolites(List As List(Of String)) As CompoundSpeciesReference()
        Dim LQuery = (From Id As String In List
                      Select New CompoundSpeciesReference With {
                          .Identifier = Id,
                          .StoiChiometry = 1}).ToArray  '
        Return LQuery
    End Function

    ''' <summary>
    ''' All of the gene expression regulation types that listed in the MetaCyc database.(在MetaCyc数据库中所列举的所有基因表达调控类型)
    ''' </summary>
    ''' <remarks></remarks>
    Dim RegulationTypes As String() = {
        "Allosteric-Regulation-of-RNAP", "Compound-Mediated-Translation-Regulation", "Protein-Mediated-Attenuation", "Protein-Mediated-Translation-Regulation",
        "Regulation-of-Translation", "Rho-Blocking-Antitermination", "Ribosome-Mediated-Attenuation", "RNA-Mediated-Translation-Regulation",
        "Small-Molecule-Mediated-Attenuation", "Transcriptional-Attenuation", "Transcription-Factor-Binding"}

    ''' <summary>
    ''' 判断某一个实体对象是否为调控因子
    ''' </summary>
    ''' <param name="Entity"></param>
    ''' <param name="Regulations"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function IsRegulator(Entity As SignalTransductions.Regulator, Regulations As Regulations) As Boolean
        For i As Integer = 0 To Entity.BaseType.Regulates.Count - 1
            Dim Regulation = Regulations.Item(Entity.BaseType.Regulates(i))
            If Array.IndexOf(RegulationTypes, Regulation.Types) > -1 Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Find the location of a specific gene object using its product uniqueid property.(根据产物的UniqueID来查找某一个基因的位置)
    ''' </summary>
    ''' <param name="List"></param>
    ''' <param name="ProductID">UniqueId of the gene product</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function IndexOf(List As GeneObject(), ProductID As String) As Integer
        Dim Query = From e In List.AsParallel Where e.BaseType.Product.IndexOf(ProductID) > -1 Select e ' '查找出某一个基因对象的表达产物中含有目标分子对象的对象实例
        If Query.Count > 0 Then  '查找到了该目标，则返回其对象句柄值
            Throw New NotImplementedException
        Else
            Return -1
        End If
    End Function

    ''' <summary>
    ''' 依照标号查询列表中的某一个代谢物的句柄值
    ''' </summary>
    ''' <param name="List"></param>
    ''' <param name="UniqueId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function IndexOf(List As IEnumerable(Of GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite), UniqueId As String) As Integer
        For i As Integer = 0 To List.Count - 1
            If String.Equals(List(i).Identifier, UniqueId) Then
                Return i
            End If
        Next
        Return -1
    End Function

    <Extension> Public Function AsMetabolite(Id As String) As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite
        Dim Metaboliet As New GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite
        Metaboliet.Identifier = Id
        Metaboliet.CommonName = Id
        Metaboliet.InitialAmount = 1000
        Metaboliet.BoundaryCondition = True
        Metaboliet.Compartment = 2

        Return Metaboliet
    End Function

    <Extension> Public Function AsStringArray(List As Generic.IEnumerable(Of Long)) As String()
        Dim Query = From e In List Select CType(e, String) '
        Return Query.ToArray
    End Function

    <Extension> Public Function AsStringArray(List As Generic.IEnumerable(Of Integer)) As String()
        Dim Query = From e In List Select CType(e, String) '
        Return Query.ToArray
    End Function

    <Extension> Public Function AsStringArray(List As Generic.IEnumerable(Of Double)) As String()
        Dim Query = From e In List Select CType(e, String) '
        Return Query.ToArray
    End Function

    <Extension> Public Function AsIntegerArray(List As Generic.IEnumerable(Of String)) As Integer()
        Dim Query = From e In List Select CType(Val(e), Integer) '
        Return Query.ToArray
    End Function

    <Extension> Public Function Generate(Of T_REF As ICompoundSpecies)(Metabolite As FLuxBalanceModel.IMetabolite,
                                                                       Metabolism As IEnumerable(Of FLuxBalanceModel.I_ReactionModel(Of T_REF))) As FBACompatibility.Vector

        Dim LQuery = From Flux In Metabolism Select Flux.GetStoichiometry(Metabolite.Key) '
        Return New FBACompatibility.Vector With {
            .Values = LQuery.ToArray,
            .Identifier = Metabolite.Key
        }
    End Function

    <Extension> Public Function Replace2(Of TMetabolite As FLuxBalanceModel.IMetabolite)(ByRef Vector As TMetabolite, StringList As Escaping()) As TMetabolite
        Vector.Key = Replace(Vector.Key, StringList)
        Return Vector
    End Function

    <Extension> Public Function Replace(Of T_REF As ICompoundSpecies)(ByRef MetabolismFlux As FLuxBalanceModel.I_ReactionModel(Of T_REF), StringList As Escaping()) As FLuxBalanceModel.I_ReactionModel(Of T_REF)
        MetabolismFlux.Key = Replace(MetabolismFlux.Key, StringList)
        Return MetabolismFlux
    End Function

    <Extension> Public Function Replace(s As String, StringList As Escaping()) As String
        Dim sBuilder As StringBuilder = New StringBuilder(s)
        For Each [string] In StringList
            Call sBuilder.Replace([string].Escape, [string].Original)
        Next

        If sBuilder.Chars(0) = "_"c Then
            Call sBuilder.Remove(0, 1)
        End If
        Return sBuilder.ToString
    End Function
End Module
