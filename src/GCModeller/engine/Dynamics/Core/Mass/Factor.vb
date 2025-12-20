#Region "Microsoft.VisualBasic::d72387750bfbda38cc28057a1cb0aa35, engine\Dynamics\Core\Mass\Factor.vb"

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


    ' Code Statistics:

    '   Total Lines: 59
    '    Code Lines: 23 (38.98%)
    ' Comment Lines: 27 (45.76%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (15.25%)
    '     File Size: 1.97 KB


    '     Class Factor
    ' 
    '         Properties: cellular_compartment, ID, name, role
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace Core

    ''' <summary>
    ''' A mass factor(molecule entity) insdie the simulator runtime environment
    ''' </summary>
    ''' <remarks>
    ''' 一个变量因子，这个对象主要是用于存储值
    ''' </remarks>
    Public Class Factor : Implements INamedValue

        ''' <summary>
        ''' the unique reference id of current molecule
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' this unique instance id usually be in format like: ``id@compart_id``
        ''' </remarks>
        Public Property ID As String Implements IKeyedEntity(Of String).Key

        Public Overridable ReadOnly Property Value As Double
            Get
                Return m_mass
            End Get
        End Property

        ''' <summary>
        ''' 分子角色
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property role As MassRoles

        ''' <summary>
        ''' the molecule entity name, just used for debug view
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String
        Public Property template_id As String

        ''' <summary>
        ''' the cellular compartment id reference of this molecule entity
        ''' </summary>
        ''' <returns></returns>
        Public Property cellular_compartment As String

        ''' <summary>
        ''' this mass factor value
        ''' </summary>
        Dim m_mass As Double

        Sub New()
            role = MassRoles.compound
        End Sub

        ''' <summary>
        ''' make value copy of the mass factor model
        ''' </summary>
        ''' <param name="copy"></param>
        Sub New(copy As Factor)
            Call Me.New(copy.ID, copy.role, copy.cellular_compartment)

            name = copy.name
            template_id = copy.template_id
        End Sub

        ''' <summary>
        ''' create a new mass factor inside the runtime environment with value assigned ZERO.
        ''' </summary>
        ''' <param name="id$"></param>
        ''' <param name="role"></param>
        Sub New(id$, role As MassRoles, compart_id As String)
            Me.cellular_compartment = compart_id
            Me.ID = id
            Me.role = role
        End Sub

        Sub New(id As String, value As Double)
            Me.ID = id
            Me.m_mass = value
        End Sub

        ''' <summary>
        ''' reset the mass value to a given <paramref name="value"/> number.
        ''' </summary>
        ''' <param name="value"></param>
        Public Sub reset(value As Double)
            m_mass = value
        End Sub

        Public Sub add(delta As Double)
            If Not Double.IsNaN(delta) Then
                Dim mass As Double = m_mass

                If Double.IsPositiveInfinity(delta) Then
                    m_mass = m_mass * 100
                ElseIf Double.IsNegativeInfinity(delta) Then
                    m_mass = m_mass / 100
                Else
                    m_mass += delta
                End If

                If m_mass.IsNaNImaginary Then
                    m_mass = mass
                End If
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return $"{If(name, ID)} ({Value} unit@{cellular_compartment}, {role.Description})"
        End Function

        Public Shared Operator >(factor As Factor, d As Double) As Boolean
            Return factor.Value > d
        End Operator

        Public Shared Operator <(factor As Factor, d As Double) As Boolean
            Return factor.Value < d
        End Operator
    End Class
End Namespace
