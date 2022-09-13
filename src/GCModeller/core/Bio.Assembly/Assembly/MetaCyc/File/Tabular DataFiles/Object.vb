#Region "Microsoft.VisualBasic::ffdb340015293834bb9c65aa2bfbc465, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\Tabular DataFiles\Object.vb"

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

    '   Total Lines: 68
    '    Code Lines: 58
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 2.37 KB


    '     Class [Object]
    ' 
    '         Properties: Activators, Cofactors, Inhibitors, Name, Pathways
    '                     ReactionEquation, SubunitComposition, UniqueId
    ' 
    '         Function: GetData, ToString
    ' 
    '         Sub: Append
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Namespace Assembly.MetaCyc.File.TabularDataFiles

    Public Class [Object]
        Public Property UniqueId As String
        Public Property Name As String
        Public Property ReactionEquation As ReactionEquation
        Public Property Pathways As String()
        Public Property Cofactors As String()
        Public Property Activators As String()
        Public Property Inhibitors As String()
        Public Property SubunitComposition As String

        Public Overrides Function ToString() As String
            Dim sbr As StringBuilder = New StringBuilder(512)

            sbr.Append(UniqueId & ", ")
            sbr.Append(Name & ", ")
            sbr.Append(ReactionEquation.ToString & ", ")
            For Each e As String In Pathways
                Append(e, sbr)
            Next
            For Each e As String In Cofactors
                Append(e, sbr)
            Next
            For Each e As String In Activators
                Append(e, sbr)
            Next
            For Each e As String In Inhibitors
                Append(e, sbr)
            Next

            sbr.Append(SubunitComposition)

            Return sbr.ToString
        End Function

        Friend Sub Append(e As String, ByRef sbr As StringBuilder)
            If String.IsNullOrEmpty(e) Then
                sbr.Append("NULL, ")
            Else
                sbr.AppendFormat("{0}, ", e)
            End If
        End Sub

        Public Shared Function GetData(e As RecordLine) As [Object]
            Dim NewObj As New [Object]

            With NewObj
                .UniqueId = e.Data(0)
                .Name = e.Data(1)
                .ReactionEquation = e.Data(ReactionEquation.INDEX)
                ReDim .Pathways(3)
                Call Array.ConstrainedCopy(e.Data, 3, .Pathways, 0, 4)
                ReDim .Cofactors(3)
                Call Array.ConstrainedCopy(e.Data, 7, .Cofactors, 0, 4)
                ReDim .Activators(3)
                Call Array.ConstrainedCopy(e.Data, 11, .Activators, 0, 4)
                ReDim .Inhibitors(3)
                Call Array.ConstrainedCopy(e.Data, 15, .Inhibitors, 0, 4)
                .SubunitComposition = e.Data(19)
            End With

            Return NewObj
        End Function
    End Class
End Namespace
