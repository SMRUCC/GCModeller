#Region "Microsoft.VisualBasic::c929b12513b0c763f184db78655220bb, sub-system\CellPhenotype\TRN\KineticsModel\SiteInfo.vb"

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

    '     Class SiteInfo
    ' 
    '         Properties: Position, Regulators
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: get_RegulatorQuantitySum
    ' 
    '         Sub: set_Regulator
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.DataMining.DFL_Driver
Imports SMRUCC.genomics.Analysis.CellPhenotype.TRN.KineticsModel.Regulators

Namespace TRN.KineticsModel

    ''' <summary>
    ''' This object represents a regulatory motif site in the gene promoter region.
    ''' (启动子区的一个调控位点)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SiteInfo : Inherits dflNode

        Sub New()
            Call MyBase.New(New List(Of RegulationExpression))
        End Sub

        ''' <summary>
        ''' The position id value of the current regulatory site or the ATG distance of this site to the target gene.
        ''' (调控位点的编号或者说是当前的这个调控位点距离ATG的距离)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property Position As Integer
        ''' <summary>
        ''' 该位点之上的调控因子
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement> Public Property Regulators As RegulationExpression()
            Get
                Return (From item In Me.__factorList Select DirectCast(item, RegulationExpression)).ToArray
            End Get
            Set(value As RegulationExpression())
                If Not value.IsNullOrEmpty Then
                    Call Me.__factorList.Clear()
                    For Each item In value
                        Call Me.__factorList.Add(item.set_TargetSite(Me))
                    Next
                End If
            End Set
        End Property

        Public Sub set_Regulator(idx As Integer, instance As RegulationExpression)
            Me.__factorList(idx) = instance.set_TargetSite(Me)
        End Sub

        Friend Function get_RegulatorQuantitySum() As Integer
            Dim LQuery = (From Regulator In Regulators Select Regulator.Quantity).ToArray.Sum
            Return LQuery
        End Function
    End Class
End Namespace
