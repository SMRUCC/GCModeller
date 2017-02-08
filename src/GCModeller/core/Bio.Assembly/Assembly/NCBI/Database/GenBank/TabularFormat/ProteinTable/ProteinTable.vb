#Region "Microsoft.VisualBasic::5238356072e54dbe918eb2b941ed5a0c, ..\GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\TabularFormat\ProteinTable.vb"

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

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.NCBI.GenBank.TabularFormat

    Public Class ProteinTable : Inherits ITextFile

        Public Property Proteins As ProteinDescription()

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Throw New NotImplementedException()
        End Function

        Public Shared Function Load(Path As String) As ProteinTable
            Dim bufs As String() = IO.File.ReadAllLines(Path)
            Dim LQuery = LinqAPI.Exec(Of ProteinDescription) <=
                From str As String
                In bufs.Skip(1).AsParallel
                Select prot = CreateObject(str)
                Order By prot.Locus_tag Ascending

            Return New ProteinTable With {
                .FilePath = Path,
                .Proteins = LQuery
            }
        End Function

        Public Overloads Shared Function CreateObject(str As String) As ProteinDescription
            Dim tokens As String() = Strings.Split(str, vbTab)
            Dim i As int = 0
            Dim protein As New ProteinDescription With {
                .RepliconName = tokens(++i),
                .RepliconAccession = tokens(++i),
                .Start = CInt(Val(tokens(++i))),
                .Stop = CInt(Val(tokens(++i))),
                .Strand = tokens(++i),
                .GeneID = tokens(++i),
                .Locus = tokens(++i),
                .Locus_tag = tokens(++i),
                .Product = tokens(++i),
                .Length = CInt(Val(tokens(++i))),
                .COG = tokens(++i),
                .ProteinName = tokens(++i)
            }

            Return Protein
        End Function

        Public Function ToPTT() As PTT
            Return New PTT With {
                .GeneObjects = Proteins.ToArray(Function(prot) prot.ToPTTGene)
            }
        End Function
    End Class
End Namespace
