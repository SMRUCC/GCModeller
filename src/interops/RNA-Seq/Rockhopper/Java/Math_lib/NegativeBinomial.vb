#Region "Microsoft.VisualBasic::ba13542eb90ba30e1254d13637648466, RNA-Seq\Rockhopper\Java\Math_lib\NegativeBinomial.vb"

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

    ' Module NegativeBinomial
    ' 
    '     Function: bd0, (+2 Overloads) pmf, stirlerr
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

'
' * Copyright 2013 Brian Tjaden
' *
' * This file is part of Rockhopper.
' *
' * Rockhopper is free software: you can redistribute it and/or modify
' * it under the terms of the GNU General Public License as published by
' * the Free Software Foundation, either version 3 of the License, or
' * any later version.
' *
' * Rockhopper is distributed in the hope that it will be useful,
' * but WITHOUT ANY WARRANTY; without even the implied warranty of
' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' * GNU General Public License for more details.
' *
' * You should have received a copy of the GNU General Public License
' * (in the file gpl.txt) along with Rockhopper.  
' * If not, see <http://www.gnu.org/licenses/>.
' 


Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

''' <summary>
''' Probability mass function of negative binomial distribution.
''' Based on saddle point algorithm found in "Fast and Accurate
''' Computation of Binomial Probabilities" by Catherine Loader, 2000.
''' </summary>
''' 
<PackageNamespace("NegativeBinomial",
                  Description:="Probability mass function of negative binomial distribution. Based on saddle point algorithm found in ""Fast And Accurate Computation of Binomial Probabilities"" by Catherine Loader, 2000.",
                  Publisher:="Catherine Loader, 2000",
                  Cites:="""Fast And Accurate Computation of Binomial Probabilities"" by Catherine Loader, 2000.")>
Public Module NegativeBinomial

    ''' <summary>
    '''***************************************
    ''' **********   CLASS VARIABLES   **********
    ''' </summary>

    Private S0 As Double = 0.0833333333333333
    Private S1 As Double = 0.00277777777777778
    Private S2 As Double = 0.000793650793650794
    Private S3 As Double = 0.000595238095238095
    Private S4 As Double = 0.000841750841750842
    Private sfe As Double() = {0.0, 0.0810614667953273, 0.0413406959554093, 0.0276779256849983, 0.0207906721037651, 0.0166446911898212,
        0.0138761288230707, 0.0118967099458918, 0.0104112652619721, 0.00925546218271273, 0.00833056343336287, 0.00757367548795184,
        0.00694284010720953, 0.00640899418800421, 0.00595137011275885, 0.0055547335519628}



    ''' <summary>
    '''********************************************
    ''' **********   PUBLIC CLASS METHODS   **********
    ''' </summary>
    <ExportAPI("pmf")>
    Public Function pmf(k As Double, n As Double, p As Double, b As Boolean) As Double
        If p = 0.0 Then
            If k = 0 Then
                Return 1.0
            Else
                Return 0.0
            End If
        ElseIf p = 1.0 Then
            If k = n Then
                Return 1.0
            Else
                Return 0.0
            End If
        ElseIf k = 0 Then
            Return Math.Exp(n * Math.Log(1.0 - p))
        ElseIf k = n Then
            Return Math.Exp(n * Math.Log(p))
        Else
            Dim lc As Double = stirlerr(n) - stirlerr(k) - stirlerr(n - k) - bd0(k, n * p) - bd0(n - k, n * (1.0 - p))
            ' We multiply by "p" here
            Return p * Math.Exp(lc) * Math.Sqrt(n / (2.0 * Math.PI * k * (n - k)))
        End If
    End Function

    <ExportAPI("pmf")>
    Public Function pmf(k As Double, n As Double, p As Double) As Double
        If p = 0.0 Then
            If k = 0 Then
                Return 1.0
            Else
                Return 0.0
            End If
        ElseIf p = 1.0 Then
            If k = n Then
                Return 1.0
            Else
                Return 0.0
            End If
        ElseIf k = 0 Then
            Return Math.Exp(n * Math.Log(1.0 - p))
        ElseIf k = n Then
            Return Math.Exp(n * Math.Log(p))
        Else
            Dim lc As Double = stirlerr(n) - stirlerr(k) - stirlerr(n - k) - bd0(k, n * p) - bd0(n - k, n * (1.0 - p))
            ' We multiply by "p" here
            Return p * Math.Exp(lc) * Math.Sqrt(n / (2.0 * Math.PI * k * (n - k)))
        End If
    End Function



    ''' <summary>
    '''*********************************************
    ''' **********   PRIVATE CLASS METHODS   **********
    ''' </summary>

    ''' <summary>
    ''' log(n!) - log(sqrt(2*pi*n)*(n/e)^n)
    ''' </summary>
    Private Function stirlerr(n As Double) As Double
        If n < 16 Then
            Return sfe(CInt(Math.Truncate(n)))
        End If
        Dim nn As Double = n * n
        If n > 500 Then
            Return (S0 - S1 / nn) / n
        End If
        If n > 80 Then
            Return (S0 - (S1 / S2 / nn) / nn) / n
        End If
        If n > 35 Then
            Return (S0 - (S1 - (S2 - S3 / nn) / nn) / nn) / n
        End If
        Return (S0 - (S1 - (S2 - (S3 - S4 / nn) / nn) / nn) / nn) / n
    End Function

    ''' <summary>
    ''' Deviance term: k*lg(k/np) + np - k
    ''' </summary>
    Private Function bd0(k As Double, np As Double) As Double
        If Math.Abs(k - np) < 0.1 * (k + np) Then
            Dim s As Double = (k - np) * (k - np) / (k + np)
            Dim v As Double = (k - np) / (k + np)
            Dim ej As Double = 2 * k * v
            Dim j As Integer = 1
            While True
                ej = ej * v * v
                Dim s1 As Double = s + ej / (2 * j + 1)
                If s1 = s Then
                    Return s
                End If
                s = s1
                j += 1

            End While
        End If
        Return (k * Math.Log(k / np) + np - k)
    End Function



    ''' <summary>
    '''***********************************
    ''' **********   MAIN METHOD   **********
    ''' </summary>

    Private Sub Main(args As String())

        If args.Length < 3 Then
            Oracle.Java.System.Err.println(vbLf & "USAGE: java NegativeBinomial <k> <r> <p>" & vbLf)
            Oracle.Java.System.Err.println("NegativeBinomial computes the probability mass function for the negative binomial distribution with parameters (r,p) at value k." & vbLf)
            Environment.[Exit](0)
        End If

        Dim k As Integer = Convert.ToInt32(args(0))
        Dim r As Integer = Convert.ToInt32(args(1))
        Dim p As Double = Convert.ToDouble(args(2))
        Console.WriteLine(pmf(r - 1, k + r - 1, p))
    End Sub

End Module
