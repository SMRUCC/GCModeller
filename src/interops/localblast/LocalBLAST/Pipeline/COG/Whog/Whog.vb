#Region "Microsoft.VisualBasic::2b6163404bf39f8112f3e84c0623743f, LocalBLAST\LocalBLAST\LocalBLAST\Application\COG\Whog\Whog.vb"

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

    '     Class Whog
    ' 
    '         Properties: Categories
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: [Imports], __assignInvoke, FindByCogId, MatchCogCategory, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput

Namespace LocalBLAST.Application.RpsBLAST.Whog

    ''' <summary>
    ''' Cog Category
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <XmlType("NCBI.whog", [Namespace]:=SMRUCC.genomics.LICENSE.GCModeller)>
    Public Class Whog : Inherits XmlDataModel

        <XmlElement("categories", [Namespace]:=GCModeller)>
        Public Property Categories As Category()
            Get
                Return _COGCategory
            End Get
            Set(value As Category())
                _COGCategory = value
                If value.IsNullOrEmpty Then
                    _categoryTable = New Dictionary(Of String, Category)
                Else
                    _categoryTable = value.ToDictionary(Function(x) x.COG_id)
                End If
            End Set
        End Property

        Dim _COGCategory As Category()
        Dim _categoryTable As Dictionary(Of String, Category)

        <XmlNamespaceDeclarations()>
        Public xmlns As XmlSerializerNamespaces

        Public Sub New()
            xmlns = New XmlSerializerNamespaces
            xmlns.Add("GCModeller", SMRUCC.genomics.LICENSE.GCModeller)
            xmlns.Add("ncbi", Category.NCBI_COG)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function FindByCogId(CogId As String) As Category
            Return _categoryTable.TryGetValue(CogId)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Shared Widening Operator CType(path As String) As Whog
            Return path.LoadXml(Of Whog)()
        End Operator

        ''' <summary>
        ''' 从Whog文本文件导入COG的分类数据，然后保存为XML文件
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function [Imports](path As String) As Whog
            Dim tokens As IEnumerable(Of String()) = path _
                .ReadAllLines _
                .Split("^[_]+$", True, RegexICMul) _
                .ToArray
            Dim LQuery = LinqAPI.Exec(Of Category) <=
 _
                From strToken As String()
                In tokens
                Where Not strToken.IsNullOrEmpty
                Let cat As Category = Category.Parse(strToken)
                Select cat
                Order By cat.COG_id

            Return New Whog With {
                .Categories = LQuery
            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="MatchedData">Myva BLASTP result</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function MatchCogCategory(MatchedData As IEnumerable(Of MyvaCOG)) As MyvaCOG()
            Dim LQuery = (From prot As MyvaCOG
                          In MatchedData.AsParallel
                          Let assignCOG As MyvaCOG = __assignInvoke(prot)
                          Select assignCOG).ToArray
            Return LQuery
        End Function

        Private Function __assignInvoke(prot As MyvaCOG) As MyvaCOG
            If String.IsNullOrEmpty(prot.MyvaCOG) OrElse
                String.Equals(prot.MyvaCOG, IBlastOutput.HITS_NOT_FOUND) Then
                Return prot '没有可以分类的数据
            End If

            Dim Cog = (From entry As Category
                       In Me.Categories
                       Where entry.ContainsGene(prot.MyvaCOG)
                       Select entry).FirstOrDefault

            If Cog Is Nothing Then
                Call $"Could Not found the COG category id for myva cog {prot.QueryName} <-> {prot.MyvaCOG}....".Warning
                Return prot
            End If

            prot.COG = Cog.COG_id
            prot.Category = Cog.Category
            prot.Description = Cog.Description

            Return prot
        End Function

        ''' <summary>
        ''' Save the whog data as XML
        ''' </summary>
        ''' <param name="FilePath"></param>
        ''' <param name="Encoding"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Save(filePath$, Optional Encoding As Encoding = Nothing) As Boolean
            Return Me.GetXml.SaveTo(filePath, Encoding Or UTF8)
        End Function
    End Class
End Namespace
