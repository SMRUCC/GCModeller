#Region "Microsoft.VisualBasic::57303b9ae4267fb569fce834fe3f3951, sub-system\FBA\FBA_DP\rFBA\old\BaseEquation.vb"

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

    '     Class BaseEquation
    ' 
    '         Properties: Value, Variables
    ' 
    '         Function: Evaluate, get_ObjectHandle
    '         Class Variable
    ' 
    '             Properties: Pcc, pHandle, Weight
    ' 
    ' 
    ' 
    '     Class NetworkModel
    ' 
    '         Properties: MetabolismHref, NetworkComponents, ObjectiveFunction
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.GCModeller.CompilerServices
Imports SMRUCC.genomics.GCModeller.Framework

Namespace rFBA

    Public Class BaseEquation : Inherits SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.Expression

        <XmlElement> Public Property Variables As Variable()

        Public Class Variable
            <XmlAttribute> Public Property pHandle As Integer
            <XmlAttribute> Public Property Pcc As Double
            <XmlAttribute> Public Property Weight As Double

            Protected Friend _Equation As BaseEquation
        End Class

        Public Overrides Function Evaluate() As Double
            Dim LQuery = (From item In Variables Select item.Pcc * (item._Equation._value) ^ item.Weight).Sum
            _value = LQuery
            Return LQuery
        End Function

        Public Overrides ReadOnly Property Value As Double
            Get
                Return _value
            End Get
        End Property

        Public Overrides Function get_ObjectHandle() As Kernel_Driver.DataStorage.FileModel.ObjectHandle
            Return New Kernel_Driver.DataStorage.FileModel.ObjectHandle With {.Handle = Handle, .ID = Identifier}
        End Function
    End Class

    Public Class NetworkModel : Inherits ModelBaseType

        <XmlElement> Public Property NetworkComponents As BaseEquation()
        <XmlAttribute> Public Property ObjectiveFunction As String()
        <XmlElement("include")> Public Property MetabolismHref As String
    End Class
End Namespace
