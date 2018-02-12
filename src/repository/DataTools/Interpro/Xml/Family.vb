#Region "Microsoft.VisualBasic::44af93241d380cd687926499663ac19b, DataTools\Interpro\Xml\Family.vb"

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

    '     Class Family
    ' 
    '         Properties: Includes, Interpro, Name, Pfam
    ' 
    '         Function: CreateObject, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq

Namespace Interpro.Xml

    Public Class Family
        Public Property Interpro As String
        Public Property Name As String
        Public Property Pfam As String()
        Public Property Includes As String()

        Public Overrides Function ToString() As String
            Return $"[{Interpro}] {Name}; //{Pfam.JoinBy(", ")}"
        End Function

        Public Shared Function CreateObject(interpro As Xml.Interpro, dict As Dictionary(Of String, Xml.Interpro)) As Family
            Dim includes = interpro.contains.Select(Function(x) dict(x.ipr_ref))
            Dim Pfam = (From x In interpro.member_list
                        Where String.Equals(x.db, "PFAM", StringComparison.OrdinalIgnoreCase)
                        Select x.dbkey).ToArray
            Dim LQuery = (From inter As Xml.Interpro
                          In includes
                          Select (From d As DbXref
                                  In inter.member_list
                                  Where String.Equals(d.db, "PFAM", StringComparison.OrdinalIgnoreCase)
                                  Select d.dbkey).ToArray).ToArray.ToVector

            Return New Family With {
                .Interpro = interpro.id,
                .Name = interpro.short_name,
                .Pfam = Pfam,
                .Includes = LQuery
            }
        End Function
    End Class
End Namespace
