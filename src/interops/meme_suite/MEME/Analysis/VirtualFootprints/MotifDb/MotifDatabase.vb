#Region "Microsoft.VisualBasic::c3fe726699f6b2091d90536dc3048dfd, ..\interops\meme_suite\MEME\Analysis\VirtualFootprints\MotifDb\MotifDatabase.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace Analysis.GenomeMotifFootPrints.MotifDb

    <XmlRoot("MotifDatabase", Namespace:="http://code.google.com/p/genome-in-code/meme/motif-database")>
    Public Class MotifDatabase

        <XmlAttribute("Database.BuildTime")>
        Public Property DatabaseBuildTime As String
        <XmlElement("MotifFamilies")>
        Public Property MotifFamilies As MotifFamily()
    End Class

    Public Class MotifFamily : Implements IKeyValuePairObject(Of String, Motif())

        Public Property Family As String Implements IKeyValuePairObject(Of String, Motif()).Identifier
        Public Property Motifs As Motif() Implements IKeyValuePairObject(Of String, Motif()).Value

        Public Overrides Function ToString() As String
            Return String.Format("{0} {1} motifs", Family, Motifs.Count)
        End Function
    End Class
End Namespace
