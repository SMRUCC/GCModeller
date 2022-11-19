#Region "Microsoft.VisualBasic::52455d0be5dc786403cd98ec174feb76, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\Schemas\Reflection\Schema.vb"

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
    '    Code Lines: 40
    ' Comment Lines: 4
    '   Blank Lines: 7
    '     File Size: 2.94 KB


    '     Class TableSchema
    ' 
    '         Properties: LinkInKeys, LinkOutKeys, Table, UnknownKeys
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetExternalLink, ToString
    ' 
    '     Class Path
    ' 
    '         Properties: ObjA, ObjB, Relationship
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace Assembly.MetaCyc.Schema.Reflection

    Public Class TableSchema
        Public Property Table As MetaCyc.File.DataFiles.Slots.Object.Tables
        Public Property LinkInKeys As Reflection.ExternalKey()
        Public Property LinkOutKeys As Reflection.ExternalKey()
        Public Property UnknownKeys As Reflection.ExternalKey()

        Sub New(Type As System.Type, Table As MetaCyc.File.DataFiles.Slots.Object.Tables)
            Dim PropertyCollection As System.Reflection.PropertyInfo() = (From [Property] As System.Reflection.PropertyInfo
                                                                              In Type.GetProperties
                                                                          Let attributes As Object() = [Property].GetCustomAttributes(attributeType:=TableSchema.ExternalLinkKey, inherit:=True)
                                                                          Where Not (attributes Is Nothing OrElse attributes.Count = 0)
                                                                          Select [Property]).ToArray
            Me.LinkOutKeys = GetExternalLink(PropertyCollection, ExternalKey.Directions.Out)
            Me.UnknownKeys = GetExternalLink(PropertyCollection, ExternalKey.Directions.Unknown)
            Me.LinkInKeys = GetExternalLink(PropertyCollection, ExternalKey.Directions.In)
            Me.Table = Table
        End Sub

        Protected Friend Shared ReadOnly ExternalLinkKey As System.Type = GetType(MetaCyc.Schema.Reflection.ExternalKey)

        Private Shared Function GetExternalLink(PropertyCollection As System.Reflection.PropertyInfo(), Direction As Reflection.ExternalKey.Directions) As Reflection.ExternalKey()
            Dim LQuery As Generic.IEnumerable(Of MetaCyc.Schema.Reflection.ExternalKey) =
                    From [Property] As System.Reflection.PropertyInfo
                    In PropertyCollection
                    Let Link As MetaCyc.Schema.Reflection.ExternalKey = DirectCast([Property].GetCustomAttributes(attributeType:=TableSchema.ExternalLinkKey, inherit:=True).First, MetaCyc.Schema.Reflection.ExternalKey)
                    Where Link.Direction = Direction
                    Select Link.Link([Property]) '
            Return LQuery.ToArray
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}, {1} external links", Table.ToString, LinkInKeys.Count + LinkOutKeys.Count + UnknownKeys.Count)
        End Function
    End Class

    ''' <summary>
    ''' ObjA ----> ObjB
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Path
        Public Property ObjA As String
        Public Property ObjB As String
        Public Property Relationship As String

        Public Overrides Function ToString() As String
            Return String.Format("{0} --{1}--> {2}", ObjA, ObjB, Relationship)
        End Function
    End Class
End Namespace
