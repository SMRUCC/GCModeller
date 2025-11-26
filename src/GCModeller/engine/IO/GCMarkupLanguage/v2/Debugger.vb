#Region "Microsoft.VisualBasic::8a5029ad8e261abdb074e07fbc36ed37, engine\IO\GCMarkupLanguage\v2\Debugger.vb"

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

    '   Total Lines: 22
    '    Code Lines: 10 (45.45%)
    ' Comment Lines: 9 (40.91%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (13.64%)
    '     File Size: 676 B


    '     Module Debugger
    ' 
    '         Function: checkModel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Linq

Namespace v2

    ''' <summary>
    ''' Model file debugger
    ''' </summary>
    Public Module Debugger

        ''' <summary>
        ''' this function is mainly address at check the errors in the virtual cell component networking.
        ''' </summary>
        ''' <param name="vcell"></param>
        ''' <param name="log"></param>
        ''' <returns></returns>
        <Extension>
        Public Function checkModel(vcell As VirtualCell, log As LogFile) As LogFile
            Return log
        End Function

        <Extension>
        Public Function GetMetaboliteSymbolNames(vcell As VirtualCell) As Dictionary(Of String, String)
            Dim symbolNames As New Dictionary(Of String, String)

            For Each cpd As Compound In vcell.metabolismStructure.compounds
                symbolNames(cpd.ID) = cpd.name
            Next

            Return symbolNames
        End Function

        <Extension>
        Public Function GetMetaboliteReferenceMaps(vcell As VirtualCell) As Dictionary(Of String, String)
            Dim refs As New Dictionary(Of String, String)

            For Each cpd As Compound In vcell.metabolismStructure.compounds
                For Each ref_id As String In cpd.referenceIds.SafeQuery
                    If ref_id.StringEmpty Then
                        Continue For
                    End If

                    refs(ref_id) = cpd.ID
                Next
            Next

            Return refs
        End Function
    End Module
End Namespace
