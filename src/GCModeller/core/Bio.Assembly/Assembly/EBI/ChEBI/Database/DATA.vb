﻿#Region "Microsoft.VisualBasic::3b210d0c80876917df2033f5e5dfc386, Bio.Assembly\Assembly\EBI\ChEBI\Database\DATA.vb"

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

    '     Module DATA
    ' 
    '         Properties: AccessionTypeNames
    ' 
    '         Function: BuildEntitysFromTsv, (+2 Overloads) GetXrefID, GetXrefIDByType, LoadNameOfDatabaseFromTsv, RegistryNumbersSearchModel
    '                   ScanLoad, SystematicName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.Database.IO.StreamProviders.Tsv
Imports SMRUCC.genomics.Assembly.EBI.ChEBI.XML

Namespace Assembly.EBI.ChEBI

    ''' <summary>
    ''' Chebi <see cref="ChEBIEntity"/> model extensions
    ''' </summary>
    Public Module DATA

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="repository$">``*.xml`` chebi online data cache</param>
        ''' <returns></returns>
        Public Function ScanLoad(repository As String) As Dictionary(Of Long, ChEBIEntity)
            Dim list As New Dictionary(Of Long, ChEBIEntity)

            For Each path As String In (ls - l - r - "*.xml" <= repository)
                Dim compound = path.LoadXml(Of ChEBIEntity)

                For Each ID As String In compound.IDlist
                    Dim int_id = CLng(Val(ID.Split(":"c).Last))

                    If Not list.ContainsKey(int_id) Then
                        list.Add(int_id, compound)
                    End If
                Next
            Next

            Return list
        End Function

        ''' <summary>
        ''' 读取从ChEBI的ftp服务器之上所下载的tsv数据表格文件然后通过链接构建出完整的分子数据模型<see cref="ChEBIEntity"/>.
        ''' </summary>
        ''' <param name="DIR$"></param>
        ''' <returns></returns>
        Public Iterator Function BuildEntitysFromTsv(DIR$) As IEnumerable(Of ChEBIEntity)

        End Function

        ''' <summary>
        ''' 从ftp文件夹之中加载数据然后构建chebi内存数据库
        ''' </summary>
        ''' <param name="DIR$"></param>
        ''' <returns></returns>
        Public Function LoadNameOfDatabaseFromTsv(DIR$) As [NameOf]
            Return New [NameOf](New TSVTables(DIR))
        End Function

        ''' <summary>
        ''' Using the **IUPAC** name as the ``Systematic Name``.
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <returns></returns>
        <Extension> Public Function SystematicName(chebi As ChEBIEntity) As String
            Dim IUPAC = chebi.IupacNames

            If IUPAC.IsNullOrEmpty Then
                Return Nothing
            End If

            For Each name As Synonyms In IUPAC
                If name.type.TextEquals("IUPAC NAME") AndAlso name.source.TextEquals("IUPAC") Then
                    Return name.data
                End If
            Next

            Return IUPAC.First.data
        End Function

        ''' <summary>
        ''' See the constant string values in <see cref="RegistryNumbers"/> or <see cref="AccessionTypeNames"/>
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <returns></returns>
        <Extension> Public Function GetXrefID(chebi As ChEBIEntity) As Func(Of String, NamedValue(Of String)())
            Dim registryNumbers = chebi.RegistryNumbersSearchModel

            Return Function(type)
                       If registryNumbers.ContainsKey(type) Then
                           Return registryNumbers(type)
                       Else
                           Return Nothing
                       End If
                   End Function
        End Function

        ''' <summary>
        ''' See the constant string values in <see cref="RegistryNumbers"/> or <see cref="AccessionTypeNames"/>
        ''' </summary>
        ''' <param name="chebi"></param>
        ''' <returns></returns>
        <Extension> Public Function GetXrefIDByType(chebi As ChEBIEntity) As Func(Of AccessionTypes, NamedValue(Of String)())
            Dim fromStringType = chebi.GetXrefID

            Return Function(type)
                       Return fromStringType(AccessionTypeNames(type))
                   End Function
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function GetXrefID(chebi As ChEBIEntity, type As AccessionTypes) As NamedValue(Of String)()
            Return chebi.GetXrefID()(AccessionTypeNames(type))
        End Function

        Public ReadOnly Property AccessionTypeNames As Dictionary(Of AccessionTypes, String) =
            Enums(Of AccessionTypes) _
            .ToDictionary(Function(key) key,
                          Function(t)
                              Return t.Description
                          End Function)

        <Extension>
        Public Function RegistryNumbersSearchModel(chebi As ChEBIEntity) As Dictionary(Of String, NamedValue(Of String)())
            Dim registryNumbers = chebi.RegistryNumbers _
                .SafeQuery _
                .GroupBy(Function(id) id.type) _
                .ToDictionary(Function(r) r.Key,
                              Function(values)
                                  Return values.Select(Function(id)
                                                           Return New NamedValue(Of String) With {
                                                               .Name = id.source,
                                                               .Value = id.data
                                                           }
                                                       End Function) _
                                               .ToArray
                              End Function)
            Dim links = chebi.DatabaseLinks _
                .SafeQuery _
                .GroupBy(Function(id) id.type)

            For Each type In links
                Dim g As NamedValue(Of String)() =
                    type _
                    .Select(Function(id)
                                Return New NamedValue(Of String) With {
                                    .Name = "",
                                    .Value = id.data
                                }
                            End Function) _
                    .ToArray
                Call registryNumbers.Add(type.Key, g)
            Next

            Return registryNumbers
        End Function
    End Module
End Namespace
