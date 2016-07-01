#Region "Microsoft.VisualBasic::83ad8caa61763b9652e4d8df6bb7e698, ..\GCModeller\analysis\ProteinTools\ProteinTools.Family\FileSystem\PfamString.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace FileSystem

    <XmlType("PfamString", [Namespace]:="http://gcmodeller.org/tools/sanger-pfam/prot_family")>
    Public Class PfamString ': Implements SequenceModel.FASTA.I_FastaToken

        <XmlAttribute> Public Property LocusTag As String
        <XmlElement> Public Property PfamString As String
        <XmlAttribute> Public Property Length As Integer
        <XmlAttribute> Public Property Domains As String()
        ' <XmlElement> Public Property Description As String

        Public Overrides Function ToString() As String
            Return $"{LocusTag}  {PfamString}"
        End Function

        Public Function AsPfamString() As Sanger.Pfam.PfamString.PfamString
            Return New Sanger.Pfam.PfamString.PfamString With {
                .Length = Length,
                .PfamString = PfamString.Split("+"c),
                .ProteinId = LocusTag,'                .Description = Description,
                .Domains = Domains
            }
        End Function

        Public Shared Function CreateObject(stringPfam As Sanger.Pfam.PfamString.PfamString) As PfamString
            Return New PfamString With {
                .Domains = stringPfam.Domains,'.Description = stringPfam.Description,
                .PfamString = stringPfam.PfamString.JoinBy("+"),
                .Length = stringPfam.Length,
                .LocusTag = stringPfam.ProteinId
            }
        End Function
    End Class
End Namespace
