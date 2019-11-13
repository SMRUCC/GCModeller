#Region "Microsoft.VisualBasic::b09fc176bd866d83d28e11234f89f46f, CLI_tools\c2\Reconstruction\ObjectEquals\Proteins.vb"

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

    '     Class Proteins
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetEquals, Initialize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports c2.Reconstruction.Operation

Namespace Reconstruction.ObjectEquals

    ''' <summary>
    ''' 本类型必须在完成构建Reconstructed数据库之后放能够执行初始化操作
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Proteins : Inherits c2.Reconstruction.ObjectEquals.EqualsOperation

        Dim SchemaCache As ObjectSchema(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein)
        Dim rctCompounds As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Compounds
        Dim sbjProteins As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Proteins
        Dim rctProteins As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Proteins

        Sub New(Session As OperationSession)
            Call MyBase.New(Session)
            SchemaCache = New EqualsOperation.ObjectSchema(Of  _
                LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein) _
                (FieldList:=New String() {"COMPONENTS"})
            rctCompounds = Reconstructed.GetCompounds
            sbjProteins = Subject.GetProteins
            rctProteins = Reconstructed.GetProteins
        End Sub

        Public Overrides Function Initialize() As Integer
            Dim n As Integer
            Dim EqualsList As List(Of KeyValuePair(Of String, String())) = New List(Of KeyValuePair(Of String, String()))

            For Each sbjProteinCPLX In sbjProteins
                Dim Temp As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein =
                    New LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein With
                    {
                        .Components = New List(Of String)}

                If Not GetEquals(sbjProteinCPLX, Temp) Then
                    Continue For
                Else
                    If String.IsNullOrEmpty(Temp.Identifier) Then
                        Dim LQuerys = From Protein As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein
                              In rctProteins.Select(Temp, SchemaCache.ItemProperties, SchemaCache.FieldAttributes)
                              Select Protein.Identifier '
                        Dim Result = LQuerys.ToArray

                        If Not Result.IsNullOrEmpty Then
                            Call EqualsList.Add(New KeyValuePair(Of String, String())(sbjProteinCPLX.Identifier, LQuerys.ToArray))
                            n += 1
                        End If
                    Else
                        Call EqualsList.Add(New KeyValuePair(Of String, String())(sbjProteinCPLX.Identifier, {Temp.Identifier}))
                        n += 1
                    End If
                End If
            Next

            MyBase.EqualsList = EqualsList.ToArray

            Return n
        End Function

        ''' <summary>
        ''' 通过BestHit列表来获取替换目标蛋白质复合物中的Monomer组件
        ''' </summary>
        ''' <param name="SubjectProtein"></param>
        ''' <param name="Out">通过这个参数来返回</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Function GetEquals(ByRef SubjectProtein As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein,
                                            ByRef Out As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein) _
            As Boolean

            If SubjectProtein.Components.IsNullOrEmpty Then '假设其为一种蛋白质单体
                '再判断BestHit列表中等价的蛋白质单体是否存在
                Dim Id As String = MyBase.HomologousProteins.GetUniqueID(SubjectProtein.Identifier)
                If String.IsNullOrEmpty(Id) Then
                    Return False '没有找到等价的蛋白质单体，则不能进行重建
                Else
                    Out.Identifier = Id
                    Return True
                End If
            End If

            For Each component As String In SubjectProtein.Components
                If rctCompounds.IndexOf(component) > -1 Then  '这个组件是一种小分子化合物，则跳过
                    Call Out.Components.Add(component)
                Else  '可能为一种蛋白质
                    Dim sbjProtein = sbjProteins.Get(UniqueId:=component) 'component可能为蛋白质复合物或者亚基

                    If sbjProtein Is Nothing Then
                        '目标对象可能为一种小分子化合物，并且不存在于目标重建数据库之中，则退出重建操作
                        Return False
                    End If

                    If Not sbjProtein.Components.IsNullOrEmpty Then '目标对象为一种蛋白质复合物：先递归的判断该组件是否存在与Reconstructed数据库之中
                        Dim Temp As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein =
                            New LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein With
                            {
                                .Components = New Generic.List(Of String)}

                        If GetEquals(sbjProtein, Out:=Temp) Then
                            Dim LQuerys = From Protein As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein
                                In rctProteins.Select(Temp, SchemaCache.ItemProperties, SchemaCache.FieldAttributes)
                                Select Protein.Identifier '
                            Dim Result As String() = LQuerys.ToArray

                            If Not Result.IsNullOrEmpty Then
                                Call Out.Components.Add(Result.First)
                            End If
                        Else
                            Return False
                        End If
                    Else
                        '再判断BestHit列表中等价的蛋白质单体是否存在
                        Dim Id As String = MyBase.HomologousProteins.GetUniqueID(component)
                        If String.IsNullOrEmpty(Id) Then
                            Return False '没有找到等价的蛋白质单体，则不能进行重建
                        Else
                            Call Out.Components.Add(Id)
                        End If
                    End If
                End If
            Next

            Return True
        End Function
    End Class
End Namespace
