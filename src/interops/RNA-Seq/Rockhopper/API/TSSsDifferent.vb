#Region "Microsoft.VisualBasic::525eeedc7c1d759db88b5f45710b3e45, RNA-Seq\Rockhopper\API\TSSsDifferent.vb"

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

    '     Class TSSsDifferent
    ' 
    '         Properties: Condition1, Condition2, GeneID, Pathway, TSSs_Condition1
    '                     TSSs_Condition2, TTSs_Condition1, TTSs_Condition2
    ' 
    '         Function: HaveBoth, HaveTSSs, HaveTTSs, ToString, TSSChanged
    '                   TTSChnaged
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Linq
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions

Namespace AnalysisAPI

    ''' <summary>
    ''' TSS位点差异性
    ''' </summary>
    Public Class TSSsDifferent
        Public Property GeneID As String
        Public Property TSSs_Condition1 As Long
        Public Property TSSs_Condition2 As Long
        Public Property TTSs_Condition1 As Long
        Public Property TTSs_Condition2 As Long

#Region "产生差异的两个实验条件的描述"

        Public Property Condition1 As String
        Public Property Condition2 As String
#End Region

        Public Property Pathway As String()

        Public Overrides Function ToString() As String
            Return GeneID
        End Function

        Public Function HaveBoth() As Boolean
            Return TSSs_Condition1 <> 0 AndAlso TSSs_Condition2 <> 0 AndAlso TTSs_Condition1 <> 0 AndAlso TTSs_Condition2 <> 0
        End Function

        Public Function HaveTSSs() As Boolean
            Return TSSs_Condition1 <> 0 AndAlso TSSs_Condition2 <> 0
        End Function

        Public Function HaveTTSs() As Boolean
            Return TTSs_Condition1 <> 0 AndAlso TTSs_Condition2 <> 0
        End Function

        Public Function TSSChanged() As Boolean
            If Not HaveTSSs() Then
                Return False
            End If
            Return Math.Abs(TSSs_Condition1 - TSSs_Condition2) > 10
        End Function

        Public Function TTSChnaged() As Boolean
            If Not HaveTTSs() Then
                Return False
            End If
            Return Math.Abs(TTSs_Condition1 - TTSs_Condition2) > 10
        End Function
    End Class

End Namespace
