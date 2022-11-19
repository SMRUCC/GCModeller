#Region "Microsoft.VisualBasic::02ca2d9ad460f73fc4807f5abe5bebea, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\MetacycObjectFindingMethods.vb"

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

    '   Total Lines: 194
    '    Code Lines: 69
    ' Comment Lines: 109
    '   Blank Lines: 16
    '     File Size: 10.85 KB


    ' Module MetacycObjectFindingMethods
    ' 
    '     Function: [Select], GetHandles, GetRegulatedObject, (+2 Overloads) IndexOf, Take
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.STDIO
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem

''' <summary>
''' An extension method collection of the metacyc object query for the model compiling processing.
''' (对MetaCyc数据库中的目标对象的查找扩展方法的集合)
''' </summary>
''' <remarks></remarks>
Module MetacycObjectFindingMethods

    ''' <summary>
    ''' 从MetaCyc数据库之中，查询出目标调控对象
    ''' </summary>
    ''' <param name="Regulation"></param>
    ''' <param name="MetaCyc"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetRegulatedObject(Regulation As Slots.Regulation, MetaCyc As DatabaseLoadder) As Slots.Object

        Dim UniqueId As String = Regulation.RegulatedEntity

        If MetaCyc.GetPromoters.IndexOf(UniqueId) > -1 Then '启动子
            Return MetaCyc.GetPromoters.Item(UniqueId)
        ElseIf MetaCyc.GetEnzrxns.IndexOf(UniqueId) > -1 Then '酶促反应
            Return MetaCyc.GetEnzrxns.Item(UniqueId)
        ElseIf MetaCyc.GetReactions.IndexOf(UniqueId) > -1 Then '代谢反应
            Return MetaCyc.GetReactions.Item(UniqueId)
        ElseIf MetaCyc.GetProteins.IndexOf(UniqueId) > -1 Then '蛋白质
            Return MetaCyc.GetProteins.Item(UniqueId)
        ElseIf MetaCyc.GetProtLigandCplx.IndexOf(UniqueId) > -1 Then '蛋白质复合物
            Return MetaCyc.GetProtLigandCplx.Item(UniqueId)
        ElseIf MetaCyc.GetTerminators.IndexOf(UniqueId) > -1 Then '终止子
            Return MetaCyc.GetTerminators.Item(UniqueId)
        ElseIf MetaCyc.GetGenes.IndexOf(UniqueId) > -1 Then '基因
            Return MetaCyc.GetGenes.Item(UniqueId)
        ElseIf MetaCyc.GetTransUnits.IndexOf(UniqueId) > -1 Then '转录单元
            Return MetaCyc.GetTransUnits.Item(UniqueId)
        Else
            If Not String.IsNullOrEmpty(Regulation.AssociatedBindingSite) > 0 Then '相关联的DNA结合位点
                Return MetaCyc.GetDNABindingSites.Item(Regulation.AssociatedBindingSite)
            Else
                Throw New DataException(MetaCyc.Database.ToString & vbCrLf & "MetaCyc dataset object link error, could not found any associated object data in the metacyc database!")  '错误
            End If
        End If
    End Function

    ''' <summary>
    ''' 获取目标MetaCyc对象在模型中的对象句柄值集合
    ''' </summary>
    ''' <param name="Object">对于模型中已经指定的类型，直接进行查找然后返回句柄值，对于在模型之中不存在的类型，则先查找出相应的对象，再返回句柄值</param>
    ''' <param name="Model"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function GetHandles([Object] As Slots.Object, Model As BacterialModel) As Long()
        'Select Case [Object].Table
        '    Case MetaCyc.File.DataFiles.Slots.Object.Tables.enzrxns
        '        Dim Query = From obj In Model.Metabolism.MetabolismNetwork.AsParallel Let enzrxn = (From e In obj.EnzymaticRxn Where String.Equals(e.UniqueId, [Object].UniqueId) Select e).ToArray Where enzrxn.Count > 0 Select obj.Handle  '
        '        Return Query.ToArray
        '    Case MetaCyc.File.DataFiles.Slots.Object.Tables.genes
        '        Dim obj = [Object].Select(Of Slots.Gene, GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.GeneObject)(Model.BacteriaGenome.Genes)
        '        If obj Is Nothing Then
        '            Return New Long() {-1}
        '        Else
        '            '          Return New Long() {obj.Handle}
        '        End If
        '    Case MetaCyc.File.DataFiles.Slots.Object.Tables.promoters
        '        Dim Query = From TU In Model.BacteriaGenome.TransUnits.AsParallel Where TU.BaseType.Components.IndexOf([Object].UniqueId) > -1 Select TU.Handle  '
        '        Return Query.ToArray
        '    Case MetaCyc.File.DataFiles.Slots.Object.Tables.proteins, MetaCyc.File.DataFiles.Slots.Object.Tables.protligandcplxes '在酶分子列表和调控因子列表中进行查询

        '        Dim Query = From obj In Model.Metabolism.Metabolites.AsParallel Where String.Equals(obj.UniqueId, [Object].UniqueId) Select obj  '
        '        Dim protHandle = Query.First '查询出底物存根
        '        '分别在酶分子和调控因子列表中进行查询，返回该对象在其自身列表中的句柄值
        '        Dim EnzQuery = From enz In Model.Metabolism.MetabolismEnzymes.AsParallel Where enz.pHwnd = protHandle.Hwnd Select enz.Handle '
        '        Dim result = EnzQuery.ToArray
        '        If result.Count > 0 Then
        '            Return result
        '        Else
        '            Return New Long() {}
        '            ' Dim RegQuery = From reg In Model.Regulators.AsParallel Where String.Equals(reg.UniqueId, reg.UniqueId) Select reg.Handle '
        '            'Return RegQuery.AsStringArray
        '            '  Throw New NotImplementedException '未完成重构
        '        End If

        '    Case MetaCyc.File.DataFiles.Slots.Object.Tables.reactions
        '        Dim obj = [Object].Select(Of Slots.Reaction, GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction)(Model.Metabolism.MetabolismNetwork)
        '        If obj Is Nothing Then
        '            Return New Long() {-1}
        '        Else
        '            '       Return {obj.Handle}
        '        End If
        '    Case MetaCyc.File.DataFiles.Slots.Object.Tables.terminators
        '        Dim Query = From TU In Model.BacteriaGenome.TransUnits.AsParallel Where TU.BaseType.Components.IndexOf([Object].UniqueId) > -1 Select TU.Handle  '
        '        Return Query.ToArray
        '    Case MetaCyc.File.DataFiles.Slots.Object.Tables.transunits
        '        Dim obj = [Object].Select(Of Slots.TransUnit, GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.TranscriptUnit)(Model.BacteriaGenome.TransUnits)
        '        If obj Is Nothing Then
        '            Return New Long() {-1}
        '        Else
        '            '         Return {obj.Handle}
        '        End If
        '    Case MetaCyc.File.DataFiles.Slots.Object.Tables.dnabindsites

        '        Dim DNABindSite = DirectCast([Object], SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.DNABindSite)
        '        Dim TUList = From TU In Model.BacteriaGenome.TransUnits Where DNABindSite.ComponentOf.IndexOf(TU.UniqueId) > -1 Select TU.Handle '
        '        Return TUList.ToArray

        'End Select

        'Return New Long() {}
        Throw New NotImplementedException
    End Function

    Public Function Take(Of obj As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object,
                            Entity As GCMarkupLanguage.GCML_Documents.ComponentModels.T_MetaCycEntity(Of obj))(Collection As Generic.IEnumerable(Of Entity), [Handles] As Long()) As Entity()
        '   Dim LQuery = (From entityObj As Entity In Collection Where Array.IndexOf([Handles], entityObj.Handle) Select entityObj).ToArray
        '    Return LQuery
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' 根据目标对象的唯一标识符，查找出模型中相应对象的句柄值
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="E"></typeparam>
    ''' <param name="Object"></param>
    ''' <param name="Table"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Extension> Public Function [Select](Of T As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object, E As GCML_Documents.ComponentModels.T_MetaCycEntity(Of T)) _
            ([Object] As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object, Table As Generic.IEnumerable(Of E)) As E
        Try
            Dim LQuery = From obj In Table.AsParallel Where String.Equals([Object].Identifier, obj.Identifier) Select obj '
            Return LQuery.First
        Catch ex As Exception
            Call Printf("[OBJECT_NOT_FOUND] could not found any object in the table reference to object %s", [Object].Identifier)
            Return Nothing
        End Try
    End Function

    '<Extension> Public Sub AddTypeHandle(ByRef Regulation As GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Regulator.Regulation, Type As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables)
    '    Select Case Type
    '        Case Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.genes,
    '            Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.transunits,
    '            Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.promoters,
    '            Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.terminators,
    '            Slots.Object.Tables.dnabindsites
    '            Regulation.Class = Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.transunits
    '        Case Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.enzrxns,
    '            Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.reactions
    '            Regulation.Class = Assembly.MetaCyc.File.DataFiles.Slots.Object.Tables.reactions
    '        Case Else
    '            Regulation.Class = Type
    '    End Select
    'End Sub

    ''' <summary>
    ''' MetabolismMap中的对象类型列表元素的通用查找方法
    ''' </summary>
    ''' <param name="ListCollection"></param>
    ''' <param name="UniqueId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IndexOf(Of T As SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object,
                               Entity As SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels.T_MetaCycEntity(Of T))( _
                               ListCollection As Generic.IEnumerable(Of Entity),
                               UniqueId As String) _
        As Integer

        For i As Integer = 0 To ListCollection.Count - 1
            If String.Equals(ListCollection(i).Identifier, UniqueId) Then
                Return i
            End If
        Next
        Return -1
    End Function

    ''' <summary>
    ''' 获取指定UniqueId的生化反应对象的句柄值
    ''' </summary>
    ''' <param name="UniqueId"></param>
    ''' <returns>Object Handle</returns>
    ''' <remarks></remarks>
    <Extension> Public Function IndexOf(List As List(Of GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction), UniqueId As String) As Integer
        For i As Integer = 0 To List.Count - 1
            If String.Equals(UniqueId, List(i).Identifier) Then
                Return i
            End If
        Next
        Return -1
    End Function
End Module
