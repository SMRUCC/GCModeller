#Region "Microsoft.VisualBasic::c532e88c4f3d9f1d61f00098002e4a22, engine\GCModeller\EngineSystem\Engine\MathematicsModels\EnzymeKinetics\pH_Tempratures.vb"

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

    '     Class pH_Tempratures
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Get_currentPH
    ' 
    '     Class EnzymeCatalystKineticLaw
    ' 
    '         Properties: pH_Saturated, Temperature_Saturated
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: [New]
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Data.SabiorkKineticLaws
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem

Namespace EngineSystem.MathematicsModels.EnzymeKinetics

    ''' <summary>
    ''' 获取得到了PH值和温度之后，在这里计算出酶活性的变化
    ''' </summary>
    ''' <remarks></remarks>
    Public Class pH_Tempratures : Inherits CompartmentAccessories

        Dim H As EngineSystem.ObjectModels.Entity.Compound

        Public get_CultivationMediumsTemperature As Func(Of Double)

        Const PH_LOG_BASE As Double = 9.8

        Sub New(EnviromentCompartment As ICompartmentObject)
            Call MyBase.New(EnviromentCompartment)
            Dim Id As String = EnviromentCompartment.get_runtimeContainer.SystemVariable(SystemVariables.ID_PROTON)
            H = EnviromentCompartment.Metabolites.GetItem(Id)
            get_CultivationMediumsTemperature = AddressOf DirectCast(EnviromentCompartment.get_runtimeContainer, Engine.GCModeller).CultivatingMediums.Get_currentTemperature

            '根据设置中的初始ph值计算出H+的浓度，所有计算模型中的H+对象的浓度总是会在这里被复写的
            H.Quantity = PH_LOG_BASE ^ (EnviromentCompartment.get_runtimeContainer.ConfigurationData.Initial_pH / 2)  'System.Math.E ^ (-EnviromentCompartment.Get_runtimeContainer.ConfigurationData.Initial_pH)
        End Sub

        ''' <summary>
        ''' A Big problem in the ph calculation
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Get_currentPH() As Double
            Return Global.System.Math.Log(H.DataSource.value + PH_LOG_BASE, PH_LOG_BASE) * 2 + 1.5
            '#If DEBUG Then
            '            Return 7
            '#End If
            '            If H.Quantity < 0 Then
            '                Call LoggingClient.WriteLine(String.Format("[H+] -> {0}, pH calculation error!", H.Quantity), "", Type:=Logging.MSG_TYPES.ERR)
            '                H.Quantity = 10 ^ -14
            '            End If
            '            Return -System.Math.Log10(H.DataSource.Value) / 2
            '    Return Me._CompartmentObject.Get_runtimeContainer.ConfigurationData.Initial_pH
        End Function
    End Class

    Public Class EnzymeCatalystKineticLaw : Inherits TabularDump.EnzymeCatalystKineticLaw
        Public Property pH_Saturated As Double
        Public Property Temperature_Saturated As Double

        Protected Sub New()
        End Sub

        Public Shared Function [New](Base As TabularDump.EnzymeCatalystKineticLaw) As EnzymeCatalystKineticLaw
            Dim obj = New EnzymeCatalystKineticLaw
            obj.PH = Base.PH
            obj.Temperature = Base.Temperature
            obj.Km = Base.Km
            obj.Kcat = Base.Kcat
            obj.KEGGReactionId = Base.KEGGReactionId
            obj.KineticRecord = Base.KineticRecord
            obj.Metabolite = Base.Metabolite
            obj.Enzyme = Base.Enzyme
            obj.Buffer = Base.Buffer
            obj.Ec = Base.Ec
            obj.KEGGCompoundId = Base.KEGGCompoundId

            obj.pH_Saturated = MichaelisMenten.pH_Factor(obj, Base.PH)
            obj.Temperature_Saturated = MichaelisMenten.T_Factor(obj, Base.Temperature)
            Return obj
        End Function
    End Class
End Namespace
