#Region "Microsoft.VisualBasic::929ad12e597051515508d66eadc63383, ..\interops\localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\COG\Whog\Whog.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput

Namespace LocalBLAST.Application.RpsBLAST.Whog

    ''' <summary>
    ''' Cog Category
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Whog : Inherits ITextFile

        <XmlElement> Public Property Categories As Category()
            Get
                Return _COGCategory
            End Get
            Set(value As Category())
                _COGCategory = value
                If value.IsNullOrEmpty Then
                    _dictCategory = New Dictionary(Of String, Category)
                Else
                    _dictCategory = value.ToDictionary(Function(x) x.CogId)
                End If
            End Set
        End Property

        Dim _COGCategory As Category()
        Dim _dictCategory As Dictionary(Of String, Category)

        Public Function FindByCogId(CogId As String) As Category
            If _dictCategory.ContainsKey(CogId) Then
                Return _dictCategory(CogId)
            Else
                Return Nothing
            End If
        End Function

        Public Overloads Shared Widening Operator CType(path As String) As Whog
            Return path.LoadTextDoc(Of Whog)()
        End Operator

        Public Shared Function [Imports](path As String) As Whog
            Dim tokens As String() = Strings.Split(FileIO.FileSystem.ReadAllText(path), vbLf & "_______" & vbLf & vbLf)
            Dim LQuery = (From strToken As String In tokens.AsParallel
                          Where Not String.IsNullOrEmpty(strToken)
                          Let item As Category = Category.Parse(Trim(strToken))
                          Select item
                          Order By item.CogId).ToArray
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
            Dim LQuery = (From prot As MyvaCOG In MatchedData.AsParallel
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
                Call $"Could Not found the COG category id for myva cog {prot.QueryName} <-> {prot.MyvaCOG}....".__DEBUG_ECHO
                Return prot
            End If

            prot.COG = Cog.CogId
            prot.Category = Cog.CategoryId
            prot.Description = Cog.Description

            Return prot
        End Function

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Return Me.GetXml.SaveTo(getPath(FilePath), getEncoding(Encoding))
        End Function
    End Class
End Namespace
