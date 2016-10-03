#Region "Microsoft.VisualBasic::106a6647a8a11ec072952f63757fd7d2, ..\GCModeller\shoalAPI\Genbank\PTTOperations.vb"

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

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.Extensions
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.DeviceDriver.DriverHandles
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat

<[PackageNamespace]("NCBI.Genbank.PTT", Publisher:="xie.guigang@gcmodeller.org")>
Module PTTOperations

    <ExportAPI("Get.Segment.Description", Info:="strand =0 Not Specific; =1 Forward; =-1 Reversed")>
    Public Function GetLocationDescription(PTT As PTTDbLoader, starts As Integer, ends As Integer, Optional strand As Integer = 0) As String
        Dim StrandData As Strands
        If strand > 0 Then
            StrandData = Strands.Forward
        ElseIf strand < 0 Then
            StrandData = Strands.Reverse
        Else
            StrandData = Strands.Unknown
        End If

        '  Dim Relation As LANS.SystemsBiology.ComponentModel.Loci. SegmentRelationships
        Dim Location = PTT.GetRelatedGenes(starts, ends, Strand:=StrandData)
        Dim describs As String() = Location.ToArray(Function(x) x.ToString)
        Return String.Join("; ", describs)
    End Function

    <ExportAPI("Loci.Gene_UpStream", Info:="Get a segment region loci on the upstream region from the gene ATG site with a specific distance length.")>
    Public Function GetUpStreamLoci(PTT As PTT, GeneID As String, dist As Integer) As NucleotideLocation
        Dim gene As ComponentModels.GeneBrief = PTT.GeneObject(GeneID)
        If gene Is Nothing Then
            Return Nothing
        Else
            Return gene.Location.GetUpStreamLoci(dist)
        End If
    End Function

    <InputDeviceHandle("PTT", "")>
    <ExportAPI("Read.Txt.PTT")>
    Public Function ReadPtt(path As String) As PTT
        Return PTT.Load(path)
    End Function
End Module
