#Region "Microsoft.VisualBasic::b17df84d7f99ea3af10b259b41fdd3b4, GCModeller\analysis\SequenceToolkit\SequencePatterns\Motif\ResidueSite.vb"

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

    '   Total Lines: 100
    '    Code Lines: 62
    ' Comment Lines: 25
    '   Blank Lines: 13
    '     File Size: 3.43 KB


    '     Class ResidueSite
    ' 
    '         Properties: AsChar, Bits, PWM, Site
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: Complement, ToChar, ToString
    ' 
    '         Sub: Assign
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language

Namespace Motif

    ''' <summary>
    ''' A column in the motif
    ''' </summary>
    <XmlType("Residue")> Public Class ResidueSite : Implements IAddressOf

        <XmlAttribute> Public Property Site As Integer Implements IAddressOf.Address
        ''' <summary>
        ''' ATGC/
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property PWM As Double()
        ''' <summary>
        ''' Information content
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Bits As Double

        Private Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            Me.Site = address
        End Sub

        ''' <summary>
        ''' ATGC -> TACG
        ''' </summary>
        ''' <returns></returns>
        Public Function Complement() As ResidueSite
            Dim A As Double = PWM(0), T As Double = PWM(1), G As Double = PWM(2), C As Double = PWM(3)
            Dim cA As Double = T, cT As Double = A, cG As Double = C, cC As Double = G
            Dim rsd As New ResidueSite With {
                .PWM = {cA, cT, cG, cC},
                .Bits = Bits,
                .Site = Site
            }
            Return rsd
        End Function

        Sub New()
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="nt"></param>
        Sub New(nt As Char)
            If nt = "A"c OrElse nt = "a"c Then
                PWM = New Double() {1, 0, 0, 0}
            ElseIf nt = "T"c OrElse nt = "t"c Then
                PWM = New Double() {0, 1, 0, 0}
            ElseIf nt = "G"c OrElse nt = "g"c Then
                PWM = New Double() {0, 0, 1, 0}
            ElseIf nt = "C"c OrElse nt = "c"c Then
                PWM = New Double() {0, 0, 0, 1}
            Else  ' N, -, ., *
                PWM = New Double() {0.25, 0.25, 0.25, 0.25}
            End If

            Bits = 1.5
        End Sub

        Public Overrides Function ToString() As String
            Dim ATGC As String = New String({ToChar("A"c, PWM(0)), ToChar("T"c, PWM(1)), ToChar("G"c, PWM(2)), ToChar("C"c, PWM(3))})
            Return $"{ATGC}   //({Math.Round(Bits, 2)} bits) [{Math.Round(PWM(0), 2)}, {Math.Round(PWM(1), 2)}, {Math.Round(PWM(2), 2)}, {Math.Round(PWM(3), 2)}];"
        End Function

        Public ReadOnly Property AsChar As Char
            Get
                Dim mxInd As Integer = PWM.MaxIndex

                If PWM.Length = 4 Then
                    Return ToChar("ATGC"(mxInd), PWM(mxInd))
                Else
                    Return ToChar(SequenceModel.AA(mxInd), PWM(mxInd))
                End If
            End Get
        End Property

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="ch">大写的</param>
        ''' <param name="p"></param>
        ''' <returns></returns>
        Public Shared Function ToChar(ch As Char, p As Double) As Char
            If p < 0.65 Then
                ch = Char.ToLower(ch)
            End If
            Return ch
        End Function

        Public Shared Narrowing Operator CType(x As ResidueSite) As Double()
            Return x.PWM
        End Operator
    End Class
End Namespace
