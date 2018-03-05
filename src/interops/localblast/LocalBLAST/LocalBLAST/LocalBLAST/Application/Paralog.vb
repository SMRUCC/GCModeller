#Region "Microsoft.VisualBasic::c273ca4d9bcc1c0abe00da1a0329fa00, localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\Paralog.vb"

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

    '     Module Paralog
    ' 
    '         Function: ExportParalog
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

Namespace LocalBLAST.Application

    Public Module Paralog

        ''' <summary>
        ''' Exports the blastp paralog hits.
        ''' </summary>
        ''' <param name="blastp"></param>
        ''' <param name="coverage"></param>
        ''' <param name="identities"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ExportParalog(blastp As v228, Optional coverage As Double = 0.5, Optional identities As Double = 0.3) As BestHit()
            Dim source As IEnumerable(Of BestHit) = blastp.ExportAllBestHist(coverage, identities)
            Dim LQuery = (From x As BestHit
                          In source.AsParallel
                          Where Not String.Equals(x.QueryName, x.HitName)
                          Select x).ToArray
            Return LQuery
        End Function
    End Module
End Namespace
