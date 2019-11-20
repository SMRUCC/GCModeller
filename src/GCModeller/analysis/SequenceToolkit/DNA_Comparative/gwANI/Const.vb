#Region "Microsoft.VisualBasic::33a5dcbd17f2c843b392fb49eea8b691, analysis\SequenceToolkit\DNA_Comparative\gwANI\Const.vb"

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

    '     Module DefineConstants
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

'
' *  Wellcome Trust Sanger Institute
' *  Copyright (C) 2016  Wellcome Trust Sanger Institute
' *  
' *  This program is free software; you can redistribute it and/or
' *  modify it under the terms of the GNU General Public License
' *  as published by the Free Software Foundation; either version 3
' *  of the License, or (at your option) any later version.
' *  
' *  This program is distributed in the hope that it will be useful,
' *  but WITHOUT ANY WARRANTY; without even the implied warranty of
' *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' *  GNU General Public License for more details.
' *  
' *  You should have received a copy of the GNU General Public License
' *  along with this program; if not, write to the Free Software
' *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
' 

Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace gwANI

    Public Module DefineConstants

        Public Const KS_SEP_SPACE As Integer = 0
        Public Const KS_SEP_TAB As Integer = 1
        Public Const KS_SEP_LINE As Integer = 2
        Public Const KS_SEP_MAX As Integer = 2
        Public Const DEFAULT_NUM_SAMPLES As Integer = 65536
        Public Const PROGRAM_NAME As String = "panito"
    End Module
End Namespace
