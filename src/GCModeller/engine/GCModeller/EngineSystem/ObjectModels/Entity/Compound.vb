#Region "Microsoft.VisualBasic::dccec270468bfe02ed23abe23149ab95, engine\GCModeller\EngineSystem\ObjectModels\Entity\Compound.vb"

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

    '     Class Compound
    ' 
    '         Properties: DataSource, DEBUG_TagId, Identifier, PBS_MMF_DATA, Quantity
    '                     SerialsHandle, TypeId
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: (+2 Overloads) CreateObject, SetFluxValue, ToString
    ' 
    '         Sub: set_Tag
    ' 
    '     Class IDisposableCompound
    ' 
    '         Properties: CompositionVector, Lamda
    '         Enum DisposableCompoundTypes
    ' 
    '             Polypeptide, Transcripts
    ' 
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.GridPBS
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.ObjectModels.Entity

    ''' <summary>
    ''' Entity basetype in GCModeller ObjectModels.(GCModeller计算引擎之中的对象模型Entity类型的基类，本类型对象是整个系统的的运行基础，也可以认为生命是建立在Compound这种物质基础上的相互作用)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Compound : Inherits ObjectModel
        Implements IAddressOf
        Implements INamedValue
        Implements IDataSourceEntity

        <DumpNode>
        Protected Friend ModelBaseElement As GCML_Documents.XmlElements.Metabolism.Metabolite

        ''' <summary>
        ''' 对于细胞内的一种Entity物质实体而言，其在具有其他的功能的同时，自身也是一种代谢物，本属性表示该物质的Entity的基本的代谢物属性
        ''' </summary>
        ''' <remarks>不要在这个位置添加DumpNode标记，会出现无限递归</remarks>
        Protected Friend EntityBaseType As Compound
        ''' <summary>
        ''' 利用到本代谢底物的<see cref="EngineSystem.ObjectModels.[Module].FluxObject">代谢流对象</see>的总数，即消耗掉本代谢物的流对象
        ''' </summary>
        ''' <remarks></remarks>
        Protected _n_AssociatedFluxObjects As Integer = 1

        ''' <summary>
        ''' 实际浓度
        ''' </summary>
        ''' <remarks></remarks>
        Protected _Quantity As Double

        Protected _DEBUG_TagId As String

        ''' <summary>
        ''' The quantity amount of this entity object instance in the system.(本实体对象在系统内的数量，在本属性中，返回的是实际浓度与本对象相关的流对象的数目的商，假若需要得到实际浓度的话，请使用<see cref="DataSource"></see>属性，
        ''' 进行这种处理的原因是由于在实际的细胞过程之中，流对象之间都是平行发生的，并且使用本操作也可以用来表示代谢物的在整个空间范围内的均匀分布)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute>
        <DumpNode>
        Public Overridable Property Quantity As Double
            Get
                Return _Quantity / _n_AssociatedFluxObjects
            End Get
            Set(value As Double)
                _Quantity = value
            End Set
        End Property

        Public ReadOnly Property DEBUG_TagId As String
            Get
                Return _DEBUG_TagId
            End Get
        End Property

        ''' <summary>
        ''' 获取一个PBS系统数据交换对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property PBS_MMF_DATA As MetaboliteCompound
            Get
                Return New MetaboliteCompound With {
                    .Handle = Handle,
                    .Quantity = _Quantity,
                    .Identifier = Identifier
                }
            End Get
        End Property

        Public Sub set_Tag(strValue As String)
            _DEBUG_TagId = strValue
        End Sub

        Public Sub New()
        End Sub

        Public Overridable ReadOnly Property DataSource As DataSource Implements IDataSourceEntity.DataSource
            Get
                Return New DataSource(Handle, _Quantity)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]   {1}. {2} --> {3}/mmol", DEBUG_TagId, Handle, Identifier, Quantity)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="rate">净生成速率</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SetFluxValue(rate As Double) As Double
            _Quantity += rate
            Return rate
        End Function

        Public Shared Function CreateObject(UniqueId As String, initQuantity As Double, numOfAssociatedFlux As Integer) As Compound
            Dim Compound As Compound = New Compound With {
                .ModelBaseElement = New GCML_Documents.XmlElements.Metabolism.Metabolite With {
                    .Identifier = UniqueId,
                    .InitialAmount = initQuantity,
                    .NumOfFluxAssociated = numOfAssociatedFlux
                }
            }
            Compound.Identifier = UniqueId
            Compound.Quantity = initQuantity
            Compound.EntityBaseType = Compound   '????无限递归？
            Compound._n_AssociatedFluxObjects = Math.Log(numOfAssociatedFlux + 1)

            If Compound._n_AssociatedFluxObjects < 1 Then
                Compound._n_AssociatedFluxObjects = 1
            End If

            Return Compound
        End Function

        Public Shared Function CreateObject(Metabolite As GCML_Documents.XmlElements.Metabolism.Metabolite, TagId As String) As Compound
            Dim Compound As Compound = New Compound With {.ModelBaseElement = Metabolite}

            Compound.Identifier = Metabolite.Identifier
            Compound.Quantity = Metabolite.InitialAmount
            Compound.EntityBaseType = Compound   '????无限递归？
            Compound._n_AssociatedFluxObjects = Math.Log(Metabolite.NumOfFluxAssociated + 1)
            Compound._DEBUG_TagId = TagId

            If Compound._n_AssociatedFluxObjects < 1 Then
                Compound._n_AssociatedFluxObjects = 1
            End If

            Return Compound
        End Function

        Public Shared Narrowing Operator CType(Compound As Compound) As Double
            Return Compound.DataSource.value
        End Operator

        Public Overrides Property Identifier As String Implements INamedValue.Key

        Public Overridable ReadOnly Property SerialsHandle As HandleF Implements IDataSourceEntity.SerialsHandle
            Get
                Return New HandleF With {
                    .Handle = Handle,
                    .Identifier = Identifier
                }
            End Get
        End Property

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.EntityCompound
            End Get
        End Property
    End Class

    Public MustInherit Class IDisposableCompound : Inherits Compound
        ''' <summary>
        ''' 降解系数，取值为0-1之间，当值为1的时候，表示不可降解，值越小，降解得越快
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> Public Property Lamda As Double
        <DumpNode> Public Property CompositionVector As Integer()

        Public Enum DisposableCompoundTypes
            Transcripts
            Polypeptide
        End Enum
    End Class
End Namespace
