#Region "Microsoft.VisualBasic::7d57433fa9a2b993e3ade8ec1b979496, engine\Dynamics\Core\Kinetics\Controls\KineticsControls.vb"

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

'   Total Lines: 106
'    Code Lines: 66 (62.26%)
' Comment Lines: 23 (21.70%)
'    - Xml Docs: 91.30%
' 
'   Blank Lines: 17 (16.04%)
'     File Size: 3.68 KB


'     Class KineticsControls
' 
'         Properties: coefficient, parameters
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: getMass, ToString
' 
'     Class KineticsOverlapsControls
' 
'         Properties: coefficient
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression.Impl
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Core

    ''' <summary>
    ''' 主要是应用于酶促动力学反应过程的计算
    ''' </summary>
    Public Class KineticsControls : Inherits Controls

        ''' <summary>
        ''' 计算出当前的调控效应单位
        ''' </summary>
        ''' <returns></returns>
        Public Overrides ReadOnly Property coefficient As Double
            Get
                If lambda Is Nothing AndAlso inhibition.IsNullOrEmpty Then
                    Return baseline
                End If

                Dim i = inhibition.Sum(Function(v) v.coefficient * v.mass.Value)
                Dim a = lambda(fp_getMass)

                ' 抑制的总量已经大于等于激活的总量的时候，返回零值，
                ' 则反应过程可能不会发生
                Return Math.Max((a + baseline) - i, 0)
            End Get
        End Property

        ''' <summary>
        ''' lambda function
        ''' </summary>
        ReadOnly lambda As DynamicInvoke
        ReadOnly env As MassTable
        ReadOnly raw As Expression
        ReadOnly fp_getMass As Func(Of String, Double) = AddressOf getMass
        ''' <summary>
        ''' debug view of the kinetics function parameters
        ''' </summary>
        ReadOnly pars As Dictionary(Of String, String)

        ''' <summary>
        ''' get kinetics parameter mass reference names
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property parameters As IEnumerable(Of String)
            Get
                Return pars.Values
            End Get
        End Property

        Sub New(env As MassTable, lambda As DynamicInvoke, raw As Expression, cellular_id As String, Optional pars As String() = Nothing)
            Me.lambda = lambda
            Me.raw = raw
            Me.env = env
            Me.pars = pars.SafeQuery _
                .Distinct _
                .ToDictionary(Function(s) s,
                              Function(s)
                                  If s.IsNumeric(, True) Then
                                      Return s
                                  Else
                                      Return s & "@" & cellular_id
                                  End If
                              End Function)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function getMass(id As String) As Double
            Return env(pars(id)).Value
        End Function

        Public Overrides Function ToString() As String
            Return "[kinetics] " & raw.ToString & If(pars.IsNullOrEmpty, "", pars.GetJson)
        End Function
    End Class

    Public Class KineticsOverlapsControls : Inherits Controls

        Public Overrides ReadOnly Property coefficient As Double
            Get
                Dim i = inhibition.Sum(Function(v) v.coefficient * v.mass.Value)
                Dim a = Aggregate k As KineticsControls
                        In kinetics
                        Into Sum(k.coefficient)

                Return Math.Max((a + baseline) - i, 0)
            End Get
        End Property

        Friend ReadOnly kinetics As KineticsControls()

        ''' <summary>
        ''' get kinetics parameter mass reference names
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Iterator Property parameters As IEnumerable(Of String)
            Get
                For Each overlap As KineticsControls In kinetics
                    For Each name As String In overlap.parameters
                        Yield name
                    Next
                Next
            End Get
        End Property

        Sub New(overlaps As IEnumerable(Of KineticsControls))
            kinetics = overlaps.ToArray
        End Sub

        Public Overrides Function ToString() As String
            Return $"[overlaps] {kinetics.Length} kinetics"
        End Function
    End Class
End Namespace
