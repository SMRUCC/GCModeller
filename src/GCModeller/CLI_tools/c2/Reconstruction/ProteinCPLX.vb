#Region "Microsoft.VisualBasic::8c077cdd0999d5977d73bfb47ffcc574, CLI_tools\c2\Reconstruction\ProteinCPLX.vb"

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

    '     Class ProteinCPLX
    ' 
    '         Properties: ReconstructList
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Performance, QueryComponents, Reconstruct
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.ConsoleDevice.STDIO

Namespace Reconstruction

    ''' <summary>
    ''' 对蛋白质复合物的重建方法
    ''' </summary>
    ''' <remarks>
    ''' ProteinAssembly包括有：
    ''' 多个多肽链蛋白质亚基之间的结合过程
    ''' 多肽链和小分子配基之间的相互结合过程
    ''' 
    ''' 小分子化合物的Unique-ID在数据库之间是唯一的
    ''' </remarks>
    Public Class ProteinCPLX : Inherits Operation

        ''' <summary>
        ''' Subject数据库之中的Proteins表
        ''' </summary>
        ''' <remarks></remarks>
        Dim SubjectProteins As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Proteins
        ''' <summary>
        ''' 待重建数据库的Compounds表
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend rctCompounds As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Compounds
        Protected Friend rctProteins As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Proteins

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ReconstructList As Operation.ReconstructedList(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein)

        Sub New(Session As c2.Reconstruction.Operation.OperationSession)
            Call MyBase.New(Session)
            Me.ReconstructList = New Operation.ReconstructedList(Of LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein)
            Me.SubjectProteins = Session.Subject.GetProteins
            Me.rctCompounds = Session.ReconstructedMetaCyc.GetCompounds
            Me.rctProteins = Session.ReconstructedMetaCyc.GetProteins
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 蛋白质复合物对象的构造标准：当所有的组件都在目标细胞内存在的时候，就认为该蛋白质复合物存在
        ''' </remarks>
        Public Overrides Function Performance() As Integer
            For Each Pair As KeyValuePair(Of String, String) In MyBase.HomologousProteins
                Dim IDList As String() = QueryComponents(Pair.Value).Distinct.ToArray
                For Each Protein As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein In SubjectProteins.Takes(IDList)
                    Call Reconstruct(Protein)
                Next
            Next
            Call Printf("\n\n ***** Reconstructed %s protein-complex in total! *****", ReconstructList.Count)
            Dim s As String = MyBase.Workspace & "\ProteinCPLX-ReconstructedList.txt"
            Call CType(Me.ReconstructList.ToTable, LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Proteins).Save(s)
            Call Printf("You can found out the reconstructed data in database file:\n  '%s'", s)
            Call rctProteins.Indexing()
            Return 0
        End Function

        ''' <summary>
        ''' 递归的查询Subject数据库中的Proteins表，获取所有的Components列表中包含有目标蛋白质的蛋白质复合物
        ''' </summary>
        ''' <param name="UniqueID">Subject数据库中的蛋白质的UniqueID</param>
        ''' <returns>包含有该蛋白质对象的蛋白质复合物的UniqueID的集合</returns>
        ''' <remarks></remarks>
        Private Function QueryComponents(UniqueID As String) As List(Of String)
            Dim Protein As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein =
                Me.SubjectProteins.Select(UniqueID)
            Dim IDList As List(Of String) = New List(Of String)
            For Each ID As String In Protein.ComponentOf '递归的获取所有包含有该蛋白质对象的蛋白质复合物的列表
                Call IDList.AddRange(QueryComponents(ID))
            Next
            Call IDList.AddRange(Protein.ComponentOf)

            Return IDList
        End Function

        ''' <summary>
        ''' 根据Subject数据库之中的ProteinCPLX对象重建待重建的目标数据库中的一个ProteinCPLX对象
        ''' </summary>
        ''' <param name="ProteinCPLX">Subject数据库之中的一个ProteinCPLX对象</param>
        ''' <remarks>
        ''' 重构算法：
        ''' 首先在Proteins表中进行查询
        ''' 当查询到目标对象的时候，进入迭代重构的过程
        ''' 当没有查询到的时候，则查询待重建数据库之中的Compounds列表
        ''' 当在Compounds列表中没有查询出目标对象的时候，则认为当前的Subject数据库之中的这个蛋白质复合物对象不存在
        ''' </remarks>
        ''' <returns>返回所重建的目标对象的UNIQUE-ID属性值</returns>
        Private Function Reconstruct(ProteinCPLX As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein) As String
            Dim ComponentList As List(Of String) = New List(Of String) '由Subject数据库中的蛋白质复合物对象所重建出来的蛋白质复合物的组分列表

            For Each Component As String In ProteinCPLX
                Dim Protein As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein =
                    SubjectProteins.Get(UniqueId:=Component)

                If Protein Is Nothing Then '目标对象可能是一种小分子化合物
                    Dim p As Integer = Me.rctCompounds.IndexOf(Component)
                    If p = -1 Then '没有查询到相应的对象
                        Return ""
                    Else
                        Call ComponentList.Add(Me.rctCompounds(p).Identifier) '查询到了目标对象，则向组分列表之中添加当前的组分对象的UniqueID
                    End If
                Else '目标对象是一种蛋白质复合物或者多肽链蛋白质单体
                    If Protein.Components.IsNullOrEmpty Then '当前的这个蛋白质对象可能为一个蛋白质单体，则在BestHit列表之中进行查询，假若查询不到，则认为不是所拥有的蛋白质，退出重构操作
                        Dim Id As String = MyBase.HomologousProteins.GetUniqueID(Protein.Identifier)
                        If String.IsNullOrEmpty(Id) Then
                            Return ""
                        Else
                            Call ComponentList.Add(Id) '查询到的目标对象是一个同源的蛋白质，则添加进入当前对象的组分列表之中
                        End If
                    Else '当目标对象为一个蛋白质复合物的时候，则递归的进行重构工作
                        Dim Id As String = Reconstruct(ProteinCPLX:=Protein)
                        If String.IsNullOrEmpty(Id) Then '组分对象没有重建成功，则当前的蛋白质复合物对象没有A重建成功，退出重构操作
                            Return ""
                        Else
                            Call ComponentList.Add(Id)
                        End If
                    End If
                End If
            Next
            '当所有的组分都能够通过验证的时候，则说明这个蛋白质复合物存在于目标待重建数据库所代表的物种之中的

            Dim ReconstructedProteinCPLX As LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein = LANS.SystemsBiology.Assembly.MetaCyc.File.DataFiles.Slots.Protein.[New]
            ReconstructedProteinCPLX.Identifier = ProteinCPLX.Identifier
            ReconstructedProteinCPLX.Components = ComponentList
            ReconstructedProteinCPLX.AbbrevName = ProteinCPLX.AbbrevName
            ReconstructedProteinCPLX.Comment = ProteinCPLX.Comment
            ReconstructedProteinCPLX.CommonName = ProteinCPLX.CommonName
            ReconstructedProteinCPLX.Names = ProteinCPLX.Names
            ReconstructedProteinCPLX.Types.Add("Protein-Complexes")
            Call ReconstructList.Add(ProteinCPLX.Identifier, ReconstructedProteinCPLX)
            Call rctProteins.Add(ReconstructedProteinCPLX)

            Call Printf("CPLX:: '%s'", ReconstructedProteinCPLX.Identifier)

            Return ReconstructedProteinCPLX.Identifier
        End Function
    End Class
End Namespace
