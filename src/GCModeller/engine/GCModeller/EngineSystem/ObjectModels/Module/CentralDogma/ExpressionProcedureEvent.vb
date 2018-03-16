#Region "Microsoft.VisualBasic::9b9d91e06c45b2cb1e7724cc7b0f9ec8, engine\GCModeller\EngineSystem\ObjectModels\Module\CentralDogma\ExpressionProcedureEvent.vb"

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

    '     Interface IEventUnit
    ' 
    '         Properties: MotifSites
    ' 
    '     Class ExpressionProcedureEvent
    ' 
    '         Properties: CompositionVector, Product, RegulationValue, Template
    ' 
    '         Function: CreateInstance
    '         Interface I_EventProcess
    ' 
    '             Properties: CompositionVector, Product, RegulationValue, Template
    ' 
    '             Function: InitializeConstraints, Invoke
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization

Namespace EngineSystem.ObjectModels.Module.CentralDogmaInstance

    ''' <summary>
    ''' 一个事件的单位，通常是指一个转录单元或者一条多顺反子的mRNA链，上面有多个motif
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IEventUnit(Of TEvent As ExpressionProcedureEvent)
        Property MotifSites As Feature.MotifSite(Of TEvent)()
    End Interface

    ''' <summary>
    ''' 对一个基因对象的表达过程中的一个步骤的描述
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class ExpressionProcedureEvent : Inherits SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.FluxObject
        Implements I_EventProcess

        ''' <summary>
        ''' 用于MotifSite对象和其继承类的解耦和作用的
        ''' </summary>
        ''' <remarks></remarks>
        Public Interface I_EventProcess

#Region "Public Property"

            Property Template As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Feature.BiomacromoleculeFeature.ITemplate
            Property Product As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Compound
            ''' <summary>
            ''' 这些事件都是建立在序列的基础之上的，而本属性则描述了改序列的组成
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Property CompositionVector As Integer()
            ReadOnly Property RegulationValue As Double
#End Region

#Region "Methods"

            Function Invoke() As Double
            Function InitializeConstraints(MetabolismSystem As ObjectModels.SubSystem.MetabolismCompartment) As Integer
#End Region
        End Interface

        ''' <summary>
        ''' 本步骤的信息来源：模板分子对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> <XmlElement> Public Property Template As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Feature.BiomacromoleculeFeature.ITemplate Implements I_EventProcess.Template
        <DumpNode> <XmlElement> Public Property Product As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Compound Implements I_EventProcess.Product

        ''' <summary>
        ''' 转录或者翻译过程中所需要的组分向量
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Public Property CompositionVector As Integer() Implements I_EventProcess.CompositionVector
        <DumpNode> Protected Friend ConstraintFlux As ObjectModels.Module.ExpressionConstraintFlux

        ''' <summary>
        ''' 现在先假设所有的链分子的单元延伸速度都是一样的，故而对于长度越长的分子而言，其单位时间内所合成的数量就会越少，故而在所有的转录和翻译事件之中都会除以本参数用来表示由于长度所带来的效应
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Protected Friend CompositionDelayEffect As Double
        <DumpNode> Protected Friend UPPER_BOUND As Double

        ''' <summary>
        ''' 调控因子对本过程对象的调控作用的总和大小
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Protected Friend _Regulations As Double

        ''' <summary>
        ''' 调控因子对本过程对象的调控作用的总和大小
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property RegulationValue As Double Implements I_EventProcess.RegulationValue
            Get
                Return _Regulations
            End Get
        End Property

        Public MustOverride Overrides Function Invoke() As Double Implements I_EventProcess.Invoke

        Protected Friend MustOverride Function InitializeConstraints(MetabolismSystem As ObjectModels.SubSystem.MetabolismCompartment) As Integer Implements I_EventProcess.InitializeConstraints

        ''' <summary>
        ''' 从一个RNA分子之上可以构建出一个转录过程和一个翻译过程，假若其为一个mRNA的话
        ''' </summary>
        ''' <param name="ExpressionObject"></param>
        ''' <param name="Transcript"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateInstance(ExpressionObject As CentralDogma,
                                              Transcript As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Transcript,
                                              MetabolismSystem As ObjectModels.SubSystem.MetabolismCompartment) As Transcription

            Dim TemplateGene = New ObjectModels.Feature.Gene With {.Identifier = Transcript.Identifier.Replace("-transcript", "")}

            Dim Transcription As Transcription = New Transcription With {
                .Product = Transcript, .Identifier = String.Format("{0}: {1}", ExpressionObject.TransUnit.Identifier, TemplateGene.Identifier),
                .Template = TemplateGene, .CompositionVector = Transcript._TranscriptModelBase.CompositionVector.T,
                .CompositionDelayEffect = Global.System.Math.Log10(Transcript._TranscriptModelBase.CompositionVector.T.Sum + 10)}

            Call Transcription.InitializeConstraints(MetabolismSystem)
            Return Transcription
        End Function
    End Class
End Namespace
