#Region "Microsoft.VisualBasic::eae865044e4565b57819cfd5b859a6f6, localblast\LocalBLAST\Pipeline\COG\Whog\Whog.vb"

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

    '     Class WhogRepository
    ' 
    '         Properties: Categories
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: [Imports], FindByCogId, MatchCogCategory, Save
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

Namespace Pipeline.COG.Whog

    ''' <summary>
    ''' Cog Category
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <XmlType("NCBI.whog", [Namespace]:=SMRUCC.genomics.LICENSE.GCModeller)>
    Public Class WhogRepository : Inherits XmlDataModel

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
        Public Overloads Shared Widening Operator CType(path As String) As WhogRepository
            Return path.LoadXml(Of WhogRepository)()
        End Operator

        ''' <summary>
        ''' 从Whog文本文件导入COG的分类数据，然后保存为XML文件
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function [Imports](path As String) As WhogRepository
            Dim tokens As IEnumerable(Of String()) = path _
                .ReadAllLines _
                .Split("^[_]+$", True, RegexICMul) _
                .ToArray
            Dim LQuery = LinqAPI.Exec(Of Category) <=
 _
                From strToken As String()
                In tokens
                Where Not strToken.IsNullOrEmpty
                Let cat As Category = TextParser.Parse(strToken)
                Select cat
                Order By cat.COG_id

            Return New WhogRepository With {
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
                          Let assignCOG As MyvaCOG = Me.DoAssign(prot)
                          Select assignCOG).ToArray
            Return LQuery
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
