﻿#Region "Microsoft.VisualBasic::d7b80db5377684811665f89745d09316, engine\Dynamics\Core\Mass\MassDynamics.vb"

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

    '   Total Lines: 194
    '    Code Lines: 142 (73.20%)
    ' Comment Lines: 24 (12.37%)
    '    - Xml Docs: 62.50%
    ' 
    '   Blank Lines: 28 (14.43%)
    '     File Size: 7.68 KB


    '     Class MassDynamics
    ' 
    '         Properties: Name, Value
    ' 
    '         Function: createMassIndex, Evaluate, getLastFluxVariants, PopulateDynamics, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics

Namespace Core

    ''' <summary>
    ''' Convert <see cref="Channel"/> matrix to mass equations
    ''' </summary>
    Public Class MassDynamics : Inherits var
        Implements IReadOnlyId, INonlinearVar
        Implements Enumeration(Of var)

        ''' <summary>
        ''' <see cref="Factor.ID"/>
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Property Name As String Implements IReadOnlyId.Identity
            Get
                Return mass.ID
            End Get
            Set(value As String)
                ' set name is not allowed
            End Set
        End Property

        Public Shadows ReadOnly Property Value As Double
            Get
                Return mass.Value
            End Get
        End Property

        Dim channels As Channel()
        ''' <summary>
        ''' 这个因子向量是相对于反应过程从左到右而言的
        ''' </summary>
        Dim factors As Double()
        Dim mass As Factor
        Dim shareFactors As (left As Dictionary(Of String, Double), right As Dictionary(Of String, Double))
        Dim fluxVariants As var()
        Dim fluxValues As Double()

        Private Sub New()
        End Sub

        Public Function Evaluate() As Double Implements INonlinearVar.Evaluate
            Dim additions As Double() = New Double(channels.Length - 1) {}
            Dim dir As Directions
            Dim variants As Double
            Dim flux As Channel
            Dim fluxVariant As Double

            If mass.role = MassRoles.gene Then
                Return 0
            End If

            For i As Integer = 0 To channels.Length - 1
                flux = channels(i)
                dir = flux.GetCurrentDirection

                Select Case dir
                    Case Directions.forward
                        variants = flux.forward.coefficient - flux.reverse.coefficient
                        fluxVariant = flux.CoverLeft(shareFactors.left, variants)
                        variants = factors(i) * fluxVariant
                    Case Directions.reverse
                        variants = flux.reverse.coefficient - flux.forward.coefficient
                        fluxVariant = -flux.CoverRight(shareFactors.right, variants)
                        variants = factors(i) * fluxVariant
                    Case Directions.stop
                        variants = 0
                        fluxVariant = 0
                    Case Else
                        Throw New InvalidProgramException($"Unknown reaction direction status of reaction flux: {flux.ID}!")
                End Select

                additions(i) = variants
                fluxValues(i) = fluxVariant
            Next

            Dim dy As Double = additions.Average
            ' dy = (channels.Length * dy) / (channels.Length + Math.Abs(dy))
            Return dy
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function getLastFluxVariants() As IEnumerable(Of var)
            For i As Integer = 0 To fluxValues.Length - 1
                If Not Double.IsNaN(fluxValues(i)) Then
                    fluxVariants(i).Value += fluxValues(i)
                End If
            Next

            Return fluxVariants
        End Function

        Public Overrides Function ToString() As String
            Return mass.ToString
        End Function

        Private Shared Function createMassIndex(channels As Channel(), ByRef templates As Index(Of String)) As Dictionary(Of String, List(Of Channel))
            Dim index As New Dictionary(Of String, List(Of Channel))
            Dim templateList As New List(Of String)

            For Each flux As Channel In channels
                Call flux.GetReactants _
                    .Where(Function(a) a.isTemplate) _
                    .Select(Function(a) a.mass.ID) _
                    .DoCall(AddressOf templateList.AddRange)

                For Each m As Variable In flux _
                    .GetReactants _
                    .Where(Function(a)
                               ' 被定义为模板的物质是不会被减少的
                               ' 所以在代谢底物部分跳过模板物质
                               Return Not a.isTemplate
                           End Function) _
                    .JoinIterates(flux.GetProducts)

                    If Not index.ContainsKey(m.mass.ID) Then
                        Call index.Add(m.mass.ID, New List(Of Channel))
                    End If

                    Call index(m.mass.ID).Add(flux)
                Next
            Next

            templates = templateList.Distinct.ToArray

            Return index
        End Function

        ''' <summary>
        ''' create dynamics equation for RK4 ODEs solver
        ''' </summary>
        ''' <param name="env"></param>
        ''' <returns></returns>
        Public Shared Iterator Function PopulateDynamics(env As Vessel) As IEnumerable(Of MassDynamics)
            Dim factors As New List(Of Double)
            Dim matter As Variable
            Dim templates As Index(Of String) = Nothing
            Dim massIndex = createMassIndex(env.Channels, templates)
            Dim channels As Channel()

            For Each mass As Factor In env.m_massIndex.Values
                Call factors.Clear()

                If Not massIndex.ContainsKey(mass.ID) Then
                    If Not mass.ID Like templates Then
                        ' compound is constant value
                        Call ($"missing dynamics for compound: " & mass.ID).Warning
                    End If

                    channels = {}
                Else
                    channels = massIndex(mass.ID).ToArray
                End If

                ' channels = channels.Where(Function(fx) Not (fx.left.IsNullOrEmpty OrElse fx.right.IsNullOrEmpty)).ToArray

                For Each flux As Channel In channels
                    matter = flux.GetReactants _
                        .Where(Function(a) a.mass.ID = mass.ID) _
                        .FirstOrDefault

                    If Not matter Is Nothing Then
                        ' 反应物端是消耗当前代谢物
                        ' 所以相关系数为负数值
                        factors.Add(-matter.coefficient)
                    Else
                        matter = flux.GetProducts _
                            .Where(Function(a) a.mass.ID = mass.ID) _
                            .First
                        ' 反应产物端则是增加当前代谢物
                        ' 相关系数为正实数
                        factors.Add(matter.coefficient)
                    End If
                Next

                If channels.IsNullOrEmpty Then
                    Continue For
                End If

                Yield New MassDynamics With {
                    .mass = mass,
                    .factors = factors.ToArray,
                    .channels = channels,
                    .shareFactors = env.shareFactors,
                    .fluxVariants = channels _
                        .Select(Function(a, i)
                                    Return New var With {
                                        .Index = i,
                                        .Name = a.ID,
                                        .Value = 0
                                    }
                                End Function) _
                        .ToArray,
                    .fluxValues = New Double(.fluxVariants.Length - 1) {}
                }
            Next
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of var) Implements Enumeration(Of var).GenericEnumerator
            For Each flux As var In fluxVariants
                Yield flux
            Next
        End Function
    End Class
End Namespace
