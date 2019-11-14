#Region "Microsoft.VisualBasic::08ea328d22dcfc5edd02db4d1ef97958, engine\GCModeller\EngineSystem\ObjectModels\Entity\Regulator.vb"

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

    '     Class Regulator
    ' 
    '         Properties: DataSource, Identifier, Quantity, RegulatesTarget, RegulateValue
    '                     SerialsHandle, TypeId, Weight
    ' 
    '         Function: CreateObject, get_RegulatorValue, ToString
    ' 
    '         Sub: set_RegulatesMotifSite
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Serialization
Imports SMRUCC.genomics.GCModeller.Assembly
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.DataSerializer
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Services.DataAcquisition.Services

Namespace EngineSystem.ObjectModels.Entity

    ''' <summary>
    ''' 对表达过程其调控作用的生物大分子，在数据模型之中，调控因子是一个调控因子对多个调控对象的，当被转换为对象模型之后，
    ''' 则变成了一个调控因子对一个调控位点或者调控对象，即在创建对象的时候，调控因子被按照调控位点进行复制拆分
    ''' </summary>
    ''' <typeparam name="TEvent"><see cref="ObjectModels.[Module].CentralDogmaInstance.Transcription"></see>或者
    ''' <see cref="ObjectModels.[Module].CentralDogmaInstance.Translation"></see>过程事件</typeparam>
    ''' <remarks></remarks>
    Public Class Regulator(Of TEvent As ObjectModels.Module.CentralDogmaInstance.ExpressionProcedureEvent.I_EventProcess) : Inherits Compound

        Protected Friend RegulatorBaseType As GCML_Documents.XmlElements.SignalTransductions.Regulator

        ''' <summary>
        ''' 这个调控因子的分子数量
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> <XmlAttribute> Public Overrides Property Quantity As Double
            Get
                Return MyBase.EntityBaseType.Quantity
            End Get
            Set(value As Double)
                MyBase.EntityBaseType.Quantity = value
            End Set
        End Property

        ''' <summary>
        '''  
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> Public ReadOnly Property RegulateValue As Double
            Get
                Dim i As Double = Rnd()
                Dim w As Double = get_RegulatorValue()

                If w = 0.0R Then
                    w = 1
                Else
                    w = Me.Quantity / w
                End If

                Dim n As Double = (1 - w) * _Abs_Weight  '数量越多，则概率事件越容易发生；权重越高，则概率事件越容易发生
                '假若权重大于1，则表示过量表达，则该事件总是会发生的

                If i > n Then
                    Return Quantity * _Weight  '该事件发生了
                Else
                    Return 0  '该事件没有发生
                End If
            End Get
        End Property

        Public Overrides ReadOnly Property DataSource As DataSource
            Get
                Return New DataSource(Handle, RegulateValue)
            End Get
        End Property

        Public Overrides ReadOnly Property SerialsHandle As HandleF
            Get
                Return New HandleF With {.Identifier = DEBUG_TagId, .Handle = Handle}
            End Get
        End Property

        ''' <summary>
        ''' 调控因子对象是被寄生在本目标对象之上的
        ''' </summary>
        ''' <remarks></remarks>
        Dim _InteractionTarget As TEvent
        ''' <summary>
        ''' 这个权重值决定了一个调控事件的发生的概率值的高低
        ''' </summary>
        ''' <remarks></remarks>
        Dim _Weight, _Abs_Weight As Double
        Dim _RegulatesMotifSite As ObjectModels.Feature.MotifSite(Of TEvent)

        ''' <summary>
        ''' 由Pcc计算而来，正负调控效果包含于Weight的符号之中了
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> Public Property Weight As Double
            Get
                Return _Weight
            End Get
            Set(value As Double)
                _Weight = value
                _Abs_Weight = 1 - Math.Abs(value)
            End Set
        End Property

        <DumpNode> <XmlAttribute> Public Overrides Property Identifier As String
            Get
                Return RegulatorBaseType.Identifier
            End Get
            Set(value As String)
                RegulatorBaseType.Identifier = value
            End Set
        End Property

        Public ReadOnly Property RegulatesTarget As String
            Get
                Return _RegulatesMotifSite.Identifier
            End Get
        End Property

        Public Overrides ReadOnly Property TypeId As ObjectModel.TypeIds
            Get
                Return TypeIds.EntityRegulator
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("{0}  =======> {1};   @site: {2}", MyBase.ToString, _InteractionTarget, RegulatesTarget)
        End Function

        Public Sub set_RegulatesMotifSite(site As Feature.MotifSite(Of TEvent))
            Me._RegulatesMotifSite = site
        End Sub

        ''' <summary>
        ''' 获取当前的这个调控因子所调控的目标位点上面的所有调控因子的数量的总和
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function get_RegulatorValue() As Double
            Return (From item In _RegulatesMotifSite.Regulators Select item.Quantity).ToArray.Sum
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ModelBase"></param>
        ''' <param name="Metabolism"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overloads Shared Function CreateObject(ModelBase As GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator,
                                                      Metabolism As ObjectModels.SubSystem.MetabolismCompartment) _
            As Regulator(Of TEvent)

            Dim RegulatorObject As Regulator(Of TEvent) = New Regulator(Of TEvent) With {.RegulatorBaseType = ModelBase}
            Dim MetaboliteBases = Metabolism.Metabolites.GetItem(ModelBase.Identifier)

            If MetaboliteBases Is Nothing Then
                Call LoggingClient.WriteLine(String.Format("Regulator {0} metabolite base is nothing!", ModelBase.Identifier), "", Type:=MSG_TYPES.ERR)
                Call LoggingClient.WriteLine(String.Format("Try to fix this error: insert new metabolite {0} into the metabolism system!", ModelBase.Identifier), "", Type:=MSG_TYPES.WRN)
                MetaboliteBases = Metabolism.InsertNewMetabolite(ModelBase.Identifier)
                If Not MetaboliteBases Is Nothing Then
                    Call LoggingClient.WriteLine(String.Format("ERROR for the metabolite {0} is empty fixed successfully!", ModelBase.Identifier), "", Type:=MSG_TYPES.INF)
                End If
            End If

            RegulatorObject.Identifier = ModelBase.Identifier
            RegulatorObject.EntityBaseType = MetaboliteBases
            RegulatorObject.ModelBaseElement = MetaboliteBases.ModelBaseElement

            Return RegulatorObject
        End Function
    End Class
End Namespace
