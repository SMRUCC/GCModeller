#Region "Microsoft.VisualBasic::bc549277be03ca2cb52a0c57324b4e32, engine\GCModeller\EngineSystem\ObjectModels\SubSystem\CompartmentObject.vb"

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

    '     Interface ICompartmentObject
    ' 
    '         Properties: CompartmentId, EnvironmentFactors, EnzymeKinetics, Metabolites
    ' 
    '         Function: Get_currentPH, Get_currentTemperature, Initialize
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization

Namespace EngineSystem.ObjectModels.SubSystem

    ''' <summary>
    ''' (表示细胞结构内的一个被生物膜所隔绝的小区间)
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface ICompartmentObject : Inherits ICellComponentContainer

#Region "Public Property"

        <DumpNode>
        Property Metabolites As ObjectModels.Entity.Compound()
        ReadOnly Property CompartmentId As String
        ReadOnly Property EnzymeKinetics As MathematicsModels.EnzymeKinetics.MichaelisMenten
        ReadOnly Property EnvironmentFactors As MathematicsModels.EnzymeKinetics.pH_Tempratures
#End Region

        Function Initialize() As Integer

        ''' <summary>
        ''' 获取当前环境下的PH值
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Get_currentPH() As Double
        Function Get_currentTemperature() As Double
    End Interface
End Namespace
