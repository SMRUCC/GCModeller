﻿#Region "Microsoft.VisualBasic::73cf4bd6e514e046b65dc06781f1d0d7, IO\GCMarkupLanguage\FBA\Model.vb"

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

    '     Class Model
    ' 
    '         Properties: Height, MAT, MetabolismNetwork, Metabolites, Reactions
    '                     Width
    ' 
    '         Function: Load, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Extensions
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.FLuxBalanceModel
Imports SMRUCC.genomics.Model.SBML.Level2.Elements

Namespace FBACompatibility

    ''' <summary>
    ''' 这是一个已经编译好的FBA模型文件，所有基于FBA模型的方法所使用的模型数据定义于此对象之中
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Model : Inherits ITextFile
        Implements I_FBAC2(Of speciesReference)
        Implements ISaveHandle

        ''' <summary>
        ''' 反应对象的UniqueID值列表
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray("Flux")> Public Property Reactions As MetabolismFlux()

        ''' <summary>
        ''' 编译好的代谢组矩阵
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlArray> Public Property MAT As Vector()

        Public Shared Function Load(File As String) As Model
            Dim Model As Model = File.LoadXml(Of FBACompatibility.Model)()
            Model.FilePath = File
            Return Model
        End Function

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Text.Encoding = Nothing) As Boolean Implements ISaveHandle.Save
            If String.IsNullOrEmpty(FilePath) Then
                FilePath = Me.FilePath
            End If
            Return Me.GetXml.SaveTo(FilePath, Encoding)
        End Function

        Public ReadOnly Property Height As Integer Implements FLuxBalanceModel.I_FBAC2(Of speciesReference).Height
            Get
                Return MAT.Count
            End Get
        End Property

        Public ReadOnly Property MetabolismNetwork As IEnumerable(Of FLuxBalanceModel.I_ReactionModel(Of speciesReference)) Implements FLuxBalanceModel.I_FBAC2(Of speciesReference).MetabolismNetwork
            Get
                Return Reactions
            End Get
        End Property

        Public ReadOnly Property Metabolites As IEnumerable(Of FLuxBalanceModel.IMetabolite) Implements FLuxBalanceModel.I_FBAC2(Of speciesReference).Metabolites
            Get
                Return MAT
            End Get
        End Property

        Public ReadOnly Property Width As Integer Implements FLuxBalanceModel.I_FBAC2(Of speciesReference).Width
            Get
                Return Reactions.Count
            End Get
        End Property
    End Class
End Namespace
