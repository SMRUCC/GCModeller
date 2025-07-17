#Region "Microsoft.VisualBasic::b2928fb9ba47253828ce7d11b471b937, core\Bio.Assembly\Assembly\MetaCyc\File\FileSystem\FASTA\FastaCollection.vb"

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
'    Code Lines: 28 (70.00%)
' Comment Lines: 4 (10.00%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 8 (20.00%)
'     File Size: 1.65 KB


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

Imports System.ComponentModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.MetaCyc.File.FileSystem.FastaObjects

    Public Class FastaCollection

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <Description("data/dnaseq.fsa")>
        Public ReadOnly Property DNAseq As GeneObject()

        <Description("data/protseq.fsa")>
        Public ReadOnly Property protseq As ProteinSeq()

        ''' <summary>
        ''' The complete genome sequence of the target species.
        ''' </summary>
        ''' <remarks>
        ''' (目标对象的全基因组序列)
        ''' </remarks>
        Public ReadOnly Property Origin As FastaSeq

        ReadOnly ProteinSourceFile As String
        ReadOnly DNASourceFilePath As String
        ReadOnly OriginSourceFile As String

        Sub New(workspace As Workspace)
            ProteinSourceFile = $"{workspace.dir}/protseq.fsa"
            DNASourceFilePath = $"{workspace.dir}/dnaseq.fsa"

            DNAseq = LoadGeneObjects(DNASourceFilePath)
            protseq = LoadProteins(ProteinSourceFile)
        End Sub

        Public Overloads Shared Function Load(Of T As FastaSeq)(filePath As String, Optional explicit As Boolean = True) As IEnumerable(Of T)
            Dim fasta As FastaFile = FastaFile.Read(filePath, explicit)
            Dim type As Type = GetType(T)

            Return From fa As FastaSeq
                   In fasta
                   Let castObj = Activator.CreateInstance(type, {fa})
                   Select DirectCast(castObj, T)
        End Function

        Public Shared Function LoadGeneObjects(file As String, Optional explicit As Boolean = True) As GeneObject()
            Return Load(Of GeneObject)(file, explicit).ToArray
        End Function

        Public Shared Function LoadProteins(file As String, Optional explicit As Boolean = True) As ProteinSeq()
            Return Load(Of ProteinSeq)(file, explicit).ToArray
        End Function
    End Class
End Namespace
