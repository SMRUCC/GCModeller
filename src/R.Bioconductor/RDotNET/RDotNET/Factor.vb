#Region "Microsoft.VisualBasic::dec89ecb3ad4fdb63026468f96dcfcd8, RDotNET\RDotNET\Factor.vb"

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

    ' Class Factor
    ' 
    '     Properties: IsOrdered
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetFactor, (+2 Overloads) GetFactors, GetLevels
    ' 
    '     Sub: SetFactor
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNet.Diagnostics
Imports RDotNet.Internals
Imports System.Diagnostics
Imports System.Linq
Imports System.Security.Permissions

''' <summary>
''' Represents factors.
''' </summary>
<DebuggerDisplay("Length = {Length}; Ordered = {IsOrdered}; RObjectType = {Type}")> _
<DebuggerTypeProxy(GetType(FactorDebugView))> _
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class Factor
	Inherits IntegerVector
	''' <summary>
	''' Creates a new instance for a factor vector.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a factor vector.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets the levels of this factor.
	''' </summary>
	''' <returns>Levels of this factor</returns>
	Public Function GetLevels() As String()
		Return GetAttribute(Engine.GetPredefinedSymbol("R_LevelsSymbol")).AsCharacter().ToArray()
	End Function

	''' <summary>
	''' Gets the values in this factor.
	''' </summary>
	''' <returns>Values of this factor</returns>
	Public Function GetFactors() As String()
		Dim levels = GetLevels()
		Dim levelIndices = Me.GetArrayFast()
		Return Array.ConvertAll(levelIndices, Function(value) (If(value = NACode, Nothing, levels(value - 1))))
	End Function

	''' <summary>
	''' Gets the levels of the factor as the specific enum type.
	''' </summary>
	''' <remarks>
	''' Be careful to the underlying values.
	''' You had better set <c>levels</c> and <c>labels</c> argument explicitly.
	''' </remarks>
	''' <example>
	''' <code>
	''' public enum Group
	''' {
	'''    Treatment = 1,
	'''    Control = 2
	''' }
	'''
	''' // You must set 'levels' and 'labels' arguments explicitly in this case
	''' // because levels of factor is sorted by default and the names in R and in enum names are different.
	''' var code = @"factor(
	'''    c(rep('T', 5), rep('C', 5), rep('T', 4), rep('C', 5)),
	'''    levels=c('T', 'C'),
	'''    labels=c('Treatment', 'Control')
	''' )";
	''' var factor = engine.Evaluate(code).AsFactor();
	''' foreach (Group g in factor.GetFactors&lt;Group&gt;())
	''' {
	'''    Console.Write("{0} ", g);
	''' }
	''' </code>
	''' </example>
	''' <typeparam name="TEnum">The type of enum.</typeparam>
	''' <param name="ignoreCase">The value indicating case-sensitivity.</param>
	''' <returns>Factors.</returns>
	Public Function GetFactors(Of TEnum As Structure)(Optional ignoreCase As Boolean = False) As TEnum()
		Dim enumType As Type = GetType(TEnum)
		If Not enumType.IsEnum Then
			Throw New ArgumentException("Only enum type is supported")
		End If
		' The exact underlying type of factor is Int32.
		' But probably other types are available.
		'if (Enum.GetUnderlyingType(enumType) != typeof(Int32))
		'{
		'   throw new ArgumentException("Only Int32 is supported");
		'}
		Dim levels = GetLevels()
		Return Me.[Select](Function(value) levels(value - 1)).[Select](Function(value) DirectCast([Enum].Parse(enumType, value, ignoreCase), TEnum)).ToArray()
	End Function

	''' <summary>
	''' Gets the value which indicating the factor is ordered or not.
	''' </summary>
	Public ReadOnly Property IsOrdered() As Boolean
		Get
			Return Me.GetFunction(Of Rf_isOrdered)()(Me.handle)
		End Get
	End Property

	''' <summary>
	''' Gets the value of the vector of factors at an index
	''' </summary>
	''' <param name="index">the zero-based index of the vector</param>
	''' <returns>The string representation of the factor, or a null reference if the value in R is NA</returns>
	Public Function GetFactor(index As Integer) As String
		Dim intValue = Me(index)
		If intValue <= 0 Then
			Return Nothing
		Else
			Return Me.GetLevels()(intValue - 1)
		End If
		' zero-based index in C#, but 1-based in R
	End Function

	''' <summary>
	''' Sets the value of a factor vector at an index
	''' </summary>
	''' <param name="index">the zero-based index item to set in the vector</param>
	''' <param name="factorValue">The value of the factor - can be a null reference</param>
	Public Sub SetFactor(index As Integer, factorValue As String)
		If factorValue Is Nothing Then
			Me(index) = NACode
		Else
			Dim levels = Me.GetLevels()
			Dim factIndex As Integer = Array.IndexOf(levels, factorValue)
			If factIndex >= 0 Then
				Me(index) = factIndex + 1
			Else
				' zero-based index in C#, but 1-based in R
				Me(index) = NACode
			End If
		End If
	End Sub
End Class

