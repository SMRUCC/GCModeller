#Region "Microsoft.VisualBasic::1263dbce136a91c3d619f395ecb50313, engine\GCModeller\EngineSystem\ObjectModels\Module\CentralDogma\CentralDogma.vb"

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

    '     Class CentralDogma
    ' 
    '         Properties: ExpressionActivity, FluxValue, MotifSites, TransUnit, TypeId
    ' 
    '         Function: __Internal_createMotifRegulators, CreateInstance, get_Regulators, Initialize, Invoke
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly

Namespace EngineSystem.ObjectModels.Module.CentralDogmaInstance

    ''' <summary>
    ''' ExpressionObject object equals to the 
    ''' <see cref="GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit">TranscriptUnit object</see> 
    ''' in the datamodels.
    ''' 一个转录对象是以一个转录单元为单位的，其可以被看作为中心法则的一个实例，描述了从基因到蛋白质的整个表达过程，一个操纵子对象的表达过程
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CentralDogma : Inherits ModellingEngine.EngineSystem.ObjectModels.Module.FluxObject
        Implements Microsoft.VisualBasic.ComponentModel.IAddressOf

#Region "Object Property"

        <DumpNode> <XmlElement> Public Property TransUnit As ObjectModels.Feature.TransUnit

        ''' <summary>
        ''' 处于表达活性状态的转录单元对象
        ''' </summary>
        ''' <remarks>
        ''' 1、对于大量细胞而言，此变量的值指的是在这些细胞之中，处于激活状态的基因的数目
        ''' 2、对于单个细胞而言，此变量的值指的是在一段时间内，该基因处于转录激活状态的时间长度
        ''' </remarks>
        <DumpNode> <XmlAttribute> Public Property ExpressionActivity As Double
            Get
                Return TransUnit.ExpressionActivity
            End Get
            Set(value As Double)
                TransUnit.ExpressionActivity = value
            End Set
        End Property

        ''' <summary>
        ''' 转录单元所转录出来的RNA分子
        ''' </summary>
        ''' <remarks></remarks>
        <DumpNode> Public Transcripts As ObjectModels.Entity.Transcript()
        <DumpNode> Protected Friend Transcriptions As Transcription()

        ''' <summary>
        ''' 对当前的这个中心法则处理步骤过程的转录过程起调控作用的<see cref="Feature.MotifSite"></see>的列表，这个仅仅是对基因的转录调控而言的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> <XmlElement> Public ReadOnly Property MotifSites As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Feature.MotifSite(Of Transcription)()
            Get
                Return TransUnit.MotifSites
            End Get
        End Property

        <DumpNode> Protected Friend PromoterGene As EngineSystem.ObjectModels.Feature.Gene
        Dim _FluxValue As Double

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.CentralDogma
            End Get
        End Property

        Public Overrides ReadOnly Property FluxValue As Double
            Get
                Return _FluxValue
            End Get
        End Property
