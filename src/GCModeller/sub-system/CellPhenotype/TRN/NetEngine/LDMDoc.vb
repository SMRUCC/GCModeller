#Region "Microsoft.VisualBasic::a75d7063744ed054f3b605e3653482d1, sub-system\CellPhenotype\TRN\NetEngine\LDMDoc.vb"

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

    '     Class NetworkModel
    ' 
    '         Properties: GeneObjects
    ' 
    '     Class NetworkInput
    ' 
    '         Properties: InitQuantity, Level, locusId, NoneRegulation
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace TRN

    ''' <summary>
    ''' 模型文件的XML文件对象
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <XmlRoot("BinaryNetwork.NetworkModel", Namespace:="http://code.google.com/p/genome-in-code/simulations/expression_network/binary_network")>
    Public Class NetworkModel
        <XmlElement("GeneObjects")> Public Property GeneObjects As KineticsModel.BinaryExpression()
    End Class

    ''' <summary>
    ''' 网络计算的蒙特卡洛输入
    ''' </summary>
    ''' <remarks></remarks>
    Public Class NetworkInput
        Implements IKeyValuePairObject(Of String, Boolean)
        Implements INamedValue

        Public Property locusId As String Implements IKeyValuePairObject(Of String, Boolean).Key, INamedValue.Key

        ''' <summary>
        ''' The initialize expression level for the target <see cref="locusId">gene</see>.(初始的表达水平)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Level As Boolean Implements IKeyValuePairObject(Of String, Boolean).Value

        ''' <summary>
        ''' 这个基因是不受任何调控作用的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property NoneRegulation As Boolean = False
        Public Property InitQuantity As Integer

        Public Overrides Function ToString() As String
            Return String.Format("{0}:= {1}", locusId, Level)
        End Function
    End Class
End Namespace
