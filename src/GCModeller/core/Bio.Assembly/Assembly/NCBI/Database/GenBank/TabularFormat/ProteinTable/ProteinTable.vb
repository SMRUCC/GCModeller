#Region "Microsoft.VisualBasic::1a054cf611653e72685499ca1cfea370, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\ProteinTable\ProteinTable.vb"

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

    '   Total Lines: 62
    '    Code Lines: 53
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 2.50 KB


    '     Class ProteinTable
    ' 
    '         Properties: Proteins
    ' 
    '         Function: CreateObject, Load, ToPTT
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Class ProteinTable

        Public Property Proteins As ProteinDescription()

        Public Shared Function Load(Path As String) As ProteinTable
            Dim bufs As String() = IO.File.ReadAllLines(Path)
            Dim schema As New Index(Of String)(bufs.First.Split(ASCII.TAB))
            Dim LQuery = LinqAPI.Exec(Of ProteinDescription) <=
 _
                From str As String
                In bufs.Skip(1).AsParallel
                Select prot = CreateObject(str, schema)
                Order By prot.Locus_tag Ascending

            Return New ProteinTable With {.Proteins = LQuery}
        End Function

        Public Overloads Shared Function CreateObject(str$, schema As Index(Of String)) As ProteinDescription
            Dim tokens As String() = Strings.Split(str, vbTab)
            Dim getValue = Function(key$)
                               Dim i% = schema(key)

                               If i = -1 Then
                                   Return ""
                               Else
                                   Return tokens(i)
                               End If
                           End Function
            Dim protein As New ProteinDescription With {
                .RepliconName = getValue(key:="#Replicon Name"),
                .RepliconAccession = getValue(key:="Replicon Accession"),
                .Start = CInt(getValue(key:="Start")),
                .Stop = CInt(getValue(key:="Stop")),
                .Strand = getValue(key:="Strand"),
                .GeneID = getValue(key:="GeneID"),
                .Locus = getValue(key:="Locus"),
                .Locus_tag = getValue(key:="Locus tag"),
                .Product = getValue(key:="Protein product"),
                .Length = CInt(getValue(key:="Length")),
                .COG = getValue(key:="COG"),
                .ProteinName = getValue(key:="Protein name")
            }

            Return protein
        End Function

        Public Function ToPTT() As PTT
            Return New PTT With {
                .GeneObjects = Proteins _
                    .Select(Function(prot) prot.ToPTTGene) _
                    .ToArray
            }
        End Function
    End Class
End Namespace
