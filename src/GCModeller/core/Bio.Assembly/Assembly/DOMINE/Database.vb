#Region "Microsoft.VisualBasic::e95c6ff6c8ca0c461d8ad50e1e9b7698, GCModeller\core\Bio.Assembly\Assembly\DOMINE\Database.vb"

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

    '   Total Lines: 56
    '    Code Lines: 45
    ' Comment Lines: 0
    '   Blank Lines: 11
    '     File Size: 2.10 KB


    '     Class Database
    ' 
    '         Properties: FilePath, Go, Interaction, MimeType, Pfam
    '                     PGMap
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetInteractionDomains, (+2 Overloads) Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text

Namespace Assembly.DOMINE

    <XmlRoot("DOMINE-DATABASE")>
    Public Class Database : Implements ISaveHandle, IFileReference

        Sub New()
        End Sub

        Sub New(file As String)
            FilePath = file
        End Sub

        Public Property Interaction As DOMINE.Tables.Interaction()
        Public Property Pfam As DOMINE.Tables.Pfam()
        Public Property Go As DOMINE.Tables.Go()
        Public Property PGMap As DOMINE.Tables.PGMap()
        Public Property FilePath As String Implements IFileReference.FilePath

        Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Function Save(Path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
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

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
