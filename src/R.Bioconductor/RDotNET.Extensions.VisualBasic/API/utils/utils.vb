#Region "Microsoft.VisualBasic::7f8c7753859faffef4dc94973493f3f7, RDotNET.Extensions.VisualBasic\API\utils\utils.vb"

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

    '     Module utils
    ' 
    '         Function: (+2 Overloads) data, memory_size
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace API.utils

    Public Module utils

        ''' <summary>
        ''' Loads specified data sets, or list the available data sets.
        ''' </summary>
        ''' <param name="x">literal character strings or names.</param>
        ''' <param name="list">a character vector.</param>
        ''' <param name="package">a character vector giving the package(s) to look in for data sets, or NULL.
        '''
        ''' By Default, all packages in the search path are used, then the 'data’ subdirectory (if present) 
        ''' of the current working directory.</param>
        ''' <param name="libloc">
        ''' a character vector of directory names of R libraries, or NULL. The default value of NULL corresponds 
        ''' to all libraries currently known.
        ''' </param>
        ''' <param name="verbose">a logical. If TRUE, additional diagnostics are printed.</param>
        ''' <param name="envir">the environment where the data should be loaded.</param>
        ''' <returns>
        ''' A character vector of all data sets specified, or information about all available data sets in an object of class "packageIQR" if none were specified.
        ''' </returns>
        ''' <remarks>
        ''' Currently, four formats of data files are supported:
        '''
        ''' + files ending '.R’ or ‘.r’ are source()d in, with the R working directory changed temporarily to the directory containing the respective file. (data ensures that the utils package is attached, in case it had been run via utils::data.)
        ''' + files ending '.RData’ or ‘.rda’ are load()ed.
        ''' + files ending '.tab’, ‘.txt’ or ‘.TXT’ are read using read.table(..., header = TRUE), and hence result in a data frame.
        ''' + files ending '.csv’ or ‘.CSV’ are read using read.table(..., header = TRUE, sep = ";"), and also result in a data frame.
        ''' 
        ''' If more than one matching file name Is found, the first On this list Is used. (Files With extensions '.txt’, ‘.tab’ or ‘.csv’ can be compressed, with or without further extension ‘.gz’, ‘.bz2’ or ‘.xz’.)
        ''' The data sets To be loaded can be specified As a Set Of character strings Or names, Or As the character vector list, Or As both.
        ''' For Each given data set, the first two types ('.R’ or ‘.r’, and ‘.RData’ or ‘.rda’ files) can create several variables in the load environment, which might all be named differently from the data set. The third and fourth types will always result in the creation of a single variable with the same name (without extension) as the data set.
        ''' 
        ''' + If no data sets are specified, data lists the available data sets. It looks For a New-style data index In the 'Meta’ or, if this is not found, an old-style ‘00Index’ file in the ‘data’ directory of each specified package, and uses these files to prepare a listing. If there is a ‘data’ area but no index, available data files for loading are computed and included in the listing, and a warning is given: such packages are incomplete. The information about available data sets is returned in an object of class "packageIQR". The structure of this class is experimental. Where the datasets have a different name from the argument that should be used to retrieve them the index will have an entry like beaver1 (beavers) which tells us that dataset beaver1 can be retrieved by the call data(beaver).
        ''' + If ``lib.loc`` And package are both NULL (the default), the data sets are searched for in all the currently loaded packages Then In the 'data’ directory (if any) of the current working directory.
        ''' + If ``lib.loc = NULL`` but package Is specified as a character vector, the specified package(s) are searched for first amongst loaded packages And Then In the Default library/ies (see .libPaths).
        ''' + If ``lib.loc`` Is specified (And Not NULL), packages are searched for in the specified library/ies, even if they are already loaded from another library.
        ''' 
        ''' To just look in the 'data’ directory of the current working directory, set package = character(0) (and lib.loc = NULL, the default).
        ''' </remarks>
        Public Function data(x As IEnumerable(Of String),
                             Optional list As String = "character()",
                             Optional package As String = NULL,
                             Optional libloc As String = NULL,
                             Optional verbose As String = "getOption(""verbose"")",
                             Optional envir As String = ".GlobalEnv") As String()
            Dim objs As String = x.JoinBy(", ")
            Dim Rscript As String = $"data({objs},list={list},package={package},lib.loc={libloc},verbos={verbose},envir={envir})"
            Dim out As String() = R.WriteLine(Rscript)
            Return out
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        ''' <remarks>Simplify version of <see cref="data"/></remarks>
        Public Function data(x As String) As String
            Return data({x}).FirstOrDefault
        End Function

        ''' <summary>
        ''' ### Report on Memory Allocation
        ''' 
        ''' reports the current or maximum memory allocation of the malloc function used in this version of R.
        ''' </summary>
        ''' <param name="max">
        ''' logical. If TRUE the maximum amount of memory obtained from the OS Is reported, 
        ''' if FALSE the amount currently in use, if NA the memory limit.
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' These functions exist on other platforms but always report infinity as R does itself provide limits 
        ''' on memory allocation—the OS's own facilities can be used.
        ''' </remarks>
        Public Function memory_size(Optional max As Boolean = False) As Double
            SyncLock R
                With R
                    Return .Evaluate($"memory.size(max={max.λ})") _
                           .AsNumeric _
                           .First
                End With
            End SyncLock
        End Function
    End Module
End Namespace
