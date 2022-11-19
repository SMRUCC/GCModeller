#Region "Microsoft.VisualBasic::7001669afafe9cce53ee4044a503c15a, GCModeller\foundation\OBO_Foundry\IO\Reflection\Field.vb"

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

    '   Total Lines: 51
    '    Code Lines: 27
    ' Comment Lines: 16
    '   Blank Lines: 8
    '     File Size: 1.55 KB


    '     Class Field
    ' 
    '         Properties: index, name, toLower
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: ToString
    ' 
    '         Sub: SetFields
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization.JSON

Namespace IO.Reflection

    ''' <summary>
    ''' Obo term field or tabular indexed column
    ''' </summary>
    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class Field : Inherits Attribute

        Public ReadOnly Property name As String
        Public ReadOnly Property toLower As Boolean

        ''' <summary>
        ''' Tablular column index
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property index As Integer

        ''' <summary>
        ''' Init with a field name in the obo term.
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <param name="toLower"></param>
        Sub New(Optional Name As String = "", Optional toLower As Boolean = True)
            Me.name = Name
            Me.toLower = toLower
        End Sub

        ''' <summary>
        ''' Init with a field index in the table columns
        ''' </summary>
        ''' <param name="Index"></param>
        Sub New(Index As Integer)
            Me.index = Index
        End Sub

        Friend Sub SetFields(Optional name$ = Nothing, Optional toLower As Boolean? = Nothing)
            If Not name.StringEmpty Then
                _name = name
            End If
            If Not toLower Is Nothing Then
                _toLower = toLower
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
