﻿#Region "Microsoft.VisualBasic::e6b836253cb52a5230598814107f53d7, core\Bio.Assembly\Assembly\MetaCyc\File\FileSystem\FASTA\Proteins.vb"

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

    '   Total Lines: 30
    '    Code Lines: 24 (80.00%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (20.00%)
    '     File Size: 859 B


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

Imports System.Text
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Assembly.MetaCyc.File.FileSystem.FastaObjects

    Public Class ProteinSeq : Inherits FastaSeq

        Public ReadOnly Property Description As String
        Public ReadOnly Property UniqueId As String

        Public Shared Shadows Sub Save(data As ProteinSeq(), filePath As String)
            Dim FASTA As FastaFile = New FastaFile
            Call FASTA.AddRange(data)
            Call FASTA.Save(filePath, Encoding.UTF8)
        End Sub

        Sub New()
        End Sub

        Sub New(fa As FastaSeq)
            Headers = fa.Headers
            SequenceData = fa.SequenceData

            Call TryParse(Headers(2), UniqueId)
        End Sub

        Private Overloads Shared Sub TryParse(title As String, ByRef uniqueId As String)
            Dim tokens As String() = title.Split

            uniqueId = tokens(0)
        End Sub
    End Class
End Namespace
