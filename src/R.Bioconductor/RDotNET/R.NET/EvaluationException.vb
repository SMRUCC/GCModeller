#Region "Microsoft.VisualBasic::f761dbb155854f1360c17857a3ca1966, ..\R.Bioconductor\RDotNET\R.NET\EvaluationException.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.Serialization
Imports System.Text

''' <summary>
''' Exception signaling that the R engine failed to evaluate a statement
''' </summary>
Public Class EvaluationException
	Inherits Exception
	''' <summary>
	''' Create an exception for a statement that failed to be evaluate by e.g. R_tryEval
	''' </summary>
	''' <param name="errorMsg">The last error message of the failed evaluation in the R engine</param>
	Public Sub New(errorMsg As String)
		MyBase.New(errorMsg)
	End Sub

	''' <summary>
	''' Create an exception for a statement that failed to be evaluate by e.g. R_tryEval
	''' </summary>
	''' <param name="errorMsg">The last error message of the failed evaluation in the R engine</param>
	''' <param name="innerException">The exception that was caught and triggered the creation of this evaluation exception</param>
	Public Sub New(errorMsg As String, innerException As Exception)
		MyBase.New(errorMsg, innerException)
	End Sub
End Class
