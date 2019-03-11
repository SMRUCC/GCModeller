#Region "Microsoft.VisualBasic::8cd8013211de63208566e3e761ecfce2, Bio.Assembly\Assembly\DOMINE\Database.vb"

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

    '     Class Database
    ' 
    '         Properties: Go, Interaction, Pfam, PGMap
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetInteractionDomains, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel

Namespace Assembly.DOMINE

    <XmlRoot("DOMINE-DATABASE")>
    Public Class Database : Inherits ITextFile

        Sub New()
        End Sub

        Sub New(file As String)
            FilePath = file
        End Sub

        Public Property Interaction As DOMINE.Tables.Interaction()
        Public Property Pfam As DOMINE.Tables.Pfam()
        Public Property Go As DOMINE.Tables.Go()
        Public Property PGMap As DOMINE.Tables.PGMap()

        Public Overrides Function Save(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean
            If String.IsNullOrEmpty(Path) Then
                Path = Me.FilePath
            End If

            Return Me.GetXml.SaveTo(Path, Encoding.ASCII)
        End Function

        Public Overloads Shared Widening Operator CType(FilePath As String) As DOMINE.Database
            Dim File As Database = FilePath.LoadXml(Of DOMINE.Database)()
            File.FilePath = FilePath
            Return File
        End Operator

        Public Function GetInteractionDomains(DomainId As String) As String()
            Dim LQuery = (From itr In Interaction Where String.Equals(itr.Domain1, DomainId) Select itr.Domain2).AsList
            LQuery += From itr In Interaction Where String.Equals(itr.Domain2, DomainId) Select itr.Domain1
            Return LQuery.ToArray
        End Function
    End Class
End Namespace
