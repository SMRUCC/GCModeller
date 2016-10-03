#Region "Microsoft.VisualBasic::5ce7ec0daec98ae076e52503b4c8166c, ..\GCModeller\sub-system\CellPhenotype\TRN\Configs\ReadModel.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Namespace TRN

    Public Class Configs

        Public Interface I_Configurable
            Function SetConfigs(conf As Configs) As Integer
        End Interface

        Public Property Regulator_Decays As Double
        Public Property Enzyme_Decays As Double

        ''' <summary>
        ''' 这个参数值调整调控事件的发生概率阈值的高低，则阈值越低，即调控事件越容易发生
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SiteSpecificDynamicsRegulations As Double
        ''' <summary>
        ''' 本底表达水平，数值越高，则表达量越高
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BasalThreshold As Double

        ''' <summary>
        ''' 没有在模型之中找到代谢物的合成的代谢途径，则可能为第二信使或者其他未知的原因，则在模型之中以很低的概率产生调控效应，这个参数配置产生火星的概率的高低
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OCS_NONE_Effector As Double

        ''' <summary>
        ''' 默认的调控值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OCS_Default_EffectValue As Double

    End Class
End Namespace
