#Region "Microsoft.VisualBasic::b719a3c37a4444b4ac39ae46927b701f, ..\interops\RNA-Seq\.NET Bio\Bio.Extensions\Extensions.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<PackageNamespace(".NET_Bio.Extensions", Description:="Data type convert between GCModeller and Microsoft .NET Bio", Publisher:="amethyst.asuka@gcmodeller.org")>
Public Module Extensions

    <ExportAPI("ToFasta", Info:="Convert the .NET Bio type sequence object as a GCModeller fasta sequence object.")>
    <Extension> Public Function ToFasta(BioSequence As Bio.ISequence) As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken
        Dim SequenceData As String = New Bio.Sequence(BioSequence).ToString
        Dim attr = (From dt In BioSequence.Metadata Select $"{dt.Key}:={dt.Value?.ToString}").ToArray
        Dim Fasta = New LANS.SystemsBiology.SequenceModel.FASTA.FastaToken With {.Attributes = attr, .SequenceData = SequenceData}
        Return Fasta
    End Function

End Module
