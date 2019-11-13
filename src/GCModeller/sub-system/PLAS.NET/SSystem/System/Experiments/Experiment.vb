#Region "Microsoft.VisualBasic::dc490dcc0989e1ad4eeb8b8226b09c39, sub-system\PLAS.NET\SSystem\System\Experiments\Experiment.vb"

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

    '     Class Experiment
    ' 
    '         Properties: DisturbType, Id, Interval, Kicks, Start
    '                     Value
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Kernel.ObjectModels

    ''' <summary>
    ''' 对系统进行的一个刺激实验
    ''' </summary>
    Public Class Experiment

        ''' <summary>
        ''' The name Id of the target.
        ''' (目标的名称)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Id As String

        ''' <summary>
        ''' The start time of this disturb.
        ''' (这个干扰动作的开始时间)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Start As Double
        ''' <summary>
        ''' The interval ticks between each kick.
        ''' (每次干扰动作执行的时间间隔)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Interval As Double
        ''' <summary>
        ''' The counts of the kicks.
        ''' (执行的次数)
        ''' </summary>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Kicks As Integer
        <XmlAttribute> Public Property DisturbType As Types
        <XmlAttribute> Public Property Value As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
