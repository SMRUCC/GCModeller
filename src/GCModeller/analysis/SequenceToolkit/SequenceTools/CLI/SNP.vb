#Region "Microsoft.VisualBasic::dab909649aa0dc21a54b9eab1f5211d1, ..\GCModeller\analysis\SequenceToolkit\SequenceTools\CLI\SNP.vb"

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

Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.SNP
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module Utilities

    <ExportAPI("/SNP",
               Usage:="/SNP /in <nt.fasta> [/ref 0 /pure /monomorphic]")>
    <ParameterInfo("/in", False, AcceptTypes:={GetType(FastaFile)}, Description:="")>
    <ParameterInfo("/ref", True, AcceptTypes:={GetType(Integer)})>
    <ParameterInfo("/pure", True, AcceptTypes:={GetType(Boolean)})>
    Public Function SNP(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim pure As Boolean = args.GetBoolean("/pure")
        Dim monomorphic As Boolean = args.GetBoolean("/monomorphic")
        Dim nt As New FastaFile([in])
        Dim ref As Integer = args.GetInt32("/ref")
        Dim json As String = [in].TrimFileExt & ".SNPs.args.json"
        Return nt.ScanSNPs(ref, pure, monomorphic).GetJson.SaveTo(json)
    End Function
End Module

