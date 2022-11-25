#Region "Microsoft.VisualBasic::e5018f5d821028e362dcf7f7a2fda350, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\GCMLDocBuilder\UniqueIdTrimer.vb"

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

    '   Total Lines: 36
    '    Code Lines: 29
    ' Comment Lines: 0
    '   Blank Lines: 7
    '     File Size: 1.21 KB


    '     Class UniqueIdTrimer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Invoke
    ' 
    '         Sub: Remove
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem

Namespace Builder

    Public Class UniqueIdTrimer : Inherits IBuilder

        Sub New(MetaCyc As DatabaseLoadder, Model As BacterialModel)
            MyBase.New(MetaCyc, Model)
        End Sub

        Public Overrides Function Invoke() As BacterialModel
            Dim MetaCycRxns = MetaCyc.GetReactions
            For i As Integer = 0 To MetaCycRxns.NumOfTokens - 1
                Dim Rxn = MetaCycRxns(i)

                Call Remove(Rxn.Left)
                Call Remove(Rxn.Right)
            Next

            Return MyBase.Model
        End Function

        Private Shared Sub Remove(TargetList As List(Of String))
            Dim Collection As String() = TargetList.ToArray
            For Each Str As String In Collection
                If Str.First() = "|"c AndAlso Str.Last() = "|"c Then
                    Call TargetList.Add(Mid(Str, 2, Len(Str) - 2))
                    Call TargetList.Remove(Str)
                End If
            Next
        End Sub
    End Class
End Namespace
