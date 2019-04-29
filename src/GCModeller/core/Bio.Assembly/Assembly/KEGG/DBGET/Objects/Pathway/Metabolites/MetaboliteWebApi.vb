#Region "Microsoft.VisualBasic::c700a8a1bd5086f8d9aec1139103a04f, Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Metabolites\MetabolitesDBGet.vb"

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

    '     Module MetaboliteDBGET
    ' 
    '         Function: __parseNamedData, DownloadCompound, DownloadCompoundFrom, FetchTo, GetCommonNames
    '                   GetDBLinks, LoadCompoundObject, MatchByName, ParseCompound, ScanLoad
    '                   TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Module MetaboliteWebApi

        <Extension>
        Public Function MatchByName(compound As Compound, name$) As Boolean
            For Each s In compound.CommonNames.SafeQuery
                If s.TextEquals(name) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Const URL = "http://www.kegg.jp/dbget-bin/www_bget?cpd:{0}"

        ''' <summary>
        ''' 使用KEGG compound的编号来下载代谢物数据
        ''' </summary>
        ''' <param name="ID">``cpd:ID``</param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function DownloadCompound(ID As String) As Compound
            Return WebQuery.Compounds.DownloadCompoundFrom(url:=String.Format(URL, ID))
        End Function

        ''' <summary>
        ''' 下载指定编号集合的代谢物数据，并保存到指定的文件夹之中
        ''' </summary>
        ''' <param name="list">KEGG compound id list</param>
        ''' <param name="EXPORT"></param>
        ''' <returns>返回下载失败的对象的编号列表</returns>
        ''' <remarks></remarks>
        Public Function FetchTo(list As String(), EXPORT As String) As String()
            Dim failures As New List(Of String)
            Dim path$

            Call $"{list.Length} KEGG compounds are going to download!".__DEBUG_ECHO

            For Each cpdID As String In list

                path = String.Format("{0}/{1}.xml", EXPORT, cpdID)

                If Not path.FileExists Then
                    Dim CompoundData As Compound = DownloadCompound(cpdID)

                    If CompoundData Is Nothing Then
                        failures += cpdID
                    Else
                        Call CompoundData.GetXml.SaveTo(path)
                    End If
                End If
            Next

            Return failures
        End Function

        ''' <summary>
        ''' 请注意，这个函数仅会根据文件名的前缀来判断类型
        ''' </summary>
        ''' <param name="xml">The file path of the compound object</param>
        ''' <returns></returns>
        <Extension>
        Public Function LoadCompoundObject(xml As String) As Compound
            Dim ID$ = xml.BaseName

            If ID.First = "G"c Then
                Return xml.LoadXml(Of Glycan)(stripInvalidsCharacter:=True)
            Else
                Return xml.LoadXml(Of Compound)(stripInvalidsCharacter:=True)
            End If
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ScanLoad(repository$) As IEnumerable(Of Compound)
            Return (ls - l - r - "*.Xml" <= repository).Select(AddressOf LoadCompoundObject)
        End Function
    End Module
End Namespace
