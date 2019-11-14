#Region "Microsoft.VisualBasic::48b88fc2076b22eb03b8dd80508f8d4b, engine\Model\Models\Extensions.vb"

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

' Module Extensions
' 
'     Function: EvalEffects
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Xml.Models

<HideModuleName>
Public Module Extensions

    <Extension> Public Function EvalEffects(regMode As String) As Double
        If regMode.StringEmpty Then
            Return 0.25
        End If

        If regMode.TextEquals("repressor") Then
            Return -1
        ElseIf regMode.TextEquals("activator") Then
            Return 1
        Else
            Return 0.25
        End If
    End Function

    <Extension>
    Public Function CreateVector(protein As ProteinComposition) As NumericVector

    End Function

    Public Function ProteinFromVector(vector As NumericVector) As ProteinComposition

    End Function

    <Extension>
    Public Function CreateVector(rna As RNAComposition) As NumericVector

    End Function

    Public Function RNAFromVector(vector As NumericVector) As RNAComposition

    End Function
End Module
