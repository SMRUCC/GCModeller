﻿#Region "Microsoft.VisualBasic::1ef23e59290a19a089f28ac1d079c6ee, core\Bio.Assembly\Assembly\MetaCyc\File\Tabular DataFiles\ReactionEquation.vb"

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

    '   Total Lines: 39
    '    Code Lines: 30 (76.92%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 9 (23.08%)
    '     File Size: 1.22 KB


    '     Class ReactionEquation
    ' 
    '         Properties: Products, Substrates
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Namespace Assembly.MetaCyc.File.TabularDataFiles

    Public Class ReactionEquation
        Public Property Substrates As String()
        Public Property Products As String()

        Public Overrides Function ToString() As String
            Dim sbr As StringBuilder = New StringBuilder(256)

            For Each e As String In Substrates
                sbr.Append("[" & e & "] + ")
            Next
            sbr.Remove(sbr.Length - 2, 2)
            sbr.Append(" <----> ")
            For Each e As String In Products
                sbr.Append("[" & e & "] + ")
            Next
            sbr.Remove(sbr.Length - 2, 2)

            Return sbr.ToString
        End Function

        Public Shared Widening Operator CType(e As String) As ReactionEquation
            Dim Eqstr As String() = Strings.Split(e, "  <-->  ")
            Dim newobj As New ReactionEquation

            With newobj
                .Substrates = Strings.Split(Eqstr.First, " + ")
                .Products = Strings.Split(Eqstr.Last, " + ")
            End With

            Return newobj
        End Operator

        Public Const INDEX As Integer = 2
    End Class
End Namespace
