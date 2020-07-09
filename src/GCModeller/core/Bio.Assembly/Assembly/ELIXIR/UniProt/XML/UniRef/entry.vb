#Region "Microsoft.VisualBasic::15a18beae505f3de40cfa4d97e51ef3c, Bio.Assembly\Assembly\ELIXIR\UniProt\XML\UniRef\entry.vb"

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

    '     Class entry
    ' 
    '         Properties: id, members, representativeMember
    ' 
    '         Function: ToString
    ' 
    '     Class representativeMember
    ' 
    '         Properties: dbReference, sequence, source_organism, UniProtKB_accession
    ' 
    '         Function: ToString
    ' 
    '     Module Extensions
    ' 
    '         Function: PopulateALL
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Text.Xml.Linq

Namespace Assembly.Uniprot.XML.UniRef

    ''' <summary>
    ''' http://uniprot.org/uniref
    ''' </summary>
    Public Class entry : Implements INamedValue

        <XmlAttribute>
        Public Property id As String Implements IKeyedEntity(Of String).Key
        Public Property representativeMember As representativeMember
        <XmlElement("member")>
        Public Property members As representativeMember()

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Class

    Public Class representativeMember

        Public Property dbReference As dbReference
        Public Property sequence As sequence

        Public ReadOnly Property UniProtKB_accession As String
            Get
                Return dbReference("UniProtKB accession")
            End Get
        End Property

        Public ReadOnly Property source_organism As String
            Get
                Return dbReference("source organism")
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return UniProtKB_accession
        End Function
    End Class

    Public Module Extensions

        Public Function PopulateALL(path$) As IEnumerable(Of entry)
            Return path.LoadXmlDataSet(Of entry)(NameOf(entry), xmlns:="http://uniprot.org/uniref")
        End Function
    End Module
End Namespace
