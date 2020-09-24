#Region "Microsoft.VisualBasic::161022478fecd726bb7c7bfce93c7c13, engine\GCModeller\EngineSystem\Engine\MathematicsModels\EnzymeKinetics\MichaelisMenten.vb"

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

    '     Class CompartmentAccessories
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '     Class MichaelisMenten
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: (+2 Overloads) Factor, GetFluxValue, pH_Factor, T_Factor, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Data.SabiorkKineticLaws
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem

Namespace EngineSystem.MathematicsModels.EnzymeKinetics

    Public MustInherit Class CompartmentAccessories : Inherits MathematicsModel

        Protected _CompartmentObject As EngineSystem.ObjectModels.SubSystem.ICompartmentObject

        Sub New(CompartmentObject As EngineSystem.ObjectModels.SubSystem.ICompartmentObject)
            _CompartmentObject = CompartmentObject
        End Sub
    End Class

    ''' <summary>
    ''' 包含有ph和温度等条件
    ''' </summary>
    ''' <remarks>
    ''' <see cref="MathematicsModels.GenericKinetic"></see>对象所计算的返回值为一个普通的反应过程的当前代谢组条件下的Vmax
    ''' 酶促反应的动力学模型
    ''' v=(Vmax*[s]/(Km+[S]))*f([E])*f(pH, T)
    ''' </remarks>
    Public Class MichaelisMenten : Inherits CompartmentAccessories

        ''' <summary>
        ''' 指针指向代谢组的<see cref="MetabolismCompartment.Get_currentPH">环境PH计算函数</see>
        ''' </summary>
        ''' <remarks></remarks>
        Protected Get_currentPH As Func(Of Double)
        Protected Get_currentTemperature As Func(Of Double)

        Sub New(CompartmentObject As EngineSystem.ObjectModels.SubSystem.ICompartmentObject)
            Call MyBase.New(CompartmentObject)
            Get_currentPH = AddressOf CompartmentObject.Get_currentPH
            Get_currentTemperature = AddressOf CompartmentObject.Get_currentTemperature
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]::pH:={1};  T:={2}", _CompartmentObject.CompartmentId, Get_currentPH(), Get_currentTemperature())
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Vmax"><see cref="MathematicsModels.GenericKinetic"></see>对象所计算的返回值为一个普通的反应过程的当前代谢组条件下的Vmax</param>
        ''' <param name="Enzyme"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetFluxValue(Vmax As Double, Enzyme As EngineSystem.ObjectModels.Feature.MetabolismEnzyme) As Double
            Dim S = Enzyme.EnzymeMetabolite.Quantity
            Dim Km = Enzyme.EnzymeKineticLaw.Km
            Dim pH = Get_currentPH()
            Dim T As Double = Get_currentTemperature()

            Dim v = Vmax * S / (Km + S)
            Dim EnzymeActivity As Double = Factor(Enzyme)
            EnzymeActivity *= Factor(Enzyme.EnzymeKineticLaw, pH, T)
            v *= EnzymeActivity

            Enzyme._EnzymeActivity = EnzymeActivity

            Return v
        End Function

        ''' <summary>
        ''' 获取与酶分子的数量相关的动力学因子
        ''' </summary>
        ''' <param name="Enzyme"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Shared Function Factor(Enzyme As EngineSystem.ObjectModels.Feature.MetabolismEnzyme) As Double
            Dim value = System.Math.Log(Enzyme.Quantity + 1, 10) / Global.System.Math.E
            Return value
        End Function

        ''' <summary>
        ''' 获取和环境条件相关的动力学因子
        ''' </summary>
        ''' <param name="Enzyme"></param>
        ''' <param name="pH"></param>
        ''' <param name="T"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 分两段计算的：当小于最最佳值的时候，使用一个动力学方程
        ''' 当大于最佳值的时候使用另外一个动力学方程
        ''' </remarks>
        Protected Friend Shared Function Factor(Enzyme As EnzymeCatalystKineticLaw, pH As Double, T As Double) As Double
            Dim f As Double = pH_Factor(Enzyme, pH) / Enzyme.pH_Saturated
            f *= (T_Factor(Enzyme, T) / Enzyme.Temperature_Saturated)
            Return f
        End Function

        Protected Friend Shared Function pH_Factor(Enzyme As TabularDump.EnzymeCatalystKineticLaw, pH As Double) As Double
            Dim f As Double
            Dim Sigma As Double = 1.5

            If pH < Enzyme.PH Then
                Sigma = 1.5
            Else
                Sigma = 1
            End If

            f = NormalDistribution(pH, Enzyme.PH, Sigma)
            Return f
        End Function

        Protected Friend Shared Function T_Factor(Enzyme As TabularDump.EnzymeCatalystKineticLaw, T As Double) As Double
            Dim f As Double
            Dim Sigma As Double = 1.5

            If T < Enzyme.Temperature Then
                Sigma = 1.5
            Else
                Sigma = 0.8
            End If

            f = NormalDistribution(T, Enzyme.Temperature, Sigma)

            Return f
        End Function
    End Class
End Namespace
