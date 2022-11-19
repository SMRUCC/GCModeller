#Region "Microsoft.VisualBasic::683aa3465b918ac2074ebdb639b762e4, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\FileSystem\FASTA\FastaCollection.vb"

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

    '   Total Lines: 40
    '    Code Lines: 28
    ' Comment Lines: 4
    '   Blank Lines: 8
    '     File Size: 1.61 KB


    '     Class FastaCollection
    ' 
    '         Properties: DNAseq, DNASourceFilePath, Origin, OriginSourceFile, ProteinSourceFile
    '                     protseq
    ' 
    '         Function: Load, LoadGeneObjects, LoadProteins
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Text
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.MetaCyc.File.FileSystem.FastaObjects

    Public Class FastaCollection

        Public Property DNAseq As GeneObject()
        Public Property protseq As Proteins()

        Public Property ProteinSourceFile As String
        Public Property DNASourceFilePath As String

        ''' <summary>
        ''' The complete genome sequence of the target species.(目标对象的全基因组序列)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Origin As FastaSeq
        Public Property OriginSourceFile As String

        Public Overloads Shared Function Load(Of T As FastaSeq)(FilePath As String, Optional Explicit As Boolean = True) As T()
            Dim FASTA As FastaFile = FastaFile.Read(FilePath, Explicit)
            Dim type As Type = GetType(T)
            Dim LQuery As T() = (From Fa As FastaSeq
                                 In FASTA.AsParallel
                                 Select DirectCast(Activator.CreateInstance(type, {Fa}), T)).ToArray
            Return LQuery
        End Function

        Public Shared Function LoadGeneObjects(file As String, Optional explicit As Boolean = True) As GeneObject()
            Return Load(Of GeneObject)(file, explicit)
        End Function

        Public Shared Function LoadProteins(file As String, Optional explicit As Boolean = True) As Proteins()
            Return Load(Of Proteins)(file, explicit)
        End Function
    End Class
End Namespace
