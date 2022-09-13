#Region "Microsoft.VisualBasic::f7234dc5a6e6e51392a2fe1356f52506, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\tRNA.vb"

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

    '   Total Lines: 30
    '    Code Lines: 14
    ' Comment Lines: 8
    '   Blank Lines: 8
    '     File Size: 1.11 KB


    '     Class tRNA
    ' 
    '         Properties: Anticodon, Codons, Table
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    Public Class tRNA : Inherits MetaCyc.File.DataFiles.Slots.Object

        <MetaCycField()> Public Property Anticodon As String
        <MetaCycField(type:=MetaCycField.Types.TStr)>
        Public Property Codons As List(Of String)

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.classes
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As tRNA
        '    Dim NewObj As tRNA = New tRNA

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of MetaCyc.File.DataFiles.Slots.tRNA) _
        '        (MetaCyc.File.AttributeValue.Object.Format(Genes.AttributeList, e), NewObj)

        '    NewObj.Anticodon = NewObj.Object("")
        '    NewObj.Codons = StringQuery(NewObj.Object, "")

        '    Return NewObj
        'End Operator
    End Class
End Namespace