#End Region

        ''' <summary>
        ''' 将<see cref="MotifSites"></see>之中的调控因子对象集合输出
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function get_Regulators() As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Entity.Regulator(Of Transcription)()
            Return (From item In MotifSites Select item.Regulators).ToArray.ToVector
        End Function

        ''' <summary>
        ''' 有多个调控因子的时候的表达的计算公式
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 算法要点
        ''' 
        ''' 0. 对于所有随机试验低于阈值的调控事件，都默认为不激活(即，没有调控因子的激活的话，基因不表达)
        ''' 1. 对于同一个位点之上，假若激活的数目多余抑制的数目，则激活的权重比较大，该位点计算为激活的可能性比较高(假设转录组数据是建立在大细胞宗系的条件之下测定的)
        ''' 2. 在Promoter区之内，假若任意一个位点被抑制，则整个基因的表达过程被抑制(单纯的分子动力学行为)
        ''' </remarks>
        Public Overrides Function Invoke() As Double
            If MotifSites.IsNullOrEmpty Then
                Return 1 '没有任何调控因子，则值为恒定值(即所设定的初始值)
            End If

            Dim ChunkBuffer As Double() = (From MotifSite In Me.MotifSites Let value = MotifSite.get_RegulationEffect Where value > 0 Select value).ToArray '.Sum / (Me.VEC.Count + 1)

            '在这里面我将表达调控作用看作为一种简单的加和过程，当V小于零的时候可能有两种情况：
            '1.完全的负调控
            '2.正调控作用被负调控作用所淹没

            If ChunkBuffer.Count < MotifSites.Count Then '当其中的任意一个位点被抑制，则无法行使功能
                _FluxValue = 0 '故而负值的时候不表达，但是程序会在其他的地方产生本地表达
                Return 0
            Else
                _FluxValue = ChunkBuffer.Sum
            End If

            Dim RegulationValue As Double = _FluxValue '这个值相当于调控作用的强弱

            For Each [Event] In Transcriptions
                [Event]._Regulations = RegulationValue
                Call [Event].Invoke()
            Next

            Return FluxValue
        End Function

        ''' <summary>
        ''' 生成Transcription和Translation对象
        ''' </summary>
        ''' <param name="CellSystem">初始化所需要用到的数据源</param>
        ''' <returns></returns> 
        ''' <remarks>根据TransUnit中的基因列表指针来生成mRNA列表并添加进入代谢组之中</remarks>
        Public Overloads Function Initialize(CellSystem As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.CellSystem) As CentralDogma

            Dim LQuery = (From Transcript In Me.Transcripts Select ExpressionProcedureEvent.CreateInstance(Me, Transcript, CellSystem.Metabolism)).ToArray ' '从每一个转录组分对象实例生成转录过程对象


            Dim TF_Motifs = __Internal_createMotifRegulators(Me.TransUnit.FeatureBaseType, CellSystem)
            Me.Transcriptions = (From TranscriptionEvent In LQuery Where Not TranscriptionEvent Is Nothing Select TranscriptionEvent).ToArray
            Me.TransUnit.MotifSites = TF_Motifs

            Return Me
        End Function

        Private Shared Function __Internal_createMotifRegulators(TU_Model As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit,
                                                                 CellSystem As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.CellSystem) _
            As Feature.MotifSite(Of Transcription)()

            Dim MotifModels = (From item In TU_Model.RegulatedMotifs Where Not item Is Nothing Select item).ToArray

            If MotifModels.IsNullOrEmpty OrElse TU_Model.get_Regulators.IsNullOrEmpty Then
                Return New Feature.MotifSite(Of Transcription)() {}
            End If

            Dim Motifs = (From T_Motif As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.MotifSite
                   In MotifModels
                          Let motif = New Feature.MotifSite(Of Transcription) With
                               {
                                   .Identifier = T_Motif.MotifName,
                                   .Regulators = (From regulator As GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator
                                                  In T_Motif.Regulators
                                                  Select ObjectModels.Entity.Regulator(Of Transcription).CreateObject(regulator, CellSystem.Metabolism)).ToArray}
                          Select motif.Initialize).ToArray
            Return Motifs
        End Function

        ''' <summary>
        ''' 创建转录单元对象以及将蛋白质单体与相应的基因进行连接
        ''' </summary>
        ''' <param name="TransUnit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Shadows Function CreateInstance(TransUnit As GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.TranscriptUnit,
                                                      CellSystem As SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.CellSystem) _
            As CentralDogma

            Return New CentralDogma With {
                .TransUnit = SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Feature.TransUnit.CreateObject(TransUnit, CellSystem),
                .Identifier = TransUnit.Identifier,
                .PromoterGene = New EngineSystem.ObjectModels.Feature.Gene With {.Identifier = TransUnit.PromoterGene.Identifier}}
        End Function

        Public Overrides Function ToString() As String
            Return TransUnit.ToString
        End Function
    End Class
End Namespace
