﻿#Region "Microsoft.VisualBasic::1e00f98e66c027993bbcaf5e9155ad99, Data_science\Mathematica\Math\Math.Statistics\Distributions\MethodOfMoments\Normal.vb"

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

    ' 	Class Normal
    ' 
    ' 	    Constructor: (+3 Overloads) Sub New
    ' 	    Function: FindArea, GetCDF, GetInvCDF, GetMean, GetPDF
    '                GetStDev, TrapazoidalIntegration, Validate
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic

'
' * To change this license header, choose License Headers in Project Properties.
' * To change this template file, choose Tools | Templates
' * and open the template in the editor.
' 
Namespace Distributions.MethodOfMoments


	''' 
	''' <summary>
	''' @author Will_and_Sara
	''' </summary>
	Public Class Normal
		Inherits Distributions.ContinuousDistribution

		Public _Mean As Double
		Public _StDev As Double
		Public Overridable Function GetMean() As Double
			Return _Mean
		End Function
		Public Overridable Function GetStDev() As Double
			Return _StDev
		End Function
		''' <summary>
		''' Creates a standard normal distribution
		''' </summary>
		Public Sub New()
			_Mean = 0
			_StDev = 1
		End Sub
		''' <summary>
		''' Creates a normal distribution based on the user defined mean and standard deviation </summary>
		''' <param name="m"> the mean of the distribution </param>
		''' <param name="sd"> the standard deviation of the distribution </param>
		Public Sub New( m As Double,  sd As Double)
			_Mean = m
			_StDev = sd
		End Sub
        ''' <summary>
        ''' Creates a normal distribution based on input data using the standard method of moments. </summary>
        ''' <param name="data"> an array of double data. </param>
        Public Sub New(data As Double())
            Dim bpm As New MomentFunctions.BasicProductMoments(data)
            _Mean = bpm.Mean()
            _StDev = bpm.StDev()
            PeriodOfRecord = (bpm.SampleSize())
        End Sub
        Public Overrides Function GetInvCDF( probability As Double) As Double
			Dim i As Integer
			Dim x As Double
			Dim c0 As Double = 2.515517
			Dim c1 As Double =.802853
			Dim c2 As Double =.010328
			Dim d1 As Double = 1.432788
			Dim d2 As Double =.189269
			Dim d3 As Double =.001308
			Dim q As Double
			q = probability
			If q=.5 Then Return _Mean
			If q<=0 Then q=.000000000000001
			If q>=1 Then q=.999999999999999
			If q<.5 Then
				i=-1
			Else
				i=1
				q = 1-q
			End If
			Dim t As Double = Math.Sqrt(Math.Log(1/Math.Pow(q, 2)))
			x = t-(c0+c1*t+c2*(Math.Pow(t,2)))/(1+d1*t+d2*Math.Pow(t,2)+d3*Math.Pow(t,3))
			x = i*x
			Return (x*_StDev)+_Mean
		End Function
		Private Function TrapazoidalIntegration( y1 As Double,  y2 As Double,  deltax As Double) As Double
			Dim deltay As Double = 0
			Dim rect As Double = 0
			If y1>y2 Then
				deltay = y1-y2
				rect = Math.Abs(y2*deltax)
			Else
				deltay = y2-y1
				rect = Math.Abs(y1*deltax)
			End If
			Dim tri As Double = (1\2)*(deltax*deltay)
			Return rect + Math.Abs(tri)
		End Function
		Private Function FindArea( a As Double,  inc As Double,  x As Double) As Double
			Dim x1 As Double = GetInvCDF(a)
			Dim x2 As Double = GetInvCDF(a+inc)
			Do While x2>=x
			   x1 = x2
			   a += inc
			   x2 = GetInvCDF(a+inc)
			Loop
			Dim y1 As Double = GetPDF(x1)
			Dim y2 As Double = GetPDF(x2)
			Dim deltax As Double = Math.Abs(x1-x2)
			Dim area As Double = TrapazoidalIntegration(y1,y2,deltax)
			Dim interpvalue As Double = (x-x1)/(x2-x1)
			a+=area*interpvalue
			Return a
		End Function
        Public Overrides Function GetCDF(value As Double) As Double
            'decide which method i want to use.  errfunction, the method i came up with in vb, or something else.
            If value = _Mean Then Return 0.5
            Dim dist As Double = value - _Mean
            Dim stdevs As Integer = CInt(Fix(Math.Floor(Math.Abs(dist / _StDev))))
            Dim inc As Double = 1 \ 250
            Dim a As Double = 0.5
            Dim a1 As Double = 0.682689492137 / 2
            Dim a2 As Double = 0.954499736104 / 2
            Dim a3 As Double = 0.997300203937 / 2
            Dim a4 As Double = 0.999936657516 / 2
            Dim a5 As Double = 0.999999426687 / 2
            Dim a6 As Double = 0.999999998027 / 2
            Dim a7 As Double = (a - a6) / 2
            Select Case stdevs
                Case 0
                    If dist < 0 Then a += -a1
                    Return FindArea(a, inc, value)
                Case 1
                    If dist < 0 Then
                        a -= a2
                    Else
                        a += a1
                    End If
                    Return FindArea(a, inc, value)
                Case 2
                    If dist < 0 Then
                        a -= a3
                    Else
                        a += a2
                    End If
                    Return FindArea(a, inc, value)
                Case 3
                    If dist < 0 Then
                        a -= a4
                    Else
                        a += a3
                    End If
                    Return FindArea(a, inc, value)
                Case 4
                    If dist < 0 Then
                        a -= a5
                    Else
                        a += a4
                    End If
                    Return FindArea(a, inc, value)
                Case 5
                    If dist < 0 Then
                        a -= a6
                    Else
                        a += a5
                    End If
                    Return FindArea(a, inc, value)
                Case 6
                    If dist < 0 Then
                        a -= a7
                    Else
                        a += a6
                    End If
                    Return FindArea(a, inc, value)
                Case Else
                    If dist < 0 Then
                        Return 0
                    Else
                        Return 1
                    End If
            End Select
        End Function
        Public Overrides Function GetPDF( value As Double) As Double
			Return (1/Math.Sqrt(2*Math.PI)*Math.Pow(_StDev,2.0))*Math.Exp((-(Math.Pow(value-_Mean, 2)/(2*Math.Pow(_StDev, 2)))))
		End Function
		Public Overrides Function Validate() As List(Of Distributions.ContinuousDistributionError)
			Dim errors As New List(Of Distributions.ContinuousDistributionError)
			If _StDev<=0 Then errors.Add(New Distributions.ContinuousDistributionError("Standard of Deviation must be greater than 0"))
			Return errors
		End Function
	End Class

End Namespace
