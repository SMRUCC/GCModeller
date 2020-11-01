#Region "Microsoft.VisualBasic::699dea6bf8f21a417204509cdad2b919, RDotNET\RDotNET\Utilities\RTypesUtil.vb"

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

    ' 	Class RTypesUtil
    ' 
    ' 	    Constructor: (+1 Overloads) Sub New
    ' 	    Function: DeserializeComplexFromDouble, SerializeComplexToDouble
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Numerics

Namespace Utilities
	''' <summary>
	''' An internal helper class to convert types of arrays, primarily for data operations necessary for .NET types to/from R concepts.
	''' </summary>
	Friend NotInheritable Class RTypesUtil
		Private Sub New()
		End Sub
		Friend Shared Function SerializeComplexToDouble(values As Complex()) As Double()
			Dim data = New Double(2 * values.Length - 1) {}
			For i As Integer = 0 To values.Length - 1
				data(2 * i) = values(i).Real
				data(2 * i + 1) = values(i).Imaginary
			Next
			Return data
		End Function

		Friend Shared Function DeserializeComplexFromDouble(data As Double()) As Complex()
			Dim dblLen As Integer = data.Length
			If dblLen Mod 2 <> 0 Then
				Throw New ArgumentException("Serialized definition of complexes must be of even length")
			End If
			Dim n As Integer = dblLen \ 2
			Dim res = New Complex(n - 1) {}
			For i As Integer = 0 To n - 1
				res(i) = New Complex(data(2 * i), data(2 * i + 1))
			Next
			Return res
		End Function
	End Class
End Namespace

