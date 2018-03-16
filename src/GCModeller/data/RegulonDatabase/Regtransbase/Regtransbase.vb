#Region "Microsoft.VisualBasic::3fe20cefec790a5fa7fd0f9fada2905f, data\RegulonDatabase\Regtransbase\Regtransbase.vb"

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
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ExportBindingSites, ExportRegulators, GetGenes, GetRegulators, GetSites
    '                   Trim
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Uri

Namespace Regtransbase

    Public Class Database

        Dim DbReflector As MySqli

        Sub New(MySQL As ConnectionUri)
            DbReflector = New MySqli(MySQL)
        End Sub

        Public Function GetGenes() As Regtransbase.StructureObjects.Gene()
            Return DbReflector.Query(Of Regtransbase.StructureObjects.Gene)("select * from genes").ToArray
        End Function

        Public Function GetSites() As Regtransbase.StructureObjects.Sites()
            Return DbReflector.Query(Of Regtransbase.StructureObjects.Sites)("select * from sites").ToArray
        End Function

        Public Function GetRegulators() As Regtransbase.StructureObjects.Regulator()
            Return DbReflector.Query(Of Regtransbase.StructureObjects.Regulator)("select * from regulators").ToArray
        End Function

        Public Function ExportBindingSites(Optional TryAutoFix As Boolean = False) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
            Dim Table = DbReflector.Query(Of Regtransbase.StructureObjects.Sites)("select * from sites")
            Dim LQuery = (From site As Regtransbase.StructureObjects.Sites In Table
                          Let Fsa As SMRUCC.genomics.SequenceModel.FASTA.FastaSeq = StructureObjects.Sites.ExportFasta(site, TryAutoFix)
                          Where Not Fsa Is Nothing
                          Select Fsa).ToArray
            Return LQuery
        End Function

        Public Function ExportRegulators(Optional TryAutoFix As Boolean = False) As SMRUCC.genomics.SequenceModel.FASTA.FastaFile
            Dim Table = DbReflector.Query(Of Regtransbase.StructureObjects.Regulator)("select * from regulators")
            Dim Genes = DbReflector.Query(Of Regtransbase.StructureObjects.Gene)("select * from genes").ToArray '在其中居然会有以TGA开头的基因序列
            Dim LQuery = (From regulator As Regtransbase.StructureObjects.Regulator
                          In Table
                          Let fsa As SMRUCC.genomics.SequenceModel.FASTA.FastaSeq = Regtransbase.StructureObjects.Regulator.ExportFasta(regulator, Genes, TryAutoFix)
                          Where Not fsa Is Nothing AndAlso Len(fsa.SequenceData) > 0
                          Select fsa).ToArray
            Return CType(LQuery, SMRUCC.genomics.SequenceModel.FASTA.FastaFile)
        End Function

        Private Shared Function Trim(str As String) As String
            Dim sBuilder As List(Of Char) = New List(Of Char)
            For Each ch In str
                If ch = " "c Then
                Else
                    Call sBuilder.Add(ch)
                End If
            Next

            Return sBuilder.ToArray
        End Function
    End Class
End Namespace
