#Region "Microsoft.VisualBasic::49d74e31bacdf9ea66c0aa10338daea3, engine\GCModeller\EngineSystem\Engine\MathematicsModels\EnzymeKinetics\ExpressionKinetics.vb"

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

    '     Class ExpressionKinetics
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetFluxValue
    ' 
    '     Class ExpressionRegulationDynamics
    ' 
    ' 
    '         Interface IDFL_Node
    ' 
    ' 
    ' 
    '         Interface IDFL_Dynamics
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace EngineSystem.MathematicsModels.EnzymeKinetics

    ''' <summary>
    ''' Enzyme catalyze like kinetics model for the gene expression events
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ExpressionKinetics : Inherits MichaelisMenten

        Sub New(CompartmentObject As EngineSystem.ObjectModels.SubSystem.ICompartmentObject)
            Call MyBase.New(CompartmentObject)
        End Sub

        Public Overloads Function GetFluxValue(Vmax As Double, Regulation As Double, Enzyme As ObjectModels.Feature.MetabolismEnzyme) As Double
            Dim S = Regulation
            Dim Km = Enzyme.EnzymeKineticLaw.Km
            Dim pH = Get_currentPH()
            Dim T As Double = Get_currentTemperature()

            Dim v = Vmax * S / (Km + S)
            v *= Factor(Enzyme)
            v *= Factor(Enzyme.EnzymeKineticLaw, pH, T)
            Return v
        End Function
    End Class

    ''' <summary>
    ''' DFL model for gene expression regulations (<see cref="ModellingEngine.EngineSystem.ObjectModels.[Module].CentralDogmaInstance.Transcription"></see>, 
    ''' <see cref="ModellingEngine.EngineSystem.ObjectModels.[Module].CentralDogmaInstance.Translation"></see>).(基因表达事件调控的动态模糊逻辑(DFL)模型)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ExpressionRegulationDynamics : Inherits MathematicsModel

        ''' <summary>
        ''' 动态模糊逻辑计算节点
        ''' </summary>
        ''' <remarks></remarks>
        Public Interface IDFL_Node

        End Interface

        ''' <summary>
        ''' 该<see cref="IDFL_Node"></see>节点之中的计算分量
        ''' </summary>
        ''' <remarks></remarks>
        Public Interface IDFL_Dynamics

        End Interface

    End Class
End Namespace
