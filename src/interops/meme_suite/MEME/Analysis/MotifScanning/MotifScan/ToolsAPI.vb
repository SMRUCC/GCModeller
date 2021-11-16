#Region "Microsoft.VisualBasic::42e9a7494ad2cf968a3eaf4216ddc1e7, meme_suite\MEME\Analysis\MotifScanning\MotifScan\ToolsAPI.vb"

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

    '     Module MotifScansTools
    ' 
    '         Function: (+2 Overloads) CreateModel, LoadRegulations, MotifScan
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MAST.HTML

Namespace Analysis.MotifScans

    <Package("MotifScansTools", Publisher:="xie.guigang@gcmodeller.org")>
    Public Module MotifScansTools

        <ExportAPI("Read.Xml.Regulations")>
        Public Function LoadRegulations(path As String) As Regulations
            Return path.LoadXml(Of Regulations)
        End Function

        <ExportAPI("ScanModel.Create.From.LDM")>
        Public Function CreateModel(MotifLDM As AnnotationModel, Regulations As Regulations,
                                    Optional delta As Double = 80, Optional delta2 As Double = 70, Optional offSet As Integer = 5) As MotifScans
            Return New MotifScans(MotifLDM, Regulations, delta, delta2, offSet)
        End Function

        <ExportAPI("ScanModel.Create.From.LDM")>
        Public Function CreateModel(<Parameter("LDM.Xml", "The file path of the motif ldm xml file.")>
                                    MotifLDM As String,
                                    Regulations As Regulations,
                                    Optional delta As Double = 80, Optional delta2 As Double = 70, Optional offSet As Integer = 5) As MotifScans
            Dim LDM As AnnotationModel = MotifLDM.LoadXml(Of AnnotationModel)
            Return New MotifScans(LDM, Regulations, delta, delta2, offSet)
        End Function

        <ExportAPI("MotifScan")>
        Public Function MotifScan(LDM As MotifScans, Nt As SequenceModel.FASTA.FastaSeq) As MatchedSite()
            Return LDM.Mast(Nt)
        End Function
    End Module
End Namespace
