#Region "Microsoft.VisualBasic::902ce0dc9a0c0fc8c192faf56af74e15, GCModeller\engine\IO\GCMarkupLanguage\FBA\Model.vb"

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

    '   Total Lines: 72
    '    Code Lines: 45
    ' Comment Lines: 16
    '   Blank Lines: 11
    '     File Size: 2.74 KB


    '     Class Model
    ' 
    '         Properties: Height, MAT, MetabolismNetwork, Metabolites, Reactions
    '                     Width
    ' 
    '         Function: Load, (+2 Overloads) Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.FLuxBalanceModel
Imports SMRUCC.genomics.Model.SBML.Level2.Elements

Namespace FBACompatibility

    ''' <summary>
    ''' 这是一个已经编译好的FBA模型文件，所有基于FBA模型的方法所使用的模型数据定义于此对象之中
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Model
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
            Return Model
        End Function

        Public Function Save(FilePath As String, Encoding As Text.Encoding) As Boolean Implements ISaveHandle.Save
            Return Me.GetXml.SaveTo(FilePath, Encoding)
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
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
