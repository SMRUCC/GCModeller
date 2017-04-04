#Region "Microsoft.VisualBasic::67b05e9d6961900df35f577eb8de8cd1, ..\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\ModsBrite.vb"

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

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' 加载代谢途径或者KEGG Modules的Brite文档的模块
    ''' </summary>
    ''' <typeparam name="TMod">
    ''' <see cref="bGetObject.Pathway"/> or <see cref="bGetObject.Module"/>
    ''' </typeparam>
    Public Class ModsBrite(Of TMod As ComponentModel.PathwayBrief)

        Sub New(Optional res As String = "")
            If GetType(TMod).Equals(GetType(bGetObject.Module)) Then
                __hash = __modsBrite(res)
            ElseIf GetType(TMod).Equals(GetType(bGetObject.Pathway)) Then
                __hash = __pathwaysBrite(res)
            Else
                Throw New Exception(GetType(TMod).FullName & " is not a valid type!")
            End If
        End Sub

        Private Shared Function __modsBrite(res As String) As Dictionary(Of String, TripleKeyValuesPair)
            Dim hash As Dictionary(Of String, [Module])
            If res.FileExists Then
                hash = [Module].GetDictionary(res)
            Else
                hash = [Module].GetDictionary
            End If

            Return hash.ToDictionary(Function(x) x.Key, AddressOf __toValue)
        End Function

        Private Shared Function __toValue(x As KeyValuePair(Of String, [Module])) As TripleKeyValuesPair
            Return New TripleKeyValuesPair(x.Value.Class, x.Value.Category, x.Value.SubCategory)
        End Function

        Private Shared Function __toValue(x As KeyValuePair(Of String, Pathway)) As TripleKeyValuesPair
            Return New TripleKeyValuesPair(x.Value.Class, x.Value.Category, x.Value.Entry.Value)
        End Function

        Private Shared Function __pathwaysBrite(res As String) As Dictionary(Of String, TripleKeyValuesPair)
            Dim hash As Dictionary(Of String, Pathway)
            If res.FileExists Then
                hash = Pathway.LoadDictionary(res)
            Else
                hash = Pathway.LoadDictionary
            End If

            Return hash.ToDictionary(Function(x) x.Key, AddressOf __toValue)
        End Function

        ReadOnly __hash As Dictionary(Of String, TripleKeyValuesPair)

        ''' <summary>
        ''' A
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Overloads Function [GetType](x As TMod) As String
            Dim name As String = x.BriteId
            If __hash.ContainsKey(name) Then
                Return __hash(name).Key
            Else
                Return ""
            End If
        End Function

        ''' <summary>
        ''' B
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Function GetClass(x As TMod) As String
            Dim name As String = x.BriteId
            If __hash.ContainsKey(name) Then
                Return __hash(name).Value1
            Else
                Return ""
            End If
        End Function

        ''' <summary>
        ''' B
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Function GetCategory(x As TMod) As String
            Dim name As String = x.BriteId
            If __hash.ContainsKey(name) Then
                Return __hash(name).Value2
            Else
                Return ""
            End If
        End Function
    End Class
End Namespace
