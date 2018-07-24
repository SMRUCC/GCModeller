#Region "Microsoft.VisualBasic::435edf8ab64d1799a393500e25194f4d, Bio.Assembly\Assembly\MetaCyc\File\FileSystem\FASTA\Proteins.vb"

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

    '     Class Proteins
    ' 
    '         Properties: UniqueId
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Sub: Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.MetaCyc.File.FileSystem.FastaObjects

    Public Class Proteins : Inherits FastaSeq
        Dim Description As String

        Public ReadOnly Property UniqueId As String
            Get
                Return Me.Headers.Last.Split.First
            End Get
        End Property

        Public Shared Shadows Sub Save(Data As Proteins(), FilePath As String)
            Dim FASTA As FastaFile = New FastaFile
            Call FASTA.AddRange(Data)
            Call FASTA.Save(FilePath)
        End Sub

        Sub New()
        End Sub

        Sub New(fa As FastaSeq)
            Headers = fa.Headers
            SequenceData = fa.SequenceData
            Description = fa.Headers.Last
        End Sub
    End Class
End Namespace
