#Region "Microsoft.VisualBasic::c3b25d7d88975400053022e3b35d5256, R#\seqtoolkit\models\AssembleResult.vb"

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

'   Total Lines: 12
'    Code Lines: 9
' Comment Lines: 0
'   Blank Lines: 3
'     File Size: 260 B


' Class AssembleResult
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: GetAssembledSequence
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop.[CType]

Public Class AssembleResult : Implements ICTypeDataframe

    Dim assembly As String
    Dim reads As String()

    Sub New(result As String, reads As String())
        Me.reads = reads
        Me.assembly = result
    End Sub

    Public Function GetAssembledSequence() As String
        Return assembly
    End Function

    Public Function toDataframe() As dataframe Implements ICTypeDataframe.toDataframe
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }
        Dim view = viewAssembles(Me) _
            .LineTokens _
            .Skip(2) _
            .ToArray

        view = New String() {GetAssembledSequence()} _
            .JoinIterates(view) _
            .ToArray

        Call df.add("AssembleResult", view)

        Return df
    End Function

    Friend Shared Function viewAssembles(asm As AssembleResult) As String
        Dim sb As New StringBuilder

        Using text As New StringWriter(sb)
            Call asm.reads.TableView(asm.GetAssembledSequence, text)
        End Using

        Return sb.ToString
    End Function
End Class
