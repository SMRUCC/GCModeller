#Region "Microsoft.VisualBasic::1320ea17f005c4500f00090a9eb751a9, ..\GCModeller\shoalAPI\LinqProvider.vb"

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

Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular
Imports LANS.SystemsBiology.ProteinModel
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Linq.Framework.Provider

Public Module LinqProvider

    <LinqEntity("GeneObject", GetType(FileStream.GeneObject))>
    Public Function GetTabularGenes(FilePath As String) As IEnumerable
        Return FilePath.LoadCsv(Of FileStream.GeneObject)(False)
    End Function

    <LinqEntity("Metabolite", GetType(FileStream.Metabolite))>
    Public Function GetTabularMetabolites(FilePath As String) As IEnumerable
        Return FilePath.LoadCsv(Of FileStream.Metabolite)(False)
    End Function

    <LinqEntity("TranscriptUnit", GetType(FileStream.TranscriptUnit))>
    Public Function GetTabularTranscriptUnit(FilePath As String) As IEnumerable
        Return FilePath.LoadCsv(Of FileStream.TranscriptUnit)(False)
    End Function

    <LinqEntity("Transcript", GetType(FileStream.Transcript))>
    Public Function GetTabularTranscripts(FilePath As String) As IEnumerable
        Return FilePath.LoadCsv(Of FileStream.Transcript)(False)
    End Function

    <LinqEntity("ProteinModel", GetType(Protein))>
    Public Function GetSMARTLDM(path As String) As IEnumerable
        Return path.LoadCsv(Of Protein)
    End Function
End Module
