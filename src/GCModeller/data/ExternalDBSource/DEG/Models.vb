#Region "Microsoft.VisualBasic::02033d3853380dcb35752165949417dc, ..\GCModeller\data\ExternalDBSource\DEG\Models.vb"

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

Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DEG

    Public Class Annotations : Implements sIdEnumerable

        <Column("#DEG_AC")> Public Property DEG_AC As String Implements sIdEnumerable.Identifier
        <Column("#Gene_Name")> Public Property GeneName As String
        <Column("#Gene_Ref")> Public Property Gene_Ref As String
        <Column("#COG")> Public Property COG As String
        <Column("#Class")> Public Property [Class] As String
        <Column("#Function")> Public Property [Function] As String
        <Column("#Organism")> Public Property Organism As String
        <Column("#Refseq")> Public Property Refseq As String
        <Column("#Condition")> Public Property Condition As String

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]  {1}", DEG_AC, Gene_Ref)
        End Function

        Public Shared Function Load(CsvData As DocumentStream.File) As DEG.Annotations()
            Dim ChunkBuffer As DEG.Annotations() = Reflector.Convert(Of DEG.Annotations)(CsvData.DataFrame(), False).ToArray
            Return ChunkBuffer
        End Function

        Public Shared Function GetSpeciesId(AnnotiationCollection As DEG.Annotations()) As String()
            Dim LQuery = (From item In AnnotiationCollection Where Not String.IsNullOrEmpty(item.Organism) Select item.Organism Distinct Order By Organism Ascending).ToArray
            Return LQuery
        End Function

        Public Shared Function Load(AnnotionFile As String) As DEG.Annotations()
            Dim CsvData = DataImports.Imports(AnnotionFile, vbTab)
            Dim ChunkBuffer As DEG.Annotations() = Reflector.Convert(Of DEG.Annotations)(CsvData.DataFrame(), False).ToArray
            Return ChunkBuffer
        End Function
    End Class

    Public Class DEG_AminoAcidSequence : Inherits FastaToken

        Public ReadOnly Property DEGAccessionId As String
        Public ReadOnly Property SpeciesId As String
        Public ReadOnly Property GeneName As String

        Public Overloads Shared Function Load(FilePath As String) As DEG.DEG_AminoAcidSequence()
            Dim LQuery = (From FsaObject As FastaToken
                          In FastaFile.Read(FilePath)
                          Select DEG.DEG_AminoAcidSequence.CreateObject(FsaObject)).ToArray
            Return LQuery
        End Function

        Public Shared Function CreateObject(FsaObject As FastaToken) As DEG.DEG_AminoAcidSequence
            Dim strData As String = FsaObject.Attributes.First
            Dim Tokens As String() = strData.Split("_"c)
            Dim Sequence As DEG.DEG_AminoAcidSequence = New DEG_AminoAcidSequence

            Call FsaObject.CopyTo(Sequence)
            Return Sequence
        End Function
    End Class
End Namespace
