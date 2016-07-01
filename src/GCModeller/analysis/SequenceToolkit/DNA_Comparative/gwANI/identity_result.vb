#Region "Microsoft.VisualBasic::fba3dbc23d36b81a06f1ae52e253790d, ..\GCModeller\analysis\SequenceToolkit\DNA_Comparative\gwANI\identity_result.vb"

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

    Public Class IdentityResult

        Public Property percentage_identity As Double
        Public Property num_matching_bases As Integer
        Public Property num_gaps As Integer
        Public Property length_without_gaps As Integer
        Public Property calculation_skipped As Boolean

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Module DefineConstants

        Public Const KS_SEP_SPACE As Integer = 0
        Public Const KS_SEP_TAB As Integer = 1
        Public Const KS_SEP_LINE As Integer = 2
        Public Const KS_SEP_MAX As Integer = 2
        Public Const DEFAULT_NUM_SAMPLES As Integer = 65536
        Public Const PROGRAM_NAME As String = "panito"
    End Module
End Namespace
