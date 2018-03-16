#Region "Microsoft.VisualBasic::87994e15ad05dd58b3c8dede04796718, engine\GCModeller\EngineSystem\Engine\MathematicsModels\GenericKinetic.vb"

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

    '     Class GenericKinetic
    ' 
    '         Constructor: (+3 Overloads) Sub New
    ' 
    '         Function: (+2 Overloads) Clone, GetInreversibleFluxValue, GetReversibleFluxValue, GetValue, ToString
    ' 
    '         Sub: HandleDenominatorEqualsNaNException, HandleDenominatorEqualsZeroException
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Serialization

Namespace EngineSystem.MathematicsModels

    Public Class GenericKinetic : Inherits MathematicsModel

        <DumpNode> Protected f1, f2 As Double
        <DumpNode> Protected VF, VB As Double
        <DumpNode> Protected KA, KP As Double

        Protected get_ValueMethod As Func(Of Double)
        Protected Friend SystemLogging As LogFile
        ''' <summary>
        ''' 不要使用<see cref="DumpNode">Dump特性</see>，否则会出现无限递归
        ''' </summary>
        ''' <remarks></remarks>
        Protected _FluxObject As EngineSystem.ObjectModels.Module.MetabolismFlux
        Protected get_CurrentTemperature As Func(Of Double)

        Protected Sub New()
        End Sub

        Public Sub New(FluxObject As EngineSystem.ObjectModels.Module.MetabolismFlux, SystemLogging As LogFile, UPPER_BOUND As Double, LOWER_BOUND As Double, K1 As Double, k2 As Double)
            Me.SystemLogging = SystemLogging

            Me.VF = UPPER_BOUND
            Me.VB = LOWER_BOUND
            Me.f1 = VF ^ 2 / (VF ^ 2 + VB ^ 2)
            Me.f2 = VB ^ 2 / (VF ^ 2 + VB ^ 2)

            If VF = 0.0R AndAlso VB = 0.0R Then
                Dim ErrMessage As String = String.Format("[{0}] :: Factor is calculate as infinity, can not initalize this mathematics model!", FluxObject.Identifier)
                Call SystemLogging.WriteLine("An critical error was unable to handle: " & vbCrLf & ErrMessage, "", Type:=MSG_TYPES.ERR)
                Throw New DataException(ErrMessage)
            End If

#If DEBUG Then
            If Double.Equals(f1, Double.NaN) OrElse Double.Equals(f1, Double.NegativeInfinity) OrElse Double.Equals(f1, Double.PositiveInfinity) OrElse Double.IsInfinity(f1) Then
                Call Console.WriteLine("f1 in {0} is not a number!", FluxObject.Identifier)
            End If
#End If

#If DEBUG Then
            If (From item In FluxObject._Reactants Where item.EntityCompound Is Nothing Select 1).ToArray.Count > 0 OrElse
 (From item In FluxObject._Products Where item.EntityCompound Is Nothing Select 1).ToArray.Count > 0 Then
                Call Console.WriteLine("{0} is null metabolite entity!", FluxObject.Identifier)
            End If
#End If

            Me.KA = K1 '将目标数组仅赋值其值，而不连带复制其元素的指针
            Me.KP = k2

#If DEBUG Then
            If KA = 0.0R Then
                Console.WriteLine(FluxObject.Identifier)
            End If
#End If

            If FluxObject.Reversible Then
                get_ValueMethod = AddressOf GetReversibleFluxValue
            Else
                get_ValueMethod = AddressOf GetInreversibleFluxValue
            End If

            Me._FluxObject = FluxObject
        End Sub

        Sub New(FluxObject As EngineSystem.ObjectModels.Module.MetabolismFlux, SystemLogging As LogFile)
            Me.SystemLogging = SystemLogging

            Me.VF = FluxObject.UPPER_BOUND
            Me.VB = FluxObject.LOWER_BOUND
            Me.f1 = VF ^ 2 / (VF ^ 2 + VB ^ 2)
            Me.f2 = VB ^ 2 / (VF ^ 2 + VB ^ 2)

            If VF = 0.0R AndAlso VB = 0.0R Then
                Dim ErrMessage As String = String.Format("[{0}] :: Factor is calculate as infinity, can not initalize this mathematics model!", FluxObject.Identifier)
                Call SystemLogging.WriteLine("An critical error was unable to handle: " & vbCrLf & ErrMessage, "", Type:=MSG_TYPES.ERR)
                Throw New DataException(ErrMessage)
            End If

#If DEBUG Then
            If Double.Equals(f1, Double.NaN) OrElse Double.Equals(f1, Double.NegativeInfinity) OrElse Double.Equals(f1, Double.PositiveInfinity) OrElse Double.IsInfinity(f1) Then
                Call Console.WriteLine("f1 in {0} is not a number!", FluxObject.Identifier)
            End If
#End If

