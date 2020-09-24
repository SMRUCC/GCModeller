#Region "Microsoft.VisualBasic::8c2ec6b3c64e232ffbaa85b7c3a1ef65, engine\GCModeller\EngineSystem\ObjectModels\Entity\Transcript.vb"

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

    '     Class Transcript
    ' 
    '         Properties: DataSource, Identifier, ModelId, Product, Quantity
    '                     TypeId
    ' 
    '         Function: CreateInstance, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.ObjectModels.Entity

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>(当某一种模板分子有多种产物的时候，则会生成多个目标模板对象)</remarks>
    Public Class Transcript : Inherits EngineSystem.ObjectModels.Entity.IDisposableCompound
        Implements EngineSystem.ObjectModels.Feature.BiomacromoleculeFeature.ITemplate

        Protected Friend _TranscriptModelBase As GCML_Documents.XmlElements.Bacterial_GENOME.Transcript

        ''' <summary>
        ''' 指向代谢组中的组分的对象句柄
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> <XmlAttribute> Public Property Product As String Implements Feature.BiomacromoleculeFeature.ITemplate.Products
            Get
                Return _TranscriptModelBase.Product
            End Get
            Set(value As String)
                _TranscriptModelBase.Product = value
            End Set
        End Property

        <DumpNode> Public ProductMetabolite As EngineSystem.ObjectModels.Entity.Compound

        ''' <summary>
        ''' 本对象在数据模型中的编号属性【对于转录组分而言，其指回生成该转录组分的基因对象】
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> <XmlAttribute> Public Property ModelId As String

        <DumpNode> <XmlAttribute> Public Overrides Property Quantity As Double Implements Feature.BiomacromoleculeFeature.ITemplate.Quantity
            Get
                Return EntityBaseType.Quantity
            End Get
            Set(value As Double)
                EntityBaseType.Quantity = value
            End Set
        End Property

        <DumpNode> Public Overrides Property Identifier As String Implements Feature.BiomacromoleculeFeature.ITemplate.locusId
            Get
                Return MyBase.Identifier
            End Get
            Set(value As String)
                MyBase.Identifier = value
            End Set
        End Property

        Public Overrides ReadOnly Property DataSource As DataSource
            Get
                Return New DataSource(Handle, EntityBaseType.DataSource.value)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", ModelId, Me.EntityBaseType.ToString)
        End Function

        Public Shared Function CreateInstance(DataModel As GCML_Documents.XmlElements.Bacterial_GENOME.Transcript, Metabolites As IEnumerable(Of Compound)) As Transcript
            Dim Metabolite = Metabolites.GetItem(DataModel.Identifier)
            Dim Transcript = New Transcript With {
 _
                ._TranscriptModelBase = DataModel,
                .Identifier = Metabolite.Identifier,
                .ModelId = DataModel.Template,
                .EntityBaseType = Metabolite,
                .ModelBaseElement = Metabolite.ModelBaseElement,
                .ProductMetabolite = Metabolites.GetItem(DataModel.Product),
                .Lamda = DataModel.Lamda,
                .CompositionVector = DataModel.CompositionVector.T}

            Return Transcript
        End Function

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.EntityTranscript
            End Get
        End Property
    End Class
End Namespace
