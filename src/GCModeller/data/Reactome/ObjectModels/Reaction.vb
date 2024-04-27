#Region "Microsoft.VisualBasic::3d0cbd7c86383c156d4827880777701b, G:/GCModeller/src/GCModeller/data/Reactome//ObjectModels/Reaction.vb"

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

    '   Total Lines: 42
    '    Code Lines: 34
    ' Comment Lines: 1
    '   Blank Lines: 7
    '     File Size: 1.38 KB


    '     Class Reaction
    ' 
    '         Properties: Comments, EC, Equation, Id, Names
    '                     Regulations, Reversible
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Namespace ObjectModels

    Public Class Reaction

        Protected Friend _innerEqur As Equation

        Public Property Equation As String
            Get
                If _innerEqur Is Nothing Then
                    Return ""
                End If
                Return EquationBuilder.ToString(_innerEqur)
            End Get
            Set(value As String)
                If String.IsNullOrEmpty(value) Then
                    Return
                End If
                _innerEqur = EquationBuilder.CreateObject(value)
            End Set
        End Property

        Public Property Regulations As KeyValuePair(Of String, String)()
        Public Property Id As String
        ' Public Property Enzymes As KeyValuePair(Of String, String)()
        Public Property EC As String
        Public Property Comments As String()
        Public Property Names As String()

        Public ReadOnly Property Reversible As Boolean
            Get
                Return _innerEqur.Reversible
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1}", EC, Comments.FirstOrDefault)
        End Function
    End Class
End Namespace