#If DEBUG Then
            If (From item In FluxObject._Reactants Where item.EntityCompound Is Nothing Select 1).ToArray.Count > 0 OrElse
 (From item In FluxObject._Products Where item.EntityCompound Is Nothing Select 1).ToArray.Count > 0 Then
                Call Console.WriteLine("{0} is null metabolite entity!", FluxObject.Identifier)
            End If
#End If

            Me.KA = FluxObject._BaseType.p_Dynamics_K_1 '将目标数组仅赋值其值，而不连带复制其元素的指针
            Me.KP = FluxObject._BaseType.p_Dynamics_K_2

#If DEBUG Then
            If KA = 0.0R Then
                Console.WriteLine(FluxObject.Identifier)
            End If
#End If

            If FluxObject.Reversible Then
                get_ValueMethod = AddressOf GetReversibleFluxValue
            Else
                get_ValueMethod = AddressOf GetInreversibleFluxValue
            End If

            Me._FluxObject = FluxObject
        End Sub

        Protected Shared Function Clone(Target As Integer()) As Integer()
            Dim ChunkBuffer As Integer() = New Integer(Target.Count - 1) {}
            For i As Integer = 0 To Target.Count - 1
                ChunkBuffer(i) = i
            Next
            Return ChunkBuffer
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Target"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Shared Function Clone(Target As Double()) As Double()
            Dim ChunkBuffer As Double() = New Double(Target.Count - 1) {}
            For i As Integer = 0 To ChunkBuffer.Count - 1
                ChunkBuffer(i) = CType(Target(i).ToString, Double)
            Next

            Return ChunkBuffer
        End Function

        ''' <summary>
        ''' 当发生错误的时候会返回-1标记
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function GetValue() As Double
            Dim value = get_ValueMethod()
            If Double.IsNegativeInfinity(value) Then
                value = Me._FluxObject.LOWER_BOUND
            ElseIf Double.IsPositiveInfinity(value) Then
                value = Me._FluxObject.UPPER_BOUND
            ElseIf Double.IsNaN(value) Then
                value = 0
                Call SystemLogging.WriteLine("NaN Exception occur in flux object: " & _FluxObject.Identifier, "GCModeller::GenericKinetic", Type:=MSG_TYPES.ERR)
            End If
            Return value
        End Function

        Protected Overridable Function GetReversibleFluxValue() As Double
            Dim Numerator As Double, Denominator As Double

            For Each Metabolite In _FluxObject._Reactants
                If Metabolite.EntityCompound.Quantity < 0 Then
                    Call SystemLogging.WriteLine(String.Format("NEGATIVE_DATA_EXCEPTION: {0}:={1}, value was set to ZERO! #{2}", Metabolite.Identifier, Metabolite.EntityCompound.Quantity, _FluxObject.Identifier),
                                                 "",
                                                 Type:=MSG_TYPES.ERR)
                    Metabolite.EntityCompound.Quantity = 0
                End If
            Next
            For Each Metabolite In _FluxObject._Products
                If Metabolite.EntityCompound.Quantity < 0 Then
                    Call SystemLogging.WriteLine(String.Format("NEGATIVE_DATA_EXCEPTION: {0}:={1}, value was set to ZERO! #{2}", Metabolite.Identifier, Metabolite.EntityCompound.Quantity, _FluxObject.Identifier),
                                                 "",
                                                 Type:=MSG_TYPES.ERR)
                    Metabolite.EntityCompound.Quantity = 0
                End If
            Next

            Numerator = KA * VF * (From Metabolite In _FluxObject._Reactants Select (Metabolite.EntityCompound.Quantity) ^ Metabolite.Stoichiometry).π +
                        KP * VB * (From Metabolite In _FluxObject._Products Select (Metabolite.EntityCompound.Quantity) ^ Metabolite.Stoichiometry).π
            Denominator = f1 * (From Metabolite In _FluxObject._Reactants Select (1 + (Metabolite.EntityCompound.Quantity)) ^ Metabolite.Stoichiometry).π +
                          f2 * (From Metabolite In _FluxObject._Products Select (1 + (Metabolite.EntityCompound.Quantity)) ^ Metabolite.Stoichiometry).π + 1

            If Denominator = 0.0R Then
                Call HandleDenominatorEqualsZeroException()
                Return 0
            End If

            If Double.IsNaN(Numerator) AndAlso Double.IsNaN(Denominator) Then
                Call HandleDenominatorEqualsNaNException()
                Call SystemLogging.WriteLine("NAN_EXCEPTION, Numerator is also NaN, return value set to 1!", "GetInreversibleFluxValue()", Type:=MSG_TYPES.ERR)
                Return 1
            ElseIf Double.IsNaN(Denominator) Then
                Call HandleDenominatorEqualsNaNException()
                Return 0
            End If

            Dim value = Numerator / Denominator
            Return value
        End Function

        Protected Overridable Function GetInreversibleFluxValue() As Double
            Dim Numerator As Double, Denominator As Double

            For Each Metabolite In _FluxObject._Reactants
                If Metabolite.EntityCompound.Quantity < 0 Then
                    Call SystemLogging.WriteLine(String.Format("NEGATIVE_DATA_EXCEPTION: {0}:={1}, value was set to ZERO! #{2}", Metabolite.Identifier, Metabolite.EntityCompound.Quantity, _FluxObject.Identifier),
                                                 "",
                                                 Type:=MSG_TYPES.ERR)
                    Metabolite.EntityCompound.Quantity = 0
                End If
            Next

            Numerator = KA * VF * (From Metabolite In _FluxObject._Reactants Select Metabolite.EntityCompound.Quantity ^ Metabolite.Stoichiometry).π
            Denominator = f1 * (From Metabolite In _FluxObject._Reactants Select (1 + Metabolite.EntityCompound.Quantity) ^ Metabolite.Stoichiometry).π

            If Denominator = 0.0R Then
                Call HandleDenominatorEqualsZeroException()
                Return 0
            End If

            If Double.IsNaN(Numerator) AndAlso Double.IsNaN(Denominator) Then
                Call HandleDenominatorEqualsNaNException()
                Call SystemLogging.WriteLine("NAN_EXCEPTION, Numerator is also NaN, return value set to 1!", "GetInreversibleFluxValue()", Type:=MSG_TYPES.ERR)
                Return 1
            ElseIf Double.IsNaN(Denominator) Then
                Call HandleDenominatorEqualsNaNException()
                Return 0
            ElseIf Double.IsNaN(Numerator) Then
                Call HandleDenominatorEqualsNaNException()
                Return _FluxObject.UPPER_BOUND
            End If

            Dim value As Double = Numerator / Denominator
            Return value
        End Function

        Public Overrides Function ToString() As String
            Return _FluxObject.Identifier
        End Function

        Protected Sub HandleDenominatorEqualsNaNException()
            Dim ErrMessage As StringBuilder = New StringBuilder("Denominator is NaN in the metabolism flux kinetic model, detail snapped data was shown below:" & vbCrLf)
            Call ErrMessage.AppendLine(String.Format("FluxObject_UniqueId:={0}, {1}", _FluxObject.Identifier, If(_FluxObject.Reversible, "Reversible", "InReversible")))
            Call ErrMessage.AppendLine(String.Format("Kinetics Parameters: f1:={0}, f2:={1}, VF:={2}, VB:={3}, Ka:={4}, Kp:={5}", f1, f2, VF, VB, KA, KP))
            Call ErrMessage.AppendLine("Reaction substrate detail:")
            Call ErrMessage.AppendLine(" Reactants:")
            For Each item In _FluxObject._Reactants
                Call ErrMessage.AppendLine(String.Format("  {0} x {1}:= {2}(mmol/mml)", item.Stoichiometry, item.Identifier, item.EntityCompound.Quantity))
            Next
            Call ErrMessage.AppendLine(" Products:")
            For Each item In _FluxObject._Products
                Call ErrMessage.AppendLine(String.Format("  {0} x {1}:= {2}(mmol/mml)", item.Stoichiometry, item.Identifier, item.EntityCompound.Quantity))
            Next

            Call SystemLogging.WriteLine(ErrMessage.ToString, "GCModeller::GenericKinetic", Type:=MSG_TYPES.ERR)
        End Sub

        Protected Sub HandleDenominatorEqualsZeroException()
            Dim ErrMessage As StringBuilder = New StringBuilder("Denominator is zero in the metabolism flux kinetic model, detail snapped data was shown below:" & vbCrLf)
            Call ErrMessage.AppendLine(String.Format("FluxObject_UniqueId:={0}, {1}", _FluxObject.Identifier, If(_FluxObject.Reversible, "Reversible", "InReversible")))
            Call ErrMessage.AppendLine(String.Format("Kinetics Parameters: f1:={0}, f2:={1}, VF:={2}, VB:={3}, Ka:={4}, Kp:={5}", f1, f2, VF, VB, KA, KP))
            Call ErrMessage.AppendLine("Reaction substrate detail:")
            Call ErrMessage.AppendLine(" Reactants:")
            For Each item In _FluxObject._Reactants
                Call ErrMessage.AppendLine(String.Format("  {0} x {1}:= {2}(mmol/mml)", item.Stoichiometry, item.Identifier, item.EntityCompound.Quantity))
            Next
            Call ErrMessage.AppendLine(" Products:")
            For Each item In _FluxObject._Products
                Call ErrMessage.AppendLine(String.Format("  {0} x {1}:= {2}(mmol/mml)", item.Stoichiometry, item.Identifier, item.EntityCompound.Quantity))
            Next

            Call SystemLogging.WriteLine(ErrMessage.ToString, "GCModeller::GenericKinetic", Type:=MSG_TYPES.ERR)
        End Sub
    End Class
End Namespace
